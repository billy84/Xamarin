using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ABP.Models;
using ABP.TableModels;

namespace ABP.Views
{
    public partial class InputResultPage : ContentPage
    {
        public InputResultPage(cSurveyInputResult item)
        {
            InitializeComponent();
            this.Title = "Input Result";
            txtProjectInfo.Text = item.DeliveryStreet + item.DlvZipCode;
        }
        private void Success_Clicked(object sender, EventArgs args)
        {
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new SurveySuccessPage(txtProjectInfo.Text)));
        }
    }
}
