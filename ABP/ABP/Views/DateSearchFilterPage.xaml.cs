using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ABP.WcfProxys;
using ABP.Models;
using ABP.TableModels;

namespace ABP.Views
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
            //var tapSearchBtn = new TapGestureRecognizer();
            //tapSearchBtn.Tapped += SearchBtn_Tapped;
            //SearchBtn.GestureRecognizers.Add(tapSearchBtn);
            
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.PopulateDropDown();
        }

        private void PopulateDropDown()
        {
            cmbProjectNo.Items.Add(cSettings.p_sAnyStatus);
            List<cProjectNo> lsProjectNos = cMain.p_cDataAccess.GetProjectNos();
            foreach (cProjectNo lsProjectNo in lsProjectNos)
            {
                this.cmbProjectNo.Items.Add(lsProjectNo.ProjectNo);
            }
            lsProjectNos = null;
            this.cmbProjectNo.SelectedIndex = 0;
            string cmbItem = cSettings.p_sAnyStatus;
            this.cmbInstallStatus.Items.Add(cmbItem);
            List<cBaseEnumsTable> oInstalls = cMain.p_cDataAccess.GetEnumsForField("MXM1002INSTALLSTATUS");
            foreach (cBaseEnumsTable oInstall in oInstalls)
            {
                this.cmbInstallStatus.Items.Add(oInstall.EnumName);
            }
            this.cmbInstallStatus.SelectedIndex = 0;

            this.cmbProgressStatus.Items.Add(cSettings.p_sAnyStatus);
            List<cBaseEnumsTable> oProgresses = cMain.p_cDataAccess.GetEnumsForField("Mxm1002ProgressStatus");
            foreach (cBaseEnumsTable oProgress in oProgresses)
            {
                this.cmbProgressStatus.Items.Add(oProgress.EnumName);
            }

            this.cmbProgressStatus.SelectedIndex = 0;
            this.cmbSurveyedStatus.Items.Add(cSettings.p_sAnyStatus);
            this.cmbSurveyedStatus.Items.Add(cSettings.p_sSurveyedStatus_NotSurveyed);
            this.cmbSurveyedStatus.Items.Add(cSettings.p_sSurveyedStatus_SurveyedOnSite);
            this.cmbSurveyedStatus.Items.Add(cSettings.p_sSurveyedStatus_SurveyedTrans);
            this.cmbSurveyedStatus.SelectedIndex = 1;

            this.cmbConfirmed.Items.Add(cSettings.p_sAnyStatus);

            List<cBaseEnumsTable> oConfirms = cMain.p_cDataAccess.GetEnumsForField("MxmConfirmedAppointmentIndicator");
            foreach (cBaseEnumsTable oConfirm in oConfirms)
            {
                this.cmbConfirmed.Items.Add(oConfirm.EnumName);
            }
            this.cmbConfirmed.SelectedIndex = 0;
            //this.cmbTimePicker.
        }
        private void SearchFilterBtn_Tapped()
        {
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new DateSearchResultPage()));
            //await this.Navigation.PushAsync(new DateSearchResultView(items));
        }
    }
}
