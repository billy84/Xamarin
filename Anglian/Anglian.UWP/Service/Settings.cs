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

[assembly: Dependency(typeof(Settings))]
namespace Anglian.UWP.Service
{
    class Settings : ISettings
    {
        /// <summary>
        /// Support message
        /// </summary>
        public const string p_sSupportMessage = "If the problem persists please contact the I.T. service desk on 01603 420566 or email service.desk@angliangroup.com";

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
    }
}
