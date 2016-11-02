using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG_ABP_SURVEYOR_APP_CLASS.Syncing
{
    public class cSyncEventParam : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public string DisplayMessage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v_sMessage"></param>
        public cSyncEventParam(string v_sMessage)
        {
            this.DisplayMessage = v_sMessage;
        }
    }
}
