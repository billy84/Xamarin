using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ABP.Interfaces;
using ABP.TableModels;
using ABP.Models;
using ABP.WcfProxys;
using FontAwesomeXamarin;

namespace ABP.Views
{
    public partial class ProjectSearchPage : ContentPage
    {
        //ObservableCollection<SearchResult> results = new ObservableCollection<SearchResult>();
        private bool m_bSurveyedMode = false;
        public ProjectSearchPage()
        {
            InitializeComponent();
            Title = "Project Search";
            SearchProjectBtn.Source = ImageSource.FromFile(String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "find"));
            var tapSearchBtn = new TapGestureRecognizer();
            tapSearchBtn.Tapped += TapSearchBtn_Tapped;
            SearchProjectBtn.GestureRecognizers.Add(tapSearchBtn);

            SearchProjectBtn.HeightRequest = 30;
            SearchProjectBtn.WidthRequest = 30;
            //ProjectList.ItemsSource = results;
            lvResults.ItemTapped += (sender, e) =>
            {
                cSurveyInputResult selectedItem = e.Item as cSurveyInputResult;
                Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new InputResultPage(selectedItem)));
            };
        }
        private void TapSearchBtn_Tapped(object sender, EventArgs e)
        {
            if (txtDeliveryStreet.Text == null || txtDeliveryStreet.Text.Trim() == string.Empty)
            {
                DisplayAlert("Error", "Please Enter Street", "OK");
                return;
            }
            if (txtPostcode.Text == null || txtPostcode.Text.Trim() == string.Empty)
            {
                DisplayAlert("Error", "Please Enter PostCode", "OK");
                return;
            }
            DisplaySearchResults();
        }
        private void DisplaySearchResults()
        {
            try
            {
                string sProjectNo = null;
                string sSubProjectNo = null;
                Int32 iInstallStatus = -1;
                Int32 iProgressStatus = -1;
                DateTime? dSurveyDate = null;
                string sSurveyor = string.Empty;
                string sSurveyedStatus = string.Empty;
                string sDateComparison = string.Empty;
                string sSurveyedOnSite = string.Empty;
                int iInstall_Awaiting = Convert.ToInt32(DependencyService.Get<IMain>().GetAppResourceValue("InstallStatus_AwaitingSurvey"));
                int iInstall_Cancel = Convert.ToInt32(DependencyService.Get<IMain>().GetAppResourceValue("InstallStatus_SurveyCancelled"));
                WcfProxys.cDataAccess.HSFilters iHSFilter = WcfProxys.cDataAccess.HSFilters.Complete;
                if (m_bSurveyedMode == true)
                {
                    iHSFilter = WcfProxys.cDataAccess.HSFilters.InComplete;
                }
                List<cSurveyInputResult> cResults = cMain.p_cDataAccess.SearchSurveyInput(
                    sProjectNo, 
                    txtDeliveryStreet.Text, 
                    txtPostcode.Text, 
                    iInstallStatus, 
                    iProgressStatus, 
                    dSurveyDate, 
                    sDateComparison, 
                    sSurveyedStatus, 
                    sSurveyor, 
                    sSurveyedOnSite, 
                    false, 
                    false, 
                    false, 
                    sSubProjectNo, 
                    iInstall_Awaiting, 
                    iInstall_Cancel, 
                    false, 
                    cSettings.p_sInstallStatusFilter_EqualTo, 
                    cSettings.p_sAnyStatus, iHSFilter);
                int iUpdates = 0;
                foreach (cSurveyInputResult cResult in cResults)
                {
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
                    cResult.SurveyedStatus = cSettings.p_sSurveyedStatus_NotSurveyed;
                    if (cResult.MXM1002TrfDate.HasValue == true)
                    {
                        if (cResult.Mxm1002InstallStatus == iInstall_Awaiting || cResult.Mxm1002InstallStatus == iInstall_Cancel)
                        {
                            cResult.SurveyedStatus = cSettings.p_sSurveyedStatus_SurveyedOnSite;

                        }
                    }
                    if (cResult.Mxm1002InstallStatus != iInstall_Awaiting && cResult.Mxm1002InstallStatus != iInstall_Cancel)
                    {
                        cResult.SurveyedStatus = cSettings.p_sSurveyedStatus_SurveyedTrans;

                    }
                    if (cResult.MXM1002TrfDate.HasValue == true)
                    {
                        cResult.SurveyInputStatus = cSettings.p_sInputStatus_Successful;
                    }
                    else if (cResult.MXM1002TrfDate.HasValue == false)
                    {
                        cResult.SurveyInputStatus = cSettings.p_sInputStatus_Pending;

                        if (cResult.MxmConfirmedAppointmentIndicator.HasValue == false || cResult.MxmConfirmedAppointmentIndicator.Value == false)
                        {
                            if (cResult.EndDateTime.HasValue == true && cResult.StartDateTime.HasValue == true)
                            {
                                cResult.SurveyInputStatus = cSettings.p_sInputStatus_Failed;

                            }


                        }

                    }
                    cResult.DeliveryStreet = cMain.RemoveNewLinesFromString(cResult.DeliveryStreet);

                    //v1.0.1 - Update tool tip text
                    cResult.ToolTipText = "Status: " + cResult.StatusName + Environment.NewLine + "Progress Status: " + cResult.ProgressStatusName;

                    //v1.0.1 - Display formatted survey date - time
                    if (cResult.EndDateTime.HasValue == true)
                    {
                        cResult.SurveyDisplayDateTime = cMain.ReturnDisplayDate(cResult.EndDateTime.Value) + " " + cMain.ReturnDisplayTime(cResult.EndDateTime.Value);
                    }

                    //v1.0.15 - Set background color if project status is "On Hold"
                    if (cResult.Status == cSettings.p_iProjectStatus_OnHold)
                    {
                        cResult.BackgroundColour = cSettings.p_sSurvey_ListView_OnHold_Background;
                    }
                    else
                    {
                        cResult.BackgroundColour = cSettings.p_sSurvey_ListView_Normal_Background;
                    }
                }
                
                if (cResults.Count() == 0)
                {
                    cSurveyInputResult cResult = new cSurveyInputResult();
                    cResult.SubProjectNo = "123";
                    cResult.DeliveryStreet = "Street";
                    cResult.DlvZipCode = "1N012";
                    cResult.SurveyDisplayDateTime = "12:35PM";
                    cResult.SurveyedStatus = "Success";
                    cResults.Add(cResult);
                }
                lvResults.ItemsSource = cResults;
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
