using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Models;
using Anglian.Classes;
using Xamarin.Forms;
using Anglian.Views;
using Anglian.Service;
namespace Anglian.Engine
{
    public class Syncing
    {
        private DataAccess m_cData = new DataAccess();

        /// <summary>
        /// Settings class.
        /// </summary>
        private Settings m_cSetting = new Settings();


        /// <summary>
        /// v1.0.1 - Flag to indicate if to sync changes only.
        /// </summary>
        public static bool p_bSyncChangesOnly = false;

        /// <summary>
        /// v1.0.1 - List of projects to sync
        /// </summary>
        public static ObservableCollection<ProjectSearch> p_ocProjectsToSync = null;
        public Syncing()
        {

            try
            {

                //this.m_cSetting.DisplayMessage += m_cSetting_DisplayMessage;

                m_cData.CheckDB();

            }
            catch (Exception ex)
            {


            }

        }
        private void UpdateMessage(string sSyncMsg)
        {
            if (Application.Current.MainPage.Navigation.NavigationStack.Count > 0)
            {
                int index = Application.Current.MainPage.Navigation.NavigationStack.Count - 1;
                Page currPage = Application.Current.MainPage.Navigation.NavigationStack[index];
                if (currPage.GetType() == typeof(ProjectSyncPage))
                {
                    //currPage.BackgroundColor = Color.Red;
                    Label vLastSyncDateTime = currPage.FindByName<Label>("tbSyncStatus");
                    vLastSyncDateTime.Text = sSyncMsg;
                }
            }
        }
        /// <summary>
        /// Upload changes to AX.
        /// </summary>
        public async Task<bool> UploadChanges(bool v_bBackgroundTask)
        {

            bool bConnected = false;
            WcfExt116 cAX = null;
            bool bErrorOccurred = false;
            bool bReturnStatus = true;

            //Create changes upload object for passing to WCF.
            ObservableCollection<AXDataUploadDataChange> cChanges = new ObservableCollection<AXDataUploadDataChange>();

            //v1.0.1 - OC for new notes.
            ObservableCollection<NoteDetails> cNotes = new ObservableCollection<NoteDetails>();

            //v1.0.1 - Processing new notes.
            List<cProjectNotesTable> pntNotes = new List<cProjectNotesTable>();

            ///v1.0.1
            NoteDetails ndNote;

            //
            AXDataUploadDataChange cChange = new AXDataUploadDataChange();

            UploadChangesResult uUploadResult = null;

            string sPurpose = Settings.ReturnPurposeType();

            int iSubProjectCount = 0;

            //v1.0.10 - Unit change upload variables.
            ObservableCollection<UnitDetails> ocUnitDetails = new ObservableCollection<UnitDetails>();
            UnitDetails udUnitDetail;
            DateTime dInstallationDate;
            string sInstallationTeam;

            List<cUnitsUpdateTable> cUnitUpdates;

            bool bUploadOK = false;


            try
            {

                if (v_bBackgroundTask == false)
                {

                    //Update screen.
                    if (Application.Current.MainPage.Navigation.NavigationStack.Count > 0)
                        this.UpdateMessage("Checking connection..");


                }


                //Check we are connected first
                bConnected = await this.m_cSetting.IsAXSystemAvailable(false);
                if (bConnected == false)
                {

                    if (v_bBackgroundTask == false)
                    {

                        //Update screen.
                        this.UpdateMessage("No connection...");

                    }

                    return false;
                }

                List<cUpdatesTable> cSubProjectUpdates = null;

                //string sUserName = await DependencyService.Get<ISettings>().GetUserName();
                //string sMachineName = DependencyService.Get<ISettings>().GetMachineName();
                //string sUserFullName = await DependencyService.Get<ISettings>().GetUserDisplayName();
                string sUserName = Session.CurrentUserName;
                string sMachineName = Settings.GetMachineName();
                string sUserFullName = Session.CurrentUserName;

                //Create new instance of 
                cAX = new WcfExt116();


                //Fetch list of sub project with updates to upload

                //Update screen.
                if (v_bBackgroundTask == false) { this.UpdateMessage("Retrieving sub project with pending data uploads..."); };

                List<SubProjectSync> cSubProjects = this.m_cData.FetchSubProjectsWithUploads();
                if (cSubProjects.Count > 0)
                {

                    //Loop through sub project and try and upload the changes for each.
                    foreach (SubProjectSync cSubProject in cSubProjects)
                    {

                        iSubProjectCount += 1;

                        bErrorOccurred = false;

                        //Update screen.
                        if (v_bBackgroundTask == false) { this.UpdateMessage("Uploading Sub project: (" + iSubProjectCount.ToString() + " of " + cSubProjects.Count.ToString() + ") - " + cSubProject.SubProjectNo); };

                        try
                        {

                            if (cSubProject.UpdateQty > 0)
                            {

                                //Clear out existing update.
                                cChanges.Clear();

                                //Fetch list of pending updates.
                                cSubProjectUpdates = this.m_cData.ReturnPendingUpdatesForSubProject(cSubProject.SubProjectNo);

                                //Add update to upload object.
                                foreach (cUpdatesTable cUpdate in cSubProjectUpdates)
                                {
                                    cChange = new AXDataUploadDataChange();
                                    cChange.ProjectNo = cUpdate.SubProjectNo;
                                    cChange.FieldName = cUpdate.FieldName;
                                    cChange.FieldValue = cUpdate.FieldValue;

                                    cChanges.Add(cChange);

                                }

                                //Pass update to AX.
                                uUploadResult = await cAX.UploadSubProjectChanges(sUserName, sMachineName, cSubProject.SubProjectNo, cChanges);

                                //If OK then delete updates from local updates table.
                                if (uUploadResult != null && uUploadResult.bSuccessfull == true)
                                {

                                    //Remove changes from upload table.
                                    this.m_cData.RemoveChangesFromUploadTable(cSubProjectUpdates);

                                    //v1.0.10 - Do not update sub project dates as can stop latest data from syncing.
                                    //Update sub project modified dates.
                                    //this.m_cData.UpdateSubProjectUpdateDates(cSubProject.SubProjectNo, uUploadResult.ProjTable_ModDate, uUploadResult.ActivitiesTable_ModDate);

                                }
                                else
                                {

                                    //Update screen.
                                    if (v_bBackgroundTask == false) { this.UpdateMessage("Upload failed for sub project: " + cSubProject.SubProjectNo); };

                                    bErrorOccurred = true;
                                }

                            }

                            if (cSubProject.UnitUpdateQty > 0)
                            {

                                //v1.0.10 - Upload unit status changes if running contract manager mode.
                                if (DependencyService.Get<ISettings>().IsThisTheSurveyorApp() == false)
                                {

                                    //Fetch pending updates for unit.                                   
                                    cUnitUpdates = this.m_cData.FetchUnitUpdatesForSubProject(cSubProject.SubProjectNo);
                                    if (cUnitUpdates.Count > 0)
                                    {

                                        //Fetch the most recent update date.
                                        dInstallationDate = this.m_cData.FetchMostRecentUnitInstallDateForSubProject(cSubProject.SubProjectNo);

                                        //Fetch installation team.
                                        sInstallationTeam = this.m_cData.FetchInstallationTeamForSubProject(cSubProject.SubProjectNo);

                                        //Make sure the collection is clean
                                        if (ocUnitDetails != null)
                                        {
                                            ocUnitDetails.Clear();
                                        }
                                        else
                                        {
                                            ocUnitDetails = new ObservableCollection<UnitDetails>();
                                        }

                                        //Update observable collection with unit updates.
                                        foreach (cUnitsUpdateTable cUpdate in cUnitUpdates)
                                        {

                                            udUnitDetail = new UnitDetails();
                                            udUnitDetail.dInstalledDate = dInstallationDate;
                                            udUnitDetail.iInstalledStatus = cUpdate.InstalledStatus;
                                            udUnitDetail.iUNITNUMBER = cUpdate.UnitNo;

                                            ocUnitDetails.Add(udUnitDetail);

                                        }

                                        //Pass up.
                                        bUploadOK = await cAX.UploadUnitStatusChanges(cSubProject.SubProjectNo, sUserName, sMachineName, dInstallationDate, sInstallationTeam, ocUnitDetails);

                                        //If OK then remove updates from table.
                                        if (bUploadOK == true)
                                        {

                                            this.m_cData.DeleteUnitUpdates(cSubProject.SubProjectNo);

                                        }
                                        else
                                        {

                                            //Update screen.
                                            if (v_bBackgroundTask == false) { this.UpdateMessage("Upload failed for sub project: " + cSubProject.SubProjectNo); };

                                            bErrorOccurred = true;

                                        }

                                    }

                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                            bErrorOccurred = true;
                        }

                        //If error occurred check connection is OK, if not leave.
                        if (bErrorOccurred == true)
                        {

                            //Update screen.
                            if (v_bBackgroundTask == false) { this.UpdateMessage("Checking connection."); };

                            //Check we are connected first
                            bConnected = await this.m_cSetting.IsAXSystemAvailable(false);
                            if (bConnected == false)
                            {

                                //Update screen.
                                this.UpdateMessage("Checking Failed.");

                                bReturnStatus = false;
                                break;
                            }

                        }


                    }


                }

                //** Upload notes to AX **

                //Update screen.
                if (v_bBackgroundTask == false) { this.UpdateMessage("Fetching sub projects with new notes"); };

                iSubProjectCount = 0;

                cSubProjects = this.m_cData.FetchSubProjectsWithNewNotes();
                if (cSubProjects != null)
                {

                    //Loop through sub project and try and upload the changes for each.
                    foreach (SubProjectSync cSubProject in cSubProjects)
                    {

                        iSubProjectCount += 1;

                        bErrorOccurred = false;

                        //Update screen.
                        if (v_bBackgroundTask == false) { this.UpdateMessage("Uploading notes for Sub project: (" + iSubProjectCount.ToString() + " of " + cSubProjects.Count.ToString() + ") - " + cSubProject.SubProjectNo); };


                        try
                        {

                            cNotes.Clear();

                            pntNotes = this.m_cData.FetchNewNotes(cSubProject.SubProjectNo);
                            foreach (cProjectNotesTable cNote in pntNotes)
                            {
                                ndNote = new NoteDetails();
                                ndNote.AXRecID = cNote.AXRecID;
                                ndNote.DeviceIDKey = cNote.IDKey;
                                ndNote.InputDate = cNote.InputDateTime;
                                ndNote.NoteText = cNote.NoteText;
                                ndNote.NoteType = cNote.NoteType;
                                ndNote.ProjectNo = cNote.SubProjectNo;
                                ndNote.Purpose = sPurpose;
                                ndNote.UserName = sUserFullName;
                                ndNote.UserProfile = sUserName;

                                cNotes.Add(ndNote);

                            }

                            //Pass update to AX.
                            uUploadResult = await cAX.UploadSubProjectNotes(sUserName, sMachineName, cSubProject.SubProjectNo, cNotes);

                            //If OK then delete updates from local updates table.
                            if (uUploadResult != null && uUploadResult.bSuccessfull == true)
                            {

                                this.m_cData.UpdateNotesWithRecID(uUploadResult.NoteValues);


                            }
                            else
                            {

                                //Update screen.
                                if (v_bBackgroundTask == false) { this.UpdateMessage("Notes upload failed for sub project: " + cSubProject.SubProjectNo); };

                                bErrorOccurred = true;
                            }

                        }
                        catch (Exception ex)
                        {
                            //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                            bReturnStatus = true;
                        }

                    }

                }


                if (cAX != null)
                {
                    await DependencyService.Get<IWcfExt116>().CloseAXConnection();
                }

                return bReturnStatus;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }
        /// <summary>
        /// Check for data changes to download.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckForDataChanges(bool v_bBackgroundTask)
        {

            bool bSaveOK = false;
            DataAccess.SaveSubProjectDataResult srResult;
            bool bConnected = false;
            bool bErrorOccurred = false;
            bool bReturnStatus = true;

            WcfExt116 cAX = null;
            //cDataAccess cDataDB = null;

            ObservableCollection<DownloadDataChange> cSubProjects = null;
            DownloadDataChange cSubProject;
            List<SubProjectSyncUpdateValues> cSubProjectDates = null;
            ObservableCollection<SubProjectData> cSubProjectsChanged = null;

            //v1.0.10 - Data change result object.
            DownloadDataChangesResult dcResult = null;

            string sProjectStatus = string.Empty;
            string sSubProjectStatus = string.Empty;

            int iSubProjectLimit = -1;
            bool bCheckForNewSubProjects = true;

            int iSubProjectsAdded = 0;
            int iSubProjectsDeleted = 0;

            List<cProjectNotesTable> cNotes = null;
            RealtimeNoteKeyValues cNoteKey;
            List<cUnitsTable> cUnits = null;
            UnitDetails udUnitDetail;

            try
            {

                //If called from background task then go off and fetch projects to sync
                if (v_bBackgroundTask == true)
                {

                    iSubProjectLimit = 2;
                    bCheckForNewSubProjects = false;

                    Syncing.p_ocProjectsToSync = this.m_cData.FetchProjectsToSync();

                }

                //Update screen.
                if (v_bBackgroundTask == false)
                {

                    this.UpdateMessage("Checking connection.");


                    //Check we are connected first
                    bConnected = await this.m_cSetting.IsAXSystemAvailable(false);
                    if (bConnected == false)
                    {

                        //Update screen.
                        if (v_bBackgroundTask == false) { this.UpdateMessage("Checking failed."); };

                        return false;
                    }


                }


                if (Syncing.p_ocProjectsToSync != null)
                {

                    int iProjectCount = 0;
                    int iSubProjectCount = 0;

                    cAX = new WcfExt116();

                    string sLastProjectNo = String.Empty;

                    foreach (ProjectSearch cProject in Syncing.p_ocProjectsToSync)
                    {

                        bErrorOccurred = false;
                        iProjectCount += 1;

                        //Update screen.
                        if (v_bBackgroundTask == false)
                        {

                            sProjectStatus = "Checking project (" + iProjectCount.ToString() + " of " + Syncing.p_ocProjectsToSync.Count.ToString() + ") " + cProject.ProjectNo;

                            this.UpdateMessage(sProjectStatus);

                            //this.SubProjectStatusUpdate(this, new cSyncEventParamProjectStatus(sLastProjectNo, cProject.ProjectNo, iSubProjectsAdded, iSubProjectsDeleted, true));

                        }


                        iSubProjectsAdded = 0;
                        iSubProjectsDeleted = 0;

                        try
                        {


                            cSubProjects = new ObservableCollection<DownloadDataChange>();

                            cSubProjectDates = this.m_cData.FetchSubProjectUpdateDateTime(cProject.ProjectNo, iSubProjectLimit);
                            foreach (SubProjectSyncUpdateValues cSubProjectValue in cSubProjectDates)
                            {


                                cSubProject = new DownloadDataChange();
                                cSubProject.sProjectNo = cProject.ProjectNo;
                                cSubProject.sSubProjectNo = cSubProjectValue.SubProjectNo;

                                cSubProject.Notes = new ObservableCollection<RealtimeNoteKeyValues>();

                                if (cSubProjectValue.ModifiedDateTime.HasValue == true)
                                {
                                    cSubProject.ProjTable_ModDate = cSubProjectValue.ModifiedDateTime.Value;
                                }

                                if (cSubProjectValue.SMMActivities_MODIFIEDDATETIME.HasValue == true)
                                {
                                    cSubProject.ActivitiesTable_ModDate = cSubProjectValue.SMMActivities_MODIFIEDDATETIME.Value;
                                }

                                //v1.0.12 - Delivery modified date time
                                if (cSubProjectValue.Delivery_ModifiedDateTime.HasValue == true)
                                {
                                    cSubProject.Delivery_ModDate = cSubProjectValue.Delivery_ModifiedDateTime.Value;

                                }


                                //Add notes to upload.
                                cNotes = this.m_cData.GetSubProjectNotesData(cSubProjectValue.SubProjectNo);
                                foreach (cProjectNotesTable cProjNote in cNotes)
                                {
                                    cNoteKey = new RealtimeNoteKeyValues();
                                    cNoteKey.DeviceIDKey = cProjNote.IDKey;
                                    cNoteKey.NotesRecID = cProjNote.AXRecID;
                                    cNoteKey.ProjectNo = cSubProjectValue.SubProjectNo;

                                    cSubProject.Notes.Add(cNoteKey);

                                }

                                //Units are only used for the installers app.
                                if (DependencyService.Get<ISettings>().IsThisTheSurveyorApp() == false)
                                {
                                    cSubProject.Units = new ObservableCollection<UnitDetails>();

                                    //Add units to upload.
                                    cUnits = this.m_cData.FetchUnitsForSubProject(cSubProject.sSubProjectNo);
                                    foreach (cUnitsTable cUnitRow in cUnits)
                                    {

                                        udUnitDetail = new UnitDetails();
                                        udUnitDetail.iInstalledStatus = cUnitRow.InstalledStatus;
                                        udUnitDetail.iUNITNUMBER = cUnitRow.UnitNo;
                                        udUnitDetail.sITEMID = cUnitRow.ItemID;
                                        udUnitDetail.sSTYLE = cUnitRow.Style;
                                        udUnitDetail.sUNITLOCATION = cUnitRow.UnitLocation;

                                        cSubProject.Units.Add(udUnitDetail);

                                    }

                                }

                                cSubProjects.Add(cSubProject);

                            }


                            //v1.0.10 - Retry on failed attempts.
                            int iAttempts = 0;
                            do
                            {

                                try
                                {

                                    dcResult = null;
                                    dcResult = await cAX.CheckForAXDataChanges(cSubProjects, bCheckForNewSubProjects);
                                    if (dcResult != null)
                                    {

                                        if (dcResult.bSuccessfull == true)
                                        {

                                            cSubProjectsChanged = dcResult.pdChanged;


                                        }

                                    }
                                    if (cSubProjectsChanged != null)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        throw new Exception("CheckForAXDataChanges returned null");
                                    }

                                }
                                catch (Exception ex)
                                {

                                    bErrorOccurred = true;

                                    iAttempts++;
                                    if (v_bBackgroundTask == false) {
                                        //this.ProjectSyncError(this, new cSyncEventErrorParams(ex.Message, cProject.ProjectNo)); 
                                    };

                                }

                            } while (iAttempts < 3); //Try 3 times.


                            //Sub projects to update.
                            if (cSubProjectsChanged != null)
                            {

                                foreach (SubProjectData cData in cSubProjectsChanged)
                                {

                                    srResult = this.m_cData.SaveSubProjectData(cProject.ProjectNo, cProject.ProjectName, cData);
                                    bSaveOK = srResult.bSavedOK;

                                    //If sub project added then increase count.
                                    if (bSaveOK == true)
                                    {
                                        if (srResult.bProjectAdded == true)
                                        {
                                            iSubProjectsAdded += 1;
                                        }
                                    }


                                }

                                //v1.0.18 - No need to update these, takes up too much time.
                                //v1.0.1 - Update the sub projects passed up to say they have synced.
                                //cProjectTable cSubProjectSync;
                                //foreach (wcfAX.DownloadDataChange ddcSub in cSubProjects)
                                //{

                                //    cSubProjectSync = this.m_cData.GetSubProjectProjectData(ddcSub.sSubProjectNo);
                                //    cSubProjectSync.DateLastSynced = DateTime.Now;
                                //    bSaveOK = this.m_cData.UpdateSubProjectData(cSubProjectSync);

                                //}        


                                //v1.0.10  - Delete list of sub projects returned.
                                if (dcResult.sDeleted != null)
                                {
                                    //Loop through list of sub project to delete
                                    foreach (string sSubProjectToDelete in dcResult.sDeleted)
                                    {

                                        await this.m_cData.DeleteSubProjectFromDevice(sSubProjectToDelete);
                                        iSubProjectsDeleted++;

                                    }


                                }

                            }
                            else
                            {
                                bErrorOccurred = true;
                            }

                            iSubProjectCount = 0;

                            //v1.0.1 - Checking files
                            foreach (SubProjectSyncUpdateValues cSubProjectValue in cSubProjectDates)
                            {

                                iSubProjectCount += 1;
                                sSubProjectStatus = "Checking files for sub project (" + iSubProjectCount.ToString() + " of " + cSubProjectDates.Count.ToString() + ")";

                                //Update screen.
                                if (v_bBackgroundTask == false) { this.UpdateMessage(sProjectStatus + " - " + sSubProjectStatus); };

                                bSaveOK = await this.CheckForFileChanges(cProject.ProjectNo, cSubProjectValue.SubProjectNo, v_bBackgroundTask);
                                if (bSaveOK == false)
                                {
                                    throw new Exception("Error checking files for sub project (" + cSubProjectValue.SubProjectNo + ")");
                                }

                            }


                        }
                        catch (Exception ex)
                        {

                            if (v_bBackgroundTask == false) {
                                //this.ProjectSyncError(this, new cSyncEventErrorParams(ex.Message, cProject.ProjectNo));
                            };

                            bErrorOccurred = true;
                        }

                        //If error occurred check connection is OK, if not leave.
                        if (bErrorOccurred == true)
                        {

                            //Update screen.
                            if (v_bBackgroundTask == false) { this.UpdateMessage("Checking Connection."); };

                            //Check we are connected first
                            bConnected = await this.m_cSetting.IsAXSystemAvailable(false);
                            if (bConnected == false)
                            {

                                //Update screen.
                                if (v_bBackgroundTask == false) { this.UpdateMessage("Connection Failed."); };

                                bReturnStatus = false;
                                break;
                            }

                        }

                        //Update last project no
                        sLastProjectNo = cProject.ProjectNo;

                    }

                    if (v_bBackgroundTask == false)
                    {

                        bool bUpdateSuccess = false;
                        if (bErrorOccurred == false)
                        {
                            bUpdateSuccess = true;
                        }

                        //this.SubProjectStatusUpdate(this, new cSyncEventParamProjectStatus(sLastProjectNo, string.Empty, iSubProjectsAdded, iSubProjectsDeleted, bUpdateSuccess));

                    }

                    if (cAX != null)
                    {
                        await DependencyService.Get<IWcfExt116>().CloseAXConnection();
                    }

                }

                return bReturnStatus;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }
        /// <summary>
        /// v1.0.1 - Check for file changes.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CheckForFileChanges(string v_sProjectNo, string v_sSubProjectNo, bool v_bBackgroundTask)
        {

            bool bSaveOK = false;
            bool bConnected = false;
            bool bErrorOccurred = false;
            bool bReturnStatus = true;

            WcfExt116 cAX = null;

            //ObservableCollection<wcfAX.DownloadDataChange> cSubProjects = null;
            SubProjectFileDetail cProjectFile;
            List<cProjectFilesTable> cSubProjectFiles = null;
            //ObservableCollection<wcfAX.SubProjectData> cSubProjectsChanged = null;
            List<SubProjectSync> cSubProjectsSync;
            SubProjectFile sfSubProject;
            FileChangesResult sfrResult;
            ObservableCollection<SubProjectFileDetail> cProjectFileDetails = null;

            SubProjectFileDownloadResult sfdResult;

            object sfSubProjectFolder = null;// Origin was storageFolder

            try
            {

                ////Update screen.
                //cMain.UpdateSyncMainPage("Checking connection.");

                ////Check we are connected first
                //bConnected = await cMain.IsAXSystemAvailable(false);
                //if (bConnected == false)
                //{

                //    //Update screen.
                //    cMain.UpdateSyncMainPage("Checking failed.");

                //    return false;
                //}


                try
                {

                    cAX = new WcfExt116();

                    cSubProjectFiles = this.m_cData.FetchSubProjectFilesForChecking(v_sSubProjectNo);

                    cProjectFileDetails = new ObservableCollection<SubProjectFileDetail>();

                    if (cSubProjectFiles.Count > 0)
                    {

                        cProjectFile = new SubProjectFileDetail();
                        cProjectFile.sProjectNo = v_sProjectNo;
                        cProjectFile.sSubProjectNo = v_sSubProjectNo;
                        cProjectFile.sfFiles = new ObservableCollection<SubProjectFile>();

                        foreach (cProjectFilesTable cSubProjectFile in cSubProjectFiles)
                        {
                            sfSubProject = new SubProjectFile();
                            sfSubProject.FileName = cSubProjectFile.FileName;
                            sfSubProject.Comments = cSubProjectFile.NoteText;
                            sfSubProject.ModifiedDate = cSubProjectFile.ModDateTime;

                            cProjectFile.sfFiles.Add(sfSubProject);

                        }

                        cProjectFileDetails.Add(cProjectFile);

                    }

                    cProjectFileDetails = await cAX.CheckForAXFileChanges(v_sProjectNo, v_sSubProjectNo, cProjectFileDetails);
                    if (cProjectFileDetails != null)
                    {


                        foreach (SubProjectFileDetail sfdFile in cProjectFileDetails)
                        {


                            sfSubProjectFolder = await DependencyService.Get<ISettings>().ReturnSubProjectImagesFolder(sfdFile.sSubProjectNo);

                            if (sfdFile.sfFiles != null)
                            {

                                int iFileCount = 0;

                                foreach (SubProjectFile spfFile in sfdFile.sfFiles)
                                {

                                    iFileCount += 1;

                                    //Update screen.
                                    if (v_bBackgroundTask == false) { this.UpdateMessage("Downloading file (" + iFileCount.ToString() + " of " + sfdFile.sfFiles.Count.ToString() + ") - Sub Project (" + v_sSubProjectNo + ")"); };


                                    sfdResult = await cAX.ReturnFileData(spfFile.FileName);


                                    //Save file to device.
                                    bool bFileSaved = await DependencyService.Get<ISettings>().SaveFileLocally(sfSubProjectFolder, sfdResult.byFileData, spfFile.FileName);
                                    if (bFileSaved == false)
                                    {
                                        throw new Exception("Unable to save file to device (" + spfFile.FileName + ")");
                                    }


                                    //Save file record into files table.
                                    bool bSavesOK = this.m_cData.SaveSubProjectFile(sfdFile.sSubProjectNo, spfFile.FileName, spfFile.Comments, spfFile.ModifiedDate, false);
                                    if (bSavesOK == false)
                                    {
                                        throw new Exception("Unable to save file details to database (" + spfFile.FileName + ")");

                                    }
                                }

                            }

                        }

                    }




                }
                catch (Exception ex)
                {
                    //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                    bErrorOccurred = true;
                }

                //If error occurred check connection is OK, if not leave.
                if (bErrorOccurred == true)
                {

                    //Update screen.
                    if (v_bBackgroundTask == false) { this.UpdateMessage("Checking Connection."); };

                    //Check we are connected first
                    bConnected = await this.m_cSetting.IsAXSystemAvailable(false);
                    if (bConnected == false)
                    {

                        //Update screen.
                        if (v_bBackgroundTask == false) { this.UpdateMessage("Connection Failed."); };

                        bReturnStatus = false;

                    }

                }

                if (cAX != null)
                {
                    await DependencyService.Get<IWcfExt116>().CloseAXConnection();
                }

                return bReturnStatus;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }
        /// <summary>
        /// Upload photos to AX.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UploadPhotos()
        {

            WcfExt116 cAX = null;
            UploadFileChange cFileUpload;
            //StorageFile sfFile = null;
            object sfFile = null;
            //bool bUploadOK = false;
            bool bSaveOK = false;
            bool bConnected = false;
            bool bErrorOccurred = false;
            bool bReturnStatus = true;

            try
            {


                string sUserName = Session.CurrentUserName;
                string sMachineName = Settings.GetMachineName(); //DependencyService.Get<ISettings>().GetMachineName();

                //Update screen.
                this.UpdateMessage("Checking connection.");

                //Check we are connected first
                bConnected = await this.m_cSetting.IsAXSystemAvailable(false);
                if (bConnected == false)
                {

                    //Update screen.
                    this.UpdateMessage("Checking failed.");

                    return false;
                }

                //Fetch list of files waiting to be uploaded.

                //Update screen.
                this.UpdateMessage("Checking for files to uploading.");

                List<cProjectFilesTable> cFiles = this.m_cData.ReturnNewFilesForUploading();
                if (cFiles != null)
                {

                    cAX = new WcfExt116();

                    int iFileCount = 0;
                    UploadChangesResult ufResult = null;


                    foreach (cProjectFilesTable cFile in cFiles)
                    {

                        iFileCount += 1;
                        bSaveOK = false;

                        //Update screen.
                        this.UpdateMessage("Uploading file (" + iFileCount.ToString() + " of " + cFiles.Count.ToString() + ") Sub project: " + cFile.SubProjectNo);

                        bErrorOccurred = false;

                        try
                        {

                            cFileUpload = new UploadFileChange();
                            cFileUpload.sComment = cFile.NoteText;
                            cFileUpload.sFileName = cFile.FileName;

                            sfFile = await DependencyService.Get<ISettings>().ReturnStorageFileForSubProject(cFile.SubProjectNo, cFile.FileName);
                            if (sfFile != null)
                            {
                                cFileUpload.byData = await DependencyService.Get<ISettings>().ConvertFileToByteArray(sfFile);

                                if (cFileUpload.byData != null)
                                {


                                    ufResult = await cAX.UploadFile(cFile.SubProjectNo, sUserName, sMachineName, cFileUpload);
                                    if (ufResult != null)
                                    {
                                        if (ufResult.bSuccessfull == true)
                                        {
                                            //v1.0.6 - Update local mod date.
                                            cFile.ModDateTime = ufResult.ProjTable_ModDate;
                                            bSaveOK = this.m_cData.UpdateFileAsNotNew(cFile);
                                        }


                                    }

                                    //v1.0.6 - If not saved ok, report as error.
                                    if (bSaveOK == false)
                                    {

                                        //Update screen.
                                        this.UpdateMessage("File upload failed.");
                                        bErrorOccurred = true;
                                    }

                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                            bErrorOccurred = true;

                        }

                        //If error occurred check connection is OK, if not leave.
                        if (bErrorOccurred == true)
                        {

                            //Update screen.
                            this.UpdateMessage("Checking Connection.");

                            //Check we are connected first
                            bConnected = await this.m_cSetting.IsAXSystemAvailable(false);
                            if (bConnected == false)
                            {

                                //Update screen.
                                this.UpdateMessage("Connection Failed.");

                                bReturnStatus = false;
                                break;
                            }

                        }

                    }

                    if (cAX != null)
                    {
                        await DependencyService.Get<IWcfExt116>().CloseAXConnection();
                    }

                }


                return bReturnStatus;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

    }
}
