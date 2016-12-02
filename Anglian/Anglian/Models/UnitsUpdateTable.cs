using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Anglian.Models
{
    public class cUnitsUpdateTable
    {
        /// <summary>
        /// IDKey field
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int IDKey { get; set; }

        /// SubProject number field
        /// </summary>
        [Indexed, MaxLength(20)]
        public string SubProjectNo { get; set; }

        /// <summary>
        /// Unit number
        /// </summary>
        public int UnitNo { get; set; }

        /// <summary>
        /// Installed status
        /// </summary>                
        public int? InstalledStatus { get; set; }

        /// <summary>
        /// Installed date
        /// </summary>                
        public DateTime? InstalledDate { get; set; }
    }
}
