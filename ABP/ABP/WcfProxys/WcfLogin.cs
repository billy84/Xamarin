using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.WcfProxys
{
    public partial class WcfLogin
    {
        public static WcfLogin m_instance { get; private set; }
        //private WcfLogin
        public LoginExt.ServiceClient m_wcfLogin = null;
        public string Token = string.Empty;
        public string LoggedUserName = string.Empty;
        static WcfLogin()
        {
            m_instance = new WcfLogin();
        }
        private WcfLogin()
        {
            m_wcfLogin = new LoginExt.ServiceClient();
            m_wcfLogin.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, 60, 0);
            m_wcfLogin.Endpoint.Binding.SendTimeout = new TimeSpan(0, 60, 0);
            m_wcfLogin.Endpoint.Address = new System.ServiceModel.EndpointAddress("https://abpwebtest.anglian-windows.com/ax-logon-ext-test/service.svc");
        }
    }
}
