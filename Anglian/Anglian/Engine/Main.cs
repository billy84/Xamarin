using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Anglian.Models;
using Anglian.Classes;
using Anglian.Service;
using Xamarin.Forms;

namespace Anglian.Engine
{
    public class Main
    {
        /// <summary>
        /// Data access variable
        /// </summary>
        public static DataAccess p_cDataAccess = null;

        /// <summary>
        /// Setting class.
        /// </summary>
        public static Settings p_cSettings = new Settings();

        /// <summary>
        /// For recording input in the survey input screen, is used to then redisplay when the user navigates back to the page.
        /// </summary>
        public static cProjectTable p_cSurveyInputScreenData = null;

        /// <summary>
        /// v1.0.4 - Used for recording survey input selections.
        /// </summary>
        public static cProjectTable p_cSurveyInputCopiedSelections = null;

        /// <summary>
        /// v1.0.6 - Used for recording last note on surveyor input screen.
        /// </summary>
        public static cProjectNotesTable p_cSurveyInputCopiedLastNote = null;


        /// <summary>
        /// For recording notes input in the survey input screen, is used to then redisplay when the user navigates back to the page.
        /// </summary>
        public static List<cProjectNotesTable> p_cSurveyInputProjectNotes = null;

        /// <summary>
        /// v1.0.21 - Flag to indicate that failed survey reasons are being checked for.
        /// </summary>
        private static bool m_bCheckingSurveyFailedReasons = false;

        /// <summary>
        /// Flag to indicate that base enums are being checked for.
        /// </summary>
        private static bool m_bCheckingBaseEnums = false;

        /// <summary>
        /// Flag to indicate that settings are being checked for.
        /// </summary>
        private static bool m_bCheckingSetings = false;

        /// <summary>
        /// Format for comparing date times.
        /// </summary>
        public const string p_sCompareDateFormat = "yyyyMMddHHmmss";

        /// <summary>
        /// Search criteria for the survey dates search screen.
        /// </summary>
        public static SearchCriteria p_cSearchCriteria_LastSearch = null;

        /// <summary>
        /// List of project photos, used on the photo page.
        /// </summary>




        /// <summary>
        /// v1.0.11 - Flag to indicate syncing is in progress.
        /// </summary>
        public static bool p_bIsSyncingInProgress = false;


        /// <summary>
        /// Image name parts.
        /// </summary>
        private enum ImageNameParts
        {
            Date = 0
            , SubProjectNo = 1
            , FileName = 2

        }
        /// <summary>
        /// v1.0.11 - Initialize database
        /// </summary>
        public static void InitialiseDB()
        {

            try
            {

                //Create data object and check database is setup.
                Main.p_cDataAccess = new DataAccess();
                Main.CreateSettingsRecord();
                Main.p_cDataAccess.CheckDB();

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Create work display title from survey date.
        /// </summary>
        /// <param name="v_dSurveyDate"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Convert null able date time to date time.
        /// </summary>
        /// <param name="v_dDateTime"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v_cWorkDB"></param>
        /// <returns></returns>
        public static string ReturnAddress(cProjectTable v_cWorkDB)
        {
            string sAddress = string.Empty;
            try
            {

                sAddress = Main.RemoveNewLinesFromString(v_cWorkDB.DeliveryStreet);

                if (v_cWorkDB.DeliveryCity.Length > 0)
                {
                    if (sAddress.Length > 0)
                    {
                        sAddress += ", ";
                    }

                    sAddress += Main.RemoveNewLinesFromString(v_cWorkDB.DeliveryCity);
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

                    sAddress += Main.RemoveNewLinesFromString(v_cWorkDB.DlvZipCode);
                }

                return sAddress;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return sAddress;

            }


        }
        /// <summary>
        /// Remove new line chars from string.
        /// </summary>
        /// <param name="v_sString"></param>
        /// <returns></returns>
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
        /// <summary>
        /// v1.0.2 - Create settings record if not already there.
        /// </summary>
        public async static void CreateSettingsRecord()
        {

            bool bChangesMade = false;
            try
            {

                cAppSettingsTable cSetting = Main.p_cDataAccess.ReturnSettings();
                if (cSetting == null)
                {
                    cSetting = new cAppSettingsTable();
                    bChangesMade = true;
                }


                if (cSetting.RunningMode == null || cSetting.RunningMode.Length == 0)
                {
                    cSetting.RunningMode = DependencyService.Get<IMain>().GetAppResourceValue("STATUS"); //v1.0.11 - Default to environment in resources file.
                    bChangesMade = true;

                }

                if (bChangesMade == true)
                {
                    await Main.p_cDataAccess.SaveSettings(cSetting);
                }


            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }


        /// <summary>
        /// v1.0.19 - Should I check for survey reasons result.
        /// </summary>
        private struct ShouldICheckForSurveyReasonResult
        {

            /// <summary>
            /// 
            /// </summary>
            public bool bCheck;

            /// <summary>
            /// 
            /// </summary>
            public DateTime dLastUpdate;

        }
        /// <summary>
        /// v1.0.19 - Check for updates.
        /// </summary>
        public async static Task CheckForUpdates()
        {

            try
            {

                //Check for base enum updates
                await Main.CheckForBaseEnums();

                //v1.0.1 - Check for setting updates
                await Main.CheckForSettings();

                //v1.0.19 - Check for failed survey reasons.
                await Main.CheckForFailedSurveyReasons();

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }
        private static bool ShouldICheckForBaseEnums()
        {

            bool bShouldICheck = false;
            try
            {

                int iDaysBetweenChecks = Convert.ToInt32(DependencyService.Get<IMain>().GetAppResourceValue("CheckBaseEnumDaysBetweenChecks").ToString());

                cAppSettingsTable cSettings = Main.p_cDataAccess.ReturnSettings();
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
        /// <summary>
        /// Should I check for settings
        /// </summary>
        /// <returns></returns>
        private static bool ShouldICheckForSettings()
        {

            bool bShouldICheck = false;
            try
            {

                int iDaysBetweenChecks = Convert.ToInt32(DependencyService.Get<IMain>().GetAppResourceValue("CheckSettingsDaysBetweenChecks").ToString());

                cAppSettingsTable cSettings = Main.p_cDataAccess.ReturnSettings();
                if (cSettings != null)
                {

                    if (cSettings.LastSettingsCheckDateTime.HasValue == true)
                    {
                        TimeSpan tsDiff = DateTime.Now.Subtract(cSettings.LastSettingsCheckDateTime.Value);
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
        /// <summary>
        /// Check for settings
        /// </summary>
        /// <returns></returns>
        public async static Task CheckForSettings()
        {

            WcfExt116 cAX = null;
            try
            {

                //If already checking then leave.
                if (Main.m_bCheckingSetings == true) { return; }

                bool bCheck = Main.ShouldICheckForSettings();
                if (bCheck == true)
                {

                    //For calling AX wcf
                    cAX = new WcfExt116();


                    //Fetch local list of base enums
                    List<SettingDetails> sdSetting = Main.p_cDataAccess.GetSettingsUpdates();

                    //Get update settings from AX
                    List<SettingDetails> beSettingsUpdate = await cAX.ReturnUpdatedSettings(sdSetting);

                    //Close connection.
                    if (cAX != null)
                    {
                        await DependencyService.Get<IWcfExt116>().CloseAXConnection();
                    }

                    await Main.p_cDataAccess.ProcessUpdatedSettings(beSettingsUpdate);


                }

                //Reset checking flag.
                Main.m_bCheckingSetings = false;

            }
            catch (Exception ex)
            {

                Main.m_bCheckingBaseEnums = false; //Reset checking flag.
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.19 - Check for Failed Survey reasons.
        /// </summary>
        public async static Task CheckForFailedSurveyReasons()
        {

            WcfExt116 cAX = null;
            try
            {

                //If already checking then leave.
                if (Main.m_bCheckingSurveyFailedReasons == true) { return; }
                Main.m_bCheckingSurveyFailedReasons = true;

                ShouldICheckForSurveyReasonResult cCheckResult = Main.ShouldICheckForFailedSurveyReasons();
                if (cCheckResult.bCheck == true)
                {

                    //For calling AX wcf
                    cAX = new WcfExt116();

                    //Get update base enums from AX
                    FetchSurveyFailedReasonsResult fsrResult = await DependencyService.Get<IWcfExt116>().FetchFailedSurveyReasons(
                        cAX.m_cCompanyName,                        
                        cCheckResult.dLastUpdate,
                        Settings.p_sSetting_AuthID,
                        Session.Token);

                    if (fsrResult != null)
                    {

                        if (fsrResult.bSuccessfull == true)
                        {

                            cAppSettingsTable cSettings = Main.p_cDataAccess.ReturnSettings();

                            if (fsrResult.sfrReasons.Count > 0)
                            {

                                bool bUpdateOK = Main.p_cDataAccess.UpdateFailedSurveyReasonsTable(fsrResult.sfrReasons);
                                if (bUpdateOK == true)
                                {

                                    cSettings.LastSurveyFailedUpdateDateTime = fsrResult.bLastUpdateDate;

                                }


                            }

                            cSettings.LastSurveyFailedCheckDateTime = DateTime.Now;

                            await Main.p_cDataAccess.SaveSettings(cSettings);

                        }

                    }

                }

                //Close connection.
                if (cAX != null)
                {
                    await DependencyService.Get<IWcfExt116>().CloseAXConnection();
                }

                //Reset checking flag.
                Main.m_bCheckingSurveyFailedReasons = false;

            }
            catch (Exception ex)
            {

                Main.m_bCheckingSurveyFailedReasons = false; //Reset checking flag.
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }
        /// <summary>
        /// v1.0.21 - Should I check for failed survey reasons.
        /// </summary>
        /// <returns></returns>
        private static ShouldICheckForSurveyReasonResult ShouldICheckForFailedSurveyReasons()
        {

            ShouldICheckForSurveyReasonResult cResult = new ShouldICheckForSurveyReasonResult();
            cResult.bCheck = false;
            cResult.dLastUpdate = DateTime.MinValue;

            try
            {

                int iDaysBetweenChecks = Convert.ToInt32(DependencyService.Get<IMain>().GetAppResourceValue("CheckFailedReasonsDaysBetweenChecks").ToString());

                cAppSettingsTable cSettings = Main.p_cDataAccess.ReturnSettings();
                if (cSettings != null)
                {

                    if (cSettings.LastSurveyFailedCheckDateTime.HasValue == true)
                    {
                        TimeSpan tsDiff = DateTime.Now.Subtract(cSettings.LastSurveyFailedCheckDateTime.Value);
                        if (tsDiff.TotalDays >= iDaysBetweenChecks)
                        {
                            cResult.bCheck = true;
                            cResult.dLastUpdate = cSettings.LastSurveyFailedUpdateDateTime.Value;
                        }

                    }
                    else
                    {
                        cResult.bCheck = true;
                    }

                }
                else
                {
                    cResult.bCheck = true;
                }

                return cResult;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return cResult;

            }

        }

        /// <summary>
        /// Check for base enums
        /// </summary>
        public async static Task CheckForBaseEnums()
        {

            WcfExt116 cAX = null;
            try
            {

                //If already checking then leave.
                if (Main.m_bCheckingBaseEnums == true) { return; }

                bool bCheck = Main.ShouldICheckForBaseEnums();
                if (bCheck == true)
                {

                    //For calling AX wcf
                    cAX = new WcfExt116();

                    //Fetch local list of base enums
                    List<BaseEnumField> beFields = Main.p_cDataAccess.GetBaseEnumUpdates();

                    //Get update base enums from AX
                    List<BaseEnumField> beFieldsUpdate = await cAX.ReturnUpdatedBaseEnums(beFields);

                    //Close connection.
                    if (cAX != null)
                    {
                        await DependencyService.Get<IWcfExt116>().CloseAXConnection();
                    }

                    await Main.p_cDataAccess.ProcessUpdatedBaseEnums(beFieldsUpdate);


                }

                //Reset checking flag.
                Main.m_bCheckingBaseEnums = false;

            }
            catch (Exception ex)
            {

                Main.m_bCheckingBaseEnums = false; //Reset checking flag.
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }
        public static async Task CheckAXConnection()
        {


            //We do not want more than 1 instance running at a time.
            //m_dpDispatcher.Stop();

            bool bOnline = false;

            try
            {


                //Windows.UI.Xaml.Controls.Frame fmFrame = (Windows.UI.Xaml.Controls.Frame)Window.Current.Content;
                //Windows.UI.Xaml.Controls.Page pgPage = (Windows.UI.Xaml.Controls.Page)fmFrame.Content;

                bOnline = await Main.p_cSettings.IsAXSystemAvailable(false);

                if (bOnline == true)
                {
                    await Main.CheckForUpdates();

                }
                else
                {

                }

            }
            catch (Exception ex)
            {
                //Main.ReportError(ex, Main.GetCallerMethodName(), string.Empty);

            }
            finally
            {

                //Restart timer.
                //m_dpDispatcher.Start();

            }

        }
        /// <summary>
        /// Update screen after syncing is complete.
        /// </summary>
        public static async void UpdateScreenAfterSyncing()
        {

            try
            {

                //Retrieve settings.
                cAppSettingsTable cSettings = Main.p_cDataAccess.ReturnSettings();

                //Update last date time.
                cSettings.LastSyncDateTime = DateTime.Now;

                //Save settings
                await Main.p_cDataAccess.SaveSettings(cSettings);
            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
            }

        }

        /// <summary>
        /// Start syncing
        /// </summary>
        public async static Task StartSyncing(bool v_bUploadChangesOnly)
        {

            Syncing cSync = null;
            bool bProcessedOK = false;
            try
            {

                Main.p_bIsSyncingInProgress = true;

                bool bConnected = await Main.p_cSettings.IsAXSystemAvailable(true);
                if (bConnected == true)
                {

                    cSync = new Syncing();
                    //cSync.SubProjectStatusUpdate += cSync_SubProjectStatusUpdate;
                    //cSync.UpdateMessage += cSync_UpdateMessage;
                    //cSync.DisplayMessage += cSync_DisplayMessage;
                    //cSync.ProjectSyncError += cSync_ProjectSyncError;

                    bProcessedOK = await cSync.UploadChanges(false);
                    if (bProcessedOK == true)
                    {

                        bProcessedOK = await cSync.UploadPhotos();

                        //If uploading changes only, then leave here.
                        if (v_bUploadChangesOnly == true)
                        {
                            return;
                        }

                        bProcessedOK = await cSync.CheckForDataChanges(false);

                        if (bProcessedOK == true)
                        {



                        }

                    }

                }


            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
            }
            finally
            {
                Main.p_bIsSyncingInProgress = false;
            }

        }

        public static string ReturnLastSyncString(DateTime? v_dSyncDate)
        {

            try
            {

                StringBuilder sDisplayText = new StringBuilder();

                if (v_dSyncDate.HasValue == false)
                {
                    sDisplayText.Append("Data has never been synced.");

                }
                else
                {
                    sDisplayText.Append("Data last synced on " + v_dSyncDate.Value.ToString("dd/MM/yyyy") + " @ " + v_dSyncDate.Value.ToString("HH:mm"));



                }

                int iUploads = Main.p_cDataAccess.GetNumberOfUploadsPending();
                int iNotes = Main.p_cDataAccess.GetNumberOfNotesPending(); //v1.0.1
                int iFiles = Main.p_cDataAccess.GetNumberOfFilesPending();

                //sDisplayText.Append(Environment.NewLine);
                sDisplayText.Append(" - (" + (iUploads + iFiles + iNotes).ToString() + ") changes waiting for upload.");

                return sDisplayText.ToString();

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return string.Empty;

            }

        }

        /// <summary>
        /// Return selected items text value from combo box
        /// </summary>
        /// <param name="v_cmbCombo"></param>
        /// <returns></returns>
        public static string ReturnComboSelectedItemText(Picker v_cmbCombo)
        {

            string sRtnValue = string.Empty;
            try
            {

                string cmbItem = v_cmbCombo.Items[v_cmbCombo.SelectedIndex];
                if (cmbItem != null)
                {
                    sRtnValue = cmbItem;

                }

                return sRtnValue;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return string.Empty;
            }
        }

        public static string ReturnComboSelectedTagValue(Picker v_cmbCombo)
        {

            string sRtnValue = string.Empty;
            try
            {

                int selectedIndex = v_cmbCombo.SelectedIndex;
                if (selectedIndex != -1)
                {
                    sRtnValue = selectedIndex.ToString();

                }

                return sRtnValue;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return string.Empty;
            }
        }

        public static string ReturnDisplayTime(DateTime v_dDate)
        {

            try
            {

                string sTime = String.Empty;

                string sNowTime = v_dDate.ToString("HH:mm");
                string sAMTime = DependencyService.Get<IMain>().GetAppResourceValue("AM_TIME");
                string sPMTime = DependencyService.Get<IMain>().GetAppResourceValue("PM_TIME");

                if (sNowTime == sAMTime)
                {
                    sTime = Settings.p_sTime_AM;

                }
                else if (sNowTime == sPMTime)
                {
                    sTime = Settings.p_sTime_PM;

                }
                else
                {
                    sTime = sNowTime;

                }

                return sTime;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return string.Empty;

            }
        }

        public static string ReturnDisplayDate(DateTime v_dtDate)
        {

            try
            {
                return ReturnDisplayDate(v_dtDate, string.Empty);

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return string.Empty;

            }

        }
        public static string ReturnDisplayDate(DateTime v_dtDate, string v_sDateCompare)
        {

            try
            {

                string sSymbol = String.Empty;
                if (v_sDateCompare == Settings.p_sDateCompare_GreaterThan)
                {
                    sSymbol = "> ";
                }

                if (v_sDateCompare == Settings.p_sDateCompare_LessThan)
                {
                    sSymbol = "< ";
                }

                string sDayName = sSymbol + v_dtDate.DayOfWeek.ToString().Substring(0, 3);
                return sDayName + " " + v_dtDate.ToString("dd/MM/yyyy");

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return string.Empty;

            }

        }
    }
}
