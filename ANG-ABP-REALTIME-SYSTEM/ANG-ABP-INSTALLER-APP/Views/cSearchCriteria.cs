using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG_ABP_INSTALLER_APP.Views
{
    class cSearchCriteria
    {

        /// <summary>
        /// 
        /// </summary>
        public string ProjectNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ProjectNoFilter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StreetAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Postcode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? InstallStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? ProgressStatus { get; set; }

        /// <summary>
        /// Survey plan date included in search.
        /// </summary>
        public bool IncludeInstallPlanDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? InstallPlanDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string InstallPlanDateComparison { get; set; }       

        /// <summary>
        /// 
        /// </summary>
        public string Selected_SubProjectNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool SyncChangesOnly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowAllStatuses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowAllProgressStatuses { get; set; }

        /// <summary>
        /// Confirmed sub project.
        /// </summary>
        public int? Confirmed { get; set; }

        /// <summary>
        /// Booked
        /// </summary>
        public bool Booked { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string InstallStatus_Filter { get; set; }

        /// <summary>
        /// v1.0.11 - Order complete filter.
        /// </summary>
        public string OrderComplete_Filter { get; set; }

        /// <summary>
        /// v1.0.11 - Order complete mode.
        /// </summary>
        public bool OrderCompleteMode { get; set; }

        /// <summary>
        /// v1.0.11 - Switch off installation date filter.
        /// </summary>
        public bool SwitchOffInstallationDateFilter { get; set; }

    }
}
