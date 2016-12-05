using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Classes;
using Xamarin.Forms;

namespace Anglian.Views
{
    public partial class InputResultPage : ContentPage
    {
        private SurveyInputResult currentItem = new SurveyInputResult();
        public InputResultPage(SurveyInputResult item)
        {
            InitializeComponent();
            this.currentItem = item;
            this.Title = "Input Result";
            txtProjectInfo.Text = item.DeliveryStreet + item.DlvZipCode;
        }
        private void Success_Clicked(object sender, EventArgs args)
        {
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new SurveySuccessPage(txtProjectInfo.Text)));
        }
        private void Failed_Clicked(object sender, EventArgs args)
        {
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new SurveyFailedPage(this.currentItem)));
        }
    }
}
