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
using ANG_ABP_SURVEYOR_APP_CLASS.Model;
using System.Threading.Tasks;
using ANG_ABP_SURVEYOR_APP_CLASS;
using System.Text;



// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ANG_ABP_SURVEYOR_APP.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SurveyInputEditPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// Flag to indicate page has been loaded.
        /// </summary>
        private bool m_bPageLoaded = false;

        /// <summary>
        /// v1.0.2 - Set when user sets a survey date.
        /// </summary>
        private bool m_bUserAppliedSurveyDate = false;

        ///// <summary>
        ///// v1.0.2 - Combo box tag info
        ///// </summary>
        //private struct ComboTagInfo
        //{

        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    public int EnumValue;

        //    /// <summary>
        //    /// Initial selected index position.
        //    /// </summary>
        //    public int InitialIndex;          

        //}

        ///// <summary>
        ///// Stores the survey object passed through on the redirect.
        ///// </summary>
        //private cSurveyInputResult m_cSurvey = null;

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


        public SurveyInputEditPage()
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
        /// Page loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SurveyInputEditPage_Loaded(object sender, RoutedEventArgs e)
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
                    this.DisplayProjectDetails(cMain.p_cSurveyInputScreenData);
                }

                this.m_bPageLoaded = true;                

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
                if (cMain.p_cSurveyInputProjectNotes != null)
                {

                    foreach (cProjectNotesTable cNote in cMain.p_cSurveyInputProjectNotes)
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
        /// Display project details on screen.
        /// </summary>
        private void DisplayProjectDetails(cProjectTable v_cProjectData)
        {

            try
            {

                //Sub project details.
                this.tbSubProjectID.Text = this.m_cProjectData.SubProjectNo;
                this.tbAddress.Text = cMain.ReturnAddress(this.m_cProjectData);

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
                this.SelectDropDown(this.cmbDoorChoiceFormRcvd.Name, v_cProjectData.MxmDoorChoiceFormReceived, this.m_cProjectData.MxmDoorChoiceFormReceived);

                this.SelectDropDown(this.cmbInternalDamage.Name, v_cProjectData.ABPAXInternalDamage, this.m_cProjectData.ABPAXInternalDamage);
                this.SelectDropDown(this.cmbWorkAccessRestrictions.Name, v_cProjectData.ABPAXWorkAccessRestrictions, this.m_cProjectData.ABPAXWorkAccessRestrictions);
                this.SelectDropDown(this.cmbPublicProtection.Name, v_cProjectData.ABPAXPublicProtection, this.m_cProjectData.ABPAXPublicProtection);
                             


                //Display correct survey input date.
                if (v_cProjectData.MXM1002TrfDate.HasValue == true)
                {
                    this.DisplaySurveyDateOnSurveyButton(v_cProjectData.MXM1002TrfDate);
                }
                else
                {
                    this.DisplaySurveyDateOnSurveyButton(v_cProjectData.EndDateTime);
                }

                //v1.0.21 - If incomplete, do not allow
                if (v_cProjectData.ABPAWHealthSafetyInComplete.HasValue == true)
                {
                    if (v_cProjectData.ABPAWHealthSafetyInComplete.Value == (int)cSettings.YesNoBaseEnum.Yes)
                    {

                        this.btnResidentSignature.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        this.btnSurveyedDate.IsEnabled = false;


                    }

                    //Update page title.
                    this.pageTitle.Text = "Surveyed Input Detail";

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
        private void SelectDropDown(string v_sControlName, int? v_iValue,int? v_iMainValue)
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
                this.UpdateDropDown(this.cmbDoorChoiceFormRcvd.Name, "MxmDoorChoiceFormReceived");
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
        /// Special note lost focus event.
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
        /// Special note got focus
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
        /// Special fly out text box text changed event.
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
        /// Add view notes.
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
        /// Add new note.
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
        /// Redirect to Add \ View photo page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddViewPhotos_Click(object sender, RoutedEventArgs e)
        {

            try
            {


                //Redirect to edit page.
                this.Frame.Navigate(typeof(SurveyInputPhotoPage), this.m_cProjectData.SubProjectNo);


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Save survey input changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnUpdateSurvey_Click(object sender, RoutedEventArgs e)
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

                this.CheckLastPageRemoval();

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
              
                    bool bLoggedOK = await this.LogChangesForSaving(false);
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
        /// Check last page removal.
        /// </summary>
        private void CheckLastPageRemoval()
        {

            try
            {

                int iPageCount = this.Frame.BackStackDepth;
                if (this.Frame.BackStack[iPageCount - 1].SourcePageType == typeof(SurveyInputOptionPage))
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
        /// Record currently input screen data.
        /// </summary>
        private void RecordScreenInput()
        {

            try
            {
                cMain.p_cSurveyInputScreenData = new cProjectTable();
                
                cMain.p_cSurveyInputScreenData.ResidentName = this.txtResidentName.Text;
                cMain.p_cSurveyInputScreenData.ResidentTelNo = this.txtResidentTelNo.Text;
                cMain.p_cSurveyInputScreenData.ResidentMobileNo = this.txtResidentMobileNo.Text;
                cMain.p_cSurveyInputScreenData.AlternativeContactName = this.txtAltContactName.Text;
                cMain.p_cSurveyInputScreenData.AlternativeContactTelNo = this.txtAltContactTelNo.Text;
                cMain.p_cSurveyInputScreenData.AlternativeContactMobileNo = this.txtAltContactMobNo.Text;
                cMain.p_cSurveyInputScreenData.MxmProjDescription = this.txtReplacementType.Text;
                cMain.p_cSurveyInputScreenData.SpecialResidentNote = this.txtSpecialResidentNote.Text;               
                
                cMain.p_cSurveyInputScreenData.PropertyType = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbPropertyType));
                cMain.p_cSurveyInputScreenData.ABPAXFloorLevel = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbFloorLevel));
                cMain.p_cSurveyInputScreenData.ABPAXInstallationType = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbInstallationType));
                cMain.p_cSurveyInputScreenData.ABPAXAsbestosPresumed = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbAsbestosPresumed));
                cMain.p_cSurveyInputScreenData.ABPAXAccessEquipment = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbAccessEquipment));
                cMain.p_cSurveyInputScreenData.ABPAXPermanentGasVent = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbPermanentGasVent));
                cMain.p_cSurveyInputScreenData.ABPAXWindowBoard = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbWindowboard));
                cMain.p_cSurveyInputScreenData.ABPAXStructuralFaults = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbStructuralFaults));
                cMain.p_cSurveyInputScreenData.ABPAXServicesToMove = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbServicesToMove));
                cMain.p_cSurveyInputScreenData.DisabledAdaptionsRequired = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbDisabledAdaptionsRqd));
                cMain.p_cSurveyInputScreenData.MxmDoorChoiceFormReceived = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbDoorChoiceFormRcvd));

                cMain.p_cSurveyInputScreenData.ABPAXInternalDamage = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbInternalDamage));
                cMain.p_cSurveyInputScreenData.ABPAXWorkAccessRestrictions = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbWorkAccessRestrictions));
                cMain.p_cSurveyInputScreenData.ABPAXPublicProtection = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbPublicProtection));

                cMain.p_cSurveyInputProjectNotes = new List<cProjectNotesTable>();
                foreach (cProjectNotesTable cNote in this.m_cProjectNotes)
                {

                    if (cNote.IDKey == -1)
                    {
                        cMain.p_cSurveyInputProjectNotes.Add(cNote);

                    }

                }

                if (this.btnSurveyedDate.Tag != null)
                {

                    //Update key fields
                    DateTime dSurveyDate = DateTime.MinValue;
                    if (DateTime.TryParse(this.btnSurveyedDate.Tag.ToString(), out dSurveyDate) == true)
                    {
                        //Log survey date.
                        cMain.p_cSurveyInputScreenData.MXM1002TrfDate = dSurveyDate;
                    }

                  
                }

                //v1.0.21 - Keep record of health and safety incomplete flag.
                cMain.p_cSurveyInputScreenData.ABPAWHealthSafetyInComplete = this.m_cProjectData.ABPAWHealthSafetyInComplete;
               
               
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

        /// <summary>
        /// Logs all the changes for saving
        /// </summary>
        /// <returns></returns>
        private async Task<bool> LogChangesForSaving(bool v_bCheckingForChangesOnly)
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
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.ResidentTelNo = this.txtResidentTelNo.Text;}
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmTelephoneNo", this.txtResidentTelNo.Text);
                }

                //Check resident mobile number has changed.
                if (this.txtResidentMobileNo.Text != this.m_cProjectData.ResidentMobileNo)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.ResidentMobileNo = this.txtResidentMobileNo.Text;}
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmResidentMobileNo", this.txtResidentMobileNo.Text);
                }

                //Check resident alternate contact name has changed.
                if (this.txtAltContactName.Text != this.m_cProjectData.AlternativeContactName)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.AlternativeContactName = this.txtAltContactName.Text;}
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmAlternativeContactName", this.txtAltContactName.Text);
                }

                //Check resident alternate contact telephone number has changed.
                if (this.txtAltContactTelNo.Text != this.m_cProjectData.AlternativeContactTelNo)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.AlternativeContactTelNo = this.txtAltContactTelNo.Text;}
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmAlternativeContactTelNo", this.txtAltContactTelNo.Text);
                }

                //Check resident alternate contact mobile number has changed.
                if (this.txtAltContactMobNo.Text != this.m_cProjectData.AlternativeContactMobileNo)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.AlternativeContactMobileNo = this.txtAltContactMobNo.Text;}
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmAlternativeContactMobileNo", this.txtAltContactMobNo.Text);
                }

                //Check replacement type text has changed.
                if (this.txtReplacementType.Text != this.m_cProjectData.MxmProjDescription)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.MxmProjDescription = this.txtReplacementType.Text;}
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

                iValue  = this.LogComboValueChange(this.cmbInstallationType.Name, "ABPAXInstallationType", this.m_cProjectData.ABPAXInstallationType);
                if (v_bCheckingForChangesOnly == false) 
                {
                    this.m_cProjectData.ABPAXInstallationType = iValue;
                }
                
                iValue  = this.LogComboValueChange(this.cmbAsbestosPresumed.Name, "ABPAXAsbestosPresumed", this.m_cProjectData.ABPAXAsbestosPresumed);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXAsbestosPresumed = iValue;
                }
                
                iValue  = this.LogComboValueChange(this.cmbAccessEquipment.Name, "ABPAXAccessEquipment", this.m_cProjectData.ABPAXAccessEquipment);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXAccessEquipment = iValue;
                }
                
                iValue = this.LogComboValueChange(this.cmbPermanentGasVent.Name, "ABPAXPermanentGasVent", this.m_cProjectData.ABPAXPermanentGasVent);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXPermanentGasVent = iValue;
                }
                
                iValue  = this.LogComboValueChange(this.cmbWindowboard.Name, "ABPAXWindowBoard", this.m_cProjectData.ABPAXWindowBoard);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXWindowBoard = iValue;
                }
                
                iValue  = this.LogComboValueChange(this.cmbStructuralFaults.Name, "ABPAXStructuralFaults", this.m_cProjectData.ABPAXStructuralFaults);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXStructuralFaults = iValue;
                }
                
                iValue  = this.LogComboValueChange(this.cmbServicesToMove.Name, "ABPAXServicesToMove", this.m_cProjectData.ABPAXServicesToMove);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXServicesToMove = iValue;
                }
                
                iValue  = this.LogComboValueChange(this.cmbDisabledAdaptionsRqd.Name, "MxmDisabledAdaptionsRequired", this.m_cProjectData.DisabledAdaptionsRequired);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.DisabledAdaptionsRequired = iValue;
                }
                
                iValue  = this.LogComboValueChange(this.cmbDoorChoiceFormRcvd.Name, "MxmDoorChoiceFormReceived", this.m_cProjectData.MxmDoorChoiceFormReceived);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.MxmDoorChoiceFormReceived = iValue;
                }
                
                iValue   = this.LogComboValueChange(this.cmbInternalDamage.Name, "ABPAXInternDamage", this.m_cProjectData.ABPAXInternalDamage);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXInternalDamage = iValue;
                }
                
                iValue  = this.LogComboValueChange(this.cmbWorkAccessRestrictions.Name, "ABPAXWrkAccRestrictions", this.m_cProjectData.ABPAXWorkAccessRestrictions);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXWorkAccessRestrictions = iValue;
                }
                
                iValue  = this.LogComboValueChange(this.cmbPublicProtection.Name, "ABPAXPublicProtect", this.m_cProjectData.ABPAXPublicProtection);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXPublicProtection = iValue;
                }
                

                //v1.0.21 - Save HS incomplete flag               
                if (this.m_cProjectData.ABPAWHealthSafetyInComplete.HasValue == true)
                {
                    if (this.m_cProjectData.ABPAWHealthSafetyInComplete.Value == (int)cSettings.YesNoBaseEnum.Yes)
                    {

                        //This is not a change the user has made so do not include when checking changes only.
                        if (v_bCheckingForChangesOnly == false) { 
                            
                            this.m_cProjectData.ABPAWHealthSafetyInComplete = (int)cSettings.YesNoBaseEnum.No;
                            this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "ABPAWHealthSafetyInComplete", Convert.ToString((int)cSettings.YesNoBaseEnum.No));
                                                
                        }
                        

                    }

                }


                //v1.0.2 - Decide if we need to check survey input date.
                bool bCheckSurveyInputDate = true;
                if (v_bCheckingForChangesOnly == true)
                {
                    if (this.m_bUserAppliedSurveyDate == false)
                    {
                        bCheckSurveyInputDate = false;

                    }

                }

                //v1.0.2 - Only check survey date if flagged
                if (bCheckSurveyInputDate == true)
                {

                    //Update key fields
                    DateTime dSurveyDate = DateTime.MinValue;
                    if (DateTime.TryParse(this.btnSurveyedDate.Tag.ToString(), out dSurveyDate) == false)
                    {
                        throw new Exception("No survey date set for sub project " + this.m_cProjectData.SubProjectNo);
                    }
                    else
                    {

                        //v1.0.2 - Decide if we need to update survey dates.
                        bool bUpdateSurveyorDetails = true;
                        if (this.m_cProjectData.MXM1002TrfDate.HasValue == true)
                        {
                            if (this.m_bUserAppliedSurveyDate == false)
                            {
                                if (this.m_cProjectData.MXM1002TrfDate.Value.Equals(dSurveyDate) == true)
                                {
                                    bUpdateSurveyorDetails = false;

                                }

                            }

                        }


                        if (bUpdateSurveyorDetails == true)
                        {                       

                            //Log survey date.
                            if (v_bCheckingForChangesOnly == false) {this.m_cProjectData.MXM1002TrfDate = dSurveyDate;}
                            this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "Mxm1002TrfDate", dSurveyDate.ToString());

                            //v1.0.2 - Update surveyor details
                            if (v_bCheckingForChangesOnly == false) {this.m_cProjectData.SurveyorName = await cSettings.GetUserDisplayName();}
                            this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmSurveyorName", this.m_cProjectData.SurveyorName);

                            if (v_bCheckingForChangesOnly == false) {this.m_cProjectData.SurveyorProfile = await cSettings.GetUserName();}
                            this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmSurveyorProfile", this.m_cProjectData.SurveyorProfile);

                            if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.SurveyorPCTag = cSettings.GetMachineName(); }
                            this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmSurveyorPCTag", this.m_cProjectData.SurveyorPCTag);
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
        /// Check if survey input passes save validation.
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

                bComboOK = await this.IsComboValueSelected(this.cmbDoorChoiceFormRcvd.Name, "Door choice form received");
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

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbDoorChoiceFormRcvd.Name);
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

                if (this.btnSurveyedDate.Tag != null)
                {
                    DateTime dSurveyDate = DateTime.Now;
                    if (DateTime.TryParse(this.btnSurveyedDate.Tag.ToString(), out dSurveyDate) == true)
                    {
                        return true;
                    }

                }

                await cSettings.DisplayMessage("You have not set a survey date for this sub project, please amend and try-again.", "Survey Date Required.");

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
        private async Task<bool> IsComboValueSelected(string v_sControlName,string v_sSelectionType)
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
        /// Set survey date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSurveyedDate_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (this.puDatePicker.IsOpen == false)
                {

                    //Default to survey date initially set.
                    DateTime dSurveyDate = DateTime.Now;

                    if (this.btnSurveyedDate.Tag != null)
                    {
                        //Try and convert button date so we can default date time picker
                        DateTime.TryParse(this.btnSurveyedDate.Tag.ToString(), out dSurveyDate);

                    }
                    else if (this.m_cProjectData.MXM1002TrfDate.HasValue == true)
                    {
                        dSurveyDate = this.m_cProjectData.MXM1002TrfDate.Value;

                    }
                    else if (this.m_cProjectData.EndDateTime.HasValue == true)
                    {
                       dSurveyDate = this.m_cProjectData.EndDateTime.Value;

                    }
                    
                                      
                    this.dtPicker.Value = dSurveyDate;

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
        /// Use date from survey date picker.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUseSurveyedDate_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //v1.0.2 - Indicate user has applied survey date.
                this.m_bUserAppliedSurveyDate = true;

                //Set survey date button.
                this.DisplaySurveyDateOnSurveyButton(this.dtPicker.Value);

                //Hide popup.
                this.puDatePicker.IsOpen = false; 

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Display survey date on survey date button.
        /// </summary>
        /// <param name="v_dSurveyDate"></param>
        private void DisplaySurveyDateOnSurveyButton(DateTime? v_dSurveyDate)
        {

            try
            {

                if (v_dSurveyDate.HasValue == true)
                {
                    this.btnSurveyedDate.Content = "Surveyed date: " + cMain.ReturnDisplayDate(v_dSurveyDate.Value);
                }
                else
                {
                    this.btnSurveyedDate.Content = "Surveyed date: N\\A";
                }
                
                this.btnSurveyedDate.Tag = v_dSurveyDate;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Go to the customer signature page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResidentSignature_Click(object sender, RoutedEventArgs e)
        {

            try
            {


                this.Frame.Navigate(typeof(SignaturePage), this.m_cProjectData.SubProjectNo);


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// v1.0.2 - Back button clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void backButton_Click(object sender, RoutedEventArgs e)
        {

            bool bGoBack = true;
            try
            {

                bool bAnyNewNotes = this.AnyUnSavedNotes();
                await this.LogChangesForSaving(true);

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
        /// v1.0.4 - Copy selections.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCopySelections_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                cMain.p_cSurveyInputCopiedSelections = new cProjectTable();

                cMain.p_cSurveyInputCopiedSelections.ProjectNo = this.m_cProjectData.ProjectNo;
                cMain.p_cSurveyInputCopiedSelections.ProjectName = this.m_cProjectData.ProjectName;
                cMain.p_cSurveyInputCopiedSelections.SubProjectNo = this.m_cProjectData.SubProjectNo;
                cMain.p_cSurveyInputCopiedSelections.SubProjectName = this.m_cProjectData.SubProjectName;
                cMain.p_cSurveyInputCopiedSelections.DeliveryStreet = this.m_cProjectData.DeliveryStreet;

                cMain.p_cSurveyInputCopiedSelections.PropertyType = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbPropertyType));
                cMain.p_cSurveyInputCopiedSelections.ABPAXFloorLevel = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbFloorLevel));
                cMain.p_cSurveyInputCopiedSelections.ABPAXInstallationType = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbInstallationType));
                cMain.p_cSurveyInputCopiedSelections.ABPAXAsbestosPresumed = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbAsbestosPresumed));
                cMain.p_cSurveyInputCopiedSelections.ABPAXAccessEquipment = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbAccessEquipment));
                cMain.p_cSurveyInputCopiedSelections.ABPAXPermanentGasVent = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbPermanentGasVent));
                cMain.p_cSurveyInputCopiedSelections.ABPAXWindowBoard = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbWindowboard));
                cMain.p_cSurveyInputCopiedSelections.ABPAXStructuralFaults = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbStructuralFaults));
                cMain.p_cSurveyInputCopiedSelections.ABPAXServicesToMove = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbServicesToMove));
                cMain.p_cSurveyInputCopiedSelections.DisabledAdaptionsRequired = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbDisabledAdaptionsRqd));
                cMain.p_cSurveyInputCopiedSelections.MxmDoorChoiceFormReceived = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbDoorChoiceFormRcvd));
                cMain.p_cSurveyInputCopiedSelections.ABPAXInternalDamage = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbInternalDamage));
                cMain.p_cSurveyInputCopiedSelections.ABPAXWorkAccessRestrictions = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbWorkAccessRestrictions));
                cMain.p_cSurveyInputCopiedSelections.ABPAXPublicProtection = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbPublicProtection));

                //v1.0.6 - Reset.
                cMain.p_cSurveyInputCopiedLastNote = null;

                //v1.0.6 - Copy last note if any exist.
                if (this.m_cProjectNotes != null)
                {
                    cMain.p_cSurveyInputCopiedLastNote = this.ReturnLastNote();

                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.5 - Return last note.
        /// </summary>
        /// <returns></returns>
        private cProjectNotesTable ReturnLastNote()
        {

            cProjectNotesTable cLastNote = null;
            try
            {

                //Return last note.
                var oResult = (from oCols in this.m_cProjectNotes
                               orderby oCols.InputDateTime descending
                               select oCols);

                foreach (cProjectNotesTable cNote in oResult)
                {
                    cLastNote = cNote;
                    break;

                }

                oResult = null;

                return cLastNote;


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return null;

            }
        }

        /// <summary>
        /// v1.0.4 - Paste selections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnPasteSelections_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (cMain.p_cSurveyInputCopiedSelections != null)
                {

                    //Only paste in if same contract.
                    if (cMain.p_cSurveyInputCopiedSelections.ProjectNo.Equals(this.m_cProjectData.ProjectNo,StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                   
                        StringBuilder sbMsg = new StringBuilder();
                        sbMsg.Append("Are you sure you want to paste in the selections from the following sub-project:");
                        sbMsg.Append(Environment.NewLine);
                        sbMsg.Append(Environment.NewLine);
                        sbMsg.Append(cMain.p_cSurveyInputCopiedSelections.SubProjectNo);
                        sbMsg.Append(Environment.NewLine);
                        sbMsg.Append(cMain.p_cSurveyInputCopiedSelections.DeliveryStreet);

                        cSettings.YesNo ynResp = await cSettings.DisplayYesNo(sbMsg.ToString(), "Paste Selections");

                        if (ynResp == cSettings.YesNo.Yes)
                        {

                            int? iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbPropertyType));
                            this.SelectDropDown(this.cmbPropertyType.Name, cMain.p_cSurveyInputCopiedSelections.PropertyType,iCurValue);

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbFloorLevel));
                            this.SelectDropDown(this.cmbFloorLevel.Name, cMain.p_cSurveyInputCopiedSelections.ABPAXFloorLevel, iCurValue);

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbInstallationType));
                            this.SelectDropDown(this.cmbInstallationType.Name, cMain.p_cSurveyInputCopiedSelections.ABPAXInstallationType, iCurValue);

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbAsbestosPresumed));
                            this.SelectDropDown(this.cmbAsbestosPresumed.Name, cMain.p_cSurveyInputCopiedSelections.ABPAXAsbestosPresumed, iCurValue);

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbAccessEquipment));
                            this.SelectDropDown(this.cmbAccessEquipment.Name, cMain.p_cSurveyInputCopiedSelections.ABPAXAccessEquipment, iCurValue);

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbPermanentGasVent));
                            this.SelectDropDown(this.cmbPermanentGasVent.Name, cMain.p_cSurveyInputCopiedSelections.ABPAXPermanentGasVent, iCurValue);

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbWindowboard));
                            this.SelectDropDown(this.cmbWindowboard.Name, cMain.p_cSurveyInputCopiedSelections.ABPAXWindowBoard, iCurValue);

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbStructuralFaults));
                            this.SelectDropDown(this.cmbStructuralFaults.Name, cMain.p_cSurveyInputCopiedSelections.ABPAXStructuralFaults, iCurValue);

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbServicesToMove));
                            this.SelectDropDown(this.cmbServicesToMove.Name, cMain.p_cSurveyInputCopiedSelections.ABPAXServicesToMove, iCurValue);

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbDisabledAdaptionsRqd));
                            this.SelectDropDown(this.cmbDisabledAdaptionsRqd.Name, cMain.p_cSurveyInputCopiedSelections.DisabledAdaptionsRequired, iCurValue);

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbDoorChoiceFormRcvd));
                            this.SelectDropDown(this.cmbDoorChoiceFormRcvd.Name, cMain.p_cSurveyInputCopiedSelections.MxmDoorChoiceFormReceived, iCurValue);

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbInternalDamage));
                            this.SelectDropDown(this.cmbInternalDamage.Name, cMain.p_cSurveyInputCopiedSelections.ABPAXInternalDamage, iCurValue);

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbWorkAccessRestrictions));
                            this.SelectDropDown(this.cmbWorkAccessRestrictions.Name, cMain.p_cSurveyInputCopiedSelections.ABPAXWorkAccessRestrictions, iCurValue);

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbPublicProtection));
                            this.SelectDropDown(this.cmbPublicProtection.Name, cMain.p_cSurveyInputCopiedSelections.ABPAXPublicProtection, iCurValue);


                            //v1.0.5 - If note, show notes popup and pre-fill text.
                            if (cMain.p_cSurveyInputCopiedLastNote != null)
                            {

                                this.btnAddViewNotes.Flyout.ShowAt(this.btnAddViewNotes);
                                this.txtNewNote.Text = cMain.p_cSurveyInputCopiedLastNote.NoteText;
                                this.txtNewNote.SelectionStart = this.txtNewNote.Text.Length;
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


    }
}
