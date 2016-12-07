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
        public PhotoScreenPage(SurveyInputResult ProjectInfo)
        {
            InitializeComponent();
            this.Title = ProjectInfo.ProjectNo + " - " + ProjectInfo.ProjectName;
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
            PhotoItem item = new PhotoItem();

            //item.ImageSource = ImageSource.FromStream(() => {
            //var stream = file.GetStream();
            //file.Dispose();
            //return stream;
            //});
            item.ImageSource = ImageSource.FromFile(file.Path);
            item.NoteText = "Test Comment";
            items.Add(item);
            items.Add(item);
            items.Add(item);
            lvPhotos.ItemsSource = items;
        }
    }
}
