using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ABP.WcfProxys;

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
            //WcfLogin.m_wcfLogin
            LoginExt.ServiceClient wcf_Login = new LoginExt.ServiceClient();
            wcf_Login.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, 60, 0);
            wcf_Login.Endpoint.Binding.SendTimeout = new TimeSpan(0, 60, 0);
            wcf_Login.Endpoint.Address = new System.ServiceModel.EndpointAddress("https://abpwebtest.anglian-windows.com/ax-logon-ext-test/service.svc");

            //object userState = new object();
            LoginExt.LogonResult userState = new LoginExt.LogonResult();

            //wcf_Login.OpenAsync();
            wcf_Login.LogonCompleted += Wcf_Login_LogonCompleted;
            wcf_Login.LogonAsync(UserName.Text, Password.Text, "c4P41E64sx", userState);
           
            //wcf_Login.CloseAsync();
            //wcf_Login.LogonCompleted()
            
            //Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new MainMenuPage()));
            //await this.Navigation.PushAsync(new MainMenuView());
        }

        private void Wcf_Login_LogonCompleted(object sender, LoginExt.LogonCompletedEventArgs e)
        {
            //throw new NotImplementedException();
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
                    UserName.Text = "";
                    Password.Text = "";
                    UserName.Focus();
                }
                else
                {
                    rResult = e.Result.Token;
                    Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new MainMenuPage()));
                }
            }
        }
    }
}
