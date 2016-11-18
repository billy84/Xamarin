using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ABP.WcfProxys;
using Acr.UserDialogs;

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
            if (UserName.Text == null || UserName.Text.Trim() == "")
            {
                DisplayAlert("Error", "Please enter UserName", "OK");
                UserName.Focus();
                return;
            }

            if (Password.Text == null || Password.Text.Trim() == "")
            {
                DisplayAlert("Error", "Please enter Password", "OK");
                Password.Focus();
                return;
            }
            LoginExt.LogonResult userState = new LoginExt.LogonResult();
            WcfLogin.m_instance.m_wcfLogin.LogonCompleted += Wcf_Login_LogonCompleted;
            WcfLogin.m_instance.m_wcfLogin.LogonAsync(UserName.Text, Password.Text, "c4P41E64sx", userState);
            UserDialogs.Instance.ShowLoading("Signning...", MaskType.Black);
        }

        private void Wcf_Login_LogonCompleted(object sender, LoginExt.LogonCompletedEventArgs e)
        {
            string rResult = null;
            if (e.Error != null)
            {
                rResult = e.Error.Message;
            }
            else if (e.Cancelled)
            {
                rResult = "Request was cancelled.";
            }
            else
            {
                if (e.Result.InvalidDetails)
                {
                    DisplayAlert("Warning", "Invalid Username and Password.", "OK");
                    UserDialogs.Instance.HideLoading();
                    UserName.Text = "";
                    Password.Text = "";
                    UserName.Focus();
                }
                else
                {
                    rResult = e.Result.Token;
                    UserDialogs.Instance.HideLoading();
                    WcfLogin.m_instance.Token = e.Result.Token;
                    WcfLogin.m_instance.LoggedUserName = UserName.Text.Trim();
                    Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new MainMenuPage()));
                }
            }
        }
    }
}
