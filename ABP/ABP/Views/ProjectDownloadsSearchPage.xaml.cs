using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ABP.Views
{
    public partial class ProjectDownloadsSearchPage : ContentPage
    {
        public ProjectDownloadsSearchPage()
        {
            InitializeComponent();
            this.Title = "Project Downloads Search";
            SearchProjectBtn.Source = ImageSource.FromFile(String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "find"));
        }
    }
}
