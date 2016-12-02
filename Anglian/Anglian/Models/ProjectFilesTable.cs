using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Anglian.Models
{
    public class cProjectFilesTable
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
        /// File Note text field
        /// </summary>
        [MaxLength(9999)]
        public string NoteText { get; set; }

        /// <summary>
        /// File path field
        /// </summary>
        [MaxLength(300)]
        public string FileName { get; set; }

        /// <summary>
        /// File modified date time.
        /// </summary>
        public DateTime ModDateTime { get; set; }

        /// <summary>
        /// File has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// File is new
        /// </summary>
        public bool NewFile { get; set; }
    }
}
