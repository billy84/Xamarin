using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Classes;
using Anglian.Models;
using Anglian.Service;
using Xamarin.Forms;
using Anglian.Engine;
namespace Anglian.Views
{
    public class CmbItem
    {
        public string Content { get; set; }
        public object Tag { get; set; }
    } 
    public partial class ProjectSearchPage : ContentPage
    {
        private ObservableCollection<CmbItem> m_InstallStatus = new ObservableCollection<CmbItem>();
        private ObservableCollection<CmbItem> m_ProgressStatus = new ObservableCollection<CmbItem>();
        private ObservableCollection<CmbItem> m_Surveyors = new ObservableCollection<CmbItem>();
        private bool m_bSurveyedMode = false;
        public ProjectSearchPage()
        {
            InitializeComponent();
            Title = "Project Search";
            btnSearch.HeightRequest = 50;
            txtDeliveryStreet.Text = string.Empty;
            txtPostcode.Text = string.Empty;
            PopulateDropDowns();
        }
        /// <summary>
        /// Populate drop downs.
        /// </summary>
        private void PopulateDropDowns()
        {

            try
            {

                // ** Project numbers drop down.
                this.cmbProjectNo.Items.Add(Settings.p_sAnyStatus);

                List<cProjectNo> lsProjectNos = Main.p_cDataAccess.GetProjectNos();
                foreach (cProjectNo lsProjectNo in lsProjectNos)
                {
                    if (lsProjectNo.ProjectNo.Length > 0)
                    {
                        this.cmbProjectNo.Items.Add(lsProjectNo.ProjectNo);

                    }

                }
                lsProjectNos = null;
                this.cmbProjectNo.SelectedIndex = 0; //Make cMain.p_sAnyStatus selection visible.


                // ** Install status drop down.
                m_InstallStatus.Add(new CmbItem { Content = Settings.p_sAnyStatus, Tag = "-1" });
                this.cmbInstallStatus.Items.Add(Settings.p_sAnyStatus);

                List<cBaseEnumsTable> oInstalls = Main.p_cDataAccess.GetEnumsForField("MXM1002INSTALLSTATUS");
                foreach (cBaseEnumsTable oInstall in oInstalls)
                {
                    m_InstallStatus.Add(new CmbItem { Content = oInstall.EnumName, Tag = oInstall.EnumValue });
                    this.cmbInstallStatus.Items.Add(oInstall.EnumName);

                }
                this.cmbInstallStatus.SelectedIndex = 0;

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


                // ** Surveyor drop down.
                m_Surveyors.Add(new CmbItem { Content = Settings.p_sAnyStatus, Tag = -1 });
                this.cmbSurveyor.Items.Add(Settings.p_sAnyStatus);

                //v1.0.3 - Retrieve logged on users profile.
                string sLoggedOnUserProfile = Session.Token;

                int iUsersIndex = -1;

                List<Surveyor> oSurveyors = Main.p_cDataAccess.GetSurveyors();
                foreach (Surveyor oSurveyor in oSurveyors)
                {
                    m_Surveyors.Add(new CmbItem { Content = oSurveyor.SurveyorName, Tag = oSurveyor.SurveyorProfile });
                    this.cmbSurveyor.Items.Add(oSurveyor.SurveyorName);

                    //v1.0.3 - If user profiles match record index position.
                    if (oSurveyor.SurveyorProfile.Equals(sLoggedOnUserProfile, StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        iUsersIndex = this.cmbSurveyor.Items.Count - 1;
                    }

                }
                if (iUsersIndex == -1)
                {
                    this.cmbSurveyor.SelectedIndex = 0; //Make Any the default selection

                }
                else
                {
                    this.cmbSurveyor.SelectedIndex = iUsersIndex; //v1.0.3 - Set to logged on user.
                }


                //Survey Status
                this.cmbSurveyInputStatus.Items.Add(Settings.p_sAnyStatus);
                this.cmbSurveyInputStatus.Items.Add(Settings.p_sInputStatus_Successful);
                this.cmbSurveyInputStatus.Items.Add(Settings.p_sInputStatus_NotPending);
                this.cmbSurveyInputStatus.Items.Add(Settings.p_sInputStatus_Pending);
                this.cmbSurveyInputStatus.Items.Add(Settings.p_sInputStatus_Failed);
                this.cmbSurveyInputStatus.SelectedIndex = 0;

                //Data comparison drop down.
                this.cmbDateCompare.Items.Add(Settings.p_sDateCompare_EqualTo);
                this.cmbDateCompare.Items.Add(Settings.p_sDateCompare_GreaterThan);
                this.cmbDateCompare.Items.Add(Settings.p_sDateCompare_LessThan);
                this.cmbDateCompare.SelectedIndex = 0;

                //v1.0.1 - Project no filter
                this.cmbProjectNoFilter.Items.Add(Settings.p_sProjectNoFilter_ProjectNo);
                this.cmbProjectNoFilter.Items.Add(Settings.p_sProjectNoFilter_SubProjectNo);
                this.cmbProjectNoFilter.SelectedIndex = 0;

                //v1.0.3 - Default to true.
                this.chkUseSurveyPlanDate.IsToggled = true;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        private void btnSearch_Clicked(object sender, EventArgs e)
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

                if (this.cmbProjectNoFilter.Items[cmbProjectNoFilter.SelectedIndex].Equals(Settings.p_sProjectNoFilter_ProjectNo) == true)
                {
                    String sValue = (String)this.cmbProjectNo.Items[cmbProjectNo.SelectedIndex];
                    if (sValue != Settings.p_sAnyStatus)
                    {
                        sProjectNo = sValue;

                    }

                }
                else if (this.cmbProjectNoFilter.Items[cmbProjectNoFilter.SelectedIndex].Equals(Settings.p_sProjectNoFilter_SubProjectNo) == true)
                {
                    //v1.0.1 - Sub Project number filter.
                    sSubProjectNo = this.txtSubProjectNoFilter.Text;
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
                if (this.chkUseSurveyPlanDate.IsToggled == true)
                {
                    dSurveyDate = this.dtPicker.Date;

                }

                //Surveyor
                string sSurveyor = string.Empty;
                cbiItems = m_Surveyors.Where(item => item.Content == cmbSurveyor.Items[cmbSurveyor.SelectedIndex]).ToList();
                sSurveyor = cbiItems[0].Tag.ToString();

                //Surveyed status.
                string sSurveyedStatus = cmbSurveyedStatus.Items[cmbSurveyedStatus.SelectedIndex];

                //Date comparison
                string sDateComparison = cmbDateCompare.Items[cmbDateCompare.SelectedIndex];

                //Surveyed on site
                string sSurveyedOnSite = cmbSurveyInputStatus.Items[cmbSurveyInputStatus.SelectedIndex];

                int iInstall_Awaiting = Convert.ToInt32(DependencyService.Get<IMain>().GetAppResourceValue("InstallStatus_AwaitingSurvey"));
                int iInstall_Cancel = Convert.ToInt32(DependencyService.Get<IMain>().GetAppResourceValue("InstallStatus_SurveyCancelled"));


                //v1.0.21 - Set health and safety incomplete filter.
                DataAccess.HSFilters iHSFilter = DataAccess.HSFilters.Complete;
                if (this.m_bSurveyedMode == true)
                {
                    iHSFilter = DataAccess.HSFilters.InComplete;
                }

                List<SurveyInputResult> cResults = Main.p_cDataAccess.SearchSurveyInput(
                    sProjectNo, this.txtDeliveryStreet.Text, this.txtPostcode.Text, iInstallStatus, 
                    iProgressStatus, dSurveyDate, sDateComparison, sSurveyedStatus, sSurveyor, sSurveyedOnSite, 
                    this.chkSyncChangesOnly.IsToggled, this.chkShowAllStatuses.IsToggled, 
                    this.chkShowAllProgessStatus.IsToggled, sSubProjectNo, iInstall_Awaiting, 
                    iInstall_Cancel, false, Settings.p_sInstallStatusFilter_EqualTo, 
                    Settings.p_sAnyStatus, iHSFilter);

                int iUpdates = 0;
                foreach (SurveyInputResult cResult in cResults)
                {

                    //cResult.ScreenWidth = this.lvResults.ActualWidth;
                    //cResult.StreetWidth = this.txtDeliveryStreet.ActualWidth; // +10;
                    //cResult.InstallWidth = this.cmbInstallStatus.ActualWidth; // +20;
                    //cResult.ProgressWidth = this.cmbProgressStatus.ActualWidth; //v1.0.14

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


                    //Survey input status
                    if (cResult.MXM1002TrfDate.HasValue == true)
                    {
                        cResult.SurveyInputStatus = Settings.p_sInputStatus_Successful;
                    }
                    else if (cResult.MXM1002TrfDate.HasValue == false)
                    {
                        cResult.SurveyInputStatus = Settings.p_sInputStatus_Pending;

                        if (cResult.MxmConfirmedAppointmentIndicator.HasValue == false || cResult.MxmConfirmedAppointmentIndicator.Value == false)
                        {
                            if (cResult.EndDateTime.HasValue == true && cResult.StartDateTime.HasValue == true)
                            {
                                cResult.SurveyInputStatus = Settings.p_sInputStatus_Failed;

                            }


                        }

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
                Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new ProjectSearchResultPage(cResults)));
                //this.lvResults.ItemsSource = cResults;


            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);


            }

        }
    }
}
