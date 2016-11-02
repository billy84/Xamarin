using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANG_ABP_SURVEYOR_APP_CLASS.Model;
using ANG_ABP_SURVEYOR_APP_CLASS.wcfCalls;
using System.Collections.ObjectModel;
using Windows.Storage;
using ANG_ABP_SURVEYOR_APP_CLASS.Syncing;
using ANG_ABP_SURVEYOR_APP_CLASS.Classes;
using ANG_ABP_SURVEYOR_APP_CLASS;

namespace ANG_ABP_SURVEYOR_APP_CLASS.Syncing
{
    public class cSyncing
    {

        /// <summary>
        /// Event handler for displaying messages
        /// </summary>
        public event EventHandler<string> DisplayMessage;

        /// <summary>
        /// Event handler for displaying update messages
        /// </summary>
        public event EventHandler<cSyncEventParam> UpdateMessage;

        /// <summary>
        /// Event handler for sub project status update.
        /// </summary>
        public event EventHandler<cSyncEventParamProjectStatus> SubProjectStatusUpdate;

        /// <summary>
        /// Event handler errors.
        /// </summary>
        public event EventHandler<cSyncEventErrorParams> ProjectSyncError;

        /// <summary>
        /// 
        /// </summary>
        private cDataAccess m_cData = new cDataAccess();

        /// <summary>
        /// Settings class.
        /// </summary>
        private cSettings m_cSetting = new cSettings();


        /// <summary>
        /// v1.0.1 - Flag to indicate if to sync changes only.
        /// </summary>
        public static bool p_bSyncChangesOnly = false;

        /// <summary>
        /// v1.0.1 - List of projects to sync
        /// </summary>
        public static ObservableCollection<cProjectSearch> p_ocProjectsToSync = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public cSyncing()
        {

            try
            {

                //this.m_cSetting.DisplayMessage += m_cSetting_DisplayMessage;

                this.m_cData.CheckDB();

            }
            catch (Exception ex)
            {


            }

          
        }

        void m_cSetting_DisplayMessage(object sender, string e)
        {

            this.DisplayMessage(this, e);

        }


        /// <summary>
        /// Upload changes to AX.
        /// </summary>
        public async Task<bool> UploadChanges(bool v_bBackgroundTask)
        {

            bool bConnected = false;
            cAXCalls cAX = null;
            bool bErrorOccurred = false;
            bool bReturnStatus = true;

            //Create changes upload object for passing to WCF.
            ObservableCollection<wcfAX.cAXDataUploadDataChange> cChanges = new ObservableCollection<wcfAX.cAXDataUploadDataChange>();

            //v1.0.1 - OC for new notes.
            ObservableCollection<wcfAX.NoteDetails> cNotes = new ObservableCollection<wcfAX.NoteDetails>();

            //v1.0.1 - Processing new notes.
            List<cProjectNotesTable> pntNotes = new List<cProjectNotesTable>();

            ///v1.0.1
            wcfAX.NoteDetails ndNote;

            //
            wcfAX.cAXDataUploadDataChange cChange = new wcfAX.cAXDataUploadDataChange();

            wcfAX.UploadChangesResult uUploadResult = null;

            string sPurpose = cSettings.ReturnPurposeType();

            int iSubProjectCount = 0;

            //v1.0.10 - Unit change upload variables.
            ObservableCollection<wcfAX.UnitDetails> ocUnitDetails = new ObservableCollection<wcfAX.UnitDetails>();
            wcfAX.UnitDetails udUnitDetail;
            DateTime dInstallationDate;
            string sInstallationTeam;

            List<cUnitsUpdateTable> cUnitUpdates;

            bool bUploadOK = false;


            try
            {

                if (v_bBackgroundTask == false)
                {

                    //Update screen.                
                    this.UpdateMessage(this, new cSyncEventParam("Checking connection.."));


                }


                //Check we are connected first
                bConnected = await this.m_cSetting.IsAXSystemAvailable(false);
                if (bConnected == false)
                {

                    if (v_bBackgroundTask == false)
                    {

                        //Update screen.
                        this.UpdateMessage(this, new cSyncEventParam("No connection..."));

                    }
                    
                    return false;
                }
                          
                List<cUpdatesTable> cSubProjectUpdates = null;

                string sUserName = await cSettings.GetUserName();
                string sMachineName = cSettings.GetMachineName();
                string sUserFullName = await cSettings.GetUserDisplayName();

                //Create new instance of 
                cAX = new cAXCalls();
 

                //Fetch list of sub project with updates to upload

                //Update screen.
                if (v_bBackgroundTask == false) { this.UpdateMessage(this, new cSyncEventParam("Retrieving sub project with pending data uploads...")); };

                List<cSubProjectSync> cSubProjects = this.m_cData.FetchSubProjectsWithUploads();
                if (cSubProjects.Count > 0)
                {
                                                           
                        //Loop through sub project and try and upload the changes for each.
                        foreach (cSubProjectSync cSubProject in cSubProjects)
                        {

                            iSubProjectCount += 1;

                            bErrorOccurred = false;

                            //Update screen.
                            if (v_bBackgroundTask == false) { this.UpdateMessage(this, new cSyncEventParam("Uploading Sub project: (" + iSubProjectCount.ToString() + " of " + cSubProjects.Count.ToString() + ") - " + cSubProject.SubProjectNo)); };

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
                                        cChange = new wcfAX.cAXDataUploadDataChange();
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
                                        if (v_bBackgroundTask == false) { this.UpdateMessage(this, new cSyncEventParam("Upload failed for sub project: " + cSubProject.SubProjectNo)); };

                                        bErrorOccurred = true;
                                    }

                                }

                                if (cSubProject.UnitUpdateQty > 0)
                                {
                                
                                    //v1.0.10 - Upload unit status changes if running contract manager mode.
                                    if (cSettings.IsThisTheSurveyorApp() == false)
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
                                                ocUnitDetails = new ObservableCollection<wcfAX.UnitDetails>();
                                            }

                                            //Update observable collection with unit updates.
                                            foreach (cUnitsUpdateTable cUpdate in cUnitUpdates)
                                            {

                                                udUnitDetail = new wcfAX.UnitDetails();
                                                udUnitDetail.dInstalledDate = dInstallationDate;
                                                udUnitDetail.iInstalledStatus = cUpdate.InstalledStatus;
                                                udUnitDetail.iUNITNUMBER = cUpdate.UnitNo;

                                                ocUnitDetails.Add(udUnitDetail);

                                            }

                                            //Pass up.
                                            bUploadOK = await cAX.UploadUnitStatusChanges(cSubProject.SubProjectNo, sUserName, sMachineName, dInstallationDate,sInstallationTeam, ocUnitDetails);

                                            //If OK then remove updates from table.
                                            if (bUploadOK == true)
                                            {

                                                this.m_cData.DeleteUnitUpdates(cSubProject.SubProjectNo);

                                            }
                                            else
                                            {

                                                //Update screen.
                                                if (v_bBackgroundTask == false) { this.UpdateMessage(this, new cSyncEventParam("Upload failed for sub project: " + cSubProject.SubProjectNo)); };

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
                                if (v_bBackgroundTask == false) { this.UpdateMessage(this, new cSyncEventParam("Checking connection.")); };

                                //Check we are connected first
                                bConnected = await this.m_cSetting.IsAXSystemAvailable(false);
                                if (bConnected == false)
                                {

                                    //Update screen.
                                    this.UpdateMessage(this, new cSyncEventParam("Checking Failed."));

                                    bReturnStatus = false;
                                    break;
                                }

                            }


                        }

                           
                }

                //** Upload notes to AX **

                //Update screen.
                if (v_bBackgroundTask == false) { this.UpdateMessage(this, new cSyncEventParam("Fetching sub projects with new notes")); };

                iSubProjectCount = 0;

                cSubProjects = this.m_cData.FetchSubProjectsWithNewNotes();
                if (cSubProjects != null)
                {

                    //Loop through sub project and try and upload the changes for each.
                    foreach (cSubProjectSync cSubProject in cSubProjects)
                    {

                        iSubProjectCount += 1;

                        bErrorOccurred = false;

                        //Update screen.
                        if (v_bBackgroundTask == false) { this.UpdateMessage(this, new cSyncEventParam("Uploading notes for Sub project: (" + iSubProjectCount.ToString() + " of " + cSubProjects.Count.ToString() + ") - " + cSubProject.SubProjectNo)); };


                        try
                        {

                            cNotes.Clear();

                            pntNotes = this.m_cData.FetchNewNotes(cSubProject.SubProjectNo);
                            foreach (cProjectNotesTable cNote in pntNotes)
                            {
                                ndNote = new wcfAX.NoteDetails();
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
                                if (v_bBackgroundTask == false) { this.UpdateMessage(this, new cSyncEventParam("Notes upload failed for sub project: " + cSubProject.SubProjectNo)); };

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
                    await cAX.CloseAXConnection();
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

            cAXCalls cAX = null;
            wcfAX.UploadFileChange cFileUpload;
            StorageFile sfFile = null;
            bool bUploadOK = false;
            bool bSaveOK = false;
            bool bConnected = false;
            bool bErrorOccurred = false;
            bool bReturnStatus = true;
            
            try
            {


                string sUserName = await cSettings.GetUserName();
                string sMachineName = cSettings.GetMachineName();

                //Update screen.
                this.UpdateMessage(this, new cSyncEventParam("Checking connection."));

                //Check we are connected first
                bConnected = await this.m_cSetting.IsAXSystemAvailable(false);
                if (bConnected == false)
                {
                    
                    //Update screen.
                    this.UpdateMessage(this, new cSyncEventParam("Checking failed."));

                    return false;
                }

                //Fetch list of files waiting to be uploaded.

                //Update screen.
                this.UpdateMessage(this, new cSyncEventParam("Checking for files to uploading."));

                List<cProjectFilesTable> cFiles = this.m_cData.ReturnNewFilesForUploading();
                if (cFiles != null)
                {

                    cAX = new cAXCalls();

                    int iFileCount = 0;
                    wcfAX.UploadChangesResult ufResult = null;


                    foreach (cProjectFilesTable cFile in cFiles)
                    {

                        iFileCount += 1;
                        bSaveOK = false;

                        //Update screen.
                        this.UpdateMessage(this, new cSyncEventParam("Uploading file (" + iFileCount.ToString() + " of " + cFiles.Count.ToString() + ") Sub project: " + cFile.SubProjectNo));

                        bErrorOccurred = false;

                        try
                        {

                            cFileUpload = new wcfAX.UploadFileChange();
                            cFileUpload.sComment = cFile.NoteText;
                            cFileUpload.sFileName = cFile.FileName;

                            sfFile = await cSettings.ReturnStorageFileForSubProject(cFile.SubProjectNo, cFile.FileName);
                            if (sfFile != null)
                            {
                                cFileUpload.byData = await cSettings.ConvertFileToByteArray(sfFile);

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
                                        this.UpdateMessage(this, new cSyncEventParam("File upload failed."));
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
                            this.UpdateMessage(this, new cSyncEventParam("Checking Connection."));

                            //Check we are connected first
                            bConnected = await this.m_cSetting.IsAXSystemAvailable(false);
                            if (bConnected == false)
                            {

                                //Update screen.
                                this.UpdateMessage(this, new cSyncEventParam("Connection Failed."));

                                bReturnStatus = false;
                                break;
                            }

                        }
                                                                     
                    }

                    if (cAX != null)
                    {
                        await cAX.CloseAXConnection();
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
        private async Task<bool> CheckForFileChanges(string v_sProjectNo, string v_sSubProjectNo,bool v_bBackgroundTask)
        {

            bool bSaveOK = false;
            bool bConnected = false;
            bool bErrorOccurred = false;
            bool bReturnStatus = true;

            cAXCalls cAX = null;

            //ObservableCollection<wcfAX.DownloadDataChange> cSubProjects = null;
            wcfAX.SubProjectFileDetail cProjectFile;
            List<cProjectFilesTable> cSubProjectFiles = null;
            //ObservableCollection<wcfAX.SubProjectData> cSubProjectsChanged = null;
            List<cSubProjectSync> cSubProjectsSync;
            wcfAX.SubProjectFile sfSubProject;
            wcfAX.FileChangesResult sfrResult;
            ObservableCollection<wcfAX.SubProjectFileDetail> cProjectFileDetails = null;

            wcfAX.SubProjectFileDownloadResult sfdResult;

            StorageFolder sfSubProjectFolder = null;

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

                    cAX = new cAXCalls();
                                                      
                    cSubProjectFiles = this.m_cData.FetchSubProjectFilesForChecking(v_sSubProjectNo);

                    cProjectFileDetails = new ObservableCollection<wcfAX.SubProjectFileDetail>();

                    if (cSubProjectFiles.Count > 0)
                    {
                                               
                        cProjectFile = new wcfAX.SubProjectFileDetail();
                        cProjectFile.sProjectNo = v_sProjectNo;
                        cProjectFile.sSubProjectNo = v_sSubProjectNo;
                        cProjectFile.sfFiles = new ObservableCollection<wcfAX.SubProjectFile>();

                       foreach (cProjectFilesTable cSubProjectFile in cSubProjectFiles)
                       {
                            sfSubProject = new wcfAX.SubProjectFile();
                            sfSubProject.FileName = cSubProjectFile.FileName;
                            sfSubProject.Comments = cSubProjectFile.NoteText;
                            sfSubProject.ModifiedDate = cSubProjectFile.ModDateTime;

                            cProjectFile.sfFiles.Add(sfSubProject);
                                       
                        }

                        cProjectFileDetails.Add(cProjectFile);

                    }

                        cProjectFileDetails = await cAX.CheckForAXFileChanges(v_sProjectNo,v_sSubProjectNo,cProjectFileDetails);
                        if (cProjectFileDetails != null)
                        {

                           
                            foreach (wcfAX.SubProjectFileDetail sfdFile in cProjectFileDetails)
                            {


                                sfSubProjectFolder = await cSettings.ReturnSubProjectImagesFolder(sfdFile.sSubProjectNo);

                                if (sfdFile.sfFiles != null)
                                {

                                    int iFileCount = 0;

                                    foreach (wcfAX.SubProjectFile spfFile in sfdFile.sfFiles)
                                    {

                                        iFileCount += 1;

                                        //Update screen.
                                        if (v_bBackgroundTask == false) { this.UpdateMessage(this, new cSyncEventParam("Downloading file (" + iFileCount.ToString() + " of " + sfdFile.sfFiles.Count.ToString() + ") - Sub Project (" + v_sSubProjectNo + ")")); };


                                        sfdResult = await cAX.ReturnFileData(spfFile.FileName);


                                        //Save file to device.
                                        bool bFileSaved = await cSettings.SaveFileLocally(sfSubProjectFolder, sfdResult.byFileData, spfFile.FileName);
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
                    if (v_bBackgroundTask == false) { this.UpdateMessage(this, new cSyncEventParam("Checking Connection.")); };

                    //Check we are connected first
                    bConnected = await this.m_cSetting.IsAXSystemAvailable(false);
                    if (bConnected == false)
                    {

                        //Update screen.
                        if (v_bBackgroundTask == false) { this.UpdateMessage(this, new cSyncEventParam("Connection Failed.")); };

                        bReturnStatus = false;
                        
                    }

                }

                if (cAX != null)
                {
                    await cAX.CloseAXConnection();
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
            cDataAccess.SaveSubProjectDataResult srResult;
            bool bConnected = false;
            bool bErrorOccurred = false;
            bool bReturnStatus = true;

            cAXCalls cAX = null;
            //cDataAccess cDataDB = null;

            ObservableCollection<wcfAX.DownloadDataChange> cSubProjects = null;
            wcfAX.DownloadDataChange cSubProject;
            List<cSubProjectSyncUpdateValues> cSubProjectDates = null;
            ObservableCollection<wcfAX.SubProjectData> cSubProjectsChanged = null;

            //v1.0.10 - Data change result object.
            wcfAX.DownloadDataChangesResult dcResult = null;

            string sProjectStatus = string.Empty;
            string sSubProjectStatus = string.Empty;

            int iSubProjectLimit = -1;
            bool bCheckForNewSubProjects = true;

            int iSubProjectsAdded = 0;
            int iSubProjectsDeleted = 0;

            List<cProjectNotesTable> cNotes = null;
            wcfAX.clsRealtimeNoteKeyValues cNoteKey;
            List<cUnitsTable> cUnits = null;
            wcfAX.UnitDetails udUnitDetail;

            try
            {

                //If called from background task then go off and fetch projects to sync
                if (v_bBackgroundTask == true)
                {

                    iSubProjectLimit = 2;
                    bCheckForNewSubProjects = false;

                    cSyncing.p_ocProjectsToSync = this.m_cData.FetchProjectsToSync();

                }

                //Update screen.
                if (v_bBackgroundTask == false) {
                    
                    this.UpdateMessage(this, new cSyncEventParam("Checking connection."));


                    //Check we are connected first
                    bConnected = await this.m_cSetting.IsAXSystemAvailable(false);
                    if (bConnected == false)
                    {

                        //Update screen.
                        if (v_bBackgroundTask == false) { this.UpdateMessage(this, new cSyncEventParam("Checking failed.")); };

                        return false;
                    }
             
                               
                }

             
                if (cSyncing.p_ocProjectsToSync != null)
                {

                    int iProjectCount = 0;
                    int iSubProjectCount = 0;

                    cAX = new cAXCalls();

                    string sLastProjectNo = String.Empty;

                    foreach (cProjectSearch cProject in cSyncing.p_ocProjectsToSync)
                    {

                        bErrorOccurred = false;
                        iProjectCount += 1;
                                                                        
                        //Update screen.
                        if (v_bBackgroundTask == false)
                        {

                            sProjectStatus = "Checking project (" + iProjectCount.ToString() + " of " + cSyncing.p_ocProjectsToSync.Count.ToString() + ") " + cProject.ProjectNo;

                            this.UpdateMessage(this, new cSyncEventParam(sProjectStatus));

                            this.SubProjectStatusUpdate(this, new cSyncEventParamProjectStatus(sLastProjectNo, cProject.ProjectNo, iSubProjectsAdded, iSubProjectsDeleted, true));

                        }


                        iSubProjectsAdded = 0;
                        iSubProjectsDeleted = 0;
                        
                        try
                        {

                            
                            cSubProjects = new  ObservableCollection<wcfAX.DownloadDataChange>();

                            cSubProjectDates = this.m_cData.FetchSubProjectUpdateDateTime(cProject.ProjectNo,iSubProjectLimit);
                            foreach (cSubProjectSyncUpdateValues cSubProjectValue in cSubProjectDates)
                            {

                                                           
                                cSubProject = new wcfAX.DownloadDataChange();
                                cSubProject.sProjectNo = cProject.ProjectNo;
                                cSubProject.sSubProjectNo = cSubProjectValue.SubProjectNo;

                                cSubProject.Notes = new ObservableCollection<wcfAX.clsRealtimeNoteKeyValues>();
                                
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
                                    cNoteKey = new wcfAX.clsRealtimeNoteKeyValues();
                                    cNoteKey.DeviceIDKey = cProjNote.IDKey;
                                    cNoteKey.NotesRecID = cProjNote.AXRecID;
                                    cNoteKey.ProjectNo = cSubProjectValue.SubProjectNo;

                                    cSubProject.Notes.Add(cNoteKey);

                                }

                                //Units are only used for the installers app.
                                if (cSettings.IsThisTheSurveyorApp() == false)
                                {
                                    cSubProject.Units = new ObservableCollection<wcfAX.UnitDetails>();

                                    //Add units to upload.
                                    cUnits = this.m_cData.FetchUnitsForSubProject(cSubProject.sSubProjectNo);
                                    foreach (cUnitsTable cUnitRow in cUnits)
                                    {

                                        udUnitDetail = new wcfAX.UnitDetails();
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
                                    if (v_bBackgroundTask == false) { this.ProjectSyncError(this, new cSyncEventErrorParams(ex.Message, cProject.ProjectNo)); };

                                }

                            } while (iAttempts < 3); //Try 3 times.

                            
                            //Sub projects to update.
                            if (cSubProjectsChanged != null)
                            {

                                foreach (wcfAX.SubProjectData cData in cSubProjectsChanged)
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
                            foreach (cSubProjectSyncUpdateValues cSubProjectValue in cSubProjectDates)
                            {

                                iSubProjectCount += 1;
                                sSubProjectStatus = "Checking files for sub project (" + iSubProjectCount.ToString() + " of " + cSubProjectDates.Count.ToString() + ")";

                                //Update screen.
                                if (v_bBackgroundTask == false) { this.UpdateMessage(this, new cSyncEventParam(sProjectStatus + " - " + sSubProjectStatus)); };

                                bSaveOK = await this.CheckForFileChanges(cProject.ProjectNo, cSubProjectValue.SubProjectNo,v_bBackgroundTask );
                                if (bSaveOK == false)
                                {
                                    throw new Exception("Error checking files for sub project (" + cSubProjectValue.SubProjectNo + ")");
                                }

                            }


                        }
                        catch (Exception ex)
                        {

                            if (v_bBackgroundTask == false) { this.ProjectSyncError(this, new cSyncEventErrorParams(ex.Message, cProject.ProjectNo)); };

                            bErrorOccurred = true;
                        }

                        //If error occurred check connection is OK, if not leave.
                        if (bErrorOccurred == true)
                        {

                            //Update screen.
                            if (v_bBackgroundTask == false) { this.UpdateMessage(this, new cSyncEventParam("Checking Connection.")); };

                            //Check we are connected first
                            bConnected = await this.m_cSetting.IsAXSystemAvailable(false);
                            if (bConnected == false)
                            {

                                //Update screen.
                                if (v_bBackgroundTask == false) { this.UpdateMessage(this, new cSyncEventParam("Connection Failed.")); };

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
                        
                        this.SubProjectStatusUpdate(this, new cSyncEventParamProjectStatus(sLastProjectNo, string.Empty, iSubProjectsAdded,iSubProjectsDeleted, bUpdateSuccess));
                                  
                    }                    

                    if (cAX != null)
                    {
                        await cAX.CloseAXConnection();
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
