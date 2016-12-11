using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Service;
using Anglian.UWP.Service;
using Xamarin.Forms;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Networking;
using Windows.Networking.Connectivity;
using System.IO;
using Windows.UI.Popups;
using Windows.System;
using Anglian.Classes;
[assembly: Dependency(typeof(Settings))]
namespace Anglian.UWP.Service
{
    class Settings : ISettings
    {
        /// <summary>
        /// Support message
        /// </summary>
        /// 
        public const string p_sImageStoreRootFolderName = "SurveyorAppImageStore";
        public const string p_sSupportMessage = "If the problem persists please contact the I.T. service desk on 01603 420566 or email service.desk@angliangroup.com";
        public async static Task<StorageFolder> ReturnImageRooFolder()
        {

            StorageFolder sfRootFolder = null;
            try
            {

                string sFolderName = Settings.p_sImageStoreRootFolderName;

                IStorageItem siItem = await Windows.Storage.KnownFolders.PicturesLibrary.TryGetItemAsync(sFolderName);
                if (siItem == null)
                {
                    sfRootFolder = await Windows.Storage.KnownFolders.PicturesLibrary.CreateFolderAsync(sFolderName);

                }
                else
                {
                    sfRootFolder = await Windows.Storage.KnownFolders.PicturesLibrary.GetFolderAsync(sFolderName);

                }

                return sfRootFolder;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }
        public async Task<bool> DeleteSubProjectImageFolder(string v_sSubProjectNo)
        {
            try
            {
                //Fetch image root folder.
                StorageFolder sfRoot = await Settings.ReturnImageRooFolder();

                //See if folder exists.
                IStorageItem siItem = await sfRoot.TryGetItemAsync(v_sSubProjectNo);
                if (siItem != null)
                {
                    //If folder exists then create a storage folder object for that folder.
                    StorageFolder sfSubProject = await sfRoot.GetFolderAsync(v_sSubProjectNo);

                    //Delete folder.
                    await sfSubProject.DeleteAsync(StorageDeleteOption.Default);

                }

                //If we get here then all OK.
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - (" + v_sSubProjectNo + ")");

            }
        }
        /// <summary>
        /// Loads the byte data from a StorageFile
        /// </summary>
        /// <param name="file">The file to read</param>
        public async Task<byte[]> ConvertFileToByteArray(object file)
        {

            try
            {

                byte[] fileBytes = null;
                using (IRandomAccessStreamWithContentType stream = await ((StorageFile)file).OpenReadAsync())
                {
                    fileBytes = new byte[stream.Size];
                    using (DataReader reader = new DataReader(stream))
                    {
                        await reader.LoadAsync((uint)stream.Size);
                        reader.ReadBytes(fileBytes);

                    }
                }
                return fileBytes;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<string> GetUserName()
        {

            try
            {

                IReadOnlyList<User> users = await User.FindAllAsync();
                var current = users.Where(p => p.AuthenticationStatus == UserAuthenticationStatus.LocallyAuthenticated &&
                                p.Type == UserType.LocalUser).FirstOrDefault();
                var data = await current.GetPropertyAsync(KnownUserProperties.AccountName);
                string displayName = (string)data;
                if (string.IsNullOrEmpty(displayName))
                {
                    string a = (string)await current.GetPropertyAsync(KnownUserProperties.FirstName);
                    string b = (string)await current.GetPropertyAsync(KnownUserProperties.LastName);
                    displayName = string.Format("{0} {1}", a, b);
                }
                return displayName;

            }
            catch (Exception ex)
            {
                return ex.Message;

            }

        }
        public async Task<string> GetUserDisplayName()
        {
            try
            {
                // Window 10
                IReadOnlyList<User> users = await User.FindAllAsync();
                var current = users.Where(p => p.AuthenticationStatus == UserAuthenticationStatus.LocallyAuthenticated &&
                                p.Type == UserType.LocalUser).FirstOrDefault();
                var data = await current.GetPropertyAsync(KnownUserProperties.AccountName);
                string displayName = (string)data;
                if (string.IsNullOrEmpty(displayName))
                {
                    string a = (string)await current.GetPropertyAsync(KnownUserProperties.DisplayName);
                    //string b = (string)await current.GetPropertyAsync(KnownUserProperties.LastName);
                    displayName = string.Format("{0}", a);
                }
                return displayName;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Return the name of the machine.
        /// </summary>
        /// <returns></returns>
        public string GetMachineName()
        {
            try
            {
                IReadOnlyList<HostName> HostNames = NetworkInformation.GetHostNames();
                foreach (HostName hsName in HostNames)
                {
                    if (hsName.Type == HostNameType.DomainName)
                    {
                        string[] sNameParts = hsName.RawName.Split('.');
                        return sNameParts[0];
                    }
                }
                return "N\\A";
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public bool IsThisTheSurveyorApp()
        {
            try
            {

                string sAppName = Windows.ApplicationModel.Package.Current.DisplayName;
                //if (sAppName.Equals("ANG-ABP-INSTALLER-APP") == true)
                //{
                //    return false;
                //}
                //else if (sAppName.Equals("ANG-ABP-SURVEYOR-APP") == true)
                //{
                    return true;

                //}
                //else
                //{
                //   throw new Exception("Unidentified application name (" + sAppName + ")");
                //}

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }


        public bool AreWeOnline()
        {
            return NetworkInformation.GetInternetConnectionProfile() != null;
        }
        public async static Task<StorageFolder> ReturnSubProjectImagesFolder1(string v_sSubProjectNo)
        {

            StorageFolder sfProject = null;
            try
            {
                //Retrieve the image root folder.
                StorageFolder sfRoot = await Settings.ReturnImageRooFolder();
                if (sfRoot != null)
                {

                    //Check if folder exists
                    IStorageItem siItem = await sfRoot.TryGetItemAsync(v_sSubProjectNo);
                    if (siItem == null)
                    {
                        sfProject = await sfRoot.CreateFolderAsync(v_sSubProjectNo);

                    }
                    else
                    {
                        sfProject = await sfRoot.GetFolderAsync(v_sSubProjectNo);

                    }


                    return sfProject;

                }

                return sfProject;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SubProjectNo(" + v_sSubProjectNo + ")");

            }

        }
        public async Task<bool> SaveFileLocally(string v_sSubProjectNo, byte[] v_bFileData, string v_sFileName)
        {

            try
            {

                //Fetch sub project image folder.
                StorageFolder sfProject = await Settings.ReturnSubProjectImagesFolder1(v_sSubProjectNo);

                //Save file.
                using (Stream f = await sfProject.OpenStreamForWriteAsync(v_sFileName, CreationCollisionOption.ReplaceExisting))
                {
                    f.Seek(0, SeekOrigin.End);
                    await f.WriteAsync(v_bFileData, 0, v_bFileData.Length);
                }
                return true;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - FileName(" + v_sFileName + ")");

            }

        }
        /// <summary>
        /// Save file locally.
        /// </summary>
        /// <param name="v_sfFolder"></param>
        /// <param name="v_bFileData"></param>
        /// <param name="v_sFileName"></param>
        /// <returns></returns>
        public async Task<bool> SaveFileLocally(StorageFolder v_sfFolder, byte[] v_bFileData, string v_sFileName)
        {

            try
            {

                //Save file data to local file.
                using (Stream f = await v_sfFolder.OpenStreamForWriteAsync(v_sFileName, CreationCollisionOption.ReplaceExisting))
                {
                    f.Seek(0, SeekOrigin.End);
                    await f.WriteAsync(v_bFileData, 0, v_bFileData.Length);
                }

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - FileName(" + v_sFileName + ")");

            }
        }
        public async Task<bool> SaveFileLocally(object v_sfFolder, byte[] v_bFileData, string v_sFileName)
        {

            try
            {

                //Save file data to local file.
                using (Stream f = await ((StorageFolder)v_sfFolder).OpenStreamForWriteAsync(v_sFileName, CreationCollisionOption.ReplaceExisting))
                {
                    f.Seek(0, SeekOrigin.End);
                    await f.WriteAsync(v_bFileData, 0, v_bFileData.Length);
                }

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - FileName(" + v_sFileName + ")");

            }
        }

        public async Task DisplayMessage(string v_sMessage, string v_sTitle)
        {

            try
            {
                MessageDialog mdMessage = new MessageDialog(v_sMessage, v_sTitle);
                await mdMessage.ShowAsync();
               

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " MESSAGE(" + v_sMessage + ") TITLE(" + v_sTitle + ")");

            }

        }
        /*
        public async Task<bool> ReturnSubProjectImagesFolder(string v_sSubProjectNo)
        {
            StorageFolder sfProject = null;
            try
            {
                //Retrieve the image root folder.
                StorageFolder sfRoot = await Settings.ReturnImageRooFolder();
                if (sfRoot != null)
                {

                    //Check if folder exists
                    IStorageItem siItem = await sfRoot.TryGetItemAsync(v_sSubProjectNo);
                    if (siItem == null)
                    {
                        sfProject = await sfRoot.CreateFolderAsync(v_sSubProjectNo);

                    }
                    else
                    {
                        sfProject = await sfRoot.GetFolderAsync(v_sSubProjectNo);

                    }

                }

                if (sfProject != null)
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SubProjectNo(" + v_sSubProjectNo + ")");

            }
        }
        */
        public async Task<object> ReturnSubProjectImagesFolder(string v_sSubProjectNo)
        {
            StorageFolder sfProject = null;
            try
            {
                //Retrieve the image root folder.
                StorageFolder sfRoot = await Settings.ReturnImageRooFolder();
                if (sfRoot != null)
                {

                    //Check if folder exists
                    IStorageItem siItem = await sfRoot.TryGetItemAsync(v_sSubProjectNo);
                    if (siItem == null)
                    {
                        sfProject = await sfRoot.CreateFolderAsync(v_sSubProjectNo);

                    }
                    else
                    {
                        sfProject = await sfRoot.GetFolderAsync(v_sSubProjectNo);

                    }

                }

                return sfProject;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SubProjectNo(" + v_sSubProjectNo + ")");

            }
        }
        public string ReturnFilePath(object sFile)
        {
            try
            {
                return ((StorageFile)sFile).Path;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
            
        }
        public async Task<bool> Delete(object v_sfFile)
        {
            try
            {
                await ((StorageFile)v_sfFile).DeleteAsync();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
            
        }
        public DateTime GetLocalDateTime(object v_oBaseProperties)
        {
            return ((Windows.Storage.FileProperties.BasicProperties)v_oBaseProperties).DateModified.LocalDateTime;
        }
        public async Task<object> GetBasicProperties(object v_sfFile)
        {
            return await ((StorageFile)v_sfFile).GetBasicPropertiesAsync();
        }
        public async Task<object> CreateFile(object v_sfImageFolder, string v_sFileName)
        {
            return await ((StorageFolder)v_sfImageFolder).CreateFileAsync(v_sFileName);
        }
        public string GetFileName(string v_sFilePath)
        {
            return Path.GetFileName(v_sFilePath);
        }
        public async Task<object> GetFileFromPath(string v_sFilePath)
        {
            return await StorageFile.GetFileFromPathAsync(v_sFilePath);
        }
        /// <summary>
        /// Return storage file.
        /// </summary>
        /// <param name="v_sfFolder"></param>
        /// <param name="v_sFileName"></param>
        /// <returns></returns>
        public async Task<object> ReturnStorageFile(object v_sfFolder, string v_sFileName)
        {
            try
            {

                //Check if signature file exists
                IStorageItem siItem = await ((StorageFolder)v_sfFolder).TryGetItemAsync(v_sFileName);
                if (siItem != null)
                {
                    return await ((StorageFolder)v_sfFolder).GetFileAsync(v_sFileName);

                }

                return null;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - FILENAME(" + v_sFileName + ")");

            }

        }
        /// <summary>
        /// Return sub project storage file.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <param name="v_sFileName"></param>
        /// <returns></returns>
        public async Task<object> ReturnStorageFileForSubProject(string v_sSubProjectNo, string v_sFileName)
        {

            StorageFile sfReturn = null;
            try
            {

                object oSubProject = await ReturnSubProjectImagesFolder(v_sSubProjectNo);
                StorageFolder sfSubProject = (StorageFolder)oSubProject;
                if (sfSubProject != null)
                {
                    sfReturn = (StorageFile)await ReturnStorageFile(sfSubProject, v_sFileName);

                }

                return sfReturn;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SubProjectNo(" + v_sSubProjectNo + "),FileName(" + v_sFileName + ")");
            }

        }
        public bool SetSessionToLocalSetting(string v_sUsername, string v_sToken, DateTime v_dLoginDate)
        {
            try
            {
                var settings = ApplicationData.Current.LocalSettings;
                settings.Values["UserName"] = v_sUsername;
                settings.Values["Token"] = v_sToken;
                settings.Values["Date"] = v_dLoginDate.ToString();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string GetSessionFromLocalSetting(string key)
        {
            try
            {
                var settings = ApplicationData.Current.LocalSettings;
                return settings.Values[key].ToString();

            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
