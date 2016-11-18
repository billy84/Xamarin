using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Interfaces
{
    public interface ISettings
    {
        Task<string> GetUserName();
        Task<string> GetUserDisplayName();
        bool AreWeOnline();
    }
}
