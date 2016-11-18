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
    }
}
