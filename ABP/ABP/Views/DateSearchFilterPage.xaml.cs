using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ABP.Views
{
    public partial class DateSearchFilterPage : ContentPage
    {
        public DateSearchFilterPage()
        {
            InitializeComponent();
            this.Title = "Survey Dates Search Filters";
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Search",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "find"),
                Command = new Command(() => SearchFilterBtn_Tapped())
            });
            this.ToolbarItems.Add(new ToolbarItem() { Text = "Reset", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "erase") });
            //var tapSearchBtn = new TapGestureRecognizer();
            //tapSearchBtn.Tapped += SearchBtn_Tapped;
            //SearchBtn.GestureRecognizers.Add(tapSearchBtn);
        }
        private void SearchFilterBtn_Tapped()
        {
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new DateSearchResultPage()));
            //await this.Navigation.PushAsync(new DateSearchResultView(items));
        }
    }
}
