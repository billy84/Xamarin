using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Models;
using Anglian.Classes;
using Xamarin.Forms;
using Anglian.Engine;
using Anglian.Service;

namespace Anglian.Views
{
    public partial class DateSearchResultPage : ContentPage
    {
        private ObservableCollection<SurveyDatesResult> results = new ObservableCollection<SurveyDatesResult>();

        public DateSearchResultPage(ObservableCollection<SurveyDatesResult> cResults)
        {
            InitializeComponent();
            cmbTimePicker.SelectedIndexChanged += CmbTimePicker_SelectedIndexChanged;
            this.Title = "Survey Dates Search Results";
            this.cmbTimePicker.Items.Add(Settings.p_sPleaseChoose);
            this.cmbTimePicker.Items.Add(Settings.p_sTime_AM);
            this.cmbTimePicker.Items.Add(Settings.p_sTime_PM);
            this.cmbTimePicker.Items.Add(Settings.p_sTime_Specific);
            this.cmbTimePicker.SelectedIndex = 0;

            ////Default time picker to hidden.
            string str = nameof(tpSurveyTime);
            this.tpSurveyTime.IsEnabled = false;

            this.ToolbarItems.Add(new ToolbarItem() { Text = "Select All", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "check"), Command = new Command(() => SelectAllBtn_Tapped()) });
            this.ToolbarItems.Add(new ToolbarItem() { Text = "De-Select All", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "uncheck"), Command = new Command(() => De_SelectAllBtn_Tapped()) });
            this.ToolbarItems.Add(new ToolbarItem() { Text = "Set Survey Date", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "date"), Command = new Command(() => btnApplySurveyDate_Click()) });
            this.ToolbarItems.Add(new ToolbarItem() { Text = "Un-Confirm", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "undo"), Command = new Command(() => btnUnConfirmSurveys_Click()) });
            populateResultList(cResults);
        }

        private void CmbTimePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                string sTime = String.Empty;

                switch (this.cmbTimePicker.Items[cmbTimePicker.SelectedIndex])
                {
                    case Settings.p_sPleaseChoose:
                        this.tpSurveyTime.IsEnabled = false;
                        //this.tpSurveyTime.Visibility = Windows.UI.Xaml.Visibility.Collapsed; //v1.0.1
                        break;

                    case Settings.p_sTime_AM:
                        this.tpSurveyTime.IsEnabled = false;
                        //this.tpSurveyTime.Visibility = Windows.UI.Xaml.Visibility.Collapsed; //v1.0.1
                        sTime = DependencyService.Get<IMain>().GetAppResourceValue("AM_TIME");
                        break;

                    case Settings.p_sTime_PM:
                        this.tpSurveyTime.IsEnabled = false;
                        //this.tpSurveyTime.Visibility = Windows.UI.Xaml.Visibility.Collapsed; //v1.0.1
                        sTime = DependencyService.Get<IMain>().GetAppResourceValue("PM_TIME");
                        break;

                    case Settings.p_sTime_Specific:
                        this.tpSurveyTime.IsEnabled = true;
                        //this.tpSurveyTime.Visibility = Windows.UI.Xaml.Visibility.Visible; //v1.0.1
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
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
            //throw new NotImplementedException();
        }
        /// <summary>
        /// v1.0.2 - Un Confirm surveys.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnUnConfirmSurveys_Click()
        {


            try
            {

                //Nothing selected, let user know.
                if (GetSelection().Count == 0)
                {

                    await DisplayAlert("Selection Required", "You need to select at least one sub-project before you can continue.", "OK");
                    this.lvResults.Focus();
                    return;


                }

                //Remove sub-projects that do not have confirmation flags.
                List<string> lsToUnConfirm = new List<string>();
                foreach (SurveyDatesResult cSurvey in GetSelection())
                {

                    if (cSurvey.MxmConfirmedAppointmentIndicator.HasValue == true)
                    {

                        if (cSurvey.MxmConfirmedAppointmentIndicator.Value == (int)Settings.YesNoBaseEnum.Yes)
                        {

                            //Record sub project we are going to un-confirm
                            lsToUnConfirm.Add(cSurvey.SubProjectNo);

                        }

                    }

                }

                //If no sub project are confirmed let user know and quit.
                if (lsToUnConfirm.Count == 0)
                {

                    await DisplayAlert("No sub-projects confirmed.", "None of the sub-projects selected have confirmed appointments.", "OK");
                    this.lvResults.Focus();
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
                bool cAnswer = await DisplayAlert("Confirm Action", sbMessage.ToString(), "Yes", "No");
                if (cAnswer == true)
                {


                    cProjectTable cSubProjectData = null;
                    foreach (string sProject in lsToUnConfirm)
                    {

                        //Fetch sub project data
                        cSubProjectData = Main.p_cDataAccess.GetSubProjectProjectData(sProject);

                        //Update confirmed flag.
                        cSubProjectData.MxmConfirmedAppointmentIndicator = (int)Settings.YesNoBaseEnum.No;

                        //Save back to database.
                        Main.p_cDataAccess.UpdateSubProjectData(cSubProjectData);

                        //Add to log table.
                        Main.p_cDataAccess.AddToUpdateTable(sProject, "MxmConfirmedAppointmentIndicator", ((int)Settings.YesNoBaseEnum.No).ToString());


                    }

                }

                //Refresh screen
                //this.DisplaySearchResults();
                await DisplayAlert("Information", "You have to search again to show changed state", "OK");

                //Reselect
                //this.ReselectSubProject(lsToUnConfirm);

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }
        /// <summary>
        /// Apply survey date to selected projects.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnApplySurveyDate_Click()
        {

            try
            {

                //Make sure a time has been picked.
                string sTimePicked = this.cmbTimePicker.Items[cmbTimePicker.SelectedIndex];
                if (sTimePicked == Settings.p_sPleaseChoose)
                {
                    await DisplayAlert("Time selection required.", "Please select a time option from the drop down.", "OK");
                    //this.SetTheSurveyDatesPopup.IsOpen = true;
                    this.cmbTimePicker.Focus();
                    return;

                }

                //Record all the sub project numbers.
                List<String> lsSubProjectNos = new List<String>();

                //v1.0.2 - List of string containing sub project that will not display on front screen
                List<string> lsSubProjectNoDisplay = new List<string>();
                bool bWillItDisplay = false;

                foreach (SurveyDatesResult lviItem in GetSelection())
                {
                    lsSubProjectNos.Add(lviItem.SubProjectNo);

                    //v1.0.2 - Check if sub projects statuses will allow it to display on the front screen.
                    bWillItDisplay = Main.WillSubProjectDisplayOnFrontScreen(lviItem.Mxm1002InstallStatus);
                    if (bWillItDisplay == false)
                    {
                        lsSubProjectNoDisplay.Add(lviItem.SubProjectNo);

                    }

                }

                //v1.0.2 - Warn user if sub project will not display on front screen as status incorrect.                              
                if (lsSubProjectNoDisplay.Count > 0)
                {

                    string sMessage = Main.ReturnSubProjectWillNotDisplayOnFrontScreenMessage(lsSubProjectNoDisplay);
                    await DisplayAlert("Warning", sMessage, "OK");

                }

                //Place date and time in strings.
                String sDate = this.dtPickerSurveyDate.Date.ToString("dd/MM/yyyy");
                String sTime = this.tpSurveyTime.Time.Hours.ToString().PadLeft(
                    2, 
                    '0') + ":" + this.tpSurveyTime.Time.Minutes.ToString().PadLeft(2, '0') + ":" + this.tpSurveyTime.Time.Seconds.ToString().PadLeft(2, 
                    '0');

                //Put date and time together.
                DateTime dSurveyDate = Convert.ToDateTime(sDate + " " + sTime);

                bool bValid = Main.p_cDataAccess.ApplySurveyDatesToSubProjects(lsSubProjectNos, dSurveyDate);
                if (bValid == true)
                {
                    await DisplayAlert("Information", "You have to search again to show changed state", "OK");
                    //this.DisplaySearchResults();
                    //this.SetTheSurveyDatesPopup.IsOpen = false;
                }
                else
                {
                    await DisplayAlert("Cannot Apply Survey Date.", "A problem occurred when trying to apply the survey dates, please try again.", "OK");

                }


            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }
        private void SelectAllBtn_Tapped()
        {
            foreach (var vResult in results)
            {
                vResult.IsEnabled = true;
            }
            //this.ResultList.ItemsSource = results;
        }

        private void De_SelectAllBtn_Tapped()
        {
            foreach (var vResult in results)
            {
                vResult.IsEnabled = false;
            }
            //this.ResultList.ItemsSource = this.results;
        }

        private void populateResultList(ObservableCollection<SurveyDatesResult> Results)
        {
            this.results = Results;

            foreach (SurveyDatesResult cResult in this.results)
            {
                cResult.DeliveryStreet = cResult.DeliveryStreet;
                cResult.DlvZipCode = cResult.DlvZipCode + " - " + cResult.SurveyDisplayDateTime;
                cResult.ProgressStatusName = cResult.ProgressStatusName;
                cResult.Confirmed = cResult.Confirmed;
                cResult.IsEnabled = false;
            }
            this.lvResults.ItemsSource = this.results;
        }
        public ObservableCollection<SurveyDatesResult> GetSelection()
        {
            ObservableCollection<SurveyDatesResult> selectedItems = new ObservableCollection<SurveyDatesResult>();
            foreach (SurveyDatesResult cProject in results)
            {
                if (cProject.IsEnabled == true)
                    selectedItems.Add(cProject);
            }
            return selectedItems;
        }

    }
}
