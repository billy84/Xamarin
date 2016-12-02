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

namespace Anglian.Views
{
    public partial class DateSearchFilterPage : ContentPage
    {
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
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "erase")
            });
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.PopulateDropDown();
        }

        private void PopulateDropDown()
        {
            cmbProjectNo.Items.Add(Settings.p_sAnyStatus);
            List<ProjectNo> lsProjectNos = Main.p_cDataAccess.GetProjectNos();
            foreach (ProjectNo lsProjectNo in lsProjectNos)
            {
                this.cmbProjectNo.Items.Add(lsProjectNo.PRojectNo);
            }
            lsProjectNos = null;
            this.cmbProjectNo.SelectedIndex = 0;
            string cmbItem = Settings.p_sAnyStatus;
            this.cmbInstallStatus.Items.Add(cmbItem);
            List<cBaseEnumsTable> oInstalls = Main.p_cDataAccess.GetEnumsForField("MXM1002INSTALLSTATUS");
            foreach (cBaseEnumsTable oInstall in oInstalls)
            {
                this.cmbInstallStatus.Items.Add(oInstall.EnumName);
            }
            this.cmbInstallStatus.SelectedIndex = 0;

            this.cmbProgressStatus.Items.Add(Settings.p_sAnyStatus);
            List<cBaseEnumsTable> oProgresses = Main.p_cDataAccess.GetEnumsForField("Mxm1002ProgressStatus");
            foreach (cBaseEnumsTable oProgress in oProgresses)
            {
                this.cmbProgressStatus.Items.Add(oProgress.EnumName);
            }

            cmbProgressStatus.SelectedIndex = 0;
            cmbSurveyedStatus.Items.Add(Settings.p_sAnyStatus);
            cmbSurveyedStatus.Items.Add(Settings.p_sSurveyedStatus_NotSurveyed);
            cmbSurveyedStatus.Items.Add(Settings.p_sSurveyedStatus_SurveyedOnSite);
            cmbSurveyedStatus.Items.Add(Settings.p_sSurveyedStatus_SurveyedTrans);
            cmbSurveyedStatus.SelectedIndex = 1;

            cmbConfirmed.Items.Add(Settings.p_sAnyStatus);
            cmbDateCompare.Items.Add(Settings.p_sDateCompare_EqualTo);
            cmbDateCompare.Items.Add(Settings.p_sDateCompare_GreaterThan);
            cmbDateCompare.Items.Add(Settings.p_sDateCompare_LessThan);
            cmbDateCompare.SelectedIndex = 0;

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
            string sProjectNo = String.Empty;
            string sSubProjectNo = String.Empty;
            if (cmbProjectNo.SelectedIndex == -1)
            {
                sProjectNo = Settings.p_sAnyStatus;
            }
            string sValue = (String)cmbProjectNo.Items[cmbProjectNo.SelectedIndex];
            if (sValue != Settings.p_sAnyStatus)
            {
                sProjectNo = sValue;
            }
            sSubProjectNo = txtSubProjectNoFilter.Text;
            Int32 iInstallStatus = -1;
            iInstallStatus = cmbInstallStatus.SelectedIndex;
            Int32 iProgressStatus = -1;
            iProgressStatus = cmbProgressStatus.SelectedIndex;

            DateTime? dSurveyDate = null;
            dSurveyDate = cmbTimePicker.Date;

            string sSurveyed = cmbSurveyedStatus.Items[cmbSurveyedStatus.SelectedIndex];
            Int32 iConfirmed = -1;
            iConfirmed = cmbConfirmed.SelectedIndex;
            string sDateComparison = cmbDateCompare.Items[cmbDateCompare.SelectedIndex];

            int iInstall_Awaiting = Convert.ToInt32(DependencyService.Get<IMain>().GetAppResourceValue("InstallStatus_AwaitingSurvey"));
            int iInstall_Cancel = Convert.ToInt32(DependencyService.Get<IMain>().GetAppResourceValue("InstallStatus_SurveyCancelled"));

            string vDeliveryStreet = string.Empty;
            if (txtDeliveryStreet.Text != null)
                vDeliveryStreet = txtDeliveryStreet.Text;
            string vPostCode = string.Empty;
            if (txtPostCode.Text != null)
                vPostCode = txtPostCode.Text;

            List<SurveyDatesResult> cResults = Main.p_cDataAccess.SearchSurveyDates(
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


            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new DateSearchResultPage(cResults)));
            //await this.Navigation.PushAsync(new DateSearchResultView(items));
        }
    }
}
