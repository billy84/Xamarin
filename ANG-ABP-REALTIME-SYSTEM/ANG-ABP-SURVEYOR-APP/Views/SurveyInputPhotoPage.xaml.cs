using ANG_ABP_SURVEYOR_APP.Common;
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
using Windows.Media.Capture;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.Graphics.Imaging;
using ANG_ABP_SURVEYOR_APP_CLASS;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ANG_ABP_SURVEYOR_APP.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SurveyInputPhotoPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// Flag to indicate page has been loaded.
        /// </summary>
        private bool m_bPageLoaded = false;

        ///// <summary>
        ///// Stores the survey object passed through on the redirect.
        ///// </summary>
        //private cSurveyInputResult m_cSurvey = null;

        /// <summary>
        /// Holds project details.
        /// </summary>
        private cProjectTable m_cProject = null;

        /// <summary>
        /// Navigation mode, how did we get here.
        /// </summary>
        private NavigationMode m_nmNavMode = NavigationMode.New;

        /// <summary>
        /// Image folder for sub project.
        /// </summary>
        private StorageFolder m_sfImageFolder = null;

        /// <summary>
        /// 
        /// </summary>
        Windows.UI.Core.CoreDispatcher m_dpDispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;

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


        public SurveyInputPhotoPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
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

            try
            {

                //Record navigation mode.
                this.m_nmNavMode = e.NavigationMode;

                if (e.Parameter != null)
                {
                    if (e.Parameter.GetType() == typeof(string))
                    {

                        this.m_cProject = cMain.p_cDataAccess.GetSubProjectProjectData(e.Parameter.ToString());

                    }

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion


        /// <summary>
        /// Survey input photo page loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SurveyInputPhotoPage_Loaded(object sender, RoutedEventArgs e)
        {


            //Disable screen controls
            await this.EnableScreenControls(false);

            try
            {

               

                //Display sub project details.
                this.tbSubProjectDetails.Text = this.m_cProject.SubProjectNo + " / " + cMain.RemoveNewLinesFromString(this.m_cProject.DeliveryStreet);

                //Fetch sub project image folder.
                this.m_sfImageFolder = await cSettings.ReturnSubProjectImagesFolder(this.m_cProject.SubProjectNo);


                if (this.m_nmNavMode == NavigationMode.New)
                {

                    //Load existing files.
                    await this.LoadExistingFiles();

                }
                else if (this.m_nmNavMode == NavigationMode.Back)
                {
                    //Redisplay image.
                    this.gvPhotos.ItemsSource = cMain.p_cProjectPhotos;

                    //Reselect image.
                    if (cMain.p_cLastSelectedPhoto != null)
                    {
                        this.gvPhotos.ScrollIntoView(cMain.p_cLastSelectedPhoto);
                        this.gvPhotos.SelectedItem = cMain.p_cLastSelectedPhoto;
                    }

                }
               
            }
            catch (Exception ex)
            {
    
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

            //Disable screen controls
            await this.EnableScreenControls(true);

        }

        /// <summary>
        /// Load existing files.
        /// </summary>
        private async Task LoadExistingFiles()
        {

            StorageFile sfFile = null;
            try
            {

                //Create new instance.
                cMain.p_cProjectPhotos = new ObservableCollection<cDisplayPhoto>();
                cMain.p_cDeletedPhotos = new ObservableCollection<cDisplayPhoto>();

                //Reset last selected photo.
                cMain.p_cLastSelectedPhoto = null;

                string sSignatureFileName = cMain.ReturnSignatureFileName(this.m_cProject.SubProjectNo);

                List<cProjectFilesTable> cFiles = cMain.p_cDataAccess.FetchSubProjectFilesList(this.m_cProject.SubProjectNo,false);
                if (cFiles != null)
                {

                    foreach (cProjectFilesTable cFile in cFiles)
                    {

                        //We do not want to display the signature file here.
                        if (cFile.FileName.Equals(sSignatureFileName) == false)
                        {
                    
                            sfFile = await cSettings.ReturnStorageFile(this.m_sfImageFolder, cFile.FileName);
                            if (sfFile != null)
                            {

                               await this.UpdatePhotoClass(sfFile.Path, cFile.IDKey, cFile.NoteText);

                            }

                        }

                        

                    }

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Select photo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnSelectPhoto_Click(object sender, RoutedEventArgs e)
        {

            //Disable screen controls
            await this.EnableScreenControls(false);
             try
              {

                    var filePicker = new FileOpenPicker();
                    filePicker.FileTypeFilter.Add(".jpg");
                   
                    filePicker.ViewMode = PickerViewMode.Thumbnail;
                    filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                    filePicker.SettingsIdentifier = "ImageSelector";
                    filePicker.CommitButtonText = "Select picture(s) to add.";

                    //Allow user to select multiple files.
                    IReadOnlyList<StorageFile> sfFiles = await filePicker.PickMultipleFilesAsync();

                    //v1.0.18 - Take a local copy.
                    List<StorageFile> sfFilesLocal = await cSettings.CopyFilesToTemp(sfFiles);

                    ////Loop through each file and add.
                    foreach (StorageFile sfFile in sfFilesLocal)
                    {
                        await this.UpdatePhotoClass(sfFile.Path, -1, String.Empty);

                    }

                                    
              }
              catch (Exception ex)
              {
                  cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

              }

             //Disable screen controls
             await this.EnableScreenControls(true);

        }

        /// <summary>
        /// Take picture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnTakePhoto_Click(object sender, RoutedEventArgs e)
        {

            //Disable screen controls
            await this.EnableScreenControls(false);
            try
            {

                var dialog = new CameraCaptureUI();
                dialog.PhotoSettings.AllowCropping = true;

                var file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);
                if (file != null)
                {

                   await this.UpdatePhotoClass(file.Path, -1, String.Empty);

                }   

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

            //Disable screen controls
            await this.EnableScreenControls(true);
        }


        /// <summary>
        /// Update photo class.
        /// </summary>
        /// <param name="v_sFile"></param>
        private async Task UpdatePhotoClass(string v_sFile,int v_iIDKey,string v_sComment)
        {

            try
            {

              
                cDisplayPhoto cPhoto = new cDisplayPhoto();
                cPhoto.IDKey = v_iIDKey;
                cPhoto.NoteText = v_sComment;
                cPhoto.FilePath = v_sFile;
                cPhoto.FileUri = new Uri(v_sFile, UriKind.RelativeOrAbsolute);

                cMain.DisplayImageDetails didImage = await this.ReturnDisplayImageFromFile(v_sFile);

                cPhoto.ImageSource = didImage.wbBitmap;
                cPhoto.ImageWidth = didImage.dOriginalWidth;
                cPhoto.ImageHeight = didImage.dOriginalHeight;

                cPhoto.SubProjectNo = this.m_cProject.SubProjectNo;
                cPhoto.GridHeight = this.gvPhotos.ActualHeight;
                cPhoto.GridWidth = this.gvPhotos.ActualWidth;
                cPhoto.UniqueID = Environment.TickCount;
                      
                cSettings.Sleep(2); //Wait 2 milliseconds so tick count will always be unique when used.
              
                cMain.p_cProjectPhotos.Add(cPhoto);

                this.gvPhotos.ItemsSource = cMain.p_cProjectPhotos;
                this.gvPhotos.SelectedIndex = this.gvPhotos.Items.Count - 1;
                this.gvPhotos.ScrollIntoView(this.gvPhotos.SelectedItem);


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Return display image from file.
        /// </summary>
        /// <param name="v_sFilePath"></param>
        /// <returns></returns>
        private async Task<cMain.DisplayImageDetails> ReturnDisplayImageFromFile(string v_sFilePath)
        {

            cMain.DisplayImageDetails didReturn = new cMain.DisplayImageDetails();
            try
            {
                //Get image.
                BitmapImage bmpImage = await cMain.ReturnImageFromFile(v_sFilePath);

                //Record original dimensions
                didReturn.dOriginalWidth = bmpImage.PixelWidth;
                didReturn.dOriginalHeight = bmpImage.PixelHeight;

                //Fetch maximum image dimension for preview.
                decimal dMaxDimension = Convert.ToDecimal(cMain.GetAppResourceValue("Max_Image_Dimension_Preview"));

                //Work out new dimensions for displaying in grid.
                Size szNewDimensions = cMain.ReturnAspectRatio(bmpImage.PixelWidth, bmpImage.PixelHeight, dMaxDimension);

                //Resize image.
                didReturn.wbBitmap = await cMain.ReadAndResizeImageFile(v_sFilePath, szNewDimensions);

                //
                return didReturn;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return didReturn;

            }
        }


        /// <summary>
        /// Notes changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPicNotes_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {

                if (sender.GetType() == typeof(TextBox))
                {

                    TextBox tbText = (TextBox)sender;

                    if (tbText.DataContext.GetType() == typeof(cDisplayPhoto))
                    {

                        cDisplayPhoto cPhoto = (cDisplayPhoto)tbText.DataContext;

                        foreach (cDisplayPhoto cAPhoto in cMain.p_cProjectPhotos)
                        {

                            if (cPhoto.UniqueID.Equals(cAPhoto.UniqueID) == true)
                            {

                                cAPhoto.NoteText = tbText.Text;
                                return;
                            }

                        }

                    }

                }

        
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Image has been tapped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgPic_Tapped(object sender, TappedRoutedEventArgs e)
        {

            try
            {
                
                if (sender.GetType() == typeof(Image))
                {
                    Image imgImage = (Image)sender;

                    if (imgImage.DataContext.GetType() == typeof(cDisplayPhoto))
                    {
                        //Extract into from image.
                        cDisplayPhoto cPhoto = (cDisplayPhoto)imgImage.DataContext;

                        //Record last selected photo.
                        cMain.p_cLastSelectedPhoto = cPhoto;

                        //Redirect to view image page.
                        this.Frame.Navigate(typeof(ViewImage), cPhoto);


                    }

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Save photo changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {

            bool bSaveOK = false;
            try
            {


                bSaveOK = await SavePhotos();

                if (bSaveOK == true)
                {

                    //Go back.
                    this.navigationHelper.GoBack();

                }
                

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }


        /// <summary>
        /// v1.0.2 - Save Photos
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SavePhotos()
        {


            //Disable screen controls
            await this.EnableScreenControls(false);

            try
            {

                string sFileName = string.Empty;
                StorageFile sfNewFile = null;
                StorageFile sfOldFile = null;
                bool bSavedOK = false;

                //Fetch maximum image dimension.
                decimal dMaxDimension = Convert.ToDecimal(cMain.GetAppResourceValue("Max_Image_Dimension"));

                //For recording image sizes when resizing.
                Size szNewSize;

                //Retrieve file properties, we need the modified date.
                Windows.Storage.FileProperties.BasicProperties bpFile = null;

                //Loop through photos in grid view.
                foreach (cDisplayPhoto cPhoto in this.gvPhotos.Items)
                {

                    try
                    {

                        //Create storage file object of existing file.
                        sfOldFile = await StorageFile.GetFileFromPathAsync(cPhoto.FilePath);

                        //Extract file name.
                        sFileName = Path.GetFileName(cPhoto.FilePath);

                        //This is a new photo.
                        if (cPhoto.IDKey == -1)
                        {

                            //Check file name is in correct format, if not create new one.
                            if (cMain.IsImageFileInCorrectFormat(sFileName, this.m_cProject.SubProjectNo) == false)
                            {
                                sFileName = cMain.ReturnCorrectImageNameFormat(sFileName, this.m_cProject.SubProjectNo);
                            }

                            //Create new file ready for copying.
                            sfNewFile = await this.m_sfImageFolder.CreateFileAsync(sFileName);

                            //Calculate new size
                            szNewSize = cMain.ReturnAspectRatio(cPhoto.ImageWidth, cPhoto.ImageHeight, dMaxDimension);

                            //Copy and resize image and put into sub project folder.
                            bSavedOK = await cMain.CopyAndConvertImage(sfOldFile, sfNewFile, szNewSize);

                            //Extract file properties.
                            bpFile = await sfNewFile.GetBasicPropertiesAsync();

                        }
                        else
                        {

                            //If existing file retrieve date time from there.
                            bpFile = await sfOldFile.GetBasicPropertiesAsync();


                        }

                        //Save file details away to project.
                        bSavedOK = cMain.p_cDataAccess.SaveSubProjectFile(this.m_cProject.SubProjectNo, sFileName, cPhoto.NoteText, bpFile.DateModified.LocalDateTime, true);

                    }
                    catch (Exception ex)
                    {
                        bSavedOK = false;
                    }

                    if (bSavedOK == false)
                    {
                        await cSettings.DisplayMessage("An error has occurred try to save the image details away, please try again.", "Error Saving");
                        return false;
                    }                
                }
                
                try
                {

                     //Mark all the photo as deleted.
                    foreach (cDisplayPhoto cPhoto in cMain.p_cDeletedPhotos)
                    {

                        sFileName = cPhoto.FilePath;

                        //Create storage file object of existing file.
                        sfOldFile = await StorageFile.GetFileFromPathAsync(cPhoto.FilePath);

                        //Delete file.
                        await sfOldFile.DeleteAsync();

                        //Update file table.
                        cMain.p_cDataAccess.MarkFileAsDeleted(cPhoto.IDKey);

                    }

                }
                catch (Exception ex)
                {
                    cMain.ReportError(ex, cMain.GetCallerMethodName(), sFileName);

                }


                //Clean up
                cMain.p_cLastSelectedPhoto = null;
                cMain.p_cProjectPhotos = null;
                cMain.p_cDeletedPhotos = null;


                //Disable screen controls
                await this.EnableScreenControls(true);

                return true;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }
        }


     
    
        /// <summary>
        /// Remove item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnRemoveItem_Click(object sender, RoutedEventArgs e)
        {

             //Disable screen controls
             await this.EnableScreenControls(false);

            try
            {

                if (sender.GetType() == typeof(Button))
                {
                    Button btnRemove = (Button)sender;

                    if (btnRemove.DataContext.GetType() == typeof(cDisplayPhoto))
                    {
                        //Extract into from image.
                        cDisplayPhoto cPhoto = (cDisplayPhoto)btnRemove.DataContext;

                        //Add image to deleted list if it's already been saved to DB.
                        if (cPhoto.IDKey > -1)
                        {
                            cMain.p_cDeletedPhotos.Add(cPhoto);
                        }


                        //Remove image from main photos list.
                        cMain.p_cProjectPhotos.Remove(cPhoto);

                    }

                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

             //Disable screen controls
             await this.EnableScreenControls(true);
        }

        /// <summary>
        /// Enable screen controls.
        /// </summary>
        /// <param name="v_bEnable"></param>
        /// <returns></returns>
        private async Task EnableScreenControls(bool v_bEnable)
        {

            try
            {

                await this.m_dpDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    // Your UI update code goes here!
                    this.backButton.IsEnabled = v_bEnable;
                    this.gvPhotos.IsEnabled = v_bEnable;
                    this.btnSaveChanges.IsEnabled = v_bEnable;
                    this.btnSelectPhoto.IsEnabled = v_bEnable;
                    this.btnTakePhoto.IsEnabled = v_bEnable;

                });

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.2 - Back button clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void backButton_Click(object sender, RoutedEventArgs e)
        {

            bool bGoBack = true;
            try
            {

                bool bNewPhotos = this.AreThereNewPhotos();
                bool bDeletedPhotos = this.AreThereDeletedPhotos();

                if (bNewPhotos == true || bDeletedPhotos == true)
                {

                    cSettings.YesNoCancel mResponse = await cSettings.PromptForUnSavedChanges();
                    if (mResponse == cSettings.YesNoCancel.Yes)
                    {
                        bool bSaveOK = await this.SavePhotos();
                        if (bSaveOK == false)
                        {
                            bGoBack = false;
                        }


                    }
                    else if (mResponse == cSettings.YesNoCancel.No)
                    {


                    }
                    else if (mResponse == cSettings.YesNoCancel.Cancel)
                    {
                        bGoBack = false;

                    }


                }


                if (bGoBack == true)
                {

                    //v1.0.18 - Clear out temp folder before we go.
                    await cSettings.ClearTempFolder();

                    this.navigationHelper.GoBack();
                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.2 - Are there deleted photos.
        /// </summary>
        /// <returns></returns>
        private bool AreThereDeletedPhotos()
        {

            try
            {

                foreach (cDisplayPhoto cPhoto in cMain.p_cDeletedPhotos)
                {                 
                    return true;
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
        /// v1.0.2 - Are there any new photos that have been added.
        /// </summary>
        /// <returns></returns>
        private bool AreThereNewPhotos()
        {

            try
            {

                foreach (cDisplayPhoto cPhoto in cMain.p_cProjectPhotos)
                {

                    if (cPhoto.IDKey == -1)
                    {
                        return true;

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


    }
}
