﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace ANG_ABP_SURVEYOR_APP_CLASS.Model
{
    public class cAXSettingsTable
    {

        /// <summary>
        /// IDKey field
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int IDKey { get; set; }

        /// <summary>
        /// Setting name
        /// </summary>
        [Indexed, MaxLength(60)]
        public string SettingName { get; set; }

        /// <summary>
        /// Setting value
        /// </summary>
        [MaxLength(500)]
        public string SettingValue { get; set; }

        /// <summary>
        /// Last update time setting was updated on server.
        /// </summary>        
        public DateTime LastUpdate { get; set; }

    }
}
