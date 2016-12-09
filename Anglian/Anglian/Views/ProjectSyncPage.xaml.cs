using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Classes;
using Anglian.Models;
using Anglian.Engine;
using Xamarin.Forms;

namespace Anglian.Views
{
    public partial class ProjectSyncPage : ContentPage
    {
        private bool Stop = false;
        private bool IsStarted = false;
        public ProjectSyncPage()
        {
            InitializeComponent();
            Title = "Project Sync";
            AppSettingsTable cSettings = Main.p_cDataAccess.ReturnSettings();
            tbLastSyncDateTime.Text = Main.ReturnLastSyncString(cSettings.LastSyncDateTime);
        }
        private void SendChanges_Clicked(object sender, EventArgs args)
        {
            try
            {

                Syncing.p_bSyncChangesOnly = true;

                //Kick off check for syncing.
                KickOffSyncing();

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }
        public async void KickOffSyncing()
        {
            if (IsStarted == false)
                Device.StartTimer(TimeSpan.FromSeconds(2), m_dpDispatcherSync_Tick);
            else
                await DisplayAlert("Error", "Already Started Thread", "OK");
        }
        private void UpdateSyncPage()
        {
            string sSyncMsg = String.Empty;
            AppSettingsTable cSettings = Main.p_cDataAccess.ReturnSettings();
            sSyncMsg = Main.ReturnLastSyncString(cSettings.LastSyncDateTime);
            if (Application.Current.MainPage.Navigation.NavigationStack.Count > 0)
            {
                int index = Application.Current.MainPage.Navigation.NavigationStack.Count - 1;
                Page currPage = Application.Current.MainPage.Navigation.NavigationStack[index];
                if (currPage.GetType() == typeof(ProjectSyncPage))
                {
                    //currPage.BackgroundColor = Color.Red;
                    Label vLastSyncDateTime = currPage.FindByName<Label>("tbLastSyncDateTime");
                    vLastSyncDateTime.Text = sSyncMsg;
                }
            }
        }

        private bool m_dpDispatcherSync_Tick()
        {
            IsStarted = true;
            if (Stop == false)
            {
                Stop = true;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        await Main.StartSyncing(Syncing.p_bSyncChangesOnly);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        Stop = false;
                    }
                    Main.UpdateScreenAfterSyncing();
                    //Update screen.
                    UpdateSyncPage();
                });
            }
            return true;
        }

        private void SyncAll_Clicked(object sender, EventArgs args)
        {
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new SyncAllPage()));
        }
    }
}
