using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.WcfProxys
{
    internal class WcfLogin
    {
        //private WcfLogin
        public static LoginExt.ServiceClient m_wcfLogin = null;
        public static string Token = string.Empty;
        public WcfLogin()
        {
            m_wcfLogin = new LoginExt.ServiceClient();
            m_wcfLogin.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, 60, 0);
            m_wcfLogin.Endpoint.Binding.SendTimeout = new TimeSpan(0, 60, 0);
            m_wcfLogin.Endpoint.Address = new System.ServiceModel.EndpointAddress("https://abpwebtest.anglian-windows.com/ax-logon-ext-test/service.svc");
        }
        public static WcfLogin getInstance()
        {
            return null;
            //if ()
        }
    }
}
