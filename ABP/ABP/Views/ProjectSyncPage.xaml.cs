using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ABP.WcfProxys;
using Acr.UserDialogs;
using System.Collections.ObjectModel;
using ABP.TableModels;
using ABP.Models;

namespace ABP.Views
{
    public partial class ProjectSyncPage : ContentPage
    {
        private cAXCalls cAX = null;
        public ProjectSyncPage()
        {
            InitializeComponent();
            this.Title = "Project Sync";
        }
        private void SendChanges_Clicked(object sender, EventArgs args)
        {
            try
            {

                cSyncing.p_bSyncChangesOnly = true;

                //Kick off check for syncing.
                cMain.p_bIsSyncingInProgress = true;
                if (cSettings.AreWeOnline == true)
                {
                    //string sUserProfile = await DependencyService.Get<ISettings>().GetUserName();
                    cAX = new cAXCalls();
                    string sUserProfile = WcfLogin.m_instance.LoggedUserName;
                    cAX.m_wcfClient.ReturnAreSystemsAvailableCompleted += M_wcfClient_ReturnAreSystemsAvailableCompleted;
                    cAX.m_wcfClient.ReturnAreSystemsAvailableAsync(cAX.m_cCompanyName, sUserProfile, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);
                    UserDialogs.Instance.ShowLoading("Checking System Available...", MaskType.Black);
                }
                else
                {
                    DisplayAlert("Warning", "Internet connection failed.", "OK");
                }

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        private void M_wcfClient_ReturnAreSystemsAvailableCompleted(object sender, ServiceExt.ReturnAreSystemsAvailableCompletedEventArgs e)
        {
            UserDialogs.Instance.HideLoading();
            if (e.Error != null)
            {

            }
            else if (e.Cancelled == true)
            {

            }
            else
            {
                if (e.Result.SystemsAvailable == true)
                {
                    cSyncing cSync = null;
                    cSync = new cSyncing();
                    // sSync upload changes
                    ObservableCollection<ServiceExt.cAXDataUploadDataChange> cChanges = new ObservableCollection<ServiceExt.cAXDataUploadDataChange>();
                    ObservableCollection<ServiceExt.NoteDetails> cNotes = new ObservableCollection<ServiceExt.NoteDetails>();
                    List<cProjectNotesTable> pntNotes = new List<cProjectNotesTable>();
                    ServiceExt.NoteDetails ndNote;
                    ServiceExt.cAXDataUploadDataChange cChange = new ServiceExt.cAXDataUploadDataChange();

                    string sPurpose = cSettings.ReturnPurposeType();
                    int iSubProjectCount = 0;
                    ObservableCollection<ServiceExt.UnitDetails> ocUnitDetails = new ObservableCollection<ServiceExt.UnitDetails>();

                    List<cUpdatesTable> cSubProjectUpdates = null;
                    bool bErrorOccurred = false;
                    bool bReturnStatus = true;

                    string sUserName = WcfLogin.m_instance.LoggedUserName;
                    string sMachineName = WcfLogin.m_instance.LoggedUserName;
                    string sUserFullName = WcfLogin.m_instance.LoggedUserName;
                    cAX = new cAXCalls();
                    List<cSubProjectSync> cSubProjects = cMain.p_cDataAccess.FetchSubProjectsWithUploads();
                    if (cSubProjects.Count > 0)
                    {
                        foreach (cSubProjectSync cSubProject in cSubProjects)
                        {
                            iSubProjectCount += 1;

                            bErrorOccurred = false;
                            // "Uploading Sub project:
                            try
                            {
                                if (cSubProject.UpdateQty > 0)
                                {
                                    cChanges.Clear();
                                    cSubProjectUpdates = cMain.p_cDataAccess.ReturnPendingUpdatesForSubProject(cSubProject.SubProjectNo);
                                    foreach (cUpdatesTable cUpdate in cSubProjectUpdates)
                                    {
                                        cChange = new ServiceExt.cAXDataUploadDataChange();
                                        cChange.ProjectNo = cUpdate.SubProjectNo;
                                        cChange.FieldName = cUpdate.FieldName;
                                        cChange.FieldValue = cUpdate.FieldValue;
                                        cChanges.Add(cChange);

                                    }
                                    cAX.m_wcfClient.UploadSubProjectDataChangesCompleted += (sender1, e1) =>
                                    {
                                        if (e1.Error != null) { }
                                        else if (e1.Cancelled == true) { }
                                        else
                                        {
                                            if (e1.Result.bSuccessfull == true)
                                            {
                                                cMain.p_cDataAccess.RemoveChangesFromUploadTable(cSubProjectUpdates);
                                            }
                                            else
                                            {
                                                bErrorOccurred = true;
                                            }
                                        }
                                    };
                                    cAX.m_wcfClient.UploadSubProjectDataChangesAsync(cAX.m_cCompanyName, cAX.m_sPurpose, WcfLogin.m_instance.LoggedUserName,
                                        WcfLogin.m_instance.LoggedUserName, cSubProject.SubProjectNo, cChanges, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);
                                }
                                if (cSubProject.UnitUpdateQty > 0)
                                {
                                    if (cSettings.IsThisTheSurveyorApp() == false)
                                    {
                                        // ignore install mode
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                bErrorOccurred = true;
                            }

                        }

                    }
                    iSubProjectCount = 0;
                    cSubProjects = cMain.p_cDataAccess.FetchSubProjectsWithNewNotes();
                    if (cSubProjects != null)
                    {
                        foreach (cSubProjectSync cSubProject in cSubProjects)
                        {
                            iSubProjectCount += 1;
                            bErrorOccurred = false;
                            try
                            {
                                cNotes.Clear();
                                pntNotes = cMain.p_cDataAccess.FetchNewNotes(cSubProject.SubProjectNo);
                                foreach (cProjectNotesTable cNote in pntNotes)
                                {
                                    ndNote = new ServiceExt.NoteDetails();
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
                                cAX.m_wcfClient.UploadSubProjectNotesChangesCompleted += (sender2, e2) =>
                                {
                                    if (e2.Error != null) { }
                                    else if (e2.Cancelled == true) { }
                                    else
                                    {
                                        if (e2.Result.bSuccessfull == true)
                                        {
                                            cMain.p_cDataAccess.UpdateNotesWithRecID(e2.Result.NoteValues);
                                        }
                                        else
                                        {
                                            bErrorOccurred = true;
                                        }
                                    }
                                };
                                cAX.m_wcfClient.UploadSubProjectNotesChangesAsync(cAX.m_cCompanyName, cAX.m_sPurpose, WcfLogin.m_instance.LoggedUserName,
                                    WcfLogin.m_instance.LoggedUserName, cSubProject.SubProjectNo, cNotes, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);
                            }
                            catch (Exception ex)
                            {
                                bReturnStatus = true;
                            }
                        }
                    }
                    if (cAX != null)
                    {
                        try
                        {
                            if (cAX.m_wcfClient != null)
                            {
                                cAX.m_wcfClient.CloseCompleted += (sender3, e3) =>
                                {
                                    cAX.m_wcfClient = null;
                                };
                                cAX.m_wcfClient.CloseAsync();
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                else
                {
                    DisplayAlert("Project Sync Error", "No connection", "OK");
                }
                //throw new NotImplementedException();
            }
        }

        private void CSync_SubProjectStatusUpdate(object sender, cSyncEventParamProjectStatus e)
        {
            throw new NotImplementedException();
        }

        private void SyncAll_Clicked(object sender, EventArgs args)
        {

        }
    }
}
