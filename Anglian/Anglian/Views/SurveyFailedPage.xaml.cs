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
        public SurveyFailedPage(SurveyInputResult item)
        {
            InitializeComponent();
            Title = "Survey Failed";
            tbProjectNo.Text = item.DeliveryStreet + ", " + item.DlvZipCode;
            tbSubProjectNo.Text = item.SubProjectNo;
            txtFailedComment.HeightRequest = 200;
            PopulateFailedDropDown();
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Save",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "save"),
                Command = new Command(() => SaveBtn_Tapped())
            });
        }
        private void SaveBtn_Tapped()
        {

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
