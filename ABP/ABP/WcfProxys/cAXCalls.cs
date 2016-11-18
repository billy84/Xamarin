using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ABP.Interfaces;

namespace ABP.WcfProxys
{
    class cAXCalls
    {
        public ServiceExt.ServiceClient m_wcfClient = null;
        public string m_cCompanyName = string.Empty;
        public string m_sPurpose = string.Empty;
        public cAXCalls()
        {
            try
            {
                m_wcfClient = new ServiceExt.ServiceClient();
                m_wcfClient.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, 60, 0);
                m_wcfClient.Endpoint.Binding.SendTimeout = new TimeSpan(0, 60, 0);
                m_wcfClient.Endpoint.Address = new System.ServiceModel.EndpointAddress("https://abpwebtest.anglian-windows.com/ax-surv-service-ext-116/service.svc");
                m_cCompanyName = cSettings.p_sSetting_AXCompany;
                m_sPurpose = "Survey";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
