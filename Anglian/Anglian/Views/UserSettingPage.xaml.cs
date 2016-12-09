using Anglian.Engine;
using Anglian.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Anglian.Views
{
    public partial class UserSettingPage : ContentPage
    {
        private AppSettingsTable m_cSettings = null;
        private bool bSaveOK = false;
        public UserSettingPage()
        {
            InitializeComponent();
            Title = "User Setting";
            //Read Settings
            this.m_cSettings = Main.p_cDataAccess.ReturnSettings();

            //Set Status
            SetStatus();

            pDatabaseType.SelectedIndexChanged += (sender, e) =>
            {
                if (pDatabaseType.SelectedIndex == 1)
                {
                    var navigationPage = Application.Current.MainPage as NavigationPage;
                    navigationPage.BarBackgroundColor = Color.Pink;
                    this.m_cSettings.RunningMode = "Test";
                }
                else
                {
                    var navigationPage = Application.Current.MainPage as NavigationPage;
                    navigationPage.BarBackgroundColor = Color.Black;
                    this.m_cSettings.RunningMode = "Live";
                }
                //Save user details to DB.
                SaveChanges();
            };
        }

        /// <summary>
        /// Save changes
        /// </summary>
        private async void SaveChanges()
        {
            bool bSaveOK = await Main.p_cDataAccess.SaveSettings(this.m_cSettings);
        }

        /// <summary>
        /// v1.0.11 - Set status switch on screen.
        /// </summary>
        private void SetStatus()
        {
            try
            {


                if (this.m_cSettings != null)
                {

                    if (this.m_cSettings.RunningMode == "Live")
                    {
                        pDatabaseType.SelectedIndex = 0;

                    }
                    else
                    {
                        pDatabaseType.SelectedIndex = 1;

                    }

                }

                //v1.0.11 - If syncing in progress, disable status switch.
                if (Main.p_bIsSyncingInProgress == true)
                {
                    this.Title = "Cannot be changed, syncing in progress.";
                    this.pDatabaseType.IsEnabled = false;
                }

            }
            catch (Exception ex)
            {
                //Main.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                //return false;

            }

        }
    }
}
