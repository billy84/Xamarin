using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Interfaces
{
    public interface ISettings
    {
        string GetUserName();
        string GetUserDisplayName(); 
    }
}
