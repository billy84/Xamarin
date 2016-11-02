using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace ANG_ABP_SURVEYOR_APP_CLASS.Model
{
    /// <summary>
    /// v1.0.8 - Installers table needed by installers app
    /// </summary>
    public class cInstallersTable
    {

        /// <summary>
        /// IDKey field
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int IDKey { get; set; }

        /// <summary>
        /// Account number field
        /// </summary>
        [Indexed, MaxLength(20)]
        public string AccountNum { get; set; }

        /// <summary>
        /// Installers name field
        /// </summary>
        [MaxLength(60)]
        public string Name { get; set; }


    }
}
