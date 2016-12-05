using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anglian.Classes
{
    public class SubProjectData
    {
        public Nullable<DateTime> ABPAWORDERCOMPLETEDDATE;

        public string ABPAWORIGINALSUBPROJECTID;

        public Nullable<int> ABPAXACCESSEQUIPMENT;

        public Nullable<int> ABPAXASBESTOSPRESUMED;

        public Nullable<int> ABPAXFLOORLEVEL;

        public string ABPAXHealthSaferyIncompleteUploadedBy;

        public Nullable<int> ABPAXHealthSafetyIncomplete;

        public DateTime ABPAXHealthSafetyIncompleteDateUploaded;

        public string ABPAXINSTALLATIONTEAM;

        public Nullable<int> ABPAXINSTALLATIONTYPE;

        public Nullable<int> ABPAXINSTALLLETTERREQUIRED;

        public Nullable<DateTime> ABPAXINSTALLLETTERSENTDATE01;

        public Nullable<DateTime> ABPAXINSTALLLETTERSENTDATE02;

        public Nullable<DateTime> ABPAXINSTALLLETTERSENTDATE03;

        public Nullable<int> ABPAXINSTALLNEXTDAYSMS;

        public Nullable<int> ABPAXINSTALLSMSSENT;

        public Nullable<int> ABPAXINTERNDAMAGE;

        public Nullable<int> ABPAXPERMANENTGASVENT;

        public Nullable<int> ABPAXPUBLICPROTECT;

        public Nullable<int> ABPAXSERVICESTOMOVE;

        public Nullable<int> ABPAXSTRUCTURALFAULTS;

        public Nullable<int> ABPAXWINDOWBOARD;

        public Nullable<int> ABPAXWRKACCRESTRICTIONS;

        public string DeliveryCity;

        public string DeliveryStreet;

        public Nullable<int> Delivery_ConfirmedAppointmentIndicator;

        public Nullable<DateTime> Delivery_EndDateTime;

        public Nullable<DateTime> Delivery_ModifiedDateTime;

        public Nullable<DateTime> Delivery_StartDateTime;

        public string DlvState;

        public string DlvZipCode;

        public string Email;

        public Nullable<DateTime> EndDateTime;

        public Nullable<DateTime> MODIFIEDDATETIME;

        public Nullable<int> MXM1002SequenceNr;

        public Nullable<DateTime> MXM1002TrfDate;

        public string MXMAlternativeContactMobileNo;

        public string MXMAlternativeContactName;

        public string MXMAlternativeContactTelNo;

        public Nullable<int> MXMConfirmedAppointmentIndicator;

        public Nullable<int> MXMContactBySMS;

        public Nullable<int> MXMDisabledAdaptionsRequired;

        public Nullable<int> MXMDoorChoiceFormReceived;

        public Nullable<int> MXMNextDaySMS;

        public Nullable<int> MXMPropertyType;

        public string MXMResidentMobileNo;

        public string MXMResidentName;

        public Nullable<int> MXMSMSSent;

        public string MXMSpecialResidentNote;

        public Nullable<DateTime> MXMSurveyLetterSentDate01;

        public Nullable<DateTime> MXMSurveyLetterSentDate02;

        public Nullable<DateTime> MXMSurveyLetterSentDate03;

        public Nullable<int> MXMSurveyletterRequired;

        public string MXMSurveyorName;

        public string MXMSurveyorPCTag;

        public string MXMSurveyorProfile;

        public string MXMTelephoneNo;

        public Nullable<int> Mxm1002InstallStatus;

        public Nullable<int> Mxm1002ProgressStatus;

        public string MxmProjDescription;

        public string Name;

        public ObservableCollection<NoteDetails> Notes;

        public string ParentID;

        public string ProjId;

        public string Purpose;

        public Nullable<DateTime> SMMActivities_MODIFIEDDATETIME;

        public Nullable<DateTime> StartDateTime;

        public Nullable<int> Status;

        public string URL;

        public ObservableCollection<UnitDetails> Units;
    }
}
