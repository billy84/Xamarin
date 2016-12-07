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

        public DateSearchResultPage(ObservableCollection<SurveyDatesResult> cResults)
        {
            InitializeComponent();
            this.Title = "Survey Dates Search Results";
            this.ToolbarItems.Add(new ToolbarItem() { Text = "Select All", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "check"), Command = new Command(() => SelectAllBtn_Tapped()) });
            this.ToolbarItems.Add(new ToolbarItem() { Text = "De-Select All", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "uncheck"), Command = new Command(() => De_SelectAllBtn_Tapped()) });
            this.ToolbarItems.Add(new ToolbarItem() { Text = "Set Survey Date", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "date") });
            this.ToolbarItems.Add(new ToolbarItem() { Text = "Un-Confirm", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "undo") });
            populateResultList(cResults);
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
            this.ResultList.ItemsSource = this.results;
        }

    }
}
