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
            if (cResults.Count() == 0)
            {
                SurveyInputResult cResult = new SurveyInputResult();
                cResult.SubProjectNo = "XXXXXXXX";
                cResult.DeliveryStreet = "XXX XXXX XXXXXX";
                cResult.DlvZipCode = "00000";
                cResult.SurveyDisplayDateTime = "00:00AM";
                cResult.SurveyedStatus = "N/A";
                cResults.Add(cResult);
            }
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
