using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace ANG_ABP_SURVEYOR_APP_CLASS.Model
{
    /// <summary>
    /// v1.0.21 - Failed survey reasons.
    /// </summary>
    public class cFailedSurveyReasonsTable
    {

        /// <summary>
        /// IDKey field
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int IDKey { get; set; }

        /// <summary>
        /// Reason description
        /// </summary>
        [Indexed, MaxLength(100)]
        public string Description { get; set; }

        /// <summary>
        /// Display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Mandatory Note
        /// </summary>
        public bool MandatoryNote { get; set; }

        /// <summary>
        /// Progress status
        /// </summary>
        public int ProgressStatus { get; set; }

    }
}
