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
    }
}
