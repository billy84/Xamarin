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
using ANG_ABP_SURVEYOR_APP_CLASS;
using ANG_ABP_SURVEYOR_APP_CLASS.Classes;
using ANG_ABP_SURVEYOR_APP_CLASS.Model;
using System.Text;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ANG_ABP_INSTALLER_APP.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SetInstallationDatesPage : Page
    {

        /// <summary>
        /// Flag to indicate if page has been loaded.
        /// </summary>
        private bool m_bPageLoaded = false;

        /// <summary>
        /// Navigation mode, how did we get here.
        /// </summary>
        private NavigationMode m_nmNavMode = NavigationMode.New;

        /// <summary>
        /// Flag to indicate that the next text changed event is to be ignored, fired because of restoring search criteria.
        /// </summary>
        private bool m_bIgnoreNextTextChanged_DeliveryStreet = false;

        /// <summary>
        /// Flag to indicate that the next text changed event is to be ignored, fired because of restoring search criteria.
        /// </summary>
        private bool m_bIgnoreNextTextChanged_Postcode = false;

        /// <summary>
        /// Flag to indicate that the next text changed event is to be ignored, fired because of restoring search criteria.
        /// </summary>
        private bool m_bIgnoreNextTextChanged_SubProjectNo = false;

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

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


        public SetInstallationDatesPage()
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
        /// Installation dates load page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetInstallationDatesPage_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {

                this.PopulateDropDowns();

                //v1.0.1 - Set default install status.
                this.SetDefaultConfigurationForInstallStatus();

                //Default the survey date button to today.
                this.SetInstallationDateButton(DateTime.Now);

                //Default is disabled.
                this.btnDisplayInstallationDate.IsEnabled = false;

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
                this.LightDismissAnimatedPopup.VerticalOffset = this.chkUseInstallationDate.ActualHeight + this.btnDisplayInstallationDate.ActualHeight + 10;


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.1 - Set default configuration of install status.
        /// </summary>
        private void SetDefaultConfigurationForInstallStatus()
        {

            try
            {

                //Set install filter.
                string sFilter = cMain.p_cDataAccess.GetSettingValue("Installation_InstallStatus_DefaultInstallFilter");
                if (sFilter.Trim().Length > 0)
                {
                    this.cmbInstallStatus_Filter.SelectedItem = sFilter;

                }

                //Set install status.
                string sInstallStatus = cMain.p_cDataAccess.GetSettingValue("Installation_InstallStatus_DefaultInstallStatus");
                if (sInstallStatus.Trim().Length > 0)
                {

                    int iStatus = 0;
                    int iTagStatus = -1;
                    if (int.TryParse(sInstallStatus, out iStatus) == true)
                    {

                        int iIndex = 0;
                        foreach (ComboBoxItem cbiStatus in this.cmbInstallStatus.Items)
                        {

                            if (cbiStatus.Tag != null)
                            {

                                if (int.TryParse(cbiStatus.Tag.ToString(), out iTagStatus) == true)
                                {

                                    if (iTagStatus == iStatus)
                                    {

                                        this.cmbInstallStatus.SelectedIndex = iIndex;
                                        break;

                                    }

                                }

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
        /// Populate drop downs.
        /// </summary>
        private void PopulateDropDowns()
        {

            try
            {

                // ** Project numbers drop down.
                ComboBoxItem cmbItem = new ComboBoxItem();
                cmbItem.Content = cSettings.p_sAnyStatus;
                cmbItem.Tag = cSettings.p_sAnyStatus;
                this.cmbProjectNo.Items.Add(cmbItem);

                //v1.0.5 - Display project name as well as no.
                List<cProjectNo> lsProjectNos = cMain.p_cDataAccess.GetProjectNos();
                foreach (cProjectNo lsProjectNo in lsProjectNos)
                {

                    cmbItem = new ComboBoxItem();
                    cmbItem.Content = lsProjectNo.ProjectNo + " - " + cMain.RemoveNewLinesFromString(lsProjectNo.ProjectName);
                    cmbItem.Tag = lsProjectNo.ProjectNo;
                    this.cmbProjectNo.Items.Add(cmbItem);                    

                }
                lsProjectNos = null;
                this.cmbProjectNo.SelectedIndex = 0; //Make cMain.p_sAnyStatus selection visible.


                // ** Install status drop down.
                cmbItem = new ComboBoxItem();
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

                    //v1.0.11 - Populate progress status box for updating.
                    cmbItem = new ComboBoxItem();
                    cmbItem.Content = oProgress.EnumName;
                    cmbItem.Tag = oProgress.EnumValue;
                    this.cmbNewProgressStatus.Items.Add(cmbItem);

                }
                this.cmbProgressStatus.SelectedIndex = 0; //Make Any the default selection



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


                //Data comparison drop down.
                this.cmbDateCompare.Items.Add(cSettings.p_sDateCompare_EqualTo);
                this.cmbDateCompare.Items.Add(cSettings.p_sDateCompare_GreaterThan);
                this.cmbDateCompare.Items.Add(cSettings.p_sDateCompare_LessThan);
                this.cmbDateCompare.SelectedIndex = 0;


                //v1.0.1 - Project no filter
                this.cmbProjectNoFilter.Items.Add(cSettings.p_sProjectNoFilter_ProjectNo);
                this.cmbProjectNoFilter.Items.Add(cSettings.p_sProjectNoFilter_SubProjectNo);
                this.cmbProjectNoFilter.SelectedIndex = 0;

                //v1.0.1 - Install status filters
                this.cmbInstallStatus_Filter.Items.Add(cSettings.p_sInstallStatusFilter_EqualTo);
                this.cmbInstallStatus_Filter.Items.Add(cSettings.p_sInstallStatusFilter_NotEqualTo);
                this.cmbInstallStatus_Filter.SelectedIndex = 0;


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Set Installation date button.
        /// </summary>
        /// <param name="v_dDate"></param>
        private void SetInstallationDateButton(DateTime v_dDate)
        {

            try
            {

                //Set button properties.
                this.btnDisplayInstallationDate.Tag = v_dDate;
                this.btnDisplayInstallationDate.Content = cMain.ReturnDisplayDate(v_dDate, this.cmbDateCompare.SelectedItem.ToString());

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
                    //v1.0.5 - Converted to combo box item from string.
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

                //Installation Date.
                DateTime? dInstallationDate = null;
                if (this.chkUseInstallationDate.IsChecked == true)
                {
                    dInstallationDate = this.dtPicker.Value;

                }

                //Surveyed
                

                //Confirmed
                cbiItem = (ComboBoxItem)this.cmbConfirmed.SelectedItem;
                int iConfirmed = -1;
                int.TryParse(cbiItem.Tag.ToString(), out iConfirmed);

                //Date comparison
                string sDateComparison = this.cmbDateCompare.SelectedItem.ToString();


                int iInstall_Awaiting = Convert.ToInt32(cMain.GetAppResourceValue("InstallStatus_AwaitingSurvey"));
                int iInstall_Cancel = Convert.ToInt32(cMain.GetAppResourceValue("InstallStatus_SurveyCancelled"));


                //Process Search
                List<cSurveyDatesResult> cResults = cMain.p_cDataAccess.SearchSurveyDates(sProjectNo, this.txtDeliveryStreet.Text, this.txtPostcode.Text, iInstallStatus,iProgressStatus, dInstallationDate, sDateComparison, string.Empty, iConfirmed, this.chkSyncChangesOnly.IsChecked.Value, this.chkShowAllStatuses.IsChecked.Value, this.chkShowAllProgessStatus.IsChecked.Value, sSubProjectNo, iInstall_Awaiting, iInstall_Cancel,this.cmbInstallStatus_Filter.SelectedItem.ToString());

                int iUpdates = 0;
                int iTotalUnits = 0;
                int iUnitsInstalled = 0;
                bool bPartInstall = false;
                bool bOnHold = false;
                foreach (cSurveyDatesResult cResult in cResults)
                {

                    bPartInstall = false;
                    bOnHold = false;

                    cResult.SubProjectWidth = this.cmbProjectNoFilter.ActualWidth + 10;
                    cResult.ScreenWidth = this.lvResults.ActualWidth;
                    cResult.StreetWidth = this.txtDeliveryStreet.ActualWidth;
                    cResult.InstallWidth = this.cmbInstallStatus.ActualWidth;
                    cResult.PostcodeWidth = this.txtPostcode.ActualWidth;
                    cResult.ProgressWidth = this.cmbProgressStatus.ActualWidth;


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

                    cResult.UnitsLeft = "0";
                    if (iTotalUnits > 0)
                    {
                        cResult.UnitsLeft = (iTotalUnits - iUnitsInstalled).ToString();
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


                    //v1.0.2 - Set background colour if project status is "On Hold"
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
        /// Restore saved search criteria.
        /// </summary>
        private void RestoreSearchCriteria()
        {

            try
            {

                int iIndex = 0;

                //If not set then leave.
                if (cMain.p_cSearchCriteria_LastSearch == null) { return; }


                //v1.0.1 - 
                this.cmbProjectNoFilter.SelectedItem = cMain.p_cSearchCriteria_LastSearch.ProjectNoFilter;
                if (cMain.p_cSearchCriteria_LastSearch.ProjectNoFilter.Equals(cSettings.p_sProjectNoFilter_ProjectNo) == true)
                {

                    //v1.0.5 - Now combo box item instead of string.
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


                //Install status
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

                this.cmbInstallStatus_Filter.SelectedItem = cMain.p_cSearchCriteria_LastSearch.InstallStatus_Filter;

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

                this.chkUseInstallationDate.IsChecked = cMain.p_cSearchCriteria_LastSearch.IncludeInstallPlanDate;
                
                this.cmbDateCompare.SelectedItem = cMain.p_cSearchCriteria_LastSearch.InstallPlanDateComparison;
                this.SetInstallationDateButton(cMain.p_cSearchCriteria_LastSearch.InstallPlanDate.Value);

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
                ComboBoxItem cmbItem = null;

                //v1.0.1 - 
                cMain.p_cSearchCriteria_LastSearch.ProjectNoFilter = this.cmbProjectNoFilter.SelectedItem.ToString();


                if (cMain.p_cSearchCriteria_LastSearch.ProjectNoFilter.Equals(cSettings.p_sProjectNoFilter_ProjectNo) == true)
                {
                    //v1.0.5 - Now using combo box item instead of string.
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

                cMain.p_cSearchCriteria_LastSearch.InstallStatus_Filter = this.cmbInstallStatus_Filter.SelectedItem.ToString();

                //Progress status
                cmbItem = (ComboBoxItem)this.cmbProgressStatus.SelectedItem;
                cMain.p_cSearchCriteria_LastSearch.ProgressStatus = Convert.ToInt32(cmbItem.Tag);             

                cMain.p_cSearchCriteria_LastSearch.IncludeInstallPlanDate = this.chkUseInstallationDate.IsChecked.Value;
                cMain.p_cSearchCriteria_LastSearch.InstallPlanDate = (DateTime)this.btnDisplayInstallationDate.Tag;
                cMain.p_cSearchCriteria_LastSearch.InstallPlanDateComparison = this.cmbDateCompare.SelectedItem.ToString();
                

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
        /// Display installation date.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDisplayInstallationDate_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (LightDismissAnimatedPopup.IsOpen != true)
                {

                    double dXOffSet = this.cmbProjectNo.ActualWidth + this.txtDeliveryStreet.ActualWidth + this.txtPostcode.ActualWidth + this.cmbInstallStatus.ActualWidth + this.cmbProgressStatus.ActualWidth;
                    this.LightDismissAnimatedPopup.HorizontalOffset = dXOffSet;
                    
                    LightDismissAnimatedPopup.IsOpen = true;
                    if (this.btnDisplayInstallationDate.Tag != null)
                    {
                        dtPicker.Value = Convert.ToDateTime(this.btnDisplayInstallationDate.Tag);

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
        /// Apply installation date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnApplyInstallationDate_Click(object sender, RoutedEventArgs e)
        {

            try
            {               

                //Record all the sub project numbers.
                List<String> lsSubProjectNos = new List<String>();

                //v1.0.2 - List of string containing sub project that will not display on front screen
                List<string> lsSubProjectNoDisplay = new List<string>();
                bool bWillItDisplay = false;
                int? iConfirmed = 1;

                //Place date and time in strings.
                String sDate = this.dtPickerSurveyDate.Value.ToString("dd/MM/yyyy");
                String sTime = "00:00";

                //Put date and time together.
                DateTime dSurveyDate = Convert.ToDateTime(sDate + " " + sTime);

                StringBuilder sbInstalMsg = new StringBuilder();

                foreach (cSurveyDatesResult lviItem in this.lvResults.SelectedItems)
                {
                    lsSubProjectNos.Add(lviItem.SubProjectNo);

                    //Check if sub projects statuses will allow it to display on the front screen.
                    bWillItDisplay = cMain.WillSubProjectDisplayOnFrontScreen(iConfirmed, lviItem.Mxm1002InstallStatus);
                    if (bWillItDisplay == false)
                    {
                        lsSubProjectNoDisplay.Add(lviItem.SubProjectNo);

                    }

                    //Check install date is not beyond delivery date, if so log so we can tell the user.
                    if (lviItem.Delivery_EndDateTime.HasValue == true && lviItem.Delivery_EndDateTime.Value.Year > 1900)
                    {

                        //v1.0.3 - Change comparison to check if installation date is before delivery date.
                        if (Convert.ToInt32(dSurveyDate.ToString("yyyyMMdd")) < Convert.ToInt32(lviItem.Delivery_EndDateTime.Value.ToString("yyyyMMdd")))
                        {

                            sbInstalMsg.Append(lviItem.SubProjectNo + " - Delivery date : " + cMain.ReturnDisplayDate(lviItem.Delivery_EndDateTime.Value));
                            sbInstalMsg.Append(Environment.NewLine);

                        }
                    }

                }

                //If any sub projects have a delivery date after the install date then let user know.
                if (sbInstalMsg.Length > 0)
                {
                    
                    string sMessage = "The installation date you have set is before the planned delivery dates for the following sub projects:" + Environment.NewLine + Environment.NewLine + sbInstalMsg.ToString();
                    await cSettings.DisplayMessage(sMessage, "Warning");

                }

                //v1.0.2 - Warn user if sub project will not display on front screen as status incorrect.                              
                if (lsSubProjectNoDisplay.Count > 0)
                {

                    string sMessage = cMain.ReturnSubProjectWillNotDisplayOnFrontScreenMessage(lsSubProjectNoDisplay);
                    await cSettings.DisplayMessage(sMessage, "Warning");

                }



                bool bValid = await cMain.p_cDataAccess.ApplySurveyDatesToSubProjects(lsSubProjectNos, dSurveyDate);
                if (bValid == true)
                {
                    this.DisplaySearchResults();
                    this.SetTheInstallationDatesPopup.IsOpen = false;
                }
                else
                {
                    await cSettings.DisplayMessage("A problem occurred when trying to apply the installation dates, please try again.", "Cannot Apply Installation Date.");

                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }
     

        /// <summary>
        /// User search date click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUseSearchDate_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //Set the date time on the survey button.
                this.SetInstallationDateButton(dtPicker.Value);

                //Close pop up.
                LightDismissAnimatedPopup.IsOpen = false;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// User installation date.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkUseInstallationDate_Checked(object sender, RoutedEventArgs e)
        {


            try
            {

                if (this.m_bPageLoaded == false)
                {
                    return;
                }

                this.DisplaySearchResults();

                this.btnDisplayInstallationDate.IsEnabled = true;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        private void chkUseInstallationDate_UnChecked(object sender, RoutedEventArgs e)
        {

            try
            {

                if (this.m_bPageLoaded == false)
                {
                    return;
                }

                this.DisplaySearchResults();

                this.btnDisplayInstallationDate.IsEnabled = false;


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Project number selection changed.
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
        /// 
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
        /// Delivery street text changed.
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
        /// Postcode text changed.
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
        /// Install status selection change.
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
        /// Confirmed drop down selection change.
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
        /// List view results selection change.
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnEditInstallation_Click(object sender, RoutedEventArgs e)
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
                this.Frame.Navigate(typeof(SetInstallationDetailsPage), sSubProjID);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Select all
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
        /// De-Select All
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
        /// Set installation dates button clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnSetInstallationDates_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (this.SetTheInstallationDatesPopup.IsOpen == true)
                {
                    this.SetTheInstallationDatesPopup.IsOpen = false;
                }
                else
                {

                    if (this.lvResults.SelectedItems.Count == 0)
                    {
                        await cSettings.DisplayMessage("You must select some project first before you can set there survey dates.", "Project selection required.");
                        return;

                    }

                    this.ConfigureInstallationDate();

                    this.SetTheInstallationDatesPopup.IsOpen = true;
                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);


            }

        }

        /// <summary>
        /// Configure installation date popup.
        /// </summary>
        private void ConfigureInstallationDate()
        {

            try
            {

                //Set to first sub project with valid date time.
                foreach (cSurveyDatesResult cSurvey in this.lvResults.SelectedItems)
                {

                    if (cSurvey.EndDateTime.HasValue == true)
                    {

                        this.dtPickerSurveyDate.Value = cSurvey.EndDateTime.Value;
                        
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
        /// Un-Confirm Installations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnUnConfirmInstallations_Click(object sender, RoutedEventArgs e)
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

                        //Clear confirmed action date.
                        cSubProjectData.ConfirmedActionDateTime = null;

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
        /// View on map
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
                    this.Frame.Navigate(typeof(ViewMapPage), lviItem.SubProjectNo);

                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Show all statuses
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
        /// Do not show all statuses
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
        /// Show all progress statuses
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
        /// Do not show all progress statuses
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
        /// v1.0.1 - Install status filter selection change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbInstallStatus_Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
        /// Progress status selection changed.
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
        /// v1.0.11 - Allow for viewing notes for sub project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnViewNotes_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //Check they have only one selected.
                if (this.lvResults.SelectedItems.Count != 1)
                {
                    await cSettings.DisplayMessage("You can only view notes for one sub project at a time, please re-select and try again.", "View Notes");
                    return;
                }


                //Loop through selected items and then redirect to the picture page.
                foreach (cSurveyDatesResult lviItem in this.lvResults.SelectedItems)
                {

                    //Redirect to photo page.
                    this.Frame.Navigate(typeof(ViewNotes), lviItem.SubProjectNo);

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.11 - Allow for viewing pictures for sub project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnViewPics_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //Check they have only one selected.
                if (this.lvResults.SelectedItems.Count != 1)
                {
                    await cSettings.DisplayMessage("You can only view pictures for one sub project at a time, please re-select and try again.", "View Pictures");
                    return;
                }


                //Loop through selected items and then redirect to the picture page.
                foreach (cSurveyDatesResult lviItem in this.lvResults.SelectedItems)
                {

                    //Redirect to photo page.
                    this.Frame.Navigate(typeof(SetInstallationPhotosPage), lviItem.SubProjectNo);

                }
                             
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


        }


        /// <summary>
        /// v1.0.11 - Allow for project status update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnProjectStatusUpdate_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //Check they have only one selected.
                if (this.lvResults.SelectedItems.Count != 1)
                {
                    await cSettings.DisplayMessage("You can only update the progress status for one sub project at a time, please re-select and try again.", "Progress Status");
                    return;
                }


                //Loop through selected items and then redirect to the picture page.
                foreach (cSurveyDatesResult lviItem in this.lvResults.SelectedItems)
                {


                    //Set status to what is currently set.
                    int iIndex = 0;
                    foreach (ComboBoxItem cmbItem in this.cmbNewProgressStatus.Items)
                    {
                        if (cmbItem.Tag.ToString() == lviItem.Mxm1002ProgressStatus.ToString())
                        {
                            this.cmbNewProgressStatus.SelectedIndex = iIndex;
                            break;
                        }

                        iIndex++;
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
        /// v1.0.11 - Update sub project to new status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateProjectStatus_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //Loop through selected items and then redirect to the picture page.
                foreach (cSurveyDatesResult lviItem in this.lvResults.SelectedItems)
                {

                    //Progress status
                    ComboBoxItem cmbItem = (ComboBoxItem)this.cmbNewProgressStatus.SelectedItem;
                    int iSelectedStatus = Convert.ToInt32(cmbItem.Tag);

                    //Only update if it's changed.
                    if (lviItem.Mxm1002ProgressStatus != iSelectedStatus)
                    {

                        //Fetch name of selected progress status
                        string sSelectedStatusName = cMain.p_cDataAccess.GetEnumValueName("ProjTable","Mxm1002ProgressStatus", iSelectedStatus);

                        //Update local database
                        cProjectTable cSubProject = cMain.p_cDataAccess.GetSubProjectProjectData(lviItem.SubProjectNo);
                        cSubProject.Mxm1002ProgressStatus = iSelectedStatus;
                        cSubProject.ProgressStatusName = sSelectedStatusName;

                        cMain.p_cDataAccess.UpdateSubProjectData(cSubProject);

                        //Add update to update list
                        cMain.p_cDataAccess.AddToUpdateTable(lviItem.SubProjectNo, "Mxm1002ProgressStatus",iSelectedStatus.ToString());

                        //Hide fly out
                        this.foProgressStatus.Hide();

                        //Re-process search.
                        this.DisplaySearchResults();

                    }
                                   
                }
                
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }
      
    }
}
