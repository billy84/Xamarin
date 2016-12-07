using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Classes;
using Anglian.Models;
using Anglian.Service;
using SQLite;
using Xamarin.Forms;

namespace Anglian.Engine
{
    public class DataAccess
    {
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
        public DataAccess()
        {

            try
            {

                //v1.0.8 - Set the flag indicating if this is the surveyor app.
                m_bSurveyorApp = DependencyService.Get<ISettings>().IsThisTheSurveyorApp();

                //v1.0.11 - Check settings database exists.
                CheckSettingsDB();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }
        public List<cProjectNo> GetProjectNos()
        {

            List<cProjectNo> sProjNos = new List<cProjectNo>();

            try
            {

                String sSQL = "SELECT ProjectNo,ProjectName FROM cProjectTable GROUP BY ProjectNo,ProjectName ORDER BY ProjectNo,ProjectName";
                sProjNos = m_conSQL.Query<cProjectNo>(sSQL);

                return sProjNos;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }


        }
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
                string sFullPath = DependencyService.Get<IDataAccess>().GetFullPath(sDBFileName);
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
        /// Fetch sub project list with uploads pending.
        /// </summary>
        /// <returns></returns>
        public List<SubProjectSync> FetchSubProjectsWithUploads()
        {

            StringBuilder sbSQL = new StringBuilder();

            try
            {

                bool bSurveyorApp = DependencyService.Get<ISettings>().IsThisTheSurveyorApp();

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


                return this.m_conSQL.Query<SubProjectSync>(sbSQL.ToString());

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
        /// Fetch sub project update date and time values for project
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public List<SubProjectSyncUpdateValues> FetchSubProjectUpdateDateTime(string v_sProjectNo, int v_iLimit)
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

                return this.m_conSQL.Query<SubProjectSyncUpdateValues>(sbSQL.ToString());


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " ProjectNo(" + v_sProjectNo + ")");

            }

        }
        public List<SurveyInputResult> SearchSurveyInput(
            string v_sProjectNo,
            string v_sStreet,
            string v_sPostcode,
            int v_iInstallStatus,
            int v_iProgressStatus,
            DateTime? v_dSurveyDate,
            string v_sDateCompare,
            string v_sSurveyedStatus,
            string v_sSurveyor,
            string v_sSurveyedOnSite,
            bool v_bSyncOnly,
            bool v_bShowAllStatuses,
            bool v_bShowAllProgressStatuses,
            string v_sSubProjectNoFilter,
            int v_iInstallStatus_AwaitingSurvey,
            int v_iInstallStatus_Cancel,
            bool v_bBooked = false,
            string v_sInstallStatus_Filter = Settings.p_sInstallStatusFilter_EqualTo,
            string v_sOrderComplete_Filter = Settings.p_sAnyStatus,
            HSFilters v_iHSIncomplete = HSFilters.Any)
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

                    if (v_iInstallStatus != Settings.p_iInstallStatus_Installing)
                    {

                        if (v_sInstallStatus_Filter == Settings.p_sInstallStatusFilter_EqualTo)
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

                            if (v_sInstallStatus_Filter == Settings.p_sInstallStatusFilter_EqualTo)
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
                    if (v_sDateCompare == Settings.p_sDateCompare_GreaterThan || v_sDateCompare == Settings.p_sDateCompare_EqualTo)
                    {
                        //v1.0.4 - Only use EndDateTime, sbWhere.Append(" PT.StartDateTime >= '" + dStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        sbWhere.Append(" PT.EndDateTime >= '" + dStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                    }
                    if (v_sDateCompare == Settings.p_sDateCompare_LessThan || v_sDateCompare == Settings.p_sDateCompare_EqualTo)
                    {

                        if (v_sDateCompare == Settings.p_sDateCompare_EqualTo)
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
                if (v_sSurveyedStatus.Equals(Settings.p_sSurveyedStatus_NotSurveyed) == true)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" PT.MXM1002TrfDate ISNULL");

                    sbWhere.Append(" AND (PT.Mxm1002InstallStatus = " + v_iInstallStatus_AwaitingSurvey + " OR PT.Mxm1002InstallStatus = " + v_iInstallStatus_Cancel + ")");

                }
                else if (v_sSurveyedStatus.Equals(Settings.p_sSurveyedStatus_SurveyedOnSite) == true)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" PT.MXM1002TrfDate IS NOT NULL");

                    sbWhere.Append(" AND (PT.Mxm1002InstallStatus = " + v_iInstallStatus_AwaitingSurvey + " OR PT.Mxm1002InstallStatus = " + v_iInstallStatus_Cancel + ")");


                }
                else if (v_sSurveyedStatus.Equals(Settings.p_sSurveyedStatus_SurveyedTrans) == true)
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
                if (v_sSurveyedOnSite.Equals(Settings.p_sInputStatus_Successful) == true)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" PT.MXM1002TrfDate IS NOT NULL");

                }
                else if (v_sSurveyedOnSite.Equals(Settings.p_sInputStatus_Pending) == true)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" PT.MXM1002TrfDate ISNULL AND PT.MxmConfirmedAppointmentIndicator='1'");


                }
                else if (v_sSurveyedOnSite.Equals(Settings.p_sInputStatus_Failed) == true)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" (PT.MxmConfirmedAppointmentIndicator ISNULL OR  PT.MxmConfirmedAppointmentIndicator='0')");
                    sbWhere.Append(" AND (PT.StartDateTime IS NOT NULL AND PT.EndDateTime IS NOT NULL)");

                }
                else if (v_sSurveyedOnSite.Equals(Settings.p_sInputStatus_NotPending) == true) //v1.0.1
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
                if (v_sOrderComplete_Filter != Settings.p_sAnyStatus)
                {

                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    if (v_sOrderComplete_Filter == Settings.p_sConfirmedStatus_No)
                    {

                        sbWhere.Append(" (ABPAWOrderCompletedDate = '" +
                                       Settings.p_dDefaultDBDate.ToString("yyyy-MM-dd HH:mm:ss") +
                                       "' OR ABPAWOrderCompletedDate IS NULL) ");

                    }
                    else if (v_sOrderComplete_Filter == Settings.p_sConfirmedStatus_Yes)
                    {

                        sbWhere.Append(" (ABPAWOrderCompletedDate > '" + Settings.p_dDefaultDBDate.ToString("yyyy-MM-dd HH:mm:ss") + "')");

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

                bool bSurveyorApp = DependencyService.Get<ISettings>().IsThisTheSurveyorApp();

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

                    sbSQL.Append(" LEFT JOIN (SELECT COUNT(*) AS TotalUnitsInstalled,SubProjectNo FROM cUnitsTable WHERE InstalledStatus = " + Settings.p_iUnits_InstalledStatus.ToString() + " GROUP BY SubProjectNo) AS UTI");
                    sbSQL.Append(" ON PT.SubProjectNo = UTI.SubProjectNo");

                }

                //Only add where clause if some criteria has been set.
                if (sbWhere.Length > 0)
                {
                    sbSQL.Append(" WHERE " + sbWhere.ToString());
                }

                //Add the Order By.
                sbSQL.Append(" ORDER BY PT.EndDateTime ASC, PT.MXM1002SequenceNr ASC, PT.ProjectNo ASC");

                List<SurveyInputResult> cResults = this.m_conSQL.Query<SurveyInputResult>(sbSQL.ToString());

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
        /// Get list of surveyors from local DB
        /// </summary>
        /// <returns></returns>
        public List<Surveyor> GetSurveyors()
        {

            List<Surveyor> sSurveyors = new List<Surveyor>();
            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("SELECT SurveyorProfile,SurveyorName FROM cProjectTable");
                sbSQL.Append(" WHERE SurveyorProfile <> ''");
                sbSQL.Append(" GROUP BY SurveyorProfile,SurveyorName");
                sbSQL.Append(" ORDER BY SurveyorProfile,SurveyorName");

                sSurveyors = this.m_conSQL.Query<Surveyor>(sbSQL.ToString());

                return sSurveyors;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SQL(" + sbSQL.ToString() + ")");

            }
        }
        /// <summary>
        /// v1.0.1 - Fetch projects for syncing
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<ProjectSearch> FetchProjectsForSync()
        {

            try
            {

                StringBuilder sbSQL = new StringBuilder();

                sbSQL.Append("SELECT ProjectNo,ProjectName,COUNT(*) AS SubProjectQty");
                sbSQL.Append(" FROM cProjectTable");
                sbSQL.Append(" GROUP BY ProjectNo,ProjectName");
                sbSQL.Append(" ORDER BY ProjectNo,ProjectName");

                return new ObservableCollection<ProjectSearch>(this.m_conSQL.Query<ProjectSearch>(sbSQL.ToString()));

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
                sbSQL.Append(" WHERE SubProjectNo = '" + v_sSubProjectNo.Replace("'", "''") + "'");
                sbSQL.Append(" ORDER BY UnitNo");

                return this.m_conSQL.Query<cUnitsTable>(sbSQL.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// v1.0.1 - Fetch projects to sync
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<ProjectSearch> FetchProjectsToSync()
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

                return new ObservableCollection<ProjectSearch>(this.m_conSQL.Query<ProjectSearch>(sbSQL.ToString()));

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
        /// v1.0.1 - Update notes records that have just been uploaded with new RECID from AX.
        /// </summary>
        /// <param name="v_nkvValues"></param>
        /// <returns></returns>
        public bool UpdateNotesWithRecID(ObservableCollection<RealtimeNoteKeyValues> v_nkvValues)
        {

            StringBuilder sbSQL = new StringBuilder();
            int iSQLRtn = -1;
            try
            {

                if (v_nkvValues != null)
                {

                    foreach (RealtimeNoteKeyValues nkvValue in v_nkvValues)
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
        public List<SubProjectSync> FetchSubProjectsWithNewNotes()
        {

            StringBuilder sbSQL = new StringBuilder();

            try
            {

                sbSQL.Append("SELECT SubProjectNo,COUNT(*) AS UpdateQty ");

                sbSQL.Append(" FROM cProjectNotesTable");

                sbSQL.Append(" WHERE AXRecID = -1");

                sbSQL.Append(" GROUP BY SubProjectNo");

                return this.m_conSQL.Query<SubProjectSync>(sbSQL.ToString());


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
        /// Searching for projects on the set survey dates screen.
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <param name="v_sStreet"></param>
        /// <param name="v_iInstallStatus"></param>
        /// <param name="v_dSurveyDate"></param>
        /// <param name="v_sSurveyed"></param>
        /// <param name="v_sConfirmed"></param>
        /// <returns></returns>
        public List<SurveyDatesResult> SearchSurveyDates(string v_sProjectNo, string v_sStreet, string v_sPostcode, int v_iInstallStatus, int v_iProgressStatus, DateTime? v_dSurveyDate, string v_sDateCompare, string v_sSurveyedStatus, int v_iConfirmed, bool v_bSyncOnly, bool v_bShowAllStatuses, bool v_bShowAllProgressStatuses, string v_sSubProjectNoFilter, int v_iInstallStatus_AwaitingSurvey, int v_iInstallStatus_Cancel, string v_sInstallStatus_Filter = Settings.p_sInstallStatusFilter_EqualTo)
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
                        sbWhere.Append(" LOWER(PT.SubProjectNo) " + FormatSQLCritieria(v_sSubProjectNoFilter.ToLower()));
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

                    sbWhere.Append(" LOWER(PT.DeliveryStreet) " + FormatSQLCritieria(v_sStreet.ToLower()));


                }

                //Add post code if specified
                if (v_sPostcode.Length > 0)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" LOWER(PT.DlvZipCode) " + FormatSQLCritieria(v_sPostcode.ToLower()));

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

                    if (v_iInstallStatus != Settings.p_iInstallStatus_Installing)
                    {

                        if (v_sInstallStatus_Filter == Settings.p_sInstallStatusFilter_EqualTo)
                        {
                            sbWhere.Append(" PT.Mxm1002InstallStatus = " + v_iInstallStatus.ToString());

                        }
                        else if (v_sInstallStatus_Filter == Settings.p_sInstallStatusFilter_NotEqualTo)
                        {
                            sbWhere.Append(" PT.Mxm1002InstallStatus <> " + v_iInstallStatus.ToString());
                        }


                    }
                    else
                    {

                        //v1.0.10 - For installing status, is made up of other statuses.
                        string[] sStatuses = GetSettingValue("Installation_Installing_InstallStatuses").Split(',');
                        if (sStatuses != null)
                        {

                            //Check the install status filter to decide
                            if (v_sInstallStatus_Filter == Settings.p_sInstallStatusFilter_EqualTo)
                            {
                                sbWhere.Append(" PT.Mxm1002InstallStatus IN (");
                            }
                            else if (v_sInstallStatus_Filter == Settings.p_sInstallStatusFilter_NotEqualTo)
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
                    if (v_sDateCompare == Settings.p_sDateCompare_GreaterThan || v_sDateCompare == Settings.p_sDateCompare_EqualTo)
                    {
                        //v1.0.4 - Only use EndDateTime, sbWhere.Append(" PT.StartDateTime >= '" + dStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        sbWhere.Append(" PT.EndDateTime >= '" + dStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                    }
                    if (v_sDateCompare == Settings.p_sDateCompare_LessThan || v_sDateCompare == Settings.p_sDateCompare_EqualTo)
                    {

                        if (v_sDateCompare == Settings.p_sDateCompare_EqualTo)
                        {
                            sbWhere.Append(" AND ");
                        }

                        //v1.0.4 - Only use EndDateTime, sbWhere.Append(" PT.StartDateTime <= '" + dEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        sbWhere.Append(" PT.EndDateTime <= '" + dEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                    }

                    //v1.0.16 - Only check if running surveyor app mode.
                    if (DependencyService.Get<ISettings>().IsThisTheSurveyorApp() == true)
                    {
                        //sbWhere.Append(" AND PT.MXM1002TrfDate ISNULL");
                    }


                    sbWhere.Append(" )");
                }


                //Add surveyed status.
                if (v_sSurveyedStatus.Equals(Settings.p_sSurveyedStatus_NotSurveyed) == true)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" PT.MXM1002TrfDate ISNULL");


                    sbWhere.Append(" AND (PT.Mxm1002InstallStatus = " + v_iInstallStatus_AwaitingSurvey + " OR PT.Mxm1002InstallStatus = " + v_iInstallStatus_Cancel + ")");

                }
                else if (v_sSurveyedStatus.Equals(Settings.p_sSurveyedStatus_SurveyedOnSite) == true)
                {

                    //If criteria already specified add an AND
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(" AND ");
                    }

                    sbWhere.Append(" PT.MXM1002TrfDate IS NOT NULL");

                    sbWhere.Append(" AND (PT.Mxm1002InstallStatus = " + v_iInstallStatus_AwaitingSurvey + " OR PT.Mxm1002InstallStatus = " + v_iInstallStatus_Cancel + ")");


                }
                else if (v_sSurveyedStatus.Equals(Settings.p_sSurveyedStatus_SurveyedTrans) == true)
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
                if (v_iConfirmed == (int)Settings.YesNoBaseEnum.Yes || v_iConfirmed == (int)Settings.YesNoBaseEnum.No)
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

                    sSettingValue = GetSettingValue("Search_ExcludeProjectStatus");
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

                    sSettingValue = GetSettingValue("Search_ExcludeProgressStatuses");
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

                bool bSurveyorApp = DependencyService.Get<ISettings>().IsThisTheSurveyorApp();


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

                    sbSQL.Append(" LEFT JOIN (SELECT COUNT(*) AS TotalUnitsInstalled,SubProjectNo FROM cUnitsTable WHERE InstalledStatus = " + Settings.p_iUnits_InstalledStatus.ToString() + " GROUP BY SubProjectNo) AS UTI");
                    sbSQL.Append(" ON PT.SubProjectNo = UTI.SubProjectNo");

                }

                //Only add where clause if some criteria has been set.
                if (sbWhere.Length > 0)
                {
                    sbSQL.Append(" WHERE " + sbWhere.ToString());
                }

                //Add the Order By.
                sbSQL.Append(" ORDER BY PT.EndDateTime ASC, PT.MXM1002SequenceNr ASC, PT.ProjectNo ASC");

                List<SurveyDatesResult> cResults = m_conSQL.Query<SurveyDatesResult>(sbSQL.ToString());

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
        /// v1.0.1 - Get setting value from ax setting table.
        /// </summary>
        /// <param name="v_sSettingName"></param>
        /// <returns></returns>
        public string GetSettingValue(string v_sSettingName)
        {

            string sRtnValue = string.Empty;
            try
            {

                var oResults = (from oCols in m_conSQL.Table<cAXSettingsTable>()
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
        /// Return application settings
        /// </summary>
        /// <returns></returns>
        public cAppSettingsTable ReturnSettings()
        {
            cAppSettingsTable rtnTable = null;
            try
            {
                var oResults = from oCols in m_conSQL_Settings.Table<cAppSettingsTable>() select oCols;
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
        /// Should I check for base enums
        /// </summary>
        /// <returns></returns>
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
                //Main.ReportError(ex, Main.GetCallerMethodName(), string.Empty);
                return false;

            }

        }
        /// <summary>
        /// v1.0.1 - Get AX settings update values
        /// </summary>
        /// <returns></returns>
        public List<SettingDetails> GetSettingsUpdates()
        {

            List<SettingDetails> sdSettings = new List<SettingDetails>();
            SettingDetails sdSetting;

            try
            {

                var oResults = (from oCols in this.m_conSQL.Table<cAXSettingsTable>()
                                select oCols);


                foreach (cAXSettingsTable stTable in oResults)
                {

                    sdSetting = new SettingDetails();
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
        /// Get the base enum update values from the table.
        /// </summary>
        /// <returns></returns>
        public List<BaseEnumField> GetBaseEnumUpdates()
        {

            List<BaseEnumField> beFields = new List<BaseEnumField>();
            BaseEnumField beField;

            try
            {

                var oResults = (from oCols in this.m_conSQL.Table<cBaseEnumUpdateTable>()
                                select oCols);


                foreach (cBaseEnumUpdateTable utTable in oResults)
                {

                    beField = new BaseEnumField();
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
        public async Task ProcessUpdatedBaseEnums(List<BaseEnumField> v_beFields)
        {

            try
            {

                //Loop through returned base enums and update.
                foreach (BaseEnumField beField in v_beFields)
                {

                    bool bCleared = ClearExistingBaseEnums(beField.TableName, beField.FieldName);
                    if (bCleared == false)
                    {
                        return;
                    }

                    bool bEnumsAdded = AddNewBaseEnums(beField);
                    if (bEnumsAdded == false)
                    {
                        return;
                    }


                    bool bEnumUpdated = UpdateBaseEnumTable(beField);
                    if (bEnumUpdated == false)
                    {
                        return;
                    }

                }


                cAppSettingsTable cSetting = this.ReturnSettings();
                cSetting.LastBaseEnumCheckDateTime = DateTime.Now;
                await SaveSettings(cSetting);

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

                m_conSQL.Execute(sbSQL.ToString());

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - PARAM(TableName=" + v_sTableName + ",FieldName=" + v_sFieldName + ")");

            }
        }
        /// <summary>
        /// Add new base enums
        /// </summary>
        /// <param name="beField"></param>
        /// <returns></returns>
        private bool AddNewBaseEnums(BaseEnumField beField)
        {

            cBaseEnumsTable beUpdate = null;

            try
            {

                foreach (BaseEnumValue beValue in beField.BaseEnums)
                {
                    beUpdate = new cBaseEnumsTable();
                    beUpdate.TableName = beField.TableName;
                    beUpdate.FieldName = beField.FieldName;
                    beUpdate.EnumName = beValue.BaseName;
                    beUpdate.EnumValue = beValue.BaseValue;

                    m_conSQL.Insert(beUpdate);

                }

                return true;

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
        private bool UpdateBaseEnumTable(BaseEnumField v_beField)
        {

            try
            {


                cBaseEnumUpdateTable cUpdate = null;
                int iCount = 0;


                var oResults = (from oCols in m_conSQL.Table<cBaseEnumUpdateTable>()
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
                    iCount = m_conSQL.Update(cUpdate);
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
        /// Save settings to DB
        /// </summary>
        /// <param name="v_cSettings"></param>
        /// <returns></returns>
        public async Task<bool> SaveSettings(cAppSettingsTable v_cSettings)
        {

            try
            {

                v_cSettings.UserProfile = await DependencyService.Get<ISettings>().GetUserName();
                if (v_cSettings.UserProfile == string.Empty)
                    v_cSettings.UserProfile = Session.CurrentUserName;
                v_cSettings.UsersFullName = await DependencyService.Get<ISettings>().GetUserDisplayName();
                if (v_cSettings.UsersFullName == string.Empty)
                    v_cSettings.UsersFullName = Session.CurrentUserName;


                if (v_cSettings.IDKey == 0) //If IDKey is 0 then we need to insert a new one.
                {
                    m_conSQL_Settings.Insert(v_cSettings);

                }
                else //Update
                {
                    m_conSQL_Settings.Update(v_cSettings);

                }

                return true;

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
        public async Task ProcessUpdatedSettings(List<SettingDetails> v_dsSettings)
        {

            try
            {

                //Loop through returned settings and update.
                foreach (SettingDetails sdSetting in v_dsSettings)
                {


                    bool bSettingUpdated = UpdateSettingsTable(sdSetting);
                    if (bSettingUpdated == false)
                    {
                        return;
                    }

                }


                cAppSettingsTable cSetting = ReturnSettings();
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
        private bool UpdateSettingsTable(SettingDetails v_sdSetting)
        {

            try
            {


                cAXSettingsTable cUpdate = null;
                int iCount = 0;


                var oResults = (from oCols in m_conSQL.Table<cAXSettingsTable>()
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

                    iCount = m_conSQL.Update(cUpdate);
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
        /// v1.0.8 - Update Failed Survey Reason table.
        /// </summary>
        /// <returns></returns>
        public bool UpdateFailedSurveyReasonsTable(ObservableCollection<SurveyFailedReason> v_sfrReasons)
        {

            try
            {

                if (v_sfrReasons != null)
                {

                    //Clear down table first.
                    ClearSurveyFailedReasonsTable();

                    cFailedSurveyReasonsTable cAddReason;

                    List<cFailedSurveyReasonsTable> lAddReasons = new List<cFailedSurveyReasonsTable>();

                    foreach (SurveyFailedReason cReason in v_sfrReasons)
                    {
                        cAddReason = new cFailedSurveyReasonsTable();
                        cAddReason.Description = cReason.sReason;
                        cAddReason.DisplayOrder = cReason.iDisplayOrder;
                        cAddReason.MandatoryNote = cReason.bMandatoryNote;
                        cAddReason.ProgressStatus = cReason.iProgressStatus;

                        lAddReasons.Add(cAddReason);

                    }

                    m_conSQL.InsertAll(lAddReasons);

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
                if (AreWeRunningInLive() == false)
                {
                    sDBFileName = "surveyordbmain_test.sqlite";

                    if (this.m_bSurveyorApp == false)
                    {
                        sDBFileName = "installerdbmain_test.sqlite";

                    }

                }

                //Create connection object.
                string sFullPath = DependencyService.Get<IDataAccess>().GetFullPath(sDBFileName);
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

                List<Status> oStatuses = m_conSQL_Settings.Query<Status>(sbSQL.ToString());

                if (oStatuses != null)
                {
                    foreach (Status oStatus in oStatuses)
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
        /// v1.0.21 - Clear survey failed reasons table.
        /// </summary>
        /// <returns></returns>
        public bool ClearSurveyFailedReasonsTable()
        {

            StringBuilder sbSQL = new StringBuilder();
            try
            {

                sbSQL.Append("DELETE FROM cFailedSurveyReasonsTable");

                m_conSQL.Execute(sbSQL.ToString());

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

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

                lProjects = m_conSQL.Query<cProjectTable>(sbSQL.ToString());

                return lProjects;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - USERNAME(" + v_sUserName + ")");

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
        public DataAccess.SaveSubProjectDataResult SaveSubProjectData(string v_sProjectNo, string v_sProjectName, SubProjectData v_spData)
        {

            bool bDoInsert = false;
            bool bNotesSaved = false;
            DataAccess.SaveSubProjectDataResult rResult = new SaveSubProjectDataResult();
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
                    cProject.ProjectName = Settings.ReturnString(v_sProjectName);
                    cProject.SubProjectNo = v_spData.ProjId;
                    cProject.SubProjectName = Settings.ReturnString(v_spData.Name);
                    cProject.DateLastSynced = DateTime.Now;

                }


                //Update remaining fields.
                cProject.AlternativeContactMobileNo = Settings.ReturnString(v_spData.MXMAlternativeContactMobileNo);
                cProject.AlternativeContactName = Settings.ReturnString(v_spData.MXMAlternativeContactName);
                cProject.AlternativeContactTelNo = Settings.ReturnString(v_spData.MXMAlternativeContactTelNo);
                cProject.ABPAXAsbestosPresumed = v_spData.ABPAXASBESTOSPRESUMED;
                cProject.ABPAXInstallationType = v_spData.ABPAXINSTALLATIONTYPE;

                cProject.MxmConfirmedAppointmentIndicator = v_spData.MXMConfirmedAppointmentIndicator;
                cProject.MxmContactBySMS = v_spData.MXMContactBySMS;
                cProject.ABPAXPermanentGasVent = v_spData.ABPAXPERMANENTGASVENT;
                cProject.DeliveryCity = Settings.ReturnString(v_spData.DeliveryCity);
                cProject.DeliveryStreet = Settings.ReturnString(v_spData.DeliveryStreet);
                cProject.DisabledAdaptionsRequired = v_spData.MXMDisabledAdaptionsRequired;
                cProject.DlvState = Settings.ReturnString(v_spData.DlvState);
                cProject.DlvZipCode = Settings.ReturnString(v_spData.DlvZipCode);
                cProject.MxmDoorChoiceFormReceived = v_spData.MXMDoorChoiceFormReceived;
                cProject.EndDateTime = ConvertDateTimeToNullable(v_spData.EndDateTime.Value);
                cProject.ABPAXFloorLevel = v_spData.ABPAXFLOORLEVEL;
                cProject.InstallStatusName = this.GetEnumValueName("ProjTable", "Mxm1002InstallStatus", Convert.ToInt32(v_spData.Mxm1002InstallStatus));
                cProject.ABPAXStructuralFaults = v_spData.ABPAXSTRUCTURALFAULTS;
                cProject.ModifiedDateTime = v_spData.MODIFIEDDATETIME;
                cProject.Mxm1002InstallStatus = v_spData.Mxm1002InstallStatus;
                cProject.Mxm1002ProgressStatus = v_spData.Mxm1002ProgressStatus;
                cProject.MXM1002SequenceNr = v_spData.MXM1002SequenceNr;
                cProject.MXM1002TrfDate = ConvertDateTimeToNullable(v_spData.MXM1002TrfDate.Value);
                cProject.MxmProjDescription = Settings.ReturnString(v_spData.MxmProjDescription);
                cProject.MxmNextDaySMS = v_spData.MXMNextDaySMS;
                cProject.ProgressStatusName = this.GetEnumValueName("ProjTable", "Mxm1002ProgressStatus", Convert.ToInt32(v_spData.Mxm1002ProgressStatus));
                cProject.PropertyType = v_spData.MXMPropertyType;
                cProject.ResidentMobileNo = Settings.ReturnString(v_spData.MXMResidentMobileNo);
                cProject.ResidentName = Settings.ReturnString(v_spData.MXMResidentName);
                cProject.ResidentTelNo = Settings.ReturnString(v_spData.MXMTelephoneNo);
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
                cProject.SpecialResidentNote = Settings.ReturnString(v_spData.MXMSpecialResidentNote);
                cProject.StartDateTime = ConvertDateTimeToNullable(v_spData.StartDateTime.Value);
                cProject.MxmSurveyletterRequired = v_spData.MXMSurveyletterRequired;
                cProject.SurveyLetterSentDate01 = v_spData.MXMSurveyLetterSentDate01;
                cProject.SurveyLetterSentDate02 = v_spData.MXMSurveyLetterSentDate02;
                cProject.SurveyLetterSentDate03 = v_spData.MXMSurveyLetterSentDate03;
                cProject.SurveyorName = Settings.ReturnString(v_spData.MXMSurveyorName);
                cProject.SurveyorPCTag = Settings.ReturnString(v_spData.MXMSurveyorPCTag);
                cProject.SurveyorProfile = Settings.ReturnString(v_spData.MXMSurveyorProfile);
                cProject.SMMActivities_MODIFIEDDATETIME = v_spData.SMMActivities_MODIFIEDDATETIME;

                //v1.0.10 - installation specific fields.
                cProject.ABPAXINSTALLATIONTEAM = Settings.ReturnString(v_spData.ABPAXINSTALLATIONTEAM);
                cProject.ABPAXInstallLetterSentDate01 = v_spData.ABPAXINSTALLLETTERSENTDATE01;
                cProject.ABPAXInstallLetterSentDate02 = v_spData.ABPAXINSTALLLETTERSENTDATE02;
                cProject.ABPAXInstallLetterSentDate03 = v_spData.ABPAXINSTALLLETTERSENTDATE03;
                cProject.ABPAXInstallletterRequired = v_spData.ABPAXINSTALLLETTERREQUIRED;
                cProject.ABPAXInstallSMSSent = v_spData.ABPAXINSTALLSMSSENT;
                cProject.ABPAXInstallNextDaySMS = v_spData.ABPAXINSTALLNEXTDAYSMS;

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
                bNotesSaved = SaveSubProjectNotes(v_spData.Notes);

                if (DependencyService.Get<ISettings>().IsThisTheSurveyorApp() == false)
                {

                    //v1.0.10 - Save sub project units.
                    SaveSubProjectUnits(v_spData.ProjId, v_spData.Name, v_spData.Units);

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
                await DependencyService.Get<ISettings>().DeleteSubProjectImageFolder(v_sSubProjectNo);

                //Delete sub project notes.
                this.DeleteSubProjectNotes(v_sSubProjectNo);

                //Only process units if not the surveyor app.
                if (DependencyService.Get<ISettings>().IsThisTheSurveyorApp() == false)
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
        /// Delete project from device.
        /// </summary>
        /// <param name="v_sProjectNo"></param>
        /// <returns></returns>
        public async Task<bool> DeleteProjectFromDevice(string v_sProjectNo)
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
        /// Save sub project file
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <param name="v_sFileName"></param>
        /// <param name="v_sComments"></param>
        /// <param name="v_dModDate"></param>
        /// <returns></returns>
        public bool SaveSubProjectFile(
            string v_sSubProjectNo, 
            string v_sFileName, 
            string v_sComments, 
            DateTime v_dModDate, 
            bool v_bIsNew_InsertOnly)
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
        /// v1.0.1 - Save sub project units.
        /// </summary>
        /// <param name="v_ndNotes"></param>
        /// <returns></returns>
        public bool SaveSubProjectUnits(string v_sSubProjectNo, string v_sSubProjectName, ObservableCollection<UnitDetails> v_udUnits)
        {

            cUnitsTable udUnit;

            try
            {

                if (v_udUnits != null)
                {

                    bool bInsert = false;
                    foreach (UnitDetails udDetail in v_udUnits)
                    {

                        bInsert = false;
                        udUnit = FetchSubProjectUnit(v_sSubProjectNo, udDetail.iUNITNUMBER);
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
        /// v1.0.1 - Save sub project notes.
        /// </summary>
        /// <param name="v_ndNotes"></param>
        /// <returns></returns>
        public bool SaveSubProjectNotes(ObservableCollection<NoteDetails> v_ndNotes)
        {

            cProjectNotesTable pnNote;
            try
            {

                if (v_ndNotes != null)
                {

                    foreach (NoteDetails ndDetail in v_ndNotes)
                    {

                        pnNote = new cProjectNotesTable();
                        pnNote.AXRecID = ndDetail.AXRecID;
                        pnNote.InputDateTime = ndDetail.InputDate;
                        pnNote.NoteText = ndDetail.NoteText;
                        pnNote.SubProjectNo = ndDetail.ProjectNo;
                        pnNote.UserName = ndDetail.UserName;
                        pnNote.UserProfile = ndDetail.UserProfile;
                        pnNote.NoteType = ndDetail.NoteType;

                        SaveSubProjectNote(pnNote);

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
    }
}
