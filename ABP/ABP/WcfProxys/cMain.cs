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
        public static bool m_bCheckingBaseEnums = false;
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
        public static string CreateWorkDisplayTitle(DateTime v_dSurveyDate)
        {

            try
            {

                //Retrieve AM and PM fixed time.
                string sAM_Time = DependencyService.Get<IMain>().GetAppResourceValue("AM_TIME");
                string sPM_Time = DependencyService.Get<IMain>().GetAppResourceValue("PM_TIME");

                //Build up display string.
                string sDisplay = "Survey " + v_dSurveyDate.ToString("dd/MM/yyyy") + " @ ";

                //Check too see what time needs to be displayed
                string sTime = v_dSurveyDate.ToString("HH:mm");
                if (sTime == sAM_Time)
                {
                    sDisplay += "AM";

                }
                else if (sTime == sPM_Time)
                {
                    sDisplay += "PM";

                }
                else
                {
                    sDisplay += sTime;

                }

                return sDisplay;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return null;

            }

        }
        public static DateTime ConvertNullableDateTimeToDateTime(DateTime? v_dDateTime)
        {

            try
            {

                if (v_dDateTime.HasValue == false)
                {
                    return new DateTime();

                }
                else
                {
                    return v_dDateTime.Value;

                }

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return new DateTime();


            }

        }
        public static string ReturnAddress(cProjectTable v_cWorkDB)
        {
            string sAddress = string.Empty;
            try
            {

                sAddress = cMain.RemoveNewLinesFromString(v_cWorkDB.DeliveryStreet);

                if (v_cWorkDB.DeliveryCity.Length > 0)
                {
                    if (sAddress.Length > 0)
                    {
                        sAddress += ", ";
                    }

                    sAddress += cMain.RemoveNewLinesFromString(v_cWorkDB.DeliveryCity);
                }

                if (v_cWorkDB.DlvState.Length > 0)
                {
                    if (sAddress.Length > 0)
                    {
                        sAddress += ", ";
                    }

                    sAddress += v_cWorkDB.DlvState;
                }

                if (v_cWorkDB.DlvZipCode.Length > 0)
                {
                    if (sAddress.Length > 0)
                    {
                        sAddress += ", ";
                    }

                    sAddress += cMain.RemoveNewLinesFromString(v_cWorkDB.DlvZipCode);
                }

                return sAddress;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return sAddress;

            }


        }
        public static string RemoveNewLinesFromString(string v_sString)
        {

            string sNewString = String.Empty;
            int iSpacesInARow = 0;
            try
            {

                foreach (char cChar in v_sString)
                {

                    if (char.IsControl(cChar) == false)
                    {
                        sNewString += cChar;
                        iSpacesInARow = 0;
                    }
                    else
                    {
                        if (iSpacesInARow == 0)
                        {
                            sNewString += " ";
                            iSpacesInARow += 1;
                        }

                    }

                }

                return sNewString;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return v_sString;
            }
        }
        public static bool ShouldICheckForBaseEnums()
        {

            bool bShouldICheck = false;
            try
            {

                int iDaysBetweenChecks = Convert.ToInt32(DependencyService.Get<IMain>().GetAppResourceValue("CheckBaseEnumDaysBetweenChecks").ToString());

                cAppSettingsTable cSettings = cMain.p_cDataAccess.ReturnSettings();
                if (cSettings != null)
                {

                    if (cSettings.LastBaseEnumCheckDateTime.HasValue == true)
                    {
                        TimeSpan tsDiff = DateTime.Now.Subtract(cSettings.LastBaseEnumCheckDateTime.Value);
                        if (tsDiff.TotalDays >= iDaysBetweenChecks)
                        {
                            bShouldICheck = true;

                        }

                    }
                    else
                    {
                        bShouldICheck = true;
                    }

                }
                else
                {
                    bShouldICheck = true;
                }

                return bShouldICheck;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }

        }
    }
}
