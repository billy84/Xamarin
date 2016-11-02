using ANG_ABP_INSTALLER_APP.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ANG_ABP_SURVEYOR_APP_CLASS.Model;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Media.Capture;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using ANG_ABP_SURVEYOR_APP_CLASS;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ANG_ABP_INSTALLER_APP.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class UserSettingsPage : Page
    {

        /// <summary>
        /// Settings table.
        /// </summary>
        private cAppSettingsTable m_cSettings = null;

        /// <summary>
        /// v1.0.11 - Flag to indicate if page has been loaded.
        /// </summary>
        private bool m_bPageLoaded = false;

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public UserSettingsPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            try
            {
                this.Loaded += MainPage_Loaded;


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        /// <summary>
        /// Page loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {

                this.txtUserName.Text = await Windows.System.UserProfile.UserInformation.GetDisplayNameAsync();


                this.m_cSettings = cMain.p_cDataAccess.ReturnSettings();
                if (this.m_cSettings.ProfilePicPath != null)
                {
                    this.backButton.IsEnabled = true;
                    this.DisplaySettings();

                }
                else
                {

                    this.txtJobTitle.Text = "ABP Contract Manager"; //v1.0.1 - Default to ABP Contract Manager.

                    this.backButton.IsEnabled = false;
                    //this.m_cSettings = new cAppSettingsTable();

                }

                this.SetStatusSwitch();


                this.m_bPageLoaded = true;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.11 - Set status switch on screen.
        /// </summary>
        private void SetStatusSwitch()
        {
            try
            {


                if (this.m_cSettings != null)
                {

                    if (this.m_cSettings.RunningMode == "LIVE")
                    {
                        this.tsStatus.IsOn = true;

                    }
                    else
                    {
                        this.tsStatus.IsOn = false;

                    }

                }

                //v1.0.11 - If syncing in progress, disable status switch.
                if (cMain.p_bIsSyncingInProgress == true)
                {
                    this.tsStatus.Header = "Cannot be changed, syncing in progress.";
                    this.tsStatus.IsEnabled = false;
                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Display settings on screen.
        /// </summary>
        private void DisplaySettings()
        {

            try
            {
                this.txtUserName.Text = this.m_cSettings.UsersFullName;
                this.txtJobTitle.Text = this.m_cSettings.UsersJobTitle;
                this.UpdateImage();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);


            }

        }

        /// <summary>
        /// Update image on screen.
        /// </summary>
        private async void UpdateImage()
        {

            try
            {
                //We want to take a copy of the profile picture so it cannot be tampered with. = await cMain.ReturnImageFromFile(this.m_cSettings.ProfilePicPath);
                this.imgProfile.Source = await cMain.ReturnImageFromFile(this.m_cSettings.ProfilePicPath);
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Let user take picture.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnTakePicture_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                var dialog = new CameraCaptureUI();
                dialog.PhotoSettings.AllowCropping = true;

                var file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);
                if (file != null)
                {
                    this.m_cSettings.ProfilePicPath = file.Path;
                    this.UpdateImage();
                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);


            }

        }

        /// <summary>
        /// Let user browse for image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnBrowsePicture_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                var filePicker = new FileOpenPicker();
                filePicker.FileTypeFilter.Add(".jpg");
                filePicker.FileTypeFilter.Add(".png");

                filePicker.ViewMode = PickerViewMode.Thumbnail;
                filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                filePicker.SettingsIdentifier = "picker1";
                filePicker.CommitButtonText = "Select your picture";

                StorageFile sfFile = await filePicker.PickSingleFileAsync();

                //v1.0.9 - Do not process if not set.
                if (sfFile != null)
                {
                    this.m_cSettings.ProfilePicPath = sfFile.Path;
                    this.UpdateImage();

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Save changes to DB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                this.txtJobTitle.Text = this.txtJobTitle.Text.Trim();
                if (this.txtJobTitle.Text.Length == 0)
                {
                    await cSettings.DisplayMessage("You need to enter a job title before you can continue.", "Job title required.");
                    this.txtJobTitle.Focus(FocusState.Programmatic);
                    return;

                }

                //Check user name is entered.
                this.txtUserName.Text = this.txtUserName.Text.Trim();
                if (this.txtUserName.Text.Length == 0)
                {
                    await cSettings.DisplayMessage("You need to enter your name before you can save.", "Please enter name.");
                    this.txtUserName.Focus(FocusState.Programmatic);
                    return;

                }


                //Check a profile picture has been provided.
                bool bFileExists = false;
                if (this.m_cSettings.ProfilePicPath != null)
                {



                    //Fetch folder
                    var sfdFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(this.m_cSettings.ProfilePicPath));

                    //Fetch file.
                    var sfFile = await sfdFolder.TryGetItemAsync(Path.GetFileName(this.m_cSettings.ProfilePicPath));

                    if (sfFile != null)
                    {

                        string sProfilePic = ApplicationData.Current.LocalFolder.Path + "\\" + cSettings.p_sProfilePicName;

                        //If ProfilePic file already exists and this is the image path then do not save again.
                        if (this.m_cSettings.ProfilePicPath.ToLower() == sProfilePic.ToLower())
                        {
                            bFileExists = true;


                        }
                        else
                        {

                            StorageFile sfOldFile = await sfdFolder.GetFileAsync(Path.GetFileName(this.m_cSettings.ProfilePicPath));

                            //Fetch maximum image dimension.
                            decimal dMaxDimension = Convert.ToDecimal(cMain.GetAppResourceValue("Max_Image_Dimension"));

                            //For recording image sizes when resizing.
                            Size szNewSize;

                            BitmapImage bmpImage = (BitmapImage)this.imgProfile.Source;


                            //Calculate new size
                            szNewSize = cMain.ReturnAspectRatio((decimal)bmpImage.PixelWidth, (decimal)bmpImage.PixelHeight, dMaxDimension);


                            //Fetch file.
                            var sfFileCheck = await ApplicationData.Current.LocalFolder.TryGetItemAsync(cSettings.p_sProfilePicName);

                            StorageFile sfNewFile = null;
                            //Check from file exists.
                            if (sfFileCheck != null)
                            {
                                sfNewFile = await ApplicationData.Current.LocalFolder.GetFileAsync(cSettings.p_sProfilePicName);
                            }
                            else
                            {
                                sfNewFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(cSettings.p_sProfilePicName, CreationCollisionOption.ReplaceExisting);
                            }

                            //We want to take a copy of the profile picture so it cannot be tampered with.
                            bool bCopyOK = await cMain.CopyAndConvertImage(sfOldFile, sfNewFile, szNewSize);


                            if (bCopyOK == true)
                            {
                                this.m_cSettings.ProfilePicPath = sProfilePic;
                                bFileExists = true;
                            }

                        }


                    }

                }

                //If no picture tell user they need to provide one.
                if (bFileExists == false)
                {
                    await cSettings.DisplayMessage("You need to provide a picture of yourself.", "Profile Picture Required.");
                    this.btnTakePicture.Focus(FocusState.Programmatic);
                    return;


                }

                //Update users full name property.
                this.m_cSettings.UsersFullName = this.txtUserName.Text;
                this.m_cSettings.UsersJobTitle = this.txtJobTitle.Text; //v1.0.1 - Users job title.

                //Save user details to DB.
                bool bSaveOK = await cMain.p_cDataAccess.SaveSettings(this.m_cSettings);
                if (bSaveOK == true)
                {
                    NavigationHelper.GoBack();
                }
                else
                {
                    await cSettings.DisplayMessage("A problem occurred when trying to save your details, please try again.", "Error when saving.");

                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Create test data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnCreateTestData_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //cMain.p_cDataAccess.CreateEnumTestData();
                //cMain.p_cDataAccess.CreateProjectTestData();

                //await cSettings.DisplayMessage("Test data created successfully.", "Test Data Created");

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.11 - Status has been toggled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void tsStatus_Toggled(object sender, RoutedEventArgs e)
        {

            try
            {
                if (this.m_bPageLoaded == false) { return; }

                if (this.tsStatus.IsOn == true)
                {
                    this.m_cSettings.RunningMode = "LIVE";

                }
                else
                {
                    this.m_cSettings.RunningMode = "TEST";

                }



                //Reset dates

                // Date last sync took place.        
                this.m_cSettings.LastSyncDateTime = DateTime.Now.AddYears(-1);

                // Date last base enum check took place.
                this.m_cSettings.LastBaseEnumCheckDateTime = DateTime.Now.AddYears(-1);

                // Date last settings check took place.
                this.m_cSettings.LastSettingsCheckDateTime = DateTime.Now.AddYears(-1);

                //v1.0.9 - Reset install check date time.
                this.m_cSettings.LastInstallersCheckDateTime = DateTime.Now.AddYears(-1);

                //Save details away
                await cMain.p_cDataAccess.SaveSettings(this.m_cSettings);

                //Update connection object now status has changed.
                cMain.p_cDataAccess.CheckDB();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }



    }
}
