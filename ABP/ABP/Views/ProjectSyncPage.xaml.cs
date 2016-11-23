using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ABP.WcfProxys;

namespace ABP.Views
{
    public partial class ProjectSyncPage : ContentPage
    {
        public ProjectSyncPage()
        {
            InitializeComponent();
            this.Title = "Project Sync";
        }
        private void SendChanges_Clicked(object sender, EventArgs args)
        {
            try
            {

                cSyncing.p_bSyncChangesOnly = true;

                //Kick off check for syncing.
                cMain.KickOffSyncing();

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }
        private void SyncAll_Clicked(object sender, EventArgs args)
        {

        }
    }
}
