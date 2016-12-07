using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Models;
using Anglian.Classes;
using Xamarin.Forms;

namespace Anglian.Views
{
    public partial class DateSearchResultPage : ContentPage
    {
        private ObservableCollection<SurveyDatesResult> results = new ObservableCollection<SurveyDatesResult>();

        public DateSearchResultPage(List<SurveyDatesResult> cResults)
        {
            InitializeComponent();
            this.Title = "Survey Dates Search Results";
            this.ToolbarItems.Add(new ToolbarItem() { Text = "Select All", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "check") });
            this.ToolbarItems.Add(new ToolbarItem() { Text = "De-Select All", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "uncheck") });
            this.ToolbarItems.Add(new ToolbarItem() { Text = "Set Survey Date", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "date") });
            this.ToolbarItems.Add(new ToolbarItem() { Text = "Un-Confirm", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "undo") });
            ResultList.ItemsSource = results;

            foreach (SurveyDatesResult cResult in cResults)
            {
                results.Add(new SurveyDatesResult
                {
                    DeliveryStreet = cResult.DeliveryStreet,
                    DlvZipCode = cResult.DlvZipCode + " - " + cResult.SurveyDisplayDateTime,
                    ProgressStatusName = cResult.ProgressStatusName,
                    Confirmed = cResult.Confirmed
                });
            }
        }
    }
}
