using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Anglian.Views;
using Anglian.Service;
using Anglian.Classes;
namespace Anglian
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            string sLatestLoginDate = DependencyService.Get<ISettings>().GetSessionFromLocalSetting("Date");
            DateTime dtLatestDate;
            if (sLatestLoginDate == string.Empty)
            {
                MainPage = new NavigationPage(new LoginPage());
                dtLatestDate = DateTime.Now;
            }
            else
            {
                if (!DateTime.TryParse(sLatestLoginDate, out dtLatestDate))
                {
                    // handle parse failure
                }
                if ((DateTime.Now - dtLatestDate).TotalDays <= 3)
                {
                    Session.CurrentUserName = DependencyService.Get<ISettings>().GetSessionFromLocalSetting("UserName");
                    Session.Token = DependencyService.Get<ISettings>().GetSessionFromLocalSetting("Token");
                    //Session.LoggedTime = DependencyService.Get<ISettings>().GetSessionFromLocalSetting("Date");
                    MainPage = new NavigationPage(new MainMenuPage());
                }
                else
                    MainPage = new NavigationPage(new LoginPage());
            }
            //MainPage = new Anglian.MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
