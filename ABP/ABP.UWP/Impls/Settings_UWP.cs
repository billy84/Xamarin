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

[assembly: Dependency(typeof(Settings_UWP))]
namespace ABP.UWP.Impls
{
    class Settings_UWP : ISettings
    {
        public string GetUserName()
        {
            return "prote01";
        }
        public string GetUserDisplayName()
        {
            return "prote01";
        }
    }
}
