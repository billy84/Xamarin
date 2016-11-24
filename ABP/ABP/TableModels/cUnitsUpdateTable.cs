using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace ABP.TableModels
{
    public class cUnitsUpdateTable
    {
        [PrimaryKey, AutoIncrement]
        public int IDKey { get; set; }
        [Indexed, MaxLength(20)]
        public string SubProjectNo { get; set; }
        public int UnitNo { get; set; }
        public int? InstalledStatus { get; set; }
        public DateTime? InstalledDate { get; set; }
    }
}
