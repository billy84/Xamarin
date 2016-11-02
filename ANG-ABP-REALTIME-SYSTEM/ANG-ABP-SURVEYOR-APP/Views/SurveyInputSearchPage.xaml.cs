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
using ANG_ABP_SURVEYOR_APP.Common;
using ANG_ABP_SURVEYOR_APP_CLASS;
using ANG_ABP_SURVEYOR_APP_CLASS.Classes;
using ANG_ABP_SURVEYOR_APP_CLASS.Model;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ANG_ABP_SURVEYOR_APP.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SurveyInputSearchPage : Page
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
        /// v1.0.19 - Flag to indicate we are in surveyed mode.
        /// </summary>
        private bool m_bSurveyedMode = false;

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


        public SurveyInputSearchPage()
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

                if (e.Parameter != null)
                {
                    if (e.Parameter.GetType() == typeof(cSearchCriteria))
                    {
                        this.m_cCriteria = (cSearchCriteria)e.Parameter;
                       

                    }                    

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
        /// Project number selection change.
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
        /// Install status selection change.
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
        /// Use survey plan date unchecked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkUseSurveyPlanDate_UnChecked(object sender, RoutedEventArgs e)
        {

            try
            {

                this.DisplaySearchResults();

                this.btnSetSurveyDate.IsEnabled = false;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Use survey plan date checked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkUseSurveyPlanDate_Checked(object sender, RoutedEventArgs e)
        {

            try
            {

                this.DisplaySearchResults();

                this.btnSetSurveyDate.IsEnabled = true;


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Set the survey date to use.
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
        /// Surveyor selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbSurveyor_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
        /// Page load event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SurveyInputPage_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {

                this.PopulateDropDowns();

                this.ConfigureSearchCriteria();

                this.SetSurveyDateButton(DateTime.Now);

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
        /// Set survey date button.
        /// </summary>
        /// <param name="v_dDate"></param>
        private void SetSurveyDateButton(DateTime v_dDate)
        {

            try
            {

                //Set button properties.
                this.btnSetSurveyDate.Tag = v_dDate;
                this.btnSetSurveyDate.Content = cMain.ReturnDisplayDate(v_dDate,this.cmbDateCompare.SelectedItem.ToString());

                //Display search results
                this.DisplaySearchResults();

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

                //v1.0.14 Progress status
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

                //Surveyor
                string sSurveyor = string.Empty;
                cbiItem = (ComboBoxItem)this.cmbSurveyor.SelectedItem;
                sSurveyor = cbiItem.Tag.ToString();

                //Surveyed status.
                string sSurveyedStatus = cmbSurveyedStatus.SelectedItem.ToString();
 
                //Date comparison
                string sDateComparison = this.cmbDateCompare.SelectedItem.ToString();

                //Surveyed on site
                string sSurveyedOnSite = this.cmbSurveyInputStatus.SelectedItem.ToString();

                int iInstall_Awaiting = Convert.ToInt32(cMain.GetAppResourceValue("InstallStatus_AwaitingSurvey"));
                int iInstall_Cancel = Convert.ToInt32(cMain.GetAppResourceValue("InstallStatus_SurveyCancelled"));


                //v1.0.21 - Set health and safety incomplete filter.
                cDataAccess.HSFilters iHSFilter = cDataAccess.HSFilters.Complete;
                if (this.m_bSurveyedMode == true)
                {
                    iHSFilter = cDataAccess.HSFilters.InComplete;
                }

                List<cSurveyInputResult> cResults = cMain.p_cDataAccess.SearchSurveyInput(sProjectNo, this.txtDeliveryStreet.Text, this.txtPostcode.Text, iInstallStatus, iProgressStatus, dSurveyDate, sDateComparison, sSurveyedStatus, sSurveyor, sSurveyedOnSite, this.chkSyncChangesOnly.IsChecked.Value, this.chkShowAllStatuses.IsChecked.Value, this.chkShowAllProgessStatus.IsChecked.Value, sSubProjectNo, iInstall_Awaiting, iInstall_Cancel,false,cSettings.p_sInstallStatusFilter_EqualTo,cSettings.p_sAnyStatus,iHSFilter);
              
                int iUpdates = 0;
                foreach (cSurveyInputResult cResult in cResults)
                {

                    cResult.ScreenWidth = this.lvResults.ActualWidth;
                    cResult.StreetWidth = this.txtDeliveryStreet.ActualWidth; // +10;
                    cResult.InstallWidth = this.cmbInstallStatus.ActualWidth; // +20;
                    cResult.ProgressWidth = this.cmbProgressStatus.ActualWidth; //v1.0.14

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

                    
                    if (cResult.UpdateQty != null)
                    {              
                        if (int.TryParse(cResult.UpdateQty, out iUpdates) == true){

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

                    

                }

                this.lvResults.ItemsSource = cResults;


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);


            }

        }

        /// <summary>
        /// Populate drop downs.
        /// </summary>
        private async void PopulateDropDowns()
        {

            try
            {

                // ** Project numbers drop down.
                this.cmbProjectNo.Items.Add(cSettings.p_sAnyStatus);

                List<cProjectNo> lsProjectNos = cMain.p_cDataAccess.GetProjectNos();
                foreach (cProjectNo lsProjectNo in lsProjectNos)
                {
                    if (lsProjectNo.ProjectNo.Length > 0)
                    {
                        this.cmbProjectNo.Items.Add(lsProjectNo.ProjectNo);

                    }
                    
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
                this.cmbInstallStatus.SelectedIndex = 0;

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
                this.cmbSurveyedStatus.Items.Add(cSettings.p_sAnyStatus); //v1.0.1 - Include "Any" status.
                this.cmbSurveyedStatus.Items.Add(cSettings.p_sSurveyedStatus_NotSurveyed);
                this.cmbSurveyedStatus.Items.Add(cSettings.p_sSurveyedStatus_SurveyedOnSite);
                this.cmbSurveyedStatus.Items.Add(cSettings.p_sSurveyedStatus_SurveyedTrans);
                this.cmbSurveyedStatus.SelectedIndex = 1; //Default on Not Surveyed


                // ** Surveyor drop down.
                cmbItem = new ComboBoxItem();
                cmbItem.Content = cSettings.p_sAnyStatus;
                cmbItem.Tag = String.Empty;
                this.cmbSurveyor.Items.Add(cmbItem);

                //v1.0.3 - Retrieve logged on users profile.
                string sLoggedOnUserProfile = await ANG_ABP_SURVEYOR_APP_CLASS.cSettings.GetUserName();

                int iUsersIndex = -1;

                List<cSurveyor> oSurveyors = cMain.p_cDataAccess.GetSurveyors();
                foreach (cSurveyor oSurveyor in oSurveyors)
                {
                    cmbItem = new ComboBoxItem();
                    cmbItem.Content = oSurveyor.SurveyorName;
                    cmbItem.Tag = oSurveyor.SurveyorProfile;
                    this.cmbSurveyor.Items.Add(cmbItem);

                    //v1.0.3 - If user profiles match record index position.
                    if (oSurveyor.SurveyorProfile.Equals(sLoggedOnUserProfile, StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        iUsersIndex = this.cmbSurveyor.Items.Count - 1;
                    }

                }
                if (iUsersIndex == -1)
                {
                    this.cmbSurveyor.SelectedIndex = 0; //Make Any the default selection

                }
                else
                {
                    this.cmbSurveyor.SelectedIndex = iUsersIndex; //v1.0.3 - Set to logged on user.
                }


                //Survey Status
                this.cmbSurveyInputStatus.Items.Add(cSettings.p_sAnyStatus);
                this.cmbSurveyInputStatus.Items.Add(cSettings.p_sInputStatus_Successful);
                this.cmbSurveyInputStatus.Items.Add(cSettings.p_sInputStatus_NotPending);
                this.cmbSurveyInputStatus.Items.Add(cSettings.p_sInputStatus_Pending);
                this.cmbSurveyInputStatus.Items.Add(cSettings.p_sInputStatus_Failed);
                this.cmbSurveyInputStatus.SelectedIndex = 0;

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
                this.chkUseSurveyPlanDate.IsChecked = true;

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
                    if (this.m_cCriteria.SurveyPlanDate.HasValue == true)
                    {
                        this.chkUseSurveyPlanDate.IsChecked = true;

                        dtPicker.Value = this.m_cCriteria.SurveyPlanDate.Value;

                        //Set the date time on the survey button.
                        this.SetSurveyDateButton(dtPicker.Value);

                      
                        //this.m_cCriteria.SurveyPlanDateComparison;

                    }

                    //v1.0.21 - Include survey plan date, if not set.
                    if (this.m_cCriteria.IncludeSurveyPlanDate == false)
                    {
                        this.chkUseSurveyPlanDate.IsChecked = false;
                    }

                    //Surveyed Status
                    if (this.m_cCriteria.SurveyedStatus != null)
                    {


                        iIndex = 0;

                        foreach (string sItem in this.cmbSurveyedStatus.Items)
                        {

                            if (this.m_cCriteria.SurveyedStatus == sItem)
                            {

                                this.cmbSurveyedStatus.SelectedIndex = iIndex;
                                break;
                            }


                            iIndex++;

                        }

                       
                    }


                    //Surveyor
                    if (this.m_cCriteria.Surveyor != null)
                    {

                        iIndex = 0;

                        foreach (ComboBoxItem cmbItem in this.cmbSurveyor.Items)
                        {

                            if (this.m_cCriteria.Surveyor == cmbItem.Tag.ToString())
                            {

                                this.cmbSurveyor.SelectedIndex = iIndex;
                                break;

                            }

                            iIndex++;

                        }

                    }


                    //Survey on site
                    if (this.m_cCriteria.SurveyOnSite != null)
                    {

                        iIndex = 0;

                        foreach (string sItem in this.cmbSurveyInputStatus.Items)
                        {

                            if (sItem == this.m_cCriteria.SurveyOnSite)
                            {

                                this.cmbSurveyInputStatus.SelectedIndex = iIndex;
                                break;

                            }

                            iIndex++;
                        }


                    }

                    //v1.0.21 - Set surveyed mode flag.
                    this.m_bSurveyedMode = this.m_cCriteria.SurveyedMode;

                    //v1.0.21 - If surveyed mode then update screen title.
                    if (this.m_bSurveyedMode == true)
                    {
                        this.pageTitle.Text = "Surveyed Search";
                    }

                }



            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


        }
        

        /// <summary>
        /// Apply selected date to search results.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApplySelectedDate_Click(object sender, RoutedEventArgs e)
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
        /// List view tapped
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
        /// Complete survey
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnCompleteSurvey_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (this.lvResults.SelectedItems.Count() != 1)
                {
                   await cSettings.DisplayMessage("You need to select a single project first.", "Single project selection required.");
                   return;
                }

                cSurveyInputResult cResult = (cSurveyInputResult)this.lvResults.SelectedItem;

                if (cResult.SurveyInputStatus.Equals(cSettings.p_sInputStatus_Successful) == true)
                {
                    //Redirect to detail.
                    this.Frame.Navigate(typeof(SurveyInputEditPage), cResult.SubProjectNo);
                }
                else
                {
                    //Redirect to option page.
                    this.Frame.Navigate(typeof(SurveyInputOptionPage), cResult);
                }

               

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Select all sub projects.
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
        /// Clear all sub project selections
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
        /// View sub project location on map/.
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
                    this.Frame.Navigate(typeof(ViewMap), lviItem.SubProjectNo);

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

                //v1.0.14 - Restore progress status.
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
                foreach (ComboBoxItem cmbItem in this.cmbSurveyor.Items)
                {
                    if (cmbItem.Tag.ToString() == cMain.p_cSearchCriteria_LastSearch.Surveyor)
                    {
                        this.cmbSurveyor.SelectedIndex = iIndex;
                        break;
                    }

                    iIndex++;
                }

                this.cmbSurveyedStatus.SelectedItem = cMain.p_cSearchCriteria_LastSearch.SurveyedStatus;

                this.chkSyncChangesOnly.IsChecked = cMain.p_cSearchCriteria_LastSearch.SyncChangesOnly;

                //v1.0.1
                this.chkShowAllStatuses.IsChecked = cMain.p_cSearchCriteria_LastSearch.ShowAllStatuses;
                this.chkShowAllProgessStatus.IsChecked = cMain.p_cSearchCriteria_LastSearch.ShowAllProgressStatuses;

                //v1.0.19 - Restore surveyed mode flag status.
                this.m_bSurveyedMode = cMain.p_cSearchCriteria_LastSearch.SurveyedMode;

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

                //v1.0.14 - Record progress status.
                cmbItem = (ComboBoxItem)this.cmbProgressStatus.SelectedItem;
                cMain.p_cSearchCriteria_LastSearch.ProgressStatus = Convert.ToInt32(cmbItem.Tag);

                cMain.p_cSearchCriteria_LastSearch.IncludeSurveyPlanDate = this.chkUseSurveyPlanDate.IsChecked.Value;
                cMain.p_cSearchCriteria_LastSearch.SurveyPlanDate = (DateTime)this.btnSetSurveyDate.Tag;
                cMain.p_cSearchCriteria_LastSearch.SurveyPlanDateComparison = this.cmbDateCompare.SelectedItem.ToString();

                cMain.p_cSearchCriteria_LastSearch.SurveyedStatus = this.cmbSurveyedStatus.SelectedItem.ToString();

                cmbItem = (ComboBoxItem)this.cmbSurveyor.SelectedItem;
                cMain.p_cSearchCriteria_LastSearch.Surveyor = cmbItem.Tag.ToString();

                cMain.p_cSearchCriteria_LastSearch.SurveyOnSite = this.cmbSurveyedStatus.SelectedItem.ToString();

                cMain.p_cSearchCriteria_LastSearch.SyncChangesOnly = this.chkSyncChangesOnly.IsChecked.Value;

                cSurveyInputResult cResult = (cSurveyInputResult)this.lvResults.SelectedItem;
                if (cResult != null)
                {
                    cMain.p_cSearchCriteria_LastSearch.Selected_SubProjectNo = cResult.SubProjectNo;
                }

                //v1.0.1
                cMain.p_cSearchCriteria_LastSearch.ShowAllStatuses = this.chkShowAllStatuses.IsChecked.Value;
                cMain.p_cSearchCriteria_LastSearch.ShowAllProgressStatuses = this.chkShowAllProgessStatus.IsChecked.Value;


                //v1.0.19 - Store surveyed flag status
                cMain.p_cSearchCriteria_LastSearch.SurveyedMode = this.m_bSurveyedMode;


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
        /// v1.0.1 - Project number filter.
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
        /// v1.0.1 - Sub project text changed.
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
        /// v1.0.1 - Show all progress status
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
        /// v1.0.1 - Show all progress status
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
        /// v1.0.1 - Show all progress status
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
        /// v1.0.1 - Show all progress status
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
        /// Postcode changed.
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
        /// v1.0.14 - Progress status selection change.
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
 
    }

}
