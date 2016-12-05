using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Anglian.Service;
using Anglian.Classes;

namespace Anglian.Engine
{
    public class WcfExt116
    {
        public string m_cCompanyName = String.Empty;

        /// <summary>
        /// Purpose setting
        /// </summary>
        public string m_sPurpose = String.Empty;
        public WcfExt116()
        {
            m_cCompanyName = Settings.p_sSetting_AXCompany;
            m_sPurpose = Settings.ReturnPurposeType();
        }
        /// <summary>
        /// Check if we are connected to AX
        /// </summary>
        /// <param name="v_bShowMessage"></param>
        /// <returns></returns>
        /// 
        public async Task<SystemsAvailableResult> AreWeConnectedToAX()
        {

            SystemsAvailableResult rResult = null;

            try
            {

                if (DependencyService.Get<ISettings>().AreWeOnline() == true)
                {

                    string sUserProfile = await DependencyService.Get<ISettings>().GetUserName();
                    if (sUserProfile.Trim() == string.Empty || sUserProfile.Trim() == "")
                        sUserProfile = Session.CurrentUserName;

                    rResult = await DependencyService.Get<IWcfExt116>().ReturnAreSystemsAvailableAsync(
                        m_cCompanyName, 
                        sUserProfile, 
                        Settings.p_sSetting_AuthID, 
                        Session.Token);


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
        /// v1.0.1 - Return AX setting updates
        /// </summary>
        /// <param name="beFields"></param>
        /// <returns></returns>
        public async Task<List<SettingDetails>> ReturnUpdatedSettings(List<SettingDetails> v_sdSettings)
        {

            List<SettingDetails> sdSettings = null;

            try
            {

                //WCF call requires observable collection instead of array.
                ObservableCollection<SettingDetails> ocSettings = new ObservableCollection<SettingDetails>(v_sdSettings);

                //Call function
                SettingsCheckResult scResult = await DependencyService.Get<IWcfExt116>().CheckForUpdatedSettingsAsync(
                    m_cCompanyName, 
                    ocSettings, 
                    Settings.p_sSetting_AuthID,
                    Session.Token);

                if (scResult != null)
                {
                    if (scResult.bSuccessfull == true)
                    {

                        sdSettings = new List<SettingDetails>(scResult.Settings);

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
        public async Task<List<BaseEnumField>> ReturnUpdatedBaseEnums(List<BaseEnumField> v_beFields)
        {

            List<BaseEnumField> beFields = null;

            try
            {

                //WCF call requires observable collection instead of array.
                ObservableCollection<BaseEnumField> ocFields = new ObservableCollection<BaseEnumField>(v_beFields);

                //Call function
                BaseEnumResult beResult = await DependencyService.Get<IWcfExt116>().ReturnBaseEnumsAsync(
                    m_cCompanyName, 
                    ocFields, 
                    Settings.p_sSetting_AuthID,
                    Session.Token);

                if (beResult != null)
                {
                    if (beResult.bSuccessfull == true)
                    {

                        beFields = new List<BaseEnumField>(beResult.BaseEnumResults);

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
        /// Search for project.
        /// </summary>
        /// <param name="v_sProjectName"></param>
        /// <returns></returns>
        public async Task<ObservableCollection<SearchResult>> SearchForProject(string v_sProjectName)
        {


            ProjectSearchResult srProject = null;
            ObservableCollection<SearchResult> oResults = new ObservableCollection<SearchResult>();

            try
            {

                srProject = await DependencyService.Get<IWcfExt116>().SearchForContractAsync(
                    m_cCompanyName, 
                    v_sProjectName,
                    Settings.p_sSetting_AuthID,
                    Session.Token);
                if (srProject != null)
                {
                    if (srProject.bSuccessfull == true)
                    {
                        oResults = srProject.SearchResults;

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
        public async Task<ProjectValidationResult> ValidateProjectNo(string v_sProjectNo)
        {

            //ProjectValidationResult cResult = null;

            try
            {

                return await DependencyService.Get<IWcfExt116>().ValidateProjectAsync(
                    this.m_cCompanyName, 
                    v_sProjectNo, 
                    Settings.p_sSetting_AuthID,
                    Session.Token);

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
        public async Task<SubProjectData> DownloadSubProjectData(string v_sSubProjectNo)
        {

            SubProjectDataResult spDataResult = null;
            SubProjectData spData = new SubProjectData();
            try
            {

                spDataResult = await DependencyService.Get<IWcfExt116>().ReturnSubProjectDataAsync(
                    this.m_cCompanyName, 
                    v_sSubProjectNo, 
                    this.m_sPurpose, 
                    Settings.p_sSetting_AuthID,
                    Session.Token);
                if (spDataResult != null)
                {
                    if (spDataResult.bSuccessfull == true)
                    {

                        spData = spDataResult.pdSubProjectData;

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
        /// Upload sub project changes to AX
        /// </summary>
        /// <returns></returns>
        public async Task<UploadChangesResult> UploadSubProjectChanges(
            string v_sUserName, 
            string v_sMachineName, 
            string v_sSubProjectNo, 
            ObservableCollection<AXDataUploadDataChange> v_uChanges)
        {

            try
            {

                return await DependencyService.Get<IWcfExt116>().UploadSubProjectDataChangesAsync(
                    this.m_cCompanyName, 
                    this.m_sPurpose, 
                    v_sUserName, 
                    v_sMachineName, 
                    v_sSubProjectNo, 
                    v_uChanges, 
                    Settings.p_sSetting_AuthID,
                    Session.Token);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - UserName(" + v_sUserName + "),MachineName(" + v_sMachineName + "),SubProjectNo(" + v_sSubProjectNo + ")");

            }
        }

        public async Task<List<SubProjectFile>> ReturnListOfSubProjectFiles(string v_sSubProjectNo)
        {

            SubProjectFilesResult sfResult = null;
            List<SubProjectFile> sfFileList = new List<SubProjectFile>();

            try
            {

                sfResult = await DependencyService.Get<IWcfExt116>().ReturnSubProjectFilesAsync(
                    this.m_cCompanyName, 
                    v_sSubProjectNo, 
                    Settings.p_sSetting_AuthID,
                    Session.Token);
                if (sfResult != null)
                {

                    if (sfResult.bSuccessfull == true)
                    {

                        sfFileList = new List<SubProjectFile>(sfResult.pdSubProjectFiles);

                    }

                }

                return sfFileList;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SubProjectNo(" + v_sSubProjectNo + ")");

            }

        }
        public async Task<UploadChangesResult> UploadSubProjectNotes(
            string v_sUserName, 
            string v_sMachineName, 
            string v_sSubProjectNo, 
            ObservableCollection<NoteDetails> v_uNotes)
        {

            try
            {

                return await DependencyService.Get<IWcfExt116>().UploadSubProjectNotesChangesAsync(
                    this.m_cCompanyName, 
                    this.m_sPurpose, 
                    v_sUserName, 
                    v_sMachineName, 
                    v_sSubProjectNo, 
                    v_uNotes, 
                    Settings.p_sSetting_AuthID,
                    Session.Token);

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
        public async Task<UploadChangesResult> UploadFile(
            string v_sSubProjectNo, 
            string v_sUserName, 
            string v_sMachineName, 
            UploadFileChange v_cFileUpload)
        {

            UploadChangesResult cUResults = null;
            try
            {

                ObservableCollection<UploadFileChange> cUploads = new ObservableCollection<UploadFileChange>();
                cUploads.Add(v_cFileUpload);

                //v1.0.3 - Use new service, new parameters user name and machine name.
                cUResults = await DependencyService.Get<IWcfExt116>().UploadSubProjectFilesAsync(
                    this.m_cCompanyName, 
                    v_sSubProjectNo, 
                    v_sUserName, 
                    v_sMachineName, 
                    cUploads, 
                    Settings.p_sSetting_AuthID,
                    Session.Token);


                return cUResults;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SubProjectNo(" + v_sSubProjectNo + ")");

            }

        }
        /// <summary>
        /// v1.0.1 - Check for AX file changes.
        /// </summary>
        /// <param name="v_sfFileDetails"></param>
        /// <returns></returns>
        public async Task<ObservableCollection<SubProjectFileDetail>> CheckForAXFileChanges(
            string v_sProjectNo, 
            string v_sSubProjectNo, 
            ObservableCollection<SubProjectFileDetail> v_sfFileDetails)
        {

            FileChangesResult rResult = null;
            try
            {

                rResult = await DependencyService.Get<IWcfExt116>().ReturnSubProjectFileChangesAsync(
                    this.m_cCompanyName, 
                    v_sProjectNo, 
                    v_sSubProjectNo, 
                    v_sfFileDetails, 
                    Settings.p_sSetting_AuthID,
                    Session.Token);
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
        /// Check for changes on AX, return sub projects that have changed.
        /// </summary>
        /// <param name="v_cSubProjects"></param>
        /// <returns></returns>
        public async Task<DownloadDataChangesResult> CheckForAXDataChanges(
            ObservableCollection<DownloadDataChange> v_cSubProjects, 
            bool v_bCheckForNewSubProjects)
        {

            try
            {
                return await DependencyService.Get<IWcfExt116>().CheckForDataDownloadChangesAsync(
                    this.m_cCompanyName, 
                    this.m_sPurpose, 
                    v_cSubProjects, 
                    v_bCheckForNewSubProjects, 
                    Settings.p_sSetting_AuthID,
                    Session.Token);

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
        public async Task<bool> UploadUnitStatusChanges(string v_sSubProjectNo, string v_sUserProfile, string v_sMachineName, DateTime v_dInstallationDate, string v_sInstallationTeam, ObservableCollection<UnitDetails> v_udUnitDetails)
        {

            UploadUnitsResult rResult = null;
            try
            {

                rResult = await DependencyService.Get<IWcfExt116>().UploadUnitInstallationStatusAsync(
                    this.m_cCompanyName, 
                    v_sSubProjectNo, 
                    v_sUserProfile, 
                    v_sMachineName, 
                    v_dInstallationDate, 
                    v_sInstallationTeam, 
                    v_udUnitDetails, 
                    Settings.p_sSetting_AuthID, 
                    Session.Token);
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

        public async Task<SubProjectFileDownloadResult> ReturnFileData(string v_sFileName)
        {

            SubProjectFileDownloadResult sfResult = null;

            try
            {

                sfResult = await DependencyService.Get<IWcfExt116>().ReturnSubProjectFileDownloadAsync(
                    v_sFileName, 
                    Settings.p_sSetting_AuthID,
                    Session.Token);
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
        /// 
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <returns></returns>
        public async Task<List<String>> ReturnListOfSubProjectsToDownload(string v_sProjectNo)
        {


            SubProjectsListResult spList = null;

            try
            {

                spList = await DependencyService.Get<IWcfExt116>().ReturnSubProjectsListAsync(
                    this.m_cCompanyName, 
                    v_sProjectNo, 
                    this.m_sPurpose, 
                    Settings.p_sSetting_AuthID,
                    Session.Token);
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
    }
}
