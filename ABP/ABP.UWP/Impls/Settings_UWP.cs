using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;
using ABP.UWP.Impls;
using ABP.Interfaces;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.System;

[assembly: Dependency(typeof(Settings_UWP))]
namespace ABP.UWP.Impls
{
    class Settings_UWP : ISettings
    {
        public async Task<string> GetUserName()
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
                    string a = (string)await current.GetPropertyAsync(KnownUserProperties.FirstName);
                    string b = (string)await current.GetPropertyAsync(KnownUserProperties.LastName);
                    displayName = string.Format("{0} {1}", a, b);
                }
                return displayName;

            }
            catch (Exception ex)
            {
                return null;
            }
            //return "prote01";
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
                return null;
            }
        }
        public bool AreWeOnline()
        {
            return NetworkInformation.GetInternetConnectionProfile() != null;
        }
        public async Task<bool> ReturnSubProjectImagesFolder(string v_sSubProjectNo)
        {
            StorageFolder sfProject = null;
            try
            {
                StorageFolder sfRoot = await ReturnImageRooFolder();
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
                if (sfProject == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message + " - SubProjectNo(" + v_sSubProjectNo + ")");
                return false;
            }
        }
        public async static Task<StorageFolder> ReturnImageRooFolder()
        {

            StorageFolder sfRootFolder = null;
            try
            {

                string sFolderName = "SurveyorAppImageStore";

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
        public async Task<bool> SaveFileLocally(string v_sSubProjectNo, byte[] v_bFileData, string v_sFileName)
        {
            try
            {
                StorageFolder sfProject = await ReturnSubProjectImagesFolder1(v_sSubProjectNo);
                return await SaveFileLocally(sfProject, v_bFileData, v_sFileName);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async static Task<bool> SaveFileLocally(StorageFolder v_sfFolder, byte[] v_bFileData, string v_sFileName)
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
        public async Task<StorageFolder> ReturnSubProjectImagesFolder1(string v_sSubProjectNo)
        {

            StorageFolder sfProject = null;
            try
            {
                //Retrieve the image root folder.
                StorageFolder sfRoot = await ReturnImageRooFolder();
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
    }
}
