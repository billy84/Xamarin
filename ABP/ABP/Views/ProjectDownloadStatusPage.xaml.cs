using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ABP.WcfProxys;
using ABP.TableModels;
using Acr.UserDialogs;
using ABP.Interfaces;

namespace ABP.Views
{
    public class Log
    {
        public string Status { get; set; }
        public Color Color { get; set; }
    }
    public partial class ProjectDownloadStatusPage : ContentPage
    {
        ObservableCollection<Log> logs = new ObservableCollection<Log>();
        cAXCalls cAX = null;
        public ProjectDownloadStatusPage(ServiceExt.SearchResult oResult)
        {
            InitializeComponent();
            this.Title = "Project Download";
            txtProjectNo.Text = oResult.ProjectNo;
            txtProjectName.Text = oResult.ProjectName;
            lvProgressStatus.ItemsSource = logs;
        }
        private void LoggingStatus(Color color, string status)
        {
            Log log = new Log() { Color = color, Status = status };
            logs.Add(log);
        }
        private void Clicked_Download(object sender, EventArgs args)
        {
            cAXCalls cAX = null;
            //cSettings.AbortRetryIgnore rResponse = cSettings.AbortRetryIgnore.Abort;
            //cDataAccess.SaveSubProjectDataResult srSaved;
            //bool bDownloadCompletedOK = false;
            try
            {
                LoggingStatus(Color.White, "Checking if project already downloaded.");
                bool bAlreadyDownloaded = IsProjectAlreadyOnDevice(txtProjectNo.Text);
                if (bAlreadyDownloaded == true)
                {
                    return;
                }
                LoggingStatus(Color.White, "Checking connection.");
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
                    LoggingStatus(Color.White, "Checking for configuration updates.");
                    if (cMain.m_bCheckingBaseEnums == true)
                    {
                        if (cMain.m_bCheckingSetings == true)
                        {
                            if (cMain.m_bCheckingSurveyFailedReasons == true)
                            {
                                try
                                {
                                    cAX.m_wcfClient.ReturnSubProjectsListCompleted += M_wcfClient_ReturnSubProjectsListCompleted;
                                    cAX.m_wcfClient.ReturnSubProjectsListAsync(cAX.m_cCompanyName, txtProjectNo.Text, cAX.m_sPurpose, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);

                                }
                                catch (Exception ex)
                                {

                                }
                            }
                            else
                            {
                                cMain.m_bCheckingSurveyFailedReasons = true;
                                cMain.ShouldICheckForSurveyReasonResult cCheckResult = cMain.ShouldICheckForFailedSurveyReasons();
                                if (cCheckResult.bCheck == true)
                                {
                                    cAX = new cAXCalls();
                                    cAX.m_wcfClient.FetchFailedSurveyReasonsCompleted += M_wcfClient_FetchFailedSurveyReasonsCompleted;
                                    cAX.m_wcfClient.FetchFailedSurveyReasonsAsync(cAX.m_cCompanyName, cCheckResult.dLastUpdate, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);

                                }
                            }
                        }
                        else
                        {
                            bool bCheck = cMain.ShouldICheckForSettings();
                            if (bCheck == true)
                            {
                                cAX = new cAXCalls();
                                List<ServiceExt.SettingDetails> sdSetting = cMain.p_cDataAccess.GetSettingsUpdates();
                                ObservableCollection<ServiceExt.SettingDetails> ocSettings = new ObservableCollection<ServiceExt.SettingDetails>(sdSetting);
                                cAX.m_wcfClient.CheckForUpdatedSettingsCompleted += M_wcfClient_CheckForUpdatedSettingsCompleted;
                                cAX.m_wcfClient.CheckForUpdatedSettingsAsync(cAX.m_cCompanyName, ocSettings, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);
                            }
                            else
                            {
                                if (cMain.m_bCheckingSurveyFailedReasons == true)
                                {
                                    try
                                    {
                                        cAX.m_wcfClient.ReturnSubProjectsListCompleted += M_wcfClient_ReturnSubProjectsListCompleted;
                                        cAX.m_wcfClient.ReturnSubProjectsListAsync(cAX.m_cCompanyName, txtProjectNo.Text, cAX.m_sPurpose, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                else
                                {
                                    cMain.m_bCheckingSurveyFailedReasons = true;
                                    cMain.ShouldICheckForSurveyReasonResult cCheckResult = cMain.ShouldICheckForFailedSurveyReasons();
                                    if (cCheckResult.bCheck == true)
                                    {
                                        cAX = new cAXCalls();
                                        cAX.m_wcfClient.FetchFailedSurveyReasonsCompleted += M_wcfClient_FetchFailedSurveyReasonsCompleted;
                                        cAX.m_wcfClient.FetchFailedSurveyReasonsAsync(cAX.m_cCompanyName, cCheckResult.dLastUpdate, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        bool bCheck = cMain.ShouldICheckForBaseEnums();
                        if (bCheck == true)
                        {
                            // check for updates
                            // 1. check for base eunms
                            cAX = new cAXCalls();
                            List<ServiceExt.BaseEnumField> bFields = cMain.p_cDataAccess.GetBaseEnumUpdates();
                            // cAX.ReturnUpdatedBaseEnums
                            ObservableCollection<ServiceExt.BaseEnumField> ocFields = new ObservableCollection<ServiceExt.BaseEnumField>(bFields);
                            cAX.m_wcfClient.ReturnBaseEnumsCompleted += M_wcfClient_ReturnBaseEnumsCompleted;
                            cAX.m_wcfClient.ReturnBaseEnumsAsync(cAX.m_cCompanyName, ocFields, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);
                        }
                        else
                        {
                            cMain.m_bCheckingBaseEnums = false;
                            if (cMain.m_bCheckingSetings == true)
                            {
                                if (cMain.m_bCheckingSurveyFailedReasons == true)
                                {
                                    try
                                    {
                                        cAX.m_wcfClient.ReturnSubProjectsListCompleted += M_wcfClient_ReturnSubProjectsListCompleted;
                                        cAX.m_wcfClient.ReturnSubProjectsListAsync(cAX.m_cCompanyName, txtProjectNo.Text, cAX.m_sPurpose, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                else
                                {
                                    cMain.m_bCheckingSurveyFailedReasons = true;
                                    cMain.ShouldICheckForSurveyReasonResult cCheckResult = cMain.ShouldICheckForFailedSurveyReasons();
                                    if (cCheckResult.bCheck == true)
                                    {
                                        cAX = new cAXCalls();
                                        cAX.m_wcfClient.FetchFailedSurveyReasonsCompleted += M_wcfClient_FetchFailedSurveyReasonsCompleted;
                                        cAX.m_wcfClient.FetchFailedSurveyReasonsAsync(cAX.m_cCompanyName, cCheckResult.dLastUpdate, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);

                                    }
                                }
                            }
                            else
                            {
                                bCheck = cMain.ShouldICheckForSettings();
                                if (bCheck == true)
                                {
                                    cAX = new cAXCalls();
                                    List<ServiceExt.SettingDetails> sdSetting = cMain.p_cDataAccess.GetSettingsUpdates();
                                    ObservableCollection<ServiceExt.SettingDetails> ocSettings = new ObservableCollection<ServiceExt.SettingDetails>(sdSetting);
                                    cAX.m_wcfClient.CheckForUpdatedSettingsCompleted += M_wcfClient_CheckForUpdatedSettingsCompleted;
                                    cAX.m_wcfClient.CheckForUpdatedSettingsAsync(cAX.m_cCompanyName, ocSettings, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);
                                }
                                else
                                {
                                    if (cMain.m_bCheckingSurveyFailedReasons == true)
                                    {
                                        try
                                        {
                                            cAX.m_wcfClient.ReturnSubProjectsListCompleted += M_wcfClient_ReturnSubProjectsListCompleted;
                                            cAX.m_wcfClient.ReturnSubProjectsListAsync(cAX.m_cCompanyName, txtProjectNo.Text, cAX.m_sPurpose, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);

                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                    else
                                    {
                                        cMain.m_bCheckingSurveyFailedReasons = true;
                                        cMain.ShouldICheckForSurveyReasonResult cCheckResult = cMain.ShouldICheckForFailedSurveyReasons();
                                        if (cCheckResult.bCheck == true)
                                        {
                                            cAX = new cAXCalls();
                                            cAX.m_wcfClient.FetchFailedSurveyReasonsCompleted += M_wcfClient_FetchFailedSurveyReasonsCompleted;
                                            cAX.m_wcfClient.FetchFailedSurveyReasonsAsync(cAX.m_cCompanyName, cCheckResult.dLastUpdate, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);

                                        }
                                        else
                                        {
                                            try
                                            {
                                                cAX = new cAXCalls();
                                                cAX.m_wcfClient.ReturnSubProjectsListCompleted += M_wcfClient_ReturnSubProjectsListCompleted;
                                                cAX.m_wcfClient.ReturnSubProjectsListAsync(cAX.m_cCompanyName, txtProjectNo.Text, cAX.m_sPurpose, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);

                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void M_wcfClient_ReturnBaseEnumsCompleted(object sender, ServiceExt.ReturnBaseEnumsCompletedEventArgs e)
        {
            if (e.Error != null)
            {

            }
            else if (e.Cancelled == true)
            {

            }
            else
            {
                if (e.Result != null)
                {
                    if (e.Result.bSuccessfull == true)
                    {
                        List<ServiceExt.BaseEnumField> beFieldsUpdate = new List<ServiceExt.BaseEnumField>(e.Result.BaseEnumResults);
                        cMain.p_cDataAccess.ProcessUpdatedBaseEnums(beFieldsUpdate);
                        cMain.m_bCheckingBaseEnums = false;
                        if (cAX != null)
                        {
                            if (cAX.m_wcfClient != null)
                            {
                                cAX.m_wcfClient.CloseCompleted += M_wcfClient_CloseCompleted;
                                cAX.m_wcfClient.CloseAsync();
                            }
                        }
                    }
                }
            }
            //throw new NotImplementedException();
        }

        private void M_wcfClient_CloseCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {

            }
            else if (e.Cancelled == true)
            {

            }
            else
            {
                cAX.m_wcfClient = null;
                // 2. check for settings
                if (cMain.m_bCheckingSetings == true)
                {
                    if (cMain.m_bCheckingSurveyFailedReasons == true)
                    {
                        try
                        {
                            cAX.m_wcfClient.ReturnSubProjectsListCompleted += M_wcfClient_ReturnSubProjectsListCompleted;
                            cAX.m_wcfClient.ReturnSubProjectsListAsync(cAX.m_cCompanyName, txtProjectNo.Text, cAX.m_sPurpose, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {
                        cMain.m_bCheckingSurveyFailedReasons = true;
                        cMain.ShouldICheckForSurveyReasonResult cCheckResult = cMain.ShouldICheckForFailedSurveyReasons();
                        if (cCheckResult.bCheck == true)
                        {
                            cAX = new cAXCalls();
                            cAX.m_wcfClient.FetchFailedSurveyReasonsCompleted += M_wcfClient_FetchFailedSurveyReasonsCompleted;
                            cAX.m_wcfClient.FetchFailedSurveyReasonsAsync(cAX.m_cCompanyName, cCheckResult.dLastUpdate, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);

                        }
                    }
                }
                else
                {
                    bool bCheck = cMain.ShouldICheckForSettings();
                    if (bCheck == true)
                    {
                        cAX = new cAXCalls();
                        List<ServiceExt.SettingDetails> sdSetting = cMain.p_cDataAccess.GetSettingsUpdates();
                        ObservableCollection<ServiceExt.SettingDetails> ocSettings = new ObservableCollection<ServiceExt.SettingDetails>(sdSetting);
                        cAX.m_wcfClient.CheckForUpdatedSettingsCompleted += M_wcfClient_CheckForUpdatedSettingsCompleted;
                        cAX.m_wcfClient.CheckForUpdatedSettingsAsync(cAX.m_cCompanyName, ocSettings, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);
                    }
                    else
                    {
                        if (cMain.m_bCheckingSurveyFailedReasons == true)
                        {
                            if (cAX == null)
                            {
                                cAX = new cAXCalls();
                            }
                            LoggingStatus(Color.White, "Fetching list of sub projects.");

                            try
                            {
                                cAX.m_wcfClient.ReturnSubProjectsListCompleted += M_wcfClient_ReturnSubProjectsListCompleted;
                                cAX.m_wcfClient.ReturnSubProjectsListAsync(cAX.m_cCompanyName, txtProjectNo.Text, cAX.m_sPurpose, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);

                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        else
                        {
                            cMain.m_bCheckingSurveyFailedReasons = true;
                            cMain.ShouldICheckForSurveyReasonResult cCheckResult = cMain.ShouldICheckForFailedSurveyReasons();
                            if (cCheckResult.bCheck == true)
                            {
                                cAX = new cAXCalls();
                                cAX.m_wcfClient.FetchFailedSurveyReasonsCompleted += M_wcfClient_FetchFailedSurveyReasonsCompleted;
                                cAX.m_wcfClient.FetchFailedSurveyReasonsAsync(cAX.m_cCompanyName, cCheckResult.dLastUpdate, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);

                            }
                        }
                    }
                }
            }
            //thrw new NotImplementedException();
        }

        private void M_wcfClient_CheckForUpdatedSettingsCompleted(object sender, ServiceExt.CheckForUpdatedSettingsCompletedEventArgs e)
        {
            if (e.Error != null)
            {

            }
            else if (e.Cancelled == true)
            {

            }
            else
            {
                if (e.Result != null)
                {
                    if (e.Result.bSuccessfull == true)
                    {
                        List<ServiceExt.SettingDetails> sdSettings = new List<ServiceExt.SettingDetails>(e.Result.Settings);
                        cMain.p_cDataAccess.ProcessUpdatedSettings(sdSettings);
                        cMain.m_bCheckingSetings = false;
                        if (cAX != null)
                        {
                            if (cAX.m_wcfClient != null)
                            {
                                cAX.m_wcfClient.CloseCompleted += M_wcfClient_CloseCompleted1;
                                cAX.m_wcfClient.CloseAsync();
                            }
                        }
                    }
                }
            }
            //throw new NotImplementedException();
        }
        private void M_wcfClient_CloseCompleted1(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {

            }
            else if (e.Cancelled == true)
            {

            }
            else
            {
                cAX.m_wcfClient = null;
                // 2. check for settings
                if (cMain.m_bCheckingSetings == true)
                {

                }
                else
                {
                    // start CheckForFailedSurveyReasons
                    if (cMain.m_bCheckingSurveyFailedReasons == true)
                    {
                        cAX = new cAXCalls();
                        LoggingStatus(Color.White, "Fetching list of sub projects.");
                        try
                        {
                            cAX.m_wcfClient.ReturnSubProjectsListCompleted += M_wcfClient_ReturnSubProjectsListCompleted;
                            cAX.m_wcfClient.ReturnSubProjectsListAsync(cAX.m_cCompanyName, txtProjectNo.Text, cAX.m_sPurpose, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {
                        cMain.m_bCheckingSurveyFailedReasons = true;
                        cMain.ShouldICheckForSurveyReasonResult cCheckResult = cMain.ShouldICheckForFailedSurveyReasons();
                        if (cCheckResult.bCheck == true)
                        {
                            cAX = new cAXCalls();
                            cAX.m_wcfClient.FetchFailedSurveyReasonsCompleted += M_wcfClient_FetchFailedSurveyReasonsCompleted;
                            cAX.m_wcfClient.FetchFailedSurveyReasonsAsync(cAX.m_cCompanyName, cCheckResult.dLastUpdate, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);

                        }
                    }
                }
            }
            //thrw new NotImplementedException();
        }

        private void M_wcfClient_FetchFailedSurveyReasonsCompleted(object sender, ServiceExt.FetchFailedSurveyReasonsCompletedEventArgs e)
        {
            if (e.Error != null)
            {

            }
            else if (e.Cancelled == true)
            {

            }
            else
            {
                if (e.Result.bSuccessfull == true)
                {
                    cAppSettingsTable vcSettings = cMain.p_cDataAccess.ReturnSettings();
                    if (e.Result.sfrReasons.Count() > 0)
                    {
                        bool bUpdateOK = cMain.p_cDataAccess.UpdateFailedSurveyReasonsTable(e.Result.sfrReasons);
                        if (bUpdateOK == true)
                        {
                            vcSettings.LastSurveyFailedUpdateDateTime = e.Result.bLastUpdateDate;
                        }
                    }
                    vcSettings.LastSurveyFailedCheckDateTime = DateTime.Now;
                    cMain.p_cDataAccess.SaveSettings(vcSettings);
                    // start downloading project
                    if(cAX == null)
                    {
                        cAX = new cAXCalls();
                    }
                    LoggingStatus(Color.White, "Fetching list of sub projects.");
                    
                    try
                    {
                        cAX.m_wcfClient.ReturnSubProjectsListCompleted += M_wcfClient_ReturnSubProjectsListCompleted;
                        cAX.m_wcfClient.ReturnSubProjectsListAsync(cAX.m_cCompanyName, txtProjectNo.Text, cAX.m_sPurpose, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token );

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            //throw new NotImplementedException();
        }

        private async void M_wcfClient_ReturnSubProjectsListCompleted(object sender, ServiceExt.ReturnSubProjectsListCompletedEventArgs e)
        {
            List<string> lSubProjects = null;
            if (e.Error != null)
            {

            }
            else if (e.Cancelled == true)
            {

            }
            else
            {
                if (e.Result.bSuccessfull == true)
                {
                    if (e.Result.SubProjects != null)
                    {
                        lSubProjects = e.Result.SubProjects.ToList();
                        
                    }
                    if (lSubProjects != null)
                    {
                        if (lSubProjects.Count() == 0)
                        {
                            await DisplayAlert("Warning", "This project does not have any sub projects to download.", "OK");
                            return;
                        }
                        int iSubProjectCount = 0;

                        bool bSubProjectOK = false;
                        bool bErrorOccured = false;
                        string sErrorMessage = String.Empty;

                        foreach (string sSubProjectNo in lSubProjects)
                        {
                            iSubProjectCount++;
                            bSubProjectOK = false;
                            LoggingStatus(Color.White, "Downloading Sub Project (" + iSubProjectCount.ToString() + " of " + lSubProjects.Count() + ") - " + sSubProjectNo);
                            //do
                            //{
                                bErrorOccured = false;
                                bSubProjectOK = false;
                                //DownloadSubProjectData by subprojectno
                                cAX.m_wcfClient.ReturnSubProjectDataCompleted += (sender1, e1) =>
                                {
                                    if (e1.Error != null)
                                    {

                                    }
                                    else if (e1.Cancelled == true)
                                    {

                                    }
                                    else
                                    {
                                        if (e1.Result.bSuccessfull == true)
                                        {
                                            try
                                            {
                                                ServiceExt.SubProjectData spData = new ServiceExt.SubProjectData();
                                                spData = e1.Result.pdSubProjectData;
                                                if (spData.ProjId == null)
                                                {
                                                    LoggingStatus(Color.Red, "No data returned for sub project.");
                                                }
                                                else
                                                {
                                                    cDataAccess.SaveSubProjectDataResult srSaved;
                                                    srSaved = cMain.p_cDataAccess.SaveSubProjectData(txtProjectNo.Text, txtProjectName.Text, spData);
                                                    if (srSaved.bSavedOK == false)
                                                    {
                                                        LoggingStatus(Color.Red, "Cannot save sub project to database.");
                                                    }
                                                    else
                                                    {
                                                        cAX.m_wcfClient.ReturnSubProjectFilesCompleted += async (sender2, e2) =>
                                                        {
                                                            if (e2.Error != null)
                                                            {

                                                            }
                                                            else if (e2.Cancelled == true)
                                                            {

                                                            }
                                                            else
                                                            {
                                                                if (e2.Result.bSuccessfull == true)
                                                                {
                                                                    List<ServiceExt.SubProjectFile> sfFileList = e2.Result.pdSubProjectFiles.ToList();
                                                                    if (sfFileList == null)
                                                                    {
                                                                        LoggingStatus(Color.Red, "Cannot retrieve list of files.");
                                                                    }
                                                                    else if (sfFileList.Count() == 0)
                                                                    {
                                                                        LoggingStatus(Color.Red, "Has no list of files. - SubProject No (" + spData.ProjId + ")");
                                                                    }
                                                                    else
                                                                    {
                                                                        if (sfFileList.Count() > 0)
                                                                        {
                                                                        //Create- return local sub project image folder.
                                                                        bool bStorageFolder = await DependencyService.Get<ISettings>().ReturnSubProjectImagesFolder(sSubProjectNo);
                                                                            if (bStorageFolder == false)
                                                                            {
                                                                                LoggingStatus(Color.Red, "Unable to retrieve local image folder.");
                                                                            }
                                                                            else
                                                                            {
                                                                                foreach (ServiceExt.SubProjectFile sfFile in sfFileList)
                                                                                {
                                                                                    cAX.m_wcfClient.ReturnSubProjectFileDownloadCompleted += async (sender3, e3) =>
                                                                                    {
                                                                                        if (e3.Error != null)
                                                                                        {

                                                                                        }
                                                                                        else if (e3.Cancelled == true)
                                                                                        {

                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (e3.Result != null)
                                                                                            {
                                                                                                bool bFileSaved = await DependencyService.Get<ISettings>().SaveFileLocally(sSubProjectNo, e3.Result.byFileData, sfFile.FileName);
                                                                                                if (bFileSaved == false)
                                                                                                {
                                                                                                    LoggingStatus(Color.Red, "Unable to save file to device (" + sfFile.FileName + ")");
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    bool bSavesOK = cMain.p_cDataAccess.SaveSubProjectFile(sSubProjectNo, sfFile.FileName, sfFile.Comments, sfFile.ModifiedDate, false);
                                                                                                    if (bSavesOK == false)
                                                                                                    {
                                                                                                        LoggingStatus(Color.Red, "Unable to save file details to database (" + sfFile.FileName + ")");
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                LoggingStatus(Color.Red, "Unable to download file data for (" + sfFile.FileName + ")");
                                                                                            }

                                                                                        }
                                                                                    };
                                                                                    cAX.m_wcfClient.ReturnSubProjectFileDownloadAsync(sfFile.FileName, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);
                                                                                }
                                                                            }

                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            //throw new NotImplementedException();
                                                        };
                                                        cAX.m_wcfClient.ReturnSubProjectFilesAsync(cAX.m_cCompanyName, sSubProjectNo, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);
                                                    }
                                                }
                                                bSubProjectOK = true;
                                            }
                                            catch (Exception ex)
                                            {
                                                bErrorOccured = true;
                                                sErrorMessage = ex.Message;
                                            }
                                            if (bErrorOccured == true)
                                            {
                                                //await DisplayActionSheet();
                                            }
                                        }
                                        else
                                        {
                                            //throw new Exception(ex.Message + " - SubProjectNo(" + v_sSubProjectNo + ")");
                                        }
                                    }
                                };
                                cAX.m_wcfClient.ReturnSubProjectDataAsync(cAX.m_cCompanyName, sSubProjectNo, cAX.m_sPurpose, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);
                            //}
                            //while (bSubProjectOK == false);
                        }
                    }
                    
                }
            }
        }

        private void M_wcfClient_ReturnSubProjectDataCompleted(object sender, ServiceExt.ReturnSubProjectDataCompletedEventArgs e)
        {
            if (e.Error != null)
            {

            }
            else if (e.Cancelled == true)
            {

            }
            else
            {
                if (e.Result.bSuccessfull == true)
                {
                    ServiceExt.SubProjectData spData = new ServiceExt.SubProjectData();
                    spData = e.Result.pdSubProjectData;
                    if (spData.ProjId == null)
                    {
                        LoggingStatus(Color.Red, "No data returned for sub project.");
                    }
                    else
                    {
                        cDataAccess.SaveSubProjectDataResult srSaved;
                        srSaved = cMain.p_cDataAccess.SaveSubProjectData(txtProjectNo.Text, txtProjectName.Text, spData);
                        if (srSaved.bSavedOK == false)
                        {
                            LoggingStatus(Color.Red, "Cannot save sub project to database.");
                        }
                        else
                        {
                            cAX.m_wcfClient.ReturnSubProjectFilesCompleted += M_wcfClient_ReturnSubProjectFilesCompleted;
                            cAX.m_wcfClient.ReturnSubProjectFilesAsync(cAX.m_cCompanyName, spData.ProjId, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);
                        }
                    }
                }
                else
                {
                    //throw new Exception(ex.Message + " - SubProjectNo(" + v_sSubProjectNo + ")");
                }
            }
            //throw new NotImplementedException();
        }

        private void M_wcfClient_ReturnSubProjectFilesCompleted(object sender, ServiceExt.ReturnSubProjectFilesCompletedEventArgs e)
        {
            if (e.Error != null)
            {

            }
            else if (e.Cancelled == true){

            }
            else
            {
                if (e.Result.bSuccessfull == true)
                {
                    List<ServiceExt.SubProjectFile> sfFileList = new List<ServiceExt.SubProjectFile>();
                    sfFileList = new List<ServiceExt.SubProjectFile>(e.Result.pdSubProjectFiles);
                    if (sfFileList == null)
                    {
                        LoggingStatus(Color.Red, "Cannot retrieve list of files.");
                    }
                    else
                    {
                        if (sfFileList.Count() > 0)
                        {
                            //Create- return local sub project image folder.
                            //bool bStorageFolder = DependencyService.Get<ISettings>().ReturnSubProjectImagesFolder();
                            
                        }
                    }
                }
            }
            //throw new NotImplementedException();
        }

        private bool IsProjectAlreadyOnDevice(string v_sProjectNo)
        {
            try
            {
                cProjectTable cProject = cMain.p_cDataAccess.IsProjectAlreadyDownloaded(v_sProjectNo);
                if (cProject != null)
                {

                    StringBuilder sbMsg = new StringBuilder();
                    sbMsg.Append("This project has already been downloaded onto your device:");
                    sbMsg.Append(Environment.NewLine);
                    sbMsg.Append(Environment.NewLine);
                    sbMsg.Append("Project No: " + v_sProjectNo);
                    sbMsg.Append(Environment.NewLine);
                    sbMsg.Append("Project Name: " + cProject.ProjectName);
                    sbMsg.Append(Environment.NewLine);
                    sbMsg.Append(Environment.NewLine);
                    sbMsg.Append("Any changes to the project or sub projects will be applied in the syncing process.");
                    Log log = new Log() { Color = Color.Green, Status = sbMsg.ToString() };
                    logs.Add(log);
                    //DisplayAlert("Project Already Downloaded", sbMsg.ToString(), "OK");

                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
