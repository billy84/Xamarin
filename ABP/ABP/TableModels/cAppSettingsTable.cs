using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
namespace ABP.TableModels
{
    public class cAppSettingsTable
    {
        [PrimaryKey, AutoIncrement]
        public int IDKey { get; set; }
        [MaxLength(20)]
        public string UserProfile { get; set; }
        [MaxLength(150)]
        public string UsersFullName { get; set; }
        [MaxLength(50)]
        public string UsersJobTitle { get; set; }
        [MaxLength(300)]
        public string ProfilePicPath { get; set; }
        public DateTime? LastSyncDateTime { get; set; }
        public DateTime? LastBaseEnumCheckDateTime { get; set; }
        public DateTime? LastSettingsCheckDateTime { get; set; }
        public DateTime? LastInstallersCheckDateTime { get; set; }
        public DateTime? LastInstallersUpdateDateTime { get; set; }
        [MaxLength(4)]
        public string RunningMode { get; set; }
        public DateTime? LastSurveyFailedUpdateDateTime { get; set; }
        public DateTime? LastSurveyFailedCheckDateTime { get; set; } 
    }
}
