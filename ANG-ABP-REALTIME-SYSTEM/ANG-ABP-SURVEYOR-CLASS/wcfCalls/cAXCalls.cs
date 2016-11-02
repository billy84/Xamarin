using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ANG_ABP_SURVEYOR_APP_CLASS;

namespace ANG_ABP_SURVEYOR_APP_CLASS.wcfCalls
{
    public class cAXCalls
    {
        /// <summary>
        /// WCF Client object.
        /// </summary>
        private wcfAX.ServiceClient m_wcfClient = null;

        /// <summary>
        /// Company name.
        /// </summary>
        private string m_cCompanyName = String.Empty;

        /// <summary>
        /// Purpose setting
        /// </summary>
        private string m_sPurpose = String.Empty;

        /// <summary>
        /// Constructor
        /// </summary>
        public cAXCalls()
        {

            try
            {

                //v1.0.11 - Fetch URL to use depending on running mode.
                string sWCF_URL = this.ReturnWCF_URL();
                
                this.m_wcfClient = new wcfAX.ServiceClient();

                //v1.0.18 - Return timeout value
                int iTimeoutInMins = this.ReturnWSTimeoutVal();


                //Set timeout
                this.m_wcfClient.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, iTimeoutInMins, 0);
                this.m_wcfClient.Endpoint.Binding.SendTimeout = new TimeSpan(0, iTimeoutInMins, 0);

                this.m_wcfClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(sWCF_URL); //v1.0.11 - Update end point address.
                this.m_cCompanyName = cSettings.p_sSetting_AXCompany;

                this.m_sPurpose = cSettings.ReturnPurposeType();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// v1.0.19 - Return web service timeout value.
        /// </summary>
        /// <returns></returns>
        private int ReturnWSTimeoutVal()
        {

            int iTimeOutVal = 59;
            try
            {
                string sVal = cSettings.FetchSettingValue("Webservice_Timeout_InMins");
                if (int.TryParse(sVal, out iTimeOutVal) == false)
                {
                    iTimeOutVal = 59;
                }

                return iTimeOutVal;
           
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        /// <summary>
        /// v1.0.11 - Return the URL we should use.
        /// </summary>
        /// <returns></returns>
        private string ReturnWCF_URL()
        {

            string sURL = string.Empty;
            try
            {

                cDataAccess cData = new cDataAccess();                
                if (cData.AreWeRunningInLive() == true)
                {
                    //sURL = "https://fclext.anglian-windows.com/ax-surveyor-service-ext/service.svc";
                    sURL = "https://fclext.anglian-windows.com/ax-surv-service-ext-115/Service.svc"; //v1.0.15

                }
                else
                {
                    sURL = "https://abpwebtest.anglian-windows.com/ax-surveyor-service-ext-105/service.svc";

                }
                cData = null;

                return sURL;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// Close AX connection
        /// </summary>
        /// <returns></returns>
        public async Task CloseAXConnection()
        {

            try
            {

                if (this.m_wcfClient != null)
                {
                    await this.m_wcfClient.CloseAsync();
                    this.m_wcfClient = null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// Check if we are connected to AX
        /// </summary>
        /// <param name="v_bShowMessage"></param>
        /// <returns></returns>
        public async Task<wcfAX.SystemsAvailableResult> AreWeConnectedToAX()
        {

            wcfAX.SystemsAvailableResult rResult = null;

            try
            {

                if (cSettings.AreWeOnline == true)
                {

                    string sUserProfile = await cSettings.GetUserName();
                   
                    rResult = await this.m_wcfClient.ReturnAreSystemsAvailableAsync(this.m_cCompanyName, sUserProfile, cSettings.p_sSetting_AuthID);

             
                    if (rResult != null)
                    {
                        if (rResult.bSuccessfull == true)
                        {
                            return rResult;

                        }

                    }

                }

                return rResult;

            }
            catch (Exception ex)
            {

                string sVal = ex.Message;
                return rResult;

            }

        }


        /// <summary>
        /// Search for project.
        /// </summary>
        /// <param name="v_sProjectName"></param>
        /// <returns></returns>
        public async Task<ObservableCollection<wcfAX.SearchResult>> SearchForProject(string v_sProjectName)
        {


            wcfAX.ProjectSearchResult srProject = null;
            ObservableCollection<wcfAX.SearchResult> oResults = new ObservableCollection<wcfAX.SearchResult>();

            try
            {

                srProject = await this.m_wcfClient.SearchForContractAsync(this.m_cCompanyName, v_sProjectName, cSettings.p_sSetting_AuthID);
                if (srProject != null)
                {
                    if (srProject.bSuccessfull == true)
                    {
                        oResults =  srProject.SearchResults;

                    }

                }

                return oResults;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - ProjectName(" + v_sProjectName + ")");

            }

        }


        /// <summary>
        /// Validate project number
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <returns></returns>
        public async Task<wcfAX.ProjectValidationResult> ValidateProjectNo(string v_sProjectNo)
        {

            wcfAX.ProjectValidationResult cResult = null;

            try
            {

                return await this.m_wcfClient.ValidateProjectAsync(this.m_cCompanyName, v_sProjectNo, cSettings.p_sSetting_AuthID);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - ProjectNo(" + v_sProjectNo + ")");

            }

        }


        /// <summary>
        /// v1.0.1 - Return AX setting updates
        /// </summary>
        /// <param name="beFields"></param>
        /// <returns></returns>
        public async Task<List<wcfAX.SettingDetails>> ReturnUpdatedSettings(List<wcfAX.SettingDetails> v_sdSettings)
        {

            List<wcfAX.SettingDetails> sdSettings = null;

            try
            {

                //WCF call requires observable collection instead of array.
                ObservableCollection<wcfAX.SettingDetails> ocSettings = new ObservableCollection<wcfAX.SettingDetails>(v_sdSettings);

                //Call function
                wcfAX.SettingsCheckResult scResult = await this.m_wcfClient.CheckForUpdatedSettingsAsync(this.m_cCompanyName, ocSettings, cSettings.p_sSetting_AuthID);

                if (scResult != null)
                {
                    if (scResult.bSuccessfull == true)
                    {

                        sdSettings = new List<wcfAX.SettingDetails>(scResult.Settings);

                    }

                }

                return sdSettings;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// Return base enum
        /// </summary>
        /// <param name="beFields"></param>
        /// <returns></returns>
        public async Task<List<wcfAX.BaseEnumField>> ReturnUpdatedBaseEnums(List<wcfAX.BaseEnumField> v_beFields)
        {

            List<wcfAX.BaseEnumField> beFields = null;

            try
            {

                //WCF call requires observable collection instead of array.
                ObservableCollection<wcfAX.BaseEnumField> ocFields = new ObservableCollection<wcfAX.BaseEnumField>(v_beFields);

                //Call function
                wcfAX.BaseEnumResult beResult = await this.m_wcfClient.ReturnBaseEnumsAsync(this.m_cCompanyName, ocFields, cSettings.p_sSetting_AuthID);

                if (beResult != null)
                {
                    if (beResult.bSuccessfull == true)
                    {

                        beFields = new List<wcfAX.BaseEnumField>(beResult.BaseEnumResults);

                    }
                                       
                }

                return beFields;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <returns></returns>
        public async Task<List<String>> ReturnListOfSubProjectsToDownload(string v_sProjectNo)
        {


            wcfAX.SubProjectsListResult spList = null;

            try
            {

                spList = await this.m_wcfClient.ReturnSubProjectsListAsync(this.m_cCompanyName, v_sProjectNo, this.m_sPurpose, cSettings.p_sSetting_AuthID);
                if (spList != null)
                {
                    if (spList.bSuccessfull == true)
                    {
                        if (spList.SubProjects != null)
                        {
                            return new List<String>(spList.SubProjects);

                        }

                    }

                }


                return null;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - ProjectNo(" + v_sProjectNo + ")");

            }

        }

        /// <summary>
        /// Download sub project.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public async Task<wcfAX.SubProjectData> DownloadSubProjectData(string v_sSubProjectNo)
        {

            wcfAX.SubProjectDataResult spDataResult = null;
            wcfAX.SubProjectData spData = new wcfAX.SubProjectData();
            try
            {

                spDataResult = await this.m_wcfClient.ReturnSubProjectDataAsync(this.m_cCompanyName, v_sSubProjectNo, this.m_sPurpose, cSettings.p_sSetting_AuthID);
                if (spDataResult != null)
                {
                    if (spDataResult.bSuccessfull == true)
                    {

                        spData =  spDataResult.pdSubProjectData;

                    }

                }

                return spData;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SubProjectNo(" + v_sSubProjectNo + ")");

            }

        }

        /// <summary>
        /// Return list of sub project files.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public async Task<List<wcfAX.SubProjectFile>> ReturnListOfSubProjectFiles(string v_sSubProjectNo)
        {

            wcfAX.SubProjectFilesResult sfResult = null;
            List<wcfAX.SubProjectFile> sfFileList = new List<wcfAX.SubProjectFile>();

            try
            {

                sfResult = await this.m_wcfClient.ReturnSubProjectFilesAsync(this.m_cCompanyName, v_sSubProjectNo, cSettings.p_sSetting_AuthID);
                if (sfResult != null)
                {

                    if (sfResult.bSuccessfull == true)
                    {

                        sfFileList = new List<wcfAX.SubProjectFile>(sfResult.pdSubProjectFiles);

                    }

                }

                return sfFileList;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SubProjectNo(" + v_sSubProjectNo + ")");

            }

        }

        /// <summary>
        /// Return file data.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <param name="v_sFileName"></param>
        /// <returns></returns>
        public async Task<wcfAX.SubProjectFileDownloadResult> ReturnFileData(string v_sFileName)
        {

            wcfAX.SubProjectFileDownloadResult sfResult = null;

            try
            {

                sfResult = await this.m_wcfClient.ReturnSubProjectFileDownloadAsync(v_sFileName, cSettings.p_sSetting_AuthID);
                if (sfResult != null)
                {

                    if (sfResult.bSuccessfull == true)
                    {

                        if (sfResult.bFileFound == true)
                        {
                            return sfResult;

                        }
                       
                    }

                }

                return null;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - FileName(" + v_sFileName + ")");

            }

        }

        /// <summary>
        /// Upload sub project changes to AX
        /// </summary>
        /// <returns></returns>
        public async Task<wcfAX.UploadChangesResult> UploadSubProjectChanges(string v_sUserName, string v_sMachineName, string v_sSubProjectNo, ObservableCollection<wcfAX.cAXDataUploadDataChange> v_uChanges)
        {
           
            try
            {

                return await this.m_wcfClient.UploadSubProjectDataChangesAsync(this.m_cCompanyName, this.m_sPurpose, v_sUserName, v_sMachineName, v_sSubProjectNo, v_uChanges, cSettings.p_sSetting_AuthID);
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - UserName(" + v_sUserName + "),MachineName(" + v_sMachineName + "),SubProjectNo(" + v_sSubProjectNo + ")");

            }
        }

        /// <summary>
        /// v1.0.1 - Upload sub project notes to AX
        /// </summary>
        /// <returns></returns>
        public async Task<wcfAX.UploadChangesResult> UploadSubProjectNotes(string v_sUserName, string v_sMachineName, string v_sSubProjectNo, ObservableCollection<wcfAX.NoteDetails> v_uNotes)
        {

            try
            {

                return await this.m_wcfClient.UploadSubProjectNotesChangesAsync(this.m_cCompanyName, this.m_sPurpose, v_sUserName, v_sMachineName, v_sSubProjectNo, v_uNotes, cSettings.p_sSetting_AuthID);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - UserName(" + v_sUserName + "),MachineName(" + v_sMachineName + "),SubProjectNo(" + v_sSubProjectNo + ")");

            }
        }

        /// <summary>
        /// Upload file to AX.
        /// </summary>
        /// <returns></returns>
        public async Task<wcfAX.UploadChangesResult> UploadFile(string v_sSubProjectNo, string v_sUserName, string v_sMachineName, wcfAX.UploadFileChange v_cFileUpload)
        {

            wcfAX.UploadChangesResult cUResults = null;
            try
            {

                ObservableCollection<wcfAX.UploadFileChange> cUploads = new ObservableCollection<wcfAX.UploadFileChange>();
                cUploads.Add(v_cFileUpload);

                //v1.0.3 - Use new service, new parameters user name and machine name.
                cUResults = await this.m_wcfClient.UploadSubProjectFilesAsync(this.m_cCompanyName, v_sSubProjectNo, v_sUserName, v_sMachineName, cUploads, cSettings.p_sSetting_AuthID);
               

                return cUResults;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SubProjectNo(" + v_sSubProjectNo + ")");

            }

        }

        /// <summary>
        /// Check for changes on AX, return sub projects that have changed.
        /// </summary>
        /// <param name="v_cSubProjects"></param>
        /// <returns></returns>
        public async Task<wcfAX.DownloadDataChangesResult> CheckForAXDataChanges(ObservableCollection<wcfAX.DownloadDataChange> v_cSubProjects, bool v_bCheckForNewSubProjects)
        {

            try
            {
                return await this.m_wcfClient.CheckForDataDownloadChangesAsync(this.m_cCompanyName, this.m_sPurpose, v_cSubProjects, v_bCheckForNewSubProjects, cSettings.p_sSetting_AuthID);
                
            }
             catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// v1.0.1 - Check for AX file changes.
        /// </summary>
        /// <param name="v_sfFileDetails"></param>
        /// <returns></returns>
        public async Task<ObservableCollection<wcfAX.SubProjectFileDetail>> CheckForAXFileChanges(string v_sProjectNo, string v_sSubProjectNo, ObservableCollection<wcfAX.SubProjectFileDetail> v_sfFileDetails)
        {

            wcfAX.FileChangesResult rResult = null;
            try
            {

                rResult = await this.m_wcfClient.ReturnSubProjectFileChangesAsync(this.m_cCompanyName, v_sProjectNo, v_sSubProjectNo, v_sfFileDetails, cSettings.p_sSetting_AuthID);
                if (rResult != null)
                {
                    if (rResult.bSuccessfull == true)
                    {

                        return rResult.sfChanges;

                    }

                }

                return null;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - ProjectNo(" + v_sProjectNo + "),SubProjectNo(" + v_sSubProjectNo + ")");

            }

        }

        /// <summary>
        /// v1.0.21 - Fetch updated failed survey reasons
        /// </summary>
        /// <param name="v_sfFileDetails"></param>
        /// <returns></returns>
        public async Task<wcfAX.FetchSurveyFailedReasonsResult> FetchFailedSurveyReasons(DateTime v_dLastUpdate)
        {

            try
            {

                return await this.m_wcfClient.FetchFailedSurveyReasonsAsync(this.m_cCompanyName, v_dLastUpdate, cSettings.p_sSetting_AuthID);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }


        /// <summary>
        /// v1.0.8 - Fetch list of installers.
        /// </summary>
        /// <param name="v_sfFileDetails"></param>
        /// <returns></returns>
        public async Task<wcfAX.FetchInstallersResult> FetchInstallersList(DateTime v_dLastUpdate)
        {
            
            try
            {

                return await this.m_wcfClient.FetchInstallersListAsync(this.m_cCompanyName,v_dLastUpdate, cSettings.p_sSetting_AuthID);
               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }


        /// <summary>
        /// v1.0.8 - Fetch list of installers projects
        /// </summary>
        /// <param name="v_sfFileDetails"></param>
        /// <returns></returns>
        public async Task<ObservableCollection<wcfAX.SearchResult>> FetchInstallersProjects()
        {

            try
            {

                string sInstallersProfile = await cSettings.GetUserName();

                wcfAX.ProjectSearchResult srResult = null;
                srResult = await this.m_wcfClient.FetchInstallersProjectsListAsync(this.m_cCompanyName, sInstallersProfile, cSettings.p_sSetting_AuthID);

                if (srResult != null)
                {

                    if (srResult.bSuccessfull == true)
                    {

                        return srResult.SearchResults;

                    }

                }

                return null;
           
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }


        /// <summary>
        /// v1.0.10 - Upload unit status changes.
        /// </summary>
        /// <param name="v_sfFileDetails"></param>
        /// <returns></returns>
        public async Task<bool> UploadUnitStatusChanges(string v_sSubProjectNo,string v_sUserProfile,string v_sMachineName,DateTime v_dInstallationDate,string v_sInstallationTeam, ObservableCollection<wcfAX.UnitDetails> v_udUnitDetails)
        {

            wcfAX.UploadUnitsResult rResult = null;
            try
            {

                rResult = await this.m_wcfClient.UploadUnitInstallationStatusAsync(this.m_cCompanyName, v_sSubProjectNo, v_sUserProfile, v_sMachineName, v_dInstallationDate,v_sInstallationTeam, v_udUnitDetails, cSettings.p_sSetting_AuthID);
                if (rResult != null)
                {
                    return rResult.bSuccessfull;

                }

                return false;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SubProjectNo(" + v_sSubProjectNo + ")");

            }

        }

    }

}
