using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ABP.Views
{
    public class Result
    {
        public string Address { get; set; }
        public string PlanDate { get; set; }
        public string ProgressStatus { get; set; }
        public string ConfirmedFlag { get; set; }


    }

    public partial class DateSearchResultPage : ContentPage
    {
        ObservableCollection<Result> results = new ObservableCollection<Result>();
        public DateSearchResultPage()
        {
            InitializeComponent();
            this.Title = "Survey Dates Search Results";
            this.ToolbarItems.Add(new ToolbarItem() { Text = "Select All", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "down") });
            this.ToolbarItems.Add(new ToolbarItem() { Text = "De-Select All", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "refresh") });
            this.ToolbarItems.Add(new ToolbarItem() { Text = "Set Survey Date", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "configuration") });
            this.ToolbarItems.Add(new ToolbarItem() { Text = "Un-Confirm", Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "configuration") });
            ResultList.ItemsSource = results;
            //ResultList.CheckBoxs = true;
            results.Add(new Result { Address = "23 Flexway Lane, Norwich, Norfolk", PlanDate = "NR3 10P - 26/10/2015 11:35 PM", ProgressStatus = "Able to progress", ConfirmedFlag = "Confirmed" });
            results.Add(new Result { Address = "The Lodge, Hasingwell Estate, Norwich, Norfolk", PlanDate = "NR3 10P - 26/10/2015 11:35 PM", ProgressStatus = "No access to installation", ConfirmedFlag = "Not Confirmed" });
        }
    }
}
