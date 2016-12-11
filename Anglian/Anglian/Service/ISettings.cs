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
        Task<bool> DeleteSubProjectImageFolder(string v_sSubProjectNo);
        Task<object> ReturnStorageFileForSubProject(string v_sSubProjectNo, string v_sFileName);
        Task<object> ReturnSubProjectImagesFolder(string v_sSubProjectNo);
        Task<bool> SaveFileLocally(object v_sfFolder, byte[] v_bFileData, string v_sFileName);
        Task<bool> SaveFileLocally(string v_sSubProjectNo, byte[] v_bFileData, string v_sFileName);
        Task<byte[]> ConvertFileToByteArray(object file);
        string GetSessionFromLocalSetting(string key);
        bool SetSessionToLocalSetting(string v_sUsername, string v_sToken, DateTime v_dLoginDate);

        Task<object> ReturnStorageFile(object v_sfFolder, string v_sFileName);
        string ReturnFilePath(object sFile);
        Task<object> GetFileFromPath(string v_sFilePath);
        string GetFileName(string v_sFilePath);
        Task<object> CreateFile(object v_sfImageFolder, string v_sFileName);
        Task<object> GetBasicProperties(object v_sfFile);
        DateTime GetLocalDateTime(object v_oBaseProperties);
        Task<bool> Delete(object v_sfFile);

    }
}
