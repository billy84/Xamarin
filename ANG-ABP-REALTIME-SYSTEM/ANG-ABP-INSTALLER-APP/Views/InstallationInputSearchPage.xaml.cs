using ANG_ABP_INSTALLER_APP.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ANG_ABP_SURVEYOR_APP_CLASS;
using ANG_ABP_SURVEYOR_APP_CLASS.Classes;
using ANG_ABP_SURVEYOR_APP_CLASS.Model;
using System.Threading.Tasks;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ANG_ABP_INSTALLER_APP.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class InstallationInputSearchPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// v1.0.1 - Flag to indicate that the next text changed event is to be ignored, fired because of restoring search criteria.
        /// </summary>
        private bool m_bIgnoreNextTextChanged_DeliveryStreet = false;

        /// <summary>
        /// v1.0.1 - Flag to indicate that the next text changed event is to be ignored, fired because of restoring search criteria.
        /// </summary>
        private bool m_bIgnoreNextTextChanged_Postcode = false;

        /// <summary>
        /// v1.0.1 - Flag to indicate that the next text changed event is to be ignored, fired because of restoring search criteria.
        /// </summary>
        private bool m_bIgnoreNextTextChanged_SubProjectNo = false;

        /// <summary>
        /// Flag to indicate if page has been loaded.
        /// </summary>
        private bool m_bPageLoaded = false;

        /// <summary>
        /// Navigation mode, how did we get here.
        /// </summary>
        private NavigationMode m_nmNavMode = NavigationMode.New;

        /// <summary>
        /// 
        /// </summary>
        private cSearchCriteria m_cCriteria = null;

        /// <summary>
        /// v1.0.11 - Flag to indicate if we are in order complete mode.
        /// </summary>
        private bool m_bOrderCompleteMode = false;


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


        public InstallationInputSearchPage()
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

            try
            {

                //Record navigation mode.
                this.m_nmNavMode = e.NavigationMode;

                //v1.0.11 - Set the order complete mode, we need to update this ASAP.
                if (e.NavigationMode == NavigationMode.Back)
                {
                    if (cMain.p_cSearchCriteria_LastSearch != null)
                    {
                        this.m_bOrderCompleteMode = cMain.p_cSearchCriteria_LastSearch.OrderCompleteMode;

                    }
                }

                if (e.Parameter != null)
                {
                    if (e.Parameter.GetType() == typeof(cSearchCriteria))
                    {
                        this.m_cCriteria = (cSearchCriteria)e.Parameter;
                        this.m_bOrderCompleteMode = this.m_cCriteria.OrderCompleteMode; //v1.0.11 - Set the order complete mode, we need to update this ASAP.


                    }

                }

                //v1.0.11 - Change page title for order complete mode.
                if (this.m_bOrderCompleteMode == true)
                {
                    this.pageTitle.Text = "Set Order Complete Page.";
                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);

            try
            {

                if (e.NavigationMode == NavigationMode.New)
                {
                    this.RecordCurrentSearchCriteria();
                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        #endregion

   

        /// <summary>
        /// Set install date button.
        /// </summary>
        /// <param name="v_dDate"></param>
        private void SetInstallDateButton(DateTime v_dDate)
        {

            try
            {

                //Set button properties.
                this.btnSetInstallDate.Tag = v_dDate;
                this.btnSetInstallDate.Content = cMain.ReturnDisplayDate(v_dDate, this.cmbDateCompare.SelectedItem.ToString());

                //Display search results
                this.DisplaySearchResults();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), v_dDate.ToString());

            }

        }

        /// <summary>
        /// Restore saved search criteria.
        /// </summary>
        private void RestoreSearchCriteria()
        {

            try
            {

                //If not set then leave.
                if (cMain.p_cSearchCriteria_LastSearch == null) { return; }

                int iIndex = 0;


                //v1.0.1 - 
                this.cmbProjectNoFilter.SelectedItem = cMain.p_cSearchCriteria_LastSearch.ProjectNoFilter;
                if (cMain.p_cSearchCriteria_LastSearch.ProjectNoFilter.Equals(cSettings.p_sProjectNoFilter_ProjectNo) == true)
                {

                    //v1.0.5 - Using combo box instead of string.
                    iIndex = 0;
                    foreach (ComboBoxItem cmbItem in this.cmbProjectNo.Items)
                    {
                        if (cmbItem.Tag.ToString() == cMain.p_cSearchCriteria_LastSearch.ProjectNo)
                        {
                            this.cmbProjectNo.SelectedIndex = iIndex;
                            break;
                        }

                        iIndex++;
                    }
                    
                }
                else
                {

                    if (cMain.p_cSearchCriteria_LastSearch.ProjectNo.Length > 0)
                    {
                        this.m_bIgnoreNextTextChanged_SubProjectNo = true;
                        this.txtSubProjectNoFilter.Text = cMain.p_cSearchCriteria_LastSearch.ProjectNo;
                    }

                }

                //Street address
                if (cMain.p_cSearchCriteria_LastSearch.StreetAddress.Length > 0)
                {
                    this.m_bIgnoreNextTextChanged_DeliveryStreet = true;
                    this.txtDeliveryStreet.Text = cMain.p_cSearchCriteria_LastSearch.StreetAddress;
                }

                //Post code
                if (cMain.p_cSearchCriteria_LastSearch.Postcode.Length > 0)
                {
                    this.m_bIgnoreNextTextChanged_Postcode = true;
                    this.txtPostcode.Text = cMain.p_cSearchCriteria_LastSearch.Postcode;
                }


                iIndex = 0;
                foreach (ComboBoxItem cmbItem in this.cmbInstallStatus.Items)
                {
                    if (Convert.ToInt32(cmbItem.Tag) == cMain.p_cSearchCriteria_LastSearch.InstallStatus)
                    {
                        this.cmbInstallStatus.SelectedIndex = iIndex;
                        break;
                    }

                    iIndex++;
                }

                //Progress status
                iIndex = 0;
                foreach (ComboBoxItem cmbItem in this.cmbProgressStatus.Items)
                {
                    if (Convert.ToInt32(cmbItem.Tag) == cMain.p_cSearchCriteria_LastSearch.ProgressStatus)
                    {
                        this.cmbProgressStatus.SelectedIndex = iIndex;
                        break;
                    }

                    iIndex++;
                }

                this.chkUseInstallPlanDate.IsChecked = cMain.p_cSearchCriteria_LastSearch.IncludeInstallPlanDate;
                this.btnSetInstallDate.IsEnabled = cMain.p_cSearchCriteria_LastSearch.IncludeInstallPlanDate;

                this.cmbDateCompare.SelectedItem = cMain.p_cSearchCriteria_LastSearch.InstallPlanDateComparison;
                this.SetInstallDateButton(cMain.p_cSearchCriteria_LastSearch.InstallPlanDate.Value);

                
                this.chkSyncChangesOnly.IsChecked = cMain.p_cSearchCriteria_LastSearch.SyncChangesOnly;

                //v1.0.1
                this.chkShowAllStatuses.IsChecked = cMain.p_cSearchCriteria_LastSearch.ShowAllStatuses;
                this.chkShowAllProgessStatus.IsChecked = cMain.p_cSearchCriteria_LastSearch.ShowAllProgressStatuses;

                //New
                this.chkBooked.IsChecked = cMain.p_cSearchCriteria_LastSearch.Booked;
                this.cmbInstallStatus_Filter.SelectedItem = cMain.p_cSearchCriteria_LastSearch.InstallStatus_Filter;


                //v1.0.11 - Restore order complete values.
                this.m_bOrderCompleteMode = cMain.p_cSearchCriteria_LastSearch.OrderCompleteMode;
                this.cmbOrderCompleteDateFilter.SelectedItem = cMain.p_cSearchCriteria_LastSearch.OrderComplete_Filter;
                

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Record current search criteria
        /// </summary>
        private void RecordCurrentSearchCriteria()
        {

            try
            {

                ComboBoxItem cmbItem = null;
                cMain.p_cSearchCriteria_LastSearch = new cSearchCriteria();

                //v1.0.1 - 
                cMain.p_cSearchCriteria_LastSearch.ProjectNoFilter = this.cmbProjectNoFilter.SelectedItem.ToString();
                if (cMain.p_cSearchCriteria_LastSearch.ProjectNoFilter.Equals(cSettings.p_sProjectNoFilter_ProjectNo) == true)
                {
                    //v1.0.5 - Using combo box item instead of string.
                    cmbItem = (ComboBoxItem)this.cmbProjectNo.SelectedItem;
                    cMain.p_cSearchCriteria_LastSearch.ProjectNo = cmbItem.Tag.ToString();
                }
                else
                {
                    cMain.p_cSearchCriteria_LastSearch.ProjectNo = this.txtSubProjectNoFilter.Text;
                }

                cMain.p_cSearchCriteria_LastSearch.StreetAddress = this.txtDeliveryStreet.Text;
                cMain.p_cSearchCriteria_LastSearch.Postcode = this.txtPostcode.Text;

                cmbItem = (ComboBoxItem)this.cmbInstallStatus.SelectedItem;
                cMain.p_cSearchCriteria_LastSearch.InstallStatus = Convert.ToInt32(cmbItem.Tag);

                //Progress status
                cmbItem = (ComboBoxItem)this.cmbProgressStatus.SelectedItem;
                cMain.p_cSearchCriteria_LastSearch.ProgressStatus = Convert.ToInt32(cmbItem.Tag);    

                cMain.p_cSearchCriteria_LastSearch.IncludeInstallPlanDate = this.chkUseInstallPlanDate.IsChecked.Value;
                cMain.p_cSearchCriteria_LastSearch.InstallPlanDate = (DateTime)this.btnSetInstallDate.Tag;
                cMain.p_cSearchCriteria_LastSearch.InstallPlanDateComparison = this.cmbDateCompare.SelectedItem.ToString();

                cMain.p_cSearchCriteria_LastSearch.SyncChangesOnly = this.chkSyncChangesOnly.IsChecked.Value;

                cSurveyInputResult cResult = (cSurveyInputResult)this.lvResults.SelectedItem;
                if (cResult != null)
                {
                    cMain.p_cSearchCriteria_LastSearch.Selected_SubProjectNo = cResult.SubProjectNo;
                }

                //v1.0.1
                cMain.p_cSearchCriteria_LastSearch.ShowAllStatuses = this.chkShowAllStatuses.IsChecked.Value;
                cMain.p_cSearchCriteria_LastSearch.ShowAllProgressStatuses = this.chkShowAllProgessStatus.IsChecked.Value;

                //New.
                cMain.p_cSearchCriteria_LastSearch.Booked = this.chkBooked.IsChecked.Value;
                cMain.p_cSearchCriteria_LastSearch.InstallStatus_Filter = this.cmbInstallStatus_Filter.SelectedItem.ToString();


                //v1.0.11 - Record order complete values.
                cMain.p_cSearchCriteria_LastSearch.OrderCompleteMode = this.m_bOrderCompleteMode;
                cMain.p_cSearchCriteria_LastSearch.OrderComplete_Filter = this.cmbOrderCompleteDateFilter.SelectedItem.ToString();


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Reselect previously selected sub project.
        /// </summary>
        private void ReselectSubProject()
        {

            try
            {

                if (this.m_nmNavMode == NavigationMode.Back)
                {

                    if (cMain.p_cSearchCriteria_LastSearch != null)
                    {

                        int iIndex = 0;

                        foreach (cSurveyInputResult cResult in this.lvResults.Items)
                        {

                            if (cResult.SubProjectNo == cMain.p_cSearchCriteria_LastSearch.Selected_SubProjectNo)
                            {
                                this.lvResults.ScrollIntoView(cResult);
                                this.lvResults.SelectedIndex = iIndex;

                                break;
                            }

                            iIndex++;

                        }

                    }

                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Page Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InstallationInputSearchPage_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {

                this.PopulateDropDowns();

                this.ConfigureSearchCriteria();

                this.SetInstallDateButton(DateTime.Now);

                if (this.m_nmNavMode == NavigationMode.Back)
                {
                    this.RestoreSearchCriteria();
                }

                this.m_bPageLoaded = true;

                //Process search
                this.DisplaySearchResults();

                //Reselect sub project.
                this.ReselectSubProject();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Populate drop downs.
        /// </summary>
        private void PopulateDropDowns()
        {

            ComboBoxItem cmbItem = null;
            int iIndex = 0;
            try
            {

                // ** Project numbers drop down.
                cmbItem = new ComboBoxItem();
                cmbItem.Content = cSettings.p_sAnyStatus;
                cmbItem.Tag = cSettings.p_sAnyStatus;
                this.cmbProjectNo.Items.Add(cmbItem);

                List<cProjectNo> lsProjectNos = cMain.p_cDataAccess.GetProjectNos();
                foreach (cProjectNo lsProjectNo in lsProjectNos)
                {
                    if (lsProjectNo.ProjectNo.Length > 0)
                    {
                        //v1.0.5 - Display project name as well number.
                        cmbItem = new ComboBoxItem();
                        cmbItem.Content = lsProjectNo.ProjectNo + " - " + cMain.RemoveNewLinesFromString(lsProjectNo.ProjectName);
                        cmbItem.Tag = lsProjectNo.ProjectNo;

                        this.cmbProjectNo.Items.Add(cmbItem);

                    }

                }
                lsProjectNos = null;
                this.cmbProjectNo.SelectedIndex = 0; //Make cMain.p_sAnyStatus selection visible.


                // ** Install status drop down.
                cmbItem = new ComboBoxItem();
                cmbItem.Content = cSettings.p_sAnyStatus;
                cmbItem.Tag = "-1";
                this.cmbInstallStatus.Items.Add(cmbItem);

                //Install status filters
                this.cmbInstallStatus_Filter.Items.Add(cSettings.p_sInstallStatusFilter_EqualTo);
                this.cmbInstallStatus_Filter.Items.Add(cSettings.p_sInstallStatusFilter_NotEqualTo);
                this.cmbInstallStatus_Filter.SelectedIndex = 1;

                //Populate install status drop downs.
                List<cBaseEnumsTable> oInstalls = cMain.p_cDataAccess.GetEnumsForField("MXM1002INSTALLSTATUS");
                foreach (cBaseEnumsTable oInstall in oInstalls)
                {
                    cmbItem = new ComboBoxItem();
                    cmbItem.Content = oInstall.EnumName;
                    cmbItem.Tag = oInstall.EnumValue;
                    this.cmbInstallStatus.Items.Add(cmbItem);

                    if (oInstall.EnumValue == cSettings.p_iInstallStatus_InstalledFully)
                    {
                        iIndex = this.cmbInstallStatus.Items.Count - 1;
                    }

                }
                this.cmbInstallStatus.SelectedIndex = iIndex;

                //Progress status drop down.
                cmbItem = new ComboBoxItem();
                cmbItem.Content = cSettings.p_sAnyStatus;
                cmbItem.Tag = "-1";
                this.cmbProgressStatus.Items.Add(cmbItem);

                List<cBaseEnumsTable> oProgresses = cMain.p_cDataAccess.GetEnumsForField("Mxm1002ProgressStatus");
                foreach (cBaseEnumsTable oProgress in oProgresses)
                {
                    cmbItem = new ComboBoxItem();
                    cmbItem.Content = oProgress.EnumName;
                    cmbItem.Tag = oProgress.EnumValue;
                    this.cmbProgressStatus.Items.Add(cmbItem);

                }
                this.cmbProgressStatus.SelectedIndex = 0; //Make Any the default selection
              
                //Data comparison drop down.
                this.cmbDateCompare.Items.Add(cSettings.p_sDateCompare_EqualTo);
                this.cmbDateCompare.Items.Add(cSettings.p_sDateCompare_GreaterThan);
                this.cmbDateCompare.Items.Add(cSettings.p_sDateCompare_LessThan);
                this.cmbDateCompare.SelectedIndex = 0;

                //v1.0.1 - Project no filter
                this.cmbProjectNoFilter.Items.Add(cSettings.p_sProjectNoFilter_ProjectNo);
                this.cmbProjectNoFilter.Items.Add(cSettings.p_sProjectNoFilter_SubProjectNo);
                this.cmbProjectNoFilter.SelectedIndex = 0;

                //v1.0.3 - Default to true.
                this.chkUseInstallPlanDate.IsChecked = true;

                //v1.0.11 - Populate order complete drop down.
                this.cmbOrderCompleteDateFilter.Items.Add(cSettings.p_sAnyStatus);
                this.cmbOrderCompleteDateFilter.Items.Add(cSettings.p_sConfirmedStatus_Yes);
                this.cmbOrderCompleteDateFilter.Items.Add(cSettings.p_sConfirmedStatus_No);
                this.cmbOrderCompleteDateFilter.SelectedIndex = 2; //Default to NO
    

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }


        /// <summary>
        /// Configure search criteria.
        /// </summary>
        private void ConfigureSearchCriteria()
        {

            try
            {


                int iIndex = -1;

                if (this.m_cCriteria != null)
                {

                    //Project no
                    if (this.m_cCriteria.ProjectNo != null)
                    {

                        if (this.m_cCriteria.ProjectNo.Length > 0)
                        {


                            //Project No
                            foreach (ComboBoxItem cmbItem in this.cmbProjectNo.Items)
                            {

                                if (cmbItem.Content.ToString() == this.m_cCriteria.ProjectNo)
                                {

                                    this.cmbProjectNo.SelectedIndex = iIndex;
                                    break;

                                }
                                iIndex++;

                            }

                        }

                    }


                    //Street Address
                    if (this.m_cCriteria.StreetAddress != null)
                    {
                        this.txtDeliveryStreet.Text = this.m_cCriteria.StreetAddress;

                    }

                    //Postcode
                    if (this.m_cCriteria.Postcode != null)
                    {
                        this.txtPostcode.Text = this.m_cCriteria.Postcode;

                    }

                    //Install status filter
                    if (this.m_cCriteria.InstallStatus_Filter != null)
                    {

                        this.cmbInstallStatus_Filter.SelectedItem = this.m_cCriteria.InstallStatus_Filter;
                        
                    }

                    //Install Status
                    if (this.m_cCriteria.InstallStatus.HasValue == true)
                    {


                       

                        iIndex = 0;

                        foreach (ComboBoxItem cmbItem in this.cmbInstallStatus.Items)
                        {

                            if (cmbItem.Tag != null)
                            {
                                if (cmbItem.Tag.ToString() == this.m_cCriteria.InstallStatus.Value.ToString())
                                {

                                    this.cmbInstallStatus.SelectedIndex = iIndex;
                                    break;

                                }
                            }

                            iIndex++;

                        }

                    }


                    //Survey plan date.
                    if (this.m_cCriteria.InstallPlanDate.HasValue == true)
                    {
                        this.chkUseInstallPlanDate.IsChecked = true;

                        dtPicker.Value = this.m_cCriteria.InstallPlanDate.Value;

                        //Set the date time on the survey button.
                        this.SetInstallDateButton(dtPicker.Value);


                        //this.m_cCriteria.InstallPlanDateComparison;

                    }

                    //v1.0.11 - Switch off installation date filter.
                    if (this.m_cCriteria.SwitchOffInstallationDateFilter == true)
                    {
                        this.chkUseInstallPlanDate.IsChecked = false;
                    }

                    this.chkBooked.IsChecked = this.m_cCriteria.Booked;                    

                }


                //v1.0.11 - Configure if order complete is required.
                if (this.m_bOrderCompleteMode == true)
                {

                    this.tbUnitsLeft.Visibility = Visibility.Collapsed;
                    this.tbOrderCompleteDate.Visibility = Visibility.Visible;
                    this.cmbOrderCompleteDateFilter.Visibility = Visibility.Visible;
                    this.btnCompleteInstallation.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    this.btnSetOrderCompleteDate.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {

                    this.tbUnitsLeft.Visibility = Visibility.Visible;
                    this.tbOrderCompleteDate.Visibility = Visibility.Collapsed;
                    this.cmbOrderCompleteDateFilter.Visibility = Visibility.Collapsed;
                    this.btnCompleteInstallation.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    this.btnSetOrderCompleteDate.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


        }


        /// <summary>
        /// Display search results
        /// </summary>
        private void DisplaySearchResults()
        {

            try
            {

                //Go no further if page is not loaded.
                if (this.m_bPageLoaded == false)
                {
                    return;
                }

                //Extract Project Number.
                string sProjectNo = null;
                string sSubProjectNo = null; //v1.0.1
                ComboBoxItem cbiItem = null;

                if (this.cmbProjectNoFilter.SelectedItem.ToString().Equals(cSettings.p_sProjectNoFilter_ProjectNo) == true)
                {
                    //v1.0.5 - Now using combo box item instead of string.
                    cbiItem = (ComboBoxItem)this.cmbProjectNo.SelectedItem;                   
                    if (cbiItem.Tag.ToString() != cSettings.p_sAnyStatus)
                    {
                        sProjectNo = cbiItem.Tag.ToString();

                    }

                }
                else if (this.cmbProjectNoFilter.SelectedItem.ToString().Equals(cSettings.p_sProjectNoFilter_SubProjectNo) == true)
                {
                    //v1.0.1 - Sub Project number filter.
                    sSubProjectNo = this.txtSubProjectNoFilter.Text;
                }

                //Install status
                Int32 iInstallStatus = -1;
                cbiItem = (ComboBoxItem)this.cmbInstallStatus.SelectedItem;
                string sTagValue = cbiItem.Tag.ToString();
                if (int.TryParse(sTagValue, out iInstallStatus) == true)
                {

                }

                //Progress status
                Int32 iProgressStatus = -1;
                cbiItem = (ComboBoxItem)this.cmbProgressStatus.SelectedItem;
                sTagValue = cbiItem.Tag.ToString();
                if (int.TryParse(sTagValue, out iProgressStatus) == true)
                {

                }

                //Install Date.
                DateTime? dInstallDate = null;
                if (this.chkUseInstallPlanDate.IsChecked == true)
                {
                    dInstallDate = this.dtPicker.Value;

                }

              
                //Date comparison
                string sDateComparison = this.cmbDateCompare.SelectedItem.ToString();

                int iInstall_Awaiting = Convert.ToInt32(cMain.GetAppResourceValue("InstallStatus_AwaitingSurvey"));
                int iInstall_Cancel = Convert.ToInt32(cMain.GetAppResourceValue("InstallStatus_SurveyCancelled"));

                //Install status compare.
                string sInstallStatusCompare = this.cmbInstallStatus_Filter.SelectedItem.ToString();

                //v1.0.11 - Set order complete filter depending on mode.
                string sOrderComplateFilter = cSettings.p_sAnyStatus;
                if (this.m_bOrderCompleteMode == true)
                {
                    sOrderComplateFilter = this.cmbOrderCompleteDateFilter.SelectedItem.ToString();
                }

                List<cSurveyInputResult> cResults = cMain.p_cDataAccess.SearchSurveyInput(sProjectNo, this.txtDeliveryStreet.Text, this.txtPostcode.Text, iInstallStatus, iProgressStatus, dInstallDate, sDateComparison, string.Empty, string.Empty, string.Empty, this.chkSyncChangesOnly.IsChecked.Value, this.chkShowAllStatuses.IsChecked.Value, this.chkShowAllProgessStatus.IsChecked.Value, sSubProjectNo, iInstall_Awaiting, iInstall_Cancel, this.chkBooked.IsChecked.Value, sInstallStatusCompare, sOrderComplateFilter);

                int iUpdates = 0;
                int iTotalUnits = 0;
                int iUnitsInstalled = 0;
                bool bPartInstall = false;
                bool bOnHold = false;
                foreach (cSurveyInputResult cResult in cResults)
                {

                    bPartInstall = false;
                    bOnHold = false;

                    cResult.ScreenWidth = this.lvResults.ActualWidth;
                    cResult.StreetWidth = this.txtDeliveryStreet.ActualWidth; // +10;
                    cResult.InstallWidth = this.cmbInstallStatus.ActualWidth; // +20;

                    cResult.PostcodeWidth = this.txtPostcode.ActualWidth;
                    cResult.ProgressWidth = this.cmbProgressStatus.ActualWidth;

                    //v1.0.10 - If confirmed action date time set then add a B.
                    if (cResult.ConfirmedActionDateTime.HasValue == true)
                    {
                        cResult.Flags += "B";

                    }

                    if (cResult.NotesQty != null)
                    {

                        if (int.TryParse(cResult.NotesQty, out iUpdates) == true)
                        {

                            if (iUpdates > 0)
                            {
                                cResult.Flags += "*";

                            }

                        }
                    }


                    if (cResult.UpdateQty != null)
                    {
                        if (int.TryParse(cResult.UpdateQty, out iUpdates) == true)
                        {

                            if (iUpdates > 0)
                            {
                                cResult.Flags += "x";

                            }

                        }

                    }




                    //Display appropriate surveyed status.
                    cResult.SurveyedStatus = cSettings.p_sSurveyedStatus_NotSurveyed;
                    if (cResult.MXM1002TrfDate.HasValue == true)
                    {
                        if (cResult.Mxm1002InstallStatus == iInstall_Awaiting || cResult.Mxm1002InstallStatus == iInstall_Cancel)
                        {
                            cResult.SurveyedStatus = cSettings.p_sSurveyedStatus_SurveyedOnSite;

                        }

                    }

                    if (cResult.Mxm1002InstallStatus != iInstall_Awaiting && cResult.Mxm1002InstallStatus != iInstall_Cancel)
                    {
                        cResult.SurveyedStatus = cSettings.p_sSurveyedStatus_SurveyedTrans;

                    }


                    //Survey input status
                    if (cResult.MXM1002TrfDate.HasValue == true)
                    {
                        cResult.SurveyInputStatus = cSettings.p_sInputStatus_Successful;
                    }
                    else if (cResult.MXM1002TrfDate.HasValue == false)
                    {
                        cResult.SurveyInputStatus = cSettings.p_sInputStatus_Pending;

                        if (cResult.MxmConfirmedAppointmentIndicator.HasValue == false || cResult.MxmConfirmedAppointmentIndicator.Value == false)
                        {
                            if (cResult.EndDateTime.HasValue == true && cResult.StartDateTime.HasValue == true)
                            {
                                cResult.SurveyInputStatus = cSettings.p_sInputStatus_Failed;

                            }


                        }

                    }


                    //Remove unwanted new lines.
                    cResult.DeliveryStreet = cMain.RemoveNewLinesFromString(cResult.DeliveryStreet);
                  

                    //v1.0.1 - Display formatted survey date - time
                    if (cResult.EndDateTime.HasValue == true)
                    {
                        cResult.SurveyDisplayDateTime = cMain.ReturnDisplayDate(cResult.EndDateTime.Value);
                    }


                    iTotalUnits = 0;
                    if (cResult.TotalUnits.HasValue == true)
                    {
                        iTotalUnits = cResult.TotalUnits.Value;
                    }

                    iUnitsInstalled = 0;
                    if (cResult.TotalUnitsInstalled.HasValue == true)
                    {
                        iUnitsInstalled = cResult.TotalUnitsInstalled.Value;
                    }

                    if (this.m_bOrderCompleteMode == false)
                    {
                        cResult.UnitsLeft = "0";
                        if (iTotalUnits > 0)
                        {
                            cResult.UnitsLeft = (iTotalUnits - iUnitsInstalled).ToString();
                        }

                    }
                    else
                    {
                        if (cResult.ABPAWOrderCompletedDate.HasValue == true)
                        {
                            if (cResult.ABPAWOrderCompletedDate.Value.Year > cSettings.p_dDefaultDBDate.Year)
                            {
                                cResult.UnitsLeft = cResult.ABPAWOrderCompletedDate.Value.ToString("dd/MM/yy");
                            }
                        }
                         

                    }
                    


                    //v1.0.11 - Updated to display contract name and units installed.
                    cResult.ToolTipText = "Contract Name: " + cResult.ProjectName + Environment.NewLine + "Units Installed: " + iUnitsInstalled.ToString();
                    cResult.ToolTipText += Environment.NewLine + "Delivery Date: " + cMain.ReturnDeliveryDisplayDate(cResult.Delivery_EndDateTime);
                    
                   
                    //If only partially installed highlight row background
                    if (iTotalUnits != iUnitsInstalled && cResult.Mxm1002InstallStatus == cSettings.p_iInstallStatus_InstalledPart)
                    {
                        bPartInstall = true;
                        cResult.BackgroundColour = cSettings.p_sInstall_ListView_PartialInstall_Background;
                    }
                    else
                    {
                        cResult.BackgroundColour = cSettings.p_sInstall_ListView_Normal_Background;
                    }

                    //v1.0.2 - Set background color if projects status is "On Hold"
                    if (cResult.Status == cSettings.p_iProjectStatus_OnHold)
                    {
                        bOnHold = true;
                        cResult.BackgroundColour = cSettings.p_sSurvey_ListView_OnHold_Background;
                    }
                    else
                    {
                        cResult.BackgroundColour = cSettings.p_sSurvey_ListView_Normal_Background;
                    }


                    //If on hold and part install display different color.
                    if (bOnHold == true && bPartInstall == true)
                    {
                        cResult.BackgroundColour = cSettings.p_sInstall_ListView_OnHold_PartInstall_Background;
                    }

                }

                this.lvResults.ItemsSource = cResults;


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);


            }

        }

        /// <summary>
        /// Apply selected date to search criteria.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApplySelectedDate_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //Set the date time on the survey button.
                this.SetInstallDateButton(dtPicker.Value);

                //Close pop up.
                LightDismissAnimatedPopup.IsOpen = false;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// View property on google map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnViewOnMap_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //Check they have only one selected.
                if (this.lvResults.SelectedItems.Count != 1)
                {
                    await cSettings.DisplayMessage("You can only view one property on the map at a time, please re-select and try again.", "Map Selection");
                    return;
                }

                //Check they have an internet connection.
                if (cSettings.AreWeOnline == false)
                {

                    await cSettings.DisplayMessage("You do not currently have an internet connection, please check your connection and try again.", "No Connection");
                    return;

                }

                //Loop through selected items and then redirect to the map page.
                foreach (cSurveyInputResult lviItem in this.lvResults.SelectedItems)
                {

                    //Redirect to map page.
                    this.Frame.Navigate(typeof(ViewMapPage), lviItem.SubProjectNo);

                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Show all statuses - checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkShowAllStatuses_Checked(object sender, RoutedEventArgs e)
        {
            try
            {

                this.DisplaySearchResults();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Show all statuses - un-checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkShowAllStatuses_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {

                this.DisplaySearchResults();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Show all progress statuses - un-checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkShowAllProgessStatus_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {

                this.DisplaySearchResults();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Show all progress statuses - checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkShowAllProgessStatus_Checked(object sender, RoutedEventArgs e)
        {
            try
            {

                this.DisplaySearchResults();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Show only sync changes - checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSyncChangesOnly_Checked(object sender, RoutedEventArgs e)
        {
            try
            {

                this.DisplaySearchResults();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Show only sync changes - un-checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSyncChangesOnly_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {

                this.DisplaySearchResults();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Project number filter selection change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbProjectNoFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {

                string sFilter = (string)this.cmbProjectNoFilter.SelectedItem;

                if (sFilter.Equals(cSettings.p_sProjectNoFilter_ProjectNo) == true)
                {
                    this.txtSubProjectNoFilter.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    this.cmbProjectNo.Visibility = Windows.UI.Xaml.Visibility.Visible;

                }
                else if (sFilter.Equals(cSettings.p_sProjectNoFilter_SubProjectNo) == true)
                {
                    this.txtSubProjectNoFilter.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    this.cmbProjectNo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                    this.txtSubProjectNoFilter.Focus(FocusState.Programmatic);

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Sub project no - text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSubProjectNoFilter_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {

                if (this.m_bIgnoreNextTextChanged_SubProjectNo == false)
                {
                    this.DisplaySearchResults();

                }

                this.m_bIgnoreNextTextChanged_SubProjectNo = false;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Delivery street - text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDeliveryStreet_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                if (this.m_bIgnoreNextTextChanged_DeliveryStreet == false)
                {
                    this.DisplaySearchResults();

                }
                this.m_bIgnoreNextTextChanged_DeliveryStreet = false;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Postcode - text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPostcode_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {

                if (this.m_bIgnoreNextTextChanged_Postcode == false)
                {
                    this.DisplaySearchResults();
                }

                this.m_bIgnoreNextTextChanged_Postcode = false;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Install status - selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbInstallStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {

                this.DisplaySearchResults();


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
        private void lvResults_Tapped(object sender, TappedRoutedEventArgs e)
        {

            try
            {

                //if (e.OriginalSource.GetType() == typeof(TextBlock))
                //{
                //    TextBlock tbProject = (TextBlock)e.OriginalSource;
                //    if (tbProject.DataContext.GetType() == typeof(cSurveyInputResult))
                //    {
                //        cSurveyInputResult cResult = (cSurveyInputResult)tbProject.DataContext;

                //        //Redirect to edit page.
                //        this.Frame.Navigate(typeof(SurveyInputOptionPage), cResult);

                //    }

                //}

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Use install date in search criteria
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkUseInstallPlanDate_Checked(object sender, RoutedEventArgs e)
        {

            try
            {

                this.DisplaySearchResults();

                this.btnSetInstallDate.IsEnabled = true;


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Do not use install date in search critiera.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkUseInstallPlanDate_UnChecked(object sender, RoutedEventArgs e)
        {

            try
            {

                this.DisplaySearchResults();

                this.btnSetInstallDate.IsEnabled = false;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Set install date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetInstallDate_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (LightDismissAnimatedPopup.IsOpen != true)
                {

                    double dXOffSet = this.cmbProjectNo.ActualWidth + this.txtDeliveryStreet.ActualWidth + this.txtPostcode.ActualWidth + this.cmbInstallStatus.ActualWidth + this.cmbProgressStatus.ActualWidth;
                    this.LightDismissAnimatedPopup.HorizontalOffset = dXOffSet;

                    LightDismissAnimatedPopup.IsOpen = true;
                    if (this.btnSetInstallDate.Tag != null)
                    {
                        dtPicker.Value = Convert.ToDateTime(this.btnSetInstallDate.Tag);

                    }
                }
                else
                {
                    LightDismissAnimatedPopup.IsOpen = false;

                }


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
        private async void btnCompleteInstallation_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                this.ProcessCompleteInstallation();
                        
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }


        /// <summary>
        /// v1.0.11 - Process set complete order
        /// </summary>
        /// <param name="sender"></param>
        private async void ProcessSetCompletedOrderDate(object sender)
        {
            
             try
            {

                if (this.lvResults.SelectedItems.Count() != 1)
                {
                    await cSettings.DisplayMessage("You need to select a single sub project first.", "Single project selection required.");
                    return;
                }

                cSurveyInputResult cResult = (cSurveyInputResult)this.lvResults.SelectedItem;

                bool bAnyRemakes = await cMain.DoesSubProjectHaveInCompleteRemakes(cResult.SubProjectNo);
                if (bAnyRemakes == false)
                {
                    //Default date to install date, if that is not set then today.
                    if (cResult.EndDateTime.HasValue == true)
                    {
                        dtPickerOrderComplete.Value = cResult.EndDateTime.Value;

                    }
                    else
                    {
                        dtPickerOrderComplete.Value = System.DateTime.Now;

                    }
                    

                    //Display pop-up.
                    try
                    {

                        //Show search screen
                        FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);

                    }
                    catch (Exception ex)
                    {
                        //No need to report these errors, bogus error.

                    }

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


        }


        /// <summary>
        /// v1.0.11 - Move code into separate routine to make it clearer.
        /// </summary>
        private async void ProcessCompleteInstallation()
        {
            
             try
            {

                if (this.lvResults.SelectedItems.Count() != 1)
                {
                    await cSettings.DisplayMessage("You need to select a single sub project first.", "Single project selection required.");
                    return;
                }

                cSurveyInputResult cResult = (cSurveyInputResult)this.lvResults.SelectedItem;

                bool bCanUpdate = await this.CanUserUpdateSubProject(cResult.SubProjectNo, cResult.Mxm1002InstallStatus);
                if (bCanUpdate == true)
                {

                    this.Frame.Navigate(typeof(InstallationInputResultPage), cResult);
                }
                           

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


        }

        /// <summary>
        /// Can user update sub project.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        private async Task<bool> CanUserUpdateSubProject(string v_sSubProjectNo,int v_iInstallStatus)
        {

            try
            {

                string sAllowedStatuses = cMain.p_cDataAccess.GetSettingValue("Installation_Update_InstallStatuses");
                string[] sAllowed = sAllowedStatuses.Split(',');

                bool bStatusMatch = false;
                foreach (string sStatus in sAllowed)
                {

                    if (v_iInstallStatus.ToString().Equals(sStatus) == true)
                    {
                        bStatusMatch = true;
                        break;
                    }

                }

                if (bStatusMatch == false)
                {

                    await cSettings.DisplayMessage("You cannot update this sub project as it is not on the correct status.", "Incorrect Install Status");
                    return false;

                }

                List<cUnitsTable> lUnits = cMain.p_cDataAccess.FetchUnitsForSubProject(v_sSubProjectNo);
                if (lUnits.Count == 0)
                {

                    await cSettings.DisplayMessage("You cannot update this sub project as it has no units associated with it, try syncing the project.", "No units..");
                    return false;

                }

                return true;
                
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }
        }

        /// <summary>
        /// Project number selection change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbProjectNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {

                this.DisplaySearchResults();


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Booked - Checked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkBooked_Checked(object sender, RoutedEventArgs e)
        {

            try
            {

                this.DisplaySearchResults();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Booked - un-checked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkBooked_Unchecked(object sender, RoutedEventArgs e)
        {

            try
            {

                this.DisplaySearchResults();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Install status filter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbInstallStatus_Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {

                this.DisplaySearchResults();


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);


            }
        }

        /// <summary>
        /// Progress status selection change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbProgressStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {

                this.DisplaySearchResults();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.11 - Allow for viewing notes for a sub project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnViewNotes_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                 if (this.lvResults.SelectedItems.Count() != 1)
                {
                    await cSettings.DisplayMessage("You need to select a single sub project first.", "Single project selection required.");
                    return;
                }

                cSurveyInputResult cResult = (cSurveyInputResult)this.lvResults.SelectedItem;
              
                this.Frame.Navigate(typeof(ViewNotes), cResult.SubProjectNo);
              

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.11 - Allow for viewing photos for a sub project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnViewPictures_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (this.lvResults.SelectedItems.Count() != 1)
                {
                    await cSettings.DisplayMessage("You need to select a single sub project first.", "Single project selection required.");
                    return;
                }

                cSurveyInputResult cResult = (cSurveyInputResult)this.lvResults.SelectedItem;

                this.Frame.Navigate(typeof(SetInstallationPhotosPage), cResult.SubProjectNo);


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.11 - Order complete date filter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbOrderCompleteDateFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {

                this.DisplaySearchResults();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.11 - Apply order complete date.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApplyOrderCompleteDate_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //Get sub project details.
                cSurveyInputResult cResult = (cSurveyInputResult)this.lvResults.SelectedItem;

                //Update local database
                cProjectTable cSubProject = cMain.p_cDataAccess.GetSubProjectProjectData(cResult.SubProjectNo);
                cSubProject.ABPAWOrderCompletedDate = dtPickerOrderComplete.Value;
                
                cMain.p_cDataAccess.UpdateSubProjectData(cSubProject);

                //Add update to update list
                cMain.p_cDataAccess.AddToUpdateTable(cResult.SubProjectNo, "ABPAWOrderCompletedDate", dtPickerOrderComplete.Value.ToString("yyyy-MM-dd HH:mm:ss"));

                //Hide fly out
                this.foOrderCompleteDate.Hide();

                //Re-process search.
                this.DisplaySearchResults();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.11 - Set order complete.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetOrderCompleteDate_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                this.ProcessSetCompletedOrderDate(sender);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }
    }
}
