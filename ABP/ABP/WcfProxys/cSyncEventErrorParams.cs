using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.WcfProxys
{
    class cSyncEventErrorParams : EventArgs
    {
        public string ErrorMessage { get; set; }
        public string ProjectNo { get; set; }
        public cSyncEventErrorParams(string v_sErrorMessage, string v_sProjectNo)
        {
            this.ErrorMessage = v_sErrorMessage;
            this.ProjectNo = v_sProjectNo;
        }
    }
}
