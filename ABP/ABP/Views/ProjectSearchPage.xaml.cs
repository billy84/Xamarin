using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ABP.Views
{
    public class SearchResult
    {
        public string Address { get; set; }
        public string PlanDate { get; set; }
        public string ProgressStatus { get; set; }
        public string ConfirmedFlag { get; set; }


    }
    public partial class ProjectSearchPage : ContentPage
    {
        ObservableCollection<SearchResult> results = new ObservableCollection<SearchResult>();
        public ProjectSearchPage()
        {
            InitializeComponent();
            this.Title = "Project Search";
            SearchProjectBtn.Source = ImageSource.FromFile(String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "find"));
            ProjectList.ItemsSource = results;
            results.Add(new SearchResult { Address = "23 Flexway Lane, Norwich, Norfolk", PlanDate = "NR3 10P - 26/10/2015 11:35 PM", ProgressStatus = "Able to progress", ConfirmedFlag = "Confirmed" });
            results.Add(new SearchResult { Address = "The Lodge, Hasingwell Estate, Norwich, Norfolk", PlanDate = "NR3 10P - 26/10/2015 11:35 PM", ProgressStatus = "No access to installation", ConfirmedFlag = "Not Confirmed" });
            ProjectList.ItemSelected += (sender, e) =>
            {
                if (e.SelectedItem == null) return;
                Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new InputResultPage()));
            };
        }
    }
}
