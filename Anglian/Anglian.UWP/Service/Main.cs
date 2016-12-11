using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
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
        private static Windows.ApplicationModel.Resources.ResourceLoader m_rlResources = new Windows.ApplicationModel.Resources.ResourceLoader();

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
        public decimal ReturnHeightBitmapImage(object bitmapImage)
        {
            return ((BitmapImage)bitmapImage).PixelHeight;
        }
        public decimal ReturnWidthBitmapImage(object bitmapImage)
        {
            return ((BitmapImage)bitmapImage).PixelWidth;
        }
        /// <summary>
        /// Read in and resize image.
        /// </summary>
        /// <param name="v_sFilePath"></param>
        /// <param name="v_szSize"></param>
        /// <returns></returns>
        public async Task<object> ReadAndResizeImageFile(string v_sFilePath, Xamarin.Forms.Size v_szSize)
        {

            WriteableBitmap wbReturn = new WriteableBitmap((int)v_szSize.Width, (int)v_szSize.Height);
            try
            {

                StorageFile sfFile = await StorageFile.GetFileFromPathAsync(v_sFilePath);

                using (IRandomAccessStream fileStream = await sfFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);

                    // Scale image to appropriate size
                    BitmapTransform transform = new BitmapTransform()
                    {
                        ScaledWidth = Convert.ToUInt32(wbReturn.PixelWidth),
                        ScaledHeight = Convert.ToUInt32(wbReturn.PixelHeight)
                    };

                    PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                        BitmapPixelFormat.Bgra8,    // WriteableBitmap uses BGRA format
                        BitmapAlphaMode.Straight,
                        transform,
                        ExifOrientationMode.IgnoreExifOrientation, // This sample ignores Exif orientation
                        ColorManagementMode.DoNotColorManage);

                    // An array containing the decoded image data, which could be modified before being displayed
                    byte[] sourcePixels = pixelData.DetachPixelData();

                    // Open a stream to copy the image contents to the WriteableBitmap's pixel buffer
                    using (Stream stream = wbReturn.PixelBuffer.AsStream())
                    {
                        await stream.WriteAsync(sourcePixels, 0, sourcePixels.Length);
                    }
                }

                return wbReturn;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return null;

            }

        }
        /// <summary>
        /// Convert image file to image object.
        /// </summary>
        public async Task<object> ReturnImageFromFile(string v_sImageFilePath)
        {

            //Return object.
            BitmapImage imgReturn = null;

            try
            {

                //Convert string path to storage file.
                StorageFile sfFile = await StorageFile.GetFileFromPathAsync(v_sImageFilePath);

                // Ensure the stream is disposed once the image is loaded
                using (IRandomAccessStream fileStream = await sfFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap
                    BitmapImage bitmapImage = new BitmapImage();

                    await bitmapImage.SetSourceAsync(fileStream);
                    imgReturn = bitmapImage;
                }


                return imgReturn;
            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return null;

            }

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v_sfFrom"></param>
        /// <param name="v_sfTo"></param>
        public async Task<bool> CopyAndConvertImage(object v_sfFrom, object v_sfTo, Xamarin.Forms.Size v_szSize)
        {

            try
            {

                using (IRandomAccessStream fileStream = await ((StorageFile)v_sfFrom).OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);

                    // Scale image to appropriate size
                    BitmapTransform transform = new BitmapTransform()
                    {
                        ScaledWidth = Convert.ToUInt32(v_szSize.Width),
                        ScaledHeight = Convert.ToUInt32(v_szSize.Height)
                    };

                    PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                        BitmapPixelFormat.Bgra8,    // WriteableBitmap uses BGRA format
                        BitmapAlphaMode.Straight,
                        transform,
                        ExifOrientationMode.IgnoreExifOrientation, // This sample ignores Exif orientation
                        ColorManagementMode.DoNotColorManage);


                    using (var destinationStream = await ((StorageFile)v_sfTo).OpenAsync(FileAccessMode.ReadWrite))
                    {
                        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, destinationStream);
                        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)v_szSize.Width, (uint)v_szSize.Height, 72, 72, pixelData.DetachPixelData());
                        await encoder.FlushAsync();
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;
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
