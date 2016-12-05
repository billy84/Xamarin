using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Classes;
using Anglian.Engine;
using Anglian.Models;
using Xamarin.Forms;

namespace Anglian.Views
{
    public partial class SyncAllPage : ContentPage
    {
        private ObservableCollection<ProjectSearch> m_ocProjects = null;
        private bool Stop = false;
        private bool IsStarted = false;
        public SyncAllPage()
        {
            InitializeComponent();
            Title = "Syncing All";
            PopulateProjectsList();
            lvProjects.ItemTapped += LvProjects_ItemTapped;
            lvProjects.ItemSelected += LvProjects_ItemSelected;
            
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Select All",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "check"),
                Command = new Command(() => SelectAll_Tapped())
            });
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "De-Select All",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "uncheck"),
                Command = new Command(() => DeSelectAll_Tapped())
            });
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Delect Project",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "erase"),
                Command = new Command(() => Delect_Tapped())
            });

        }
        private void SelectAll_Tapped()
        {
            foreach(var vProject in m_ocProjects)
            {
                vProject.IsEnabled = true;
            }
        }
        private void Delect_Tapped()
        {

        }
        private void DeSelectAll_Tapped()
        {
            foreach (var vProject in m_ocProjects)
            {
                vProject.IsEnabled = false;
            }

        }

        private void LvProjects_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            
            //e.SelectedItem
            //throw new NotImplementedException();
        }

        private void LvProjects_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
            {

            }
            else
            {
                
            }
            //throw new NotImplementedException();
        }
        public ObservableCollection<ProjectSearch> GetSelection()
        {
            ObservableCollection<ProjectSearch> selectedItems = new ObservableCollection<ProjectSearch>();
            foreach(ProjectSearch cProject in m_ocProjects)
            {
                if (cProject.IsEnabled == true)
                    selectedItems.Add(cProject);
            }
            return selectedItems;
        }
        private async void btnStartSyncing_Click(object sender, EventArgs args)
        {
            try
            {

                if (GetSelection().Count() == 0)
                {
                    await DisplayAlert("Project Selection Required", "You need to select some projects before you can start syncing, please select and try-again.", "OK");
                    this.lvProjects.Focus();
                    return;
                }

                //v1.0.11 - Check if sync is in progress.
                if (Main.p_bIsSyncingInProgress == true)
                {
                    await DisplayAlert("Sync in progress", "A sync is currently in progress, please wait until the sync has finished before starting another one.", "OK");
                    return;
                }

                //await this.EnableScreenControls(false);
                btnStartSyncing.IsEnabled = false;

                Syncing.p_ocProjectsToSync = new ObservableCollection<ProjectSearch>();
                Syncing.p_bSyncChangesOnly = false;

                foreach (ProjectSearch cProject in GetSelection())
                {
                    Syncing.p_ocProjectsToSync.Add(cProject);

                }

                //Start the syncing.
                KickOffSyncing();


            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
            //lvProjects.i
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
            cAppSettingsTable cSettings = Main.p_cDataAccess.ReturnSettings();
            sSyncMsg = Main.ReturnLastSyncString(cSettings.LastSyncDateTime);
            if (Application.Current.MainPage.Navigation.NavigationStack.Count > 0)
            {
                int index = Application.Current.MainPage.Navigation.NavigationStack.Count - 1;
                Page currPage = Application.Current.MainPage.Navigation.NavigationStack[index];
                if (currPage.GetType() == typeof(SyncAllPage))
                {
                    //currPage.BackgroundColor = Color.Red;
                    Label tbSyncStatus = currPage.FindByName<Label>("tbSyncStatus");
                    tbSyncStatus.Text = sSyncMsg;
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
        /// <summary>
        /// Populate projects list.
        /// </summary>
        private void PopulateProjectsList()
        {

            try
            {

                this.m_ocProjects = Main.p_cDataAccess.FetchProjectsForSync();

                foreach (ProjectSearch cProject in this.m_ocProjects)
                {

                    cProject.ProjectNo = cProject.ProjectNo;
                    cProject.SubProjectQtyDisplay = cProject.SubProjectQty.ToString();
                    cProject.IsEnabled = true;
                    //cProject.ListViewWidth = this.lvProjects.ActualWidth;

                }

                this.lvProjects.ItemsSource = this.m_ocProjects;


            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }
    }
}
