using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ABP.Models;

namespace ABP.Views
{
    public partial class DateSearchResultPage : ContentPage
    {
        private ObservableCollection<SurveyDatesSearchResultViewItem> results = new ObservableCollection<SurveyDatesSearchResultViewItem>();
        public DateSearchResultPage(List<cSurveyDatesResult> cResults)
        {
            InitializeComponent();
            this.Title = "Survey Dates Search Results";
            this.ToolbarItems.Add(new ToolbarItem() { Text = "Select All", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "down") });
            this.ToolbarItems.Add(new ToolbarItem() { Text = "De-Select All", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "refresh") });
            this.ToolbarItems.Add(new ToolbarItem() { Text = "Set Survey Date", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "configuration") });
            this.ToolbarItems.Add(new ToolbarItem() { Text = "Un-Confirm", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "configuration") });
            ResultList.ItemsSource = results;

            foreach (cSurveyDatesResult cResult in cResults)
            {
                results.Add(new SurveyDatesSearchResultViewItem
                {
                    Address = cResult.DeliveryStreet,
                    PlanDate = cResult.DlvZipCode + " - " + cResult.SurveyDisplayDateTime,
                    ProgressStatus = cResult.ProgressStatusName,
                    ConfirmedFlag = cResult.Confirmed
                });
            }
        }
    }
}
