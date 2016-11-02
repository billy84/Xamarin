using ANG_ABP_SURVEYOR_APP.Common;
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
using ANG_ABP_SURVEYOR_APP_CLASS.Classes;
using ANG_ABP_SURVEYOR_APP_CLASS;
using System.Text;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ANG_ABP_SURVEYOR_APP
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SetSurveyDatesSearchPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// Flag to indicate if page has been loaded.
        /// </summary>
        private bool m_bPageLoaded = false;

        /// <summary>
        /// Navigation mode, how did we get here.
        /// </summary>
        private NavigationMode m_nmNavMode = NavigationMode.New;

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


        /// <summary>
        /// Constructor
        /// </summary>
        public SetSurveyDatesSearchPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            this.Loaded += this.SetSurveyDatesPage_Loaded;

            try
            {




            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Page loaded event.
        /// </summary>
        void SetSurveyDatesPage_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {

                this.PopulateDropDowns();

                //Default the survey date button to today.
                this.SetSurveyDateButton(DateTime.Now);

                //Default is disabled.
                this.btnSetSurveyDate.IsEnabled = false;

                //If navigated back then restore the last search.
                if (this.m_nmNavMode == NavigationMode.Back)
                {
                    this.RestoreSearchCriteria();
                }

                this.m_bPageLoaded = true;

                this.DisplaySearchResults();

                //Reselect sub project.
                if (this.m_nmNavMode == NavigationMode.Back)
                {

                    List<string> sSubProjects = new List<string>();
                    sSubProjects.Add(cMain.p_cSearchCriteria_LastSearch.Selected_SubProjectNo);

                    this.ReselectSubProject(sSubProjects);
                }

                this.LightDismissAnimatedPopup.HorizontalOffset = this.cmbProjectNo.ActualWidth + this.txtDeliveryStreet.ActualWidth + this.cmbInstallStatus.ActualWidth + 50;
                this.LightDismissAnimatedPopup.VerticalOffset = this.chkUseSurveyPlanDate.ActualHeight + this.btnSetSurveyDate.ActualHeight + 10;

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

            try
            {

                // ** Project numbers drop down.
                this.cmbProjectNo.Items.Add(cSettings.p_sAnyStatus);

                List<cProjectNo> lsProjectNos = cMain.p_cDataAccess.GetProjectNos();
                foreach (cProjectNo lsProjectNo in lsProjectNos)
                {
                    this.cmbProjectNo.Items.Add(lsProjectNo.ProjectNo);


                }
                lsProjectNos = null;
                this.cmbProjectNo.SelectedIndex = 0; //Make cMain.p_sAnyStatus selection visible.


                // ** Install status drop down.
                ComboBoxItem cmbItem = new ComboBoxItem();
                cmbItem.Content = cSettings.p_sAnyStatus;
                cmbItem.Tag = "-1";
                this.cmbInstallStatus.Items.Add(cmbItem);

                List<cBaseEnumsTable> oInstalls = cMain.p_cDataAccess.GetEnumsForField("MXM1002INSTALLSTATUS");
                foreach (cBaseEnumsTable oInstall in oInstalls)
                {
                    cmbItem = new ComboBoxItem();
                    cmbItem.Content = oInstall.EnumName;
                    cmbItem.Tag = oInstall.EnumValue;
                    this.cmbInstallStatus.Items.Add(cmbItem);

                }
                this.cmbInstallStatus.SelectedIndex = 0; //Make Any the default selection


                //v1.0.14 - Progress status drop down.
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


                // ** Surveyed Status Drop Down
                this.cmbSurveyedStatus.Items.Add(cSettings.p_sAnyStatus);
                this.cmbSurveyedStatus.Items.Add(cSettings.p_sSurveyedStatus_NotSurveyed);
                this.cmbSurveyedStatus.Items.Add(cSettings.p_sSurveyedStatus_SurveyedOnSite);
                this.cmbSurveyedStatus.Items.Add(cSettings.p_sSurveyedStatus_SurveyedTrans);
                this.cmbSurveyedStatus.SelectedIndex = 1;


                // ** Confirmed appointment indicator
                cmbItem = new ComboBoxItem();
                cmbItem.Content = cSettings.p_sAnyStatus;
                cmbItem.Tag = "-1";
                this.cmbConfirmed.Items.Add(cmbItem);

                List<cBaseEnumsTable> oConfirms = cMain.p_cDataAccess.GetEnumsForField("MxmConfirmedAppointmentIndicator");
                foreach (cBaseEnumsTable oConfirm in oConfirms)
                {
                    cmbItem = new ComboBoxItem();
                    cmbItem.Content = oConfirm.EnumName;
                    cmbItem.Tag = oConfirm.EnumValue;
                    this.cmbConfirmed.Items.Add(cmbItem);

                }
                this.cmbConfirmed.SelectedIndex = 0; //Make Any the default selection


                // ** Set survey dates time picker
                this.cmbTimePicker.Items.Add(cSettings.p_sPleaseChoose);
                this.cmbTimePicker.Items.Add(cSettings.p_sTime_AM);
                this.cmbTimePicker.Items.Add(cSettings.p_sTime_PM);
                this.cmbTimePicker.Items.Add(cSettings.p_sTime_Specific);
                this.cmbTimePicker.SelectedIndex = 0;

                ////Default time picker to hidden.
                this.tpSurveyTime.IsEnabled = false;
                this.tpSurveyTime.Visibility = Windows.UI.Xaml.Visibility.Collapsed; //v1.0.1

                //Data comparison drop down.
                this.cmbDateCompare.Items.Add(cSettings.p_sDateCompare_EqualTo);
                this.cmbDateCompare.Items.Add(cSettings.p_sDateCompare_GreaterThan);
                this.cmbDateCompare.Items.Add(cSettings.p_sDateCompare_LessThan);
                this.cmbDateCompare.SelectedIndex = 0;


                //v1.0.1 - Project no filter
                this.cmbProjectNoFilter.Items.Add(cSettings.p_sProjectNoFilter_ProjectNo);
                this.cmbProjectNoFilter.Items.Add(cSettings.p_sProjectNoFilter_SubProjectNo);
                this.cmbProjectNoFilter.SelectedIndex = 0;


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

                if (this.cmbProjectNoFilter.SelectedItem.ToString().Equals(cSettings.p_sProjectNoFilter_ProjectNo) == true)
                {
                    String sValue = (String)this.cmbProjectNo.SelectedItem;
                    if (sValue != cSettings.p_sAnyStatus)
                    {
                        sProjectNo = sValue;

                    }

                }
                else if (this.cmbProjectNoFilter.SelectedItem.ToString().Equals(cSettings.p_sProjectNoFilter_SubProjectNo) == true)
                {
                    //v1.0.1 - Sub Project number filter.
                    sSubProjectNo = this.txtSubProjectNoFilter.Text;
                }


                //Install status
                Int32 iInstallStatus = -1;
                ComboBoxItem cbiItem = (ComboBoxItem)this.cmbInstallStatus.SelectedItem;
                string sTagValue = cbiItem.Tag.ToString();
                if (int.TryParse(sTagValue, out iInstallStatus) == true)
                {

                }

                //v1.0.14 - Progress status
                Int32 iProgressStatus = -1;
                cbiItem = (ComboBoxItem)this.cmbProgressStatus.SelectedItem;
                sTagValue = cbiItem.Tag.ToString();
                if (int.TryParse(sTagValue, out iProgressStatus) == true)
                {

                }

                //Surveyed Date.
                DateTime? dSurveyDate = null;
                if (this.chkUseSurveyPlanDate.IsChecked == true)
                {
                    dSurveyDate = this.dtPicker.Value;

                }

                //Surveyed
                string sSurveyed = this.cmbSurveyedStatus.SelectedItem.ToString();

                //Confirmed
                cbiItem = (ComboBoxItem)this.cmbConfirmed.SelectedItem;
                int iConfirmed = -1;
                int.TryParse(cbiItem.Tag.ToString(), out iConfirmed);

                //Date comparison
                string sDateComparison = this.cmbDateCompare.SelectedItem.ToString();


                int iInstall_Awaiting = Convert.ToInt32(cMain.GetAppResourceValue("InstallStatus_AwaitingSurvey"));
                int iInstall_Cancel = Convert.ToInt32(cMain.GetAppResourceValue("InstallStatus_SurveyCancelled"));


                //Process Search
                List<cSurveyDatesResult> cResults = cMain.p_cDataAccess.SearchSurveyDates(sProjectNo, this.txtDeliveryStreet.Text, this.txtPostcode.Text, iInstallStatus,iProgressStatus, dSurveyDate, sDateComparison, sSurveyed, iConfirmed, this.chkSyncChangesOnly.IsChecked.Value, this.chkShowAllStatuses.IsChecked.Value, this.chkShowAllProgessStatus.IsChecked.Value, sSubProjectNo,iInstall_Awaiting,iInstall_Cancel);

                

                int iUpdates = 0;
                foreach (cSurveyDatesResult cResult in cResults)
                {

                    cResult.ScreenWidth = this.lvResults.ActualWidth;
                    cResult.StreetWidth = this.txtDeliveryStreet.ActualWidth;
                    cResult.InstallWidth = this.cmbInstallStatus.ActualWidth - 10;
                    cResult.ProgressWidth = this.cmbProgressStatus.ActualWidth - 10;
                    cResult.PostcodeWidth = this.txtPostcode.ActualWidth;
                    
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

                    iUpdates = 0;
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

                    //Display relevant confirmed status.
                    cResult.Confirmed = cSettings.p_sConfirmedStatus_No;
                    if (cResult.MxmConfirmedAppointmentIndicator.HasValue == true)
                    {
                        if (cResult.MxmConfirmedAppointmentIndicator.Value == (int)cSettings.YesNoBaseEnum.Yes)
                        {
                            cResult.Confirmed = cSettings.p_sConfirmedStatus_Yes;
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


                    //Remove unwanted new lines.
                    cResult.DeliveryStreet = cMain.RemoveNewLinesFromString(cResult.DeliveryStreet);

                    //v1.0.1 - Update tool tip text
                    cResult.ToolTipText = "Status: " + cResult.StatusName + Environment.NewLine + "Progress Status: " + cResult.ProgressStatusName;


                    //v1.0.1 - Display formatted survey date - time
                    if (cResult.EndDateTime.HasValue == true)
                    {
                        cResult.SurveyDisplayDateTime = cMain.ReturnDisplayDate(cResult.EndDateTime.Value) + " " + cMain.ReturnDisplayTime(cResult.EndDateTime.Value);
                    }

                    
                    //v1.0.15 - Set background color if project status is "On Hold"
                    if (cResult.Status == cSettings.p_iProjectStatus_OnHold)
                    {
                        cResult.BackgroundColour = cSettings.p_sSurvey_ListView_OnHold_Background;
                    }
                    else
                    {
                        cResult.BackgroundColour = cSettings.p_sSurvey_ListView_Normal_Background;
                    }

                    //v1.0.19 - Display work type if toggled to.
                    if (this.cmbInstallFilterType.SelectedIndex == 0)
                    {
                        cResult.InstallStatusName = cResult.MxmProjDescription;
                    }

                }


                this.lvResults.ItemsSource = cResults;

                //v1.0.2
                this.tbSubProjectCount.Text = string.Empty;
                this.tbSelectedCount.Text = this.lvResults.Items.Count() + " Listed";


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);


            }

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

            try
            {

                string sName = String.Empty;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), String.Empty);
            }

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

            try
            {

                string sName = String.Empty;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), String.Empty);
            }

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

            //Record navigation mode.
            this.m_nmNavMode = e.NavigationMode;

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);

            if (e.NavigationMode == NavigationMode.New)
            {
                this.RecordCurrentSearchCriteria();
            }
            else
            {
                try
                {
                    this.lvResults.ItemsSource = null;

                    GC.Collect();

                }
                catch (Exception ex)
                {
                    cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

                }

            }
        }

        #endregion


        /// <summary>
        /// Set survey date button.
        /// </summary>
        /// <param name="v_dDate"></param>
        private void SetSurveyDateButton(DateTime v_dDate)
        {

            try
            {

                //Set button properties.
                this.btnSetSurveyDate.Tag = v_dDate;
                this.btnSetSurveyDate.Content = cMain.ReturnDisplayDate(v_dDate, this.cmbDateCompare.SelectedItem.ToString());

                if (this.m_bPageLoaded == true)
                {
                    this.DisplaySearchResults();
                }         

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), v_dDate.ToString());

            }

        }

        /// <summary>
        /// Set survey date.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetSurveyDate_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (LightDismissAnimatedPopup.IsOpen != true)
                {

                    double dXOffSet = this.cmbProjectNo.ActualWidth + this.txtDeliveryStreet.ActualWidth + this.txtPostcode.ActualWidth + this.cmbInstallStatus.ActualWidth + this.cmbProgressStatus.ActualWidth;
                    this.LightDismissAnimatedPopup.HorizontalOffset = dXOffSet;

                    LightDismissAnimatedPopup.IsOpen = true;
                    if (this.btnSetSurveyDate.Tag != null)
                    {
                        dtPicker.Value = Convert.ToDateTime(this.btnSetSurveyDate.Tag);

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
        /// Select all items in list view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                this.lvResults.SelectAll();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Clear all selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeSelectAll_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                this.lvResults.SelectedItems.Clear();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Set survey dates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnSetSurveyDates_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (this.SetTheSurveyDatesPopup.IsOpen == true)
                {
                    this.SetTheSurveyDatesPopup.IsOpen = false;
                }
                else
                {

                    if (this.lvResults.SelectedItems.Count == 0)
                    {
                        await cSettings.DisplayMessage("You must select some project first before you can set there survey dates.", "Project selection required.");
                        return;

                    }

                    this.ConfigureSurveyDate();

                    this.SetTheSurveyDatesPopup.IsOpen = true;
                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);


            }
        }

        /// <summary>
        /// Configure survey date popup.
        /// </summary>
        private void ConfigureSurveyDate()
        {

            try
            {

                //Set to first sub project with valid date time.
                foreach (cSurveyDatesResult cSurvey in this.lvResults.SelectedItems)
                {

                    if (cSurvey.EndDateTime.HasValue == true)
                    {

                        this.dtPickerSurveyDate.Value = cSurvey.EndDateTime.Value;                        
                        this.cmbTimePicker.SelectedIndex = 0;

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
        /// Time picker selection change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbTimePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {

                string sTime = String.Empty;

                switch (this.cmbTimePicker.SelectedItem.ToString())
                {
                    case cSettings.p_sPleaseChoose:
                        this.tpSurveyTime.IsEnabled = false;
                        this.tpSurveyTime.Visibility = Windows.UI.Xaml.Visibility.Collapsed; //v1.0.1
                        break;

                    case cSettings.p_sTime_AM:
                        this.tpSurveyTime.IsEnabled = false;
                        this.tpSurveyTime.Visibility = Windows.UI.Xaml.Visibility.Collapsed; //v1.0.1
                        sTime = cMain.GetAppResourceValue("AM_TIME");
                        break;

                    case cSettings.p_sTime_PM:
                        this.tpSurveyTime.IsEnabled = false;
                        this.tpSurveyTime.Visibility = Windows.UI.Xaml.Visibility.Collapsed; //v1.0.1
                        sTime = cMain.GetAppResourceValue("PM_TIME");
                        break;

                    case cSettings.p_sTime_Specific:
                        this.tpSurveyTime.IsEnabled = true;
                        this.tpSurveyTime.Visibility = Windows.UI.Xaml.Visibility.Visible; //v1.0.1
                        break;

                    default:
                        break;
                }


                if (sTime.Length > 0)
                {

                    string[] sTimeParts = sTime.Split(':');
                    tpSurveyTime.Time = new TimeSpan(Convert.ToInt32(sTimeParts[0]), Convert.ToInt32(sTimeParts[1]), 0);

                }



            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Project number selection change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbProjectNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                if (this.m_bPageLoaded == true)
                {
                    this.DisplaySearchResults();
                }         


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


        }

        /// <summary>
        /// Install status selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbInstallStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                if (this.m_bPageLoaded == true)
                {
                    this.DisplaySearchResults();
                }         


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Use survey plan date checked status changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkUseSurveyPlanDate_Checked(object sender, RoutedEventArgs e)
        {

            try
            {

                if (this.m_bPageLoaded == false)
                {
                    return;
                }

                this.DisplaySearchResults();

                this.btnSetSurveyDate.IsEnabled = true;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }


        /// <summary>
        /// Use survey plan date checked status changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkUseSurveyPlanDate_UnChecked(object sender, RoutedEventArgs e)
        {

            try
            {

                if (this.m_bPageLoaded == false)
                {
                    return;
                }

                this.DisplaySearchResults();

                this.btnSetSurveyDate.IsEnabled = false;


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Confirmed selection criteria changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbConfirmed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {

                if (this.m_bPageLoaded == true)
                {
                    this.DisplaySearchResults();
                }               

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Surveyed status selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbSurveyedStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                if (this.m_bPageLoaded == true)
                {
                    this.DisplaySearchResults();
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
        /// Edit survey button clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnEditSurvey_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (this.lvResults.SelectedItems.Count() == 0)
                {

                    await cSettings.DisplayMessage("You must select a project first before you can edit there details.", "Project selection required.");
                    return;

                }
                else if (this.lvResults.SelectedItems.Count() > 1)
                {

                    await cSettings.DisplayMessage("Please only select one project to edit.", "Only one project can be edited at a time.");
                    return;

                }

                //Fetch selected sub project number.
                string sSubProjID = String.Empty;
                foreach (cSurveyDatesResult cResult in this.lvResults.SelectedItems)
                {
                    sSubProjID = cResult.SubProjectNo;

                }

                //Redirect to edit page.
                this.Frame.Navigate(typeof(SetSurveyDatesEditPage), sSubProjID);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Apply survey date to selected projects.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnApplySurveyDate_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //Make sure a time has been picked.
                string sTimePicked = this.cmbTimePicker.SelectedItem.ToString();
                if (sTimePicked == cSettings.p_sPleaseChoose)
                {
                    await cSettings.DisplayMessage("Please select a time option from the drop down.", "Time selection required.");
                    this.SetTheSurveyDatesPopup.IsOpen = true;
                    this.cmbTimePicker.Focus(FocusState.Programmatic);
                    return;

                }

                //Record all the sub project numbers.
                List<String> lsSubProjectNos = new List<String>();

                //v1.0.2 - List of string containing sub project that will not display on front screen
                List<string> lsSubProjectNoDisplay = new List<string>();
                bool bWillItDisplay = false;

                foreach (cSurveyDatesResult lviItem in this.lvResults.SelectedItems)
                {
                    lsSubProjectNos.Add(lviItem.SubProjectNo);

                    //v1.0.2 - Check if sub projects statuses will allow it to display on the front screen.
                    bWillItDisplay = cMain.WillSubProjectDisplayOnFrontScreen(lviItem.Mxm1002InstallStatus);
                    if (bWillItDisplay == false)
                    {
                        lsSubProjectNoDisplay.Add(lviItem.SubProjectNo);

                    }

                }

                //v1.0.2 - Warn user if sub project will not display on front screen as status incorrect.                              
                if (lsSubProjectNoDisplay.Count > 0)
                {

                    string sMessage = cMain.ReturnSubProjectWillNotDisplayOnFrontScreenMessage(lsSubProjectNoDisplay);
                    await cSettings.DisplayMessage(sMessage, "Warning");

                }

                //Place date and time in strings.
                String sDate = this.dtPickerSurveyDate.Value.ToString("dd/MM/yyyy");
                String sTime = this.tpSurveyTime.Time.Hours.ToString().PadLeft(2, '0') + ":" + this.tpSurveyTime.Time.Minutes.ToString().PadLeft(2, '0') + ":" + this.tpSurveyTime.Time.Seconds.ToString().PadLeft(2, '0');

                //Put date and time together.
                DateTime dSurveyDate = Convert.ToDateTime(sDate + " " + sTime);

                bool bValid = await cMain.p_cDataAccess.ApplySurveyDatesToSubProjects(lsSubProjectNos, dSurveyDate);
                if (bValid == true)
                {
                    this.DisplaySearchResults();
                    this.SetTheSurveyDatesPopup.IsOpen = false;
                }
                else
                {
                    await cSettings.DisplayMessage("A problem occurred when trying to apply the survey dates, please try again.", "Cannot Apply Survey Date.");

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
        private void btnUseSearchDate_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //Set the date time on the survey button.
                this.SetSurveyDateButton(dtPicker.Value);

                //Close pop up.
                LightDismissAnimatedPopup.IsOpen = false;

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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSyncChangesOnly_UnChecked(object sender, RoutedEventArgs e)
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
        /// View property on map.
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
                foreach (cSurveyDatesResult lviItem in this.lvResults.SelectedItems)
                {

                    //Redirect to map page.
                    this.Frame.Navigate(typeof(ViewMap), lviItem.SubProjectNo);

                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

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


                //v1.0.1 - 
                this.cmbProjectNoFilter.SelectedItem = cMain.p_cSearchCriteria_LastSearch.ProjectNoFilter;
                if (cMain.p_cSearchCriteria_LastSearch.ProjectNoFilter.Equals(cSettings.p_sProjectNoFilter_ProjectNo) == true)
                {
                    this.cmbProjectNo.SelectedItem = cMain.p_cSearchCriteria_LastSearch.ProjectNo;
                }
                else
                {
                    if (cMain.p_cSearchCriteria_LastSearch.ProjectNo.Length > 0)
                    {
                        this.m_bIgnoreNextTextChanged_SubProjectNo = true;
                        this.txtSubProjectNoFilter.Text = cMain.p_cSearchCriteria_LastSearch.ProjectNo;
                    }
                                        
                }


                //Update street
                if (cMain.p_cSearchCriteria_LastSearch.StreetAddress.Length > 0)
                {
                    this.m_bIgnoreNextTextChanged_DeliveryStreet = true;
                    this.txtDeliveryStreet.Text = cMain.p_cSearchCriteria_LastSearch.StreetAddress;

                }

                //Update postcode
                if (cMain.p_cSearchCriteria_LastSearch.Postcode.Length > 0)
                {
                    this.m_bIgnoreNextTextChanged_Postcode = true;
                    this.txtPostcode.Text = cMain.p_cSearchCriteria_LastSearch.Postcode;

                }
                
                

                int iIndex = 0;
                foreach (ComboBoxItem cmbItem in this.cmbInstallStatus.Items)
                {
                    if (Convert.ToInt32(cmbItem.Tag) == cMain.p_cSearchCriteria_LastSearch.InstallStatus)
                    {
                        this.cmbInstallStatus.SelectedIndex = iIndex;
                        break;
                    }

                    iIndex++;
                }

                //v1.0.14 - Restore progress status
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

                this.chkUseSurveyPlanDate.IsChecked = cMain.p_cSearchCriteria_LastSearch.IncludeSurveyPlanDate;
                this.btnSetSurveyDate.IsEnabled = cMain.p_cSearchCriteria_LastSearch.IncludeSurveyPlanDate;

                this.cmbDateCompare.SelectedItem = cMain.p_cSearchCriteria_LastSearch.SurveyPlanDateComparison;
                this.SetSurveyDateButton(cMain.p_cSearchCriteria_LastSearch.SurveyPlanDate.Value);

                this.cmbSurveyedStatus.SelectedItem = cMain.p_cSearchCriteria_LastSearch.SurveyedStatus;

                iIndex = 0;
                foreach (ComboBoxItem cmbItem in this.cmbConfirmed.Items)
                {
                    if (Convert.ToInt32(cmbItem.Tag) == cMain.p_cSearchCriteria_LastSearch.Confirmed)
                    {
                        this.cmbConfirmed.SelectedIndex = iIndex;
                        break;
                    }
                }

                this.chkSyncChangesOnly.IsChecked = cMain.p_cSearchCriteria_LastSearch.SyncChangesOnly;

                //v1.0.1
                this.chkShowAllStatuses.IsChecked = cMain.p_cSearchCriteria_LastSearch.ShowAllStatuses;
                this.chkShowAllProgessStatus.IsChecked = cMain.p_cSearchCriteria_LastSearch.ShowAllProgressStatuses;

                //v1.0.19
                this.cmbInstallFilterType.SelectedIndex = cMain.p_cSearchCriteria_LastSearch.InstallStatus_FilterIndex;


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

                cMain.p_cSearchCriteria_LastSearch = new cSearchCriteria();

                //v1.0.1 - 
                cMain.p_cSearchCriteria_LastSearch.ProjectNoFilter = this.cmbProjectNoFilter.SelectedItem.ToString();
                if (cMain.p_cSearchCriteria_LastSearch.ProjectNoFilter.Equals(cSettings.p_sProjectNoFilter_ProjectNo) == true)
                {
                    cMain.p_cSearchCriteria_LastSearch.ProjectNo = this.cmbProjectNo.SelectedItem.ToString();
                }
                else
                {
                    cMain.p_cSearchCriteria_LastSearch.ProjectNo = this.txtSubProjectNoFilter.Text;
                }


                cMain.p_cSearchCriteria_LastSearch.StreetAddress = this.txtDeliveryStreet.Text;
                cMain.p_cSearchCriteria_LastSearch.Postcode = this.txtPostcode.Text;

                ComboBoxItem cmbItem = (ComboBoxItem)this.cmbInstallStatus.SelectedItem;
                cMain.p_cSearchCriteria_LastSearch.InstallStatus = Convert.ToInt32(cmbItem.Tag);

                //v1.0.14 - Record progress status
                cmbItem = (ComboBoxItem)this.cmbProgressStatus.SelectedItem;
                cMain.p_cSearchCriteria_LastSearch.ProgressStatus = Convert.ToInt32(cmbItem.Tag);                

                cMain.p_cSearchCriteria_LastSearch.IncludeSurveyPlanDate = this.chkUseSurveyPlanDate.IsChecked.Value;
                cMain.p_cSearchCriteria_LastSearch.SurveyPlanDate = (DateTime)this.btnSetSurveyDate.Tag;
                cMain.p_cSearchCriteria_LastSearch.SurveyPlanDateComparison = this.cmbDateCompare.SelectedItem.ToString();

                cMain.p_cSearchCriteria_LastSearch.SurveyedStatus = this.cmbSurveyedStatus.SelectedItem.ToString();

                cmbItem = (ComboBoxItem)this.cmbConfirmed.SelectedItem;
                cMain.p_cSearchCriteria_LastSearch.Confirmed = Convert.ToInt32(cmbItem.Tag);

                cMain.p_cSearchCriteria_LastSearch.SyncChangesOnly = this.chkSyncChangesOnly.IsChecked.Value;

                cSurveyDatesResult cResult = (cSurveyDatesResult)this.lvResults.SelectedItem;
                if (cResult != null)
                {
                    cMain.p_cSearchCriteria_LastSearch.Selected_SubProjectNo = cResult.SubProjectNo;
                }

                //v1.0.1
                cMain.p_cSearchCriteria_LastSearch.ShowAllStatuses = this.chkShowAllStatuses.IsChecked.Value;
                cMain.p_cSearchCriteria_LastSearch.ShowAllProgressStatuses = this.chkShowAllProgessStatus.IsChecked.Value;

                //v1.0.19
                cMain.p_cSearchCriteria_LastSearch.InstallStatus_FilterIndex = this.cmbInstallFilterType.SelectedIndex;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Reselect previously selected sub project.
        /// </summary>
        private void ReselectSubProject(List<string> v_sSubProjects)
        {

            try
            {

                
                    
                    int iIndex = 0;

                    foreach (cSurveyDatesResult cResult in this.lvResults.Items)
                    {

                        foreach (string sSubProject in v_sSubProjects)
                        {
                                                    
                            if (cResult.SubProjectNo == sSubProject)
                            {

                                this.lvResults.UpdateLayout();

                                this.lvResults.ScrollIntoView(cResult);
                                this.lvResults.SelectedItems.Add(cResult);
                                
                                    
                            }
                                                                                                                                                              
                        }
                            
                        iIndex++;

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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkShowAllStatuses_UnChecked(object sender, RoutedEventArgs e)
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
        private void chkShowAllProgessStatus_UnChecked(object sender, RoutedEventArgs e)
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
        /// v1.0.1 - Project number filter changed.
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
        /// v1.0.1 - Sub project no filter, text changed.
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
        /// v1.0.1 - Postcode, text changed.
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
        /// v1.0.2 - Un Confirm surveys.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnUnConfirmSurveys_Click(object sender, RoutedEventArgs e)
        {


            try
            {

                //Nothing selected, let user know.
                if (this.lvResults.SelectedItems.Count == 0)
                {

                    await cSettings.DisplayMessage("You need to select at least one sub-project before you can continue.", "Selection Required");
                    this.lvResults.Focus(FocusState.Programmatic);
                    return;


                }

                //Remove sub-projects that do not have confirmation flags.
                List<string> lsToUnConfirm = new List<string>();                
                foreach (cSurveyDatesResult cSurvey in this.lvResults.SelectedItems)
                {

                    if (cSurvey.MxmConfirmedAppointmentIndicator.HasValue == true)
                    {

                         if (cSurvey.MxmConfirmedAppointmentIndicator.Value == (int)cSettings.YesNoBaseEnum.Yes)
                         {                           

                            //Record sub project we are going to un-confirm
                            lsToUnConfirm.Add(cSurvey.SubProjectNo);

                         }

                    }
                                     
                }

                //If no sub project are confirmed let user know and quit.
                if (lsToUnConfirm.Count == 0)
                {

                    await cSettings.DisplayMessage("None of the sub-projects selected have confirmed appointments.", "No sub-projects confirmed.");
                    this.lvResults.Focus(FocusState.Programmatic);
                    return;


                }

                //Build confirmation message.
                StringBuilder sbMessage = new StringBuilder();
                sbMessage.Append("Are you sure you want to un-confirm the following sub-projects:");
                sbMessage.Append(Environment.NewLine);
                sbMessage.Append(Environment.NewLine);

                //Add sub-projects to message.
                foreach (string sSubProject in lsToUnConfirm)
                {
                    sbMessage.Append(sSubProject);
                    sbMessage.Append(Environment.NewLine);

                }
              

                //Ask user if they want to continue
                cSettings.YesNo cAnswer = await cSettings.DisplayYesNo(sbMessage.ToString(), "Confirm Action");
                if (cAnswer == cSettings.YesNo.Yes)
                {


                    cProjectTable cSubProjectData = null;
                    foreach (string sProject in lsToUnConfirm)
                    {

                        //Fetch sub project data
                        cSubProjectData = cMain.p_cDataAccess.GetSubProjectProjectData(sProject);

                        //Update confirmed flag.
                        cSubProjectData.MxmConfirmedAppointmentIndicator = (int)cSettings.YesNoBaseEnum.No;

                        //Save back to database.
                        cMain.p_cDataAccess.UpdateSubProjectData(cSubProjectData);

                        //Add to log table.
                        cMain.p_cDataAccess.AddToUpdateTable(sProject, "MxmConfirmedAppointmentIndicator", ((int)cSettings.YesNoBaseEnum.No).ToString());

                                             
                    }
                               
                }

                //Refresh screen
                this.DisplaySearchResults();

                //Reselect
                this.ReselectSubProject(lsToUnConfirm);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.2 - List view selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {

                this.tbSubProjectCount.Text = string.Empty;
                this.tbSelectedCount.Text = this.lvResults.Items.Count() + " Listed";

                

                if (this.lvResults.SelectedItems.Count() > 0)
                {

                    this.tbSelectedCount.Text = this.lvResults.SelectedItems.Count() + " Selected";

                    bool bDisplaySubProjectCount = true;

                    string sProject = string.Empty;

                    foreach (cSurveyDatesResult cSubProject in this.lvResults.SelectedItems)
                    {

                        if (sProject.Length == 0)
                        {
                            sProject = cSubProject.ProjectNo;
                        }
                        else if (sProject != cSubProject.ProjectNo)
                        {

                            bDisplaySubProjectCount = false;
                            break;

                        }
                       
                    }

                    if (bDisplaySubProjectCount == true)
                    {

                        int iTotal = cMain.p_cDataAccess.ReturnSubProjectQty(sProject);
                        this.tbSubProjectCount.Text = sProject + " Subprojects " + iTotal.ToString();

                    }

                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


        }

        /// <summary>
        /// v1.0.14 - Progress status selection change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbProgressStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                if (this.m_bPageLoaded == true)
                {
                    this.DisplaySearchResults();
                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.19 - Set work types.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnWorkType_Click(object sender, RoutedEventArgs e)
        {

            try
            {


                //Nothing selected, let user know.
                if (this.lvResults.SelectedItems.Count == 0)
                {

                    await cSettings.DisplayMessage("You need to select at least one sub-project before you can continue.", "Selection Required");
                    this.lvResults.Focus(FocusState.Programmatic);
                    return;


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
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.19 - Update selected work types.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateWorkType_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //Loop through selected items and then redirect to the picture page.
                foreach (cSurveyDatesResult lviItem in this.lvResults.SelectedItems)
                {
                  
                    //Update local database
                    cProjectTable cSubProject = cMain.p_cDataAccess.GetSubProjectProjectData(lviItem.SubProjectNo);
                    cSubProject.MxmProjDescription = this.txtWorkType.Text;
                       

                    cMain.p_cDataAccess.UpdateSubProjectData(cSubProject);

                    //Add update to update list
                    cMain.p_cDataAccess.AddToUpdateTable(lviItem.SubProjectNo, "MxmProjDescription", this.txtWorkType.Text);
                   
                }

                //Hide fly out
                this.foWorkType.Hide();

                //Re-process search.
                this.DisplaySearchResults();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.19 - Install filter selection change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbInstallFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {

                if (this.m_bPageLoaded == false) { return; }            

                this.DisplaySearchResults();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }
    }
}
