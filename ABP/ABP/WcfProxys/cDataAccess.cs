using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.IO;
using ABP.Interfaces;
using Xamarin.Forms;
using ABP.TableModels;
using ABP.Models;

namespace ABP.WcfProxys
{
    public class cDataAccess
    {
        private SQLiteConnection m_conSQL = null;
        private SQLiteConnection m_conSQL_Settings = null;
        private bool m_bSurveyorApp = false;
        public struct SaveSubProjectDataResult
        {
            public bool bSavedOK;
            public bool bProjectAdded;
        }
        public enum HSFilters
        {
            Any = 0,
            InComplete = 1,
            Complete = 2
        }
        public cDataAccess()
        {
            try
            {
                this.m_bSurveyorApp = cSettings.IsThisTheSurveyorApp();
                this.CheckSettingDB();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void CheckSettingDB()
        {
            try
            {
                this.m_conSQL_Settings = DependencyService.Get<IDatabaseConnection>().SettingDbConnection();
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
        public void CheckDB()
        {
            try
            {
                this.m_conSQL = DependencyService.Get<IDatabaseConnection>().MainDbConnection();
                if (this.AreWeRunningInLive() == false)
                {
                    this.m_conSQL = DependencyService.Get<IDatabaseConnection>().TestDbConnection();
                }
                if (this.m_conSQL.TableMappings.Count() == 0)
                {
                    this.m_conSQL.CreateTable<cProjectTable>();
                    this.m_conSQL.CreateTable<cProjectFilesTable>();
                    this.m_conSQL.CreateTable<cUpdatesTable>();
                    this.m_conSQL.CreateTable<cBaseEnumsTable>();
                    this.m_conSQL.CreateTable<cBaseEnumUpdateTable>();
                    this.m_conSQL.CreateTable<cAXSettingsTable>();
                    this.m_conSQL.CreateTable<cProjectNotesTable>();
                    this.m_conSQL.CreateTable<cFailedSurveyReasonsTable>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
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

        public List<cProjectNo> GetProjectNos()
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
        public List<cSurveyDatesResult> SearchSurveyDates(
            string v_sProjectNo, 
            string v_sStreet, 
            string v_sPostcode, 
            int v_iInstallStatus, 
            int v_iProgressStatus, 
            DateTime? v_dSurveyDate, 
            string v_sDateCompare, 
            string v_sSurveyedStatus, 
            int v_iConfirmed, 
            bool v_bSyncOnly, 
            bool v_bShowAllStatuses, 
            bool v_bShowAllProgressStatuses, 
            string v_sSubProjectNoFilter, 
            int v_iInstallStatus_AwaitingSurvey, 
            int v_iInstallStatus_Cancel, 
            string v_sInstallStatus_Filter = cSettings.p_sInstallStatusFilter_EqualTo)
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
                    bool flag = false;

                    //Add SQL comparison criteria.
                    if (v_sDateCompare == cSettings.p_sDateCompare_GreaterThan || v_sDateCompare == cSettings.p_sDateCompare_EqualTo)
                    {
                        //v1.0.4 - Only use EndDateTime, sbWhere.Append(" PT.StartDateTime >= '" + dStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        sbWhere.Append(" PT.EndDateTime >= '" + dStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        flag = true;

                    }
                    if (v_sDateCompare == cSettings.p_sDateCompare_LessThan || v_sDateCompare == cSettings.p_sDateCompare_EqualTo)
                    {

                        if (v_sDateCompare == cSettings.p_sDateCompare_EqualTo)
                        {
                            if (flag == true)
                                sbWhere.Append(" AND ");
                        }

                        //v1.0.4 - Only use EndDateTime, sbWhere.Append(" PT.StartDateTime <= '" + dEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        sbWhere.Append(" PT.EndDateTime <= '" + dEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        flag = true;

                    }

                    //v1.0.16 - Only check if running surveyor app mode.
                    if (cSettings.IsThisTheSurveyorApp() == true)
                    {
                        if (flag == true)
                            sbWhere.Append(" AND PT.MXM1002TrfDate ISNULL");
                        else
                            sbWhere.Append(" PT.MXM1002TrfDate ISNULL");
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
        public List<ServiceExt.BaseEnumField> GetBaseEnumUpdates()
        {

            List<ServiceExt.BaseEnumField> beFields = new List<ServiceExt.BaseEnumField>();
            ServiceExt.BaseEnumField beField;

            try
            {

                var oResults = (from oCols in this.m_conSQL.Table<cBaseEnumUpdateTable>()
                                select oCols);


                foreach (cBaseEnumUpdateTable utTable in oResults)
                {

                    beField = new ServiceExt.BaseEnumField();
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
        public void ProcessUpdatedBaseEnums(List<ServiceExt.BaseEnumField> v_beFields)
        {

            try
            {

                //Loop through returned base enums and update.
                foreach (ServiceExt.BaseEnumField beField in v_beFields)
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
                SaveSettings(cSetting);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }
        private bool UpdateBaseEnumTable(ServiceExt.BaseEnumField v_beField)
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
        private bool AddNewBaseEnums(ServiceExt.BaseEnumField beField)
        {

            cBaseEnumsTable beUpdate = null;

            try
            {

                foreach (ServiceExt.BaseEnumValue beValue in beField.BaseEnums)
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
        public bool SaveSettings(cAppSettingsTable v_cSettings)
        {

            try
            {

                v_cSettings.UserProfile = WcfLogin.m_instance.LoggedUserName;
                v_cSettings.UsersFullName = WcfLogin.m_instance.LoggedUserName;

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
    }

}
