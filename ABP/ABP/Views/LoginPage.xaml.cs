using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ABP.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            this.Title = "Anglian ABP Surveyor App";
        }
        private void GoToMainMenuView(object sender, EventArgs args)
        {
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new MainMenuPage()));
            //await this.Navigation.PushAsync(new MainMenuView());
        }
    }
}
