using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Anglian.Models
{
    public class cProjectTable
    {
        /// <summary>
        /// IDKey field
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int IDKey { get; set; }

        /// <summary>
        /// Project number field
        /// </summary>
        [Indexed, MaxLength(20)]
        public string ProjectNo { get; set; }

        /// <summary>
        /// Project name field
        /// </summary>
        [MaxLength(60)]
        public string ProjectName { get; set; }

        /// <summary>
        /// SubProject number field
        /// </summary>
        [Indexed, MaxLength(20)]
        public string SubProjectNo { get; set; }

        /// <summary>
        /// SubProject name field
        /// </summary>
        [MaxLength(60)]
        public string SubProjectName { get; set; }

        /// <summary>
        /// Delivery street name field
        /// </summary>
        [MaxLength(250)]
        public string DeliveryStreet { get; set; }

        /// <summary>
        /// Delivery city name field
        /// </summary>
        [MaxLength(60)]
        public string DeliveryCity { get; set; }

        /// <summary>
        /// Delivery county name field
        /// </summary>
        [MaxLength(20)]
        public string DlvState { get; set; }

        /// <summary>
        /// Delivery zip code name field
        /// </summary>
        [MaxLength(10)]
        public string DlvZipCode { get; set; }

        /// <summary>
        /// v1.0.1 - Status name field
        /// </summary>
        [Indexed]
        public int? Status { get; set; }

        /// <summary>
        /// v1.0.1 - Status description name field
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// Progress status name field
        /// </summary>
        [Indexed]
        public int? Mxm1002ProgressStatus { get; set; }

        /// <summary>
        /// Progress status description name field
        /// </summary>
        public string ProgressStatusName { get; set; }

        /// <summary>
        /// Install status name field
        /// </summary>
        [Indexed]
        public int? Mxm1002InstallStatus { get; set; }

        /// <summary>
        /// Install status description name field
        /// </summary>
        public string InstallStatusName { get; set; }

        /// <summary>
        /// Resident name field
        /// </summary>
        [MaxLength(100)]
        public string ResidentName { get; set; }

        /// <summary>
        /// Resident telephone no field
        /// </summary>
        [MaxLength(50)]
        public string ResidentTelNo { get; set; }

        /// <summary>
        /// Surveyed date time field
        /// </summary>
        [Indexed]
        public DateTime? MXM1002TrfDate { get; set; }

        /// <summary>
        /// Sequence number field
        /// </summary>
        [Indexed]
        public int? MXM1002SequenceNr { get; set; }

        /// <summary>
        /// Replacement type field
        /// </summary>
        [MaxLength(9999)]
        public string MxmProjDescription { get; set; }

        /// <summary>
        /// Resident mobile no field
        /// </summary>
        [MaxLength(50)]
        public string ResidentMobileNo { get; set; }

        /// <summary>
        /// Resident alternative contact name field
        /// </summary>
        [MaxLength(100)]
        public string AlternativeContactName { get; set; }

        /// <summary>
        /// Resident alternative contact number field
        /// </summary>
        [MaxLength(50)]
        public string AlternativeContactTelNo { get; set; }

        /// <summary>
        /// Resident alternative contact mobile number field
        /// </summary>
        [MaxLength(50)]
        public string AlternativeContactMobileNo { get; set; }

        /// <summary>
        /// Special resident note field
        /// </summary>
        [MaxLength(500)]
        public string SpecialResidentNote { get; set; }

        /// <summary>
        /// Door choice form received field
        /// </summary>
        public int? MxmDoorChoiceFormReceived { get; set; }

        /// <summary>
        /// Property type field
        /// </summary>
        public int? PropertyType { get; set; }

        /// <summary>
        /// Floor level field
        /// </summary>
        public int? ABPAXFloorLevel { get; set; }

        /// <summary>
        /// Installation type field
        /// </summary>
        public int? ABPAXInstallationType { get; set; }

        /// <summary>
        /// Asbestos Presumed field
        /// </summary>
        public int? ABPAXAsbestosPresumed { get; set; }

        /// <summary>
        /// Access Equipment field
        /// </summary>
        public int? ABPAXAccessEquipment { get; set; }

        /// <summary>
        /// Disabled adaption required field
        /// </summary>
        public int? DisabledAdaptionsRequired { get; set; }

        /// <summary>
        /// Permanent Gas vent field
        /// </summary>
        public int? ABPAXPermanentGasVent { get; set; }

        /// <summary>
        /// Window board field
        /// </summary>
        public int? ABPAXWindowBoard { get; set; }

        /// <summary>
        /// Structural Faults field
        /// </summary>
        public int? ABPAXStructuralFaults { get; set; }

        /// <summary>
        /// Services to move
        /// </summary>
        public int? ABPAXServicesToMove { get; set; }

        /// <summary>
        /// Internal Damage
        /// </summary>
        public int? ABPAXInternalDamage { get; set; }

        /// <summary>
        /// Work Access Restrictions
        /// </summary>
        public int? ABPAXWorkAccessRestrictions { get; set; }

        /// <summary>
        /// Public Protection
        /// </summary>
        public int? ABPAXPublicProtection { get; set; }

        /// <summary>
        /// Survey letter 1 sent date time field
        /// </summary>
        public DateTime? SurveyLetterSentDate01 { get; set; }

        /// <summary>
        /// Survey letter 2 sent date time field
        /// </summary>
        public DateTime? SurveyLetterSentDate02 { get; set; }

        /// <summary>
        /// Survey letter 3 sent date time field
        /// </summary>
        public DateTime? SurveyLetterSentDate03 { get; set; }

        /// <summary>
        /// Survey letter required field
        /// </summary>
        public int? MxmSurveyletterRequired { get; set; }

        /// <summary>
        /// Can resident be contacted by SMS field
        /// </summary>
        public int? MxmContactBySMS { get; set; }

        /// <summary>
        /// SMS has been sent field
        /// </summary>
        public int? MxmSMSSent { get; set; }

        /// <summary>
        /// Next day SMS has been sent field
        /// </summary>
        public int? MxmNextDaySMS { get; set; }

        /// <summary>
        /// Start date time field
        /// </summary>
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// End date time field
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// Surveyor profile field
        /// </summary>
        [MaxLength(50)]
        public string SurveyorProfile { get; set; }

        /// <summary>
        /// Surveyor name field
        /// </summary>
        [MaxLength(150)]
        public string SurveyorName { get; set; }

        /// <summary>
        /// Surveyor PC tag field
        /// </summary>
        [MaxLength(150)]
        public string SurveyorPCTag { get; set; }

        /// <summary>
        /// Surveyor PC tag field
        /// </summary>
        [Indexed]
        public int? MxmConfirmedAppointmentIndicator { get; set; }

        /// <summary>
        /// Last time records was updated from server field
        /// </summary>
        public DateTime? ModifiedDateTime { get; set; }

        /// <summary>
        /// Last time activities records was updated from server field
        /// </summary>
        public DateTime? SMMActivities_MODIFIEDDATETIME { get; set; }

        /// <summary>
        /// Last time sub project was synced
        /// </summary>
        [Indexed]
        public DateTime? DateLastSynced { get; set; }

        /// <summary>
        /// v1.0.10 - Contract manager installation team.
        /// </summary>
        [MaxLength(20)]
        public string ABPAXINSTALLATIONTEAM { get; set; }

        /// <summary>
        /// v1.0.10 - Install letter 1 sent date time field
        /// </summary>
        public DateTime? ABPAXInstallLetterSentDate01 { get; set; }

        /// <summary>
        /// v1.0.10 - Install letter 2 sent date time field
        /// </summary>
        public DateTime? ABPAXInstallLetterSentDate02 { get; set; }

        /// <summary>
        /// v1.0.10 - Install letter 3 sent date time field
        /// </summary>
        public DateTime? ABPAXInstallLetterSentDate03 { get; set; }

        /// <summary>
        /// v1.0.10 - Install letter required field
        /// </summary>
        public int? ABPAXInstallletterRequired { get; set; }

        /// <summary>
        /// v1.0.10 - Install SMS has been sent field
        /// </summary>
        public int? ABPAXInstallSMSSent { get; set; }

        /// <summary>
        /// v1.0.10 - Install Next day SMS has been sent field
        /// </summary>
        public int? ABPAXInstallNextDaySMS { get; set; }

        /// <summary>
        /// v1.0.10 - Confirmed action date time.
        /// </summary>
        public DateTime? ConfirmedActionDateTime { get; set; }

        /// <summary>
        /// v1.0.12 - Delivery confirmed appointment indicator
        /// </summary>
        public int? Delivery_ConfirmedAppointmentIndicator { get; set; }

        /// <summary>
        /// v1.0.12 - Delivery end date time
        /// </summary>
        public DateTime? Delivery_EndDateTime { get; set; }

        /// <summary>
        /// v1.0.12 - Delivery record modified date time
        /// </summary>
        public DateTime? Delivery_ModifiedDateTime { get; set; }

        /// <summary>
        /// v1.0.12 - Delivery start date time
        /// </summary>
        public DateTime? Delivery_StartDateTime { get; set; }

        /// <summary>
        /// v1.0.19  -
        /// </summary>
        [Indexed]
        public DateTime? ABPAWOrderCompletedDate { get; set; }

        /// <summary>
        /// v1.0.19  -
        /// </summary>
        [Indexed]
        public string ABPAWOriginalSubProjectID { get; set; }

        /// <summary>
        /// v1.0.21 - Health and Safety complete
        /// </summary>
        [Indexed]
        public int? ABPAWHealthSafetyInComplete { get; set; }

        /// <summary>
        /// v1.0.21 - Health and Safety date uploaded.
        /// </summary>
        public DateTime? ABPAWHealthSafetyInCompleteDateUploaded { get; set; }

        /// <summary>
        /// v1.0.21 - Health and Safety date uploaded.
        /// </summary>
        public string ABPAWHealthSafetyInCompleteUserUploaded { get; set; }
    }
}
