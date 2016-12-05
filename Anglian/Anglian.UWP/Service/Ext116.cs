using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.UWP.Service;
using Xamarin.Forms;
using Anglian.Service;
using Anglian.Classes;
[assembly: Dependency(typeof(Ext116))]
namespace Anglian.UWP.Service
{
    class Ext116 : IWcfExt116
    {
        private WcfExt116.ServiceClient m_wcfClient = new WcfExt116.ServiceClient();
        public async Task<FetchSurveyFailedReasonsResult> FetchFailedSurveyReasons(
            string v_sCompanyName, 
            DateTime v_dLastUpdate,
            string v_sAuthID,
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            FetchSurveyFailedReasonsResult result = new FetchSurveyFailedReasonsResult();
            try
            {
                WcfExt116.FetchSurveyFailedReasonsResult sResult = await m_wcfClient.FetchFailedSurveyReasonsAsync(
                    v_sCompanyName,
                    v_dLastUpdate,
                    v_sAuthID,
                    v_sToken);
                if (sResult.bSuccessfull == true)
                {
                    result.bSuccessfull = sResult.bSuccessfull;
                    result.bLastUpdateDate = sResult.bLastUpdateDate;
                    result.sfrReasons = new ObservableCollection<SurveyFailedReason>();
                    foreach (WcfExt116.SurveyFailedReason o in sResult.sfrReasons)
                    {
                        SurveyFailedReason oSFR = new SurveyFailedReason();
                        oSFR.bMandatoryNote = o.bMandatoryNote;
                        oSFR.iDisplayOrder = o.iDisplayOrder;
                        oSFR.iProgressStatus = o.iProgressStatus;
                        oSFR.sReason = o.sReason;
                        result.sfrReasons.Add(oSFR);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //return null;
            }
        }
        public async Task<BaseEnumResult> ReturnBaseEnumsAsync(
            string v_sCompanyName, 
            ObservableCollection<BaseEnumField> v_beFields, 
            string v_sAuthID, 
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            BaseEnumResult result = new BaseEnumResult();
            try
            {
                ObservableCollection<WcfExt116.BaseEnumField> lSettingDetails = new ObservableCollection<WcfExt116.BaseEnumField>();
                foreach (BaseEnumField sd in v_beFields)
                {
                    WcfExt116.BaseEnumField oSD = new WcfExt116.BaseEnumField();
                    oSD.LastUpdate = sd.LastUpdate;
                    foreach (BaseEnumValue o in sd.BaseEnums)
                    {
                        WcfExt116.BaseEnumValue oBEV = new WcfExt116.BaseEnumValue();
                        oBEV.BaseName = o.BaseName;
                        oBEV.BaseValue = o.BaseValue;
                        oSD.BaseEnums.Add(oBEV);
                    }
                    oSD.FieldName = sd.FieldName;
                    oSD.TableName = sd.TableName;
                    lSettingDetails.Add(oSD);
                }
                WcfExt116.BaseEnumResult sResult = await m_wcfClient.ReturnBaseEnumsAsync(
                    v_sCompanyName,
                    lSettingDetails,
                    v_sAuthID,
                    v_sToken);
                if (sResult.bSuccessfull == true)
                {
                    result.bSuccessfull = sResult.bSuccessfull;
                    result.BaseEnumResults = new ObservableCollection<BaseEnumField>();
                    foreach (WcfExt116.BaseEnumField o in sResult.BaseEnumResults)
                    {
                        BaseEnumField oSD = new BaseEnumField();
                        oSD.LastUpdate = o.LastUpdate;
                        oSD.BaseEnums = new ObservableCollection<BaseEnumValue>();
                        foreach (WcfExt116.BaseEnumValue oBEV in o.BaseEnums)
                        {
                            BaseEnumValue bEV = new BaseEnumValue();
                            bEV.BaseName = oBEV.BaseName;
                            bEV.BaseValue = oBEV.BaseValue;
                            oSD.BaseEnums.Add(bEV);
                        }
                        oSD.FieldName = o.FieldName;
                        oSD.TableName = o.TableName;
                        result.BaseEnumResults.Add(oSD);
                    }

                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<SettingsCheckResult> CheckForUpdatedSettingsAsync(
            string v_sCompanyName,
            ObservableCollection<SettingDetails> v_svSettings,
            string v_sAuthID,
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            SettingsCheckResult result = new SettingsCheckResult();
            try
            {
                ObservableCollection<WcfExt116.SettingDetails> lSettingDetails = new ObservableCollection<WcfExt116.SettingDetails>();
                foreach (SettingDetails sd in v_svSettings)
                {
                    WcfExt116.SettingDetails oSD = new WcfExt116.SettingDetails();
                    oSD.LastUpdate = sd.LastUpdate;
                    oSD.SettingName = sd.SettingName;
                    oSD.SettingValue = sd.SettingValue;
                    lSettingDetails.Add(oSD);
                }
                WcfExt116.SettingsCheckResult sResult = await m_wcfClient.CheckForUpdatedSettingsAsync(
                    v_sCompanyName,
                    lSettingDetails,
                    v_sAuthID,
                    v_sToken);
                if (sResult.bSuccessfull == true)
                {
                    result.bSuccessfull = sResult.bSuccessfull;
                    result.Settings = new ObservableCollection<SettingDetails>();
                    foreach (WcfExt116.SettingDetails o in sResult.Settings)
                    {
                        SettingDetails oSD = new SettingDetails();
                        oSD.LastUpdate = o.LastUpdate;
                        oSD.SettingName = o.SettingName;
                        oSD.SettingValue = o.SettingValue;
                        result.Settings.Add(oSD);
                    }
                    
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<SystemsAvailableResult> ReturnAreSystemsAvailableAsync(
            string v_sCompanyName, 
            string v_sUserProfile, 
            string v_sAuthID, 
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            SystemsAvailableResult result = new SystemsAvailableResult();
            try
            {
                WcfExt116.SystemsAvailableResult sResult = await m_wcfClient.ReturnAreSystemsAvailableAsync(
                    v_sCompanyName, 
                    v_sUserProfile, 
                    v_sAuthID, 
                    v_sToken);
                if (sResult.bSuccessfull == true)
                {
                    result.bSuccessfull = sResult.bSuccessfull;
                    result.SystemsAvailable = sResult.SystemsAvailable;
                    result.UserAccountOK = sResult.UserAccountOK;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Close AX connection
        /// </summary>
        /// <returns></returns>
        public async Task CloseAXConnection()
        {

            try
            {
                m_wcfClient = new WcfExt116.ServiceClient();

                if (this.m_wcfClient != null)
                {
                    await this.m_wcfClient.CloseAsync();
                    this.m_wcfClient = null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }
        public async Task<SubProjectDataResult> ReturnSubProjectDataAsync(
            string v_sCompanyName, 
            string v_sProjectNo, 
            string v_sPurpose, 
            string v_sAuthID, 
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            SubProjectDataResult result = new SubProjectDataResult();
            try
            {
                WcfExt116.SubProjectDataResult sResult = await m_wcfClient.ReturnSubProjectDataAsync(
                    v_sCompanyName,
                    v_sProjectNo,
                    v_sPurpose,
                    v_sAuthID,
                    v_sToken);
                if (sResult.bSuccessfull == true)
                {
                    result.bSuccessfull = sResult.bSuccessfull;
                    result.pdSubProjectData = new SubProjectData();
                    result.pdSubProjectData.ABPAWORDERCOMPLETEDDATE = sResult.pdSubProjectData.ABPAWORDERCOMPLETEDDATE;
                    result.pdSubProjectData.ABPAWORIGINALSUBPROJECTID = sResult.pdSubProjectData.ABPAWORIGINALSUBPROJECTID;
                    result.pdSubProjectData.ABPAXACCESSEQUIPMENT = sResult.pdSubProjectData.ABPAXACCESSEQUIPMENT;
                    result.pdSubProjectData.ABPAXASBESTOSPRESUMED = sResult.pdSubProjectData.ABPAXASBESTOSPRESUMED;
                    result.pdSubProjectData.ABPAXFLOORLEVEL = sResult.pdSubProjectData.ABPAXFLOORLEVEL;
                    result.pdSubProjectData.ABPAXHealthSaferyIncompleteUploadedBy = sResult.pdSubProjectData.ABPAXHealthSaferyIncompleteUploadedBy;
                    result.pdSubProjectData.ABPAXHealthSafetyIncomplete = sResult.pdSubProjectData.ABPAXHealthSafetyIncomplete;
                    result.pdSubProjectData.ABPAXHealthSafetyIncompleteDateUploaded = sResult.pdSubProjectData.ABPAXHealthSafetyIncompleteDateUploaded;
                    result.pdSubProjectData.ABPAXINSTALLATIONTEAM = sResult.pdSubProjectData.ABPAXINSTALLATIONTEAM;
                    result.pdSubProjectData.ABPAXINSTALLATIONTYPE = sResult.pdSubProjectData.ABPAXINSTALLATIONTYPE;
                    result.pdSubProjectData.ABPAXINSTALLLETTERREQUIRED = sResult.pdSubProjectData.ABPAXINSTALLLETTERREQUIRED;
                    result.pdSubProjectData.ABPAXINSTALLLETTERSENTDATE01 = sResult.pdSubProjectData.ABPAXINSTALLLETTERSENTDATE01;
                    result.pdSubProjectData.ABPAXINSTALLLETTERSENTDATE02 = sResult.pdSubProjectData.ABPAXINSTALLLETTERSENTDATE02;
                    result.pdSubProjectData.ABPAXINSTALLLETTERSENTDATE03 = sResult.pdSubProjectData.ABPAXINSTALLLETTERSENTDATE03;
                    result.pdSubProjectData.ABPAXINSTALLNEXTDAYSMS = sResult.pdSubProjectData.ABPAXINSTALLNEXTDAYSMS;
                    result.pdSubProjectData.ABPAXINSTALLSMSSENT = sResult.pdSubProjectData.ABPAXINSTALLSMSSENT;
                    result.pdSubProjectData.ABPAXINTERNDAMAGE = sResult.pdSubProjectData.ABPAXINTERNDAMAGE;
                    result.pdSubProjectData.ABPAXPERMANENTGASVENT = sResult.pdSubProjectData.ABPAXPERMANENTGASVENT;
                    result.pdSubProjectData.ABPAXPUBLICPROTECT = sResult.pdSubProjectData.ABPAXPUBLICPROTECT;
                    result.pdSubProjectData.ABPAXSERVICESTOMOVE = sResult.pdSubProjectData.ABPAXSERVICESTOMOVE;
                    result.pdSubProjectData.ABPAXSTRUCTURALFAULTS = sResult.pdSubProjectData.ABPAXSTRUCTURALFAULTS;
                    result.pdSubProjectData.ABPAXWINDOWBOARD = sResult.pdSubProjectData.ABPAXWINDOWBOARD;
                    result.pdSubProjectData.ABPAXWRKACCRESTRICTIONS = sResult.pdSubProjectData.ABPAXWRKACCRESTRICTIONS;
                    result.pdSubProjectData.DeliveryCity = sResult.pdSubProjectData.DeliveryCity;
                    result.pdSubProjectData.DeliveryStreet = sResult.pdSubProjectData.DeliveryStreet;
                    result.pdSubProjectData.Delivery_ConfirmedAppointmentIndicator = sResult.pdSubProjectData.Delivery_ConfirmedAppointmentIndicator;
                    result.pdSubProjectData.Delivery_EndDateTime = sResult.pdSubProjectData.Delivery_EndDateTime;
                    result.pdSubProjectData.Delivery_ModifiedDateTime = sResult.pdSubProjectData.Delivery_ModifiedDateTime;
                    result.pdSubProjectData.Delivery_StartDateTime = sResult.pdSubProjectData.Delivery_StartDateTime;
                    result.pdSubProjectData.DlvState = sResult.pdSubProjectData.DlvState;
                    result.pdSubProjectData.DlvZipCode = sResult.pdSubProjectData.DlvZipCode;
                    result.pdSubProjectData.Email = sResult.pdSubProjectData.Email;
                    result.pdSubProjectData.EndDateTime = sResult.pdSubProjectData.EndDateTime;
                    result.pdSubProjectData.MODIFIEDDATETIME = sResult.pdSubProjectData.MODIFIEDDATETIME;
                    result.pdSubProjectData.Mxm1002InstallStatus = sResult.pdSubProjectData.Mxm1002InstallStatus;
                    result.pdSubProjectData.Mxm1002ProgressStatus = sResult.pdSubProjectData.Mxm1002ProgressStatus;
                    result.pdSubProjectData.MXM1002SequenceNr = sResult.pdSubProjectData.MXM1002SequenceNr;
                    result.pdSubProjectData.MXM1002TrfDate = sResult.pdSubProjectData.MXM1002TrfDate;
                    result.pdSubProjectData.MXMAlternativeContactMobileNo = sResult.pdSubProjectData.MXMAlternativeContactMobileNo;
                    result.pdSubProjectData.MXMAlternativeContactName = sResult.pdSubProjectData.MXMAlternativeContactName;
                    result.pdSubProjectData.MXMAlternativeContactTelNo = sResult.pdSubProjectData.MXMAlternativeContactTelNo;
                    result.pdSubProjectData.MXMConfirmedAppointmentIndicator = sResult.pdSubProjectData.MXMConfirmedAppointmentIndicator;
                    result.pdSubProjectData.MXMContactBySMS = sResult.pdSubProjectData.MXMContactBySMS;
                    result.pdSubProjectData.MXMDisabledAdaptionsRequired = sResult.pdSubProjectData.MXMDisabledAdaptionsRequired;
                    result.pdSubProjectData.MXMDoorChoiceFormReceived = sResult.pdSubProjectData.MXMDoorChoiceFormReceived;
                    result.pdSubProjectData.MXMNextDaySMS = sResult.pdSubProjectData.MXMNextDaySMS;
                    result.pdSubProjectData.MxmProjDescription = sResult.pdSubProjectData.MxmProjDescription;
                    result.pdSubProjectData.MXMPropertyType = sResult.pdSubProjectData.MXMPropertyType;
                    result.pdSubProjectData.MXMResidentMobileNo = sResult.pdSubProjectData.MXMResidentMobileNo;
                    result.pdSubProjectData.MXMResidentName = sResult.pdSubProjectData.MXMResidentName;
                    result.pdSubProjectData.MXMSMSSent = sResult.pdSubProjectData.MXMSMSSent;
                    result.pdSubProjectData.MXMSpecialResidentNote = sResult.pdSubProjectData.MXMSpecialResidentNote;
                    result.pdSubProjectData.MXMSurveyletterRequired = sResult.pdSubProjectData.MXMSurveyletterRequired;
                    result.pdSubProjectData.MXMSurveyLetterSentDate01 = sResult.pdSubProjectData.MXMSurveyLetterSentDate01;
                    result.pdSubProjectData.MXMSurveyLetterSentDate02 = sResult.pdSubProjectData.MXMSurveyLetterSentDate02;
                    result.pdSubProjectData.MXMSurveyLetterSentDate03 = sResult.pdSubProjectData.MXMSurveyLetterSentDate03;
                    result.pdSubProjectData.MXMSurveyorName = sResult.pdSubProjectData.MXMSurveyorName;
                    result.pdSubProjectData.MXMSurveyorPCTag = sResult.pdSubProjectData.MXMSurveyorPCTag;
                    result.pdSubProjectData.MXMSurveyorProfile = sResult.pdSubProjectData.MXMSurveyorProfile;
                    result.pdSubProjectData.MXMTelephoneNo = sResult.pdSubProjectData.MXMTelephoneNo;
                    result.pdSubProjectData.Name = sResult.pdSubProjectData.Name;
                    result.pdSubProjectData.Notes = new ObservableCollection<NoteDetails>();
                    if (sResult.pdSubProjectData.Notes != null)
                    {
                        foreach (WcfExt116.NoteDetails o in sResult.pdSubProjectData.Notes)
                        {
                            NoteDetails oND = new NoteDetails();
                            oND.AXRecID = o.AXRecID;
                            oND.DeviceIDKey = o.DeviceIDKey;
                            oND.InputDate = o.InputDate;
                            oND.NoteText = o.NoteText;
                            oND.NoteType = o.NoteType;
                            oND.ProjectNo = o.ProjectNo;
                            oND.Purpose = o.Purpose;
                            oND.UserName = o.UserName;
                            oND.UserProfile = o.UserProfile;
                            result.pdSubProjectData.Notes.Add(oND);
                        }
                    }
                    result.pdSubProjectData.ParentID = sResult.pdSubProjectData.ParentID;
                    result.pdSubProjectData.ProjId = sResult.pdSubProjectData.ProjId;
                    result.pdSubProjectData.Purpose = sResult.pdSubProjectData.Purpose;
                    result.pdSubProjectData.SMMActivities_MODIFIEDDATETIME = sResult.pdSubProjectData.SMMActivities_MODIFIEDDATETIME;
                    result.pdSubProjectData.StartDateTime = sResult.pdSubProjectData.StartDateTime;
                    result.pdSubProjectData.Status = sResult.pdSubProjectData.Status;
                    result.pdSubProjectData.Units = new ObservableCollection<UnitDetails>();
                    if (sResult.pdSubProjectData.Units != null)
                    {
                        foreach (WcfExt116.UnitDetails o in sResult.pdSubProjectData.Units)
                        {
                            UnitDetails oUD = new UnitDetails();
                            oUD.dInstalledDate = o.dInstalledDate;
                            oUD.iInstalledStatus = o.iInstalledStatus;
                            oUD.iUNITNUMBER = o.iUNITNUMBER;
                            oUD.sITEMID = o.sITEMID;
                            oUD.sSTYLE = o.sSTYLE;
                            oUD.sUNITLOCATION = o.sUNITLOCATION;
                            result.pdSubProjectData.Units.Add(oUD);
                        }
                    }
                    result.pdSubProjectData.URL = sResult.pdSubProjectData.URL;

                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<SubProjectsListResult> ReturnSubProjectsListAsync(
            string v_sCompanyName, 
            string v_sProjectNo, 
            string v_sPurpose, 
            string v_sAuthID, 
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            SubProjectsListResult result = new SubProjectsListResult();
            try
            {
                WcfExt116.SubProjectsListResult sResult = await m_wcfClient.ReturnSubProjectsListAsync(
                    v_sCompanyName,
                    v_sProjectNo,
                    v_sPurpose,
                    v_sAuthID,
                    v_sToken);
                if (sResult.bSuccessfull == true)
                {
                    result.bSuccessfull = sResult.bSuccessfull;
                    result.SubProjects = new ObservableCollection<string>();
                    foreach (string o in sResult.SubProjects)
                    {
                        result.SubProjects.Add(o);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<ProjectValidationResult> ValidateProjectAsync(
            string v_sCompanyName, 
            string v_sProjectNo, 
            string v_sAuthID, 
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            ProjectValidationResult result = new ProjectValidationResult();
            try
            {
                WcfExt116.ProjectValidationResult sResult = await m_wcfClient.ValidateProjectAsync(
                    v_sCompanyName,
                    v_sProjectNo,
                    v_sAuthID,
                    v_sToken);
                if (sResult.bSuccessfull == true)
                {
                    result.bSuccessfull = sResult.bSuccessfull;
                    result.bProjectFound = sResult.bProjectFound;
                    result.ValidationResult = new SearchResult();
                    result.ValidationResult.ProjectName = sResult.ValidationResult.ProjectName;
                    result.ValidationResult.ProjectNo = sResult.ValidationResult.ProjectNo;
                    result.ValidationResult.Status = sResult.ValidationResult.Status;
                }
                return result;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<ProjectSearchResult> SearchForContractAsync(
            string v_sCompanyName, 
            string v_sProjectName, 
            string v_sAuthID, 
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            ProjectSearchResult result = new ProjectSearchResult();
            try
            {
                WcfExt116.ProjectSearchResult sResults = await m_wcfClient.SearchForContractAsync(
                    v_sCompanyName,
                    v_sProjectName,
                    v_sAuthID,
                    v_sToken);
                if (sResults.bSuccessfull == true)
                {
                    result.bSuccessfull = sResults.bSuccessfull;
                    result.SearchResults = new ObservableCollection<SearchResult>();
                    foreach (WcfExt116.SearchResult o in sResults.SearchResults)
                    {
                        SearchResult oSR = new SearchResult();
                        oSR.ProjectName = o.ProjectName;
                        oSR.ProjectNo = o.ProjectNo;
                        oSR.Status = o.Status;
                        result.SearchResults.Add(oSR);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<UploadChangesResult> UploadSubProjectFilesAsync(
            string v_sCompanyName, 
            string v_sSubProjectNo, 
            string v_sUserName, 
            string v_sMachineName, 
            ObservableCollection<UploadFileChange> v_cChanges, 
            string v_sAuthID, 
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            UploadChangesResult result = new UploadChangesResult();
            try
            {
                ObservableCollection<WcfExt116.UploadFileChange> cChanges = new ObservableCollection<WcfExt116.UploadFileChange>();
                foreach (UploadFileChange o in v_cChanges)
                {
                    WcfExt116.UploadFileChange oUFC = new WcfExt116.UploadFileChange();
                    oUFC.byData = new byte[o.byData.Length];
                    Array.Copy(o.byData, oUFC.byData, o.byData.Length);
                    oUFC.sComment = o.sComment;
                    oUFC.sFileName = o.sFileName;
                    cChanges.Add(oUFC);
                }
                WcfExt116.UploadChangesResult sResult = await m_wcfClient.UploadSubProjectFilesAsync(
                    v_sCompanyName,
                    v_sSubProjectNo,
                    v_sUserName,
                    v_sMachineName,
                    cChanges,
                    v_sAuthID,
                    v_sToken);
                if (sResult.bSuccessfull == true)
                {
                    result.bSuccessfull = sResult.bSuccessfull;
                    result.ActivitiesTable_ModDate = sResult.ActivitiesTable_ModDate;
                    result.ProjTable_ModDate = sResult.ProjTable_ModDate;
                    result.NoteValues = new ObservableCollection<RealtimeNoteKeyValues>();
                    foreach(WcfExt116.clsRealtimeNoteKeyValues o in sResult.NoteValues)
                    {
                        RealtimeNoteKeyValues oRNKV = new RealtimeNoteKeyValues();
                        oRNKV.DeviceIDKey = o.DeviceIDKey;
                        oRNKV.NotesRecID = o.NotesRecID;
                        oRNKV.ProjectNo = o.ProjectNo;
                        result.NoteValues.Add(oRNKV);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<FileChangesResult> ReturnSubProjectFileChangesAsync(
            string v_sCompanyName, 
            string v_sProjectNo, 
            string v_sSubProjectNo, 
            ObservableCollection<SubProjectFileDetail> v_fdFiles, 
            string v_sAuthID, 
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            FileChangesResult result = new FileChangesResult();
            try
            {
                ObservableCollection<WcfExt116.SubProjectFileDetail> fdFiles = new ObservableCollection<WcfExt116.SubProjectFileDetail>();
                foreach (SubProjectFileDetail sd in v_fdFiles)
                {
                    WcfExt116.SubProjectFileDetail oSPFD = new WcfExt116.SubProjectFileDetail();
                    oSPFD.sProjectNo = sd.sProjectNo;
                    oSPFD.sSubProjectNo = sd.sSubProjectNo;
                    oSPFD.sfFiles = new ObservableCollection<WcfExt116.SubProjectFile>();
                    foreach (SubProjectFile o in sd.sfFiles)
                    {
                        WcfExt116.SubProjectFile oSPF = new WcfExt116.SubProjectFile();
                        oSPF.bFileDeleted = o.bFileDeleted;
                        oSPF.Comments = o.Comments;
                        oSPF.FileName = o.FileName;
                        oSPF.ModifiedDate = o.ModifiedDate;
                        oSPFD.sfFiles.Add(oSPF);
                    }
                    fdFiles.Add(oSPFD);
                }
                WcfExt116.FileChangesResult sResult = await m_wcfClient.ReturnSubProjectFileChangesAsync(
                    v_sCompanyName,
                    v_sProjectNo,
                    v_sSubProjectNo,
                    fdFiles,
                    v_sAuthID,
                    v_sToken);
                if (sResult.bSuccessfull == true)
                {
                    result.bSuccessfull = sResult.bSuccessfull;
                    result.sfChanges = new ObservableCollection<SubProjectFileDetail>();
                    foreach (WcfExt116.SubProjectFileDetail o in sResult.sfChanges)
                    {
                        SubProjectFileDetail oSPFD = new SubProjectFileDetail();
                        oSPFD.sProjectNo = o.sProjectNo;
                        oSPFD.sSubProjectNo = o.sSubProjectNo;
                        oSPFD.sfFiles = new ObservableCollection<SubProjectFile>();
                        foreach (WcfExt116.SubProjectFile oBEV in o.sfFiles)
                        {
                            SubProjectFile oSPF = new SubProjectFile();
                            oSPF.bFileDeleted = oBEV.bFileDeleted;
                            oSPF.Comments = oBEV.Comments;
                            oSPF.FileName = oBEV.FileName;
                            oSPF.ModifiedDate = oBEV.ModifiedDate;
                            oSPFD.sfFiles.Add(oSPF);
                        }
                        result.sfChanges.Add(oSPFD);
                    }

                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DownloadDataChangesResult> CheckForDataDownloadChangesAsync(
            string v_sCompanyName, 
            string v_sPurpose, 
            ObservableCollection<DownloadDataChange> v_cSubProjects, 
            bool v_bCheckForNewSubProjects, 
            string v_sAuthID, 
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            DownloadDataChangesResult result = new DownloadDataChangesResult();
            try
            {
                ObservableCollection<WcfExt116.DownloadDataChange> cSubProjects = new ObservableCollection<WcfExt116.DownloadDataChange>();
                foreach (DownloadDataChange o in v_cSubProjects)
                {
                    WcfExt116.DownloadDataChange oND = new WcfExt116.DownloadDataChange();
                    oND.ActivitiesTable_ModDate = o.ActivitiesTable_ModDate;
                    oND.Delivery_ModDate = o.Delivery_ModDate;
                    oND.ProjTable_ModDate = o.ProjTable_ModDate;
                    oND.sProjectNo = o.sProjectNo;
                    oND.sSubProjectNo = o.sSubProjectNo;
                    oND.Notes = new ObservableCollection<WcfExt116.clsRealtimeNoteKeyValues>();
                    oND.Units = new ObservableCollection<WcfExt116.UnitDetails>();
                    cSubProjects.Add(oND);
                }
                WcfExt116.DownloadDataChangesResult sResult = await m_wcfClient.CheckForDataDownloadChangesAsync(
                    v_sCompanyName,
                    v_sPurpose,
                    cSubProjects,
                    v_bCheckForNewSubProjects,
                    v_sAuthID,
                    v_sToken);
                if (sResult.bSuccessfull == true)
                {
                    result.bSuccessfull = sResult.bSuccessfull;
                    result.pdChanged = new ObservableCollection<SubProjectData>();
                    result.sDeleted = new ObservableCollection<string>();
                    foreach (WcfExt116.SubProjectData o in sResult.pdChanged)
                    {
                        SubProjectData oSPD = new SubProjectData();

                        oSPD.ABPAWORDERCOMPLETEDDATE = o.ABPAWORDERCOMPLETEDDATE;
                        oSPD.ABPAWORIGINALSUBPROJECTID = o.ABPAWORIGINALSUBPROJECTID;
                        oSPD.ABPAXACCESSEQUIPMENT = o.ABPAXACCESSEQUIPMENT;
                        oSPD.ABPAXASBESTOSPRESUMED = o.ABPAXASBESTOSPRESUMED;
                        oSPD.ABPAXFLOORLEVEL = o.ABPAXFLOORLEVEL;
                        oSPD.ABPAXHealthSaferyIncompleteUploadedBy = o.ABPAXHealthSaferyIncompleteUploadedBy;
                        oSPD.ABPAXHealthSafetyIncomplete = o.ABPAXHealthSafetyIncomplete;
                        oSPD.ABPAXHealthSafetyIncompleteDateUploaded = o.ABPAXHealthSafetyIncompleteDateUploaded;
                        oSPD.ABPAXINSTALLATIONTEAM = o.ABPAXINSTALLATIONTEAM;
                        oSPD.ABPAXINSTALLATIONTYPE = o.ABPAXINSTALLATIONTYPE;
                        oSPD.ABPAXINSTALLLETTERREQUIRED = o.ABPAXINSTALLLETTERREQUIRED;
                        oSPD.ABPAXINSTALLLETTERSENTDATE01 = o.ABPAXINSTALLLETTERSENTDATE01;
                        oSPD.ABPAXINSTALLLETTERSENTDATE02 = o.ABPAXINSTALLLETTERSENTDATE02;
                        oSPD.ABPAXINSTALLLETTERSENTDATE03 = o.ABPAXINSTALLLETTERSENTDATE03;
                        oSPD.ABPAXINSTALLNEXTDAYSMS = o.ABPAXINSTALLNEXTDAYSMS;
                        oSPD.ABPAXINSTALLSMSSENT = o.ABPAXINSTALLSMSSENT;
                        oSPD.ABPAXINTERNDAMAGE = o.ABPAXINTERNDAMAGE;
                        oSPD.ABPAXPERMANENTGASVENT = o.ABPAXPERMANENTGASVENT;
                        oSPD.ABPAXPUBLICPROTECT = o.ABPAXPUBLICPROTECT;
                        oSPD.ABPAXSERVICESTOMOVE = o.ABPAXSERVICESTOMOVE;
                        oSPD.ABPAXSTRUCTURALFAULTS = o.ABPAXSTRUCTURALFAULTS;
                        oSPD.ABPAXWINDOWBOARD = o.ABPAXWINDOWBOARD;
                        oSPD.ABPAXWRKACCRESTRICTIONS = o.ABPAXWRKACCRESTRICTIONS;
                        oSPD.DeliveryCity = o.DeliveryCity;
                        oSPD.DeliveryStreet = o.DeliveryStreet;
                        oSPD.Delivery_ConfirmedAppointmentIndicator = o.Delivery_ConfirmedAppointmentIndicator;
                        oSPD.Delivery_EndDateTime = o.Delivery_EndDateTime;
                        oSPD.Delivery_ModifiedDateTime = o.Delivery_ModifiedDateTime;
                        oSPD.Delivery_StartDateTime = o.Delivery_StartDateTime;
                        oSPD.DlvState = o.DlvState;
                        oSPD.DlvZipCode = o.DlvZipCode;
                        oSPD.Email = o.Email;
                        oSPD.EndDateTime = o.EndDateTime;
                        oSPD.MODIFIEDDATETIME = o.MODIFIEDDATETIME;
                        oSPD.Mxm1002InstallStatus = o.Mxm1002InstallStatus;
                        oSPD.Mxm1002ProgressStatus = o.Mxm1002ProgressStatus;
                        oSPD.MXM1002SequenceNr = o.MXM1002SequenceNr;
                        oSPD.MXM1002TrfDate = o.MXM1002TrfDate;
                        oSPD.MXMAlternativeContactMobileNo = o.MXMAlternativeContactMobileNo;
                        oSPD.MXMAlternativeContactName = o.MXMAlternativeContactName;
                        oSPD.MXMAlternativeContactTelNo = o.MXMAlternativeContactTelNo;
                        oSPD.MXMConfirmedAppointmentIndicator = o.MXMConfirmedAppointmentIndicator;
                        oSPD.MXMContactBySMS = o.MXMContactBySMS;
                        oSPD.MXMDisabledAdaptionsRequired = o.MXMDisabledAdaptionsRequired;
                        oSPD.MXMDoorChoiceFormReceived = o.MXMDoorChoiceFormReceived;
                        oSPD.MXMNextDaySMS = o.MXMNextDaySMS;
                        oSPD.MxmProjDescription = o.MxmProjDescription;
                        oSPD.MXMPropertyType = o.MXMPropertyType;
                        oSPD.MXMResidentMobileNo = o.MXMResidentMobileNo;
                        oSPD.MXMResidentName = o.MXMResidentName;
                        oSPD.MXMSMSSent = o.MXMSMSSent;
                        oSPD.MXMSpecialResidentNote = o.MXMSpecialResidentNote;
                        oSPD.MXMSurveyletterRequired = o.MXMSurveyletterRequired;
                        oSPD.MXMSurveyLetterSentDate01 = o.MXMSurveyLetterSentDate01;
                        oSPD.MXMSurveyLetterSentDate02 = o.MXMSurveyLetterSentDate02;
                        oSPD.MXMSurveyLetterSentDate03 = o.MXMSurveyLetterSentDate03;
                        oSPD.MXMSurveyorName = o.MXMSurveyorName;
                        oSPD.MXMSurveyorPCTag = o.MXMSurveyorPCTag;
                        oSPD.MXMSurveyorProfile = o.MXMSurveyorProfile;
                        oSPD.MXMTelephoneNo = o.MXMTelephoneNo;
                        oSPD.Name = o.Name;
                        oSPD.Notes = new ObservableCollection<NoteDetails>();
                        if (o.Notes != null)
                        {
                            foreach (WcfExt116.NoteDetails vo in o.Notes)
                            {
                                NoteDetails oND = new NoteDetails();
                                oND.AXRecID = vo.AXRecID;
                                oND.DeviceIDKey = vo.DeviceIDKey;
                                oND.InputDate = vo.InputDate;
                                oND.NoteText = vo.NoteText;
                                oND.NoteType = vo.NoteType;
                                oND.ProjectNo = vo.ProjectNo;
                                oND.Purpose = vo.Purpose;
                                oND.UserName = vo.UserName;
                                oND.UserProfile = vo.UserProfile;
                                oSPD.Notes.Add(oND);
                            }
                        }
                        oSPD.ParentID = o.ParentID;
                        oSPD.ProjId = o.ProjId;
                        oSPD.Purpose = o.Purpose;
                        oSPD.SMMActivities_MODIFIEDDATETIME = o.SMMActivities_MODIFIEDDATETIME;
                        oSPD.StartDateTime = o.StartDateTime;
                        oSPD.Status = o.Status;
                        oSPD.Units = new ObservableCollection<UnitDetails>();
                        if (o.Units != null)
                        {
                            foreach (WcfExt116.UnitDetails vo in o.Units)
                            {
                                UnitDetails oUD = new UnitDetails();
                                oUD.dInstalledDate = vo.dInstalledDate;
                                oUD.iInstalledStatus = vo.iInstalledStatus;
                                oUD.iUNITNUMBER = vo.iUNITNUMBER;
                                oUD.sITEMID = vo.sITEMID;
                                oUD.sSTYLE = vo.sSTYLE;
                                oUD.sUNITLOCATION = vo.sUNITLOCATION;
                                oSPD.Units.Add(oUD);
                            }
                        }
                        oSPD.URL = o.URL;
                        result.pdChanged.Add(oSPD);
                    }

                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UploadChangesResult> UploadSubProjectNotesChangesAsync(
            string v_sCompanyName, 
            string v_sPurpose, 
            string v_sUserName, 
            string v_sMachineName, 
            string v_sSubProjectNo, 
            ObservableCollection<NoteDetails> v_cNotes, 
            string v_sAuthID, 
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            UploadChangesResult result = new UploadChangesResult();
            try
            {
                ObservableCollection<WcfExt116.NoteDetails> cNotes = new ObservableCollection<WcfExt116.NoteDetails>();
                foreach (NoteDetails o in v_cNotes)
                {
                    WcfExt116.NoteDetails oND = new WcfExt116.NoteDetails();
                    oND.AXRecID = o.AXRecID;
                    oND.DeviceIDKey = o.DeviceIDKey;
                    oND.InputDate = o.InputDate;
                    oND.NoteText = o.NoteText;
                    oND.NoteType = o.NoteType;
                    oND.ProjectNo = o.ProjectNo;
                    oND.Purpose = o.Purpose;
                    oND.UserName = o.UserName;
                    oND.UserProfile = o.UserProfile;
                    cNotes.Add(oND);
                }
                WcfExt116.UploadChangesResult sResult = await m_wcfClient.UploadSubProjectNotesChangesAsync(
                    v_sCompanyName,
                    v_sPurpose,
                    v_sUserName,
                    v_sMachineName,
                    v_sSubProjectNo,
                    cNotes,
                    v_sAuthID,
                    v_sToken);
                if (sResult.bSuccessfull == true)
                {
                    result.bSuccessfull = sResult.bSuccessfull;
                    result.ActivitiesTable_ModDate = sResult.ActivitiesTable_ModDate;
                    result.ProjTable_ModDate = sResult.ProjTable_ModDate;
                    result.NoteValues = new ObservableCollection<RealtimeNoteKeyValues>();
                    foreach (WcfExt116.clsRealtimeNoteKeyValues o in sResult.NoteValues)
                    {
                        RealtimeNoteKeyValues oRNKV = new RealtimeNoteKeyValues();
                        oRNKV.DeviceIDKey = o.DeviceIDKey;
                        oRNKV.NotesRecID = o.NotesRecID;
                        oRNKV.ProjectNo = o.ProjectNo;
                        result.NoteValues.Add(oRNKV);
                    }

                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<SubProjectFileDownloadResult> ReturnSubProjectFileDownloadAsync(string v_sFileName, string v_sAuthID, string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            SubProjectFileDownloadResult result = new SubProjectFileDownloadResult();
            try
            {
                WcfExt116.SubProjectFileDownloadResult sResult = await m_wcfClient.ReturnSubProjectFileDownloadAsync(
                    v_sFileName, v_sAuthID, v_sToken);
                if (sResult.bSuccessfull == true)
                {
                    result.bSuccessfull = sResult.bSuccessfull;
                    result.bFileFound = sResult.bFileFound;
                    result.byFileData = new byte[sResult.byFileData.Length];
                    Array.Copy(sResult.byFileData, result.byFileData, sResult.byFileData.Length);
                    result.FileLastModDate = sResult.FileLastModDate;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UploadUnitsResult> UploadUnitInstallationStatusAsync(
            string v_sCompanyName, 
            string v_sSubProjectNo, 
            string v_sUserProfile, 
            string v_sMachineName, 
            DateTime v_dInstallationDate, 
            string v_sInstallationTeam, 
            ObservableCollection<UnitDetails> v_udUnits, 
            string v_sAuthID, 
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            UploadUnitsResult result = new UploadUnitsResult();
            try
            {
                ObservableCollection<WcfExt116.UnitDetails> udUnits = new ObservableCollection<WcfExt116.UnitDetails>();
                foreach (UnitDetails o in v_udUnits)
                {
                    WcfExt116.UnitDetails oUD = new WcfExt116.UnitDetails();
                    oUD.dInstalledDate = o.dInstalledDate;
                    oUD.iInstalledStatus = o.iInstalledStatus;
                    oUD.iUNITNUMBER = o.iUNITNUMBER;
                    oUD.sITEMID = o.sITEMID;
                    oUD.sSTYLE = o.sSTYLE;
                    oUD.sUNITLOCATION = o.sUNITLOCATION;
                    udUnits.Add(oUD);
                }
                WcfExt116.UploadUnitsResult sResult = await m_wcfClient.UploadUnitInstallationStatusAsync(
                    v_sCompanyName,
                    v_sSubProjectNo,
                    v_sUserProfile,
                    v_sMachineName,
                    v_dInstallationDate,
                    v_sInstallationTeam,
                    udUnits,
                    v_sAuthID,
                    v_sToken);
                if (sResult.bSuccessfull == true)
                {
                    result.bSuccessfull = sResult.bSuccessfull;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UploadChangesResult> UploadSubProjectDataChangesAsync(
            string v_sCompanyName, 
            string v_sPurpose, 
            string v_sUserName, 
            string v_sMachineName, 
            string v_sSubProjectNo, 
            ObservableCollection<AXDataUploadDataChange> v_cChanges, 
            string v_sAuthID, 
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            UploadChangesResult result = new UploadChangesResult();
            try
            {
                ObservableCollection<WcfExt116.cAXDataUploadDataChange> cChanges = new ObservableCollection<WcfExt116.cAXDataUploadDataChange>();
                foreach (AXDataUploadDataChange o in v_cChanges)
                {
                    WcfExt116.cAXDataUploadDataChange oAUDC = new WcfExt116.cAXDataUploadDataChange();
                    oAUDC.FieldName = o.FieldName;
                    oAUDC.FieldValue = o.FieldValue;
                    oAUDC.ProjectNo = o.ProjectNo;
                    cChanges.Add(oAUDC);
                }
                WcfExt116.UploadChangesResult sResult = await m_wcfClient.UploadSubProjectDataChangesAsync(
                    v_sCompanyName,
                    v_sPurpose,
                    v_sUserName,
                    v_sMachineName,
                    v_sSubProjectNo,
                    cChanges,
                    v_sAuthID,
                    v_sToken);
                if (sResult.bSuccessfull == true)
                {
                    result.bSuccessfull = sResult.bSuccessfull;
                    result.ActivitiesTable_ModDate = sResult.ActivitiesTable_ModDate;
                    result.ProjTable_ModDate = sResult.ProjTable_ModDate;
                    result.NoteValues = new ObservableCollection<RealtimeNoteKeyValues>();
                    foreach (WcfExt116.clsRealtimeNoteKeyValues o in sResult.NoteValues)
                    {
                        RealtimeNoteKeyValues oRNV = new RealtimeNoteKeyValues();
                        oRNV.DeviceIDKey = o.DeviceIDKey;
                        oRNV.NotesRecID = o.NotesRecID;
                        oRNV.ProjectNo = o.ProjectNo;
                        result.NoteValues.Add(oRNV);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<SubProjectFilesResult> ReturnSubProjectFilesAsync(
            string v_sCompanyName, 
            string v_sProjectNo, 
            string v_sAuthID, 
            string v_sToken)
        {
            m_wcfClient = new WcfExt116.ServiceClient();
            SubProjectFilesResult result = new SubProjectFilesResult();
            try
            {
                WcfExt116.SubProjectFilesResult sResult = await m_wcfClient.ReturnSubProjectFilesAsync(
                    v_sCompanyName,
                    v_sProjectNo,
                    v_sAuthID,
                    v_sToken);
                if (sResult.bSuccessfull == true)
                {
                    result.bSuccessfull = sResult.bSuccessfull;
                    result.pdSubProjectFiles = new ObservableCollection<SubProjectFile>();
                    foreach (WcfExt116.SubProjectFile o in sResult.pdSubProjectFiles)
                    {
                        SubProjectFile oSPF = new SubProjectFile();
                        oSPF.bFileDeleted = o.bFileDeleted;
                        oSPF.Comments = o.Comments;
                        oSPF.FileName = o.FileName;
                        oSPF.ModifiedDate = o.ModifiedDate;
                        result.pdSubProjectFiles.Add(oSPF);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
