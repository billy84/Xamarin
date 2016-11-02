using ANG_ABP_INSTALLER_APP.Common;
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
using System.Collections.ObjectModel;
using ANG_ABP_SURVEYOR_APP_CLASS.Classes;
using ANG_ABP_SURVEYOR_APP_CLASS;
using System.Threading.Tasks;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ANG_ABP_INSTALLER_APP.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class DownloadAllMyProjects : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();


        /// <summary>
        /// List of projects on the device.
        /// </summary>
        private ObservableCollection<cProjectSearch> m_ocProjects = null;

        /// <summary>
        /// 
        /// </summary>
        Windows.UI.Core.CoreDispatcher m_dpDispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;

        /// <summary>
        /// Total projects being processed
        /// </summary>
        int m_iProjectTotal = 0;

        /// <summary>
        /// Current project count
        /// </summary>
        int m_iProjectCount = 0;

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public DownloadAllMyProjects()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        /// <summary>
        /// Page loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {

                this.PopulateProjectsList();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Refresh projects list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefreshProjectsList_Click(object sender, RoutedEventArgs e)
        {


            try
            {

                this.PopulateProjectsList();


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Populate projects list.
        /// </summary>
        private async void PopulateProjectsList()
        {

            await this.EnableScreenControls(false);
            try
            {

                //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, this.lvProjects.Items.Clear);               
                this.m_ocProjects = null;
                

                await this.UpdateDownloadStatus("Checking connection.");

                bool bConnected = await cMain.p_cSettings.IsAXSystemAvailable(true);
                if (bConnected == true)
                {

                    await this.UpdateDownloadStatus("Fetching list of projects assigned to you...");

                    ANG_ABP_SURVEYOR_APP_CLASS.wcfCalls.cAXCalls cWCFCalls = new ANG_ABP_SURVEYOR_APP_CLASS.wcfCalls.cAXCalls();

                    ObservableCollection<ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.SearchResult> srProjects = await cWCFCalls.FetchInstallersProjects();

                    cProjectSearch cProject = null;

                    if (srProjects != null)
                    {

                        this.m_ocProjects = new ObservableCollection<cProjectSearch>();

                        ObservableCollection<cProjectSearch> cLocalProjects = cMain.p_cDataAccess.FetchProjectsForSync();

                        foreach (ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.SearchResult axProject in srProjects)
                        {
                            cProject = new cProjectSearch();
                            cProject.ProjectNo = axProject.ProjectNo;
                            cProject.ProjectName = axProject.ProjectName;
                            cProject.ProjectStatus  = cMain.p_cDataAccess.GetEnumValueName("ProjTable", "Status", Convert.ToInt32(axProject.Status));
                            cProject.ListViewWidth = this.lvProjects.ActualWidth;
                            cProject.IsEnabled = true;                                                       

                            //Check if project is already downloaded.
                            foreach (cProjectSearch cLocalProject in cLocalProjects)
                            {
                                if (cLocalProject.ProjectNo == axProject.ProjectNo)
                                {
                                    cProject.IsEnabled = false;
                                    cProject.Status = "Downloaded (" + cLocalProject.SubProjectQty.ToString() + ")";                                     
                                    break;

                                }
                            }
                            
                            this.m_ocProjects.Add(cProject);
                                
                        }

                        this.lvProjects.ItemsSource = this.m_ocProjects;
                        this.lvProjects.SelectAll();

                                             
                    }

                }


                await this.UpdateDownloadStatus("Ready...");

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


            await this.EnableScreenControls(true);


        }

        /// <summary>
        /// Update download status text block
        /// </summary>
        /// <param name="v_sMessage"></param>
        /// <returns></returns>
        private async Task UpdateDownloadStatus(string v_sMessage)
        {


            try
            {

                await this.m_dpDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    // Your UI update code goes here!
                    this.tbStatus.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                    this.tbStatus.Text = v_sMessage;

                });

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }


        /// <summary>
        /// Update list view status
        /// </summary>
        /// <param name="v_sMessage"></param>
        /// <returns></returns>
        private async Task UpdateListViewStatus(string v_sStatus,string v_sProjectNo,bool v_bIsEnabled)
        {


            try
            {

                await this.m_dpDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    foreach (cProjectSearch cProject in this.m_ocProjects)
                    {

                        if (cProject.ProjectNo == v_sProjectNo)
                        {

                            this.lvProjects.ScrollIntoView(cProject);
                            cProject.Status = v_sStatus;
                            cProject.IsEnabled = v_bIsEnabled;
                           
                            break;

                        }

                    }

                });

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Enable screen controls.
        /// </summary>
        /// <param name="v_bEnable"></param>
        /// <returns></returns>
        private async Task EnableScreenControls(bool v_bEnable)
        {

            try
            {

                await this.m_dpDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    // Your UI update code goes here!
                    this.backButton.IsEnabled = v_bEnable;
                    this.btnRefreshProjectsList.IsEnabled = v_bEnable;
                    this.lvProjects.IsEnabled = v_bEnable;
                    this.btnDownloadProjects.IsEnabled = v_bEnable;
                    this.btnDeSelectAll.IsEnabled = v_bEnable;
                    this.btnSelectAll.IsEnabled = v_bEnable;
                    

                });

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Download selected projects.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDownloadProjects_Click(object sender, RoutedEventArgs e)
        {

            ANG_ABP_SURVEYOR_APP_CLASS.wcfCalls.cAXCalls cAX_WCF = null;
            await this.EnableScreenControls(false);
            try
            {

                this.m_iProjectCount = 0;
                this.m_iProjectTotal = 0;

                
                foreach (cProjectSearch cProject in this.lvProjects.SelectedItems)
                {

                    if (cProject.IsEnabled == true)
                    {
                        this.m_iProjectTotal++;
                    }

                }

                if (this.m_iProjectTotal == 0)
                {

                    await cSettings.DisplayMessage("You have not selected any projects to download, please only select projects that have not already been downloaded.", "Selection Required");
                    await this.EnableScreenControls(true);
                    return;

                }


                cAX_WCF = new ANG_ABP_SURVEYOR_APP_CLASS.wcfCalls.cAXCalls();
                Downloads.cDownloadCommon cAX_Download = new Downloads.cDownloadCommon();
                cAX_Download.UpdateMessage += cAX_Download_UpdateMessage;

                Downloads.cDownloadCommon.DownloadResult drResult;
               

                foreach (cProjectSearch cProject in this.lvProjects.SelectedItems)
                {
                    if (cProject.IsEnabled == true)
                    {

                        this.m_iProjectCount++;

                        await this.UpdateListViewStatus("Downloading....", cProject.ProjectNo,true);

                        drResult = await cAX_Download.DownloadSubProjects(cProject.ProjectNo, cProject.ProjectName);
                      
                        if (drResult.bSuccessful == false)
                        {
                            await this.UpdateListViewStatus("Download Failed...", cProject.ProjectNo,true);
                        }
                        else
                        {
                            await this.UpdateListViewStatus("Downloaded (" + drResult.iSubProjectCount.ToString() + ")     ", cProject.ProjectNo,false);

                        }
                     
                    }
                }

                await this.UpdateDownloadStatus("Download Complete...");
                await this.EnableScreenControls(true);

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
        async void cAX_Download_UpdateMessage(object sender, string e)
        {

            try
            {

                string sMsg = "Project (" + this.m_iProjectCount.ToString() + " of " + this.m_iProjectTotal.ToString() + ") " + e;

                await this.UpdateDownloadStatus(sMsg);

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
        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                this.lvProjects.SelectAll();

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
        private void btnDeSelectAll_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                this.lvProjects.SelectedIndex = -1;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

    }
}
