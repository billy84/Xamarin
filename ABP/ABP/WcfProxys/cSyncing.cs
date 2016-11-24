using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ABP.Models;

namespace ABP.WcfProxys
{
    public class cSyncing
    {
        /// <summary>
        /// Event handler for displaying messages
        /// </summary>
        public event EventHandler<string> DisplayMessage;

        /// <summary>
        /// Event handler for displaying update messages
        /// </summary>
        //public event EventHandler<cSyncEventParam> UpdateMessage;

        /// <summary>
        /// Event handler for sub project status update.
        /// </summary>
        //public event EventHandler<cSyncEventParamProjectStatus> SubProjectStatusUpdate;

        /// <summary>
        /// Event handler errors.
        /// </summary>
        //public event EventHandler<cSyncEventErrorParams> ProjectSyncError;

        /// <summary>
        /// 
        /// </summary>
        private cDataAccess m_cData = new cDataAccess();

        /// <summary>
        /// Settings class.
        /// </summary>
        private cSettings m_cSetting = new cSettings();


        /// <summary>
        /// v1.0.1 - Flag to indicate if to sync changes only.
        /// </summary>
        public static bool p_bSyncChangesOnly = false;

        /// <summary>
        /// v1.0.1 - List of projects to sync
        /// </summary>
        public static ObservableCollection<cProjectSearch> p_ocProjectsToSync = null;
        public cSyncing()
        {

            try
            {

                //this.m_cSetting.DisplayMessage += m_cSetting_DisplayMessage;

                this.m_cData.CheckDB();

            }
            catch (Exception ex)
            {


            }


        }
        void m_cSetting_DisplayMessage(object sender, string e)
        {

            this.DisplayMessage(this, e);

        }

    }
}
