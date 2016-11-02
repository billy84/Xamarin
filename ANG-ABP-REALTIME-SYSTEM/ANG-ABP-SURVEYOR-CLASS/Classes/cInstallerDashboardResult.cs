using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG_ABP_SURVEYOR_APP_CLASS.Classes
{
    /// <summary>
    /// v1.0.5 - Used to return the unit quantities.
    /// </summary>
    public class cInstallerDashboardResult
    {

        /// <summary>
        /// 
        /// </summary>
        public string SubProjectNo { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndDateTime { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string DeliveryStreet { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ResidentName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string InstallStatusName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ProgressStatusName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalUnits { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalUnitsInstalled { get; set; }


    }
}
