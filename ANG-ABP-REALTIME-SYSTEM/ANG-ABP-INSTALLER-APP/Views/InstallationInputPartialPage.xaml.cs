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
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using ANG_ABP_SURVEYOR_APP_CLASS.Model;
using ANG_ABP_SURVEYOR_APP_CLASS.Classes;
using ANG_ABP_SURVEYOR_APP_CLASS;
using System.Text;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ANG_ABP_INSTALLER_APP.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class InstallationInputPartialPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// Flag to indicate page has been loaded.
        /// </summary>
        private bool m_bPageLoaded = false;


        /// <summary>
        /// Holds project details.
        /// </summary>
        private cProjectTable m_cProject = null;

        /// <summary>
        /// List of units.
        /// </summary>
        private ObservableCollection<cUnits> m_cUnits = new ObservableCollection<cUnits>();

        /// <summary>
        /// List of units retrieved from database.
        /// </summary>
        private List<cUnitsTable> m_lDBUnits = new List<cUnitsTable>();

        /// <summary>
        /// Count of how many units are installed.
        /// </summary>
        private int m_iUnits_Installed_Qty = 0;

        /// <summary>
        /// Count of how many units are not installed.
        /// </summary>
        private int m_iUnits_NotInstalled_Qty = 0;

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


        public InstallationInputPartialPage()
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
                    if (e.Parameter.GetType() == typeof(string))
                    {
                        string sSubProjectNo = e.Parameter.ToString();
                        this.m_cProject = cMain.p_cDataAccess.GetSubProjectProjectData(sSubProjectNo);

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
        /// Page load event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InstallationInputPartialPage_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {

                this.PopulatePartialReasons(); //v1.0.1 - Populate partial reasons.
                this.PopulateDetails();
                this.PopulateUnitsList();

                this.isSearch.InstallerSelected += isSearch_InstallerSelected;



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
        private void isSearch_InstallerSelected(object sender, cInstallersTable e)
        {

            try
            {

                this.btnSelectInstallers.Tag = e;
                this.btnSelectInstallers.Content = e.Name;
                this.SetInstallersButtonToolTip(e.Name);

                this.foSearch.Hide();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Populate header details.
        /// </summary>
        private void PopulateDetails()
        {

            try
            {

                //Sub project details.
                this.tbSubProjectNo.Text = this.m_cProject.SubProjectNo;
                this.tbStreetName.Text = this.m_cProject.DeliveryStreet;

                //If team used previously then select that one.
                if (this.m_cProject.ABPAXINSTALLATIONTEAM != null)
                {
                    if (this.m_cProject.ABPAXINSTALLATIONTEAM.Length > 0)
                    {
                        this.PreSelectInstallationTeam();
                    }

                }

                //Default installation date to today.
                this.SetInstallDateButton(DateTime.Now);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.1 - Populate partial reasons.
        /// </summary>
        private void PopulatePartialReasons()
        {

            try
            {

                this.cmbPartialReasons.Items.Add(cSettings.p_sPleaseChoose);

                List<cBaseEnumsTable> lReasons = cMain.p_cDataAccess.GetEnumsForField("ABPAXPartInstallReasons_NotUsed");
                foreach (cBaseEnumsTable cReason in lReasons)
                {
                    if (cReason.EnumName.Trim().Length > 0)
                    {
                        this.cmbPartialReasons.Items.Add(cReason.EnumName);
                    }
                    
                }

                this.cmbPartialReasons.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Pre-select installation team.
        /// </summary>
        private void PreSelectInstallationTeam()
        {

            try
            {

                cInstallersTable cInstallTeam = cMain.p_cDataAccess.FetchInstallationTeam(this.m_cProject.ABPAXINSTALLATIONTEAM);
                if (cInstallTeam != null)
                {

                    this.btnSelectInstallers.Tag = cInstallTeam;
                    this.btnSelectInstallers.Content = cInstallTeam.Name;
                    this.SetInstallersButtonToolTip(cInstallTeam.Name);


                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Populate units list.
        /// </summary>
        private void PopulateUnitsList()
        {

            try
            {

                cUnits cUnit = null;

                this.m_lDBUnits = cMain.p_cDataAccess.FetchUnitsForSubProject(this.m_cProject.SubProjectNo);
                if (this.m_lDBUnits != null)
                {

                    foreach (cUnitsTable cDBUnit in this.m_lDBUnits)
                    {

                        cUnit = new cUnits();
                        cUnit.UnitNo = cDBUnit.UnitNo;
                        cUnit.Design = cDBUnit.ItemID + cDBUnit.Style;
                        cUnit.Location = cDBUnit.UnitLocation;
                        if (cDBUnit.InstalledStatus == cSettings.p_iUnits_InstalledStatus)
                        {
                            cUnit.Installed = true;
                        }
                        else
                        {
                            cUnit.Installed = false;
                        }
                    
                        
                        this.m_cUnits.Add(cUnit);

                    }

                }
               
                this.lvUnits.ItemsSource = this.m_cUnits;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Installation Date.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInstallDate_Click(object sender, RoutedEventArgs e)
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
        /// Set Install date button.
        /// </summary>
        /// <param name="v_dDate"></param>
        private void SetInstallDateButton(DateTime v_dDate)
        {

            try
            {

                //Set button properties.
                this.btnInstallDate.Tag = v_dDate;
                this.btnInstallDate.Content = cMain.ReturnDisplayDate(v_dDate, ANG_ABP_SURVEYOR_APP_CLASS.cSettings.p_sDateCompare_EqualTo);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), v_dDate.ToString());

            }

        }

        /// <summary>
        /// 
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
        /// Update project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnUpdateProject_Click(object sender, RoutedEventArgs e)
        {

            cInstallersTable cInstallTeam = null;
            try
            {

                //Fetch installed stats.
                bool bAllUnitsInstalled = this.HaveAllUnitsBeenInstalled();

                //If none installed, let user know.
                if (this.m_iUnits_Installed_Qty == 0)
                {

                    await cSettings.DisplayMessage("You have not marked any units as installed, please amend and try-again.", "No Installed Units.");
                    this.lvUnits.Focus(FocusState.Programmatic);
                    return;

                }

                //Make sure an installation team is set.
                if (this.btnSelectInstallers.Tag == null)
                {

                    await cSettings.DisplayMessage("You need to select an installation team before you can proceed, please amend and try-again.", "Installation Team Missing");
                    this.btnSelectInstallers.Focus(FocusState.Programmatic);
                    return;

                }
                else
                {
                    cInstallTeam = (cInstallersTable)this.btnSelectInstallers.Tag;

                }

                //Make sure an installation date has been set.
                DateTime? dInstallDate = this.ReturnInstallationDate();
                if (dInstallDate.HasValue == false)
                {

                    await cSettings.DisplayMessage("You need to select an installation date before you can proceed, please amend and try-again.", "Installation Date Missing");
                    this.btnInstallDate.Focus(FocusState.Programmatic);
                    return;

                }

                //v1.0.1 - Make sure a reason has been selected.
                if (this.cmbPartialReasons.SelectedIndex == 0)
                {

                    await cSettings.DisplayMessage("You need to select an reason for this partial install, please amend and try-again.", "Partial Install Reason Missing");
                    this.cmbPartialReasons.Focus(FocusState.Programmatic);
                    return;

                }

                

                //v1.0.1 - Only need reason if partial install and "Other" installed as reason.
                //If partial install the user needs to enter a reason
                this.txtComments.Text = this.txtComments.Text.Trim();
                if (bAllUnitsInstalled == false && this.cmbPartialReasons.SelectedItem.ToString() == cSettings.p_sNoteIt_Other)
                {

                    if (this.txtComments.Text.Length == 0)
                    {
                        await cSettings.DisplayMessage("You need to enter a reason description for the partial install, please amend and try-again.", "Reason Description Required.");
                        this.txtComments.Focus(FocusState.Programmatic);
                        return;

                    }

                }
                

                //If all installed 
                if (bAllUnitsInstalled == true)
                {

                    bool bContinue = await this.DoesUserWantToContinueWithFullInstall();
                    if (bContinue == false)
                    {
                        return;
                    }

                }

                //Log installation team.
                this.m_cProject.ABPAXINSTALLATIONTEAM = cInstallTeam.AccountNum;

                //If all units install then install status should be Fully.
                if (bAllUnitsInstalled == true)
                {
                    this.m_cProject.Mxm1002InstallStatus = cSettings.p_iInstallStatus_InstalledFully;

                }
                else
                {
                    this.m_cProject.Mxm1002InstallStatus = cSettings.p_iInstallStatus_InstalledPart;
                    

                    this.m_cProject.MxmConfirmedAppointmentIndicator = (int)cSettings.YesNoBaseEnum.No;
                    cMain.p_cDataAccess.AddToUpdateTable(this.m_cProject.SubProjectNo, "MxmConfirmedAppointmentIndicator", ((int)cSettings.YesNoBaseEnum.No).ToString());

                }

                //Update status name.
                this.m_cProject.InstallStatusName = cMain.p_cDataAccess.GetEnumValueName("PROJTABLE","Mxm1002InstallStatus",this.m_cProject.Mxm1002InstallStatus.Value);

                cMain.p_cDataAccess.AddToUpdateTable(this.m_cProject.SubProjectNo, "Mxm1002InstallStatus", this.m_cProject.Mxm1002InstallStatus.Value.ToString());

                this.m_cProject.EndDateTime = dInstallDate;
                cMain.p_cDataAccess.AddToUpdateTable(this.m_cProject.SubProjectNo, "StartDateTime", dInstallDate.ToString());

                this.m_cProject.StartDateTime = dInstallDate;
                cMain.p_cDataAccess.AddToUpdateTable(this.m_cProject.SubProjectNo, "EndDateTime", dInstallDate.ToString());

                
                //v1.0.1 - Add partial reason to the start of the note text.
                string sNote = this.cmbPartialReasons.SelectedItem.ToString();
                if (this.txtComments.Text.Length > 0)
                {
                    sNote = sNote + ": " + this.txtComments.Text;
                }
                    

                //Create note object for saving.
                cProjectNotesTable cNote = await cSettings.ReturnNoteObject(this.m_cProject.SubProjectNo, sNote, DateTime.Now, cSettings.p_sProjectNoteType_PartialInstall);
                bool bSavedOK = cMain.p_cDataAccess.SaveSubProjectNote(cNote);
                if (bSavedOK == false)
                {

                    await cSettings.DisplayMessage("An issue has occurred when trying to save, please try again.", "Please save again.");
                    return;

                }
           
                //Loop through units and update.
                bool bOK = false;
                foreach (cUnits oUnit in this.m_cUnits)
                {

                    if (oUnit.Installed == true)
                    {

                        foreach (cUnitsTable oUnitTable in this.m_lDBUnits)
                        {

                            if (oUnitTable.UnitNo == oUnit.UnitNo)
                            {

                                oUnitTable.InstalledStatus = cSettings.p_iUnits_InstalledStatus;

                                bOK = cMain.p_cDataAccess.UpdateUnit(oUnitTable);
                                if (bOK == true)
                                {

                                    bOK = cMain.p_cDataAccess.LogUnitUpdate(this.m_cProject.SubProjectNo, oUnitTable.UnitNo, cSettings.p_iUnits_InstalledStatus, dInstallDate.Value);
                                    if (bOK == false)
                                    {
                                        await cSettings.DisplayMessage("An issue has occurred when trying to save, please try again.", "Please save again.");
                                        return;

                                    }                                    

                                }

                                break;

                            }

                        }

                    }

                }

                bOK = cMain.p_cDataAccess.UpdateProjectTable(this.m_cProject);
                if (bOK == true)
                {

                    this.RedirectBack();

                }
                else
                {

                    await cSettings.DisplayMessage("An issue has occurred when trying to save, please try again.", "Please save again.");
                    return;

                }
             
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Redirect back.
        /// </summary>
        private void RedirectBack()
        {

            try
            {

                this.CheckLastPageRemoval();

                this.navigationHelper.GoBack();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// We want to go back to the search screen, not the result page.
        /// </summary>
        private void CheckLastPageRemoval()
        {

            try
            {

                int iPageCount = this.Frame.BackStackDepth;
                if (this.Frame.BackStack[iPageCount - 1].SourcePageType == typeof(InstallationInputResultPage))
                {
                    this.Frame.BackStack.RemoveAt(iPageCount - 1);
                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
            }
        }

        /// <summary>
        /// Return installation date as set by the user.
        /// </summary>
        /// <returns></returns>
        private DateTime? ReturnInstallationDate()
        {

            DateTime? dInstallDate = null;
            try
            {

                if (this.btnInstallDate.Tag != null)
                {

                    DateTime dDate;
                    if (DateTime.TryParse(this.btnInstallDate.Tag.ToString(), out dDate) == true)
                    {
                        dInstallDate = dDate;

                    }

                }

                return dInstallDate;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return dInstallDate;

            }
        }

        /// <summary>
        /// Check if user intended to do a full install.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> DoesUserWantToContinueWithFullInstall()
        {

            StringBuilder sbMsg = new StringBuilder();
            try
            {

                sbMsg.Append("You have marked all units as installed, the sub project will not be marked as a full install.");
                sbMsg.Append(Environment.NewLine);
                sbMsg.Append(Environment.NewLine);
                sbMsg.Append("Are you sure you want to continue?.");

                cSettings.YesNo cResp = await cSettings.DisplayYesNo(sbMsg.ToString(), "Continue with full install.");

                if (cResp == cSettings.YesNo.Yes)
                {
                    return true;
                }

                return false;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }


        }

        /// <summary>
        /// Check if all units have been installed.
        /// </summary>
        /// <returns></returns>
        private bool HaveAllUnitsBeenInstalled()
        {

            this.m_iUnits_Installed_Qty = 0;
            this.m_iUnits_NotInstalled_Qty = 0;
            try
            {

                foreach (cUnits oUnit in this.m_cUnits)
                {

                    if (oUnit.Installed == true)
                    {
                        this.m_iUnits_Installed_Qty++;

                    }
                    else
                    {
                        this.m_iUnits_NotInstalled_Qty++;
                    }

                }

                if (this.m_iUnits_Installed_Qty > 0 && this.m_iUnits_NotInstalled_Qty == 0)
                {
                    return true;
                }

                return false;

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
        private void tsInstalled_Toggled(object sender, RoutedEventArgs e)
        {

            try
            {

                if (e.OriginalSource.GetType() == typeof(ToggleSwitch))
                {

                    ToggleSwitch tsSwitch = (ToggleSwitch)e.OriginalSource;
                    if (tsSwitch.DataContext.GetType() == typeof(cUnits))
                    {
                        cUnits oUnit = (cUnits)tsSwitch.DataContext;

                        foreach (cUnits ocUnit in this.m_cUnits)
                        {

                            if (ocUnit.UnitNo == oUnit.UnitNo)
                            {

                                ocUnit.Installed = tsSwitch.IsOn;
                                break;

                            }
                            
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
        /// v1.0.9 - Select installers
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


    }
}
