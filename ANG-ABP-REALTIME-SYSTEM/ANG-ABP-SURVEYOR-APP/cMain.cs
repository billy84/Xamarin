using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using ANG_ABP_SURVEYOR_APP_CLASS.wcfCalls;
using Windows.UI.Popups;
using Windows.Storage;
using System.IO;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using ANG_ABP_SURVEYOR_APP_CLASS.Model;
using ANG_ABP_SURVEYOR_APP.Views;
using Windows.Foundation;
using System.Collections.ObjectModel;
using Windows.Graphics.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using ANG_ABP_SURVEYOR_APP_CLASS;
using ANG_ABP_SURVEYOR_APP_CLASS.Classes;
using ANG_ABP_SURVEYOR_APP_CLASS.Syncing;
using Windows.ApplicationModel.Background;

namespace ANG_ABP_SURVEYOR_APP
{

    /// <summary>
    /// 
    /// </summary>
    class cMain
    {

        /// <summary>
        /// Background task
        /// </summary>
        private static BackgroundTaskRegistration m_tsBackgroundTask;

        /// <summary>
        /// Resource loader object for accessing settings in the Resources.resw file.
        /// </summary>
        private static Windows.ApplicationModel.Resources.ResourceLoader m_rlResources = new Windows.ApplicationModel.Resources.ResourceLoader();

  

        /// <summary>
        /// Data access variable
        /// </summary>
        public static cDataAccess p_cDataAccess = null;

        /// <summary>
        /// Setting class.
        /// </summary>
        public static cSettings p_cSettings = new cSettings();

        /// <summary>
        /// Dispatch timer, for checking connection
        /// </summary>
        private static DispatcherTimer m_dpDispatcher = null;

        /// <summary>
        /// Dispatch timer, for syncing
        /// </summary>
        private static DispatcherTimer m_dpDispatcherSync = null;
        
  

        /// <summary>
        /// For recording input in the survey input screen, is used to then redisplay when the user navigates back to the page.
        /// </summary>
        public static cProjectTable p_cSurveyInputScreenData = null;

        /// <summary>
        /// v1.0.4 - Used for recording survey input selections.
        /// </summary>
        public static cProjectTable p_cSurveyInputCopiedSelections = null;

        /// <summary>
        /// v1.0.6 - Used for recording last note on surveyor input screen.
        /// </summary>
        public static cProjectNotesTable p_cSurveyInputCopiedLastNote = null;


        /// <summary>
        /// For recording notes input in the survey input screen, is used to then redisplay when the user navigates back to the page.
        /// </summary>
        public static List<cProjectNotesTable> p_cSurveyInputProjectNotes = null;

        /// <summary>
        /// v1.0.21 - Flag to indicate that failed survey reasons are being checked for.
        /// </summary>
        private static bool m_bCheckingSurveyFailedReasons = false;

        /// <summary>
        /// Flag to indicate that base enums are being checked for.
        /// </summary>
        private static bool m_bCheckingBaseEnums = false;

        /// <summary>
        /// Flag to indicate that settings are being checked for.
        /// </summary>
        private static bool m_bCheckingSetings = false;

        /// <summary>
        /// Format for comparing date times.
        /// </summary>
        public const string p_sCompareDateFormat = "yyyyMMddHHmmss";

        /// <summary>
        /// Search criteria for the survey dates search screen.
        /// </summary>
        public static cSearchCriteria p_cSearchCriteria_LastSearch = null;

        /// <summary>
        /// List of project photos, used on the photo page.
        /// </summary>
        public static ObservableCollection<cDisplayPhoto> p_cProjectPhotos = null;

        /// <summary>
        /// List of deleted project photos, used on the photo page.
        /// </summary>
        public static ObservableCollection<cDisplayPhoto> p_cDeletedPhotos = null;

        /// <summary>
        /// Last selected photo.
        /// </summary>
        public static cDisplayPhoto p_cLastSelectedPhoto = null;



        /// <summary>
        /// v1.0.11 - Flag to indicate syncing is in progress.
        /// </summary>
        public static bool p_bIsSyncingInProgress = false;

                
        /// <summary>
        /// Image name parts.
        /// </summary>
        private enum ImageNameParts
        {
            Date = 0
            ,SubProjectNo = 1
            ,FileName = 2

        }


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
        /// v1.0.19 - Should I check for survey reasons result.
        /// </summary>
        private struct ShouldICheckForSurveyReasonResult
        {

            /// <summary>
            /// 
            /// </summary>
            public bool bCheck;

            /// <summary>
            /// 
            /// </summary>
            public DateTime dLastUpdate;

        }
 
        /// <summary>
        /// Copy file.
        /// </summary>
        /// <param name="v_sFromFile"></param>
        /// <param name="v_sFileTo"></param>
        /// <returns></returns>
        public async static Task<bool> CopyFile(string v_sFromFile, string v_sToFile)
        {

            try
            {


                //Fetch folder
                var sfdFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(v_sFromFile));

                //Fetch file.
                var sfFileFrom = await sfdFolder.TryGetItemAsync(Path.GetFileName(v_sFromFile));

                //Check from file exists.
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
                cMain.ReportError(ex, cMain.GetCallerMethodName(), "FROM FILE(" + v_sFromFile + ") TO FILE(" + v_sToFile + ")");
                return false;
            }


        }


        /// <summary>
        /// Setup the dispatcher.
        /// </summary>
        public static void SetupDisptcher()
        {

            try
            {

                //If not initialized then initialize.
                if (m_dpDispatcher == null)
                {

                    m_dpDispatcher = new DispatcherTimer();
                    m_dpDispatcher.Tick += m_dpDispatcher_Tick;
                    m_dpDispatcher.Interval = new TimeSpan(0, 1, 0);

                }

                //If not started then start.
                if (m_dpDispatcher.IsEnabled == false)
                {                    

                    m_dpDispatcher.Start();

                }
                

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }


        /// <summary>
        /// Dispatch timer tick event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static async void  m_dpDispatcher_Tick(object sender, object e)
        {

            try
            {

                await cMain.CheckAXConnection();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
           
        }

        /// <summary>
        /// Check AX connection is available.
        /// </summary>
        public static async Task CheckAXConnection()
        {


            //We do not want more than 1 instance running at a time.
            m_dpDispatcher.Stop();

            bool bOnline = false;

            try
            {


                Windows.UI.Xaml.Controls.Frame fmFrame = (Windows.UI.Xaml.Controls.Frame)Window.Current.Content;
                Windows.UI.Xaml.Controls.Page pgPage = (Windows.UI.Xaml.Controls.Page)fmFrame.Content;

                if (pgPage is MainPage)
                {

                    MainPage mpPage = (MainPage)pgPage;
                    Windows.UI.Xaml.Controls.Border brOnline = (Windows.UI.Xaml.Controls.Border)mpPage.FindName("brOnlineStatus");
                    Windows.UI.Xaml.Controls.TextBlock tbOnline = (Windows.UI.Xaml.Controls.TextBlock)mpPage.FindName("tbOnlineStatus");
                    if (tbOnline != null)
                    {
                        
                        tbOnline.Text = "Checking..";
                        brOnline.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White);

                        bOnline = await cMain.p_cSettings.IsAXSystemAvailable(false);

                        if (bOnline == true)
                        {

                          
                            await cMain.CheckForUpdates();
                           

                            tbOnline.Text = "Online";
                            brOnline.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Green);
                          
                        }
                        else
                        {

                            tbOnline.Text = "Offline";
                            brOnline.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Red);
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
            finally
            {

                //Restart timer.
                m_dpDispatcher.Start();

            }

        }

        /// <summary>
        /// v1.0.19 - Check for updates.
        /// </summary>
        public async static Task CheckForUpdates()
        {

            try
            {

                //Check for base enum updates
                await cMain.CheckForBaseEnums();

                //v1.0.1 - Check for setting updates
                await cMain.CheckForSettings();

                //v1.0.19 - Check for failed survey reasons.
                await cMain.CheckForFailedSurveyReasons();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Returns a value from the resources file match passed name.
        /// </summary>
        /// <param name="v_sResourceName"></param>
        /// <returns></returns>
        public static string GetAppResourceValue(string v_sResourceName)
        {

            try
            {
                return cMain.m_rlResources.GetString(v_sResourceName);

            }
            catch(Exception ex)
            {
                return ex.Message;

            }

        }


        /// <summary>
        /// Get the method name of the calling routine.
        /// </summary>
        /// <param name="caller"></param>
        /// <returns></returns>
        public static string GetCallerMethodName([CallerMemberName] string caller = "")
        {
            return caller;
        }


        /// <summary>
        /// Reports any errors back to head office.
        /// </summary>
        /// <param name="v_ex"></param>
        /// <param name="v_sMethodName"></param>
        /// <param name="v_sMessage"></param>
        /// <param name="v_iPriority"></param>
        /// <returns></returns>
        public static void ReportError(Exception v_ex, string v_sMethodName, string v_sMessage)
        {
            cMain.ReportError(v_ex, v_sMethodName, v_sMessage, Convert.ToInt32(cMain.GetAppResourceValue("DefaultErrorPriority")));

        }

        /// <summary>
        /// Reports any errors back to head office.
        /// </summary>
        /// <param name="v_ex"></param>
        /// <param name="v_sMethodName"></param>
        /// <param name="v_sMessage"></param>
        /// <param name="v_iPriority"></param>
        /// <returns></returns>
        public static async Task ReportError(Exception v_ex, string v_sMethodName, string v_sMessage, int v_iPriority)
        {

            try
            {

                ANG_ABP_SURVEYOR_APP_CLASS.wcfErrorLog.ErrorInfo eiError = new ANG_ABP_SURVEYOR_APP_CLASS.wcfErrorLog.ErrorInfo();
                eiError.sProductName = cMain.GetAppResourceValue("ProductName");
                eiError.sProductVersion = cMain.GetAppResourceValue("ProductVersion");
                eiError.sUserName = await cSettings.GetUserName();
                eiError.sMethodName = v_sMethodName;
                eiError.sMachineName = cSettings.GetMachineName();
                eiError.sEx_StackTrace = v_ex.StackTrace;
                eiError.sEx_Source = v_ex.Source;
                eiError.sEx_Message = v_ex.Message;
                eiError.sClassName = String.Empty; //v_mbInfo.Module.Name
                eiError.iPriority = v_iPriority;
                eiError.sInformation = v_sMessage;

                cErrorLogWCF cLogError = new ANG_ABP_SURVEYOR_APP_CLASS.wcfCalls.cErrorLogWCF(cMain.GetAppResourceValue("Status"));
                cLogError.LogError(eiError);
                cLogError = null;

            }
            catch
            {



            }

        }


        /// <summary>
        /// Convert image file to image object.
        /// </summary>
        public static async Task<BitmapImage> ReturnImageFromFile(string v_sImageFilePath)
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
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return null;

            }

        }

        /// <summary>
        /// Read in and resize image.
        /// </summary>
        /// <param name="v_sFilePath"></param>
        /// <param name="v_szSize"></param>
        /// <returns></returns>
        public static async Task<WriteableBitmap> ReadAndResizeImageFile(string v_sFilePath, Size v_szSize)
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
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return null;

            }

        }


        /// <summary>
        /// Create work display title from survey date.
        /// </summary>
        /// <param name="v_dSurveyDate"></param>
        /// <returns></returns>
        public static string CreateWorkDisplayTitle(DateTime v_dSurveyDate)
        {

            try
            {

                //Retrieve AM and PM fixed time.
                string sAM_Time = cMain.GetAppResourceValue("AM_TIME");
                string sPM_Time = cMain.GetAppResourceValue("PM_TIME");

                //Build up display string.
                string sDisplay = "Survey " + v_dSurveyDate.ToString("dd/MM/yyyy") + " @ ";

                //Check too see what time needs to be displayed
                string sTime = v_dSurveyDate.ToString("HH:mm");
                if (sTime == sAM_Time)
                {
                    sDisplay += "AM";

                }
                else if (sTime == sPM_Time)
                {
                    sDisplay += "PM";

                }
                else
                {
                    sDisplay += sTime;

                }

                return sDisplay;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return null;

            }

        }


        /// <summary>
        /// Convert null able date time to date time.
        /// </summary>
        /// <param name="v_dDateTime"></param>
        /// <returns></returns>
        public static DateTime ConvertNullableDateTimeToDateTime(DateTime? v_dDateTime)
        {

            try
            {

                if (v_dDateTime.HasValue == false)
                {
                    return new DateTime();
                 
                }
                else
                {
                    return v_dDateTime.Value;

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return new DateTime();


            }

        }


        /// <summary>
        /// Return string for displaying last sync date time.
        /// </summary>
        /// <param name="v_dSyncDate"></param>
        /// <returns></returns>
        public static string ReturnLastSyncString(DateTime? v_dSyncDate)
        {

            try
            {

                StringBuilder sDisplayText = new StringBuilder();

                if (v_dSyncDate.HasValue == false)
                {
                    sDisplayText.Append("Data has never been synced.");

                }
                else
                {
                    sDisplayText.Append("Data last synced on " + v_dSyncDate.Value.ToString("dd/MM/yyyy") + " @ " + v_dSyncDate.Value.ToString("HH:mm"));

                   

                }

                int iUploads = cMain.p_cDataAccess.GetNumberOfUploadsPending();
                int iNotes = cMain.p_cDataAccess.GetNumberOfNotesPending(); //v1.0.1
                int iFiles = cMain.p_cDataAccess.GetNumberOfFilesPending();

                //sDisplayText.Append(Environment.NewLine);
                sDisplayText.Append(" - (" + (iUploads + iFiles + iNotes).ToString() + ") changes waiting for upload.");

                return sDisplayText.ToString();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return string.Empty;

            }

        }

        /// <summary>
        /// Return a display date from the passed date time object.
        /// </summary>
        /// <param name="v_dtDate"></param>
        /// <returns></returns>
        public static string ReturnDisplayDate(DateTime v_dtDate)
        {

            try
            {
                return ReturnDisplayDate(v_dtDate, string.Empty);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return string.Empty;

            }

        }

        /// <summary>
        /// Return a display date from the passed date time object.
        /// </summary>
        /// <param name="v_dtDate"></param>
        /// <returns></returns>
        public static string ReturnDisplayDate(DateTime v_dtDate,string v_sDateCompare)
        {

            try
            {

                string sSymbol = String.Empty;
                if (v_sDateCompare == cSettings.p_sDateCompare_GreaterThan)
                {
                    sSymbol = "> ";
                }

                if (v_sDateCompare == cSettings.p_sDateCompare_LessThan)
                {
                    sSymbol = "< ";
                }

                string sDayName = sSymbol + v_dtDate.DayOfWeek.ToString().Substring(0, 3);
                return sDayName + " " + v_dtDate.ToString("dd/MM/yyyy");

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return string.Empty;

            }

        }

        /// <summary>
        /// Return display time.
        /// </summary>
        /// <param name="v_dDate"></param>
        /// <returns></returns>
        public static string ReturnDisplayTime(DateTime v_dDate)
        {

            try
            {

                string sTime = String.Empty;

                string sNowTime = v_dDate.ToString("HH:mm");
                string sAMTime = cMain.GetAppResourceValue("AM_TIME");
                string sPMTime = cMain.GetAppResourceValue("PM_TIME");

                if (sNowTime == sAMTime){
                    sTime = cSettings.p_sTime_AM;

                }
                else if (sNowTime == sPMTime)
                {
                    sTime = cSettings.p_sTime_PM;

                }
                else 
                {
                    sTime = sNowTime;

                }

                return sTime;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return string.Empty;

            }
        }

        /// <summary>
        /// v1.0.21 - Should I check for failed survey reasons.
        /// </summary>
        /// <returns></returns>
        private static ShouldICheckForSurveyReasonResult ShouldICheckForFailedSurveyReasons()
        {
            
            ShouldICheckForSurveyReasonResult cResult = new ShouldICheckForSurveyReasonResult();
            cResult.bCheck = false;
            cResult.dLastUpdate = DateTime.MinValue;

            try
            {

                int iDaysBetweenChecks = Convert.ToInt32(cMain.GetAppResourceValue("CheckFailedReasonsDaysBetweenChecks").ToString());

                cAppSettingsTable cSettings = cMain.p_cDataAccess.ReturnSettings();
                if (cSettings != null)
                {

                    if (cSettings.LastSurveyFailedCheckDateTime.HasValue == true)
                    {
                        TimeSpan tsDiff = DateTime.Now.Subtract(cSettings.LastSurveyFailedCheckDateTime.Value);
                        if (tsDiff.TotalDays >= iDaysBetweenChecks)
                        {
                            cResult.bCheck = true;
                            cResult.dLastUpdate = cSettings.LastSurveyFailedUpdateDateTime.Value;
                        }

                    }
                    else
                    {
                        cResult.bCheck = true;
                    }

                }
                else
                {
                    cResult.bCheck = true;
                }

                return cResult;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return cResult;

            }

        }

        /// <summary>
        /// Should I check for base enums
        /// </summary>
        /// <returns></returns>
        private static bool ShouldICheckForBaseEnums()
        {

            bool bShouldICheck = false;
            try
            {

                 int iDaysBetweenChecks = Convert.ToInt32(cMain.GetAppResourceValue("CheckBaseEnumDaysBetweenChecks").ToString());

                cAppSettingsTable cSettings = cMain.p_cDataAccess.ReturnSettings();
                if (cSettings != null)
                {

                    if (cSettings.LastBaseEnumCheckDateTime.HasValue == true)
                    {
                        TimeSpan tsDiff = DateTime.Now.Subtract(cSettings.LastBaseEnumCheckDateTime.Value);
                        if (tsDiff.TotalDays >= iDaysBetweenChecks)
                        {
                            bShouldICheck = true;

                        }

                    }
                    else
                    {
                        bShouldICheck = true;
                    }
                    
                }
                else
                {
                    bShouldICheck = true;
                }

                return bShouldICheck;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }

        }

        /// <summary>
        /// Should I check for settings
        /// </summary>
        /// <returns></returns>
        private static bool ShouldICheckForSettings()
        {

            bool bShouldICheck = false;
            try
            {

                int iDaysBetweenChecks = Convert.ToInt32(cMain.GetAppResourceValue("CheckSettingsDaysBetweenChecks").ToString());

                cAppSettingsTable cSettings = cMain.p_cDataAccess.ReturnSettings();
                if (cSettings != null)
                {

                    if (cSettings.LastSettingsCheckDateTime.HasValue == true)
                    {
                        TimeSpan tsDiff = DateTime.Now.Subtract(cSettings.LastSettingsCheckDateTime.Value);
                        if (tsDiff.TotalDays >= iDaysBetweenChecks)
                        {
                            bShouldICheck = true;

                        }

                    }
                    else
                    {
                        bShouldICheck = true;
                    }

                }
                else
                {
                    bShouldICheck = true;
                }

                return bShouldICheck;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }

        }

        /// <summary>
        /// Check for settings
        /// </summary>
        /// <returns></returns>
        public async static Task CheckForSettings()
        {

            cAXCalls cAX = null;
            try
            {

                //If already checking then leave.
                if (cMain.m_bCheckingSetings == true) { return; }

                bool bCheck = cMain.ShouldICheckForSettings();
                if (bCheck == true)
                {

                    //For calling AX wcf
                    cAX = new cAXCalls();

                    
                    //Fetch local list of base enums
                    List<ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.SettingDetails> sdSetting = cMain.p_cDataAccess.GetSettingsUpdates();

                    //Get update settings from AX
                    List<ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.SettingDetails> beSettingsUpdate = await cAX.ReturnUpdatedSettings(sdSetting);

                    //Close connection.
                    if (cAX != null)
                    {
                        await cAX.CloseAXConnection();
                    }

                    await cMain.p_cDataAccess.ProcessUpdatedSettings(beSettingsUpdate);


                }

                //Reset checking flag.
                cMain.m_bCheckingSetings = false;

            }
            catch (Exception ex)
            {

                cMain.m_bCheckingBaseEnums = false; //Reset checking flag.
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.19 - Check for Failed Survey reasons.
        /// </summary>
        public async static Task CheckForFailedSurveyReasons()
        {

            cAXCalls cAX = null;
            try
            {

                //If already checking then leave.
                if (cMain.m_bCheckingSurveyFailedReasons == true) { return; }
                cMain.m_bCheckingSurveyFailedReasons = true;

                ShouldICheckForSurveyReasonResult cCheckResult = cMain.ShouldICheckForFailedSurveyReasons();
                if (cCheckResult.bCheck == true)
                {

                    //For calling AX wcf
                    cAX = new cAXCalls();

                    //Get update base enums from AX
                    ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.FetchSurveyFailedReasonsResult fsrResult = await cAX.FetchFailedSurveyReasons(cCheckResult.dLastUpdate);

                    if (fsrResult != null)
                    {

                        if (fsrResult.bSuccessfull == true)
                        {

                            cAppSettingsTable cSettings = cMain.p_cDataAccess.ReturnSettings();

                            if (fsrResult.sfrReasons.Count > 0)
                            {

                                bool bUpdateOK = cMain.p_cDataAccess.UpdateFailedSurveyReasonsTable(fsrResult.sfrReasons);
                                if (bUpdateOK == true)
                                {

                                    cSettings.LastSurveyFailedUpdateDateTime = fsrResult.bLastUpdateDate;

                                }


                            }

                            cSettings.LastSurveyFailedCheckDateTime = DateTime.Now;

                            cMain.p_cDataAccess.SaveSettings(cSettings);

                        }

                    }
                                    
                }

                //Close connection.
                if (cAX != null)
                {
                    await cAX.CloseAXConnection();
                }

                //Reset checking flag.
                cMain.m_bCheckingSurveyFailedReasons = false;

            }
            catch (Exception ex)
            {

                cMain.m_bCheckingSurveyFailedReasons = false; //Reset checking flag.
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);              

            }         

        }

        /// <summary>
        /// Check for base enums
        /// </summary>
        public async static Task CheckForBaseEnums()
        {

            cAXCalls cAX = null;
            try
            {

                //If already checking then leave.
                if (cMain.m_bCheckingBaseEnums == true) { return; }

                bool bCheck = cMain.ShouldICheckForBaseEnums();
                if (bCheck == true)
                {

                    //For calling AX wcf
                    cAX = new cAXCalls();

                    //Fetch local list of base enums
                    List<ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.BaseEnumField> beFields = cMain.p_cDataAccess.GetBaseEnumUpdates();

                    //Get update base enums from AX
                    List<ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.BaseEnumField> beFieldsUpdate = await cAX.ReturnUpdatedBaseEnums(beFields);

                    //Close connection.
                    if (cAX != null)
                    {
                        await cAX.CloseAXConnection();
                    }

                    await cMain.p_cDataAccess.ProcessUpdatedBaseEnums(beFieldsUpdate);


                }

                //Reset checking flag.
                cMain.m_bCheckingBaseEnums = false;

            }
            catch (Exception ex)
            {

                cMain.m_bCheckingBaseEnums = false; //Reset checking flag.
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Return signature file name.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public static string ReturnSignatureFileName(string v_sSubProjectNo)
        {

            try
            {

                return v_sSubProjectNo + "+" + cSettings.p_sSignaturePicName;


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return null;

            }
        }

        /// <summary>
        /// Return signature file for sub project if one exists.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public static async Task<StorageFile> ReturnSignatureFile(string v_sSubProjectNo)
        {

            try
            {

                string sSigFileName = cMain.ReturnSignatureFileName(v_sSubProjectNo);
                List<cProjectFilesTable> cSigFiles =   cMain.p_cDataAccess.FetchSubProjectFilesList(v_sSubProjectNo, sSigFileName,false);

                if (cSigFiles != null)
                {
                    if (cSigFiles.Count > 0)
                    {

                        foreach (cProjectFilesTable cFile in cSigFiles)
                        {
                            
                            //Fetch sub project image folder.
                            StorageFolder sfSubProject = await cSettings.ReturnSubProjectImagesFolder(v_sSubProjectNo);

                            //Check if signature file exists
                            IStorageItem siItem = await sfSubProject.TryGetItemAsync(sSigFileName);
                            if (siItem != null)
                            {
                                return await sfSubProject.GetFileAsync(sSigFileName);

                            }
                           
                        }

                    }

                }

                return null;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return null;

            }

        }

    

        /// <summary>
        /// CHeck image file name is in the correct format.
        /// </summary>
        /// <param name="v_sImageFileName"></param>
        /// <returns></returns>
        public static bool IsImageFileInCorrectFormat(string v_sImageFileName,string v_sSubProjectNo)
        {
           
            try
            {

                string[] sFileParts = v_sImageFileName.Split('+');
                if (sFileParts.Length == 3)
                {

                    if (sFileParts[(int)cMain.ImageNameParts.Date].Length == 14)
                    {

                        if (sFileParts[(int)cMain.ImageNameParts.SubProjectNo].Equals(v_sSubProjectNo, StringComparison.CurrentCultureIgnoreCase) == true)
                        {

                            if (sFileParts[(int)cMain.ImageNameParts.FileName].Length > 0)
                            {

                                return true;

                            }

                        }

                    }
                  
                }

                return false;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;
            }

        }

        /// <summary>
        /// Return correct image name.
        /// </summary>
        /// <param name="v_sImageFileName"></param>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public static string ReturnCorrectImageNameFormat(string v_sImageFileName, string v_sSubProjectNo)
        {

            try
            {

                return  DateTime.Now.ToString("yyyyMMddHHmmss") + "+" + v_sSubProjectNo + "+" + v_sImageFileName;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return null;

            }
        }


        /// <summary>
        /// Return sub project last update date \ time
        /// </summary>
        /// <param name="v_cSubProjectData"></param>
        /// <returns></returns>
        public static DateTime? ReturnSubProjectLastUpdate(cProjectTable v_cSubProjectData)
        {

            DateTime? dLastUpdate = null;
            try
            {

                //Compare projtable modifieddatetime
                if (v_cSubProjectData.ModifiedDateTime.HasValue == true)
                {
                    dLastUpdate = v_cSubProjectData.ModifiedDateTime;

                }

                //Compare SMMactivities date time.
                if (v_cSubProjectData.SMMActivities_MODIFIEDDATETIME.HasValue == true)
                {
                    if (dLastUpdate.HasValue == false)
                    {
                        dLastUpdate = v_cSubProjectData.SMMActivities_MODIFIEDDATETIME;
                    }
                    else
                    {
                        if (Convert.ToInt64(dLastUpdate.Value.ToString(cMain.p_sCompareDateFormat)) < Convert.ToInt64(v_cSubProjectData.SMMActivities_MODIFIEDDATETIME.Value.ToString(cMain.p_sCompareDateFormat)))
                        {
                            dLastUpdate = v_cSubProjectData.SMMActivities_MODIFIEDDATETIME;


                        }

                    }

                }                

                return dLastUpdate;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return null;

            }

        }


        /// <summary>
        /// Remove new line chars from string.
        /// </summary>
        /// <param name="v_sString"></param>
        /// <returns></returns>
        public static string RemoveNewLinesFromString(string v_sString)
        {

            string sNewString = String.Empty;
            int iSpacesInARow = 0;
            try
            {

                foreach (char cChar in v_sString)
                {

                    if (char.IsControl(cChar) == false)
                    {
                        sNewString += cChar;
                        iSpacesInARow = 0;
                    }
                    else
                    {
                        if (iSpacesInARow == 0)
                        {
                            sNewString += " ";
                            iSpacesInARow += 1;
                        }
                       
                    }

                }

                return sNewString;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return v_sString;
            }
        }

        /// <summary>
        /// Return the string text for time drop down option for the time passed.
        /// </summary>
        /// <param name="v_dtTime"></param>
        /// <returns></returns>
        public static string ReturnTimeDropDownSelection(DateTime v_dtTime)
        {

            try
            {

                //Extract time into comparable format.
                string sTime = v_dtTime.ToString("HH:mm");

                //Check against AM time.
                string sAMTime = cMain.GetAppResourceValue("AM_TIME");
                if (sTime.Equals(sAMTime) == true)
                {
                    return cSettings.p_sTime_AM;
                }

                //Check against PM time.
                string sPMTime = cMain.GetAppResourceValue("PM_TIME");
                if (sTime.Equals(sPMTime) == true)
                {
                    return cSettings.p_sTime_PM;
                }

                //Must be a specific time.
                return cSettings.p_sTime_Specific;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return string.Empty;
            }
        }



        /// <summary>
        /// Return selected items tag value from combo box
        /// </summary>
        /// <param name="v_cmbCombo"></param>
        /// <returns></returns>
        public static string ReturnComboSelectedTagValue(ComboBox v_cmbCombo)
        {

            string sRtnValue = string.Empty;
            try
            {

                ComboBoxItem cmbItem = (ComboBoxItem)v_cmbCombo.SelectedItem;
                if (cmbItem.Tag != null)
                {
                    sRtnValue = cmbItem.Tag.ToString();

                }

                return sRtnValue;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return string.Empty;
            }
        }

        /// <summary>
        /// Return selected items text value from combo box
        /// </summary>
        /// <param name="v_cmbCombo"></param>
        /// <returns></returns>
        public static string ReturnComboSelectedItemText(ComboBox v_cmbCombo)
        {

            string sRtnValue = string.Empty;
            try
            {

                ComboBoxItem cmbItem = (ComboBoxItem)v_cmbCombo.SelectedItem;
                if (cmbItem.Tag != null)
                {
                    sRtnValue = cmbItem.Content.ToString();

                }

                return sRtnValue;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return string.Empty;
            }
        }


        /// <summary>
        /// Return aspect ratio
        /// </summary>
        /// <param name="v_dWidth"></param>
        /// <param name="v_dHeight"></param>
        /// <returns></returns>
        public static Size ReturnAspectRatio(decimal v_dWidth, decimal v_dHeight,decimal v_dMaxDimension)
        {

            Size szReturn = new Size();
            try
            {


                if (v_dWidth < v_dMaxDimension && v_dHeight < v_dMaxDimension)
                {

                    szReturn.Width = (double)v_dWidth;
                    szReturn.Height = (double)v_dHeight;

                }
                else
                {
                    double dAspectRatio = (double)v_dWidth / (double)v_dHeight;
                    if (v_dWidth > v_dHeight)
                    {
                        szReturn.Width = (double)v_dMaxDimension;
                        szReturn.Height = (double)((double)v_dMaxDimension / dAspectRatio);

                    }
                    else
                    {
                        szReturn.Width = (double)((double)v_dMaxDimension * dAspectRatio);
                        szReturn.Height = (double)v_dMaxDimension;

                    }

                }

                return szReturn;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return szReturn;
            }

        }



 
  
        /// <summary>
        /// Kick off the syncing process.
        /// </summary>
        public static void KickOffSyncing()
        {

            try
            {

                 //If not initialized then initialize.
                if (cMain.m_dpDispatcherSync == null)
                {

                    cMain.m_dpDispatcherSync = new DispatcherTimer();
                    cMain.m_dpDispatcherSync.Tick += cMain.m_dpDispatcherSync_Tick;
                    cMain.m_dpDispatcherSync.Interval = new TimeSpan(0, 0, 1);

                }

                //If not started then start.
                if (cMain.m_dpDispatcherSync.IsEnabled == false)
                {
                    if (cMain.p_bIsSyncingInProgress == false)
                    {
                        cMain.m_dpDispatcherSync.Start();
                    }
                                        
                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
            }

        }


        /// <summary>
        /// Dispatch sync timer tick event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static async void m_dpDispatcherSync_Tick(object sender, object e)
        {



           cMain.m_dpDispatcher.Stop();
           cMain.m_dpDispatcherSync.Stop();

            try
            {
               
                await cMain.StartSyncing(cSyncing.p_bSyncChangesOnly);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
            finally
            {
                cMain.m_dpDispatcher.Start();
            }

            cMain.UpdateScreenAfterSyncing();
            
        }

        /// <summary>
        /// Update screen after syncing is complete.
        /// </summary>
        private static async void UpdateScreenAfterSyncing()
        {

            try
            {

                 //Retrieve settings.
                cAppSettingsTable cSettings = cMain.p_cDataAccess.ReturnSettings();

                //Update last date time.
                cSettings.LastSyncDateTime = DateTime.Now;

                //Save settings
                await cMain.p_cDataAccess.SaveSettings(cSettings);


                //Update screen.
                await cMain.UpdateMainSyncStatus();
                

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
            }

        }

        /// <summary>
        /// Update main sync status
        /// </summary>
        public async static Task UpdateMainSyncStatus()
        {

            try
            {

                string sSyncMsg = String.Empty;

                Windows.UI.Xaml.Controls.Frame fmFrame = (Windows.UI.Xaml.Controls.Frame)Window.Current.Content;
                Windows.UI.Xaml.Controls.Page pgPage = (Windows.UI.Xaml.Controls.Page)fmFrame.Content;

                if (pgPage is MainPage)
                {

                    //Retrieve settings.
                    cAppSettingsTable cSettings = cMain.p_cDataAccess.ReturnSettings();

                    sSyncMsg = cMain.ReturnLastSyncString(cSettings.LastSyncDateTime);

                }
                else if (pgPage is SyncPage)
                {

                    SyncPage spPage = (SyncPage)pgPage;
                    await spPage.EnableScreenControls(true);

                    sSyncMsg = "Sync Completed";



                }

                cMain.UpdateSyncMainPage(sSyncMsg);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
            }

        }

        /// <summary>
        /// Start syncing
        /// </summary>
        private async static Task StartSyncing(bool v_bUploadChangesOnly)
        {

            cSyncing cSync = null;
            bool bProcessedOK = false;
            try
            {

                cMain.p_bIsSyncingInProgress = true;

                bool bConnected = await cMain.p_cSettings.IsAXSystemAvailable(true);
                if (bConnected == true)
                {

                    cSync = new cSyncing();
                    cSync.SubProjectStatusUpdate += cSync_SubProjectStatusUpdate;
                    cSync.UpdateMessage += cSync_UpdateMessage;
                    cSync.DisplayMessage += cSync_DisplayMessage;
                    cSync.ProjectSyncError += cSync_ProjectSyncError;

                    bProcessedOK = await cSync.UploadChanges(false);
                    if (bProcessedOK == true)
                    {

                        bProcessedOK = await cSync.UploadPhotos();

                        //If uploading changes only, then leave here.
                        if (v_bUploadChangesOnly == true)
                        {
                            return;
                        }

                        bProcessedOK = await cSync.CheckForDataChanges(false);

                        if (bProcessedOK == true)
                        {



                        }

                    }

                }
                

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
            }
            finally
            {
                cMain.p_bIsSyncingInProgress = false;
            }

        }

        /// <summary>
        /// Report syncing error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void cSync_ProjectSyncError(object sender, cSyncEventErrorParams e)
        {
            try
            {

                throw new Exception("Error syncing project (" + e.ProjectNo + ") Error (" + e.ErrorMessage + ")");

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), "ABP_SYNC_ERROR");
            }
        }

        /// <summary>
        /// Display Message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static async void cSync_DisplayMessage(object sender, string e)
        {
          await  cSettings.DisplayMessage(e, "Application Message");
        }

        /// <summary>
        /// v1.0.1 - Sub Project update status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void cSync_UpdateMessage(object sender, cSyncEventParam e)
        {

            try
            {

                cMain.UpdateSyncMainPage(e.DisplayMessage);                

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
            }

        }

        /// <summary>
        /// v1.0.1 - Sub project status update.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void cSync_SubProjectStatusUpdate(object sender, cSyncEventParamProjectStatus e)
        {

            try
            {

                cMain.UpdateSyncPageProjectStatus(e.PrevProjectNo, e.NextProjectNo,e.PrevProjectSubProjectsAdded,e.PrevProjectSubProjectsDeleted,e.UpdateSuccess);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
            }
            
        }

         /// <summary>
        /// Update sync main page.
        /// </summary>
        public static void UpdateSyncMainPage(string v_sSyncStatus)
        {

            try
            {

                cMain.UpdateSyncMainPage(v_sSyncStatus, false);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
            }

        }

        /// <summary>
        /// Update sync main page.
        /// </summary>
        public static void UpdateSyncMainPage(string v_sSyncStatus,bool v_bMainPageOnly)
        {

            try
            {

                Windows.UI.Xaml.Controls.Frame fmFrame = (Windows.UI.Xaml.Controls.Frame)Window.Current.Content;
                Windows.UI.Xaml.Controls.Page pgPage = (Windows.UI.Xaml.Controls.Page)fmFrame.Content;

                if (pgPage is MainPage)
                {

                    MainPage mpPage = (MainPage)pgPage;
                    Windows.UI.Xaml.Controls.TextBlock tbOnline = (Windows.UI.Xaml.Controls.TextBlock)mpPage.FindName("tbLastSyncDateTime");
                    tbOnline.Text = v_sSyncStatus;

                }
                else if (pgPage is SyncPage && v_bMainPageOnly == false)
                {

                    SyncPage spPage = (SyncPage)pgPage;
                    Windows.UI.Xaml.Controls.TextBlock tbOnline = (Windows.UI.Xaml.Controls.TextBlock)spPage.FindName("tbSyncStatus");
                    tbOnline.Text = v_sSyncStatus;

                }
                       
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
            }

        }

        /// <summary>
        /// v1.0.1 - Update project on the sync page projects list.
        /// </summary>
        /// <param name="v_sPrevProjNo"></param>
        /// <param name="v_sNextProjNo"></param>
        public static void UpdateSyncPageProjectStatus(string v_sPrevProjNo, string v_sNextProjNo, int v_iPrevProjSubProjectsAdded, int v_iPrevProjSubProjectsDeleted, bool v_bUpdateSuccess)
        {

            try
            {

                Windows.UI.Xaml.Controls.Frame fmFrame = (Windows.UI.Xaml.Controls.Frame)Window.Current.Content;
                Windows.UI.Xaml.Controls.Page pgPage = (Windows.UI.Xaml.Controls.Page)fmFrame.Content;

                if (pgPage is SyncPage)
                {

                    SyncPage spPage = (SyncPage)pgPage;

                    if (v_sPrevProjNo.Length > 0)
                    {

                        if (v_bUpdateSuccess == true)
                        {
                            spPage.UpdateProjectsSyncStatus(v_sPrevProjNo, "Synced", v_iPrevProjSubProjectsAdded, v_iPrevProjSubProjectsDeleted);

                        }
                        else
                        {
                            spPage.UpdateProjectsSyncStatus(v_sPrevProjNo, "Sync Failed", v_iPrevProjSubProjectsAdded, v_iPrevProjSubProjectsDeleted);
                        }

                        
                    }

                    if (v_sNextProjNo.Length > 0)
                    {
                        spPage.UpdateProjectsSyncStatus(v_sNextProjNo, "Syncing", v_iPrevProjSubProjectsAdded, v_iPrevProjSubProjectsDeleted);
                    }
                    
                   
                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
            }

        }

        /// <summary>
        /// Check background task is registered.
        /// </summary>
        public static void CheckBackgroundTasks()
        {

            try
            {

                var taskRegistered = false;
                var exampleTaskName = "cTasks";
                
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == exampleTaskName)
                    {
                       
                        task.Value.Unregister(true);

                        //BackgroundTaskSample.UnregisterBackgroundTasks
                        //taskRegistered = true;
                        break;
                    }
                }

                if (taskRegistered == false)
                {


                    var builder = new BackgroundTaskBuilder();

                    builder.Name = exampleTaskName;
                    builder.TaskEntryPoint = "background.cTask";
                    
                    builder.SetTrigger(new SystemTrigger(SystemTriggerType.UserAway, false));
                    builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));

                    cMain.m_tsBackgroundTask = builder.Register();

                    cMain.m_tsBackgroundTask.Progress += new BackgroundTaskProgressEventHandler(task_Progress);
                    cMain.m_tsBackgroundTask.Completed += new BackgroundTaskCompletedEventHandler(task_Completed);                

                }

               

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
            }



        }


        /// <summary>
        /// Task completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        static void task_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            
            var settings = ApplicationData.Current.LocalSettings;
            string sName = settings.Values["BackgroundWorkCost"].ToString();

        }

        /// <summary>
        /// Progress of task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        static void task_Progress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args)
        {
            string sName = "Brian";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v_sfFrom"></param>
        /// <param name="v_sfTo"></param>
        public static async Task<bool> CopyAndConvertImage(StorageFile v_sfFrom, StorageFile v_sfTo, Size v_szSize)
        {

            try
            {

                using (IRandomAccessStream fileStream = await v_sfFrom.OpenAsync(Windows.Storage.FileAccessMode.Read))
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


                    using (var destinationStream = await v_sfTo.OpenAsync(FileAccessMode.ReadWrite))
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
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;
            }

        }


        /// <summary>
        /// v1.0.2 - Return message warning that passed sub projects will not be displayed on front screen d
        /// </summary>
        /// <param name="v_lstJobs"></param>
        /// <returns></returns>
        public static string ReturnSubProjectWillNotDisplayOnFrontScreenMessage(List<string> v_lstJobs)
        {

            StringBuilder sbMsg = new StringBuilder();
            try
            {

                int iInstall_Awaiting = Convert.ToInt32(cMain.GetAppResourceValue("InstallStatus_AwaitingSurvey"));               
                string sInstallStatusName = cMain.p_cDataAccess.GetEnumValueName("ProjTable", "Mxm1002InstallStatus", iInstall_Awaiting);              

                if (v_lstJobs.Count == 1)
                {
                    sbMsg.Append("Warning, due to the status of this job " + v_lstJobs[0] + " it will not display on the front screen.");
                    sbMsg.Append(Environment.NewLine);
                    
                }
                else
                {

                    sbMsg.Append("Warning, due to the status of the following jobs they will not be displayed on the front screen:");
                    sbMsg.Append(Environment.NewLine);
                    foreach (string sJob in v_lstJobs)
                    {
                        sbMsg.Append(sJob);
                        sbMsg.Append(Environment.NewLine);

                    }
                   
                }

                sbMsg.Append(Environment.NewLine);
                sbMsg.Append("The status needed to display on the front screen are:");
                sbMsg.Append(Environment.NewLine);
                sbMsg.Append("Install status: " + sInstallStatusName);
                sbMsg.Append(Environment.NewLine);


                return sbMsg.ToString();


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return null;
            }

        }


        /// <summary>
        /// v1.0.2 - Checks too see if sub project status is such that it will not show on front screen.
        /// </summary>
        /// <param name="v_iSubProjectProgressStatus"></param>
        /// <param name="v_iSubProjectInstallStatus"></param>
        /// <returns></returns>
        public static bool WillSubProjectDisplayOnFrontScreen(int v_iSubProjectInstallStatus)
        {


            try
            {

                int iInstall_Awaiting = Convert.ToInt32(cMain.GetAppResourceValue("InstallStatus_AwaitingSurvey"));


                if (v_iSubProjectInstallStatus != iInstall_Awaiting)
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
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;
            }


        }

        /// <summary>
        /// v1.0.2 - Create settings record if not already there.
        /// </summary>
        public async static void CreateSettingsRecord()
        {

            bool bChangesMade = false;
            try
            {

                cAppSettingsTable cSetting = cMain.p_cDataAccess.ReturnSettings();
                if (cSetting == null)
                {
                    cSetting = new cAppSettingsTable();
                    bChangesMade = true;
                }

          
                if (cSetting.RunningMode == null || cSetting.RunningMode.Length == 0)
                {
                    cSetting.RunningMode = cMain.GetAppResourceValue("STATUS"); //v1.0.11 - Default to environment in resources file.
                    bChangesMade = true;

                }

                if (bChangesMade == true)
                {
                    await cMain.p_cDataAccess.SaveSettings(cSetting);
                }
                

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                
            }
           
        }

        /// <summary>
        /// v1.0.11 - Initialize database
        /// </summary>
        public static void InitialiseDB()
        {

            try
            {

                //Create data object and check database is setup.
                cMain.p_cDataAccess = new ANG_ABP_SURVEYOR_APP_CLASS.cDataAccess();
                cMain.CreateSettingsRecord();
                cMain.p_cDataAccess.CheckDB();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v_cWorkDB"></param>
        /// <returns></returns>
        public static string ReturnAddress(cProjectTable v_cWorkDB)
        {
            string sAddress = string.Empty;
            try
            {

                sAddress = cMain.RemoveNewLinesFromString(v_cWorkDB.DeliveryStreet);

                if (v_cWorkDB.DeliveryCity.Length > 0)
                {
                    if (sAddress.Length > 0)
                    {
                        sAddress += ", ";
                    }

                    sAddress += cMain.RemoveNewLinesFromString(v_cWorkDB.DeliveryCity);
                }

                if (v_cWorkDB.DlvState.Length > 0)
                {
                    if (sAddress.Length > 0)
                    {
                        sAddress += ", ";
                    }

                    sAddress += v_cWorkDB.DlvState;
                }

                if (v_cWorkDB.DlvZipCode.Length > 0)
                {
                    if (sAddress.Length > 0)
                    {
                        sAddress += ", ";
                    }

                    sAddress += cMain.RemoveNewLinesFromString(v_cWorkDB.DlvZipCode);
                }

                return sAddress;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return sAddress;

            }


        }


    }
}
