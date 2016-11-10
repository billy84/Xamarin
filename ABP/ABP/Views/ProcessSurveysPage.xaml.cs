using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ABP.Views
{
    public class Appointment
    {
        public string Address { get; set; }
        public string Time { get; set; }
    }
    public partial class ProcessSurveysPage : TabbedPage
    {
        public ObservableCollection<Appointment> Appointments = new ObservableCollection<Appointment>();
        public ProcessSurveysPage()
        {
            InitializeComponent();
            this.Title = "Current Bookings";
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Download",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "down"),
                Command = new Command(() => DownBtn_Tapped())
            });
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Sync",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "refresh"),
                Command = new Command(() => SyncBtn_Tapped())
            });
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Search",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "find"),
                Command = new Command(() => SearchBtn_Tapped())
            });
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Setting",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "configuration")
            });

            //CarouselAppointments.ItemsSource = Appointments;
            //Appointments.Add(new Appointment { Address = "23 Flexway Lane, Norwich, Norfolk", Time = "NR3 10P - Time: 11:45" });
            //Appointments.Add(new Appointment { Address = "The Lodge, Hasingwell Estate, Norwich, Norfolk", Time = "NR33 10P - Time: 14:35" });
        }
        private void DownBtn_Tapped()
        {
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new ProjectDownloadsSearchPage()));
            //await this.Navigation.PushAsync(new DateSearchResultView(items));
        }
        private void SyncBtn_Tapped()
        {
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new ProjectSyncPage()));
            //await this.Navigation.PushAsync(new DateSearchResultView(items));
        }
        private void SearchBtn_Tapped()
        {
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new ProjectSearchPage()));
            //await this.Navigation.PushAsync(new DateSearchResultView(items));
        }
    }
}
