using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anglian.Classes
{
    public class InstallerDashboardResult
    {
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
