using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ABP.Surveyor.Views
{
    /// <summary>
    /// Logging in, the surveyor will have to logon to the phone app using their Anglian username and password.
    /// These details will be validated against the Anglian active directory.
    /// </summary>
    public partial class LogInView : ContentPage
    {
        public LogInView()
        {
            InitializeComponent();
            this.Title = "Anglian ABP Surveyor App";
            
        }
        private async void GoToMainMenuView(object sender, EventArgs args)
        {
            await this.Navigation.PushAsync(new MainMenuView());
        }
    }
}
