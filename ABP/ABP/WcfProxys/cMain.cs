using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABP.TableModels;
using ABP.Interfaces;
using Xamarin.Forms;

namespace ABP.WcfProxys
{
    public class cMain
    {
        public static cDataAccess p_cDataAccess = null;
        public static cSettings p_cSettings = new cSettings();
        public static void InitialiseDB()
        {
            try
            {
                cMain.p_cDataAccess = new cDataAccess();
                cMain.CreateSettingsRecord();
                cMain.p_cDataAccess.CheckDB();
            }
            catch (Exception ex)
            {

            }
        }
        public static void CreateSettingsRecord()
        {
            bool bChangesMade = false;
            try
            {
                cAppSettingsTable cSetting = cMain.p_cDataAccess.ReturnSettings();
                if (cSetting == null)
                {
                    cSetting = new cAppSettingsTable();
                    bChangesMade = true;
                }
                if (cSetting.RunningMode == null || cSetting.RunningMode.Length == 0)
                {
                    cSetting.RunningMode = DependencyService.Get<IMain>().GetAppResourceValue("STATUS");
                    bChangesMade = true;
                }
                if (bChangesMade == true)
                {
                    cMain.p_cDataAccess.SaveSettings(cSetting);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
