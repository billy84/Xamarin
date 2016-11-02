using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG_ABP_SURVEYOR_APP_CLASS.Syncing
{
    public class cSyncEventErrorParams : EventArgs 
    {

         /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string ProjectNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v_sMessage"></param>
        public cSyncEventErrorParams(string v_sErrorMessage,string v_sProjectNo)
        {
            this.ErrorMessage = v_sErrorMessage;
            this.ProjectNo = v_sProjectNo;
        }

    }
}
