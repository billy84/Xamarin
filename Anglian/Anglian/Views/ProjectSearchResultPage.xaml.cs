using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Classes;
using Xamarin.Forms;

namespace Anglian.Views
{
    public partial class ProjectSearchResultPage : ContentPage
    {
        ObservableCollection<SurveyInputResult> m_sResult = new ObservableCollection<SurveyInputResult>(); 
        public ProjectSearchResultPage(List<SurveyInputResult> cResults)
        {
            InitializeComponent();
            Title = "Survey Search Result";
            lvResults.ItemsSource = cResults;
            lvResults.ItemTapped += LvResults_ItemTapped;
        }

        private void LvResults_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            SurveyInputResult selectedItem = e.Item as SurveyInputResult;
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new InputResultPage(selectedItem)));
            //throw new NotImplementedException();
        }
    }
}
