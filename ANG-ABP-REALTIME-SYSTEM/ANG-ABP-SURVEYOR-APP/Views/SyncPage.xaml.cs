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
using System.Threading.Tasks;
using ANG_ABP_SURVEYOR_APP.Common;
using ANG_ABP_SURVEYOR_APP_CLASS.Classes;
using ANG_ABP_SURVEYOR_APP_CLASS.Syncing;
using System.Text;
using ANG_ABP_SURVEYOR_APP_CLASS;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ANG_ABP_SURVEYOR_APP.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SyncPage : Page
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


        public SyncPage()
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {

                this.PopulateProjectsList();

                //Select all by default.
                this.lvProjects.SelectAll();


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Populate projects list.
        /// </summary>
        private void PopulateProjectsList()
        {

            try
            {

                this.m_ocProjects = cMain.p_cDataAccess.FetchProjectsForSync();

                foreach (cProjectSearch cProject in this.m_ocProjects)
                {

                    cProject.ProjectNo = cProject.ProjectNo;
                    cProject.SubProjectQtyDisplay = cProject.SubProjectQty.ToString();
                    cProject.ListViewWidth = this.lvProjects.ActualWidth;
                    
                }

                this.lvProjects.ItemsSource = this.m_ocProjects;


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Select all projects
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
        /// De-select all projects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeSelectAll_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                this.lvProjects.SelectedItems.Clear();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Start syncing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnStartSyncing_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (this.lvProjects.SelectedItems.Count() == 0)
                {
                    await cSettings.DisplayMessage("You need to select some projects before you can start syncing, please select and try-again.", "Project Selection Required");
                    this.lvProjects.Focus(FocusState.Programmatic);
                    return;
                }

                //v1.0.11 - Check if sync is in progress.
                if (cMain.p_bIsSyncingInProgress == true)
                {
                    await cSettings.DisplayMessage("A sync is currently in progress, please wait until the sync has finished before starting another one.", "Sync in progress");
                    return;
                }

                await this.EnableScreenControls(false);

                cSyncing.p_ocProjectsToSync = new ObservableCollection<cProjectSearch>();
                cSyncing.p_bSyncChangesOnly = false;

                foreach (cProjectSearch cProject in this.lvProjects.SelectedItems)
                {
                    cSyncing.p_ocProjectsToSync.Add(cProject);

                }

                //Start the syncing.
                cMain.KickOffSyncing();


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }


        /// <summary>
        /// Update project sync status
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        public void UpdateProjectsSyncStatus(string v_sProjectNo, string v_sStatus, int v_iSubProjectsAdded, int v_iSubProjectsDeleted)
        {

            try
            {


                int iTotal = 0;
                string sDesc = string.Empty;

                foreach (cProjectSearch cProject in this.m_ocProjects)
                {

                    if (cProject.ProjectNo == v_sProjectNo)
                    {

                        this.lvProjects.ScrollIntoView(cProject);
                        cProject.Status = v_sStatus;

                        //If any additions or reductions then update.
                        if (v_iSubProjectsAdded > 0 || v_iSubProjectsDeleted > 0)
                        {

                            iTotal = (cProject.SubProjectQty + v_iSubProjectsAdded) - v_iSubProjectsDeleted;

                            if (v_iSubProjectsAdded > 0)
                            {
                                sDesc = " Add:" + v_iSubProjectsAdded.ToString();
                            }

                            if (v_iSubProjectsDeleted > 0)
                            {
                                sDesc += " Del:" + v_iSubProjectsDeleted.ToString();
                            }

                            cProject.SubProjectQtyDisplay = iTotal.ToString() + sDesc;

                        }


                        break;

                    }

                }


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
        public async Task EnableScreenControls(bool v_bEnable)
        {

            try
            {

                await this.m_dpDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    // Your UI update code goes here!
                    this.backButton.IsEnabled = v_bEnable;
                    this.lvProjects.IsEnabled = v_bEnable;
                    this.btnStartSyncing.IsEnabled = v_bEnable;
                    this.btnDeSelectAll.IsEnabled = v_bEnable;
                    this.btnSelectAll.IsEnabled = v_bEnable;
                    this.btnDeleteProject.IsEnabled = v_bEnable;

                });

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.2 - Delete project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDeleteProject_Click(object sender, RoutedEventArgs e)
        {

            await this.EnableScreenControls(false);
            try
            {

                if (this.lvProjects.SelectedItems.Count == 0)
                {

                    await cSettings.DisplayMessage("You need to select a project before you can delete.", "Project Selection Required.");
                    this.lvProjects.Focus(FocusState.Programmatic);
                    await this.EnableScreenControls(true);
                    return;

                }

                List<string> lsCannotDelete = new List<string>();
                List<string> lsCanDelete = new List<string>();

                
                

                bool bPendingUpdates = false;
                foreach (cProjectSearch cProject in this.lvProjects.SelectedItems)
                {

                    bPendingUpdates = cMain.p_cDataAccess.DoesProjectHaveAnyPendingUploads(cProject.ProjectNo);
                    if (bPendingUpdates == true)
                    {
                        lsCannotDelete.Add(cProject.ProjectNo);                        
                        
                    }
                    else
                    {
                        lsCanDelete.Add(cProject.ProjectNo);

                    }                   

                }


                StringBuilder sbMsg = new StringBuilder();

                if (lsCannotDelete.Count > 0)
                {

                    sbMsg.Append("The following projects will not be deleted as they have pending uploads");
                    sbMsg.Append(Environment.NewLine);
                    sbMsg.Append(Environment.NewLine);

                    foreach (string sProject in lsCannotDelete)
                    {
                        sbMsg.Append(sProject);
                        sbMsg.Append(Environment.NewLine);

                    }

                    await cSettings.DisplayMessage(sbMsg.ToString(), "Cannot Delete");


                }

                if (lsCanDelete.Count > 0)
                {
                    sbMsg.Append("Are you sure you want to delete the following projects?");
                    sbMsg.Append(Environment.NewLine);
                    sbMsg.Append(Environment.NewLine);

                    foreach (string sProject in lsCanDelete)
                    {
                        sbMsg.Append(sProject);
                        sbMsg.Append(Environment.NewLine);

                    }

                    cSettings.YesNo ynResponse = await cSettings.DisplayYesNo(sbMsg.ToString(), "Confirm");
                    if (ynResponse == cSettings.YesNo.Yes)
                    {


                        foreach (string sProject in lsCanDelete)
                        {

                            cMain.UpdateSyncMainPage("Deleting project (" + sProject + ")");
                            await cMain.p_cDataAccess.DeleteProjectFromDevice(sProject);

                        }

                        this.PopulateProjectsList();
                        cMain.UpdateSyncMainPage("Ready...");

                    }


                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

            await this.EnableScreenControls(true);

        }
    }
}
