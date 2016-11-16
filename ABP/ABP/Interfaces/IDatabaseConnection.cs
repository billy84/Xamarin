using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace ABP.Interfaces
{
    public interface IDatabaseConnection
    {
        SQLite.SQLiteConnection MainDbConnection();
        SQLite.SQLiteConnection TestDbConnection();
        SQLite.SQLiteConnection SettingDbConnection();
    }
}
