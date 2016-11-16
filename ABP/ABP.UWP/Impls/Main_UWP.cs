using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ABP.UWP.Impls;
using ABP.Interfaces;
[assembly: Dependency(typeof(Main_UWP))]
namespace ABP.UWP.Impls
{
    class Main_UWP : IMain
    {
        private Windows.ApplicationModel.Resources.ResourceLoader m_rlResources = new Windows.ApplicationModel.Resources.ResourceLoader();
        public string GetAppResourceValue(string v_sResourceName)
        {
            try
            {
                return this.m_rlResources.GetString(v_sResourceName);

            }
            catch (Exception ex)
            {
                return ex.Message;

            }
        }
    }
}
