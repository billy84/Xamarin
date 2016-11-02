using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG_ABP_SURVEYOR_APP.Views
{
    class cSearchCriteria
    {

        /// <summary>
        /// 
        /// </summary>
        public string ProjectNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ProjectNoFilter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StreetAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Postcode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? InstallStatus { get; set; }

        /// <summary>
        /// v1.0.14 - Progress status
        /// </summary>
        public int? ProgressStatus { get; set; }

        /// <summary>
        /// Survey plan date included in search.
        /// </summary>
        public bool IncludeSurveyPlanDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? SurveyPlanDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SurveyPlanDateComparison { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SurveyedStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Surveyor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SurveyOnSite { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Selected_SubProjectNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool SyncChangesOnly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowAllStatuses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowAllProgressStatuses { get; set; }

        /// <summary>
        /// Confirmed sub project.
        /// </summary>
        public int? Confirmed { get; set; }

        /// <summary>
        /// v1.0.19
        /// </summary>
        public int InstallStatus_FilterIndex { get; set; }

        /// <summary>
        /// v1.0.19 - Keep record if we are in surveyed mode.
        /// </summary>
        public bool SurveyedMode { get; set; }
    }
}
