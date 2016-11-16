using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using ABP.UWP.Impls;
using Windows.Storage;
using System.IO;
using ABP;
using ABP.Interfaces;

[assembly: Dependency(typeof(DatabaseConnection_UWP))]
namespace ABP.UWP.Impls
{
    public class DatabaseConnection_UWP : IDatabaseConnection
    {
        public DatabaseConnection_UWP()
        {

        }
        public SQLiteConnection MainDbConnection()
        {
            var sDBFileName = "surveyordbmain.sqlite";
            string sFullPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sDBFileName);
            var conn = new SQLite.SQLiteConnection(sFullPath);
            return conn; 
        }
        public SQLiteConnection TestDbConnection()
        {
            var sDBFileName = "surveyordbmain_test.sqlite";
            string sFullPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sDBFileName);
            var conn = new SQLite.SQLiteConnection(sFullPath);
            return conn;
        }
        public SQLiteConnection SettingDbConnection()
        {
            var sDBFileName = "surveyordbsetting.sqlite";
            string sFullPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sDBFileName);
            var conn = new SQLite.SQLiteConnection(sFullPath);
            return conn;
        }
    }
}
