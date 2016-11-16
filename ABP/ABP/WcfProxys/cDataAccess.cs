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
                this.m_bSurveyorApp = cSettings.IsthisTheSurveyorApp();
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
        public bool SaveSettings(cAppSettingsTable v_cSettings)
        {

            try
            {

                v_cSettings.UserProfile = "prote01";
                v_cSettings.UsersFullName = "prote01";

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
    }
}
