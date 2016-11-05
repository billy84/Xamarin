using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ABP.Surveyor.Views
{
    public partial class MainMenuView : ContentPage
    {
        public MainMenuView()
        {
            InitializeComponent();
            this.Title = "Main Menu Options";
        }
        private async void GotoDateSearchFilterView(object sender, EventArgs args)
        {
            await this.Navigation.PushAsync(new DateSearchFilterView());
        }
    }
}
