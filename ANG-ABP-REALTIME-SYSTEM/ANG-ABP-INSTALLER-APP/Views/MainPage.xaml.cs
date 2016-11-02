using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
using ANG_ABP_SURVEYOR_APP_CLASS.Model;
using ANG_ABP_SURVEYOR_APP_CLASS.Classes;
using ANG_ABP_INSTALLER_APP.Views;
using ANG_ABP_SURVEYOR_APP_CLASS;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ANG_ABP_INSTALLER_APP
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
        }

        /// <summary>
        /// Download project
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
        /// Set installation dates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetInstallationDates_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                this.Frame.Navigate(typeof(SetInstallationDatesPage));

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Work for today
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWorkForToday_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Frame.Navigate(typeof(InstallationInputSearchPage));

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Send changes
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
        /// Sync everything
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

        /// <summary>
        /// Update user details.
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
        /// Total for today
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTotalForToday_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                cSearchCriteria cCriteria = new cSearchCriteria();
                cCriteria.InstallPlanDate = DateTime.Now;
                cCriteria.InstallPlanDateComparison = cSettings.p_sDateCompare_EqualTo;
                cCriteria.InstallStatus_Filter = cSettings.p_sInstallStatusFilter_EqualTo;
                cCriteria.InstallStatus = -1;
                cCriteria.Booked = true;
                
                this.Frame.Navigate(typeof(InstallationInputSearchPage), cCriteria);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);


            }
        }

        /// <summary>
        /// Total completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTotalCompleted_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                cSearchCriteria cCriteria = new cSearchCriteria();
                cCriteria.InstallPlanDate = DateTime.Now;
                cCriteria.InstallPlanDateComparison = cSettings.p_sDateCompare_EqualTo;
                cCriteria.InstallStatus_Filter = cSettings.p_sInstallStatusFilter_EqualTo;
                cCriteria.InstallStatus = cSettings.p_iInstallStatus_Installing;
                cCriteria.Booked = true;

                this.Frame.Navigate(typeof(InstallationInputSearchPage), cCriteria);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Total pending
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTotalPending_Click(object sender, RoutedEventArgs e)
        {

                            
            try
            {

                cSearchCriteria cCriteria = new cSearchCriteria();
                cCriteria.InstallPlanDate = DateTime.Now;
                cCriteria.InstallPlanDateComparison = cSettings.p_sDateCompare_EqualTo;
                cCriteria.Booked = true;
                cCriteria.InstallStatus_Filter = cSettings.p_sInstallStatusFilter_NotEqualTo;
                cCriteria.InstallStatus = cSettings.p_iInstallStatus_InstalledFully;


                this.Frame.Navigate(typeof(InstallationInputSearchPage), cCriteria);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


        }

        /// <summary>
        /// Work for today
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvTodaysWork_Tapped(object sender, TappedRoutedEventArgs e)
        {

            try
            {

                //If not loaded then exit.
                if (this.m_bPageLoaded == false) { return; };


                if (e.OriginalSource is TextBlock)
                {
                    TextBlock tbText = (TextBlock)e.OriginalSource;
                    cDashboardWork cDash = (cDashboardWork)tbText.DataContext;

                    this.Frame.Navigate(typeof(InstallationInputResultPage), cDash);

                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Online status double tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbOnlineStatus_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

        }

        /// <summary>
        /// Main page loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
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
        /// Display the work that is upcoming on the tiles.
        /// </summary>
        private async void DisplayWorkDetails()
        {

            try
            {

                List<cDashboardWork> lWorks = new List<cDashboardWork>();

                int iInstall_Awaiting = Convert.ToInt32(cMain.GetAppResourceValue("InstallStatus_AwaitingSurvey"));
                int iProgress_Progress = Convert.ToInt32(cMain.GetAppResourceValue("ProgressStatus_AbleToProgress"));
                string sUsername = await cSettings.GetUserName();

                List<cInstallerDashboardResult> lWorksDB = cMain.p_cDataAccess.GetUpComingWork_Installer();
                if (lWorksDB != null)
                {


                    cDashboardWork cWork = null;

                    foreach (cInstallerDashboardResult cWorkDB in lWorksDB)
                    {

                        cWork = new cDashboardWork();

                        //v1.0.5 - Update unit quantities.
                        cWork.TotalUnits = cWorkDB.TotalUnits;
                        cWork.InstalledUnits = cWorkDB.TotalUnitsInstalled;
                        cWork.UnitsNotInstalled = (cWorkDB.TotalUnits - cWorkDB.TotalUnitsInstalled);

                        cWork.SubProjectDisplay = cWorkDB.SubProjectNo + " - Units left: " + cWork.UnitsNotInstalled.ToString();
                        cWork.SubProjectNo = cWorkDB.SubProjectNo;
                        cWork.Header = "Install " + cWorkDB.EndDateTime.Value.ToString("dd/MM/yyyy");
                        cWork.Address = cMain.RemoveNewLinesFromString(cWorkDB.DeliveryStreet);
                        cWork.Name = cWorkDB.ResidentName;
                        cWork.Install = "Install Status: " + cWorkDB.InstallStatusName;
                        cWork.Progress = "Progress Status: " + cWorkDB.ProgressStatusName;                       

                        lWorks.Add(cWork);

                    }

                }

                gvTodaysWork.ItemsSource = lWorks;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Update screen totals.
        /// </summary>
        private void UpdateScreenTotals()
        {

            try
            {               

                //Today Total
                int iTodayQty = cMain.p_cDataAccess.TotalsForToday_Installer();
                this.tbTotalToday.Text = iTodayQty.ToString().PadLeft(2, '0') + Environment.NewLine + "total for today";

                //Completed Total
                int iCompletedQty = cMain.p_cDataAccess.TotalCompleted_Installer();
                this.tbTotalCompleted.Text = iCompletedQty.ToString().PadLeft(2, '0') + Environment.NewLine + "completed";

                //Pending Total
                int iPendingQty = cMain.p_cDataAccess.TotalPending_Installer();
                this.tbTotalPending.Text = iPendingQty.ToString().PadLeft(2, '0') + Environment.NewLine + "pending";


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Display version number.
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
                    this.tbSurveyorName.Text = cSettings.UsersFullName + Environment.NewLine;

                    //v1.0.1 - Add job title if set.
                    if (cSettings.UsersJobTitle != null)
                    {
                        this.tbSurveyorName.Text += cSettings.UsersJobTitle;
                    }
                       

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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetOrderComplete_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                cSearchCriteria cCriteria = new cSearchCriteria();               
                cCriteria.InstallStatus_Filter = cSettings.p_sInstallStatusFilter_EqualTo;
                cCriteria.InstallStatus = cSettings.p_iInstallStatus_InstalledFully;
                cCriteria.IncludeInstallPlanDate = false;
                cCriteria.OrderCompleteMode = true;
                cCriteria.SwitchOffInstallationDateFilter = true;
                
                this.Frame.Navigate(typeof(InstallationInputSearchPage), cCriteria);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }


    }
}
