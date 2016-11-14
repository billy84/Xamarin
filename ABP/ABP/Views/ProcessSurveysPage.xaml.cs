using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ABP.Views
{
    public partial class ProcessSurveysPage : TabbedPage
    {
        public ProcessSurveysPage()
        {
            InitializeComponent();
            this.Title = "Current Bookings";
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Download",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "down"),
                Command = new Command(() => DownBtn_Tapped())
            });
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Sync",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "refresh"),
                Command = new Command(() => SyncBtn_Tapped())
            });
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Search",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "find"),
                Command = new Command(() => SearchBtn_Tapped())
            });
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Setting",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "configuration")
            });
        }
        private void DownBtn_Tapped()
        {
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new ProjectDownloadsSearchPage()));
            //await this.Navigation.PushAsync(new DateSearchResultView(items));
        }
        private void SyncBtn_Tapped()
        {
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new ProjectSyncPage()));
            //await this.Navigation.PushAsync(new DateSearchResultView(items));
        }
        private void SearchBtn_Tapped()
        {
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new ProjectSearchPage()));
            //await this.Navigation.PushAsync(new DateSearchResultView(items));
        }
    }
}
