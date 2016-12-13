using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Models;
using Anglian.Classes;
using Anglian.Service;
using Anglian.Engine;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace Anglian.Views
{
    public partial class DateSearchFilterPage : ContentPage
    {
        private ObservableCollection<CmbItem> m_InstallStatus = new ObservableCollection<CmbItem>();
        private ObservableCollection<CmbItem> m_ProgressStatus = new ObservableCollection<CmbItem>();
        public DateSearchFilterPage()
        {
            InitializeComponent();
            this.Title = "Survey Dates Search Filters";
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Search",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "find"),
                Command = new Command(() => SearchFilterBtn_Tapped())
            });
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Reset",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "erase"),
                Command = new Command(() => ResetFilterBtn_Tapped())
            });
            txtSubProjectNoFilter.IsEnabled = false;
            cmbProjectNoFilter.SelectedIndexChanged += CmbProjectNoFilter_SelectedIndexChanged;
        }

        private void CmbProjectNoFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string sFilter = cmbProjectNoFilter.Items[cmbProjectNoFilter.SelectedIndex];
                if (sFilter.Equals(Settings.p_sProjectNoFilter_ProjectNo) == true)
                {
                    txtSubProjectNoFilter.IsEnabled = false;
                    cmbProjectNo.IsEnabled = true;
                }
                else if (sFilter.Equals(Settings.p_sProjectNoFilter_SubProjectNo) == true)
                {
                    txtSubProjectNoFilter.IsEnabled = true;
                    cmbProjectNo.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {

            }
            //throw new NotImplementedException();
        }

        private void ResetFilterBtn_Tapped()
        {
            OnAppearing();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            cmbProjectNo.Items.Clear();
            cmbProjectNoFilter.Items.Clear();
            cmbInstallStatus.Items.Clear();
            cmbProgressStatus.Items.Clear();
            cmbSurveyedStatus.Items.Clear();
            cmbDateCompare.Items.Clear();
            cmbConfirmed.Items.Clear();
            PopulateDropDown();
        }

        private void PopulateDropDown()
        {
            // ** Project numbers drop down.
            this.cmbProjectNo.Items.Add(Settings.p_sAnyStatus);
            this.cmbProjectNoFilter.Items.Add(Settings.p_sProjectNoFilter_ProjectNo);
            this.cmbProjectNoFilter.Items.Add(Settings.p_sProjectNoFilter_SubProjectNo);
            this.cmbProjectNoFilter.SelectedIndex = 0;

            List<cProjectNo> lsProjectNos = Main.p_cDataAccess.GetProjectNos();
            foreach (cProjectNo lsProjectNo in lsProjectNos)
            {

                if (lsProjectNo.ProjectNo.Length > 0)
                {
                    this.cmbProjectNo.Items.Add(lsProjectNo.ProjectNo);

                }
            }
            lsProjectNos = null;
            cmbProjectNo.SelectedIndex = 0; //Make Any Status Selection Visible

            // ** Install status drop down.

            m_InstallStatus.Add(new CmbItem { Content = Settings.p_sAnyStatus, Tag = "-1" });
            this.cmbInstallStatus.Items.Add(Settings.p_sAnyStatus);

            List<cBaseEnumsTable> oInstalls = Main.p_cDataAccess.GetEnumsForField("MXM1002INSTALLSTATUS");
            foreach (cBaseEnumsTable oInstall in oInstalls)
            {
                m_InstallStatus.Add(new CmbItem { Content = oInstall.EnumName, Tag = oInstall.EnumValue });
                this.cmbInstallStatus.Items.Add(oInstall.EnumName);
            }
            this.cmbInstallStatus.SelectedIndex = 0; //Make Any the default selection

            //v1.0.14 - Progress status drop down.
            m_ProgressStatus.Add(new CmbItem { Content = Settings.p_sAnyStatus, Tag = "-1" });
            this.cmbProgressStatus.Items.Add(Settings.p_sAnyStatus);

            List<cBaseEnumsTable> oProgresses = Main.p_cDataAccess.GetEnumsForField("Mxm1002ProgressStatus");
            foreach (cBaseEnumsTable oProgress in oProgresses)
            {
                m_ProgressStatus.Add(new CmbItem { Content = oProgress.EnumName, Tag = oProgress.EnumValue });
                this.cmbProgressStatus.Items.Add(oProgress.EnumName);
            }

            this.cmbProgressStatus.SelectedIndex = 0; //Make Any the default selection

            // ** Surveyed Status Drop Down
            this.cmbSurveyedStatus.Items.Add(Settings.p_sAnyStatus); //v1.0.1 - Include "Any" status.
            this.cmbSurveyedStatus.Items.Add(Settings.p_sSurveyedStatus_NotSurveyed);
            this.cmbSurveyedStatus.Items.Add(Settings.p_sSurveyedStatus_SurveyedOnSite);
            this.cmbSurveyedStatus.Items.Add(Settings.p_sSurveyedStatus_SurveyedTrans);
            this.cmbSurveyedStatus.SelectedIndex = 1; //Default on Not Surveyed

            this.cmbConfirmed.Items.Add(Settings.p_sAnyStatus);

            //Data comparison drop down.
            this.cmbDateCompare.Items.Add(Settings.p_sDateCompare_EqualTo);
            this.cmbDateCompare.Items.Add(Settings.p_sDateCompare_GreaterThan);
            this.cmbDateCompare.Items.Add(Settings.p_sDateCompare_LessThan);
            this.cmbDateCompare.SelectedIndex = 0;

            List<cBaseEnumsTable> oConfirms = Main.p_cDataAccess.GetEnumsForField("MxmConfirmedAppointmentIndicator");
            foreach (cBaseEnumsTable oConfirm in oConfirms)
            {
                this.cmbConfirmed.Items.Add(oConfirm.EnumName);
            }
            this.cmbConfirmed.SelectedIndex = 0;
            //this.cmbTimePicker.
        }

        private void SearchFilterBtn_Tapped()
        {
            DisplaySearchResults();
        }

        /// <summary>
        /// Display search results
        /// </summary>
        private void DisplaySearchResults()
        {

            try
            {
                //Extract Project Number.
                string sProjectNo = string.Empty;
                string sSubProjectNo = string.Empty; //v1.0.1
                if (cmbProjectNoFilter.Items[cmbProjectNoFilter.SelectedIndex].Equals(Settings.p_sProjectNoFilter_ProjectNo) == true)
                {
                    string sValue = cmbProjectNo.Items[cmbProjectNo.SelectedIndex];
                    if (sValue != Settings.p_sAnyStatus)
                    {
                        sProjectNo = sValue;

                    }
                }
                else if (cmbProjectNoFilter.Items[cmbProjectNoFilter.SelectedIndex].Equals(Settings.p_sProjectNoFilter_SubProjectNo) == true)
                {
                    sSubProjectNo = txtSubProjectNoFilter.Text;
                }

                //Install status
                Int32 iInstallStatus = -1;
                List<CmbItem> cbiItems = m_InstallStatus.Where(item => item.Content == cmbInstallStatus.Items[cmbInstallStatus.SelectedIndex]).ToList();
                string sTagValue = cbiItems[0].Tag.ToString();
                if (int.TryParse(sTagValue, out iInstallStatus) == true)
                {

                }

                //v1.0.14 Progress status
                Int32 iProgressStatus = -1;
                cbiItems = m_ProgressStatus.Where(item => item.Content == cmbProgressStatus.Items[cmbProgressStatus.SelectedIndex]).ToList();
                sTagValue = cbiItems[0].Tag.ToString();
                if (int.TryParse(sTagValue, out iProgressStatus) == true)
                {

                }

                //Surveyed Date.
                DateTime? dSurveyDate = null;
                if (chkUseSurveyPlanDate.IsToggled == true)
                {
                    dSurveyDate = this.dtPicker.Date;
                }

                //Surveyed
                string sSurveyed = this.cmbSurveyedStatus.Items[cmbSurveyedStatus.SelectedIndex];

                //Confirmed
                Int32 iConfirmed = -1;
                iConfirmed = this.cmbConfirmed.SelectedIndex;

                //Date comparison
                string sDateComparison = this.cmbDateCompare.Items[cmbDateCompare.SelectedIndex];


                int iInstall_Awaiting = Convert.ToInt32(DependencyService.Get<IMain>().GetAppResourceValue("InstallStatus_AwaitingSurvey"));
                int iInstall_Cancel = Convert.ToInt32(DependencyService.Get<IMain>().GetAppResourceValue("InstallStatus_SurveyCancelled"));

                string vDeliveryStreet = string.Empty;
                if (this.txtDeliveryStreet.Text != null)
                    vDeliveryStreet = this.txtDeliveryStreet.Text;
                string vPostCode = string.Empty;
                if (this.txtPostCode.Text != null)
                    vPostCode = this.txtPostCode.Text;

                //Process Search
                ObservableCollection<SurveyDatesResult> cResults = Main.p_cDataAccess.SearchSurveyDates(
                sProjectNo,
                vDeliveryStreet,
                vPostCode,
                iInstallStatus,
                iProgressStatus,
                dSurveyDate,
                sDateComparison,
                sSurveyed,
                iConfirmed,
                false,
                chkShowAllStatus.IsToggled,
                chkShowAllProgressStatus.IsToggled,
                sSubProjectNo,
                iInstall_Awaiting,
                iInstall_Cancel
                );

                int iUpdates = 0;
                foreach (SurveyDatesResult cResult in cResults)
                {

                    //cResult.ScreenWidth = this.lvResults.ActualWidth;
                    //cResult.StreetWidth = this.txtDeliveryStreet.ActualWidth;
                    //cResult.InstallWidth = this.cmbInstallStatus.ActualWidth - 10;
                    //cResult.ProgressWidth = this.cmbProgressStatus.ActualWidth - 10;
                    //cResult.PostcodeWidth = this.txtPostcode.ActualWidth;

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
                    cResult.Confirmed = Settings.p_sConfirmedStatus_No;
                    if (cResult.MxmConfirmedAppointmentIndicator.HasValue == true)
                    {
                        if (cResult.MxmConfirmedAppointmentIndicator.Value == (int)Settings.YesNoBaseEnum.Yes)
                        {
                            cResult.Confirmed = Settings.p_sConfirmedStatus_Yes;
                        }

                    }

                    //Display appropriate surveyed status.
                    cResult.SurveyedStatus = Settings.p_sSurveyedStatus_NotSurveyed;
                    if (cResult.MXM1002TrfDate.HasValue == true)
                    {

                        if (cResult.Mxm1002InstallStatus == iInstall_Awaiting || cResult.Mxm1002InstallStatus == iInstall_Cancel)
                        {
                            cResult.SurveyedStatus = Settings.p_sSurveyedStatus_SurveyedOnSite;

                        }

                    }

                    if (cResult.Mxm1002InstallStatus != iInstall_Awaiting && cResult.Mxm1002InstallStatus != iInstall_Cancel)
                    {
                        cResult.SurveyedStatus = Settings.p_sSurveyedStatus_SurveyedTrans;

                    }


                    //Remove unwanted new lines.
                    cResult.DeliveryStreet = Main.RemoveNewLinesFromString(cResult.DeliveryStreet);

                    //v1.0.1 - Update tool tip text
                    cResult.ToolTipText = "Status: " + cResult.StatusName + Environment.NewLine + "Progress Status: " + cResult.ProgressStatusName;


                    //v1.0.1 - Display formatted survey date - time
                    if (cResult.EndDateTime.HasValue == true)
                    {
                        cResult.SurveyDisplayDateTime = Main.ReturnDisplayDate(cResult.EndDateTime.Value) + " " + Main.ReturnDisplayTime(cResult.EndDateTime.Value);
                    }


                    //v1.0.15 - Set background color if project status is "On Hold"
                    if (cResult.Status == Settings.p_iProjectStatus_OnHold)
                    {
                        cResult.BackgroundColour = Settings.p_sSurvey_ListView_OnHold_Background;
                    }
                    else
                    {
                        cResult.BackgroundColour = Settings.p_sSurvey_ListView_Normal_Background;
                    }
                }
                Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new DateSearchResultPage(cResults)));
                //await this.Navigation.PushAsync(new DateSearchResultView(items));
            }
            catch (Exception ex)
            {
                //Main.ReportError(ex, Main.GetCallerMethodName(), string.Empty);
            }
        }
    }
}
