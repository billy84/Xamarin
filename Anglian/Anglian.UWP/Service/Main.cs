using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using Windows.Foundation;
using System.Collections.ObjectModel;
using Windows.Graphics.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Anglian.UWP.Service;
using Anglian.Service;
using Anglian.UWP.Classes;
using Anglian.Models;
using Xamarin.Forms;
using Windows.UI.Xaml.Media.Imaging;
[assembly: Dependency(typeof(Main))]
namespace Anglian.UWP.Service
{
    class Main : IMain
    {
        private static BackgroundTaskRegistration m_tsBackgroundTask;
        private static Windows.ApplicationModel.Resources.ResourceLoader m_rlResources = new Windows.ApplicationModel.Resources.ResourceLoader();
        /// <summary>
        /// Dispatch timer, for checking connection
        /// </summary>
        private static DispatcherTimer m_dpDispatcher = null;

        /// <summary>
        /// Dispatch timer, for syncing
        /// </summary>
        private static DispatcherTimer m_dpDispatcherSync = null;

        public static ObservableCollection<DisplayPhoto> p_cProjectPhotos = null;

        /// <summary>
        /// List of deleted project photos, used on the photo page.
        /// </summary>
        public static ObservableCollection<DisplayPhoto> p_cDeletedPhotos = null;

        /// <summary>
        /// Last selected photo.
        /// </summary>
        public static DisplayPhoto p_cLastSelectedPhoto = null;
        /// <summary>
        /// Structure for return data after applying survey date changes.
        /// </summary>
        public struct DisplayImageDetails
        {

            /// <summary>
            /// Bitmap
            /// </summary>
            public WriteableBitmap wbBitmap;

            /// <summary>
            /// Original width
            /// </summary>
            public decimal dOriginalWidth;

            /// <summary>
            /// Original height
            /// </summary>
            public decimal dOriginalHeight;

        }
        /// <summary>
        /// Returns a value from the resources file match passed name.
        /// </summary>
        /// <param name="v_sResourceName"></param>
        /// <returns></returns>
        public string GetAppResourceValue(string v_sResourceName)
        {

            try
            {
                return m_rlResources.GetString(v_sResourceName);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<bool> CopyFile(string v_sFromFile, string v_sToFile)
        {
            try
            {
                var sfdFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(v_sFromFile));
                var sfFileFrom = await sfdFolder.TryGetItemAsync(Path.GetFileName(v_sFromFile));
                if (sfFileFrom != null)
                {

                    IStorageFolder sfToFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(v_sToFile));

                    //Convert string path to storage file.
                    StorageFile sfFile = await StorageFile.GetFileFromPathAsync(v_sFromFile);
                    await sfFile.CopyAsync(sfToFolder, Path.GetFileName(v_sToFile), NameCollisionOption.ReplaceExisting);

                    return true;
                }
                else
                {
                    throw new Exception("From file does not exist (" + v_sFromFile + ")");

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Setup the dispatcher.
        /// </summary>

        /// <summary>
        /// Dispatch timer tick event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        public static string GetCallerMethodName([CallerMemberName] string caller = "")
        {
            return caller;
        }
        public static void ReportError(Exception v_ex, string v_sMethodName, string v_sMessage)
        {
            //cMain.ReportError(v_ex, v_sMethodName, v_sMessage, Convert.ToInt32(cMain.GetAppResourceValue("DefaultErrorPriority")));
        }
    }
}
