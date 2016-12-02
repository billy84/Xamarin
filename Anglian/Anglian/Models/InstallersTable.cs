using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Anglian.Models
{
    public class cInstallersTable
    {
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
