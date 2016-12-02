using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Anglian.Models
{
    public class cAppSettingsTable
    {
        /// <summary>
        /// IDKey field
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int IDKey { get; set; }

        /// <summary>
        /// Users login profile field
        /// </summary>
        [MaxLength(20)]
        public string UserProfile { get; set; }

        /// <summary>
        /// Users full name field
        /// </summary>
        [MaxLength(150)]
        public string UsersFullName { get; set; }

        /// <summary>
        /// v1.0.1 - Users job title.
        /// </summary>
        [MaxLength(50)]
        public string UsersJobTitle { get; set; }

        /// <summary>
        /// Path to the users profile picture
        /// </summary>
        [MaxLength(300)]
        public string ProfilePicPath { get; set; }

        /// <summary>
        /// Date last sync took place.
        /// </summary>
        public DateTime? LastSyncDateTime { get; set; }

        /// <summary>
        /// Date last base enum check took place.
        /// </summary>
        public DateTime? LastBaseEnumCheckDateTime { get; set; }

        /// <summary>
        /// Date last settings check took place.
        /// </summary>
        public DateTime? LastSettingsCheckDateTime { get; set; }

        /// <summary>
        /// v1.0.8 - Date last installers check took place.
        /// </summary>
        public DateTime? LastInstallersCheckDateTime { get; set; }

        /// <summary>
        /// v1.0.8 - Date time of last installers update as set on the server.
        /// </summary>
        public DateTime? LastInstallersUpdateDateTime { get; set; }

        /// <summary>
        /// v1.0.5 - Mode in which the application is running, LIVE or TEST
        /// </summary>
        [MaxLength(4)]
        public string RunningMode { get; set; }

        /// <summary>
        /// v1.0.21 - Date time of last survey failed reasons update from server
        /// </summary>
        public DateTime? LastSurveyFailedUpdateDateTime { get; set; }

        /// <summary>
        /// v1.0.21 - Date time of last survey failed check
        /// </summary>
        public DateTime? LastSurveyFailedCheckDateTime { get; set; }
    }
}
