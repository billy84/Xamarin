using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using ABP.Surveyor.Views;

namespace ABP.Surveyor
{
    public partial class App : Application
    {
        /// <summary>
        /// 
        /// </summary>
        public App()
        {
            InitializeComponent();

            //MainPage = new ABP.Surveyor.MainPage();
            MainPage = new NavigationPage(new LogInView());
            //{
            //    BarBackgroundColor = Color.Black,
            //    BarTextColor = Color.White,
            //    BackgroundColor = Color.Black
                
            //};
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
