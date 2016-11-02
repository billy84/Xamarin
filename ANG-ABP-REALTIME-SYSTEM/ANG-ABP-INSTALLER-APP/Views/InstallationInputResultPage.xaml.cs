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
using ANG_ABP_SURVEYOR_APP_CLASS.Classes;
using ANG_ABP_SURVEYOR_APP_CLASS.Model;
using ANG_ABP_INSTALLER_APP;
using ANG_ABP_SURVEYOR_APP_CLASS;
using System.Collections.ObjectModel;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ANG_ABP_INSTALLER_APP.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class InstallationInputResultPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// Stores the survey object passed through on the redirect.
        /// </summary>
        private cSurveyInputResult m_cSurvey = null;

        /// <summary>
        /// Set if redirected from dashboard.
        /// </summary>
        private cDashboardWork m_cDashWk = null;

        /// <summary>
        /// Holds project details.
        /// </summary>
        private cProjectTable m_cProject = null;

        /// <summary>
        /// Flag to indicate the page has loaded.
        /// </summary>
        private bool m_bPageLoaded = false;

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


        public InstallationInputResultPage()
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

                if (e.Parameter != null)
                {
                    if (e.Parameter.GetType() == typeof(cSurveyInputResult))
                    {
                        this.m_cSurvey = (cSurveyInputResult)e.Parameter;
                        this.m_cProject = cMain.p_cDataAccess.GetSubProjectProjectData(this.m_cSurvey.SubProjectNo);

                    }
                    else if (e.Parameter.GetType() == typeof(cDashboardWork))
                    {
                        this.m_cDashWk = (cDashboardWork)e.Parameter;
                        this.m_cProject = cMain.p_cDataAccess.GetSubProjectProjectData(this.m_cDashWk.SubProjectNo);

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
        }

        #endregion

        /// <summary> 
        /// Page loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InstallationInputResultPage_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {

                //Default to invisible.
                this.gdControls.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                this.tbSubProjectNo.Text = this.m_cProject.SubProjectNo;
                this.tbAddress.Text = this.m_cProject.DeliveryStreet;

                //v1.0.11 - If installation date use that, if not todays date.
                if (this.m_cProject.EndDateTime.HasValue == true)
                {
                    this.SetInstallDateButton(this.m_cProject.EndDateTime.Value);
                    this.SetOrderCompleteDateButton(this.m_cProject.EndDateTime.Value); //v1.0.11
                }
                else
                {
                    this.SetInstallDateButton(DateTime.Now);
                    this.SetOrderCompleteDateButton(DateTime.Now); //v1.0.11
                }
                
                this.dtPicker.Value = DateTime.Now;

                this.isSearch.InstallerSelected += isSearch_InstallerSelected;

                this.m_bPageLoaded = true;
             

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// v1.0.9 - Installer selected event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void isSearch_InstallerSelected(object sender, cInstallersTable e)
        {

            try
            {

                this.ConfigureInstallersButton(e);

                this.foSearch.Hide();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.9 - Configure installers button.
        /// </summary>
        /// <param name="v_cInstaller"></param>
        private void ConfigureInstallersButton(cInstallersTable v_cInstaller)
        {
            try
            {

                this.btnSelectInstallers.Tag = v_cInstaller;
                this.btnSelectInstallers.Content = v_cInstaller.Name;
                this.btnSelectInstallers.FontSize = 18;
                this.SetInstallersButtonToolTip(v_cInstaller.Name);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


        }

        /// <summary>
        /// v1.0.9 - Set installers button tooltip
        /// </summary>
        /// <param name="v_sText"></param>
        private void SetInstallersButtonToolTip(string v_sText)
        {

            try
            {

                ToolTip toolTip = new ToolTip();
                toolTip.Content = v_sText;
                toolTip.FontSize = 20;
                ToolTipService.SetToolTip(this.btnSelectInstallers, toolTip);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Use selected installation date.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUseInstallDate_Click(object sender, RoutedEventArgs e)
        {


            try
            {


                this.SetInstallDateButton(this.dtPicker.Value);
                this.puDatePicker.IsOpen = false;


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }




        /// <summary>
        /// Set Install date button.
        /// </summary>
        /// <param name="v_dDate"></param>
        private void SetInstallDateButton(DateTime v_dDate)
        {

            try
            {

                //Set button properties.
                this.btnSelectInstallDate.Tag = v_dDate;
                this.btnSelectInstallDate.Content = cMain.ReturnDisplayDate(v_dDate, ANG_ABP_SURVEYOR_APP_CLASS.cSettings.p_sDateCompare_EqualTo);
               
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), v_dDate.ToString());

            }

        }


        /// <summary>
        /// v1.0.11 - Set Order Complete date button.
        /// </summary>
        /// <param name="v_dDate"></param>
        private void SetOrderCompleteDateButton(DateTime v_dDate)
        {

            try
            {                 

                //Set button properties.
                this.btnSetOrderCompleteDate.Tag = v_dDate;
                this.btnSetOrderCompleteDate.Content = cMain.ReturnDisplayDate(v_dDate, ANG_ABP_SURVEYOR_APP_CLASS.cSettings.p_sDateCompare_EqualTo);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), v_dDate.ToString());

            }

        }

        /// <summary>
        /// Select installation date.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectInstallDate_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (this.puDatePicker.IsOpen == false)
                {
                    this.puDatePicker.IsOpen = true;
                }
                else
                {
                    this.puDatePicker.IsOpen = false;
                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Installed full option selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInstalledFull_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //v1.0.5 - Populate remake reasons.
                this.PopulateRemakeReasons();

                this.ShowInstallersList(Windows.UI.Xaml.Visibility.Visible);     
        
                //v1.0.9 - Display previously selected installation team if there is one.
                if (this.m_cProject.ABPAXINSTALLATIONTEAM.Length > 0)
                {
                     cInstallersTable cInstall = cMain.p_cDataAccess.FetchInstallationTeam(this.m_cProject.ABPAXINSTALLATIONTEAM);
                     this.ConfigureInstallersButton(cInstall); 
                   
                }
               

                this.btnPartialInstall.IsEnabled = false;
                this.btnReBook.IsEnabled = false;

                this.tbEnterReasonPrompt.Visibility = Windows.UI.Xaml.Visibility.Visible;
                this.tbEnterReasonPrompt.Text = "Add Note:";

                this.txtReason.Visibility = Windows.UI.Xaml.Visibility.Visible;

                this.tbInstallerPrompt.Text = "Select Installation Team:";
                this.cmbRebookReason.Visibility = Windows.UI.Xaml.Visibility.Collapsed; //v1.0.10 - no need to show rebook drop down.

                this.gdControls.Visibility = Windows.UI.Xaml.Visibility.Visible;              

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Show installers list.
        /// </summary>
        /// <param name="v_vVisibility"></param>
        private void ShowInstallersList(Windows.UI.Xaml.Visibility v_vVisibility)
        {

            try
            {

                this.btnSelectInstallers.Visibility = v_vVisibility;
                this.tbInstallerPrompt.Visibility = v_vVisibility;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Update project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnUpdateProject_Click(object sender, RoutedEventArgs e)
        {

            bool bOK = false;
            DateTime dInstallDate;
            DateTime dOrderCompDate = DateTime.MinValue; //v1.0.11
            cInstallersTable cInstallers = null; //v1.0.9
            try
            {

                if (this.btnSelectInstallDate.Tag == null)
                {

                    await cSettings.DisplayMessage("You need to select an installation date before you can proceed.", "Installation Date Required.");
                    this.btnSelectInstallDate.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                    return;

                }
                if (DateTime.TryParse(this.btnSelectInstallDate.Tag.ToString(), out dInstallDate) == false)
                {
                    await cSettings.DisplayMessage("Please re-select the installation date and try again.", "Installation Date Required.");
                    this.btnSelectInstallDate.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                    return;
                }

                //Full install update.
                if (this.btnInstalledFull.IsEnabled == true)
                {

                    if (this.btnSelectInstallers.Tag == null)                    
                    {
                        await cSettings.DisplayMessage("You need to select an installation team before you can proceed.", "Installation team Required.");
                        this.btnSelectInstallers.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                        return;

                    }
                    else
                    {
                        cInstallers = (cInstallersTable)this.btnSelectInstallers.Tag;

                    }

                    //v1.0.11 - If setting order complete date, check no remakes exist.
                    if (this.chkSetOrderCompleteDate.IsChecked == true)
                    {

                        if (this.btnSetOrderCompleteDate.Tag == null)
                        {

                            await cSettings.DisplayMessage("You need to select an order complete date before you can proceed.", "Order Completion Date Required.");
                            this.btnSetOrderCompleteDate.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                            return;

                        }
                        if (DateTime.TryParse(this.btnSetOrderCompleteDate.Tag.ToString(), out dOrderCompDate) == false)
                        {
                            await cSettings.DisplayMessage("Please re-select the order complete date and try again.", "Order Completion Date Required.");
                            this.btnSetOrderCompleteDate.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                            return;
                        }

                        bool bAnyRemakes = await cMain.DoesSubProjectHaveInCompleteRemakes(this.m_cProject.SubProjectNo, "Please un-tick the 'set order complete date' option and try again.");
                        if (bAnyRemakes == true)
                        {
                            return;
                        }

                    }

                    string sNoteText = string.Empty;

                    //v1.0.5 - Check if reason for remake reason has been specified.
                    this.txtReason.Text = this.txtReason.Text.Trim();
                    if (this.cmbRemakeRequired.SelectedItem.ToString() != cSettings.p_sNotRequired)
                    {
                        //If no comment then prompt.
                        if (this.txtReason.Text.Length == 0)
                        {
                            await cSettings.DisplayMessage("Please enter a note as to why a remake is required.", "Note Required.");
                            this.txtReason.Focus(FocusState.Programmatic);
                            return;
                  
                        }
                        else
                        {
                            sNoteText = "Remake Required (" + this.cmbRemakeRequired.SelectedItem.ToString() + "): ";
                        }
                         
                    }

                    sNoteText += this.txtReason.Text;
                    if (sNoteText.Length > 0)
                    {

                        //Return note object
                        cProjectNotesTable cNote = await cSettings.ReturnNoteObject(this.m_cProject.SubProjectNo, sNoteText, DateTime.Now, cSettings.p_sProjectNoteType_FullInstall);

                        //Save sub project notes.
                        bOK = cMain.p_cDataAccess.SaveSubProjectNote(cNote);
                        if (bOK == false)
                        {
                            await cSettings.DisplayMessage("An error occurred when trying to save, please try again.", "Error When Saving.");
                            return;
                        }

                    }

                    

                    //Update main table with installation team number.
                    cProjectTable cSubProject = cMain.p_cDataAccess.GetSubProjectProjectData(this.m_cProject.SubProjectNo);

                    cSubProject.ABPAXINSTALLATIONTEAM = cInstallers.AccountNum;

                    cSubProject.Mxm1002InstallStatus = cSettings.p_iInstallStatus_InstalledFully;                    
                    cSubProject.InstallStatusName = cMain.p_cDataAccess.GetEnumValueName("PROJTABLE", "Mxm1002InstallStatus", cSettings.p_iInstallStatus_InstalledFully);

                    cMain.p_cDataAccess.AddToUpdateTable(this.m_cProject.SubProjectNo, "Mxm1002InstallStatus", cSettings.p_iInstallStatus_InstalledFully.ToString());

                    cSubProject.EndDateTime = dInstallDate;
                    cMain.p_cDataAccess.AddToUpdateTable(this.m_cProject.SubProjectNo, "StartDateTime", dInstallDate.ToString());

                    cSubProject.StartDateTime = dInstallDate;
                    cMain.p_cDataAccess.AddToUpdateTable(this.m_cProject.SubProjectNo, "EndDateTime", dInstallDate.ToString());

                    //v1.0.11 - Order complete date.
                    if (this.chkSetOrderCompleteDate.IsChecked == true)
                    {
                        cSubProject.ABPAWOrderCompletedDate = dOrderCompDate;
                        cMain.p_cDataAccess.AddToUpdateTable(this.m_cProject.SubProjectNo, "ABPAWOrderCompletedDate", dInstallDate.ToString());
                    }
                    
                    cMain.p_cDataAccess.UpdateProjectTable(cSubProject);

                    
                    //Work through units and update.
                    List<cUnitsTable> cAllUnits = cMain.p_cDataAccess.FetchUnitsForSubProject(this.m_cProject.SubProjectNo);
                    foreach (cUnitsTable cUnit in cAllUnits)
                    {
                                              
                        cUnit.InstalledStatus = cSettings.p_iUnits_InstalledStatus;
                                                     
                        bOK = cMain.p_cDataAccess.UpdateUnit(cUnit);
                        if (bOK == true)
                        {

                            bOK = cMain.p_cDataAccess.LogUnitUpdate(this.m_cProject.SubProjectNo, cUnit.UnitNo, cSettings.p_iUnits_InstalledStatus, dInstallDate);
                            if (bOK == false)
                            {
                                await cSettings.DisplayMessage("An issue has occurred when trying to save, please try again.", "Please save again.");
                                return;

                            }
                           
                        }
                  
                    }
  
                    //v1.0.11 - Go back now we are complete.
                    this.navigationHelper.GoBack();

                }
                else if (this.btnReBook.IsEnabled == true)
                {

                    if (this.cmbRebookReason.SelectedIndex == 0)
                    {
                        await cSettings.DisplayMessage("Please select a reason for this re-book and then try again.", "Re-book reason Required.");
                        this.cmbRemakeRequired.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                        return;
                    }

                    this.txtReason.Text = this.txtReason.Text.Trim();
                    if (this.cmbRebookReason.SelectedItem.ToString() == "Other")
                    {
                        if (this.txtReason.Text.Trim().Length == 0)
                        {

                            await cSettings.DisplayMessage("As you have selected 'Other' as the re-book reason, you must enter a reason in the comment box.", "Re-book comment Required.");
                            this.txtReason.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                            return;

                        }

                    }

                    //Put together comment, add selected reason first then comment.
                    string sComment = this.cmbRebookReason.SelectedItem.ToString() + ": " + this.txtReason.Text;

                    //Create note object for saving.
                    cProjectNotesTable cNote = await cSettings.ReturnNoteObject(this.m_cProject.SubProjectNo, sComment.Trim(), DateTime.Now, cSettings.p_sProjectNoteType_InstallReBook);
                    bool bSavedOK = cMain.p_cDataAccess.SaveSubProjectNote(cNote);
                    if (bSavedOK == false)
                    {

                        await cSettings.DisplayMessage("An issue has occurred when trying to save, please try again.", "Please save again.");
                        return;

                    }

                    //Apply install dates to sub project.
                    List<string> lSubProjects = new List<string>();
                    lSubProjects.Add(this.m_cProject.SubProjectNo);

                    await cMain.p_cDataAccess.ApplySurveyDatesToSubProjects(lSubProjects, dInstallDate);


                    //We have finished, go back.
                    this.navigationHelper.GoBack();

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Rebook Clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReBook_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //v1.0.11 - Hide order complete controls
                this.btnSetOrderCompleteDate.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.chkSetOrderCompleteDate.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.tbSetOrderCompleteDate.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                this.tbEnterReasonPrompt.Visibility = Windows.UI.Xaml.Visibility.Visible;
                this.txtReason.Visibility = Windows.UI.Xaml.Visibility.Visible;

                this.tbInstallerPrompt.Text = "Select re-book reason:";

                this.PopuleReBookReasonsDropDown();
                this.ShowInstallersList(Windows.UI.Xaml.Visibility.Visible);
                this.btnSelectInstallers.Visibility = Windows.UI.Xaml.Visibility.Collapsed; //v1.0.10 - Hide installers button.
                this.cmbRebookReason.Visibility = Windows.UI.Xaml.Visibility.Visible; //Show rebook reason list

                this.btnSelectInstallers.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.btnPartialInstall.IsEnabled = false;
                this.btnInstalledFull.IsEnabled = false;

                //v1.0.5 - We do not show remake on re-booking.
                this.tbRemakeRequired.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.cmbRemakeRequired.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                this.gdControls.Visibility = Windows.UI.Xaml.Visibility.Visible;     

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Populate rebook reasons drop down.
        /// </summary>
        private void PopuleReBookReasonsDropDown()
        {
            try
            {

                this.cmbRebookReason.Items.Clear();

                this.cmbRebookReason.Items.Add(cSettings.p_sPleaseChoose);

                List<cBaseEnumsTable> beEnums =  cMain.p_cDataAccess.GetEnumsForField("ABPAXREBOOKREASON_NOTUSED");
                foreach (cBaseEnumsTable beEnum in beEnums)
                {

                    if (beEnum.EnumName.Trim().Length > 0)
                    {
                        this.cmbRebookReason.Items.Add(beEnum.EnumName);

                    }
                   
                }


                this.cmbRebookReason.SelectedIndex = 0; //v1.0.10 - Select first in list.
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Partially installed option.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPartialInstall_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                this.Frame.Navigate(typeof(InstallationInputPartialPage), this.m_cProject.SubProjectNo);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.1 - Populate remake reasons.
        /// </summary>
        private void PopulateRemakeReasons()
        {

            try
            {

                this.cmbRemakeRequired.Items.Clear();              

                this.cmbRemakeRequired.Items.Add(cSettings.p_sNotRequired);

                List<cBaseEnumsTable> lReasons = cMain.p_cDataAccess.GetEnumsForField("ABPAXPartInstallReasons_NotUsed");
                foreach (cBaseEnumsTable cReason in lReasons)
                {
                    if (cReason.EnumName.Trim().Length > 0)
                    {
                        this.cmbRemakeRequired.Items.Add(cReason.EnumName);
                    }

                }

                this.cmbRemakeRequired.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.9 - Select installers button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectInstallers_Click(object sender, RoutedEventArgs e)
        {

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

        /// <summary>
        /// v1.0.11 - Display date popup when clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetOrderCompleteDate_Click(object sender, RoutedEventArgs e)
        {

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

        /// <summary>
        /// v1.0.11
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSetOrderCompleteDate_UnChecked(object sender, RoutedEventArgs e)
        {

            try
            {

                this.btnSetOrderCompleteDate.IsEnabled = false;
                this.btnSetOrderCompleteDate.Content = "Not required.";

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.11
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSetOrderCompleteDate_Checked(object sender, RoutedEventArgs e)
        {

            try
            {

                if (this.m_bPageLoaded == false) { return; }

                this.btnSetOrderCompleteDate.IsEnabled = true;
              
                DateTime dInstallDate;
                if (DateTime.TryParse(this.btnSelectInstallDate.Tag.ToString(), out dInstallDate) == false)
                {
                    dInstallDate = DateTime.Now;
                }

                this.SetOrderCompleteDateButton(dInstallDate);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }


        /// <summary>
        /// v1.0.11 - Apply selected order complete date to order complete button and hide calendar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApplyOrderCompleteDate_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                this.SetOrderCompleteDateButton(this.dtPickerOrderComplete.Value);
                this.foOrderCompleteDate.Hide();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }
    }
}
