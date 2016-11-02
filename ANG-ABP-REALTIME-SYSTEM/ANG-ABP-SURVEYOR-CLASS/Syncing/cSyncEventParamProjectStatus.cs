using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG_ABP_SURVEYOR_APP_CLASS.Syncing
{
    public class cSyncEventParamProjectStatus
    {


         /// <summary>
        /// 
        /// </summary>
        public string PrevProjectNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string NextProjectNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PrevProjectSubProjectsAdded { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PrevProjectSubProjectsDeleted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool UpdateSuccess { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v_sPrevProjectNo"></param>
        /// <param name="v_sNextProjectNo"></param>
        /// <param name="v_iPrevProjectSubProjectsAdded"></param>
        /// <param name="v_iPrevProjectSubProjectsDeleted"></param>
        /// <param name="v_bUpdateSuccess"></param>
        public cSyncEventParamProjectStatus(string v_sPrevProjectNo, string v_sNextProjectNo, int v_iPrevProjectSubProjectsAdded, int v_iPrevProjectSubProjectsDeleted, bool v_bUpdateSuccess)
        {
            this.PrevProjectNo = v_sPrevProjectNo;
            this.NextProjectNo = v_sNextProjectNo;
            this.PrevProjectSubProjectsAdded = v_iPrevProjectSubProjectsAdded;
            this.PrevProjectSubProjectsDeleted = v_iPrevProjectSubProjectsDeleted;
            this.UpdateSuccess = v_bUpdateSuccess;
        }
    }
}
