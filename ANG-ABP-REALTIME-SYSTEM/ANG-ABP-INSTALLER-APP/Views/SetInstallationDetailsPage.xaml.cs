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
using ANG_ABP_SURVEYOR_APP_CLASS.Model;
using ANG_ABP_SURVEYOR_APP_CLASS.Classes;
using System.Threading.Tasks;
using System.Text;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ANG_ABP_INSTALLER_APP.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SetInstallationDetailsPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// v1.0.2 - Set when user sets a survey date.
        /// </summary>
        private bool m_bUserAppliedInstallDate = false;

        /// <summary>
        /// Holds project details.
        /// </summary>
        private cProjectTable m_cProjectData = null;

        /// <summary>
        /// v1.0.1 - Store the project notes.
        /// </summary>
        private List<cProjectNotesTable> m_cProjectNotes = null;

        /// <summary>
        /// Log of updates made.
        /// </summary>
        private List<cUpdatesTable> m_cUpdateLog = null;

        /// <summary>
        /// Navigation mode, how did we get here.
        /// </summary>
        private NavigationMode m_nmNavMode = NavigationMode.New;

        /// <summary>
        /// Selected date.
        /// </summary>
        private DateTime? m_dSelectedDate = null;
        

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


        public SetInstallationDetailsPage()
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
                    if (e.Parameter.GetType() == typeof(string))
                    {

                        this.m_cProjectData = cMain.p_cDataAccess.GetSubProjectProjectData(e.Parameter.ToString());
                        this.m_cProjectNotes = cMain.p_cDataAccess.GetSubProjectNotesData(e.Parameter.ToString()); //v1.0.1 - Fetch notes.

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

            if (e.NavigationMode == NavigationMode.New)
            {
                this.RecordScreenInput();
            }
        }

        #endregion

        /// <summary>
        /// Record currently input screen data.
        /// </summary>
        private void RecordScreenInput()
        {

            try
            {
                cMain.p_cInstallInputScreenData = new cProjectTable();

                cMain.p_cInstallInputScreenData.ResidentName = this.txtResidentName.Text;
                cMain.p_cInstallInputScreenData.ResidentTelNo = this.txtResidentTelNo.Text;
                cMain.p_cInstallInputScreenData.ResidentMobileNo = this.txtResidentMobileNo.Text;
                cMain.p_cInstallInputScreenData.AlternativeContactName = this.txtAltContactName.Text;
                cMain.p_cInstallInputScreenData.AlternativeContactTelNo = this.txtAltContactTelNo.Text;
                cMain.p_cInstallInputScreenData.AlternativeContactMobileNo = this.txtAltContactMobNo.Text;
                cMain.p_cInstallInputScreenData.MxmProjDescription = this.txtReplacementType.Text;
                cMain.p_cInstallInputScreenData.SpecialResidentNote = this.txtSpecialResidentNote.Text;

                cMain.p_cInstallInputScreenData.PropertyType = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbPropertyType));
                cMain.p_cInstallInputScreenData.ABPAXFloorLevel = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbFloorLevel));
                cMain.p_cInstallInputScreenData.ABPAXInstallationType = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbInstallationType));
                cMain.p_cInstallInputScreenData.ABPAXAsbestosPresumed = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbAsbestosPresumed));
                cMain.p_cInstallInputScreenData.ABPAXAccessEquipment = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbAccessEquipment));
                cMain.p_cInstallInputScreenData.ABPAXPermanentGasVent = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbPermanentGasVent));
                cMain.p_cInstallInputScreenData.ABPAXWindowBoard = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbWindowboard));
                cMain.p_cInstallInputScreenData.ABPAXStructuralFaults = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbStructuralFaults));
                cMain.p_cInstallInputScreenData.ABPAXServicesToMove = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbServicesToMove));
                cMain.p_cInstallInputScreenData.DisabledAdaptionsRequired = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbDisabledAdaptionsRqd));                               

                cMain.p_cInstallInputScreenData.ABPAXInternalDamage = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbInternalDamage));
                cMain.p_cInstallInputScreenData.ABPAXWorkAccessRestrictions = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbWorkAccessRestrictions));
                cMain.p_cInstallInputScreenData.ABPAXPublicProtection = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbPublicProtection));

                cMain.p_cInstallInputProjectNotes = new List<cProjectNotesTable>();
                foreach (cProjectNotesTable cNote in this.m_cProjectNotes)
                {

                    if (cNote.IDKey == -1)
                    {
                        cMain.p_cInstallInputProjectNotes.Add(cNote);

                    }

                }

                if (this.btnInstallationDate.Tag != null)
                {

                    //Update key fields
                    DateTime dSurveyDate = DateTime.MinValue;
                    if (DateTime.TryParse(this.btnInstallationDate.Tag.ToString(), out dSurveyDate) == true)
                    {
                        //Log survey date.
                        cMain.p_cInstallInputScreenData.MXM1002TrfDate = dSurveyDate;
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
        private void btnInstallationDate_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (this.puDatePicker.IsOpen == false)
                {

                    //Default to survey date initially set.
                    DateTime dInstallDate = DateTime.Now;

                    if (this.btnInstallationDate.Tag != null)
                    {
                        //Try and convert button date so we can default date time picker
                        DateTime.TryParse(this.btnInstallationDate.Tag.ToString(), out dInstallDate);

                    }
                    else if (this.m_cProjectData.MXM1002TrfDate.HasValue == true)
                    {
                        dInstallDate = this.m_cProjectData.MXM1002TrfDate.Value;

                    }
                    else if (this.m_cProjectData.EndDateTime.HasValue == true)
                    {
                        dInstallDate = this.m_cProjectData.EndDateTime.Value;

                    }


                    this.dtPicker.Value = dInstallDate;

                    //Open date time picker.
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
        /// Update installation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnUpdateInstallation_Click(object sender, RoutedEventArgs e)
        {

             bool bSaveOK = false;
            try
            {


                bSaveOK = await this.SaveProject();


                if (bSaveOK == true)
                {
                    this.RedirectBack();
                }
                
                                      
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.2 - Redirect back.
        /// </summary>
        private void RedirectBack()
        {

            try
            {
               

                this.navigationHelper.GoBack();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.2 - Save project
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SaveProject()
        {


            bool bValidationOK = false;
            bool bSaveOK = true;
            try
            {

                bValidationOK = await this.IsSaveValidationOK();
                if (bValidationOK == false)
                {
                    bSaveOK = false;

                }
                else
                {

                    bool bLoggedOK = this.LogChangesForSaving(false);
                    if (bLoggedOK == false)
                    {
                        bSaveOK = false;

                    }
                    else
                    {

                        if (this.m_cUpdateLog.Count > 0)
                        {

                            //Save details away.
                            bSaveOK = cMain.p_cDataAccess.UpdateProjectTable(this.m_cProjectData);
                            if (bSaveOK == true)
                            {

                                //Save log changes for syncing.
                                bSaveOK = cMain.p_cDataAccess.AddUpdatesToUpdateTable(this.m_cUpdateLog);
                                if (bSaveOK == false)
                                {
                                    bSaveOK = false;

                                }

                            }

                        }

                        //v1.0.1 - Save project notes.
                        bSaveOK = this.SaveProjectNotes();
                        if (bSaveOK == false)
                        {
                            bSaveOK = false;
                        }

                    }


                    if (bSaveOK == false)
                    {
                        await cSettings.DisplayMessage("An error has occurred when trying to save your changes, please try again.", "Error Saving");

                    }

                }

                return bSaveOK;
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }
        }

        /// <summary>
        /// Logs all the changes for saving
        /// </summary>
        /// <returns></returns>
        private bool LogChangesForSaving(bool v_bCheckingForChangesOnly)
        {

            try
            {


                //Create new log table.
                this.m_cUpdateLog = new List<cUpdatesTable>();

                //Check resident name has changed.
                if (this.txtResidentName.Text != this.m_cProjectData.ResidentName)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.ResidentName = this.txtResidentName.Text; }
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmResidentName", this.txtResidentName.Text);
                }

                //Check resident telephone number has changed.
                if (this.txtResidentTelNo.Text != this.m_cProjectData.ResidentTelNo)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.ResidentTelNo = this.txtResidentTelNo.Text; }
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmTelephoneNo", this.txtResidentTelNo.Text);
                }

                //Check resident mobile number has changed.
                if (this.txtResidentMobileNo.Text != this.m_cProjectData.ResidentMobileNo)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.ResidentMobileNo = this.txtResidentMobileNo.Text; }
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmResidentMobileNo", this.txtResidentMobileNo.Text);
                }

                //Check resident alternate contact name has changed.
                if (this.txtAltContactName.Text != this.m_cProjectData.AlternativeContactName)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.AlternativeContactName = this.txtAltContactName.Text; }
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmAlternativeContactName", this.txtAltContactName.Text);
                }

                //Check resident alternate contact telephone number has changed.
                if (this.txtAltContactTelNo.Text != this.m_cProjectData.AlternativeContactTelNo)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.AlternativeContactTelNo = this.txtAltContactTelNo.Text; }
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmAlternativeContactTelNo", this.txtAltContactTelNo.Text);
                }

                //Check resident alternate contact mobile number has changed.
                if (this.txtAltContactMobNo.Text != this.m_cProjectData.AlternativeContactMobileNo)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.AlternativeContactMobileNo = this.txtAltContactMobNo.Text; }
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmAlternativeContactMobileNo", this.txtAltContactMobNo.Text);
                }

                //Check replacement type text has changed.
                if (this.txtReplacementType.Text != this.m_cProjectData.MxmProjDescription)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.MxmProjDescription = this.txtReplacementType.Text; }
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmProjDescription", this.txtReplacementType.Text);
                }

                //Special Resident note.
                if (this.txtSpecialResidentNote.Text != this.m_cProjectData.SpecialResidentNote)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.SpecialResidentNote = this.txtSpecialResidentNote.Text; }
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmSpecialResidentNote", this.txtSpecialResidentNote.Text);
                }

                //Log combo box changes.

                int? iValue = this.LogComboValueChange(this.cmbPropertyType.Name, "MxmPropertyType", this.m_cProjectData.PropertyType);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.PropertyType = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbFloorLevel.Name, "ABPAXFloorLevel", this.m_cProjectData.ABPAXFloorLevel);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXFloorLevel = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbInstallationType.Name, "ABPAXInstallationType", this.m_cProjectData.ABPAXInstallationType);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXInstallationType = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbAsbestosPresumed.Name, "ABPAXAsbestosPresumed", this.m_cProjectData.ABPAXAsbestosPresumed);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXAsbestosPresumed = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbAccessEquipment.Name, "ABPAXAccessEquipment", this.m_cProjectData.ABPAXAccessEquipment);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXAccessEquipment = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbPermanentGasVent.Name, "ABPAXPermanentGasVent", this.m_cProjectData.ABPAXPermanentGasVent);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXPermanentGasVent = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbWindowboard.Name, "ABPAXWindowBoard", this.m_cProjectData.ABPAXWindowBoard);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXWindowBoard = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbStructuralFaults.Name, "ABPAXStructuralFaults", this.m_cProjectData.ABPAXStructuralFaults);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXStructuralFaults = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbServicesToMove.Name, "ABPAXServicesToMove", this.m_cProjectData.ABPAXServicesToMove);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXServicesToMove = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbDisabledAdaptionsRqd.Name, "MxmDisabledAdaptionsRequired", this.m_cProjectData.DisabledAdaptionsRequired);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.DisabledAdaptionsRequired = iValue;
                }               

                iValue = this.LogComboValueChange(this.cmbInternalDamage.Name, "ABPAXInternDamage", this.m_cProjectData.ABPAXInternalDamage);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXInternalDamage = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbWorkAccessRestrictions.Name, "ABPAXWrkAccRestrictions", this.m_cProjectData.ABPAXWorkAccessRestrictions);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXWorkAccessRestrictions = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbPublicProtection.Name, "ABPAXPublicProtect", this.m_cProjectData.ABPAXPublicProtection);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXPublicProtection = iValue;
                }



                //v1.0.2 - Decide if we need to check survey input date.
                bool bCheckInstallInputDate = true;
                if (v_bCheckingForChangesOnly == true)
                {
                    if (this.m_bUserAppliedInstallDate == false)
                    {
                        bCheckInstallInputDate = false;

                    }

                }

                //v1.0.2 - Only check survey date if flagged
                if (bCheckInstallInputDate == true)
                {


                    //v1.0.4 - Extract dates and compare.
                    DateTime dEndDateTime = DateTime.MinValue;
                    DateTime dSelectedDate = DateTime.MinValue;

                    if (this.m_cProjectData.EndDateTime.HasValue == true)
                    {
                        dEndDateTime = this.m_cProjectData.EndDateTime.Value;
                    }

                    if (this.m_dSelectedDate.HasValue == true)
                    {
                        dSelectedDate = this.m_dSelectedDate.Value;
                    }


                    if (dEndDateTime != dSelectedDate)
                    {
                                             
                        cSettings.SurveyDatesApply cApply;
                        cApply = cSettings.ApplySurveyDates(this.m_cProjectData, this.m_cUpdateLog, dSelectedDate, this.m_cProjectData.SubProjectNo, string.Empty, string.Empty, string.Empty);

                        this.m_cUpdateLog = cApply.cUpdates;

                        //If changes only being recorded do not update main project data class.
                        if (v_bCheckingForChangesOnly == false)
                        {
                            this.m_cProjectData = cApply.cProjectData;
                        }

                    }
                    

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
        /// Log combo box value change.
        /// </summary>
        /// <param name="v_sControlName"></param>
        /// <param name="v_sFieldName"></param>
        /// <param name="v_iOldValue"></param>
        /// <returns></returns>
        private int? LogComboValueChange(string v_sControlName, string v_sFieldName, int? v_iOldValue)
        {

            int? iRtnValue = v_iOldValue;
            int? iNewValue = null;
            string sValue = string.Empty;

            try
            {
                ComboBox cmbCombo = (ComboBox)this.FindName(v_sControlName);
                if (cmbCombo != null)
                {

                    sValue = cMain.ReturnComboSelectedTagValue(cmbCombo);
                    iNewValue = Convert.ToInt32(sValue);

                    if (cmbCombo.SelectedIndex != Convert.ToInt32(cmbCombo.Tag))
                    {

                        if (iNewValue != v_iOldValue)
                        {
                            iRtnValue = iNewValue;
                            this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, v_sFieldName, iNewValue.ToString());

                        }

                    }


                }

                return iRtnValue;

            }
            catch (Exception ex)
            {
                throw new Exception("Field:" + v_sFieldName + ",Control:" + v_sControlName + ",Error:" + ex.Message);

            }

        }

        /// <summary>
        /// Check if installation input passes save validation.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsSaveValidationOK()
        {

            bool bComboOK = false;
            try
            {


                bComboOK = await this.IsComboValueSelected(this.cmbPropertyType.Name, "Property Type");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbFloorLevel.Name, "Floor Level");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbInstallationType.Name, "Installation Type");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbAsbestosPresumed.Name, "Asbestos Presumed");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbAccessEquipment.Name, "Access Equipment");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbPermanentGasVent.Name, "Permanent Gas Vent");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbWindowboard.Name, "Window Board");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbStructuralFaults.Name, "Structural Faults");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbServicesToMove.Name, "Services to move");
                if (bComboOK == false) { return false; }
               
                bComboOK = await this.IsComboValueSelected(this.cmbDisabledAdaptionsRqd.Name, "Disabled adaption's");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbInternalDamage.Name, "Internal damage");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbWorkAccessRestrictions.Name, "Work access restrictions");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbPublicProtection.Name, "Public protection");
                if (bComboOK == false) { return false; }

                //v1.0.4 - Check if other is selected, if so make sure notes is attached.
                if (this.HasOtherBeenSelected() == true)
                {
                    if (this.m_cProjectNotes.Count() == 0)
                    {

                        await cSettings.DisplayMessage("You have selected Other on one of the drop downs, you need to add a note.", "Note Required");
                        return false;

                    }

                }

                bool bSurveyDateOK = await this.IsSurveyDateSet();
                if (bSurveyDateOK == false) { return false; }

                return true;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }

        }

        /// <summary>
        /// v1.0.4 - Has OTHER been selected on any of the drop downs.
        /// </summary>
        /// <returns></returns>
        private bool HasOtherBeenSelected()
        {

            try
            {
                bool bOtherSelected = false;

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbPropertyType.Name);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbFloorLevel.Name);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbInstallationType.Name);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbAsbestosPresumed.Name);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbAccessEquipment.Name);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbPermanentGasVent.Name);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbWindowboard.Name);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbStructuralFaults.Name);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbServicesToMove.Name);
                if (bOtherSelected == true) { return true; }               

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbDisabledAdaptionsRqd.Name);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbInternalDamage.Name);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbWorkAccessRestrictions.Name);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbPublicProtection.Name);
                if (bOtherSelected == true) { return true; }

                return false;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }

        }

        /// <summary>
        /// v1.0.4 - Check if OTHER has been selected on combo
        /// </summary>
        /// <param name="v_sComboBoxControlID"></param>
        /// <returns></returns>
        private bool HasOtherBeenSelectedOnCombo(string v_sComboBoxControlID)
        {

            try
            {

                ComboBox cmbItem = (ComboBox)this.FindName(v_sComboBoxControlID);
                if (cmbItem != null)
                {

                    string sValue = cMain.ReturnComboSelectedItemText(cmbItem);
                    if (sValue.Equals("Other", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        return true;
                    }


                }

                return false;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), v_sComboBoxControlID);
                return false;

            }

        }

        /// <summary>
        /// Is survey date set.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsSurveyDateSet()
        {

            try
            {

                if (this.btnInstallationDate.Tag != null)
                {
                    DateTime dSurveyDate = DateTime.Now;
                    if (DateTime.TryParse(this.btnInstallationDate.Tag.ToString(), out dSurveyDate) == true)
                    {
                        return true;
                    }

                }

                await cSettings.DisplayMessage("You have not set a installation date for this sub project, please amend and try-again.", "Installation Date Required.");

                this.puDatePicker.IsOpen = true;
                this.dtPicker.Focus(FocusState.Programmatic);

                return false;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }
        }

        /// <summary>
        /// Is combo value selected
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsComboValueSelected(string v_sControlName, string v_sSelectionType)
        {

            string sValue = string.Empty;
            string sMsg = string.Empty;
            try
            {

                //Try and locate the combo box on screen.
                ComboBox cmbCombo = (ComboBox)this.FindName(v_sControlName);
                if (cmbCombo != null)
                {

                    //Check floor level is selected.
                    sValue = cMain.ReturnComboSelectedTagValue(cmbCombo);
                    if (Convert.ToInt32(sValue) == -1)
                    {
                        sMsg = "You must select a " + v_sSelectionType + " option before you can update the sub projects survey details.";
                        await cSettings.DisplayMessage(sMsg, "Selection Required.");
                        cmbCombo.IsDropDownOpen = true;
                        cmbCombo.Focus(FocusState.Programmatic);
                        return false;

                    }

                    return true;
                }
                else
                {
                    throw new Exception("Control " + v_sControlName + " is not a combobox.");
                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }


        }

        /// <summary>
        /// v1.0.1 - Save project notes.
        /// </summary>
        /// <returns></returns>
        private bool SaveProjectNotes()
        {

            bool bSaveOK = false;
            try
            {

                foreach (cProjectNotesTable cNote in this.m_cProjectNotes)
                {

                    //if (cNote.AXRecID == -1 && cNote.IDKey != 10)
                    if (cNote.IDKey == -1)
                    {

                        bSaveOK = cMain.p_cDataAccess.SaveSubProjectNote(cNote);
                        if (bSaveOK == false)
                        {
                            return false;
                        }
                        else
                        {

                            cNote.IDKey = 10;
                        }

                    }

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
        /// Page loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetInstallationDetailsPage_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {

                this.PopulateDropDowns();

                if (this.m_nmNavMode == NavigationMode.New)
                {
                    this.DisplayProjectDetails(this.m_cProjectData);
                }
                else if (this.m_nmNavMode == NavigationMode.Back)
                {

                    this.RestoreNotes();
                    this.DisplayProjectDetails(cMain.p_cInstallInputScreenData);
                }                

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Display project details on screen.
        /// </summary>
        private void DisplayProjectDetails(cProjectTable v_cProjectData)
        {

            try
            {

                //Sub project details.
                this.tbSubProjectID.Text = this.m_cProjectData.SubProjectNo;
                this.tbStreetAddress.Text = this.m_cProjectData.DeliveryStreet;

                //Resident details
                this.txtResidentName.Text = cSettings.ReturnString(v_cProjectData.ResidentName);
                this.txtResidentTelNo.Text = cSettings.ReturnString(v_cProjectData.ResidentTelNo);
                this.txtReplacementType.Text = cSettings.ReturnString(v_cProjectData.MxmProjDescription);
                this.txtResidentMobileNo.Text = cSettings.ReturnString(v_cProjectData.ResidentMobileNo);
                this.txtAltContactTelNo.Text = cSettings.ReturnString(v_cProjectData.AlternativeContactTelNo);
                this.txtAltContactName.Text = cSettings.ReturnString(v_cProjectData.AlternativeContactName);
                this.txtAltContactMobNo.Text = cSettings.ReturnString(v_cProjectData.AlternativeContactMobileNo);
                this.UpdateSpecialNotes(v_cProjectData);

                //Drop down details
                this.SelectDropDown(this.cmbPropertyType.Name, v_cProjectData.PropertyType, this.m_cProjectData.PropertyType);
                this.SelectDropDown(this.cmbFloorLevel.Name, v_cProjectData.ABPAXFloorLevel, this.m_cProjectData.ABPAXFloorLevel);
                this.SelectDropDown(this.cmbInstallationType.Name, v_cProjectData.ABPAXInstallationType, this.m_cProjectData.ABPAXInstallationType);
                this.SelectDropDown(this.cmbAsbestosPresumed.Name, v_cProjectData.ABPAXAsbestosPresumed, this.m_cProjectData.ABPAXAsbestosPresumed);
                this.SelectDropDown(this.cmbAccessEquipment.Name, v_cProjectData.ABPAXAccessEquipment, this.m_cProjectData.ABPAXAccessEquipment);
                this.SelectDropDown(this.cmbPermanentGasVent.Name, v_cProjectData.ABPAXPermanentGasVent, this.m_cProjectData.ABPAXPermanentGasVent);
                this.SelectDropDown(this.cmbWindowboard.Name, v_cProjectData.ABPAXWindowBoard, this.m_cProjectData.ABPAXWindowBoard);
                this.SelectDropDown(this.cmbStructuralFaults.Name, v_cProjectData.ABPAXStructuralFaults, this.m_cProjectData.ABPAXStructuralFaults);
                this.SelectDropDown(this.cmbServicesToMove.Name, v_cProjectData.ABPAXServicesToMove, this.m_cProjectData.ABPAXServicesToMove);
                this.SelectDropDown(this.cmbDisabledAdaptionsRqd.Name, v_cProjectData.DisabledAdaptionsRequired, this.m_cProjectData.DisabledAdaptionsRequired);

                this.tbDoorChoiceForm.Text = cMain.p_cDataAccess.GetEnumValueName("PROJTABLE","MxmDoorChoiceFormReceived",this.m_cProjectData.MxmDoorChoiceFormReceived.Value);

                this.SelectDropDown(this.cmbInternalDamage.Name, v_cProjectData.ABPAXInternalDamage, this.m_cProjectData.ABPAXInternalDamage);
                this.SelectDropDown(this.cmbWorkAccessRestrictions.Name, v_cProjectData.ABPAXWorkAccessRestrictions, this.m_cProjectData.ABPAXWorkAccessRestrictions);
                this.SelectDropDown(this.cmbPublicProtection.Name, v_cProjectData.ABPAXPublicProtection, this.m_cProjectData.ABPAXPublicProtection);
                           
                this.DisplayInstallDateOnSurveyButton(v_cProjectData.EndDateTime);
               
                this.DisplayUnitsInstalledCount();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Display number of units installed
        /// </summary>
        private void DisplayUnitsInstalledCount()
        {

            try
            {


                List<cUnitsTable> cUnits = cMain.p_cDataAccess.FetchUnitsForSubProject(this.m_cProjectData.SubProjectNo);

                int iInstalled = 0;

                foreach (cUnitsTable cUnit in cUnits)
                {

                    if (cUnit.InstalledStatus == cSettings.p_iUnits_InstalledStatus)
                    {
                        iInstalled++;
                    }


                }

                this.tbUnitsInstalled.Text = iInstalled.ToString() + " of " + cUnits.Count.ToString() + " units installed.";


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Display Install date on install date button.
        /// </summary>
        /// <param name="v_dSurveyDate"></param>
        private void DisplayInstallDateOnSurveyButton(DateTime? v_dInstallDate)
        {

            try
            {

                this.m_dSelectedDate = v_dInstallDate;

                if (v_dInstallDate.HasValue == true)
                {                   
                    this.btnInstallationDate.Content = "Installed date: " + cMain.ReturnDisplayDate(v_dInstallDate.Value);
                }
                else
                {
                    this.btnInstallationDate.Content = "Installed date: N\\A";
                }

                this.btnInstallationDate.Tag = v_dInstallDate;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Update special note.
        /// </summary>
        /// <param name="v_sNoteText"></param>
        private void UpdateSpecialNotes(cProjectTable v_sProjectData)
        {

            try
            {

                if (String.IsNullOrEmpty(v_sProjectData.SpecialResidentNote) == true)
                {
                    this.txtSpecialResidentNote.Text = String.Empty;
                    this.txtSpecialNoteFlyout.Text = String.Empty;
                }
                else
                {
                    this.txtSpecialResidentNote.Text = v_sProjectData.SpecialResidentNote;
                    this.txtSpecialNoteFlyout.Text = v_sProjectData.SpecialResidentNote;

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Update drop down with value.
        /// </summary>
        /// <param name="v_sControlID"></param>
        /// <param name="v_sValue"></param>
        private void SelectDropDown(string v_sControlName, int? v_iValue, int? v_iMainValue)
        {

            try
            {

                //If null we go no further.
                if (v_iValue.HasValue == false)
                {
                    return;
                }

                //Try and locate the combo box on screen.
                ComboBox cmbCombo = (ComboBox)this.FindName(v_sControlName);
                if (cmbCombo != null)
                {


                    int iIndex = 0;

                    //Loop through combo box items and find one matching passed values.
                    foreach (ComboBoxItem cbItem in cmbCombo.Items)
                    {

                        //v1.0.2 - This is the original value, not a restore value.
                        if (Convert.ToInt32(cbItem.Tag) == v_iMainValue)
                        {

                            //v1.0.2 - Update initial index                            
                            cmbCombo.Tag = iIndex;

                        }


                        //If match then select index.
                        if (Convert.ToInt32(cbItem.Tag) == v_iValue)
                        {

                            cmbCombo.SelectedIndex = iIndex;

                        }

                        //Count through items.
                        iIndex++;

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

                this.UpdateDropDown(this.cmbPropertyType.Name, "MxmPropertyType");
                this.UpdateDropDown(this.cmbFloorLevel.Name, "ABPAXFloorLevel");
                this.UpdateDropDown(this.cmbInstallationType.Name, "ABPAXInstallationType");
                this.UpdateDropDown(this.cmbAsbestosPresumed.Name, "ABPAXAsbestosPresumed");
                this.UpdateDropDown(this.cmbAccessEquipment.Name, "ABPAXAccessEquipment");
                this.UpdateDropDown(this.cmbPermanentGasVent.Name, "ABPAXPermanentGasVent");
                this.UpdateDropDown(this.cmbWindowboard.Name, "ABPAXWindowBoard");
                this.UpdateDropDown(this.cmbStructuralFaults.Name, "ABPAXStructuralFaults");
                this.UpdateDropDown(this.cmbServicesToMove.Name, "ABPAXServicesToMove");
                
                this.UpdateDropDown(this.cmbDisabledAdaptionsRqd.Name, "MxmDisabledAdaptionsRequired");

                this.UpdateDropDown(this.cmbInternalDamage.Name, "ABPAXInternDamage");
                this.UpdateDropDown(this.cmbWorkAccessRestrictions.Name, "ABPAXWrkAccRestrictions");
                this.UpdateDropDown(this.cmbPublicProtection.Name, "ABPAXPublicProtect");

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// v1.0.1 - Restore notes recorded when navigating to photos page.
        /// </summary>
        private void RestoreNotes()
        {

            try
            {

                if (cMain.p_cInstallInputProjectNotes != null)
                {

                    foreach (cProjectNotesTable cNote in cMain.p_cInstallInputProjectNotes)
                    {
                        this.m_cProjectNotes.Add(cNote);

                    }
                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Update drop down list.
        /// </summary>
        /// <param name="v_sControlName"></param>
        /// <param name="v_sFieldName"></param>
        private void UpdateDropDown(string v_sControlName, string v_sFieldName)
        {

            try
            {

                //Try and locate the combo box on screen.
                ComboBox cmbCombo = (ComboBox)this.FindName(v_sControlName);
                if (cmbCombo != null)
                {


                    //Add please choose to the list.
                    ComboBoxItem cmbItem = new ComboBoxItem();
                    cmbItem.Content = cSettings.p_sPleaseChoose;
                    cmbItem.Tag = -1;

                    cmbCombo.Items.Add(cmbItem);

                    //Fetch enumerators and populate.
                    List<cBaseEnumsTable> oEnums = cMain.p_cDataAccess.GetEnumsForField(v_sFieldName);
                    foreach (cBaseEnumsTable cEnum in oEnums)
                    {
                        if (cEnum.EnumName.Length > 0)
                        {

                            cmbItem = new ComboBoxItem();
                            cmbItem.Content = cEnum.EnumName;
                            cmbItem.Tag = cEnum.EnumValue;

                            cmbCombo.Items.Add(cmbItem);
                        }

                    }

                    //Select first item in list.
                    cmbCombo.SelectedIndex = 0;
                    cmbCombo.Tag = 0; //v1.0.2 - Set combo tag to initial selected index.

                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


        }

        /// <summary>
        /// Refresh notes list.
        /// </summary>
        private void RefreshNotesList()
        {

            try
            {

                //Create new instance, we need to bind to list view.
                List<cNotesHistory> cNotes = new List<cNotesHistory>();
                cNotesHistory cNote = null;

                //v1.0.1 - Order by input date time.
                var oResult = (from oCols in this.m_cProjectNotes
                               orderby oCols.InputDateTime descending
                               select oCols);


                foreach (cProjectNotesTable cProjNote in oResult)
                {
                    cNote = new cNotesHistory();
                    cNote.InputDateTime = cProjNote.InputDateTime;
                    cNote.NoteText = cProjNote.NoteText;
                    cNote.UserName = cProjNote.UserName;

                    if (this.lvNotes.ActualWidth > 0)
                    {
                        cNote.ListViewWidth = this.lvNotes.ActualWidth;
                    }
                    else
                    {
                        cNote.ListViewWidth = 800;
                    }


                    cNotes.Add(cNote);

                }

                //v1.0.1 - Update notes.
                this.lvNotes.ItemsSource = null;
                this.lvNotes.ItemsSource = cNotes;
                this.lvNotes.UpdateLayout();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Add new note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnAddNewNote_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                this.txtNewNote.Text = this.txtNewNote.Text.Trim();
                if (this.txtNewNote.Text.Length > 0)
                {
                    string sNoteText = this.txtNewNote.Text;

                    //v1.0.1 - Add notes the notes collection
                    cProjectNotesTable cNote = await cSettings.ReturnNoteObject(this.m_cProjectData.SubProjectNo, sNoteText, DateTime.Now, cSettings.p_sProjectNoteType_General);
                    this.m_cProjectNotes.Add(cNote);

                    this.txtNewNote.Text = String.Empty;
                    this.txtNewNote.Focus(FocusState.Programmatic);

                    this.RefreshNotesList();
                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Add view notes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddViewNotes_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                this.RefreshNotesList();

                try
                {

                    //foNotes.
                    FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);

                }
                catch (Exception ex)
                {
                    //Not sure why, but still works as expected.

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Add view photos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddViewPhotos_Click(object sender, RoutedEventArgs e)
        {

            try
            {


                //Redirect to edit page.
                this.Frame.Navigate(typeof(SetInstallationPhotosPage), this.m_cProjectData.SubProjectNo);


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


        }

        /// <summary>
        /// Use Install Date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnUseInstallDate_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //v1.0.2 - Indicate user has applied survey date.
                this.m_bUserAppliedInstallDate = true;

                //Set install date button.
                this.DisplayInstallDateOnSurveyButton(this.dtPicker.Value);

                //Hide popup.
                this.puDatePicker.IsOpen = false;

                int? iConfirmed = 1;

                //Warn user if installation date is greater then 
                if (this.m_cProjectData.Delivery_EndDateTime.HasValue == true && this.m_cProjectData.Delivery_EndDateTime.Value.Year > 1900)
                {

                    //v1.0.3 - Change comparison to check if installation date is before delivery date.
                    if (Convert.ToInt32(this.dtPicker.Value.ToString("yyyyMMdd")) <  Convert.ToInt32(this.m_cProjectData.Delivery_EndDateTime.Value.ToString("yyyyMMdd")))
                    {

                        StringBuilder sbMessage = new StringBuilder();
                        sbMessage.Append("The installation date you have set is before the planned delivery date.");
                        sbMessage.Append(Environment.NewLine);
                        sbMessage.Append("Delivery Date: " + cMain.ReturnDisplayDate(this.m_cProjectData.Delivery_EndDateTime.Value));

                        await cSettings.DisplayMessage(sbMessage.ToString(), "Warning");
                    }

                }

                //Let user know if sub project will not display on the front screen.
                bool bWillItDisplay = cMain.WillSubProjectDisplayOnFrontScreen(iConfirmed, this.m_cProjectData.Mxm1002InstallStatus.Value);
                if (bWillItDisplay == false)
                {

                    List<string> lsSubProjectNos = new List<string>();
                    lsSubProjectNos.Add(this.m_cProjectData.SubProjectNo);

                    string sMessage = cMain.ReturnSubProjectWillNotDisplayOnFrontScreenMessage(lsSubProjectNos);
                    await cSettings.DisplayMessage(sMessage, "Warning");

                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Special note has focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSpecialResidentNote_GotFocus(object sender, RoutedEventArgs e)
        {

            try
            {

                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);

            }
            catch (Exception ex)
            {
                //No need to report these errors, bogus.

            }
        }

        /// <summary>
        /// Lost focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSpecialResidentNote_LostFocus(object sender, RoutedEventArgs e)
        {

            try
            {



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
        private void txtSpecialNoteFlyout_GotFocus(object sender, RoutedEventArgs e)
        {

            try
            {

                this.txtSpecialNoteFlyout.Text = this.txtSpecialResidentNote.Text;
                this.txtSpecialResidentNote.IsEnabled = false;

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
        private void txtSpecialNoteFlyout_LostFocus(object sender, RoutedEventArgs e)
        {

            try
            {


                this.txtSpecialResidentNote.IsEnabled = true;


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
        private void txtSpecialNoteFlyout_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {

                this.txtSpecialResidentNote.Text = this.txtSpecialNoteFlyout.Text;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Back button clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void backButton_Click(object sender, RoutedEventArgs e)
        {

             bool bGoBack = true;
            try
            {

                bool bAnyNewNotes = this.AnyUnSavedNotes();
                this.LogChangesForSaving(true);

                if (this.m_cUpdateLog.Count > 0 || bAnyNewNotes == true)
                {

                    cSettings.YesNoCancel mResponse = await cSettings.PromptForUnSavedChanges();
                    if (mResponse == cSettings.YesNoCancel.Yes)
                    {
                        bool bSaveOK = await this.SaveProject();
                        if (bSaveOK == false)
                        {
                            bGoBack = false;
                        }


                    }
                    else if (mResponse == cSettings.YesNoCancel.No)
                    {


                    }
                    else if (mResponse == cSettings.YesNoCancel.Cancel)
                    {
                        bGoBack = false;

                    }

                }

                if (bGoBack == true)
                {
                    this.RedirectBack();
                }
              
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.2 - Check if there are any un-saved notes.
        /// </summary>
        /// <returns></returns>
        private bool AnyUnSavedNotes()
        {
            try
            {


                foreach (cProjectNotesTable cNote in this.m_cProjectNotes)
                {

                    if (cNote.IDKey == -1)
                    {

                        return true;

                    }

                }

                return false;


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;
            }
        }

    }
}
