using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace ANG_ABP_SURVEYOR_APP_CLASS.Model
{
    /// <summary>
    /// v1.0.1 - Project notes table.
    /// </summary>
    public class cProjectNotesTable
    {

        /// <summary>
        /// IDKey field
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int IDKey { get; set; }

        /// <summary>
        /// SubProject number field
        /// </summary>
        [Indexed, MaxLength(20)]
        public string SubProjectNo { get; set; }

        /// <summary>
        /// Field Note text field
        /// </summary>
        [MaxLength(9999)]
        public string NoteText { get; set; }

        /// <summary>
        /// Field Note type text field
        /// </summary>
        [MaxLength(30)]
        public string NoteType { get; set; }

        /// <summary>
        /// Date time note added
        /// </summary>
        public DateTime InputDateTime { get; set; }

        /// <summary>
        /// AX record id
        /// </summary>
        public Int64 AXRecID { get; set; }

        /// <summary>
        /// Name of user who created.
        /// </summary>
        [MaxLength(100)]
        public string UserName { get; set; }

        /// <summary>
        /// User profile of user who created.
        /// </summary>
        [MaxLength(50)]
        public string UserProfile { get; set; }

    }
}
