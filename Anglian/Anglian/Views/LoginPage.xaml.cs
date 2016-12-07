using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Classes;
using Anglian.Service;
using Xamarin.Forms;

namespace Anglian.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            this.Title = "Anglian ABP Surveyor App";
        }
        private async void GoToMainMenuView(object sender, EventArgs args)
        {
            if (UserName.Text == null || UserName.Text.Trim() == "")
            {
                await DisplayAlert("Error", "Please enter UserName", "OK");
                UserName.Focus();
                return;
            }

            if (Password.Text == null || Password.Text.Trim() == "")
            {
                await DisplayAlert("Error", "Please enter Password", "OK");
                Password.Focus();
                return;
            }
            btnLogin.IsEnabled = false;
            LogonResult result = await DependencyService.Get<ILogon>().LogonAsync(
                UserName.Text.Trim(),
                Password.Text.Trim(),
                Session.AuthID);
            btnLogin.IsEnabled = true;
            if (result.InvalidDetails == true)
            {
                await DisplayAlert("Warning", "Invalid Username and Password.", "OK");
                UserName.Text = "";
                Password.Text = "";
                UserName.Focus();

            }
            else
            {
                DependencyService.Get<ISettings>().SetSessionToLocalSetting(
                    UserName.Text.Trim(), 
                    result.Token,
                    DateTime.Now);
                Session.Token = result.Token;
                Session.CurrentUserName = UserName.Text.Trim();
                Session.LoggedTime = DateTime.Now;
                Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new MainMenuPage()));
            }
        }
    }
}
