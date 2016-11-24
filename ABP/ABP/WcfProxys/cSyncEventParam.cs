using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.WcfProxys
{
    class cSyncEventParam : EventArgs
    {
        public string DisplayMessage { get; set; }
        public cSyncEventParam(string v_sMessage)
        {
            this.DisplayMessage = v_sMessage;
        }
    }
}
