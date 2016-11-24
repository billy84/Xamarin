using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.WcfProxys
{
    class cSyncEventParamProjectStatus
    {
        public string PrevProjectNo { get; set; }
        public string NextProjectNo { get; set; }
        public int PrevProjectSubProjectsAdded { get; set; }
        public int PrevProjectSubProjectsDeleted { get; set; }
        public bool UpdateSuccess { get; set; }
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
