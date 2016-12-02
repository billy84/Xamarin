using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anglian.Classes
{
    public class SubProjectSyncUpdateValues
    {
        /// <summary>
        /// SubProject number field
        /// </summary>        
        public string SubProjectNo { get; set; }

        /// <summary>
        /// Last time records was updated from server field
        /// </summary>
        public DateTime? ModifiedDateTime { get; set; }

        /// <summary>
        /// Last time activities records was updated from server field
        /// </summary>
        public DateTime? SMMActivities_MODIFIEDDATETIME { get; set; }

        /// <summary>
        /// v1.0.12 - Delivery modified date time.
        /// </summary>
        public DateTime? Delivery_ModifiedDateTime { get; set; }
    }
}
