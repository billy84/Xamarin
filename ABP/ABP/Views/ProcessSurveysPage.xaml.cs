using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ABP.WcfProxys;
using ABP.Interfaces;
using Acr.UserDialogs;
using ABP.Models;
using ABP.TableModels;

namespace ABP.Views
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
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "configuration")
            });
            TabbedPage_loaded();


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

        private void TabbedPage_loaded()
        {
            cAXCalls cAX = null;
            string sSupportMsg = string.Empty;
            cAX = new cAXCalls();
            if (cSettings.AreWeOnline == true)
            {
                //string sUserProfile = await DependencyService.Get<ISettings>().GetUserName();
                string sUserProfile = WcfLogin.m_instance.LoggedUserName;
                cAX.m_wcfClient.ReturnAreSystemsAvailableCompleted += M_wcfClient_ReturnAreSystemsAvailableCompleted;
                cAX.m_wcfClient.ReturnAreSystemsAvailableAsync(cAX.m_cCompanyName, sUserProfile, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);
                UserDialogs.Instance.ShowLoading("Checking System Available...", MaskType.Black);
            }
            else
            {
                DisplayAlert("Warning", "Internet connection failed.","OK");
            }

        }

        private void M_wcfClient_ReturnAreSystemsAvailableCompleted(object sender, ServiceExt.ReturnAreSystemsAvailableCompletedEventArgs e)
        {
            UserDialogs.Instance.HideLoading();
            if (e.Error != null)
            {
                DisplayAlert("Error", e.Error.Message, "OK");
            }
            else if (e.Cancelled == true)
            {
                DisplayAlert("Error", "Request was canceled.", "OK");
            }
            else
            {
                if (e.Result.SystemsAvailable == true)
                {
                    // display work details
                }
                else
                {
                    DisplayAlert("Error", "System non Available.", "OK");
                }
            }
            //throw new NotImplementedException();
        }

        private void DisplayWorkDetails()
        {
            try
            {
                List<cDashboardWork> lWorksAM = new List<cDashboardWork>();
                List<cDashboardWork> lWorksPM = new List<cDashboardWork>();
                int iInstall_Awaiting = Convert.ToInt32(DependencyService.Get<IMain>().GetAppResourceValue("InstallStatus_AwaitingSurvey"));
                string sUserName = WcfLogin.m_instance.LoggedUserName;
                List<cProjectTable> lWorksDB = cMain.p_cDataAccess.GetUpComingWork_Surveyor(sUserName, iInstall_Awaiting);

                if (lWorksDB != null)
                {
                    cDashboardWork cWork = null;
                    foreach (cProjectTable cWorkDB in lWorksDB)
                    {
                        cWork = new cDashboardWork();
                        cWork.Header = cMain.CreateWorkDisplayTitle(cMain.ConvertNullableDateTimeToDateTime(cWorkDB.EndDateTime));
                        cWork.TelephoneNo = "Tel: " + cWorkDB.ResidentTelNo;
                        cWork.SubProjectNo = cWorkDB.SubProjectNo;
                        cWork.Address = cMain.ReturnAddress(cWorkDB);
                        cWork.Name = cWorkDB.ResidentName;
                        cWork.WorkType = "Real Type: " + cWorkDB.MxmProjDescription;
                        cWork.Progress = "Progress Status: " + cWorkDB.ProgressStatusName;

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
                TodayList.ItemsSource = lWorksAM;
                TomList.ItemsSource = lWorksPM;


            }
            catch (Exception ex)
            {

            }
        }
    }
}
