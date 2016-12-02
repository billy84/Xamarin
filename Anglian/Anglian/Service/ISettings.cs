using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anglian.Service
{
    public interface ISettings
    {
        Task<string> GetUserName();
        string GetMachineName();
        Task<string> GetUserDisplayName();
        bool IsThisTheSurveyorApp();
        Task DisplayMessage(string v_sMessage, string v_sTitle);
        bool AreWeOnline();

        //Task<bool> SaveFileLocally(StorageFolder v_sfFolder, byte[] v_bFileData, string v_sFileName);
    }
}
