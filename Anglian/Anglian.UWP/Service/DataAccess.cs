using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.UWP.Service;
using Anglian.Service;
using Anglian.Classes;
using Xamarin.Forms;
using System.IO;
using Anglian.Models;

[assembly: Dependency(typeof(DataAccess))]
namespace Anglian.UWP.Service
{
    class DataAccess : IDataAccess
    {
        public string GetFullPath(string sDBFileName)
        {
            return Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sDBFileName);
        }
    }
}
