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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ANG_ABP_SURVEYOR_APP.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SetSurveyDatesEditPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// Hold the stored project data.
        /// </summary>
        private cProjectTable m_cProjectData = null;

        /// <summary>
        /// v1.0.1 - Store the project notes.
        /// </summary>
        private List<cProjectNotesTable> m_cProjectNotes = null;

        ///// <summary>
        ///// Hold the stored project notes.
        ///// </summary>
        //private List<cProjectNotesTable> m_cProjectNotes = null;

        /// <summary>
        /// Flag to indicate if screen has been loaded.
        /// </summary>
        private bool m_bScreenLoaded = false;

        /// <summary>
        /// Selected survey date time.
        /// </summary>
        private DateTime? m_dSelectedDate = null;

        /// <summary>
        /// Log of updates made.
        /// </summary>
        private List<cUpdatesTable> m_cUpdateLog = null;

        /// <summary>
        /// v1.0.2 - Flag to indicate user wants to un confirm survey.
        /// </summary>
        private bool m_bUnConfirmSurvey = false;

        /// <summary>
        /// v1.0.2 - Flag if any note changes have been made.
        /// </summary>
        private bool m_bAreThereNoteChanges = false;

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
        public SetSurveyDatesEditPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            try
            {
                this.PopulateDropDowns();

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
                this.tbSubProjectID.Text = e.Parameter.ToString();
                this.DisplayProjectDetails();

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
        /// Display project details.
        /// </summary>
        private void DisplayProjectDetails()
        {

            try
            {

                //Fetch data from DB
                this.m_cProjectData = cMain.p_cDataAccess.GetSubProjectProjectData(this.tbSubProjectID.Text);

                //v1.0.1 - Fetch sub project notes.
                this.m_cProjectNotes = cMain.p_cDataAccess.GetSubProjectNotesData(this.tbSubProjectID.Text);
            
                //Project name
                this.tbSubProjectName.Text = this.m_cProjectData.SubProjectName;

                //Survey date
                this.DisplaySurveyDate(this.m_cProjectData.EndDateTime);

                //Set Install status.
                this.SelectInstallStatus(this.m_cProjectData.Mxm1002InstallStatus.Value);

                //Set Progress status.
                this.SelectProgressStatus(this.m_cProjectData.Mxm1002ProgressStatus.Value);

                //Set door choice from received drop down.
                this.SelectDoorChoice(this.m_cProjectData.MxmDoorChoiceFormReceived);

                //Street name.
                this.txtStreetName.Text = cSettings.ReturnString(this.m_cProjectData.DeliveryStreet);

                //Resident name.
                this.txtResidentName.Text = cSettings.ReturnString(this.m_cProjectData.ResidentName);

                //Resident Tel No.
                this.txtResidentTelNo.Text = cSettings.ReturnString(this.m_cProjectData.ResidentTelNo);

                //Resident Mob No.
                this.txtResidentMobNo.Text = cSettings.ReturnString(this.m_cProjectData.ResidentMobileNo);

                //Alternative contact Name.
                this.txtAltContactName.Text = cSettings.ReturnString(this.m_cProjectData.AlternativeContactName);

                //Alternative contact tel no.
                this.txtAltContactTelNo.Text = cSettings.ReturnString(this.m_cProjectData.AlternativeContactTelNo);

                //Alternative contact mob no.
                this.txtAltContactMobNo.Text = cSettings.ReturnString(this.m_cProjectData.AlternativeContactMobileNo);

                //Replacement type.
                this.txtReplacementType.Text = cSettings.ReturnString(this.m_cProjectData.MxmProjDescription);

                //Display Last Update date time
                this.DisplayLastUpdateDate();

                //Hide other notes text box.
                this.txtOtherNotes.Text = String.Empty;
                this.txtOtherNotes.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                //Special resident notes.
                if (this.m_cProjectData.SpecialResidentNote == null)
                {
                    this.txtSpecialResidentNote.Text = String.Empty;
                }
                else
                {
                    this.txtSpecialResidentNote.Text = this.m_cProjectData.SpecialResidentNote;
                }

                //Display updates pending
                this.DisplayUpdateCount();
                

                //Flag to indicate that the screen controls have been loaded.
                this.m_bScreenLoaded = true;
              

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Display on screen how many updates are pending.
        /// </summary>
        private void DisplayUpdateCount()
        {

            try
            {
               
                List<cUpdatesTable> cUpdates = cMain.p_cDataAccess.ReturnPendingUpdatesForSubProject(this.m_cProjectData.SubProjectNo);

                if (cUpdates.Count() > 0)
                {
                    string sMsg = cMain.GetAppResourceValue("UpdateCountMessage");

                    this.tbUpdatesPending.Text = cUpdates.Count().ToString() + " " + sMsg;

                }
                else
                {
                    this.tbUpdatesPending.Text = String.Empty;
                }
               

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Select door choice form received selection.
        /// </summary>
        /// <param name="v_bDoorChoiceFormReceived"></param>
        private void SelectDoorChoice(int? v_iDoorChoiceFormReceived)
        {

            try
            {

                int iIndex = 0;
                foreach (ComboBoxItem cmbItem in this.cmbDoorChoiceFormReceived.Items)
                {
                    if (cmbItem.Tag.ToString() == v_iDoorChoiceFormReceived.ToString())
                    {
                        this.cmbDoorChoiceFormReceived.SelectedIndex = iIndex;
                        return;
                    }

                    iIndex += 1;
                }
             
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);


            }

        }

        /// <summary>
        /// Select install status on combo box.
        /// </summary>
        /// <param name="v_sTag"></param>
        private void SelectInstallStatus(int v_iTag)
        {

            try
            {

                int iIndex = 0;
                foreach (ComboBoxItem cmbItem in this.cmbInstallStatus.Items)
                {
                    if (cmbItem.Tag.ToString() == v_iTag.ToString())
                    {
                        this.cmbInstallStatus.SelectedIndex = iIndex;
                        return;
                    }

                    iIndex += 1;
                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }


        /// <summary>
        /// Select progress status on combo box.
        /// </summary>
        /// <param name="v_sTag"></param>
        private void SelectProgressStatus(int v_iTag)
        {

            try
            {

                int iIndex = 0;
                foreach (ComboBoxItem cmbItem in this.cmbProgressStatus.Items)
                {
                    if (cmbItem.Tag.ToString() == v_iTag.ToString())
                    {
                        this.cmbProgressStatus.SelectedIndex = iIndex;
                        return;
                    }

                    iIndex += 1;
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

              
                ComboBoxItem cmbItem = null;

                // ** Install status drop down.
                List<cBaseEnumsTable> oInstalls = cMain.p_cDataAccess.GetEnumsForField("MXM1002INSTALLSTATUS");
                foreach (cBaseEnumsTable oInstall in oInstalls)
                {
                    cmbItem = new ComboBoxItem();
                    cmbItem.Content = oInstall.EnumName;
                    cmbItem.Tag = oInstall.EnumValue;
                    this.cmbInstallStatus.Items.Add(cmbItem);

                }


                //Progress Status
                List<cBaseEnumsTable> oProgesses = cMain.p_cDataAccess.GetEnumsForField("MXM1002PROGRESSSTATUS");
                foreach (cBaseEnumsTable oProgress in oProgesses)
                {
                    cmbItem = new ComboBoxItem();
                    cmbItem.Content = oProgress.EnumName;
                    cmbItem.Tag = oProgress.EnumValue;
                    this.cmbProgressStatus.Items.Add(cmbItem);

                }


                //Notes  
                this.cmbNotes.Items.Add(cSettings.p_sNoteIt_None);
                this.cmbNotes.Items.Add(cSettings.p_sNoteIt_Carded);
                this.cmbNotes.Items.Add(cSettings.p_sNoteIt_AlreadyPVC);
                this.cmbNotes.Items.Add(cSettings.p_sNoteIt_OmitRTB);
                this.cmbNotes.Items.Add(cSettings.p_sNoteIt_Omitted);
                this.cmbNotes.Items.Add(cSettings.p_sNoteIt_Other);
                this.cmbNotes.SelectedIndex = 0;


                // ** Door choice form received drop down
                List<cBaseEnumsTable> cDoorChoices = cMain.p_cDataAccess.GetEnumsForField("MxmDoorChoiceFormReceived");
                foreach (cBaseEnumsTable oDoor in cDoorChoices)
                {
                    cmbItem = new ComboBoxItem();
                    cmbItem.Content = oDoor.EnumName;
                    cmbItem.Tag = oDoor.EnumValue;
                    this.cmbDoorChoiceFormReceived.Items.Add(cmbItem);


                }


                // ** Set survey dates time picker
                this.cmbTimePicker.Items.Add(cSettings.p_sPleaseChoose);
                this.cmbTimePicker.Items.Add(cSettings.p_sTime_AM);
                this.cmbTimePicker.Items.Add(cSettings.p_sTime_PM);
                this.cmbTimePicker.Items.Add(cSettings.p_sTime_Specific);
                this.cmbTimePicker.SelectedIndex = 0;

                ////Default time picker to hidden.
                this.tpSurveyTime.IsEnabled = false;
                this.tpSurveyTime.Visibility = Windows.UI.Xaml.Visibility.Collapsed; //v1.0.1

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Apply survey date
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
                    this.cmbTimePicker.Focus(FocusState.Programmatic);
                    this.SetTheSurveyDatesPopup.IsOpen = true;
                    return;

                }

                //v1.0.2 - Warn user if sub project will not display on front screen as status incorrect.                
                bool bWillItDisplay = cMain.WillSubProjectDisplayOnFrontScreen(this.m_cProjectData.Mxm1002InstallStatus.Value);

                if (bWillItDisplay == false)
                {

                    List<string> sJobs = new List<string>();
                    sJobs.Add(this.m_cProjectData.SubProjectNo);


                    string sMessage = cMain.ReturnSubProjectWillNotDisplayOnFrontScreenMessage(sJobs);

                    await cSettings.DisplayMessage(sMessage, "Warning");

                }

                //v1.0.2 - Display un-confirm button now user has confirmed.
                this.m_bUnConfirmSurvey = false;
                this.btnUnConfirmSurvey.Visibility = Windows.UI.Xaml.Visibility.Visible;

                //Place date and time in strings.
                String sDate = this.dtPickerSurveyDate.Value.ToString("dd/MM/yyyy");
                String sTime = this.tpSurveyTime.Time.Hours.ToString().PadLeft(2, '0') + ":" + this.tpSurveyTime.Time.Minutes.ToString().PadLeft(2, '0') + ":" + this.tpSurveyTime.Time.Seconds.ToString().PadLeft(2, '0');

                //Put date and time together.
                DateTime ?dSurveyDate = Convert.ToDateTime(sDate + " " + sTime);

                //Display survey date on screen.
                this.DisplaySurveyDate(dSurveyDate);

                //Close pop up
                this.SetTheSurveyDatesPopup.IsOpen = false;
            
            
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


        }

        /// <summary>
        /// Display survey date on screen.
        /// </summary>
        /// <param name="v_dSurveyDate"></param>
        private void DisplaySurveyDate(DateTime? v_dSurveyDate)
        {

            try
            {

                //Set selected survey date.
                this.m_dSelectedDate = v_dSurveyDate;

                if (v_dSurveyDate.HasValue == true)
                {
                    //Display date
                    this.tbSurveyPlanDate.Text = cMain.ReturnDisplayDate(v_dSurveyDate.Value);
                    this.tbSurveyPlanDate.Text += " " + cMain.ReturnDisplayTime(v_dSurveyDate.Value);

                }
                else
                {
                    this.tbSurveyPlanDate.Text = "No date set.";

                }

                //v1.0.2 - Display un confirm button if survey is confirmed.
                if (this.m_cProjectData.MxmConfirmedAppointmentIndicator.HasValue == true)
                {
                    if (this.m_cProjectData.MxmConfirmedAppointmentIndicator.Value == (int)cSettings.YesNoBaseEnum.Yes)
                    {

                        this.btnUnConfirmSurvey.Visibility = Windows.UI.Xaml.Visibility.Visible;

                    }
                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);


            }
        }


        /// <summary>
        /// Display last update date on screen.
        /// </summary>
        /// <param name="v_dSurveyDate"></param>
        private void DisplayLastUpdateDate()
        {

            try
            {

                DateTime? dLastUpdate = cMain.ReturnSubProjectLastUpdate(this.m_cProjectData);

                if (dLastUpdate.HasValue == true)
                {
                    //Display date
                    this.tbLastUpdate.Text = cMain.ReturnDisplayDate(dLastUpdate.Value);
                    this.tbLastUpdate.Text += " " + dLastUpdate.Value.ToString("HH:mm:ss");

                }
                else
                {
                    this.tbLastUpdate.Text = "No update date available.";

                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Time picker selection changed.
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
        /// Set survey plan date.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetSurveyPlanDate_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (this.SetTheSurveyDatesPopup.IsOpen == false)
                {

                    this.ConfigureDatePicker();

                    this.SetTheSurveyDatesPopup.IsOpen = true;
                }
                else
                {
                    this.SetTheSurveyDatesPopup.IsOpen = false;
                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Configure date picker with survey date.
        /// </summary>
        private void ConfigureDatePicker()
        {

            try
            {
                if (this.m_dSelectedDate.HasValue == true)
                {
                    this.dtPickerSurveyDate.Value = this.m_dSelectedDate.Value;
                    this.cmbTimePicker.SelectedValue = cMain.ReturnTimeDropDownSelection(this.m_dSelectedDate.Value);

                    TimeSpan tsTime = new TimeSpan(this.m_dSelectedDate.Value.Hour,this.m_dSelectedDate.Value.Minute,this.m_dSelectedDate.Value.Second);
                    this.tpSurveyTime.Time = tsTime;

                }                

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Notes drop down selection change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbNotes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {

                //If screen not loaded then return.
                if (this.m_bScreenLoaded == false)
                {
                    return;
                }
                
                string sItem = this.cmbNotes.SelectedItem.ToString();
                if (sItem.Equals(cSettings.p_sNoteIt_Other) == true)
                {

                    this.txtOtherNotes.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    this.txtOtherNotes.Focus(FocusState.Programmatic);

                }
                else
                {
                    this.txtOtherNotes.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// View notes history.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewNotesHistory_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //Only if popup is not open already.
                if (this.NotesHistoryPopup.IsOpen == false)
                {

                    ////Create new instance, we need to bind to list view.
                    //List<cNotesHistory> cNotes = new List<cNotesHistory>();

                    ////Split notes up
                    //List<string> sNotes = this.m_cProjectData.Noteitmemo.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList<string>();

                 
                    //cNotesHistory cNote = null;

                    ////Loop through notes and add to class list for binding.
                    //foreach (string sNote in sNotes)
                    //{
                    //    cNote = new cNotesHistory();
                    //    cNote.NoteText = sNote;
                    //    cNotes.Add(cNote);
                    //}

                    //cNotes.Reverse();

                    //Set notes pop-up to open in middle of screen.
                    this.NotesHistoryPopup.Width = Window.Current.Bounds.Width / 2;
                    this.gdNotesPopupGrid.Width = Window.Current.Bounds.Width  / 2;
                    this.gdNotesPopupGrid.Height = Window.Current.Bounds.Height - 10;

                    //this.NotesHistoryPopup.Height = Window.Current.Bounds.Height - this.NotesHistoryPopup.VerticalOffset;
                    this.NotesHistoryPopup.VerticalOffset = 0;
                    this.NotesHistoryPopup.HorizontalOffset = 0; // (Window.Current.Bounds.Width - this.NotesHistoryPopup.Width) / 2;
                    this.NotesHistoryPopup.IsOpen = true;

                    //Bind
                    this.lvNotes.ItemsSource = this.m_cProjectNotes; // cNotes;

                }
                else
                {
                    this.NotesHistoryPopup.IsOpen = false;

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Update survey
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnUpdateSurvey_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                bool bSaveOK = await this.SaveSubProject();

                if (bSaveOK == true)
                {
                    this.navigationHelper.GoBack();
                   
                }
                          
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// v1.0.2 - Save sub-project
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SaveSubProject()
        {
            try
            {
                // Check notes are OK.
                if (await this.AreNotesOK() == false)
                {
                    return false;
                }

                bool bSaveOK = await this.LogChangesForUpdateTable(true);

                if (bSaveOK == true)
                {

                    //If changed logged then update.
                    if (this.m_cUpdateLog.Count > 0)
                    {
                        bSaveOK = cMain.p_cDataAccess.AddUpdatesToUpdateTable(this.m_cUpdateLog);
                    }
                    else
                    {
                        bSaveOK = true; //No changes say all is OK.
                    }


                    return bSaveOK;                   

                }

                await cSettings.DisplayMessage("An error has occurred when trying to save your changes, please try again.", "Error Saving");
                return false;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }



        }

        /// <summary>
        /// Log the changes for sync table.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> LogChangesForUpdateTable(bool v_bSaveChanges)
        {

            try
            {

                //Create new instance of log list.
                this.m_cUpdateLog = new List<cUpdatesTable>();

                //Check for survey date update.
                this.CheckSurveyDate(v_bSaveChanges);

                //Check street name has changed.
                if (this.txtStreetName.Text != this.m_cProjectData.DeliveryStreet)
                {
                    if (v_bSaveChanges == true) { this.m_cProjectData.DeliveryStreet = this.txtStreetName.Text; }
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "DeliveryStreet", this.txtStreetName.Text);
                }

                //Check resident name has changed.
                if (this.txtResidentName.Text != this.m_cProjectData.ResidentName)
                {
                    if (v_bSaveChanges == true) { this.m_cProjectData.ResidentName = this.txtResidentName.Text;}
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmResidentName", this.txtResidentName.Text);
                }

                //Check resident telephone number has changed.
                if (this.txtResidentTelNo.Text != this.m_cProjectData.ResidentTelNo)
                {
                    if (v_bSaveChanges == true) { this.m_cProjectData.ResidentTelNo = this.txtResidentTelNo.Text;}
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmTelephoneNo", this.txtResidentTelNo.Text);
                }

                //Check resident mobile number has changed.
                if (this.txtResidentMobNo.Text != this.m_cProjectData.ResidentMobileNo)
                {
                    if (v_bSaveChanges == true) { this.m_cProjectData.ResidentMobileNo = this.txtResidentMobNo.Text;}
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmResidentMobileNo", this.txtResidentMobNo.Text);
                }

                //Check resident alternate contact name has changed.
                if (this.txtAltContactName.Text != this.m_cProjectData.AlternativeContactName)
                {
                    if (v_bSaveChanges == true) { this.m_cProjectData.AlternativeContactName = this.txtAltContactName.Text;}
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmAlternativeContactName", this.txtAltContactName.Text);
                }

                //Check resident alternate contact telephone number has changed.
                if (this.txtAltContactTelNo.Text != this.m_cProjectData.AlternativeContactTelNo)
                {
                    if (v_bSaveChanges == true) { this.m_cProjectData.AlternativeContactTelNo = this.txtAltContactTelNo.Text;}
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmAlternativeContactTelNo", this.txtAltContactTelNo.Text);
                }

                //Check resident alternate contact mobile number has changed.
                if (this.txtAltContactMobNo.Text != this.m_cProjectData.AlternativeContactMobileNo)
                {
                    if (v_bSaveChanges == true) { this.m_cProjectData.AlternativeContactMobileNo = this.txtAltContactMobNo.Text;}
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmAlternativeContactMobileNo", this.txtAltContactMobNo.Text);
                }

                //Check replacement type text has changed.
                if (this.txtReplacementType.Text != this.m_cProjectData.MxmProjDescription)
                {
                    if (v_bSaveChanges == true) { this.m_cProjectData.MxmProjDescription = this.txtReplacementType.Text;}
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmProjDescription", this.txtReplacementType.Text);
                }

               
                //Check door choice form.
                this.CheckDoorChoiceForm(v_bSaveChanges);
                    
                //Check progress status has changed.
                this.CheckProgressStatus(v_bSaveChanges);

                //Check notes.
                await this.CheckNotes(v_bSaveChanges);

                //Special resident note.
                if (this.txtSpecialResidentNote.Text != this.m_cProjectData.SpecialResidentNote)
                {
                    if (v_bSaveChanges == true) { this.m_cProjectData.SpecialResidentNote = this.txtSpecialResidentNote.Text.Trim(); }
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmSpecialResidentNote", this.m_cProjectData.SpecialResidentNote);

                }

                //v1.0.2
                if (v_bSaveChanges == true)
                {
                    //Save details away.
                    bool bSaveOK = cMain.p_cDataAccess.UpdateProjectTable(this.m_cProjectData);

                    //All OK
                    return bSaveOK;

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
        /// Check notes for update.
        /// </summary>
        private async Task CheckNotes(bool v_bSaveChanges)
        {

            try
            {

                //v1.0.2 - Reset notes change flag.
                this.m_bAreThereNoteChanges = false;

                string sItem = this.cmbNotes.SelectedItem.ToString();
                if (sItem != null)
                {

                    if (sItem.Equals(cSettings.p_sNoteIt_None) == true)
                    {
                        return;
                    }

                    string sNoteText = string.Empty;

                    if (sItem.Equals(cSettings.p_sNoteIt_Other) == true)
                    {
                        sNoteText = this.txtOtherNotes.Text;

                    }
                    else
                    {
                        sNoteText = sItem;

                    }

                    //v1.0.2 - If we get here there are note changes.
                    this.m_bAreThereNoteChanges = true;

                    if (v_bSaveChanges == true) //v1.0.2
                    {
                        //v1.0.1 - Save note to notes table.
                        cProjectNotesTable cNote = await cSettings.ReturnNoteObject(this.m_cProjectData.SubProjectNo, sNoteText, DateTime.Now, cSettings.p_sProjectNoteType_General);
                        cMain.p_cDataAccess.SaveSubProjectNote(cNote);

                    }
                                       
                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.2 - Return selected progress status ID
        /// </summary>
        /// <returns></returns>
        private int ReturnSelectedProgressStatusID()
        {

            try
            {

                int iNewStatusID = -1;

                //Fetch selected status ID
                ComboBoxItem cmbItem = (ComboBoxItem)this.cmbProgressStatus.SelectedItem;
                if (cmbItem != null)
                {
                    if (cmbItem.Tag != null)
                    {
                        int.TryParse(cmbItem.Tag.ToString(), out iNewStatusID);

                    }

                }

                return iNewStatusID;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return -1;

            }
        }

        /// <summary>
        /// Check progress status
        /// </summary>
        private void CheckProgressStatus(bool v_bSaveChanges)
        {

            try
            {

                //v1.0.2 - Move code into it's own function as needed else where.
                int iNewStatusID = this.ReturnSelectedProgressStatusID();


                //Fetch selected status ID
                ComboBoxItem cmbItem = (ComboBoxItem)this.cmbProgressStatus.SelectedItem;
                if (cmbItem != null)
                {
                    if (cmbItem.Tag != null)
                    {
                        int.TryParse(cmbItem.Tag.ToString(), out iNewStatusID);
                       
                    }

                }

                int iOldStatusID = -1;

                //Fetch existing status ID        
                iOldStatusID = this.m_cProjectData.Mxm1002ProgressStatus.Value;

                //Compare status IDs and update if different.
                if (iOldStatusID != iNewStatusID)
                {

                    if (v_bSaveChanges == true)
                    {
                        this.m_cProjectData.Mxm1002ProgressStatus = iNewStatusID;
                        this.m_cProjectData.ProgressStatusName = cMain.p_cDataAccess.GetEnumValueName("ProjTable", "Mxm1002ProgressStatus", iNewStatusID);
                    }
                    
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog,this.m_cProjectData.SubProjectNo, "Mxm1002ProgressStatus", iNewStatusID.ToString());

                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Check of door choice form needs updating.
        /// </summary>
        private void CheckDoorChoiceForm(bool v_bSaveChanges)
        {

            try
            {

                //Work out original value.
                bool bOriginalValue = false;
                if (this.m_cProjectData.MxmDoorChoiceFormReceived.HasValue == true)
                {
                    if (this.m_cProjectData.MxmDoorChoiceFormReceived.Value == (int)cSettings.YesNoBaseEnum.Yes)
                    {
                        bOriginalValue = true;

                    }
                }


                ComboBoxItem cmbItem = (ComboBoxItem)this.cmbDoorChoiceFormReceived.SelectedItem;

                //Check door choice form received selection has changed.
                if ((int)cmbItem.Tag == (int)cSettings.YesNoBaseEnum.Yes)
                {
                    //If different then log.
                    if (bOriginalValue == false)
                    {
                        if (v_bSaveChanges == true) { this.m_cProjectData.MxmDoorChoiceFormReceived = (int)cSettings.YesNoBaseEnum.Yes; }
                        this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmDoorChoiceFormReceived", ((int)cSettings.YesNoBaseEnum.Yes).ToString());

                    }

                }
                else if (this.cmbDoorChoiceFormReceived.SelectedValue.ToString() == cSettings.p_sConfirmedStatus_No)
                {
                    //If different then log.
                    if (bOriginalValue == true)
                    {
                        if (v_bSaveChanges == true) { this.m_cProjectData.MxmDoorChoiceFormReceived = (int)cSettings.YesNoBaseEnum.No; }
                        this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmDoorChoiceFormReceived", ((int)cSettings.YesNoBaseEnum.No).ToString());

                    }

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


        }

        /// <summary>
        /// Check if survey date needs to be updated.
        /// </summary>
        private async void CheckSurveyDate(bool v_bSaveChanges)
        {

            try
            {

                bool bUpdateSureyDate = false;

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
                    bUpdateSureyDate = true;

                }

                if (bUpdateSureyDate == true)
                {

                    string sSurveyorProfile = await cSettings.GetUserName();
                    string sSurveyorName = await cSettings.GetUserDisplayName();
                    string sMachineName = cSettings.GetMachineName();


                    cSettings.SurveyDatesApply cApply;
                    cApply = cSettings.ApplySurveyDates(this.m_cProjectData, this.m_cUpdateLog, this.m_dSelectedDate.Value, this.m_cProjectData.SubProjectNo, sSurveyorName, sSurveyorProfile, sMachineName);

                    if (v_bSaveChanges == true) {this.m_cProjectData = cApply.cProjectData;}
                    this.m_cUpdateLog.AddRange(cApply.cUpdates);                  
                   
                                    
                }
                else if (this.m_bUnConfirmSurvey == true) //v1.0.2 - UnConfirm survey
                {

                    if (v_bSaveChanges == true) {this.m_cProjectData.MxmConfirmedAppointmentIndicator = (int)cSettings.YesNoBaseEnum.No;}
                    this.m_cUpdateLog = cSettings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmConfirmedAppointmentIndicator", ((int)cSettings.YesNoBaseEnum.No).ToString());
                
                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Check that notes are okay.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> AreNotesOK()
        {

            try
            {

                //Remove unwanted spaces.
                this.txtOtherNotes.Text = this.txtOtherNotes.Text.Trim();

                //Retrieve selected item.
                string sItem = this.cmbNotes.SelectedItem.ToString();
                if (sItem.Equals(cSettings.p_sNoteIt_Other) == true)
                {

                    //If not text entered prompt user to enter some.
                    if (this.txtOtherNotes.Text.Length == 0)
                    {
                        await cSettings.DisplayMessage("You have selected '" + cSettings.p_sNoteIt_Other + "' in the notes selection, please enter some text in the notes box or select 'None'", "Notes Issue.");
                        this.txtOtherNotes.Focus(FocusState.Programmatic);
                        return false;

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
        /// v1.0.2 - UnConfirm survey.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUnConfirmSurvey_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                this.m_bUnConfirmSurvey = true;
                this.btnUnConfirmSurvey.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

              
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

                await this.LogChangesForUpdateTable(false);

                if (this.m_cUpdateLog.Count > 0 || this.m_bAreThereNoteChanges == true)
                {

                    cSettings.YesNoCancel mResponse = await cSettings.PromptForUnSavedChanges();
                    if (mResponse == cSettings.YesNoCancel.Yes)
                    {
                        bool bSaveOK = await this.SaveSubProject();
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
                    this.navigationHelper.GoBack();
                }
              

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

    }
}
