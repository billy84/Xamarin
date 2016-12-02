using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Classes;
using System.Collections.ObjectModel;

namespace Anglian.Service
{
    public interface IWcfExt116
    {
        Task CloseAXConnection();
        Task<FetchSurveyFailedReasonsResult> FetchFailedSurveyReasons(
            string v_sCompanyName,
            DateTime v_dLastUpdate,
            string v_sAuthID,
            string v_sToken);
        Task<BaseEnumResult> ReturnBaseEnumsAsync(
            string v_sCompanyName,
            ObservableCollection<BaseEnumField> v_beFields,
            string v_sAuthID,
            string v_sToken);
        Task<SystemsAvailableResult> ReturnAreSystemsAvailableAsync(
            string v_sCompanyName,
            string v_sUserProfile,
            string v_sAuthID,
            string v_sToken);
        Task<SettingsCheckResult> CheckForUpdatedSettingsAsync(
            string v_sCompanyName, 
            ObservableCollection<SettingDetails> v_svSettings, 
            string v_sAuthID, 
            string v_sToken);

        Task<ProjectSearchResult> SearchForContractAsync(
            string v_sCompanyName,
            string v_sProjectName,
            string v_sAuthID,
            string v_sToken);
    }
}
