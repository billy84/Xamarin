using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anglian.Service
{
    public interface IMain
    {
        Task<bool> CopyFile(string v_sFromFile, string v_sToFile);
        string GetAppResourceValue(string v_sResourceName);
    }
}
