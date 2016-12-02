using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anglian.Classes
{
    public class SurveyInputResult
    {
        /// <summary>
        /// 
        /// </summary>
        public int IDKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ProjectNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SubProjectNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DeliveryStreet { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DlvZipCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string InstallStatusName { get; set; }

        /// <summary>
        /// v1.0.13 - Return status.
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// v1.0.1 
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// v1.0.1 
        /// </summary>
        public string ProgressStatusName { get; set; }

        /// <summary>
        /// v1.0.14 - Progress status
        /// </summary>
        public int Mxm1002ProgressStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// Surveyor profile field
        /// </summary>
        public string SurveyorProfile { get; set; }

        /// <summary>
        /// Surveyor name field
        /// </summary>
        public string SurveyorName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SurveyedStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Double ScreenWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Double StreetWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Double PostcodeWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Double InstallWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Double ProgressWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Flags { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SurveyInputStatus { get; set; }

        /// <summary>
        /// Surveyed Date
        /// </summary>
        public DateTime? MXM1002TrfDate { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string UpdateQty { get; set; }

        /// <summary>
        /// Install status
        /// </summary>
        public int Mxm1002InstallStatus { get; set; }

        /// <summary>
        /// Surveyor PC tag field
        /// </summary>
        public bool? MxmConfirmedAppointmentIndicator { get; set; }

        /// <summary>
        /// v1.0.1
        /// </summary>
        public string ToolTipText { get; set; }

        /// <summary>
        /// v1.0.1
        /// </summary>
        public string SurveyDisplayDateTime { get; set; }

        /// <summary>
        /// v1.0.1 - Notes quantity.
        /// </summary>
        public string NotesQty { get; set; }

        /// <summary>
        /// Units Left
        /// </summary>
        public string UnitsLeft { get; set; }

        /// <summary>
        /// Total Units
        /// </summary>
        public int? TotalUnits { get; set; }

        /// <summary>
        /// Total Units Installed
        /// </summary>
        public int? TotalUnitsInstalled { get; set; }

        /// <summary>
        /// Background color for each row
        /// </summary>
        public string BackgroundColour { get; set; }

        /// <summary>
        /// v1.0.10 - Confirmed action date time flag.
        /// </summary>
        public DateTime? ConfirmedActionDateTime { get; set; }

        /// <summary>
        /// v1.0.19 - Project name for tool-tip
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// v1.0.19 - Delivery Date
        /// </summary>
        public DateTime? Delivery_EndDateTime { get; set; }

        /// <summary>
        /// v1.0.19 - Order complete date
        /// </summary>
        public DateTime? ABPAWOrderCompletedDate { get; set; }
    }
}
