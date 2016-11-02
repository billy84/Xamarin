using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace ANG_ABP_SURVEYOR_APP_CLASS.Model
{

    /// <summary>
    /// Table for holding the base enumerator values.
    /// </summary>
    public class cBaseEnumsTable
    {

        /// <summary>
        /// IDKey field
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int IDKey { get; set; }


        /// <summary>
        /// Table Name
        /// </summary>
        [Indexed, MaxLength(60)]
        public string TableName { get; set; }

        /// <summary>
        /// Field Name
        /// </summary>
        [Indexed,MaxLength(60)]
        public string FieldName { get; set; }

        /// <summary>
        /// Enumerator Name
        /// </summary>
        [MaxLength(150)]
        public string EnumName { get; set; }

        /// <summary>
        /// Enumerator Value
        /// </summary>
        public int EnumValue { get; set; }


    }
}
