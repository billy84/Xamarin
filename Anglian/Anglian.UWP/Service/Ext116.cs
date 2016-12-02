using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.UWP.Service;
using Xamarin.Forms;
using Anglian.Service;
using Anglian.Classes;
[assembly: Dependency(typeof(Ext116))]
namespace Anglian.UWP.Service
{
    class Ext116 : IWcfExt116
    {
        private WcfExt116.ServiceClient m_wcfClient = new WcfExt116.ServiceClient();
        public async Task<FetchSurveyFailedReasonsResult> FetchFailedSurveyReasons(
            string v_sCompanyName, 
            DateTime v_dLastUpdate,
            string v_sAuthID,
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            FetchSurveyFailedReasonsResult result = new FetchSurveyFailedReasonsResult();
            try
            {
                WcfExt116.FetchSurveyFailedReasonsResult sResult = await m_wcfClient.FetchFailedSurveyReasonsAsync(
                    v_sCompanyName,
                    v_dLastUpdate,
                    v_sAuthID,
                    v_sToken);
                if (sResult.bSuccessfull == true)
                {
                    result.bSuccessfull = sResult.bSuccessfull;
                    result.bLastUpdateDate = sResult.bLastUpdateDate;
                    result.sfrReasons = new ObservableCollection<SurveyFailedReason>();
                    foreach (WcfExt116.SurveyFailedReason o in sResult.sfrReasons)
                    {
                        SurveyFailedReason oSFR = new SurveyFailedReason();
                        oSFR.bMandatoryNote = o.bMandatoryNote;
                        oSFR.iDisplayOrder = o.iDisplayOrder;
                        oSFR.iProgressStatus = o.iProgressStatus;
                        oSFR.sReason = o.sReason;
                        result.sfrReasons.Add(oSFR);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //return null;
            }
        }
        public async Task<BaseEnumResult> ReturnBaseEnumsAsync(
            string v_sCompanyName, 
            ObservableCollection<BaseEnumField> v_beFields, 
            string v_sAuthID, 
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            BaseEnumResult result = new BaseEnumResult();
            try
            {
                ObservableCollection<WcfExt116.BaseEnumField> lSettingDetails = new ObservableCollection<WcfExt116.BaseEnumField>();
                foreach (BaseEnumField sd in v_beFields)
                {
                    WcfExt116.BaseEnumField oSD = new WcfExt116.BaseEnumField();
                    oSD.LastUpdate = sd.LastUpdate;
                    foreach (BaseEnumValue o in sd.BaseEnums)
                    {
                        WcfExt116.BaseEnumValue oBEV = new WcfExt116.BaseEnumValue();
                        oBEV.BaseName = o.BaseName;
                        oBEV.BaseValue = o.BaseValue;
                        oSD.BaseEnums.Add(oBEV);
                    }
                    oSD.FieldName = sd.FieldName;
                    oSD.TableName = sd.TableName;
                    lSettingDetails.Add(oSD);
                }
                WcfExt116.BaseEnumResult sResult = await m_wcfClient.ReturnBaseEnumsAsync(
                    v_sCompanyName,
                    lSettingDetails,
                    v_sAuthID,
                    v_sToken);
                if (sResult.bSuccessfull == true)
                {
                    result.bSuccessfull = sResult.bSuccessfull;
                    result.BaseEnumResults = new ObservableCollection<BaseEnumField>();
                    foreach (WcfExt116.BaseEnumField o in sResult.BaseEnumResults)
                    {
                        BaseEnumField oSD = new BaseEnumField();
                        oSD.LastUpdate = o.LastUpdate;
                        oSD.BaseEnums = new ObservableCollection<BaseEnumValue>();
                        foreach (WcfExt116.BaseEnumValue oBEV in o.BaseEnums)
                        {
                            BaseEnumValue bEV = new BaseEnumValue();
                            bEV.BaseName = oBEV.BaseName;
                            bEV.BaseValue = oBEV.BaseValue;
                            oSD.BaseEnums.Add(bEV);
                        }
                        oSD.FieldName = o.FieldName;
                        oSD.TableName = o.TableName;
                        result.BaseEnumResults.Add(oSD);
                    }

                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<SettingsCheckResult> CheckForUpdatedSettingsAsync(
            string v_sCompanyName,
            ObservableCollection<SettingDetails> v_svSettings,
            string v_sAuthID,
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            SettingsCheckResult result = new SettingsCheckResult();
            try
            {
                ObservableCollection<WcfExt116.SettingDetails> lSettingDetails = new ObservableCollection<WcfExt116.SettingDetails>();
                foreach (SettingDetails sd in v_svSettings)
                {
                    WcfExt116.SettingDetails oSD = new WcfExt116.SettingDetails();
                    oSD.LastUpdate = sd.LastUpdate;
                    oSD.SettingName = sd.SettingName;
                    oSD.SettingValue = sd.SettingValue;
                    lSettingDetails.Add(oSD);
                }
                WcfExt116.SettingsCheckResult sResult = await m_wcfClient.CheckForUpdatedSettingsAsync(
                    v_sCompanyName,
                    lSettingDetails,
                    v_sAuthID,
                    v_sToken);
                if (sResult.bSuccessfull == true)
                {
                    result.bSuccessfull = sResult.bSuccessfull;
                    result.Settings = new ObservableCollection<SettingDetails>();
                    foreach (WcfExt116.SettingDetails o in sResult.Settings)
                    {
                        SettingDetails oSD = new SettingDetails();
                        oSD.LastUpdate = o.LastUpdate;
                        oSD.SettingName = o.SettingName;
                        oSD.SettingValue = o.SettingValue;
                        result.Settings.Add(oSD);
                    }
                    
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<SystemsAvailableResult> ReturnAreSystemsAvailableAsync(
            string v_sCompanyName, 
            string v_sUserProfile, 
            string v_sAuthID, 
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            SystemsAvailableResult result = new SystemsAvailableResult();
            try
            {
                WcfExt116.SystemsAvailableResult sResult = await m_wcfClient.ReturnAreSystemsAvailableAsync(
                    v_sCompanyName, 
                    v_sUserProfile, 
                    v_sAuthID, 
                    v_sToken);
                if (sResult.bSuccessfull == true)
                {
                    result.bSuccessfull = sResult.bSuccessfull;
                    result.SystemsAvailable = sResult.SystemsAvailable;
                    result.UserAccountOK = sResult.UserAccountOK;
                }
                return result;
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
                m_wcfClient = new WcfExt116.ServiceClient();

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
        public async Task<ProjectSearchResult> SearchForContractAsync(
            string v_sCompanyName, 
            string v_sProjectName, 
            string v_sAuthID, 
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            ProjectSearchResult result = new ProjectSearchResult();
            try
            {
                WcfExt116.ProjectSearchResult sResults = await m_wcfClient.SearchForContractAsync(
                    v_sCompanyName,
                    v_sProjectName,
                    v_sAuthID,
                    v_sToken);
                if (sResults.bSuccessfull == true)
                {
                    result.bSuccessfull = sResults.bSuccessfull;
                    result.SearchResults = new ObservableCollection<SearchResult>();
                    foreach (WcfExt116.SearchResult o in sResults.SearchResults)
                    {
                        SearchResult oSR = new SearchResult();
                        oSR.ProjectName = o.ProjectName;
                        oSR.ProjectNo = o.ProjectNo;
                        oSR.Status = o.Status;
                        result.SearchResults.Add(oSR);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
