using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ABP.Views
{
    public partial class InputResultPage : ContentPage
    {
        public InputResultPage()
        {
            InitializeComponent();
            this.Title = "Input Result";
        }
        private void Success_Clicked(object sender, EventArgs args)
        {
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new SurveySuccessPage()));
        }
    }
}
