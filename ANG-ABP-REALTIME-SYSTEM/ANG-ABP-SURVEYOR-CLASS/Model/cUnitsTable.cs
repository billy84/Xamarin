using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace ANG_ABP_SURVEYOR_APP_CLASS.Model
{
    /// <summary>
    /// v1.0.8 - Required for installers application.
    /// </summary>
    public class cUnitsTable
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
        /// SubProject name field
        /// </summary>
        [MaxLength(60)]
        public string SubProjectName { get; set; }

        /// <summary>
        /// SubProject name field
        /// </summary>
        public int UnitNo { get; set; }

        /// <summary>
        /// Item ID field
        /// </summary>
        [MaxLength(4)]
        public string ItemID { get; set; }

        /// <summary>
        /// Style field
        /// </summary>        
        [MaxLength(10)]
        public string Style { get; set; }

        /// <summary>
        /// Unit location field
        /// </summary>        
        [MaxLength(20)]
        public string UnitLocation { get; set; }

        /// <summary>
        /// Installed status
        /// </summary>                
        public int? InstalledStatus { get; set; }

    }
}
