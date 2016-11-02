using ANG_ABP_SURVEYOR_APP.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using ANG_ABP_SURVEYOR_APP_CLASS.Model;
using ANG_ABP_SURVEYOR_APP_CLASS.Classes;
using ANG_ABP_SURVEYOR_APP_CLASS;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ANG_ABP_SURVEYOR_APP.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SurveyInputOptionPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// v1.0.1 - Store the project notes.
        /// </summary>
        private List<cProjectNotesTable> m_cProjectNotes = null;

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
        /// v1.0.19 - List of failed survey reasons.
        /// </summary>
        public List<cFailedSurveyReasonsTable> p_lsFailedReasons = null;

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


        public SurveyInputOptionPage()
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
        private void SurveyInputOptionPage_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {

                //Update the identification
                this.tbSubProjectNo.Text = this.m_cProject.SubProjectNo;
                this.tbStreetName.Text = this.m_cProject.DeliveryStreet;
                

                //Hide failed grid until we need it.
                this.gdFailed.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                //Populate the failed visit drop down.
                this.PopulateFailedDropDown();

                //Update failed visit attempt button.
                this.UpdateAttemptedSurveyDateButton(DateTime.Now);

                //Page has now loaded.
                this.m_bPageLoaded = true;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }


        /// <summary>
        /// Populate the failed survey drop down list.
        /// </summary>
        private void PopulateFailedDropDown()
        {

            try
            {

                this.p_lsFailedReasons = cMain.p_cDataAccess.FetchAllSurveyFailedReasons();


                //Add please choose to the list.
                this.cmbFailedSurvey.Items.Add(cSettings.p_sPleaseChoose);

                //v1.0.19
                foreach (cFailedSurveyReasonsTable oReason in this.p_lsFailedReasons)
                {
                    this.cmbFailedSurvey.Items.Add(oReason.Description);

                }
                          

                //Select first item in list.
                this.cmbFailedSurvey.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.19 - Return reason object for selected reason.
        /// </summary>
        /// <returns></returns>
        private cFailedSurveyReasonsTable ReturnReasonObject()
        {
            try
            {

                string sItem = this.cmbFailedSurvey.SelectedItem.ToString();
                foreach (cFailedSurveyReasonsTable oReason in this.p_lsFailedReasons)
                {

                    if (sItem.Equals(oReason.Description) == true)
                    {
                        return oReason;
                    }

                }

                return null;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return null;

            }

        }

        /// <summary>
        /// Update survey failed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnUpdateSurveyFailed_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                cFailedSurveyReasonsTable oReason = this.ReturnReasonObject();

                //Remove blank spaces from comment box.
                this.txtFailedComment.Text = this.txtFailedComment.Text.Trim();

                string sItem = this.cmbFailedSurvey.SelectedItem.ToString();
                if (sItem.Equals(cSettings.p_sPleaseChoose) == true)
                {

                    await cSettings.DisplayMessage("Please select a reason for the failed visit.", "Failed Visit Reason Required.");
                    this.cmbFailedSurvey.Focus(FocusState.Programmatic);
                    return;

                }
                else if (oReason.MandatoryNote == true)
                {

                    //If other selected then a comment is required.
                    if (this.txtFailedComment.Text.Length == 0)
                    {

                        await cSettings.DisplayMessage("You need to enter a comment when you select a failed visit reason of " + cSettings.p_sNoteIt_Other + ".", "Comment Required.");
                        this.txtFailedComment.Focus(FocusState.Programmatic);
                        return;

                    }

                }

                this.m_cProject.MxmConfirmedAppointmentIndicator = (int)cSettings.YesNoBaseEnum.No; //v1.0.1
                cMain.p_cDataAccess.AddToUpdateTable(this.m_cProject.SubProjectNo,"MxmConfirmedAppointmentIndicator", ((int)cSettings.YesNoBaseEnum.No).ToString());

                //v1.0.19 - If progress status specified we need to update.
                if (oReason.ProgressStatus > -1)
                {
                    
                    this.m_cProject.Mxm1002ProgressStatus = oReason.ProgressStatus;
                    this.m_cProject.ProgressStatusName = cMain.p_cDataAccess.GetEnumValueName("ProjTable", "Mxm1002ProgressStatus", oReason.ProgressStatus);

                    cMain.p_cDataAccess.AddToUpdateTable(this.m_cProject.SubProjectNo, "Mxm1002ProgressStatus", oReason.ProgressStatus.ToString());
                }
                
                //Save sub project.
                bool bSaveOK = cMain.p_cDataAccess.UpdateProjectTable(this.m_cProject);
                if (bSaveOK == false)
                {
                    await cSettings.DisplayMessage("An error occurred when trying to save, please try again.", "Error When Saving.");
                    return;
                }

                string sNewNote = sItem + ": " + this.txtFailedComment.Text;

                DateTime dtFailedDate = DateTime.Now;
                DateTime.TryParse(this.btnUpdateSurveyAttemptDate.Tag.ToString(), out dtFailedDate);

                //v1.0.1 - Return note object
                cProjectNotesTable cNote = await cSettings.ReturnNoteObject(this.m_cProject.SubProjectNo, sNewNote, dtFailedDate, cSettings.p_sProjectNoteType_SurveyFailed);

                //v1.0.1 - Save sub project notes.
                bSaveOK = cMain.p_cDataAccess.SaveSubProjectNote(cNote);
                if (bSaveOK == false)
                {
                    await cSettings.DisplayMessage("An error occurred when trying to save, please try again.", "Error When Saving.");
                    return;
                }


                //Go back to previous page.
                navigationHelper.GoBack();


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);


            }

        }

        /// <summary>
        /// User clicked survey has failed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSurveyFailed_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                this.gdFailed.Visibility = Windows.UI.Xaml.Visibility.Visible;
                cmbFailedSurvey.Focus(FocusState.Programmatic);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Selection changed on failed visit reasons combo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbFailedSurvey_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {

                if (this.m_bPageLoaded == true)
                {
                    this.txtFailedComment.Focus(FocusState.Programmatic);

                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Survey successful.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSurveySuccessfull_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //Redirect to edit page.
                this.Frame.Navigate(typeof(SurveyInputEditPage), this.m_cProject.SubProjectNo);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Update survey failed dates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateSurveyAttemptDate_Click(object sender, RoutedEventArgs e)
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
        /// Use fail date.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUseFailedDate_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                this.UpdateAttemptedSurveyDateButton(this.dtPicker.Value);
                this.puDatePicker.IsOpen = false;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Update attempted survey date button.
        /// </summary>
        /// <param name="v_dVisitDate"></param>
        private void UpdateAttemptedSurveyDateButton(DateTime v_dVisitDate)
        {

            try
            {

                this.btnUpdateSurveyAttemptDate.Content = "Attempted Survey Date: " + cMain.ReturnDisplayDate(v_dVisitDate, string.Empty);
                this.btnUpdateSurveyAttemptDate.Tag = v_dVisitDate;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }
    }
}
