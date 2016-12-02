using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.UWP.Service;
using Anglian.Service;
using Anglian.Classes;
using Xamarin.Forms;

[assembly: Dependency(typeof(Logon))]
namespace Anglian.UWP.Service
{
    class Logon : ILogon
    {
        private WcfLogin.ServiceClient m_loginClient = new WcfLogin.ServiceClient();
        public async Task<LogonResult> LogonAsync(string v_sUsername, string v_sPassword, string v_sAuthID)
        {
            LogonResult rtn = new LogonResult();
            try
            {
                WcfLogin.LogonResult sResult = await m_loginClient.LogonAsync(v_sUsername, v_sPassword, v_sAuthID);
                if (sResult.CallSuccessfull == true)
                {
                    rtn.CallSuccessfull = sResult.CallSuccessfull;
                    rtn.AccountDisabled = sResult.AccountDisabled;
                    rtn.InvalidDetails = sResult.InvalidDetails;
                    rtn.Token = sResult.Token;
                }
                return rtn;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
