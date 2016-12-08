using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Classes;
using Anglian.Engine;
using Anglian.Models;
using Anglian.Service;

using Xamarin.Forms;

namespace Anglian.Views
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
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "configuration"),
                Command = new Command(() => SettingBtn_Tapped())
            });
            TabbedPage_loaded();
        }
        private void SettingBtn_Tapped()
        {
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new UserSettingPage()));
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

        private async void TabbedPage_loaded()
        {
            try
            {
                await Main.CheckAXConnection();
                DisplayWorkDetails();
                if (Main.p_cDataAccess.AreWeRunningInLive() == false)
                {
                    var navigationPage = Application.Current.MainPage as NavigationPage;
                    navigationPage.BarBackgroundColor = Color.Red;
                }
                else
                {
                    var navigationPage = Application.Current.MainPage as NavigationPage;
                    navigationPage.BarBackgroundColor = Color.Black;
                }
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// Display the work that is upcoming on the tiles.
        /// </summary>
        private async void DisplayWorkDetails()
        {

            try
            {

                List<DashboardWork> lWorksAM = new List<DashboardWork>();
                List<DashboardWork> lWorksPM = new List<DashboardWork>();

                int iInstall_Awaiting = Convert.ToInt32(DependencyService.Get<IMain>().GetAppResourceValue("InstallStatus_AwaitingSurvey"));
                string sUsername = await DependencyService.Get<ISettings>().GetUserName();
                if (sUsername == string.Empty || sUsername == " ")
                    sUsername = Session.CurrentUserName;

                List<cProjectTable> lWorksDB = Main.p_cDataAccess.GetUpComingWork_Surveyor(sUsername, iInstall_Awaiting);
                if (lWorksDB != null)
                {


                    DashboardWork cWork = null;

                    foreach (cProjectTable cWorkDB in lWorksDB)
                    {

                        cWork = new DashboardWork();
                        cWork.Header = Main.CreateWorkDisplayTitle(Main.ConvertNullableDateTimeToDateTime(cWorkDB.EndDateTime));
                        cWork.TelephoneNo = "Tel: " + cWorkDB.ResidentTelNo;
                        cWork.SubProjectNo = cWorkDB.SubProjectNo;
                        cWork.Address = Main.ReturnAddress(cWorkDB);
                        cWork.Name = cWorkDB.ResidentName;
                        cWork.WorkType = "Repl Type: " + cWorkDB.MxmProjDescription;
                        cWork.Progress = "Progress Status: " + cWorkDB.ProgressStatusName;

                        //v1.0.19 - Split results by AM and PM.
                        if (cWorkDB.EndDateTime.Value.Hour < 12)
                        {
                            lWorksAM.Add(cWork);

                        }
                        else
                        {
                            lWorksPM.Add(cWork);
                        }


                    }

                }

                gvWorkForTodayAM.ItemsSource = lWorksAM;
                gvWorkForTodayPM.ItemsSource = lWorksPM;


            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }
    }
}
