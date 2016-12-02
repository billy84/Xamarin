using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anglian.Service
{
    public interface IDataAccess
    {
        string GetFullPath(string sDBFileName);
    }
}
