using System;
using System.Collections.ObjectModel;
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
        public List<ServiceExt.SettingDetails> GetSettingsUpdates()
        {

            List<ServiceExt.SettingDetails> sdSettings = new List<ServiceExt.SettingDetails>();
            ServiceExt.SettingDetails sdSetting;

            try
            {

                var oResults = (from oCols in this.m_conSQL.Table<cAXSettingsTable>()
                                select oCols);


                foreach (cAXSettingsTable stTable in oResults)
                {

                    sdSetting = new ServiceExt.SettingDetails();
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
        public void ProcessUpdatedSettings(List<ServiceExt.SettingDetails> v_dsSettings)
        {

            try
            {

                //Loop through returned settings and update.
                foreach (ServiceExt.SettingDetails sdSetting in v_dsSettings)
                {


                    bool bSettingUpdated = UpdateSettingsTable(sdSetting);
                    if (bSettingUpdated == false)
                    {
                        return;
                    }

                }


                cAppSettingsTable cSetting = this.ReturnSettings();
                cSetting.LastSettingsCheckDateTime = DateTime.Now;
                SaveSettings(cSetting);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }
        private bool UpdateSettingsTable(ServiceExt.SettingDetails v_sdSetting)
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
        public bool UpdateFailedSurveyReasonsTable(ObservableCollection<ServiceExt.SurveyFailedReason> v_sfrReasons)
        {

            try
            {

                if (v_sfrReasons != null)
                {

                    //Clear down table first.
                    ClearSurveyFailedReasonsTable();

                    cFailedSurveyReasonsTable cAddReason;

                    List<cFailedSurveyReasonsTable> lAddReasons = new List<cFailedSurveyReasonsTable>();

                    foreach (ServiceExt.SurveyFailedReason cReason in v_sfrReasons)
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
        public cDataAccess.SaveSubProjectDataResult SaveSubProjectData(string v_sProjectNo, string v_sProjectName, ServiceExt.SubProjectData v_spData)
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
                cProject.EndDateTime = ConvertDateTimeToNullable(v_spData.EndDateTime.Value);
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

                if (cSettings.IsThisTheSurveyorApp() == false)
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
        public bool SaveSubProjectUnits(string v_sSubProjectNo, string v_sSubProjectName, ObservableCollection<ServiceExt.UnitDetails> v_udUnits)
        {

            cUnitsTable udUnit;

            try
            {

                if (v_udUnits != null)
                {

                    bool bInsert = false;
                    foreach (ServiceExt.UnitDetails udDetail in v_udUnits)
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
        public cUnitsTable FetchSubProjectUnit(string v_sSubProjectNo, int v_iUnitNo)
        {

            cUnitsTable cUnit = null;
            try
            {

                //See if sub project already exists in our DB.
                var oResults = (from oCols in m_conSQL.Table<cUnitsTable>()
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
        public bool SaveSubProjectNotes(ObservableCollection<ServiceExt.NoteDetails> v_ndNotes)
        {

            cProjectNotesTable pnNote;
            try
            {

                if (v_ndNotes != null)
                {

                    foreach (ServiceExt.NoteDetails ndDetail in v_ndNotes)
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
                //throw new Exception(ex.Message + " - PARAM(SubProjectNo=" + v_sSubProjectNo + ",FileName=" + v_sFileName + ",Comment=" + v_sComments + ",ModDateTime=" + v_dModDate.ToString() + ",IsNew_InsertOnly=" + v_bIsNew_InsertOnly.ToString() + ")");
                return false;

            }
        }
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
        public bool UpdateNotesWithRecID(ObservableCollection<ServiceExt.clsRealtimeNoteKeyValues> v_nkvValues)
        {

            StringBuilder sbSQL = new StringBuilder();
            int iSQLRtn = -1;
            try
            {

                if (v_nkvValues != null)
                {

                    foreach (ServiceExt.clsRealtimeNoteKeyValues nkvValue in v_nkvValues)
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
        public List<cSurveyInputResult> SearchSurveyInput(
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
            string v_sInstallStatus_Filter = cSettings.p_sInstallStatusFilter_EqualTo, 
            string v_sOrderComplete_Filter = cSettings.p_sAnyStatus, 
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
                    else if (v_sOrderComplete_Filter == cSettings.p_sConfirmedStatus_Yes)
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
    }
}
