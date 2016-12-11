using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Models;
using Anglian.Classes;
using Xamarin.Forms;
using Plugin.Media;
using Anglian.Service;
using Anglian.Engine;
using Anglian.Models;
namespace Anglian.Views
{
    public class PhotoItem
    {
        public ImageSource ImageSource { get; set; }
        public string NoteText { get; set; }
    }

    public partial class PhotoScreenPage : ContentPage
    {
        ObservableCollection<PhotoItem> items = new ObservableCollection<PhotoItem>();
        private cProjectTable m_cProject = null;
        private object m_sfImageFolder = null;//StorageFolder
        public PhotoScreenPage(SurveyInputResult ProjectInfo)
        {
            InitializeComponent();
            this.m_cProject = Main.p_cDataAccess.GetSubProjectProjectData(ProjectInfo.SubProjectNo);
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Save",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "save"),
                Command = new Command(() => btnSaveChanges_Click())
            });
            this.Title = ProjectInfo.ProjectNo + " - " + ProjectInfo.ProjectName;
            SurveyInputPhotoPage_Loaded();

        }
        private async void SurveyInputPhotoPage_Loaded()
        {
            try
            {
                this.m_sfImageFolder = await DependencyService.Get<ISettings>().ReturnSubProjectImagesFolder(this.m_cProject.SubProjectNo);
                await LoadExistingFiles();
            }
            catch (Exception ex)
            {

            }
        }
        private async Task LoadExistingFiles()
        {
            //StorageFile sfFile = null;
            object sfFile = null;
            try
            {
                Main.p_cProjectPhotos = new ObservableCollection<DisplayPhoto>();
                Main.p_cDeletedPhotos = new ObservableCollection<DisplayPhoto>();
                Main.p_cLastSelectedPhoto = null;
                string sSignatureFileName = Main.ReturnSignatureFileName(this.m_cProject.SubProjectNo);
                List<cProjectFilesTable> cFiles = Main.p_cDataAccess.FetchSubProjectFilesList(this.m_cProject.SubProjectNo, false);
                if (cFiles != null)
                {

                    foreach (cProjectFilesTable cFile in cFiles)
                    {

                        //We do not want to display the signature file here.
                        if (cFile.FileName.Equals(sSignatureFileName) == false)
                        {

                            sfFile = await DependencyService.Get<ISettings>().ReturnStorageFile(this.m_sfImageFolder, cFile.FileName);
                            if (sfFile != null)
                            {
                                string strPath = DependencyService.Get<ISettings>().ReturnFilePath(sfFile);
                                await this.UpdatePhotoClass(strPath, cFile.IDKey, cFile.NoteText);

                            }

                        }



                    }

                }

            }
            catch (Exception ex)
            {
                //
            }
        }
        private async Task UpdatePhotoClass(string v_sFile, int v_iIDKey, string v_sComment)
        {
            try
            {
                DisplayPhoto cPhoto = new DisplayPhoto();
                cPhoto.IDKey = v_iIDKey;
                cPhoto.NoteText = v_sComment;
                cPhoto.FilePath = v_sFile;
                cPhoto.FileUri = new Uri(v_sFile, UriKind.RelativeOrAbsolute);
                Main.DisplayImageDetails didImage = await this.ReturnDisplayImageFromFile(v_sFile);

                cPhoto.ImageSource = didImage.wbBitmap;
                cPhoto.ImageWidth = didImage.dOriginalWidth;
                cPhoto.ImageHeight = didImage.dOriginalHeight;

                cPhoto.SubProjectNo = this.m_cProject.SubProjectNo;
                //cPhoto.GridHeight = this.gvPhotos.ActualHeight;
                //cPhoto.GridWidth = this.gvPhotos.ActualWidth;
                cPhoto.UniqueID = Environment.TickCount;

                await Task.Delay(2);//Wait 2 milliseconds so tick count will always be unique when used.

                Main.p_cProjectPhotos.Add(cPhoto);

                //DisplayUI(Main.p_cProjectPhotos);
                DisplayUI(cPhoto);
                //this.gvPhotos.ItemsSource = cMain.p_cProjectPhotos;
                //this.gvPhotos.SelectedIndex = this.gvPhotos.Items.Count - 1;

            }
            catch (Exception ex)
            {

            }
        }
        private void DisplayUI(DisplayPhoto ImageItem)
        {
            try
            {
                Image imgPic = new Image();
                imgPic.Source = ImageSource.FromFile(ImageItem.FileUri.ToString());
                //imgPic.HeightRequest = Convert.ToDouble(ImageItem.ImageHeight);
                //imgPic.WidthRequest = Convert.ToDouble(ImageItem.ImageWidth);
                Label lbComment = new Label() { Text = "Enter Comment:" };
                Entry txtPicNotes = new Entry();
                txtPicNotes.Text = ImageItem.NoteText;
                txtPicNotes.HeightRequest = 50;
                //Button btnRemoveItem = new Button();
                Label lbRemove = new Label();
                lbRemove.Text = "Remove this image";
                lbRemove.HorizontalOptions = LayoutOptions.Start;
                lbRemove.VerticalOptions = LayoutOptions.Center;
                Switch btnRemoveItem = new Switch();
                btnRemoveItem.IsToggled = false;
                btnRemoveItem.HorizontalOptions = LayoutOptions.EndAndExpand;
                btnRemoveItem.VerticalOptions = LayoutOptions.Center;
                StackLayout removeItem = new StackLayout();
                removeItem.Orientation = StackOrientation.Horizontal;
                removeItem.Children.Add(lbRemove);
                removeItem.Children.Add(btnRemoveItem);
                StackLayout item = new StackLayout();
                item.Children.Add(imgPic);
                item.Children.Add(lbComment);
                item.Children.Add(txtPicNotes);
                item.Children.Add(removeItem);
                lvPhotos.Children.Add(item);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Return display image from file.
        /// </summary>
        /// <param name="v_sFilePath"></param>
        /// <returns></returns>
        private async Task<Main.DisplayImageDetails> ReturnDisplayImageFromFile(string v_sFilePath)
        {

            Main.DisplayImageDetails didReturn = new Main.DisplayImageDetails();
            try
            {
                //Get image.
                object bmpImage = await DependencyService.Get<IMain>().ReturnImageFromFile(v_sFilePath);

                //Record original dimensions
                didReturn.dOriginalWidth = DependencyService.Get<IMain>().ReturnWidthBitmapImage(bmpImage);
                didReturn.dOriginalHeight = DependencyService.Get<IMain>().ReturnHeightBitmapImage(bmpImage);

                //Fetch maximum image dimension for preview.
                decimal dMaxDimension = Convert.ToDecimal(DependencyService.Get<IMain>().GetAppResourceValue("Max_Image_Dimension_Preview"));

                //Work out new dimensions for displaying in grid.
                Size szNewDimensions = Main.ReturnAspectRatio(didReturn.dOriginalWidth, didReturn.dOriginalHeight, dMaxDimension);

                //Resize image.
                didReturn.wbBitmap = await DependencyService.Get<IMain>().ReadAndResizeImageFile(v_sFilePath, szNewDimensions);

                //
                return didReturn;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return didReturn;

            }
        }

        private async void btnSaveChanges_Click()
        {
            bool bSaveOK = false;
            try
            {


                bSaveOK = await SavePhotos();

                if (bSaveOK == true)
                {

                    //Go back.
                    Device.BeginInvokeOnMainThread(() => Navigation.PopAsync());

                }


            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// v1.0.2 - Save Photos
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SavePhotos()
        {


            //Disable screen controls
            //await this.EnableScreenControls(false);

            try
            {

                string sFileName = string.Empty;
                object sfNewFile = null;//StorageFile sfNewFile = null;
                object sfOldFile = null;//StorageFile sfOldFile = null;
                bool bSavedOK = false;

                //Fetch maximum image dimension.
                decimal dMaxDimension = Convert.ToDecimal(DependencyService.Get<IMain>().GetAppResourceValue("Max_Image_Dimension"));

                //For recording image sizes when resizing.
                Size szNewSize;

                //Retrieve file properties, we need the modified date.
                object bpFile = null;//Windows.Storage.FileProperties.BasicProperties bpFile = null;
                int index = 0;
                //Loop through photos in grid view.
                foreach (DisplayPhoto cPhoto in Main.p_cProjectPhotos)
                {
                    StackLayout parentStack = (StackLayout)lvPhotos.Children[index];
                    StackLayout swtichStack = (StackLayout)parentStack.Children[3];
                    Switch switchItem = (Switch)swtichStack.Children[1];
                    if (switchItem.IsToggled == true)
                    {
                        DisplayPhoto cRemovePhoto = new DisplayPhoto();
                        cRemovePhoto.FilePath = cPhoto.FilePath;
                        cRemovePhoto.IDKey = cPhoto.IDKey;
                        Main.p_cDeletedPhotos.Add(cRemovePhoto);
                        continue;
                    }
                    index++;
                    try
                    {

                        //Create storage file object of existing file.
                        sfOldFile = await DependencyService.Get<ISettings>().GetFileFromPath(cPhoto.FilePath);

                        //Extract file name.
                        sFileName = DependencyService.Get<ISettings>().GetFileName(cPhoto.FilePath);

                        //This is a new photo.
                        if (cPhoto.IDKey == -1)
                        {

                            //Check file name is in correct format, if not create new one.
                            if (Main.IsImageFileInCorrectFormat(sFileName, this.m_cProject.SubProjectNo) == false)
                            {
                                sFileName = Main.ReturnCorrectImageNameFormat(sFileName, this.m_cProject.SubProjectNo);
                            }

                            //Create new file ready for copying.
                            sfNewFile = await DependencyService.Get<ISettings>().CreateFile(this.m_sfImageFolder, sFileName);

                            //Calculate new size
                            szNewSize = Main.ReturnAspectRatio(cPhoto.ImageWidth, cPhoto.ImageHeight, dMaxDimension);

                            //Copy and resize image and put into sub project folder.
                            bSavedOK = await DependencyService.Get<IMain>().CopyAndConvertImage(sfOldFile, sfNewFile, szNewSize);

                            //Extract file properties.
                            bpFile = await DependencyService.Get<ISettings>().GetBasicProperties(sfNewFile);

                        }
                        else
                        {

                            //If existing file retrieve date time from there.
                            bpFile = await DependencyService.Get<ISettings>().GetBasicProperties(sfOldFile);


                        }
                        DateTime dtLocalDateTime = DependencyService.Get<ISettings>().GetLocalDateTime(bpFile);
                        //Save file details away to project.
                        bSavedOK = Main.p_cDataAccess.SaveSubProjectFile(
                            this.m_cProject.SubProjectNo, 
                            sFileName, 
                            cPhoto.NoteText,
                            dtLocalDateTime, 
                            true);

                    }
                    catch (Exception ex)
                    {
                        bSavedOK = false;
                    }

                    if (bSavedOK == false)
                    {
                        await DisplayAlert("Error Saving", "An error has occurred try to save the image details away, please try again.", "OK");
                        return false;
                    }
                }

                try
                {

                    //Mark all the photo as deleted.
                    foreach (DisplayPhoto cPhoto in Main.p_cDeletedPhotos)
                    {

                        sFileName = cPhoto.FilePath;

                        //Create storage file object of existing file.
                        sfOldFile = await DependencyService.Get<ISettings>().GetFileFromPath(cPhoto.FilePath);

                        //Delete file.
                        await DependencyService.Get<ISettings>().Delete(sfOldFile);

                        //Update file table.
                        Main.p_cDataAccess.MarkFileAsDeleted(cPhoto.IDKey);

                    }

                }
                catch (Exception ex)
                {
                    //cMain.ReportError(ex, cMain.GetCallerMethodName(), sFileName);

                }


                //Clean up
                Main.p_cLastSelectedPhoto = null;
                Main.p_cProjectPhotos = null;
                Main.p_cDeletedPhotos = null;


                //Disable screen controls
                //await this.EnableScreenControls(true);

                return true;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }
        }

        private async void CameraBtn_Clicked(object sender, EventArgs args)
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "No Camera avaiable.", "OK");
                return;
            }
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg"
            });
            if (file == null)
                return;
            await UpdatePhotoClass(file.Path, -1, string.Empty);
        }
        private async void BrowseBtn_Clicked(object sender, EventArgs args)
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Photos Not Supported", "Permission not granted to photos", "OK");
                return;
            }
            var file = await CrossMedia.Current.PickPhotoAsync();
            if (file == null)
                return;
            await UpdatePhotoClass(file.Path, -1, string.Empty);
        }
    }
}
