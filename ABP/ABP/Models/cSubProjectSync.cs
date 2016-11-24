using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Models
{
    public class cSubProjectSync
    {
        public string SubProjectNo { get; set; }

        /// <summary>
        /// Quantity of updates.
        /// </summary>
        public int UpdateQty { get; set; }

        /// <summary>
        /// Files waiting to be uploaded.
        /// </summary>             
        public int FileUpdateQty { get; set; }

        /// <summary>
        /// Notes update quantity
        /// </summary>
        public int NotesUpdateQty { get; set; }

        /// <summary>
        /// Unit update quantity
        /// </summary>
        public int UnitUpdateQty { get; set; }
    }
}
