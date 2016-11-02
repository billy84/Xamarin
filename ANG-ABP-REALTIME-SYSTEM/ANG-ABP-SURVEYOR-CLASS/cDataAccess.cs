using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.IO;
using ANG_ABP_SURVEYOR_APP_CLASS.Model;
using ANG_ABP_SURVEYOR_APP_CLASS.Classes;
using System.Collections.ObjectModel;
using ANG_ABP_SURVEYOR_APP_CLASS;

namespace ANG_ABP_SURVEYOR_APP_CLASS
{
    public class cDataAccess
    {

        /// <summary>
        /// SQLite database connection object.
        /// </summary>
        private SQLiteConnection m_conSQL = null;

        /// <summary>
        /// v1.0.11 - SQLite settings database connection object.
        /// </summary>
        private SQLiteConnection m_conSQL_Settings = null;

        /// <summary>
        /// Flag to indicate if we are running the surveyor app.
        /// </summary>
        private bool m_bSurveyorApp = false;

        /// <summary>
        /// v1.0.2 - Save sub project data result.
        /// </summary>
        public struct SaveSubProjectDataResult
        {

            /// <summary>
            /// Saved OK.
            /// </summary>
            public bool bSavedOK;

            /// <summary>
            /// Flag if project added, otherwise updated.
            /// </summary>
            public bool bProjectAdded;

        }

        /// <summary>
        /// v1.0.21 - Health and safety incomplete filters.
        /// </summary>
        public enum HSFilters
        {

            Any = 0,
            InComplete = 1,
            Complete = 2

        }

        /// <summary>
        /// Constructor
        /// </summary>
        public cDataAccess()
        {

            try
            {

                //v1.0.8 - Set the flag indicating if this is the surveyor app.
                this.m_bSurveyorApp = cSettings.IsThisTheSurveyorApp();
                                        
                //v1.0.11 - Check settings database exists.
                this.CheckSettingsDB();
                               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// v1.0.11 - Check settings database exists
        /// </summary>
        private void CheckSettingsDB()
        {

            try
            {               
                

                //Specify Name
                string sDBFileName = "surveyordbsettings.sqlite";

                //v1.0.8 - Set appropriate name for installer application.
                if (this.m_bSurveyorApp == false)
                {
                    sDBFileName = "installerdbsettings.sqlite";

                }

                //Create connection object.
                string sFullPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sDBFileName);
                this.m_conSQL_Settings = new SQLiteConnection(sFullPath);

                //v1.0.11 - Check settings database exists.
                if (this.m_conSQL_Settings.TableMappings.Count() == 0)
                {
                    this.m_conSQL_Settings.CreateTable<cAppSettingsTable>();

                }               

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void CheckDB()
        {

            try
            {

                //Retrieve database name from resources file.
                string sDBFileName = "surveyordbmain.sqlite";

                if (this.m_bSurveyorApp == false)
                {
                    sDBFileName = "installerdbmain.sqlite";

                }

                //v1.0.11 - If in test then use different database.
                if (this.AreWeRunningInLive() == false)
                {
                    sDBFileName = "surveyordbmain_test.sqlite";

                    if (this.m_bSurveyorApp == false)
                    {
                        sDBFileName = "installerdbmain_test.sqlite";

                    }

                }

                //Create connection object.
                string sFullPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sDBFileName);
                this.m_conSQL = new SQLiteConnection(sFullPath);

                //If no tables then we need to create.
                if (this.m_conSQL.TableMappings.Count() == 0)
                {

                    this.m_conSQL.CreateTable<cProjectTable>();
                    this.m_conSQL.CreateTable<cProjectFilesTable>();
                    this.m_conSQL.CreateTable<cUpdatesTable>();
                    this.m_conSQL.CreateTable<cBaseEnumsTable>();
                    this.m_conSQL.CreateTable<cBaseEnumUpdateTable>();
                    this.m_conSQL.CreateTable<cAXSettingsTable>(); //v1.0.1
                    this.m_conSQL.CreateTable<cProjectNotesTable>(); //v1.0.1
                    this.m_conSQL.CreateTable<cFailedSurveyReasonsTable>(); //v1.0.19

                    //If installer application then we need to create additional tables.
                    if (this.m_bSurveyorApp == false)
                    {
                        this.m_conSQL.CreateTable<cUnitsTable>();
                        this.m_conSQL.CreateTable<cInstallersTable>();
                        this.m_conSQL.CreateTable<cUnitsUpdateTable>();

                    }

                }


             


                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }      

        /// <summary>
        /// Return application settings
        /// </summary>
        /// <returns></returns>
        public cAppSettingsTable ReturnSettings()
        {

            cAppSettingsTable rtnTable = null;

            try
            {

                var oResults = from oCols in this.m_conSQL_Settings.Table<cAppSettingsTable>() select oCols;
                foreach (var oResult in oResults)
                {
                    rtnTable = oResult;

                }

              

                return rtnTable;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);                
            }

        }

        /// <summary>
        /// Save settings to DB
        /// </summary>
        /// <param name="v_cSettings"></param>
        /// <returns></returns>
        public async Task<bool> SaveSettings(cAppSettingsTable v_cSettings)
        {

            try
            {

                v_cSettings.UserProfile = await cSettings.GetUserName();
                v_cSettings.UsersFullName = await cSettings.GetUserDisplayName();

                if (v_cSettings.IDKey == 0) //If IDKey is 0 then we need to insert a new one.
                {
                    this.m_conSQL_Settings.Insert(v_cSettings);

                }
                else //Update
                {
                    this.m_conSQL_Settings.Update(v_cSettings);

                }

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }


        }


        /// <summary>
        /// Returns the number of survey visits booked for today.
        /// </summary>
        /// <returns></returns>
        public int TotalsForToday_Surveyor(string v_sUserName, int v_iInstallStatus_AwaitingSurvey)
        {

            StringBuilder sbSQL = new StringBuilder();
            int iTotal = 0;
            try
            {
               
                List<cTotal> Total = null;

                DateTime dNowStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
                DateTime dNowEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                
                sbSQL.Append("SELECT Count(*) AS Total FROM cProjectTable");
                //sbSQL.Append(" WHERE MXM1002TrfDate ISNULL");
                sbSQL.Append(" WHERE (MxmConfirmedAppointmentIndicator = '1'");
                sbSQL.Append(" OR ((MxmConfirmedAppointmentIndicator = '0' OR MxmConfirmedAppointmentIndicator IS NULL)");
                sbSQL.Append("      AND EndDateTime IS NOT NULL AND StartDateTime IS NOT NULL))");

                sbSQL.Append(" AND EndDateTime >= '" + dNowStart.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                sbSQL.Append(" AND EndDateTime <= '" + dNowEnd.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                sbSQL.Append(" AND SurveyorProfile='" + v_sUserName + "'");
                sbSQL.Append(" AND Mxm1002InstallStatus=" + v_iInstallStatus_AwaitingSurvey);                
                sbSQL.Append(" ORDER BY EndDateTime");

                Total = this.m_conSQL.Query<cTotal>(sbSQL.ToString());


                foreach (cTotal TTl in Total)
                {
                    if (TTl.Total.HasValue == true)
                    {
                        iTotal = TTl.Total.Value;
                    }
                }

                return iTotal;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SQL(" + sbSQL.ToString() + ")");

            }

        }

        /// <summary>
        /// Total surveys completed.
        /// </summary>
        /// <returns></returns>
        public int TotalCompleted_Surveyor(string v_sUserName, int v_iInstallStatus_AwaitingSurvey)
        {

            StringBuilder sbSQL = new StringBuilder();

            try
            {

                int iTotal = 0;
                
                DateTime dNowStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
                DateTime dNowEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

                List<cTotal> Total = null;
                
                sbSQL.Append("SELECT Count(*) AS Total FROM cProjectTable");
                //sbSQL.Append(" WHERE MXM1002TrfDate ISNULL");
                sbSQL.Append(" WHERE (MXM1002TrfDate IS NOT NULL");
                sbSQL.Append(" OR ((MxmConfirmedAppointmentIndicator = '0' OR MxmConfirmedAppointmentIndicator IS NULL)");
                sbSQL.Append("      AND EndDateTime IS NOT NULL AND StartDateTime IS NOT NULL))");

                sbSQL.Append(" AND EndDateTime >= '" + dNowStart.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                sbSQL.Append(" AND EndDateTime <= '" + dNowEnd.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                sbSQL.Append(" AND SurveyorProfile='" + v_sUserName + "'");
                sbSQL.Append(" AND Mxm1002InstallStatus=" + v_iInstallStatus_AwaitingSurvey);
                //sbSQL.Append(" AND Mxm1002ProgressStatus=" + v_iProgressStatus_AbleToProgress);
                sbSQL.Append(" ORDER BY EndDateTime");

                Total = this.m_conSQL.Query<cTotal>(sbSQL.ToString());

                foreach (cTotal TTl in Total)
                {
                    if (TTl.Total.HasValue == true)
                    {
                        iTotal = TTl.Total.Value;
                    }
                }

                return iTotal;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SQL(" + sbSQL.ToString() + ")");

            }


        }

        /// <summary>
        /// Total pending.
        /// </summary>
        /// <returns></returns>
        public int TotalPending_Surveyor(string v_sUserName, int v_iInstallStatus_AwaitingSurvey)
        {

            try
            {
                int iTotal = 0;
                
                DateTime dNowStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
                DateTime dNowEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);


                var oResults = from oCols in this.m_conSQL.Table<cProjectTable>()
                               where (oCols.MXM1002TrfDate == null)
                               && (oCols.StartDateTime >= dNowStart) && (oCols.EndDateTime <= dNowEnd)
                               && (oCols.MxmConfirmedAppointmentIndicator == 1)
                               && (oCols.SurveyorProfile.Equals(v_sUserName)
                               && (oCols.Mxm1002InstallStatus.Equals(v_iInstallStatus_AwaitingSurvey)))                               
                               select oCols;

                iTotal = oResults.Count();
                oResults = null;

                return iTotal;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// Returns the number of install visits booked for today.
        /// </summary>
        /// <returns></returns>
        public int TotalsForToday_Installer()
        {

            StringBuilder sbSQL = new StringBuilder();
            int iTotal = 0;
            try
            {

                List<cTotal> Total = null;

                DateTime dNowStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
                DateTime dNowEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

                sbSQL.Append("SELECT Count(*) AS Total FROM cProjectTable");

                sbSQL.Append(" WHERE (MxmConfirmedAppointmentIndicator = '1'");
                sbSQL.Append(" OR ConfirmedActionDateTime IS NOT NULL)");                

                sbSQL.Append(" AND EndDateTime >= '" + dNowStart.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                sbSQL.Append(" AND EndDateTime <= '" + dNowEnd.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                                             
                sbSQL.Append(" ORDER BY EndDateTime");

                Total = this.m_conSQL.Query<cTotal>(sbSQL.ToString());


                foreach (cTotal TTl in Total)
                {
                    if (TTl.Total.HasValue == true)
                    {
                        iTotal = TTl.Total.Value;
                    }
                }

                return iTotal;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SQL(" + sbSQL.ToString() + ")");

            }

        }

        /// <summary>
        /// Total installs pending.
        /// </summary>
        /// <returns></returns>
        public int TotalPending_Installer()
        {

            try
            {
                int iTotal = 0;

                DateTime dNowStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
                DateTime dNowEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);


                var oResults = from oCols in this.m_conSQL.Table<cProjectTable>()
                               where ((oCols.StartDateTime >= dNowStart) && (oCols.EndDateTime <= dNowEnd)
                               && (oCols.MxmConfirmedAppointmentIndicator == 1)
                               && (oCols.Mxm1002InstallStatus != cSettings.p_iInstallStatus_InstalledFully))
                               select oCols;

                iTotal = oResults.Count();
                oResults = null;

                return iTotal;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// Total installs completed.
        /// </summary>
        /// <returns></returns>
        public int TotalCompleted_Installer()
        {

            StringBuilder sbSQL = new StringBuilder();

            try
            {

                int iTotal = 0;

                DateTime dNowStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
                DateTime dNowEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

                List<cTotal> Total = null;

                sbSQL.Append("SELECT Count(*) AS Total FROM cProjectTable");

                sbSQL.Append(" WHERE (Mxm1002InstallStatus IN(" + cSettings.p_iInstallStatus_InstalledFully.ToString() + ")");

                sbSQL.Append(" OR (ConfirmedActionDateTime IS NOT NULL AND MxmConfirmedAppointmentIndicator='0'))");

                sbSQL.Append(" AND EndDateTime >= '" + dNowStart.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                sbSQL.Append(" AND EndDateTime <= '" + dNowEnd.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                
                sbSQL.Append(" ORDER BY EndDateTime");

                Total = this.m_conSQL.Query<cTotal>(sbSQL.ToString());

                foreach (cTotal TTl in Total)
                {
                    if (TTl.Total.HasValue == true)
                    {
                        iTotal = TTl.Total.Value;
                    }
                }

                return iTotal;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SQL(" + sbSQL.ToString() + ")");

            }


        }

  


        /// <summary>
        /// Gets the name for the field and enumerator value passed.
        /// </summary>
        /// <param name="v_sFieldName"></param>
        /// <param name="v_iEnumValue"></param>
        /// <returns></returns>
        public int GetEnumNameValue(string v_sTableName, string v_sFieldName, string v_sEnumName)
        {

            try
            {

                int sEnumName = 0;

                var oResults = from oCols in this.m_conSQL.Table<cBaseEnumsTable>()
                               where (oCols.FieldName.ToLower().Equals(v_sFieldName.ToLower()))
                               && (oCols.TableName.ToLower().Equals(v_sTableName.ToLower()))
                               && (oCols.EnumName.ToLower().Equals(v_sEnumName.ToLower()))
                               select oCols;


                foreach (var oResult in oResults)
                {
                    sEnumName = oResult.EnumValue;

                }
                oResults = null;

                return sEnumName;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - TABLENAME(" + v_sTableName + "),FIELDNAME(" + v_sFieldName + ",ENUMNAME(" + v_sEnumName + ")");

            }

        }

        /// <summary>
        /// Gets the name for the field and enumerator value passed.
        /// </summary>
        /// <param name="v_sFieldName"></param>
        /// <param name="v_iEnumValue"></param>
        /// <returns></returns>
        public string GetEnumValueName(string v_sTableName, string v_sFieldName, int v_iEnumValue)
        {

            try
            {

                string sEnumName = null;

                var oResults = from oCols in this.m_conSQL.Table<cBaseEnumsTable>()
                               where (oCols.FieldName.ToLower().Equals(v_sFieldName.ToLower()))
                               && (oCols.TableName.ToLower().Equals(v_sTableName.ToLower()))
                               && (oCols.EnumValue.Equals(v_iEnumValue))
                               select oCols;


                foreach (var oResult in oResults)
                {
                    sEnumName = oResult.EnumName;

                }
                oResults = null;

                return sEnumName;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - TABLENAME(" + v_sTableName + "),FIELDNAME(" + v_sFieldName + ",ENUMVALUE(" + v_iEnumValue.ToString() + ")");

            }

        }

        /// <summary>
        /// Get enumerator details for field.
        /// </summary>
        /// <param name="v_sFieldName"></param>
        /// <returns></returns>
        public List<cBaseEnumsTable> GetEnumsForField(string v_sFieldName)
        {

            try
            {

                var oResults = from oCols in this.m_conSQL.Table<cBaseEnumsTable>()
                               where (oCols.FieldName.ToLower().Equals(v_sFieldName.ToLower()))
                               orderby oCols.EnumValue
                               select oCols;


                List<cBaseEnumsTable> oReturn = oResults.ToList<cBaseEnumsTable>();
                oResults = null;

                return oReturn;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - FIELDNAME(" + v_sFieldName + ")");

            }

        }

        /// <summary>
        /// Get the list of upcoming work to display on the front screen.
        /// </summary>
        /// <returns></returns>
        public List<cProjectTable> GetUpComingWork_Surveyor(string v_sUserName, int v_iInstallStatus_AwaitingSurvey, DateTime? v_dtDate = null)
        {

            try
            {
                
                List<cProjectTable> lProjects = null;

                StringBuilder sbSQL = new StringBuilder();
                sbSQL.Append("SELECT * FROM cProjectTable");
                sbSQL.Append(" WHERE MXM1002TrfDate ISNULL");
                sbSQL.Append(" AND MxmConfirmedAppointmentIndicator = '1'");
                sbSQL.Append(" AND SurveyorProfile='" + v_sUserName + "'");
                sbSQL.Append(" AND Mxm1002InstallStatus=" + v_iInstallStatus_AwaitingSurvey);
                //sbSQL.Append(" AND Mxm1002ProgressStatus=" + v_iProgressStatus_AbleToProgress); //Remove progress status filter.

                if (v_dtDate.HasValue == true)
                {

                    //Create start and end date objects. v1.0.4 - Set start time to 00:00:00, not 00:00:01
                    DateTime dStartDate = new DateTime(v_dtDate.Value.Year, v_dtDate.Value.Month, v_dtDate.Value.Day, 00, 00, 00);
                    DateTime dEndDate = new DateTime(v_dtDate.Value.Year, v_dtDate.Value.Month, v_dtDate.Value.Day, 23, 59, 59);


                    sbSQL.Append(" AND (");

                    sbSQL.Append(" EndDateTime >= '" + dStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                    sbSQL.Append(" AND EndDateTime <= '" + dEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                    sbSQL.Append(")");


                }

                sbSQL.Append(" ORDER BY EndDateTime");

                lProjects = this.m_conSQL.Query<cProjectTable>(sbSQL.ToString());

                return lProjects;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - USERNAME(" + v_sUserName + ")");

            }

        }

        /// <summary>
        /// Get the list of upcoming work to display on the front screen.
        /// </summary>
        /// <returns></returns>
        public List<cInstallerDashboardResult> GetUpComingWork_Installer()
        {

            try
            {

                List<cInstallerDashboardResult> lProjects = null;

                StringBuilder sbSQL = new StringBuilder();

                sbSQL.Append("SELECT ");

                sbSQL.Append("PT.SubProjectNo");
                sbSQL.Append(",PT.EndDateTime");
                sbSQL.Append(",PT.DeliveryStreet");
                sbSQL.Append(",PT.ResidentName");
                sbSQL.Append(",PT.InstallStatusName");
                sbSQL.Append(",PT.ProgressStatusName");
                sbSQL.Append(",UT.TotalUnits");
                sbSQL.Append(",UTI.TotalUnitsInstalled");

                sbSQL.Append(" FROM cProjectTable AS PT");

                sbSQL.Append(" LEFT JOIN (SELECT COUNT(*) AS TotalUnits,SubProjectNo FROM cUnitsTable GROUP BY SubProjectNo) AS UT");
                sbSQL.Append(" ON PT.SubProjectNo = UT.SubProjectNo");

                sbSQL.Append(" LEFT JOIN (SELECT COUNT(*) AS TotalUnitsInstalled,SubProjectNo FROM cUnitsTable WHERE InstalledStatus = 5 GROUP BY SubProjectNo) AS UTI");
                sbSQL.Append(" ON PT.SubProjectNo = UTI.SubProjectNo");

                sbSQL.Append(" WHERE PT.MxmConfirmedAppointmentIndicator = '1'");
                sbSQL.Append(" AND PT.MXM1002InstallStatus <> " + cSettings.p_iInstallStatus_InstalledFully.ToString());       
                
                sbSQL.Append(" ORDER BY PT.EndDateTime");

                lProjects = this.m_conSQL.Query<cInstallerDashboardResult>(sbSQL.ToString());

                return lProjects;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// v1.0.1 - Fetch all the new notes for a sub project.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public List<cProjectNotesTable> FetchNewNotes(string v_sSubProjectNo)
        {

            try
            {

                var oResults = from oCols in this.m_conSQL.Table<cProjectNotesTable>()
                               where (oCols.SubProjectNo.ToLower().Equals(v_sSubProjectNo.ToLower())
                               && (oCols.AXRecID.Equals(-1) == true))
                               orderby oCols.InputDateTime
                               select oCols;

                List<cProjectNotesTable> oReturn = oResults.ToList<cProjectNotesTable>();
                oResults = null;

                return oReturn;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SubProjectNo(" + v_sSubProjectNo + ")");

            }


        }

        /// <summary>
        /// v1.0.1 - Fetch sub projects with new notes.
        /// </summary>
        /// <returns></returns>
        public List<cSubProjectSync> FetchSubProjectsWithNewNotes()
        {

            StringBuilder sbSQL = new StringBuilder();

            try
            {

                sbSQL.Append("SELECT SubProjectNo,COUNT(*) AS UpdateQty ");

                sbSQL.Append(" FROM cProjectNotesTable");

                sbSQL.Append(" WHERE AXRecID = -1");

                sbSQL.Append(" GROUP BY SubProjectNo");

                return this.m_conSQL.Query<cSubProjectSync>(sbSQL.ToString());


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }


        /// <summary>
        /// v1.0.1 - Update notes records that have just been uploaded with new RECID from AX.
        /// </summary>
        /// <param name="v_nkvValues"></param>
        /// <returns></returns>
        public bool UpdateNotesWithRecID(ObservableCollection<wcfAX.clsRealtimeNoteKeyValues> v_nkvValues)
        {

            StringBuilder sbSQL = new StringBuilder();
            int iSQLRtn = -1;
            try
            {
                     
                if (v_nkvValues != null)
                {

                    foreach (wcfAX.clsRealtimeNoteKeyValues nkvValue in v_nkvValues)
                    {

                        sbSQL.Clear();

                        sbSQL.Append("UPDATE cProjectNotesTable");
                        sbSQL.Append(" SET AXRecID=" + nkvValue.NotesRecID);
                        sbSQL.Append(" WHERE IDKey=" + nkvValue.DeviceIDKey);

                        iSQLRtn = this.m_conSQL.Execute(sbSQL.ToString());


                    }

                }

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        ///// <summary>
        ///// Get notes for the project.
        ///// </summary>
        ///// <param name="v_sSubProjID"></param>
        ///// <returns></returns>
        //public List<cProjectNotesTable> GetProjectNotes(String v_sSubProjID)
        //{

        //    try
        //    {

        //        List<cProjectNotesTable> lProjectNotes = null;

        //        var oResults = (from oCols in this.m_conSQL.Table<cProjectNotesTable>()
        //                        where (oCols.SubProjectNo.Equals(v_sSubProjID))
        //                        orderby oCols.IDKey ascending
        //                        select oCols);

        //        lProjectNotes = oResults.ToList<cProjectNotesTable>();

        //        oResults = null;

        //        return lProjectNotes;

        //    }
        //    catch (Exception ex)
        //    {
        //        cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
        //        return null;

        //    }
        //}


        /// <summary>
        /// Get project form DB
        /// </summary>
        /// <param name="v_sSubProjID"></param>
        /// <returns></returns>
        public cProjectTable GetSubProjectProjectData(String v_sSubProjectNo)
        {

            try
            {

                cProjectTable lProject = null;

                var oResults = (from oCols in this.m_conSQL.Table<cProjectTable>()
                                where (oCols.SubProjectNo.Equals(v_sSubProjectNo))
                                select oCols);

                lProject = oResults.Single<cProjectTable>();

                oResults = null;

                return lProject;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SubProjectNo(" + v_sSubProjectNo + ")");

            }

        }

        /// <summary>
        /// v1.0.1 - Get sub project notes data.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public List<cProjectNotesTable> GetSubProjectNotesData(string v_sSubProjectNo)
        {

            List<cProjectNotesTable> cNotes = null;
            try
            {

                var oResults = (from oCols in this.m_conSQL.Table<cProjectNotesTable>()
                                where (oCols.SubProjectNo.Equals(v_sSubProjectNo))
                                orderby oCols.InputDateTime descending
                                select oCols);

                cNotes = oResults.ToList<cProjectNotesTable>();

                return cNotes;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SubProjectNo(" + v_sSubProjectNo + ")");

            }

        }

        /// <summary>
        /// Get list of project numbers from local DB
        /// </summary>
        /// <returns></returns>
        public List<Classes.cProjectNo> GetProjectNos()
        {

            List<cProjectNo> sProjNos = new List<cProjectNo>();

            try
            {

                String sSQL = "SELECT ProjectNo,ProjectName FROM cProjectTable GROUP BY ProjectNo,ProjectName ORDER BY ProjectNo,ProjectName";
                sProjNos = this.m_conSQL.Query<cProjectNo>(sSQL);

                return sProjNos;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }


        }


        /// <summary>
        /// Get list of surveyors from local DB
        /// </summary>
        /// <returns></returns>
        public List<cSurveyor> GetSurveyors()
        {

            List<cSurveyor> sSurveyors = new List<cSurveyor>();
            StringBuilder sbSQL = new StringBuilder();
            try
            {
                
                sbSQL.Append("SELECT SurveyorProfile,SurveyorName FROM cProjectTable");
                sbSQL.Append(" WHERE SurveyorProfile <> ''");
                sbSQL.Append(" GROUP BY SurveyorProfile,SurveyorName");
                sbSQL.Append(" ORDER BY SurveyorProfile,SurveyorName");

                sSurveyors = this.m_conSQL.Query<cSurveyor>(sbSQL.ToString());

                return sSurveyors;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SQL(" + sbSQL.ToString() + ")");

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <param name="v_sStreet"></param>
        /// <param name="v_sPostcode"></param>
        /// <param name="v_iInstallStatus"></param>
        /// <param name="v_iProgressStatus"></param>
        /// <param name="v_dSurveyDate"></param>
        /// <param name="v_sDateCompare"></param>
        /// <param name="v_sSurveyedStatus"></param>
        /// <param name="v_sSurveyor"></param>
        /// <param name="v_sSurveyedOnSite"></param>
        /// <param name="v_bSyncOnly"></param>
        /// <param name="v_bShowAllStatuses"></param>
        /// <param name="v_bShowAllProgressStatuses"></param>
        /// <param name="v_sSubProjectNoFilter"></param>
        /// <param name="v_iInstallStatus_AwaitingSurvey"></param>
        /// <param name="v_iInstallStatus_Cancel"></param>
        /// <param name="v_bBooked"></param>
        /// <param name="v_sInstallStatus_Filter"></param>
        /// <param name="v_sOrderComplete_Filter"></param>
        /// <param name="v_bHSIncomplete">v1.0.19</param>
        /// <returns></returns>
        public List<cSurveyInputResult> SearchSurveyInput(string v_sProjectNo, string v_sStreet, string v_sPostcode, int v_iInstallStatus, int v_iProgressStatus, DateTime? v_dSurveyDate, string v_sDateCompare, string v_sSurveyedStatus, string v_sSurveyor, string v_sSurveyedOnSite, bool v_bSyncOnly, bool v_bShowAllStatuses, bool v_bShowAllProgressStatuses, string v_sSubProjectNoFilter, int v_iInstallStatus_AwaitingSurvey, int v_iInstallStatus_Cancel, bool v_bBooked = false, string v_sInstallStatus_Filter = cSettings.p_sInstallStatusFilter_EqualTo, string v_sOrderComplete_Filter = cSettings.p_sAnyStatus, HSFilters v_iHSIncomplete = HSFilters.Any)
        {

            try
            {

                //Creating our WHERE clause.
                StringBuilder sbWhere = new StringBuilder();


                //Add project number if specified.
                if (v_sProjectNo != null)
                {
                    if (v_sProjectNo.Length > 0)
                    {
                        sbWhere.Append("PT.ProjectNo='" + v_sProjectNo.Replace("'", "''") + "'");

                    }
                }
                else if (v_sSubProjectNoFilter != null) //v1.0.1
                {

                    if (v_sSubProjectNoFilter.Length > 0)
                    {
                        sbWhere.Append(" LOWER(PT.SubProjectNo) " + this.FormatSQLCritieria(v_sSubProjectNoFilter.ToLower()));
                    }

                }

                //Add street if specified.
                if (v_sStreet.Length > 0)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" LOWER(PT.DeliveryStreet) " + this.FormatSQLCritieria(v_sStreet.ToLower()));

                }

                //Add postcode if specified.
                if (v_sPostcode.Length > 0)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" LOWER(PT.DlvZipCode) " + this.FormatSQLCritieria(v_sPostcode.ToLower()));

                }

                //v1.0.12 - Progress status.
                if (v_iProgressStatus > -1)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" PT.Mxm1002ProgressStatus = " + v_iProgressStatus.ToString());

                }

                //Add install status if specified.
                if (v_iInstallStatus > -1)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    if (v_iInstallStatus != cSettings.p_iInstallStatus_Installing)
                    {

                        if (v_sInstallStatus_Filter == cSettings.p_sInstallStatusFilter_EqualTo)
                        {
                            sbWhere.Append(" PT.Mxm1002InstallStatus = " + v_iInstallStatus.ToString());
                        }
                        else
                        {
                            sbWhere.Append(" PT.Mxm1002InstallStatus <> " + v_iInstallStatus.ToString());
                        }
                        

                    }
                    else
                    {

                        string[] sStatuses = this.GetSettingValue("Installation_Installing_InstallStatuses").Split(',');
                        if (sStatuses != null)
                        {

                            if (v_sInstallStatus_Filter == cSettings.p_sInstallStatusFilter_EqualTo)
                            {
                                sbWhere.Append(" PT.Mxm1002InstallStatus IN (");
                            }
                            else
                            {
                                sbWhere.Append(" PT.Mxm1002InstallStatus NOT IN (");
                            }
                            foreach (string sStatus in sStatuses)
                            {
                                sbWhere.Append(sStatus + ",");

                            }
                            sbWhere.Remove(sbWhere.Length - 1, 1); //Remove the last comma
                            sbWhere.Append(")");

                        }

                    }

                }

                //Add survey date if specified.
                if (v_dSurveyDate.HasValue == true)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    //Create start and end date objects. v1.0.4 - Set start time to 00:00:00, not 00:00:01
                    DateTime dStartDate = new DateTime(v_dSurveyDate.Value.Year, v_dSurveyDate.Value.Month, v_dSurveyDate.Value.Day, 00, 00, 00);
                    DateTime dEndDate = new DateTime(v_dSurveyDate.Value.Year, v_dSurveyDate.Value.Month, v_dSurveyDate.Value.Day, 23, 59, 59);


                    sbWhere.Append(" (");

                    //Add SQL comparison criteria.
                    if (v_sDateCompare == cSettings.p_sDateCompare_GreaterThan || v_sDateCompare == cSettings.p_sDateCompare_EqualTo)
                    {
                        //v1.0.4 - Only use EndDateTime, sbWhere.Append(" PT.StartDateTime >= '" + dStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        sbWhere.Append(" PT.EndDateTime >= '" + dStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                    }
                    if (v_sDateCompare == cSettings.p_sDateCompare_LessThan || v_sDateCompare == cSettings.p_sDateCompare_EqualTo)
                    {

                        if (v_sDateCompare == cSettings.p_sDateCompare_EqualTo)
                        {
                            sbWhere.Append(" AND ");
                        }

                        //v1.0.4 - Only use EndDateTime, sbWhere.Append(" PT.StartDateTime <= '" + dEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        sbWhere.Append(" PT.EndDateTime <= '" + dEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                    }


                    //sbWhere.Append(" AND PT.MXM1002TrfDate ISNULL");

                    sbWhere.Append(" )");
                }


                //Surveyed Status
                if (v_sSurveyedStatus.Equals(cSettings.p_sSurveyedStatus_NotSurveyed) == true)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" PT.MXM1002TrfDate ISNULL");

                    sbWhere.Append(" AND (PT.Mxm1002InstallStatus = " + v_iInstallStatus_AwaitingSurvey + " OR PT.Mxm1002InstallStatus = " + v_iInstallStatus_Cancel + ")");

                }
                else if (v_sSurveyedStatus.Equals(cSettings.p_sSurveyedStatus_SurveyedOnSite) == true)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" PT.MXM1002TrfDate IS NOT NULL");

                    sbWhere.Append(" AND (PT.Mxm1002InstallStatus = " + v_iInstallStatus_AwaitingSurvey + " OR PT.Mxm1002InstallStatus = " + v_iInstallStatus_Cancel + ")");


                }
                else if (v_sSurveyedStatus.Equals(cSettings.p_sSurveyedStatus_SurveyedTrans) == true)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    //sbWhere.Append(" PT.MXM1002TrfDate IS NOT NULL");

                    sbWhere.Append(" (PT.Mxm1002InstallStatus <> " + v_iInstallStatus_AwaitingSurvey + " AND PT.Mxm1002InstallStatus <> " + v_iInstallStatus_Cancel + ")");


                }


                //Add surveyor status.
                if (v_sSurveyor.Length > 0)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" PT.SurveyorProfile = '" + v_sSurveyor + "'");

                }

                //Add survey input status
                if (v_sSurveyedOnSite.Equals(cSettings.p_sInputStatus_Successful) == true)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" PT.MXM1002TrfDate IS NOT NULL");

                }
                else if (v_sSurveyedOnSite.Equals(cSettings.p_sInputStatus_Pending) == true)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" PT.MXM1002TrfDate ISNULL AND PT.MxmConfirmedAppointmentIndicator='1'");


                }
                else if (v_sSurveyedOnSite.Equals(cSettings.p_sInputStatus_Failed) == true)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" (PT.MxmConfirmedAppointmentIndicator ISNULL OR  PT.MxmConfirmedAppointmentIndicator='0')");
                    sbWhere.Append(" AND (PT.StartDateTime IS NOT NULL AND PT.EndDateTime IS NOT NULL)");

                }
                else if (v_sSurveyedOnSite.Equals(cSettings.p_sInputStatus_NotPending) == true) //v1.0.1
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" (PT.MXM1002TrfDate IS NOT NULL OR  ");
                    sbWhere.Append(" ((PT.MxmConfirmedAppointmentIndicator ISNULL OR PT.MxmConfirmedAppointmentIndicator='0')");
                    sbWhere.Append(" AND (PT.StartDateTime IS NOT NULL AND PT.EndDateTime IS NOT NULL)))");

                }

                //Sync Only changes
                if (v_bSyncOnly == true)
                {

                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" UPD.UpdateQty > 0");

                }


                //v1.0.1
                string sSettingValue = string.Empty;

                //v1.0.1 - Show all statuses.
                if (v_bShowAllStatuses == false)
                {

                    sSettingValue = this.GetSettingValue("Search_ExcludeProjectStatus");
                    if (sSettingValue.Length > 0)
                    {

                        if (sbWhere.Length > 0)
                        {
                            sbWhere.Append(" AND ");
                        }

                        sbWhere.Append(" PT.Status NOT IN (" + sSettingValue + ")");

                    }

                }

                //v1.0.1 - Show all progress statuses.
                if (v_bShowAllProgressStatuses == false)
                {

                    sSettingValue = this.GetSettingValue("Search_ExcludeProgressStatuses");
                    if (sSettingValue.Length > 0)
                    {

                        if (sbWhere.Length > 0)
                        {
                            sbWhere.Append(" AND ");
                        }

                        sbWhere.Append(" PT.Mxm1002ProgressStatus NOT IN (" + sSettingValue + ")");

                    }

                }

                //v1.0.10 - Booked flag, makes sure the sub project has actually been booked.
                if (v_bBooked == true)
                {

                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" (PT.MxmConfirmedAppointmentIndicator='1' OR PT.ConfirmedActionDateTime IS NOT NULL)");
                    //sbWhere.Append(" (PT.ConfirmedActionDateTime IS NOT NULL)");

                }


                //v1.0.11 - Order complete date flag.
                if (v_sOrderComplete_Filter != cSettings.p_sAnyStatus)
                {

                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    if (v_sOrderComplete_Filter == cSettings.p_sConfirmedStatus_No)
                    {

                        sbWhere.Append(" (ABPAWOrderCompletedDate = '" +
                                       cSettings.p_dDefaultDBDate.ToString("yyyy-MM-dd HH:mm:ss") +
                                       "' OR ABPAWOrderCompletedDate IS NULL) ");

                    }
                    else if (v_sOrderComplete_Filter == cSettings.p_sConfirmedStatus_Yes )
                    {

                        sbWhere.Append(" (ABPAWOrderCompletedDate > '" + cSettings.p_dDefaultDBDate.ToString("yyyy-MM-dd HH:mm:ss") + "')");
                        
                    }

                }

                //v1.0.21 - Add health and safety filter is required.
                if (v_iHSIncomplete == HSFilters.InComplete)
                {
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" (PT.ABPAWHealthSafetyInComplete='1')");

                }
                else if (v_iHSIncomplete == HSFilters.Complete)
                {
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    //v1.0.23 - Check for null as well as 0
                    sbWhere.Append(" (PT.ABPAWHealthSafetyInComplete = '0' OR PT.ABPAWHealthSafetyInComplete IS NULL)");

                }

                StringBuilder sbSQL = new StringBuilder();

                bool bSurveyorApp = cSettings.IsThisTheSurveyorApp();

                sbSQL.Append("SELECT PT.IDKey");
                sbSQL.Append(", PT.ProjectNo");
                sbSQL.Append(", PT.SubProjectNo");
                sbSQL.Append(", PT.DeliveryStreet");
                sbSQL.Append(", PT.DlvZipCode");
                sbSQL.Append(", PT.MXM1002TrfDate");
                sbSQL.Append(", PT.InstallStatusName");
                sbSQL.Append(", PT.EndDateTime");
                sbSQL.Append(", PT.StartDateTime");
                sbSQL.Append(", PT.SurveyorProfile");
                sbSQL.Append(", PT.SurveyorName");
                sbSQL.Append(", PT.ConfirmedActionDateTime"); //v1.0.10.
                sbSQL.Append(", UPD.UpdateQty");
                sbSQL.Append(", PT.Mxm1002InstallStatus");
                sbSQL.Append(", PT.MxmConfirmedAppointmentIndicator");
                sbSQL.Append(", PT.Status"); //v1.0.13 - Return project status.
                sbSQL.Append(", PT.StatusName"); //v1.0.1
                sbSQL.Append(", PT.ProgressStatusName"); //v1.0.1
                sbSQL.Append(", PT.Mxm1002ProgressStatus"); //v1.0.12
                sbSQL.Append(", NTS.NotesQty"); //v1.0.1 - Return notes quantity
                sbSQL.Append(", PT.ProjectName"); //v1.0.19 - Return project name for tool tip
                sbSQL.Append(", PT.Delivery_EndDateTime"); //v1.0.19 - Return Delivery date for tool tip
                sbSQL.Append(", PT.ABPAWOrderCompletedDate"); //v1.0.19 - Order complete date
                

                if (bSurveyorApp == false)
                {
                    sbSQL.Append(", UT.TotalUnits");
                    sbSQL.Append(" ,UTI.TotalUnitsInstalled");

                }

                sbSQL.Append(" FROM cProjectTable AS PT");

                sbSQL.Append(" LEFT JOIN (SELECT COUNT(*) AS UpdateQty,SubProjectNo FROM cUpdatesTable GROUP BY SubProjectNo) AS UPD");
                sbSQL.Append(" ON PT.SubProjectNo = UPD.SubProjectNo");

                //v1.0.1 - Include notes count
                sbSQL.Append(" LEFT JOIN (SELECT COUNT(*) AS NotesQty,SubProjectNo FROM cProjectNotesTable GROUP BY SubProjectNo) AS NTS");
                sbSQL.Append(" ON PT.SubProjectNo = NTS.SubProjectNo ");

                //When in installers app mode we need to get the unit info back.
                if (bSurveyorApp == false)
                {

                    sbSQL.Append(" LEFT JOIN (SELECT COUNT(*) AS TotalUnits,SubProjectNo FROM cUnitsTable GROUP BY SubProjectNo) AS UT");
                    sbSQL.Append(" ON PT.SubProjectNo = UT.SubProjectNo");

                    sbSQL.Append(" LEFT JOIN (SELECT COUNT(*) AS TotalUnitsInstalled,SubProjectNo FROM cUnitsTable WHERE InstalledStatus = " + cSettings.p_iUnits_InstalledStatus.ToString() + " GROUP BY SubProjectNo) AS UTI");
                    sbSQL.Append(" ON PT.SubProjectNo = UTI.SubProjectNo");

                }

                //Only add where clause if some criteria has been set.
                if (sbWhere.Length > 0)
                {
                    sbSQL.Append(" WHERE " + sbWhere.ToString());
                }

                //Add the Order By.
                sbSQL.Append(" ORDER BY PT.EndDateTime ASC, PT.MXM1002SequenceNr ASC, PT.ProjectNo ASC");

                List<cSurveyInputResult> cResults = this.m_conSQL.Query<cSurveyInputResult>(sbSQL.ToString());

                return cResults;

            }
            catch (Exception ex)
            {

                StringBuilder sbParams = new StringBuilder();
                sbParams.Append("ProjectNo=" + v_sProjectNo);
                sbParams.Append(",Street=" + v_sStreet);
                sbParams.Append(",Postcode=" + v_sPostcode);
                sbParams.Append(",InstallStatus=" + v_iInstallStatus.ToString());
                sbParams.Append(",SurveyDate=" + v_dSurveyDate.ToString());
                sbParams.Append(",DateCompare=" + v_sDateCompare);
                sbParams.Append(",SurveyedStatus=" + v_sSurveyedStatus);
                sbParams.Append(",Surveyor=" + v_sSurveyor);
                sbParams.Append(",SurveyedOnSite=" + v_sSurveyedOnSite);
                sbParams.Append(",SyncOnly=" + v_bSyncOnly.ToString());
                sbParams.Append(",ShowAllStatuses=" + v_bShowAllStatuses.ToString());
                sbParams.Append(",ShowAllProgressStatuses=" + v_bShowAllProgressStatuses.ToString());
                sbParams.Append(",SubProjectNoFilter=" + v_sSubProjectNoFilter);
                sbParams.Append(",InstallStatusAwaitingSurvey=" + v_iInstallStatus_AwaitingSurvey.ToString());
                sbParams.Append(",InstallStatusCancel=" + v_iInstallStatus_Cancel);
                sbParams.Append(",v_bBooked=" + v_bBooked);
                sbParams.Append(",v_sInstallStatus_Filter=" + v_sInstallStatus_Filter);
                sbParams.Append(",v_sOrderComplete_Filter=" + v_sOrderComplete_Filter);
                sbParams.Append(",v_iHSIncomplete=" + v_iHSIncomplete.ToString());                

                throw new Exception(ex.Message + " - PARAMS(" + sbParams.ToString() + ")");

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sValue"></param>
        /// <returns></returns>
        private string FormatSQLCritieria(string v_sValue)
        {

            try
            {

               
                //if (v_sValue.EndsWith("*") == true || v_sValue.StartsWith("*") == true)
                //{

                //    string sStart = String.Empty;
                //    if (v_sValue.StartsWith("*") == true) { sStart = "%"; v_sValue = v_sValue.Substring(1); };

                //    string sEnd = String.Empty;
                //    if (v_sValue.EndsWith("*") == true) { sEnd = "%"; v_sValue = v_sValue.Substring(0, v_sValue.Length - 1); };

                //v1.0.19 - All searches are wild card by default now.
                return " LIKE '%" + v_sValue.Replace("'", "''") + "%'";

                //}
                //else
                //{

                //    return " = '" + v_sValue.Replace("'", "''") + "'";

                //}

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - VALUE(" + v_sValue + ")");

            }

        }

        /// <summary>
        /// Searching for projects on the set survey dates screen.
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <param name="v_sStreet"></param>
        /// <param name="v_iInstallStatus"></param>
        /// <param name="v_dSurveyDate"></param>
        /// <param name="v_sSurveyed"></param>
        /// <param name="v_sConfirmed"></param>
        /// <returns></returns>
        public List<cSurveyDatesResult> SearchSurveyDates(string v_sProjectNo, string v_sStreet, string v_sPostcode, int v_iInstallStatus, int v_iProgressStatus, DateTime? v_dSurveyDate, string v_sDateCompare, string v_sSurveyedStatus, int v_iConfirmed, bool v_bSyncOnly, bool v_bShowAllStatuses, bool v_bShowAllProgressStatuses, string v_sSubProjectNoFilter, int v_iInstallStatus_AwaitingSurvey, int v_iInstallStatus_Cancel, string v_sInstallStatus_Filter = cSettings.p_sInstallStatusFilter_EqualTo)
        {


            try
            {

                //Creating our WHERE clause.
                StringBuilder sbWhere = new StringBuilder();


                //Add project number if specified.
                if (v_sProjectNo != null)
                {
                    if (v_sProjectNo.Length > 0)
                    {
                        sbWhere.Append("PT.ProjectNo='" + v_sProjectNo.Replace("'", "''") + "'");

                    }
                }
                else if (v_sSubProjectNoFilter != null) //v1.0.1
                {

                    if (v_sSubProjectNoFilter.Length > 0)
                    {
                        sbWhere.Append(" LOWER(PT.SubProjectNo) " + this.FormatSQLCritieria(v_sSubProjectNoFilter.ToLower()));
                    }

                }


                //Add street if specified.
                if (v_sStreet.Length > 0)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" LOWER(PT.DeliveryStreet) " + this.FormatSQLCritieria(v_sStreet.ToLower()));


                }

                //Add post code if specified
                if (v_sPostcode.Length > 0)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" LOWER(PT.DlvZipCode) " + this.FormatSQLCritieria(v_sPostcode.ToLower()));

                }

                //v1.0.12 - Include progress status if specified
                if (v_iProgressStatus > -1)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" PT.Mxm1002ProgressStatus = " + v_iProgressStatus.ToString());


                }

                //Add install status if specified.
                if (v_iInstallStatus > -1)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    if (v_iInstallStatus != cSettings.p_iInstallStatus_Installing)
                    {

                        if (v_sInstallStatus_Filter == cSettings.p_sInstallStatusFilter_EqualTo)
                        {
                            sbWhere.Append(" PT.Mxm1002InstallStatus = " + v_iInstallStatus.ToString());

                        }
                        else if (v_sInstallStatus_Filter == cSettings.p_sInstallStatusFilter_NotEqualTo)
                        {
                            sbWhere.Append(" PT.Mxm1002InstallStatus <> " + v_iInstallStatus.ToString());
                        }
                        

                    }
                    else
                    {

                        //v1.0.10 - For installing status, is made up of other statuses.
                        string[] sStatuses = this.GetSettingValue("Installation_Installing_InstallStatuses").Split(',');
                        if (sStatuses != null)
                        {

                            //Check the install status filter to decide
                            if (v_sInstallStatus_Filter == cSettings.p_sInstallStatusFilter_EqualTo)
                            {
                                sbWhere.Append(" PT.Mxm1002InstallStatus IN (");
                            }
                            else if (v_sInstallStatus_Filter == cSettings.p_sInstallStatusFilter_NotEqualTo)
                            {
                                sbWhere.Append(" PT.Mxm1002InstallStatus NOT IN (");
                            }

                            foreach (string sStatus in sStatuses)
                            {
                                sbWhere.Append(sStatus + ",");

                            }
                            sbWhere.Remove(sbWhere.Length - 1, 1); //Remove the last comma
                            sbWhere.Append(")");

                        }

                    }

                }

                //Add survey date if specified.
                if (v_dSurveyDate.HasValue == true)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    //Create start and end date objects.
                    //v1.0.4 - Set start date time to 00:00:00 not 00:00:01, 
                    DateTime dStartDate = new DateTime(v_dSurveyDate.Value.Year, v_dSurveyDate.Value.Month, v_dSurveyDate.Value.Day, 00, 00, 00);
                    DateTime dEndDate = new DateTime(v_dSurveyDate.Value.Year, v_dSurveyDate.Value.Month, v_dSurveyDate.Value.Day, 23, 59, 59);


                    sbWhere.Append(" (");

                    //Add SQL comparison criteria.
                    if (v_sDateCompare == cSettings.p_sDateCompare_GreaterThan || v_sDateCompare == cSettings.p_sDateCompare_EqualTo)
                    {
                        //v1.0.4 - Only use EndDateTime, sbWhere.Append(" PT.StartDateTime >= '" + dStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        sbWhere.Append(" PT.EndDateTime >= '" + dStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                    }
                    if (v_sDateCompare == cSettings.p_sDateCompare_LessThan || v_sDateCompare == cSettings.p_sDateCompare_EqualTo)
                    {

                        if (v_sDateCompare == cSettings.p_sDateCompare_EqualTo)
                        {
                            sbWhere.Append(" AND ");
                        }

                        //v1.0.4 - Only use EndDateTime, sbWhere.Append(" PT.StartDateTime <= '" + dEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        sbWhere.Append(" PT.EndDateTime <= '" + dEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                    }

                    //v1.0.16 - Only check if running surveyor app mode.
                    if (cSettings.IsThisTheSurveyorApp() == true)
                    {
                        sbWhere.Append(" AND PT.MXM1002TrfDate ISNULL");
                    }
                   

                    sbWhere.Append(" )");
                }


                //Add surveyed status.
                if (v_sSurveyedStatus.Equals(cSettings.p_sSurveyedStatus_NotSurveyed) == true)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" PT.MXM1002TrfDate ISNULL");


                    sbWhere.Append(" AND (PT.Mxm1002InstallStatus = " + v_iInstallStatus_AwaitingSurvey + " OR PT.Mxm1002InstallStatus = " + v_iInstallStatus_Cancel + ")");

                }
                else if (v_sSurveyedStatus.Equals(cSettings.p_sSurveyedStatus_SurveyedOnSite) == true)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" PT.MXM1002TrfDate IS NOT NULL");

                    sbWhere.Append(" AND (PT.Mxm1002InstallStatus = " + v_iInstallStatus_AwaitingSurvey + " OR PT.Mxm1002InstallStatus = " + v_iInstallStatus_Cancel + ")");


                }
                else if (v_sSurveyedStatus.Equals(cSettings.p_sSurveyedStatus_SurveyedTrans) == true)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    //sbWhere.Append(" PT.MXM1002TrfDate IS NOT NULL");
                   
                    sbWhere.Append(" (PT.Mxm1002InstallStatus <> " + v_iInstallStatus_AwaitingSurvey + " AND PT.Mxm1002InstallStatus <> " + v_iInstallStatus_Cancel + ")");


                }

                //Add confirmed status.               
                if (v_iConfirmed == (int)cSettings.YesNoBaseEnum.Yes || v_iConfirmed == (int)cSettings.YesNoBaseEnum.No)
                {

                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" PT.MxmConfirmedAppointmentIndicator = " + v_iConfirmed.ToString());

                }


                //Sync Only changes
                if (v_bSyncOnly == true)
                {

                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" UPD.UpdateQty > 0");

                }


                //v1.0.1
                string sSettingValue = string.Empty;

                //v1.0.1 - Show all statuses.
                if (v_bShowAllStatuses == false)
                {

                    sSettingValue = this.GetSettingValue("Search_ExcludeProjectStatus");
                    if (sSettingValue.Length > 0)
                    {

                        if (sbWhere.Length > 0)
                        {
                            sbWhere.Append(" AND ");
                        }

                        sbWhere.Append(" PT.Status NOT IN (" + sSettingValue + ")");

                    }

                }

                //v1.0.1 - Show all progress statuses.
                if (v_bShowAllProgressStatuses == false)
                {

                    sSettingValue = this.GetSettingValue("Search_ExcludeProgressStatuses");
                    if (sSettingValue.Length > 0)
                    {

                        if (sbWhere.Length > 0)
                        {
                            sbWhere.Append(" AND ");
                        }

                        sbWhere.Append(" PT.Mxm1002ProgressStatus NOT IN (" + sSettingValue + ")");

                    }

                }



                StringBuilder sbSQL = new StringBuilder();

                bool bSurveyorApp = cSettings.IsThisTheSurveyorApp();


                sbSQL.Append("SELECT PT.IDKey");
                sbSQL.Append(", PT.ProjectNo");
                sbSQL.Append(", PT.SubProjectNo");
                sbSQL.Append(", PT.DeliveryStreet");
                sbSQL.Append(", PT.DlvZipCode");
                sbSQL.Append(", PT.MXM1002TrfDate");
                sbSQL.Append(", PT.InstallStatusName");
                sbSQL.Append(", PT.EndDateTime");
                sbSQL.Append(", PT.SurveyorProfile");
                sbSQL.Append(", PT.SurveyorName");
                sbSQL.Append(", PT.MxmConfirmedAppointmentIndicator");
                sbSQL.Append(", UPD.UpdateQty");
                sbSQL.Append(", PT.Mxm1002InstallStatus");
                sbSQL.Append(", PT.StatusName"); //v1.0.1
                sbSQL.Append(", PT.Status"); //v1.0.13 - Return project status.
                sbSQL.Append(", PT.ProgressStatusName"); //v1.0.1
                sbSQL.Append(", NTS.NotesQty"); //v1.0.1 - Notes quantity.
                sbSQL.Append(", PT.Mxm1002ProgressStatus"); //v1.0.2 - Progress status
                sbSQL.Append(", PT.Delivery_EndDateTime"); //
                sbSQL.Append(", PT.ProjectName"); //v1.0.19 - Return project name for tool tip   
                sbSQL.Append(", PT.MxmProjDescription"); //v1.0.21 - Work Type             

                if (bSurveyorApp == false)
                {
                    sbSQL.Append(", UT.TotalUnits");
                    sbSQL.Append(" ,UTI.TotalUnitsInstalled");

                }

                sbSQL.Append(" FROM cProjectTable AS PT");

                sbSQL.Append(" LEFT JOIN (SELECT COUNT(*) AS UpdateQty,SubProjectNo FROM cUpdatesTable GROUP BY SubProjectNo) AS UPD");
                sbSQL.Append(" ON PT.SubProjectNo = UPD.SubProjectNo");

                //v1.0.1 - Include notes count
                sbSQL.Append(" LEFT JOIN (SELECT COUNT(*) AS NotesQty,SubProjectNo FROM cProjectNotesTable GROUP BY SubProjectNo) AS NTS");
                sbSQL.Append(" ON PT.SubProjectNo = NTS.SubProjectNo ");

                //When in installers app mode we need to get the unit info back.
                if (bSurveyorApp == false)
                {

                    sbSQL.Append(" LEFT JOIN (SELECT COUNT(*) AS TotalUnits,SubProjectNo FROM cUnitsTable GROUP BY SubProjectNo) AS UT");
                    sbSQL.Append(" ON PT.SubProjectNo = UT.SubProjectNo");

                    sbSQL.Append(" LEFT JOIN (SELECT COUNT(*) AS TotalUnitsInstalled,SubProjectNo FROM cUnitsTable WHERE InstalledStatus = " + cSettings.p_iUnits_InstalledStatus.ToString() + " GROUP BY SubProjectNo) AS UTI");
                    sbSQL.Append(" ON PT.SubProjectNo = UTI.SubProjectNo");

                }

                //Only add where clause if some criteria has been set.
                if (sbWhere.Length > 0)
                {
                    sbSQL.Append(" WHERE " + sbWhere.ToString());
                }

                //Add the Order By.
                sbSQL.Append(" ORDER BY PT.EndDateTime ASC, PT.MXM1002SequenceNr ASC, PT.ProjectNo ASC");

                List<cSurveyDatesResult> cResults = this.m_conSQL.Query<cSurveyDatesResult>(sbSQL.ToString());

                return cResults;

            }
            catch (Exception ex)
            {
               
                StringBuilder sbParams = new StringBuilder();
                sbParams.Append("ProjectNo=" + v_sProjectNo);
                sbParams.Append(",Street=" + v_sStreet);
                sbParams.Append(",Postcode=" + v_sPostcode);
                sbParams.Append(",InstallStatus=" + v_iInstallStatus.ToString());
                sbParams.Append(",SurveyDate=" + v_dSurveyDate.ToString());
                sbParams.Append(",DateCompare=" + v_sDateCompare);
                sbParams.Append(",SurveyedStatus=" + v_sSurveyedStatus);
                sbParams.Append(",Confirmed=" + v_iConfirmed.ToString());
                sbParams.Append(",SyncOnly=" + v_bSyncOnly.ToString());
                sbParams.Append(",ShowAllStatuses=" + v_bShowAllStatuses.ToString());
                sbParams.Append(",ShowAllProgressStatuses=" + v_bShowAllProgressStatuses.ToString());
                sbParams.Append(",SubProjectNoFilter=" + v_sSubProjectNoFilter);
                sbParams.Append(",InstallStatusAwaitingSurvey=" + v_iInstallStatus_AwaitingSurvey.ToString());
                sbParams.Append(",InstallStatusCancel=" + v_iInstallStatus_Cancel);

                throw new Exception(ex.Message + " - PARAMS(" + sbParams.ToString() + ")");

            }


        }

        /// <summary>
        /// Log survey date update to the log table for syncing.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <param name="v_dSurveyDate"></param>
        /// <returns></returns>
        public bool LogSurveyDateUpdate(String v_sSubProjectNo, DateTime v_dSurveyDate)
        {

            try
            {

                //Log the fields that are to be updated so they can be synced.
                bool bUpdateLogged = this.AddToUpdateTable(v_sSubProjectNo, "MxmConfirmedAppointmentIndicator", "1");
                if (bUpdateLogged == false)
                {
                    return false;
                }

                bUpdateLogged = this.AddToUpdateTable(v_sSubProjectNo, "StartDateTime", v_dSurveyDate.ToString("yyyy-MM-dd HH:mm:ss"));
                if (bUpdateLogged == false)
                {
                    return false;
                }

                bUpdateLogged = this.AddToUpdateTable(v_sSubProjectNo, "EndDateTime", v_dSurveyDate.ToString("yyyy-MM-dd HH:mm:ss"));
                if (bUpdateLogged == false)
                {
                    return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SubProjectNo(" + v_sSubProjectNo + ") SurveyDate(" + v_dSurveyDate.ToString() + ")");

            }
        }


        /// <summary>
        /// Apply survey dates to projects.
        /// </summary>
        /// <param name="v_sProjectNos"></param>
        /// <param name="v_dSurveyDate"></param>
        /// <returns></returns>
        public async Task<bool> ApplySurveyDatesToSubProjects(List<string> v_lsSubProjectNos, DateTime v_dSurveyDate)
        {

            try
            {

                //Project table variable for updating.
                cProjectTable cProj = null;
                List<cUpdatesTable> cUpdates = new List<cUpdatesTable>();
                cSettings.SurveyDatesApply cApply;


                string sSurveyorProfile = await cSettings.GetUserName();
                string sSurveyorName = await cSettings.GetUserDisplayName();
                string sMachineName = cSettings.GetMachineName();

                //Loop through each project nu
                foreach (String sSubProjNo in v_lsSubProjectNos)
                {

                    //Clear out any existing updates.
                    cUpdates.Clear();

                    //Fetch project class
                    cProj = this.GetSubProjectProjectData(sSubProjNo);

                    cApply = cSettings.ApplySurveyDates(cProj, cUpdates, v_dSurveyDate, sSubProjNo, sSurveyorName, sSurveyorProfile, sMachineName);

                    //Update database
                    this.m_conSQL.Update(cApply.cProjectData);

                    //Apply all updates to table.
                    this.AddUpdatesToUpdateTable(cApply.cUpdates);

                }


                //Clean up.
                cProj = null;

                //If we get here then all OK.
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SurveyDate=" + v_dSurveyDate.ToString());

            }

        }

        /// <summary>
        /// Add updates to update table.
        /// </summary>
        /// <param name="v_lUpdates"></param>
        /// <returns></returns>
        public bool AddUpdatesToUpdateTable(List<cUpdatesTable> v_lUpdates)
        {

            try
            {

                bool bOK = false;
                foreach (cUpdatesTable cUpdate in v_lUpdates)
                {
                    bOK = this.AddToUpdateTable(cUpdate.SubProjectNo, cUpdate.FieldName, cUpdate.FieldValue);

                    if (bOK == false)
                    {
                        return false;
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        /// <summary>
        /// Add update to update table.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <param name="v_sField"></param>
        /// <param name="v_sValue"></param>
        /// <returns></returns>
        public bool AddToUpdateTable(String v_sSubProjectNo, String v_sField, String v_sValue)
        {

            try
            {


                cUpdatesTable cUpdate = null; // new cUpdatesTable();
                int iCount = 0;


                var oResults = (from oCols in this.m_conSQL.Table<cUpdatesTable>()
                                where (oCols.SubProjectNo.Equals(v_sSubProjectNo) && oCols.FieldName.Equals(v_sField))
                                select oCols);

                if (oResults.Count() > 0)
                {
                    cUpdate = oResults.Single<cUpdatesTable>();
                }


                if (cUpdate == null)
                {
                    cUpdate = new cUpdatesTable();
                    cUpdate.SubProjectNo = v_sSubProjectNo;
                    cUpdate.FieldName = v_sField;
                    cUpdate.FieldValue = v_sValue;

                    iCount = this.m_conSQL.Insert(cUpdate);
                }
                else
                {

                    cUpdate.FieldValue = v_sValue;

                    iCount = this.m_conSQL.Update(cUpdate);
                }

                if (iCount > 0)
                {
                    return true;
                }

                return false;

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message + " - PARAMS(SubProjectNo=" + v_sSubProjectNo + ",Field=" + v_sField + ",Value=" + v_sValue + ")");

            }

        }

        /// <summary>
        /// Update project table.
        /// </summary>
        /// <param name="v_cProject"></param>
        /// <returns></returns>
        public bool UpdateProjectTable(cProjectTable v_cProject)
        {

            try
            {

                this.m_conSQL.Update(v_cProject);
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }


        }

        /// <summary>
        /// Return number of uploads pending.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfUploadsPending()
        {

            try
            {

                int iCount = 0;

                var oResults = (from oCols in this.m_conSQL.Table<cUpdatesTable>()
                                select oCols);

                iCount = oResults.Count();
                oResults = null;

                return iCount;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// v1.0.1 - Return number of notes pending.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfNotesPending()
        {

            try
            {

                int iCount = 0;

                var oResults = (from oCols in this.m_conSQL.Table<cProjectNotesTable>()
                                where oCols.AXRecID.Equals(-1)
                                select oCols);

                iCount = oResults.Count();
                oResults = null;

                return iCount;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// v1.0.10 - Return number of files pending.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfFilesPending()
        {

            try
            {

                int iCount = 0;

                var oResults = (from oCols in this.m_conSQL.Table<cProjectFilesTable>()
                                where oCols.NewFile.Equals(true) && oCols.Deleted.Equals(false)
                                select oCols);

                iCount = oResults.Count();
                oResults = null;

                return iCount;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// v1.0.10 - Return number of unit updates pending.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfUnitUpdatesPending()
        {

            try
            {

                int iCount = 0;

                var oResults = (from oCols in this.m_conSQL.Table<cUnitsUpdateTable>()                                
                                select oCols);

                iCount = oResults.Count();
                oResults = null;

                return iCount;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        /// <summary>
        /// v1.0.1 - Get AX settings update values
        /// </summary>
        /// <returns></returns>
        public List<wcfAX.SettingDetails> GetSettingsUpdates()
        {

            List<wcfAX.SettingDetails> sdSettings = new List<wcfAX.SettingDetails>();
            wcfAX.SettingDetails sdSetting;

            try
            {

                var oResults = (from oCols in this.m_conSQL.Table<cAXSettingsTable>()
                                select oCols);


                foreach (cAXSettingsTable stTable in oResults)
                {

                    sdSetting = new wcfAX.SettingDetails();
                    sdSetting.SettingName = stTable.SettingName;
                    sdSetting.LastUpdate = stTable.LastUpdate;

                    sdSettings.Add(sdSetting);

                }

                return sdSettings;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// v1.0.1 - Process updated settings
        /// </summary>
        /// <param name="v_dsSettings"></param>
        public async Task ProcessUpdatedSettings(List<wcfAX.SettingDetails> v_dsSettings)
        {

            try
            {

                //Loop through returned settings and update.
                foreach (wcfAX.SettingDetails sdSetting in v_dsSettings)
                {


                    bool bSettingUpdated = this.UpdateSettingsTable(sdSetting);
                    if (bSettingUpdated == false)
                    {
                        return;
                    }

                }


                cAppSettingsTable cSetting = this.ReturnSettings();
                cSetting.LastSettingsCheckDateTime = DateTime.Now;
                await this.SaveSettings(cSetting);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// v1.0.1 - Update setting update table.
        /// </summary>
        /// <param name="beField"></param>
        /// <returns></returns>
        private bool UpdateSettingsTable(wcfAX.SettingDetails v_sdSetting)
        {

            try
            {


                cAXSettingsTable cUpdate = null;
                int iCount = 0;


                var oResults = (from oCols in this.m_conSQL.Table<cAXSettingsTable>()
                                where (oCols.SettingName.Equals(v_sdSetting.SettingName))
                                select oCols);

                if (oResults.Count() > 0)
                {
                    cUpdate = oResults.Single<cAXSettingsTable>();
                }


                if (cUpdate == null)
                {
                    cUpdate = new cAXSettingsTable();
                    cUpdate.SettingName = v_sdSetting.SettingName;
                    cUpdate.SettingValue = v_sdSetting.SettingValue;
                    cUpdate.LastUpdate = v_sdSetting.LastUpdate;

                    iCount = this.m_conSQL.Insert(cUpdate);
                }
                else
                {

                    cUpdate.SettingValue = v_sdSetting.SettingValue;
                    cUpdate.LastUpdate = v_sdSetting.LastUpdate;

                    iCount = this.m_conSQL.Update(cUpdate);
                }

                if (iCount > 0)
                {
                    return true;
                }

                return false;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }


        /// <summary>
        /// Get the base enum update values from the table.
        /// </summary>
        /// <returns></returns>
        public List<wcfAX.BaseEnumField> GetBaseEnumUpdates()
        {

            List<wcfAX.BaseEnumField> beFields = new List<wcfAX.BaseEnumField>();
            wcfAX.BaseEnumField beField;

            try
            {

                var oResults = (from oCols in this.m_conSQL.Table<cBaseEnumUpdateTable>()
                                select oCols);


                foreach (cBaseEnumUpdateTable utTable in oResults)
                {

                    beField = new wcfAX.BaseEnumField();
                    beField.TableName = utTable.TableName;
                    beField.FieldName = utTable.FieldName;
                    beField.LastUpdate = utTable.LastUpdate;

                    beFields.Add(beField);

                }

                return beFields;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// Process updated base enums
        /// </summary>
        /// <param name="v_beFields"></param>
        public async Task ProcessUpdatedBaseEnums(List<wcfAX.BaseEnumField> v_beFields)
        {

            try
            {

                //Loop through returned base enums and update.
                foreach (wcfAX.BaseEnumField beField in v_beFields)
                {

                    bool bCleared = this.ClearExistingBaseEnums(beField.TableName, beField.FieldName);
                    if (bCleared == false)
                    {
                        return;
                    }

                    bool bEnumsAdded = this.AddNewBaseEnums(beField);
                    if (bEnumsAdded == false)
                    {
                        return;
                    }


                    bool bEnumUpdated = this.UpdateBaseEnumTable(beField);
                    if (bEnumUpdated == false)
                    {
                        return;
                    }

                }


                cAppSettingsTable cSetting = this.ReturnSettings();
                cSetting.LastBaseEnumCheckDateTime = DateTime.Now;
                await this.SaveSettings(cSetting);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// Update base enum update table.
        /// </summary>
        /// <param name="beField"></param>
        /// <returns></returns>
        private bool UpdateBaseEnumTable(wcfAX.BaseEnumField v_beField)
        {

            try
            {


                cBaseEnumUpdateTable cUpdate = null;
                int iCount = 0;


                var oResults = (from oCols in this.m_conSQL.Table<cBaseEnumUpdateTable>()
                                where (oCols.TableName.Equals(v_beField.TableName) && oCols.FieldName.Equals(v_beField.FieldName))
                                select oCols);

                if (oResults.Count() > 0)
                {
                    cUpdate = oResults.Single<cBaseEnumUpdateTable>();
                }


                if (cUpdate == null)
                {
                    cUpdate = new cBaseEnumUpdateTable();
                    cUpdate.TableName = v_beField.TableName;
                    cUpdate.FieldName = v_beField.FieldName;
                    cUpdate.LastUpdate = v_beField.LastUpdate;

                    iCount = this.m_conSQL.Insert(cUpdate);
                }
                else
                {

                    cUpdate.LastUpdate = v_beField.LastUpdate;
                    iCount = this.m_conSQL.Update(cUpdate);
                }

                if (iCount > 0)
                {
                    return true;
                }

                return false;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// Add new base enums
        /// </summary>
        /// <param name="beField"></param>
        /// <returns></returns>
        private bool AddNewBaseEnums(wcfAX.BaseEnumField beField)
        {

            cBaseEnumsTable beUpdate = null;

            try
            {

                foreach (wcfAX.BaseEnumValue beValue in beField.BaseEnums)
                {
                    beUpdate = new cBaseEnumsTable();
                    beUpdate.TableName = beField.TableName;
                    beUpdate.FieldName = beField.FieldName;
                    beUpdate.EnumName = beValue.BaseName;
                    beUpdate.EnumValue = beValue.BaseValue;

                    this.m_conSQL.Insert(beUpdate);

                }

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        /// <summary>
        /// Clear existing base enums
        /// </summary>
        /// <param name="v_sTableName"></param>
        /// <param name="v_sFieldName"></param>
        /// <returns></returns>
        private bool ClearExistingBaseEnums(string v_sTableName, string v_sFieldName)
        {

            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("DELETE FROM cBaseEnumsTable");
                sbSQL.Append(" WHERE TableName='" + v_sTableName + "'");
                sbSQL.Append(" AND FieldName='" + v_sFieldName + "'");

                this.m_conSQL.Execute(sbSQL.ToString());

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - PARAM(TableName=" + v_sTableName + ",FieldName=" + v_sFieldName + ")");

            }
        }

        /// <summary>
        /// Save sub project file
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <param name="v_sFileName"></param>
        /// <param name="v_sComments"></param>
        /// <param name="v_dModDate"></param>
        /// <returns></returns>
        public bool SaveSubProjectFile(string v_sSubProjectNo, string v_sFileName, string v_sComments, DateTime v_dModDate, bool v_bIsNew_InsertOnly)
        {

            bool bDoInsert = false;
            try
            {

                cProjectFilesTable cProjectFile = null;

                //See if sub project already exists in our DB.
                var oResults = (from oCols in this.m_conSQL.Table<cProjectFilesTable>()
                                where (oCols.SubProjectNo.Equals(v_sSubProjectNo))
                                && (oCols.FileName.ToLower().Equals(v_sFileName.ToLower()))
                                select oCols);

                //If match extract data.
                if (oResults.Count() > 0)
                {
                    cProjectFile = oResults.Single<cProjectFilesTable>();
                }

                //If null, we need to create a new one.
                if (cProjectFile == null)
                {
                    //This sub project is new so we need to insert the data.
                    bDoInsert = true;

                    //Set key fields
                    cProjectFile = new cProjectFilesTable();
                    cProjectFile.SubProjectNo = v_sSubProjectNo;
                    cProjectFile.FileName = v_sFileName;
                    cProjectFile.NewFile = v_bIsNew_InsertOnly;

                }

                //Common values regardless of update or insert.
                cProjectFile.NoteText = v_sComments;
                cProjectFile.ModDateTime = v_dModDate;

                if (bDoInsert == true)
                {
                    this.m_conSQL.Insert(cProjectFile);
                }
                else
                {
                    this.m_conSQL.Update(cProjectFile);
                }

                return true;

            }
            catch (Exception ex)
            {              
                throw new Exception(ex.Message + " - PARAM(SubProjectNo=" + v_sSubProjectNo + ",FileName=" + v_sFileName + ",Comment=" + v_sComments + ",ModDateTime=" + v_dModDate.ToString() + ",IsNew_InsertOnly=" + v_bIsNew_InsertOnly.ToString() + ")");

            }
        }

        /// <summary>
        /// v1.0.1 - Fetch projects to sync
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<cProjectSearch> FetchProjectsToSync()
        {

            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("SELECT ProjectNo");
                sbSQL.Append(",ProjectName");
                sbSQL.Append(",COUNT(*) AS SubProjectQty");
                sbSQL.Append(",MIN(DateLastSynced) AS LastSync");
                
                sbSQL.Append(" FROM cProjectTable");

                sbSQL.Append(" GROUP BY ProjectNo");

                sbSQL.Append(" ORDER BY LastSync");

                sbSQL.Append(" Limit 5");

                return new ObservableCollection<cProjectSearch>(this.m_conSQL.Query<cProjectSearch>(sbSQL.ToString()));

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

  
        /// <summary>
        /// v1.0.1 - Save sub project data
        /// </summary>
        /// <param name="v_cSubProject"></param>
        /// <returns></returns>
        public bool UpdateSubProjectData(cProjectTable v_cSubProject)
        {

            try
            {


                if (v_cSubProject.IDKey < 1)
                {
                    this.m_conSQL.Insert(v_cSubProject);
                }
                else
                {
                    this.m_conSQL.Update(v_cSubProject);
                }
                
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// Save sub project data away
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <param name="v_sProjectName"></param>
        /// <param name="v_spData"></param>
        /// <returns></returns>
        public cDataAccess.SaveSubProjectDataResult SaveSubProjectData(string v_sProjectNo, string v_sProjectName, wcfAX.SubProjectData v_spData)
        {

            bool bDoInsert = false;
            bool bNotesSaved = false;
            cDataAccess.SaveSubProjectDataResult rResult = new SaveSubProjectDataResult();
            rResult.bSavedOK = false;
            rResult.bProjectAdded = false;

            try
            {

                cProjectTable cProject = null;

                //See if sub project already exists in our DB.
                var oResults = (from oCols in this.m_conSQL.Table<cProjectTable>()
                                where (oCols.SubProjectNo.Equals(v_spData.ProjId))
                                select oCols);

                //If match extract data.
                if (oResults.Count() > 0)
                {
                    cProject = oResults.Single<cProjectTable>();
                }

                //If null, we need to create a new one.
                if (cProject == null)
                {
                    //This sub project is new so we need to insert the data.
                    bDoInsert = true;

                    rResult.bProjectAdded = true;

                    //Set key fields
                    cProject = new cProjectTable();
                    cProject.ProjectNo = v_sProjectNo;
                    cProject.ProjectName = cSettings.ReturnString(v_sProjectName);
                    cProject.SubProjectNo = v_spData.ProjId;
                    cProject.SubProjectName = cSettings.ReturnString(v_spData.Name);
                    cProject.DateLastSynced = DateTime.Now;

                }


                //Update remaining fields.
                cProject.AlternativeContactMobileNo = cSettings.ReturnString(v_spData.MXMAlternativeContactMobileNo);
                cProject.AlternativeContactName = cSettings.ReturnString(v_spData.MXMAlternativeContactName);
                cProject.AlternativeContactTelNo = cSettings.ReturnString(v_spData.MXMAlternativeContactTelNo);
                cProject.ABPAXAsbestosPresumed = v_spData.ABPAXASBESTOSPRESUMED;
                cProject.ABPAXInstallationType = v_spData.ABPAXINSTALLATIONTYPE;
              
                cProject.MxmConfirmedAppointmentIndicator = v_spData.MXMConfirmedAppointmentIndicator;
                cProject.MxmContactBySMS = v_spData.MXMContactBySMS;
                cProject.ABPAXPermanentGasVent = v_spData.ABPAXPERMANENTGASVENT;
                cProject.DeliveryCity = cSettings.ReturnString(v_spData.DeliveryCity);
                cProject.DeliveryStreet = cSettings.ReturnString(v_spData.DeliveryStreet);
                cProject.DisabledAdaptionsRequired = v_spData.MXMDisabledAdaptionsRequired;
                cProject.DlvState = cSettings.ReturnString(v_spData.DlvState);
                cProject.DlvZipCode = cSettings.ReturnString(v_spData.DlvZipCode);
                cProject.MxmDoorChoiceFormReceived = v_spData.MXMDoorChoiceFormReceived;
                cProject.EndDateTime = this.ConvertDateTimeToNullable(v_spData.EndDateTime.Value);
                cProject.ABPAXFloorLevel = v_spData.ABPAXFLOORLEVEL;
                cProject.InstallStatusName = this.GetEnumValueName("ProjTable", "Mxm1002InstallStatus", Convert.ToInt32(v_spData.Mxm1002InstallStatus));
                cProject.ABPAXStructuralFaults = v_spData.ABPAXSTRUCTURALFAULTS;
                cProject.ModifiedDateTime = v_spData.MODIFIEDDATETIME;
                cProject.Mxm1002InstallStatus = v_spData.Mxm1002InstallStatus;
                cProject.Mxm1002ProgressStatus = v_spData.Mxm1002ProgressStatus;
                cProject.MXM1002SequenceNr = v_spData.MXM1002SequenceNr;
                cProject.MXM1002TrfDate = this.ConvertDateTimeToNullable(v_spData.MXM1002TrfDate.Value);
                cProject.MxmProjDescription = cSettings.ReturnString(v_spData.MxmProjDescription);
                cProject.MxmNextDaySMS = v_spData.MXMNextDaySMS;                
                cProject.ProgressStatusName = this.GetEnumValueName("ProjTable", "Mxm1002ProgressStatus", Convert.ToInt32(v_spData.Mxm1002ProgressStatus));
                cProject.PropertyType = v_spData.MXMPropertyType;                
                cProject.ResidentMobileNo = cSettings.ReturnString(v_spData.MXMResidentMobileNo);
                cProject.ResidentName = cSettings.ReturnString(v_spData.MXMResidentName);
                cProject.ResidentTelNo = cSettings.ReturnString(v_spData.MXMTelephoneNo);
                cProject.ABPAXWindowBoard = v_spData.ABPAXWINDOWBOARD;
                cProject.ABPAXAccessEquipment = v_spData.ABPAXACCESSEQUIPMENT;
                cProject.ABPAXServicesToMove = v_spData.ABPAXSERVICESTOMOVE;
      
                cProject.ABPAXInternalDamage = v_spData.ABPAXINTERNDAMAGE;
                cProject.ABPAXWorkAccessRestrictions = v_spData.ABPAXWRKACCRESTRICTIONS;
                cProject.ABPAXPublicProtection = v_spData.ABPAXPUBLICPROTECT;

                //v1.0.1 - Update status details.
                cProject.Status = v_spData.Status;
                cProject.StatusName = this.GetEnumValueName("ProjTable", "Status", Convert.ToInt32(v_spData.Status));

                cProject.MxmSMSSent = v_spData.MXMSMSSent;
                cProject.SpecialResidentNote = cSettings.ReturnString(v_spData.MXMSpecialResidentNote);
                cProject.StartDateTime = this.ConvertDateTimeToNullable(v_spData.StartDateTime.Value);
                cProject.MxmSurveyletterRequired = v_spData.MXMSurveyletterRequired;
                cProject.SurveyLetterSentDate01 = v_spData.MXMSurveyLetterSentDate01;
                cProject.SurveyLetterSentDate02 = v_spData.MXMSurveyLetterSentDate02;
                cProject.SurveyLetterSentDate03 = v_spData.MXMSurveyLetterSentDate03;
                cProject.SurveyorName = cSettings.ReturnString(v_spData.MXMSurveyorName);
                cProject.SurveyorPCTag = cSettings.ReturnString(v_spData.MXMSurveyorPCTag);
                cProject.SurveyorProfile = cSettings.ReturnString(v_spData.MXMSurveyorProfile);                
                cProject.SMMActivities_MODIFIEDDATETIME = v_spData.SMMActivities_MODIFIEDDATETIME;

                //v1.0.10 - installation specific fields.
                cProject.ABPAXINSTALLATIONTEAM = cSettings.ReturnString(v_spData.ABPAXINSTALLATIONTEAM);
                cProject.ABPAXInstallLetterSentDate01 = v_spData.ABPAXINSTALLLETTERSENTDATE01;
                cProject.ABPAXInstallLetterSentDate02  = v_spData.ABPAXINSTALLLETTERSENTDATE02;
                cProject.ABPAXInstallLetterSentDate03 = v_spData.ABPAXINSTALLLETTERSENTDATE03;      
                cProject.ABPAXInstallletterRequired  = v_spData.ABPAXINSTALLLETTERREQUIRED;       
                cProject.ABPAXInstallSMSSent =  v_spData.ABPAXINSTALLSMSSENT;    
                cProject.ABPAXInstallNextDaySMS  = v_spData.ABPAXINSTALLNEXTDAYSMS;

                //v1.0.12 - Delivery details.
                cProject.Delivery_ConfirmedAppointmentIndicator = v_spData.Delivery_ConfirmedAppointmentIndicator;
                cProject.Delivery_EndDateTime = v_spData.Delivery_EndDateTime;
                cProject.Delivery_ModifiedDateTime = v_spData.Delivery_ModifiedDateTime;
                cProject.Delivery_StartDateTime = v_spData.Delivery_StartDateTime;

                //v1.0.19 - 
                cProject.ABPAWOrderCompletedDate = v_spData.ABPAWORDERCOMPLETEDDATE;
                cProject.ABPAWOriginalSubProjectID = v_spData.ABPAWORIGINALSUBPROJECTID;


                ///v1.0.21 - Health and Safety complete
                cProject.ABPAWHealthSafetyInComplete = v_spData.ABPAXHealthSafetyIncomplete;
                cProject.ABPAWHealthSafetyInCompleteDateUploaded = v_spData.ABPAXHealthSafetyIncompleteDateUploaded;
                cProject.ABPAWHealthSafetyInCompleteUserUploaded = v_spData.ABPAXHealthSaferyIncompleteUploadedBy;


                //Update database
                if (bDoInsert == true)
                {
                    this.m_conSQL.Insert(cProject);

                }
                else
                {
                    this.m_conSQL.Update(cProject);

                }

                //v1.0.1 - Save notes.
                bNotesSaved = this.SaveSubProjectNotes(v_spData.Notes);

                if (cSettings.IsThisTheSurveyorApp() == false)
                {

                    //v1.0.10 - Save sub project units.
                    this.SaveSubProjectUnits(v_spData.ProjId, v_spData.Name, v_spData.Units);

                }
                

                rResult.bSavedOK = true;

                return rResult;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// v1.0.1 - Save sub project notes.
        /// </summary>
        /// <param name="v_ndNotes"></param>
        /// <returns></returns>
        public bool SaveSubProjectNotes(ObservableCollection<wcfAX.NoteDetails> v_ndNotes)
        {

            cProjectNotesTable pnNote;
            try
            {

                if (v_ndNotes != null)
                {

                    foreach (wcfAX.NoteDetails ndDetail in v_ndNotes)
                    {

                        pnNote = new cProjectNotesTable();
                        pnNote.AXRecID = ndDetail.AXRecID;
                        pnNote.InputDateTime = ndDetail.InputDate;
                        pnNote.NoteText = ndDetail.NoteText;
                        pnNote.SubProjectNo = ndDetail.ProjectNo;
                        pnNote.UserName = ndDetail.UserName;
                        pnNote.UserProfile = ndDetail.UserProfile;
                        pnNote.NoteType = ndDetail.NoteType;

                        this.SaveSubProjectNote(pnNote);

                    }

                }

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// v1.0.1 - Save sub project note.
        /// </summary>
        /// <param name="cNote"></param>
        /// <returns></returns>
        public bool SaveSubProjectNote(cProjectNotesTable cNote)
        {
            try
            {

                this.m_conSQL.Insert(cNote);
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }


        }

        /// <summary>
        /// Update unit.
        /// </summary>
        /// <param name="v_cUnit"></param>
        /// <returns></returns>
        public bool UpdateUnit(cUnitsTable v_cUnit)
        {

            try
            {

                this.m_conSQL.Update(v_cUnit);
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }


        }

        /// <summary>
        /// v1.0.1 - Save sub project units.
        /// </summary>
        /// <param name="v_ndNotes"></param>
        /// <returns></returns>
        public bool SaveSubProjectUnits(string v_sSubProjectNo,string v_sSubProjectName, ObservableCollection<wcfAX.UnitDetails> v_udUnits)
        {

            cUnitsTable udUnit;
            
            try
            {

                if (v_udUnits != null)
                {

                    bool bInsert = false;
                    foreach (wcfAX.UnitDetails udDetail in v_udUnits)
                    {

                        bInsert = false;
                        udUnit = this.FetchSubProjectUnit(v_sSubProjectNo, udDetail.iUNITNUMBER);
                        if (udUnit == null)
                        {
                            udUnit = new cUnitsTable();
                            bInsert = true;
                        }
                     
                        udUnit.SubProjectNo = v_sSubProjectNo;
                        udUnit.SubProjectName = v_sSubProjectName;
                        udUnit.UnitNo = udDetail.iUNITNUMBER;
                        udUnit.ItemID = udDetail.sITEMID;
                        udUnit.Style = udDetail.sSTYLE;
                        udUnit.InstalledStatus = udDetail.iInstalledStatus;
                        udUnit.UnitLocation = udDetail.sUNITLOCATION;


                        if (bInsert == true)
                        {
                            this.m_conSQL.Insert(udUnit);
                        }
                        else
                        {
                            this.m_conSQL.Update(udUnit);
                        }

                    }
                  
                }

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <param name="v_iUnitNo"></param>
        /// <returns></returns>
        public cUnitsTable FetchSubProjectUnit(string v_sSubProjectNo, int v_iUnitNo)
        {

            cUnitsTable cUnit = null;
            try
            {
               
                //See if sub project already exists in our DB.
                var oResults = (from oCols in this.m_conSQL.Table<cUnitsTable>()
                                where (oCols.SubProjectNo.Equals(v_sSubProjectNo) && (oCols.UnitNo.Equals(v_iUnitNo)))
                                select oCols);

                //If match extract data.
                if (oResults.Count() > 0)
                {
                    cUnit = oResults.Single<cUnitsTable>();
                }

                return cUnit;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " v_sSubProjectNo(" + v_sSubProjectNo + "),v_iUnitNo(" + v_iUnitNo.ToString() + ")");

            }

        }

        /// <summary>
        /// Convert date time to field to null if year is 1900
        /// </summary>
        /// <param name="v_dDateTime"></param>
        /// <returns></returns>
        private DateTime? ConvertDateTimeToNullable(DateTime v_dDateTime)
        {

            try
            {
                if (v_dDateTime.Year == 1900)
                {
                    return null;
                }

                return v_dDateTime;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " DateTime(" + v_dDateTime.ToString() + ")");

            }

        }

        /// <summary>
        /// Check if project has already been downloaded.
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <returns></returns>
        public cProjectTable IsProjectAlreadyDownloaded(string v_sProjectNo)
        {

            cProjectTable cProject = null;
            try
            {

                var oResults = (from oCols in this.m_conSQL.Table<cProjectTable>()
                                where (oCols.ProjectNo.Equals(v_sProjectNo))
                                select oCols);


                foreach (cProjectTable cResult in oResults)
                {
                    cProject = cResult;
                    break;
                }



                return cProject;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " ProjectNo(" + v_sProjectNo + ")");

            }
        }


        /// <summary>
        /// Return list of sub projects for project.
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <returns></returns>
        public List<cProjectTable> ReturnSubProjectsForProject(string v_sProjectNo)
        {

            List<cProjectTable> cProjects = null;
            try
            {

                var oResults = (from oCols in this.m_conSQL.Table<cProjectTable>()
                                where (oCols.ProjectNo.Equals(v_sProjectNo))
                                select oCols);

                cProjects = oResults.ToList<cProjectTable>();

                return cProjects;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " ProjectNo(" + v_sProjectNo + ")");

            }

        }

        /// <summary>
        /// v1.0.2 - Return sub project quantity
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <returns></returns>
        public int ReturnSubProjectQty(string v_sProjectNo)
        {

            StringBuilder sbSQL = new StringBuilder();
            int iCount = 0;
            try
            {

                sbSQL.Append("SELECT ProjectNo");
                sbSQL.Append(", COUNT(*) AS Total");
                
                sbSQL.Append(" FROM cProjectTable");
                sbSQL.Append(" GROUP BY ProjectNo");

                List<cTotal> cResult = this.m_conSQL.Query<cTotal>(sbSQL.ToString());
                foreach (cTotal cProject in cResult)
                {

                    if (cProject.Total.HasValue == true)
                    {

                        iCount = cProject.Total.Value;

                    }

                }

                return iCount;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " ProjectNo(" + v_sProjectNo + ")");

            }

        }


        /// <summary>
        /// Delete sub project files from table.
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <returns></returns>
        public bool DeleteSubProjectFiles(string v_sSubProjectNo)
        {

            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("DELETE FROM cProjectFilesTable");
                sbSQL.Append(" WHERE SubProjectNo='" + v_sSubProjectNo + "'");

                this.m_conSQL.Execute(sbSQL.ToString());

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " SubProjectNo(" + v_sSubProjectNo + ")");

            }

        }

        /// <summary>
        /// v1.0.2 - Delete sub project notes from table.
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <returns></returns>
        public bool DeleteSubProjectNotes(string v_sSubProjectNo)
        {

            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("DELETE FROM cProjectNotesTable");
                sbSQL.Append(" WHERE SubProjectNo='" + v_sSubProjectNo + "'");

                this.m_conSQL.Execute(sbSQL.ToString());

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " SubProjectNo(" + v_sSubProjectNo + ")");

            }

        }


        /// <summary>
        /// v1.0.10 - Delete sub project units from table.
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <returns></returns>
        public bool DeleteSubProjectUnits(string v_sSubProjectNo)
        {

            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("DELETE FROM cUnitsTable");
                sbSQL.Append(" WHERE SubProjectNo='" + v_sSubProjectNo + "'");

                this.m_conSQL.Execute(sbSQL.ToString());

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " SubProjectNo(" + v_sSubProjectNo + ")");

            }

        }

        /// <summary>
        /// Delete all project data from main project table.
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <returns></returns>
        public bool DeleteProjectFromProjectTable(string v_sProjectNo)
        {

            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("DELETE FROM cProjectTable");
                sbSQL.Append(" WHERE ProjectNo='" + v_sProjectNo + "'");

                this.m_conSQL.Execute(sbSQL.ToString());

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " ProjectNo(" + v_sProjectNo + ")");

            }

        }

        /// <summary>
        /// v1.0.10 - Delete sub project from project table
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public bool DeleteSubProjectFromProjectTable(string v_sSubProjectNo)
        {

            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("DELETE FROM cProjectTable");
                sbSQL.Append(" WHERE SubProjectNo='" + v_sSubProjectNo + "'");

                this.m_conSQL.Execute(sbSQL.ToString());

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " SubProjectNo(" + v_sSubProjectNo + ")");

            }

        }



        /// <summary>
        /// Return list of pending updates for sub project.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public List<cUpdatesTable> ReturnPendingUpdatesForSubProject(string v_sSubProjectNo)
        {

            List<cUpdatesTable> cUpdates = null;
            try
            {

                var oResults = (from oCols in this.m_conSQL.Table<cUpdatesTable>()
                                where (oCols.SubProjectNo.Equals(v_sSubProjectNo))
                                select oCols);

                cUpdates = oResults.ToList<cUpdatesTable>();

                return cUpdates;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " SubProjectNo(" + v_sSubProjectNo + ")");

            }

        }


        /// <summary>
        /// Fetch files list for sub project.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <param name="v_sFileName"></param>
        /// <returns></returns>
        public List<cProjectFilesTable> FetchSubProjectFilesList(string v_sSubProjectNo, bool v_bIncludeDeleted)
        {

            try
            {

                return this.FetchSubProjectFilesList(v_sSubProjectNo, String.Empty, v_bIncludeDeleted);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " SubProjectNo(" + v_sSubProjectNo + "), IncludeDeleted(" + v_bIncludeDeleted.ToString() + ")");

            }

        }

        /// <summary>
        /// Fetch files list for sub project.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <param name="v_sFileName"></param>
        /// <returns></returns>
        public List<cProjectFilesTable> FetchSubProjectFilesList(string v_sSubProjectNo, string v_sFileName, bool v_bIncludeDeleted)
        {


            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("SELECT * FROM cProjectFilesTable");
                sbSQL.Append(" WHERE SubProjectNo = '" + v_sSubProjectNo + "'");
                if (v_sFileName.Length > 0)
                {
                    sbSQL.Append(" AND FileName = '" + v_sFileName.Replace("'", "''") + "'");
                }

                if (v_bIncludeDeleted == false)
                {
                    sbSQL.Append(" AND (Deleted IS NULL OR Deleted <> '1')");

                }

                return this.m_conSQL.Query<cProjectFilesTable>(sbSQL.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " SubProjectNo(" + v_sSubProjectNo + "), FileName(" + v_sFileName + "), IncludeDeleted(" + v_bIncludeDeleted.ToString() + ")");

            }

        }

        /// <summary>
        /// v1.0.19 - Fetch files for project.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <param name="v_sFileName"></param>
        /// <returns></returns>
        public List<cProjectFilesTable> FetchProjectFilesList(string v_sProjectNo)
        {

            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("SELECT * FROM cProjectFilesTable");
                sbSQL.Append(" WHERE SubProjectNo LIKE '" + v_sProjectNo + "-%'");              

                return this.m_conSQL.Query<cProjectFilesTable>(sbSQL.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " ProjectNo(" + v_sProjectNo + ")");

            }

        }

        /// <summary>
        /// Mark file as deleted.
        /// </summary>
        /// <param name="v_iFileIDKey"></param>
        /// <returns></returns>
        public bool MarkFileAsDeleted(int v_iFileIDKey)
        {


            try
            {

                var oResults = (from oCols in this.m_conSQL.Table<cProjectFilesTable>()
                                where (oCols.IDKey.Equals(v_iFileIDKey))
                                select oCols);

                cProjectFilesTable cFile = null;

                foreach (var oResult in oResults)
                {
                    cFile = oResult;

                    cFile.Deleted = true;

                    this.m_conSQL.Update(cFile);

                    return true;

                }

                return false;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " FileIDKey(" + v_iFileIDKey.ToString() + ")");

            }

        }

        /// <summary>
        /// Fetch sub project list with uploads pending.
        /// </summary>
        /// <returns></returns>
        public List<cSubProjectSync> FetchSubProjectsWithUploads()
        {

            StringBuilder sbSQL = new StringBuilder();

            try
            {

                bool bSurveyorApp = cSettings.IsThisTheSurveyorApp();

                sbSQL.Append("SELECT PT.SubProjectNo");
                sbSQL.Append(",UT.UpdateQty");                
                sbSQL.Append(",PFT.FileUpdateQty");
                sbSQL.Append(",PNT.NotesUpdateQty");

                if (bSurveyorApp == false) { sbSQL.Append(",UUT.UnitUpdateQty"); };


                sbSQL.Append(" FROM cProjectTable AS PT");

                sbSQL.Append(" LEFT JOIN (SELECT COUNT(*) AS UpdateQty,SubProjectNo FROM cUpdatesTable GROUP BY SubProjectNo) AS UT");
                sbSQL.Append(" ON UT.SubProjectNo = PT.SubProjectNo");

                if (bSurveyorApp == false)
                {
                    sbSQL.Append(" LEFT JOIN (SELECT COUNT(*) AS UnitUpdateQty,SubProjectNo FROM cUnitsUpdateTable GROUP BY SubProjectNo) AS UUT");
                    sbSQL.Append(" ON UUT.SubProjectNo = PT.SubProjectNo");
                }

                sbSQL.Append(" LEFT JOIN (SELECT COUNT(*) AS FileUpdateQty,SubProjectNo FROM cProjectFilesTable WHERE NewFile='1' GROUP BY SubProjectNo) AS PFT");
                sbSQL.Append(" ON PFT.SubProjectNo = PT.SubProjectNo");

                sbSQL.Append(" LEFT JOIN (SELECT COUNT(*) AS NotesUpdateQty,SubProjectNo FROM cProjectNotesTable WHERE AXRecID = -1 GROUP BY SubProjectNo) AS PNT");
                sbSQL.Append(" ON PNT.SubProjectNo = PT.SubProjectNo");

                sbSQL.Append(" WHERE UT.UpdateQty > 0");                
                sbSQL.Append(" OR PFT.FileUpdateQty > 0");
                sbSQL.Append(" OR PNT.NotesUpdateQty > 0");

                if (bSurveyorApp == false) { sbSQL.Append(" OR UUT.UnitUpdateQty > 0"); };

                sbSQL.Append(" ORDER BY PT.SubProjectNo");

              
                return this.m_conSQL.Query<cSubProjectSync>(sbSQL.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// Remove changes from the upload table.
        /// </summary>
        /// <param name="v_cUpdates"></param>
        /// <returns></returns>
        public bool RemoveChangesFromUploadTable(List<cUpdatesTable> v_cUpdates)
        {

            try
            {

                foreach (cUpdatesTable cUpdate in v_cUpdates)
                {
                    this.m_conSQL.Delete(cUpdate);

                }

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// Return list of new files for uploading.
        /// </summary>
        /// <returns></returns>
        public List<cProjectFilesTable> ReturnNewFilesForUploading()
        {

            List<cProjectFilesTable> cFiles = null;
            try
            {

                var oResults = (from oCols in this.m_conSQL.Table<cProjectFilesTable>()
                                where (oCols.NewFile.Equals(true) && oCols.Deleted.Equals(false))
                                orderby oCols.SubProjectNo
                                select oCols);

                cFiles = oResults.ToList<cProjectFilesTable>();

                return cFiles;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// Update file as not new.
        /// </summary>
        /// <param name="v_cFile"></param>
        /// <returns></returns>
        public bool UpdateFileAsNotNew(cProjectFilesTable v_cFile)
        {

            try
            {

                v_cFile.NewFile = false;
                this.m_conSQL.Update(v_cFile);

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// v1.0.1 - Fetch sub project file update date and time values for project
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public List<cSubProjectSync> FetchSubProjectFileCountForProject(string v_sProjectNo)
        {

            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("SELECT");

                sbSQL.Append(" PFT.SubProjectNo");
                sbSQL.Append(",COUNT(*) AS UpdateQty");

                sbSQL.Append(" FROM cProjectFilesTable AS PFT");

                sbSQL.Append(" INNER JOIN cProjectTable AS PT");
                sbSQL.Append(" ON PFT.SubProjectNo = PT.SubProjectNo");

                sbSQL.Append(" WHERE PT.ProjectNo = '" + v_sProjectNo + "'");

                sbSQL.Append(" GROUP BY PFT.SubProjectNo");

                return this.m_conSQL.Query<cSubProjectSync>(sbSQL.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " ProjectNo(" + v_sProjectNo + ")");

            }

        }


        /// <summary>
        /// v1.0.1 - Fetch sub project file update date and time values for project
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public List<cProjectFilesTable> FetchSubProjectFilesForChecking(string v_sSubProjectNo)
        {

            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("SELECT");
                sbSQL.Append(" IDKey");
                sbSQL.Append(",SubProjectNo");
                sbSQL.Append(",NoteText");
                sbSQL.Append(",FileName");
                sbSQL.Append(",ModDateTime");

                sbSQL.Append(" FROM cProjectFilesTable");

                sbSQL.Append(" WHERE SubProjectNo = '" + v_sSubProjectNo + "'");

                return this.m_conSQL.Query<cProjectFilesTable>(sbSQL.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " SubProjectNo(" + v_sSubProjectNo + ")");

            }

        }




        /// <summary>
        /// Fetch sub project update date and time values for project
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public List<cSubProjectSyncUpdateValues> FetchSubProjectUpdateDateTime(string v_sProjectNo,int v_iLimit)
        {

            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("SELECT SubProjectNo");
                sbSQL.Append(",ModifiedDateTime");
                sbSQL.Append(",SMMActivities_MODIFIEDDATETIME");
                sbSQL.Append(",Delivery_ModifiedDateTime");

                sbSQL.Append(" FROM cProjectTable ");

                sbSQL.Append(" WHERE ProjectNo='" + v_sProjectNo + "'");

                sbSQL.Append(" ORDER BY DateLastSynced");

                if (v_iLimit > 0)
                {
                    sbSQL.Append(" LIMIT " + v_iLimit.ToString());
                }

                return this.m_conSQL.Query<cSubProjectSyncUpdateValues>(sbSQL.ToString());


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " ProjectNo(" + v_sProjectNo + ")");

            }

        }

        /// <summary>
        /// Update modified date times on project.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <param name="v_dModDateTime"></param>
        /// <param name="v_dActModDateTime"></param>
        /// <param name="v_dNotModDateTime"></param>
        /// <returns></returns>
        public bool UpdateSubProjectUpdateDates(string v_sSubProjectNo, DateTime v_dModDateTime, DateTime v_dActModDateTime)
        {

            cProjectTable pTable = null;
            try
            {

                pTable = this.GetSubProjectProjectData(v_sSubProjectNo);

                pTable.ModifiedDateTime = v_dModDateTime;
                pTable.SMMActivities_MODIFIEDDATETIME = v_dActModDateTime;
                
                this.m_conSQL.Update(pTable);

                return true;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " SubProjectNo(" + v_sSubProjectNo + "), ModDateTime(" + v_dModDateTime.ToString() + ")");

            }


        }

        /// <summary>
        /// v1.0.2 - Does project have any pending uploads.
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <returns></returns>
        public bool DoesProjectHaveAnyPendingUploads(string v_sProjectNo)
        {

            int iTotal = 0;
            try
            {


                //1. Uploads table
                iTotal = this.TotalPendingUpdatesForProject(v_sProjectNo);
                if (iTotal > 0)
                {
                    return true;
                }


                //2. Notes table
                iTotal = this.TotalPendingNotesUpdatesForProject(v_sProjectNo);
                if (iTotal > 0)
                {
                    return true;
                }


                //3. Photos table.
                iTotal = this.TotalPendingFileUploadsForProject(v_sProjectNo);
                if (iTotal > 0)
                {
                    return true;
                }

                //
                return false;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " ProjectNo(" + v_sProjectNo + ")");

            }

        }

        /// <summary>
        /// v1.0.2 - Return number of rows in uploads table for project.
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <returns></returns>
        public int TotalPendingUpdatesForProject(string v_sProjectNo)
        {

            int iTotal = 0;
            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("SELECT PT.ProjectNo");
                sbSQL.Append(",COUNT(*) AS Total");
                
                sbSQL.Append(" FROM cUpdatesTable AS UT");

                sbSQL.Append(" INNER JOIN cProjectTable AS PT");
                sbSQL.Append(" ON UT.SubProjectNo = PT.SubProjectNo");

                sbSQL.Append(" WHERE PT.ProjectNo = '" + v_sProjectNo + "'");

                sbSQL.Append(" GROUP BY PT.ProjectNo");

                List<cTotal> ctTotal = this.m_conSQL.Query<cTotal>(sbSQL.ToString());

                foreach(cTotal cCount in ctTotal)
                {
                    if (cCount.Total.HasValue == true)
                    {
                        iTotal = cCount.Total.Value;
                    }
                }

                return iTotal;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " ProjectNo(" + v_sProjectNo + ")");

            }

        }

        /// <summary>
        /// v1.0.2 - Return number of new un-synced notes in project notes table for project.
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <returns></returns>
        public int TotalPendingNotesUpdatesForProject(string v_sProjectNo)
        {

            int iTotal = 0;
            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("SELECT PT.ProjectNo");
                sbSQL.Append(",COUNT(*) AS Total");

                sbSQL.Append(" FROM cProjectNotesTable AS NT");

                sbSQL.Append(" INNER JOIN cProjectTable AS PT");
                sbSQL.Append(" ON NT.SubProjectNo = PT.SubProjectNo");

                sbSQL.Append(" WHERE PT.ProjectNo = '" + v_sProjectNo + "'");
                sbSQL.Append(" AND NT.AXRecID = -1");

                sbSQL.Append(" GROUP BY PT.ProjectNo");

                List<cTotal> ctTotal = this.m_conSQL.Query<cTotal>(sbSQL.ToString());

                foreach (cTotal cCount in ctTotal)
                {
                    if (cCount.Total.HasValue == true)
                    {
                        iTotal = cCount.Total.Value;
                    }
                }

                return iTotal;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " ProjectNo(" + v_sProjectNo + ")");

            }

        }

        /// <summary>
        /// v1.0.2 - Return number of new files in project files table for project.
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <returns></returns>
        public int TotalPendingFileUploadsForProject(string v_sProjectNo)
        {

            int iTotal = 0;
            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("SELECT PT.ProjectNo");
                sbSQL.Append(",COUNT(*) AS Total");

                sbSQL.Append(" FROM cProjectFilesTable AS FT");

                sbSQL.Append(" INNER JOIN cProjectTable AS PT");
                sbSQL.Append(" ON FT.SubProjectNo = PT.SubProjectNo");

                sbSQL.Append(" WHERE PT.ProjectNo = '" + v_sProjectNo + "'");
                sbSQL.Append(" AND FT.Deleted = '0'");
                sbSQL.Append(" AND FT.NewFile = '1'");

                sbSQL.Append(" GROUP BY PT.ProjectNo");

                List<cTotal> ctTotal = this.m_conSQL.Query<cTotal>(sbSQL.ToString());

                foreach (cTotal cCount in ctTotal)
                {
                    if (cCount.Total.HasValue == true)
                    {
                        iTotal = cCount.Total.Value;
                    }
                }

                return iTotal;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " ProjectNo(" + v_sProjectNo + ")");

            }

        }

        ///// <summary>
        ///// Fetch list of projects.
        ///// </summary>
        ///// <returns></returns>
        //public List<cProjectSearch> FetchProjects()
        //{

        //    StringBuilder sbSQL = new StringBuilder();

        //    try
        //    {

        //        sbSQL.Append("SELECT ProjectNo,ProjectName");
        //        sbSQL.Append(", COUNT(*) AS SubProjectQty");
        //        sbSQL.Append(" FROM cProjectTable");
        //        sbSQL.Append(" GROUP BY ProjectNo,ProjectName");

        //        return this.m_conSQL.Query<cProjectSearch>(sbSQL.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
        //        return null;

        //    }

        //}


        /// <summary>
        /// v1.0.1 - Get setting value from ax setting table.
        /// </summary>
        /// <param name="v_sSettingName"></param>
        /// <returns></returns>
        public string GetSettingValue(string v_sSettingName)
        {

            string sRtnValue = string.Empty;
            try
            {

                var oResults = (from oCols in this.m_conSQL.Table<cAXSettingsTable>()
                                where (oCols.SettingName.ToLower().Equals(v_sSettingName.ToLower()) == true)
                                select oCols);

                foreach (cAXSettingsTable cSetting in oResults)
                {

                    sRtnValue = cSetting.SettingValue;

                }

                return sRtnValue;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " SettingName(" + v_sSettingName + ")");

            }

        }


        /// <summary>
        /// v1.0.1 - Fetch projects for syncing
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<cProjectSearch> FetchProjectsForSync()
        {

            try
            {

                StringBuilder sbSQL = new StringBuilder();

                sbSQL.Append("SELECT ProjectNo,ProjectName,COUNT(*) AS SubProjectQty");
                sbSQL.Append(" FROM cProjectTable");
                sbSQL.Append(" GROUP BY ProjectNo,ProjectName");
                sbSQL.Append(" ORDER BY ProjectNo,ProjectName");

                return new ObservableCollection<cProjectSearch>(this.m_conSQL.Query<cProjectSearch>(sbSQL.ToString()));

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        /// <summary>
        /// v1.0.11 - Are we running in Live
        /// </summary>
        /// <returns></returns>
        public bool AreWeRunningInLive()
        {

            StringBuilder sbSQL = new StringBuilder();
            bool bInLive = false;
            try
            {

                sbSQL.Append("SELECT RunningMode");
                sbSQL.Append(" FROM cAppSettingsTable");

                List<cStatus> oStatuses = this.m_conSQL_Settings.Query<cStatus>(sbSQL.ToString());

                if (oStatuses != null)
                {
                    foreach (cStatus oStatus in oStatuses)
                    {
                        if (oStatus.RunningMode == "LIVE")
                        {
                            bInLive = true;
                        }                        
                    }

                }

                return bInLive;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }


        /// <summary>
        /// v1.0.21 - Fetch all survey failed reasons.
        /// </summary>
        /// <returns></returns>
        public List<cFailedSurveyReasonsTable> FetchAllSurveyFailedReasons()
        {

            try
            {

                var oResults = (from oCols in this.m_conSQL.Table<cFailedSurveyReasonsTable>()
                                select oCols);

                return oResults.ToList<cFailedSurveyReasonsTable>();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }


        /// <summary>
        /// v1.0.21 - Clear survey failed reasons table.
        /// </summary>
        /// <returns></returns>
        public bool ClearSurveyFailedReasonsTable()
        {

            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("DELETE FROM cFailedSurveyReasonsTable");

                this.m_conSQL.Execute(sbSQL.ToString());

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// v1.0.8 - Update Failed Survey Reason table.
        /// </summary>
        /// <returns></returns>
        public bool UpdateFailedSurveyReasonsTable(ObservableCollection<wcfAX.SurveyFailedReason> v_sfrReasons)
        {

            try
            {

                if (v_sfrReasons != null)
                {

                    //Clear down table first.
                    this.ClearSurveyFailedReasonsTable();

                    cFailedSurveyReasonsTable cAddReason;

                    List<cFailedSurveyReasonsTable> lAddReasons = new List<cFailedSurveyReasonsTable>();

                    foreach (wcfAX.SurveyFailedReason cReason in v_sfrReasons)
                    {
                        cAddReason = new cFailedSurveyReasonsTable();
                        cAddReason.Description = cReason.sReason;
                        cAddReason.DisplayOrder = cReason.iDisplayOrder;
                        cAddReason.MandatoryNote = cReason.bMandatoryNote;
                        cAddReason.ProgressStatus = cReason.iProgressStatus;

                        lAddReasons.Add(cAddReason);

                    }

                    this.m_conSQL.InsertAll(lAddReasons);

                    //Clean up
                    v_sfrReasons = null;
                    lAddReasons = null;

                    return true;

                }

                return false;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// v1.0.8 - Clear installers table.
        /// </summary>
        /// <returns></returns>
        public bool ClearInstallersTable()
        {

            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("DELETE FROM cInstallersTable");

                this.m_conSQL.Execute(sbSQL.ToString());

                return true;
                 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }


        /// <summary>
        /// v1.0.8 - Clear installers table.
        /// </summary>
        /// <returns></returns>
        public bool UpdateInstallersTable(ObservableCollection<wcfAX.InstallerDetails> v_idDetails)
        {
            
            try                                  
            {

                if (v_idDetails != null)
                {

                    cInstallersTable cAddInstaller;

                    List<cInstallersTable> lNewInstallers = new List<cInstallersTable>();

                    foreach (wcfAX.InstallerDetails cInstaller in v_idDetails)
                    {
                        cAddInstaller = new cInstallersTable();
                        cAddInstaller.AccountNum = cInstaller.sACCOUNTNUM;
                        cAddInstaller.Name = cInstaller.sNAME;

                        lNewInstallers.Add(cAddInstaller);

                    }

                    this.m_conSQL.InsertAll(lNewInstallers);

                    //Clean up
                    v_idDetails = null;
                    lNewInstallers = null;

                    return true;

                }

                return false;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// Fetch all installers.
        /// </summary>
        /// <returns></returns>
        public List<cInstallersTable> FetchAllInstallers()
        {

            try
            {

                var oResults = (from oCols in this.m_conSQL.Table<cInstallersTable>()                                
                                select oCols);

                return oResults.ToList<cInstallersTable>();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// v1.0.18 - Fetch installation team details
        /// </summary>
        /// <param name="v_sAccountNo"></param>
        /// <returns></returns>
        public cInstallersTable FetchInstallationTeam(string v_sAccountNo)
        {

            try
            {

                var oResults = (from oCols in this.m_conSQL.Table<cInstallersTable>()
                                where oCols.AccountNum.Equals(v_sAccountNo)
                                select oCols);

                foreach (cInstallersTable cInstallTeam in oResults.ToList<cInstallersTable>())
                {

                    return cInstallTeam;

                }

                return null;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// v1.0.18 - Installers Search
        /// </summary>
        /// <param name="v_sInstallerName"></param>
        /// <returns></returns>
        public List<cInstallersTable> SearchInstallers(string v_sInstallerName)
        {

            StringBuilder sbSQL = new StringBuilder();
            try
            {
                sbSQL.Append("SELECT * FROM cInstallersTable");
                sbSQL.Append(" WHERE LOWER(Name) " + this.FormatSQLCritieria(v_sInstallerName.ToLower()));
                sbSQL.Append(" ORDER BY Name");

                return this.m_conSQL.Query<cInstallersTable>(sbSQL.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }



        /// <summary>
        /// Fetch units for sub project
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public List<cUnitsTable> FetchUnitsForSubProject(string v_sSubProjectNo)
        {
            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("SELECT * FROM cUnitsTable");
                sbSQL.Append(" WHERE SubProjectNo = '" + v_sSubProjectNo.Replace("'","''") + "'");
                sbSQL.Append(" ORDER BY UnitNo");

                return this.m_conSQL.Query<cUnitsTable>(sbSQL.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Log unit update for uploading.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <param name="v_iUnitNo"></param>
        /// <param name="v_iInstallStatus"></param>
        /// <param name="v_dInstallDate"></param>
        /// <returns></returns>
        public bool LogUnitUpdate(string v_sSubProjectNo, int v_iUnitNo, int v_iInstallStatus, DateTime v_dInstallDate)
        {

            cUnitsUpdateTable cUpdate = null;
            bool bInsert = true;
            try
            {

                var oResults = (from oCols in this.m_conSQL.Table<cUnitsUpdateTable>()
                                where (oCols.SubProjectNo.Equals(v_sSubProjectNo) && oCols.UnitNo.Equals(v_iUnitNo))
                                select oCols);


                //If match extract data.
                if (oResults.Count() > 0)
                {
                    cUpdate = oResults.Single<cUnitsUpdateTable>();
                    bInsert = false;
                }

                if (cUpdate == null)
                {
                    cUpdate = new cUnitsUpdateTable();
                    cUpdate.SubProjectNo = v_sSubProjectNo;
                    cUpdate.UnitNo = v_iUnitNo;
                }


                cUpdate.InstalledDate = v_dInstallDate;
                cUpdate.InstalledStatus = v_iInstallStatus;


                if (bInsert == true)
                {
                    this.m_conSQL.Insert(cUpdate);
                }
                else
                {
                    this.m_conSQL.Update(cUpdate);
                }

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// v1.0.10 - Fetch unit updates for sub project.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public List<cUnitsUpdateTable> FetchUnitUpdatesForSubProject(string v_sSubProjectNo)
        {

            try
            {

                var oResults = (from oCols in this.m_conSQL.Table<cUnitsUpdateTable>()
                                where (oCols.SubProjectNo.Equals(v_sSubProjectNo))
                                orderby oCols.UnitNo
                                select oCols);

                return oResults.ToList<cUnitsUpdateTable>();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// v1.0.10 - Fetch the most recent install date used for the units in the specified sub project.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public DateTime FetchMostRecentUnitInstallDateForSubProject(string v_sSubProjectNo)
        {

            StringBuilder sbSQL = new StringBuilder();
            DateTime dInstallDate = DateTime.MinValue;

            try
            {

                sbSQL.Append("SELECT * ");
                sbSQL.Append(" FROM cUnitsUpdateTable");
                sbSQL.Append(" WHERE SubProjectNo = '" + v_sSubProjectNo + "'");
                sbSQL.Append(" ORDER BY InstalledDate DESC LIMIT 1");

                List<cUnitsUpdateTable> oUnits = this.m_conSQL.Query<cUnitsUpdateTable>(sbSQL.ToString());

                if (oUnits != null)
                {
                    foreach (cUnitsUpdateTable oUnit in oUnits)
                    {

                        if (oUnit.InstalledDate.HasValue == true)
                        {
                            dInstallDate = oUnit.InstalledDate.Value;

                        }
                        

                    }

                }

                return dInstallDate;
              
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// v1.0.10 - Fetch installation team for sub project.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public string FetchInstallationTeamForSubProject(string v_sSubProjectNo)
        {

            try
            {

                cProjectTable cData = this.GetSubProjectProjectData(v_sSubProjectNo);
                return cData.ABPAXINSTALLATIONTEAM;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// v1.0.10 - Delete unit updates for sub project.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public bool DeleteUnitUpdates(string v_sSubProjectNo)
        {

            StringBuilder sbSQL = new StringBuilder();
           
            try
            {

                sbSQL.Append("DELETE FROM cUnitsUpdateTable");                
                sbSQL.Append(" WHERE SubProjectNo = '" + v_sSubProjectNo + "'");

                this.m_conSQL.Execute(sbSQL.ToString());
          
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        /// <summary>
        /// Delete project from device.
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <returns></returns>
        public async  Task<bool> DeleteProjectFromDevice(string v_sProjectNo)
        {

            try
            {
                //Fetch list of sub projects.
                List<cProjectTable> cSubProjects = this.ReturnSubProjectsForProject(v_sProjectNo);
                foreach (cProjectTable cSubProject in cSubProjects)
                {

                    await this.DeleteSubProjectFromDevice(cSubProject.SubProjectNo);

                }

                //Delete project data.
                this.DeleteProjectFromProjectTable(v_sProjectNo);

                //We get here, all OK.
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - (" + v_sProjectNo + ")");

            }


        }

        /// <summary>
        /// Delete sub project from device.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public async Task<bool> DeleteSubProjectFromDevice(string v_sSubProjectNo)
        {

            try
            {
                //Delete from files table.
                this.DeleteSubProjectFiles(v_sSubProjectNo);

                //Delete from device folder.
                await cSettings.DeleteSubProjectImageFolder(v_sSubProjectNo);

                //Delete sub project notes.
                this.DeleteSubProjectNotes(v_sSubProjectNo);

                //Only process units if not the surveyor app.
                if (cSettings.IsThisTheSurveyorApp() == false)
                {

                    //Delete sub project units.
                    this.DeleteSubProjectUnits(v_sSubProjectNo);


                }

                //Delete sub project from header table.
                this.DeleteSubProjectFromProjectTable(v_sSubProjectNo);
                
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - (" + v_sSubProjectNo + ")");

            }

        }

        /// <summary>
        /// v1.0.19 - A quicker way of deleting an entire project.
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <returns></returns>
        public async Task<bool> DeleteProject(string v_sProjectNo)
        {
            StringBuilder sbSQL = new StringBuilder();
            try
            {

                //Delete notes.
                sbSQL.Append("DELETE FROM cProjectNotesTable");
                sbSQL.Append(" WHERE SubProjectNo LIKE '" + v_sProjectNo + "-%'");

                this.m_conSQL.Execute(sbSQL.ToString());

                               

                //Only process units if not the surveyor app.
                if (cSettings.IsThisTheSurveyorApp() == false)
                {

                    sbSQL.Clear();

                    sbSQL.Append("DELETE FROM cUnitsTable");
                    sbSQL.Append(" WHERE SubProjectNo LIKE '" + v_sProjectNo + "-%'");

                    this.m_conSQL.Execute(sbSQL.ToString());
              
                }

                //Delete from main project table.
                sbSQL.Clear();

                sbSQL.Append("DELETE FROM cProjectTable");
                sbSQL.Append(" WHERE ProjectNo = '" + v_sProjectNo + "'");

                this.m_conSQL.Execute(sbSQL.ToString());

                //Delete any files
                List<cProjectFilesTable> cFiles = this.FetchProjectFilesList(v_sProjectNo);
                if (cFiles.Count > 0)
                {

                    foreach (cProjectFilesTable cFile in cFiles)
                    {

                        await cSettings.DeleteSubProjectImageFolder(cFile.SubProjectNo);

                    }


                    //Delete from pictures table.
                    sbSQL.Clear();

                    sbSQL.Append("DELETE FROM cProjectFilesTable");
                    sbSQL.Append(" WHERE SubProjectNo LIKE '" + v_sProjectNo + "-%'");   

                    this.m_conSQL.Execute(sbSQL.ToString());

                }              

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - (" + v_sProjectNo + ")");

            }

        }

                /// <summary>
        /// v1.0.19 - Fetch sub-projects matching original order id.
        /// </summary>
        /// <param name="v_sSubProjectID"></param>
        /// <returns></returns>
        public List<cSubProjectResult> FetchInCompleteRemakesForSubProject(string v_sSubProjectNo)
        {
            
            StringBuilder sbSQL = new StringBuilder();
            try
            {

                string sStatuses = this.GetSettingValue("Installation_OrderComplete_RemakeStatuses");
               
                sbSQL.Append("SELECT SubProjectNo, DeliveryStreet, Mxm1002ProgressStatus, ProgressStatusName,  Mxm1002InstallStatus, InstallStatusName");
                sbSQL.Append(" FROM cProjectTable");
                sbSQL.Append(" WHERE ABPAWOriginalSubProjectID = '" + v_sSubProjectNo.Replace("'", "''") + "'");
                sbSQL.Append(" AND Mxm1002InstallStatus NOT IN (" + sStatuses + ")");
                sbSQL.Append(" ORDER BY SubProjectNo");

                return this.m_conSQL.Query<cSubProjectResult>(sbSQL.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - (" + v_sSubProjectNo + ")");

            }

        }

        /// <summary>
        /// Maintain database.
        /// </summary>
        public void MaintainDB()
        {

            try
            {
                SQLiteCommand cmd = this.m_conSQL.CreateCommand("vacuum;");               
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// v1.0.18 - Clean up, close connections
        /// </summary>
        public void CleanUp()
        {

            try
            {               
              
                if (this.m_conSQL != null)
                {
                    this.m_conSQL.Close();

                }

                if (this.m_conSQL_Settings != null)
                {
                    this.m_conSQL_Settings.Close();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }


        }
       
    }
}
