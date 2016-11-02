using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG_ABP_SURVEYOR_APP_CLASS.Classes
{
    public class cSubProjectResult
    {

        /// <summary>
        /// SubProject number field
        /// </summary>        
        public string SubProjectNo { get; set; }

        /// <summary>
        /// Delivery street name field
        /// </summary>
        public string DeliveryStreet { get; set; }

        /// <summary>
        /// Progress status name field
        /// </summary>
        public int? Mxm1002ProgressStatus { get; set; }

        /// <summary>
        /// Progress status description name field
        /// </summary>
        public string ProgressStatusName { get; set; }

        /// <summary>
        /// Install status name field
        /// </summary>
        public int? Mxm1002InstallStatus { get; set; }

        /// <summary>
        /// Install status description name field
        /// </summary>
        public string InstallStatusName { get; set; }
    }
}
