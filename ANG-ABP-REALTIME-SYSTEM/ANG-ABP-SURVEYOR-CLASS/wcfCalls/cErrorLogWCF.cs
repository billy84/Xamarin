using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG_ABP_SURVEYOR_APP_CLASS.wcfCalls
{
    public class cErrorLogWCF
    {

        /// <summary>
        /// Error logging variable.
        /// </summary>
        private wcfErrorLog.ServiceClient m_wcfErrorLog = null;

        private string m_sStatus = string.Empty;


        /// <summary>
        /// Constructor
        /// </summary>
        public cErrorLogWCF(string v_sStatus)
        {
            this.m_sStatus = v_sStatus;

        }


        /// <summary>
        /// Log error.
        /// </summary>
        /// <param name="v_eiInfo"></param>
        public void LogError(wcfErrorLog.ErrorInfo v_eiInfo)
        {

            try
            {

                this.m_wcfErrorLog = new wcfErrorLog.ServiceClient();
                this.m_wcfErrorLog.ReportErrorAsync(this.m_sStatus, v_eiInfo);

            }
            catch (Exception ex)
            {



            }
            finally
            {
                this.m_wcfErrorLog.CloseAsync();

            }

        }

    }
}
