using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Classes;
using Anglian.Engine;
using Anglian.Models;
using Xamarin.Forms;

namespace Anglian.Views
{
    public partial class SurveyFailedPage : ContentPage
    {
        public List<cFailedSurveyReasonsTable> p_lsFailedReasons = null;
        private cProjectTable m_cProject = null;
        public SurveyFailedPage(SurveyInputResult item)
        {
            InitializeComponent();
            Title = "Survey Failed";
            tbProjectNo.Text = item.DeliveryStreet + ", " + item.DlvZipCode;
            tbSubProjectNo.Text = item.SubProjectNo;
            txtFailedComment.HeightRequest = 200;
            PopulateFailedDropDown();
            //this.m_cSurvey = (SurveyInputResult)e.Parameter;
            this.m_cProject = Main.p_cDataAccess.GetSubProjectProjectData(item.SubProjectNo);
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Save",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "save"),
                Command = new Command(() => btnUpdateSurveyFailed_Click())
            });
        }
        /// <summary>
        /// v1.0.19 - Return reason object for selected reason.
        /// </summary>
        /// <returns></returns>
        private cFailedSurveyReasonsTable ReturnReasonObject()
        {
            try
            {

                string sItem = this.cmbFailedSurvey.Items[cmbFailedSurvey.SelectedIndex];
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
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return null;

            }

        }
        /// <summary>
        /// Update survey failed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnUpdateSurveyFailed_Click()
        {

            try
            {

                cFailedSurveyReasonsTable oReason = this.ReturnReasonObject();

                //Remove blank spaces from comment box.
                this.txtFailedComment.Text = this.txtFailedComment.Text.Trim();

                string sItem = this.cmbFailedSurvey.Items[cmbFailedSurvey.SelectedIndex];
                if (sItem.Equals(Settings.p_sPleaseChoose) == true)
                {

                    await DisplayAlert("Failed Visit Reason Required.", "Please select a reason for the failed visit.", "OK");
                    this.cmbFailedSurvey.Focus();
                    return;

                }
                else if (oReason.MandatoryNote == true)
                {

                    //If other selected then a comment is required.
                    if (this.txtFailedComment.Text.Length == 0)
                    {

                        await DisplayAlert(
                            "Comment Required.", 
                            "You need to enter a comment when you select a failed visit reason of " + Settings.p_sNoteIt_Other + ".",
                            "OK");
                        this.txtFailedComment.Focus();
                        return;

                    }

                }

                this.m_cProject.MxmConfirmedAppointmentIndicator = (int)Settings.YesNoBaseEnum.No; //v1.0.1
                Main.p_cDataAccess.AddToUpdateTable(
                    this.m_cProject.SubProjectNo, "MxmConfirmedAppointmentIndicator", ((int)Settings.YesNoBaseEnum.No).ToString());

                //v1.0.19 - If progress status specified we need to update.
                if (oReason.ProgressStatus > -1)
                {

                    this.m_cProject.Mxm1002ProgressStatus = oReason.ProgressStatus;
                    this.m_cProject.ProgressStatusName = Main.p_cDataAccess.GetEnumValueName("ProjTable", "Mxm1002ProgressStatus", oReason.ProgressStatus);

                    Main.p_cDataAccess.AddToUpdateTable(this.m_cProject.SubProjectNo, "Mxm1002ProgressStatus", oReason.ProgressStatus.ToString());
                }

                //Save sub project.
                bool bSaveOK = Main.p_cDataAccess.UpdateProjectTable(this.m_cProject);
                if (bSaveOK == false)
                {
                    await DisplayAlert("Error When Saving.", "An error occurred when trying to save, please try again.", "OK");
                    return;
                }

                string sNewNote = sItem + ": " + this.txtFailedComment.Text;

                DateTime dtFailedDate = DateTime.Now;
                dtFailedDate = btnUpdateSurveyAttemptDate.Date;

                //v1.0.1 - Return note object
                cProjectNotesTable cNote = Settings.ReturnNoteObject(
                    this.m_cProject.SubProjectNo, 
                    sNewNote, dtFailedDate, 
                    Settings.p_sProjectNoteType_SurveyFailed);

                //v1.0.1 - Save sub project notes.
                bSaveOK = Main.p_cDataAccess.SaveSubProjectNote(cNote);
                if (bSaveOK == false)
                {
                    await DisplayAlert("Error When Saving.", "An error occurred when trying to save, please try again.", "OK");
                    return;
                }


                //Go back to previous page.
                //navigationHelper.GoBack();


            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);


            }

        }
        private void PopulateFailedDropDown()
        {
            try
            {
                p_lsFailedReasons = Main.p_cDataAccess.FetchAllSurveyFailedReasons();


                //Add please choose to the list.
                cmbFailedSurvey.Items.Add(Settings.p_sPleaseChoose);

                //v1.0.19
                foreach (cFailedSurveyReasonsTable oReason in this.p_lsFailedReasons)
                {
                    cmbFailedSurvey.Items.Add(oReason.Description);

                }


                //Select first item in list.
                cmbFailedSurvey.SelectedIndex = 0;
            }
            catch (Exception ex)
            {

            }
        }
    }
}
