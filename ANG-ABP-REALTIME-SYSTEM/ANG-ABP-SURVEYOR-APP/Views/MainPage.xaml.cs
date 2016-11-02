using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using ANG_ABP_SURVEYOR_APP.Views;
using ANG_ABP_SURVEYOR_APP_CLASS.Model;
using ANG_ABP_SURVEYOR_APP_CLASS;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ANG_ABP_SURVEYOR_APP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page

    {

        /// <summary>
        /// Flag to indicate if page has been loaded.
        /// </summary>
        bool m_bPageLoaded = false;


        public MainPage()
        {
            this.InitializeComponent();


            try
            {

                this.Loaded += MainPage_Loaded;
                Application.Current.Resuming += new EventHandler<Object>(App_Resuming);

                //cMain.SetupDisptcher();
                //cMain.p_cDataAccess.CheckDB();
                //cMain.CheckBackgroundTasks();

                //cMain.p_cDataAccess.CreateEnumTestData();
                //cMain.p_cDataAccess.CreateProjectTestData();
             
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);


            }


        }

        private void App_Resuming(Object sender, Object e)
        {

            cMain.SetupDisptcher();
            // TODO: Refresh network data
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            try
            {
                this.imgUserProfile.Source = null;
                this.gvWorkForTodayAM.ItemsSource = null;
                this.gvWorkForTodayPM.ItemsSource = null;
                GC.Collect();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Page loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                //Set control colors.
                this.SetControlColours();

                //v1.0.7 - Update version number.
                this.UpdateVersion();
                              
                cMain.CheckAXConnection();
                
                //Display user details.
                bool bDisplayOK =  await this.DisplayUserDetails();

                //Update screen totals.
                this.UpdateScreenTotals();

                //Display work details.
                this.DisplayWorkDetails();

                //Update main sync status
                await cMain.UpdateMainSyncStatus();

                //Flag to say everything is loaded.
                this.m_bPageLoaded = true;

                //v1.0.11 - If not in live, change background of front screen.
                if (cMain.p_cDataAccess.AreWeRunningInLive() == false)
                {
                    this.gdMain.Background = new SolidColorBrush(Windows.UI.Colors.IndianRed);
                }
                
              
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
            
        }

        /// <summary>
        /// v1.0.7 - Display version number.
        /// </summary>
        private void UpdateVersion()
        {

            try
            {

                string sVersion = cMain.GetAppResourceValue("ProductVersion");
                this.tbAppVersion.Text = "v" + sVersion;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Display the work that is upcoming on the tiles.
        /// </summary>
        private async void DisplayWorkDetails()
        {

            try
            {

                List<cDashboardWork> lWorksAM = new List<cDashboardWork>();
                List<cDashboardWork> lWorksPM = new List<cDashboardWork>();

                int iInstall_Awaiting = Convert.ToInt32(cMain.GetAppResourceValue("InstallStatus_AwaitingSurvey"));                
                string sUsername = await cSettings.GetUserName();

                List<cProjectTable> lWorksDB = cMain.p_cDataAccess.GetUpComingWork_Surveyor(sUsername,iInstall_Awaiting);
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
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }


        /// <summary>
        /// Set colors of controls
        /// </summary>
        private void SetControlColours()
        {

            try
            {

                //Common background color.
                SolidColorBrush sbBackBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

                this.brSyncDateTime.Background = sbBackBrush;
                this.brTotalToday.Background = sbBackBrush;
                this.brTotalPending.Background = sbBackBrush;
                this.brTotalCompleted.Background = sbBackBrush;


                //Common foreground color.
                SolidColorBrush sbForeBrush = new SolidColorBrush(Windows.UI.Colors.Black);

                this.tbLastSyncDateTime.Foreground = sbForeBrush;
                this.tbAppVersion.Foreground = sbForeBrush;
                this.tbTotalToday.Foreground = sbForeBrush;
                this.tbTotalPending.Foreground = sbForeBrush;
                this.tbTotalCompleted.Foreground = sbForeBrush;


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


        }

        /// <summary>
        /// Display user details on screen.
        /// </summary>
        private async Task<bool> DisplayUserDetails()
        {

            try
            {

                //Retrieve settings.
                cAppSettingsTable cSettings = cMain.p_cDataAccess.ReturnSettings();

                //Check if we need user to setup user settings.
                if (cSettings.ProfilePicPath == null)
                {
                    //No user settings set.
                    this.Frame.Navigate(typeof(UserSettingsPage));

                    return false;

                }
                else
                {

  
                    this.imgUserProfile.Source = await cMain.ReturnImageFromFile(cSettings.ProfilePicPath);
                    this.tbSurveyorName.Text = cSettings.UsersFullName + Environment.NewLine + "ABP Surveyor";
                    this.tbLastSyncDateTime.Text = cMain.ReturnLastSyncString(cSettings.LastSyncDateTime);
                    return true;

                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;


            }


        }

        /// <summary>
        /// User wants to redirect to the user details page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateUserDetails_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                this.Frame.Navigate(typeof(UserSettingsPage));


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }


        /// <summary>
        /// Update screen totals.
        /// </summary>
        private async void UpdateScreenTotals()
        {

            try
            {

                int iInstall_Awaiting = Convert.ToInt32(cMain.GetAppResourceValue("InstallStatus_AwaitingSurvey"));                
                string sUsername = await cSettings.GetUserName();

                //Today Total
                int iTodayQty = cMain.p_cDataAccess.TotalsForToday_Surveyor(sUsername,iInstall_Awaiting);
                this.tbTotalToday.Text = iTodayQty.ToString().PadLeft(2, '0') + Environment.NewLine + "total for today";

                //Completed Total
                int iCompletedQty = cMain.p_cDataAccess.TotalCompleted_Surveyor(sUsername, iInstall_Awaiting);
                this.tbTotalCompleted.Text = iCompletedQty.ToString().PadLeft(2, '0') + Environment.NewLine + "completed";

                //Pending Total
                int iPendingQty = cMain.p_cDataAccess.TotalPending_Surveyor(sUsername, iInstall_Awaiting);
                this.tbTotalPending.Text = iPendingQty.ToString().PadLeft(2, '0') + Environment.NewLine + "pending";


            }
            catch(Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }


        /// <summary>
        /// Redirect to download page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDownloadProject_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                bool bConnected = await cMain.p_cSettings.IsAXSystemAvailable(true);
                if (bConnected == true)
                {
                    this.Frame.Navigate(typeof(DownloadProjectPage));

                }
                
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Set survey dates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetSurveyDates_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                this.Frame.Navigate(typeof(SetSurveyDatesSearchPage));


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


        }

        /// <summary>
        /// Work for today page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWorkForToday_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                this.Frame.Navigate(typeof(SurveyInputSearchPage));


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Total for today 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnTotalForToday_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                int iInstall_Awaiting = Convert.ToInt32(cMain.GetAppResourceValue("InstallStatus_AwaitingSurvey"));   

                cSearchCriteria cCriteria = new cSearchCriteria();
                cCriteria.SurveyPlanDate = DateTime.Now;
                cCriteria.IncludeSurveyPlanDate = true;      
                cCriteria.SurveyPlanDateComparison = cSettings.p_sDateCompare_EqualTo;
                cCriteria.SurveyedStatus = cSettings.p_sAnyStatus; //v1.0.1
                cCriteria.InstallStatus = iInstall_Awaiting;
                cCriteria.Surveyor = await cSettings.GetUserName();

                
                this.Frame.Navigate(typeof(SurveyInputSearchPage),cCriteria);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);


            }
        }

        /// <summary>
        /// Total completed stats button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnTotalCompleted_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                int iInstall_Awaiting = Convert.ToInt32(cMain.GetAppResourceValue("InstallStatus_AwaitingSurvey"));  

                cSearchCriteria cCriteria = new cSearchCriteria();
                cCriteria.SurveyPlanDate = DateTime.Now;
                cCriteria.IncludeSurveyPlanDate = true;  
                cCriteria.SurveyPlanDateComparison = cSettings.p_sDateCompare_EqualTo;
                cCriteria.Surveyor = await cSettings.GetUserName();
                cCriteria.SurveyedStatus = cSettings.p_sAnyStatus;
                cCriteria.SurveyOnSite = cSettings.p_sInputStatus_NotPending;
                cCriteria.InstallStatus = iInstall_Awaiting;

                this.Frame.Navigate(typeof(SurveyInputSearchPage), cCriteria);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnTotalPending_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                int iInstall_Awaiting = Convert.ToInt32(cMain.GetAppResourceValue("InstallStatus_AwaitingSurvey"));  

                cSearchCriteria cCriteria = new cSearchCriteria();
                cCriteria.SurveyPlanDate = DateTime.Now;
                cCriteria.IncludeSurveyPlanDate = true;  
                cCriteria.SurveyPlanDateComparison = cSettings.p_sDateCompare_EqualTo;
                cCriteria.Surveyor = await cSettings.GetUserName();
                cCriteria.SurveyOnSite = cSettings.p_sInputStatus_Pending;
                cCriteria.InstallStatus = iInstall_Awaiting;

                this.Frame.Navigate(typeof(SurveyInputSearchPage), cCriteria);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Send changes only.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendChanges_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                               
                ANG_ABP_SURVEYOR_APP_CLASS.Syncing.cSyncing.p_bSyncChangesOnly = true;

                //Kick off check for syncing.
                cMain.KickOffSyncing();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.1 - Sync all changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSyncAll_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                this.Frame.Navigate(typeof(SyncPage));

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        private void tbOnlineStatus_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

            this.Frame.Navigate(typeof(ConnectionTest));
        }


        /// <summary>
        /// v1.0.19 - AM Tap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvWorkForTodayAM_Tapped(object sender, TappedRoutedEventArgs e)
        {

            try
            {

                this.ProcessWorkForTodayTap(e);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }


        /// <summary>
        /// v1.0.19 - PM Tap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvWorkForTodayPM_Tapped(object sender, TappedRoutedEventArgs e)
        {

            try
            {

                this.ProcessWorkForTodayTap(e);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// v1.0.19 - Process work for today tap.
        /// </summary>
        /// <param name="e"></param>
        private void ProcessWorkForTodayTap(TappedRoutedEventArgs e)
        {

            try
            {

                //If not loaded then exit.
                if (this.m_bPageLoaded == false) { return; };


                if (e.OriginalSource is TextBlock)
                {
                    TextBlock tbText = (TextBlock)e.OriginalSource;
                    cDashboardWork cDash = (cDashboardWork)tbText.DataContext;


                    this.Frame.Navigate(typeof(SurveyInputOptionPage), cDash);

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.19 - Surveyed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSurveyed_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                cSearchCriteria cCriteria = new cSearchCriteria();
                cCriteria.IncludeSurveyPlanDate = false;                           
                cCriteria.SurveyedStatus = cSettings.p_sSurveyedStatus_SurveyedOnSite;
                cCriteria.SurveyedMode = true;

                this.Frame.Navigate(typeof(SurveyInputSearchPage), cCriteria);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.21 - Print schedule.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrintSchedule_Click(object sender, RoutedEventArgs e)
        {

            try
            {
             
                this.Frame.Navigate(typeof(SchedulePrintPage));

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

    }

       
}
