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
        Task<bool> ReturnSubProjectImagesFolder(string subProjectNo);
        Task<bool> SaveFileLocally(string v_sSubProjectNo, byte[] v_bFileData, string v_sFileName);
    }
}
