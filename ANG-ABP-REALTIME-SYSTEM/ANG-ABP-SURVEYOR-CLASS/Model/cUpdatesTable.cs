using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace ANG_ABP_SURVEYOR_APP_CLASS.Model
{
    public class cUpdatesTable
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
        /// Name of field to be updated.
        /// </summary>
        [MaxLength(60)]
        public string FieldName { get; set; }

        /// <summary>
        /// Value to be saved to field.
        /// </summary>
        [MaxLength(9999)]
        public string FieldValue { get; set; }


    }
}
