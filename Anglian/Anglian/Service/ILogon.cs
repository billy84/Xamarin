using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Classes;

namespace Anglian.Service
{
    public interface ILogon
    {
        Task<LogonResult> LogonAsync(string v_sUsername, string v_sPassword, string v_sAuthID);
    }
}
