using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Models;
using Anglian.Classes;
using Xamarin.Forms;
using Anglian.Service;
using Plugin.DeviceInfo;
using Plugin.DeviceInfo.Abstractions;

namespace Anglian.Engine
{
    public class Settings
    {
        public const string p_sSetting_AXCompany = "abpl";


        /// <summary>
        /// Authority ID for web calls.
        /// </summary>
        public const string p_sSetting_AuthID = "fCQLe6SD1WmS";

        /// <summary>
        /// Date comparison - equal to.
        /// </summary>
        public const string p_sDateCompare_EqualTo = "Equal To";

        /// <summary>
        /// Date comparison - less than
        /// </summary>
        public const string p_sDateCompare_LessThan = "Less Than";

        /// <summary>
        /// Date comparison - greater than
        /// </summary>
        public const string p_sDateCompare_GreaterThan = "Greater Than";

        /// <summary>
        /// Not surveyed, text to display.
        /// </summary>
        public static readonly string p_sSurveyedStatus_NotSurveyed = "Not Surveyed";

        /// <summary>
        /// Surveyed, text to display.
        /// </summary>
        public static readonly string p_sSurveyedStatus_SurveyedOnSite = "Surveyed on site";

        /// <summary>
        /// Surveyed, text to display.
        /// </summary>
        public static readonly string p_sSurveyedStatus_SurveyedTrans = "Surveyed Trans";

        /// <summary>
        /// Survey input status - successful.
        /// </summary>
        public const string p_sInputStatus_Successful = "Successful";

        /// <summary>
        /// Survey input status - failed.
        /// </summary>
        public const string p_sInputStatus_Failed = "Failed";

        /// <summary>
        /// Survey input status - pending.
        /// </summary>
        public const string p_sInputStatus_Pending = "Pending";

        /// <summary>
        /// v1.0.1 - Survey input status - Not pending.
        /// </summary>
        public const string p_sInputStatus_NotPending = "Not Pending";

        /// <summary>
        /// Support message
        /// </summary>
        public const string p_sSupportMessage = "If the problem persists please contact the I.T. service desk on 01603 420566 or email service.desk@angliangroup.com";

        /// <summary>
        /// v1.0.1 - Image Root folder.
        /// </summary>
        public const string p_sImageStoreRootFolderName = "SurveyorAppImageStore";


        /// <summary>
        /// Failed Install drop down value - Already PVC
        /// </summary>
        public const string p_sFailedInstall_Already_PVC = "Already PVC";

        /// <summary>
        /// Failed Install drop down value - Resident Refused
        /// </summary>      
        public const string p_sFailedInstall_Resident_Refused = "Resident Refused";

        /// <summary>
        /// Failed Install drop down value - No Access
        /// </summary>  
        public const string p_sFailedInstall_No_Access = "No Access";

        /// <summary>
        /// Failed Install drop down value - OtherO
        /// </summary>  
        public const string p_sFailedInstall_Other = "Other";

        /// <summary>
        /// v1.0.1 - Project number filter - Project No
        /// </summary>  
        public const string p_sProjectNoFilter_ProjectNo = "Project No";

        /// <summary>
        /// v1.0.1 - Project number filter - Sub project no
        /// </summary>  
        public const string p_sProjectNoFilter_SubProjectNo = "Sub Project No";


        /// <summary>
        /// v1.0.10 - Install status filter - Equal To
        /// </summary>  
        public const string p_sInstallStatusFilter_EqualTo = "Install Status EQ";

        /// <summary>
        /// v1.0.10 - Install status filter - Not Equal To
        /// </summary>  
        public const string p_sInstallStatusFilter_NotEqualTo = "Install Status NE";

        /// <summary>
        /// v1.0.1 - Project note type - general
        /// </summary>
        public const string p_sProjectNoteType_General = "GENERAL";

        /// <summary>
        /// v1.0.1 - Project note type - survey failed
        /// </summary>
        public const string p_sProjectNoteType_SurveyFailed = "SURVEYFAILED";

        /// <summary>
        /// v1.0.10 - Project note type - partial install
        /// </summary>
        public const string p_sProjectNoteType_PartialInstall = "PARTIALINSTALL";

        /// <summary>
        /// v1.0.16 - Project note type - full install
        /// </summary>
        public const string p_sProjectNoteType_FullInstall = "FULLINSTALL";

        /// <summary>
        /// v1.0.10 - Project note type - install rebook.
        /// </summary>
        public const string p_sProjectNoteType_InstallReBook = "INSTALLREBOOK";

        /// <summary>
        /// Note it drop down option - None
        /// </summary>        
        public const string p_sNoteIt_None = "None";

        /// <summary>
        /// Note it drop down option - No Access Carded
        /// </summary>
        public const string p_sNoteIt_Carded = "No Access – Carded";

        /// <summary>
        /// Note it drop down option - Already PVC
        /// </summary>
        public const string p_sNoteIt_AlreadyPVC = "Already PVC";

        /// <summary>
        /// Note it drop down option - Omit RTB
        /// </summary>
        public const string p_sNoteIt_OmitRTB = "Omit RTB";

        /// <summary>
        /// Note it drop down option - Omitted
        /// </summary>
        public const string p_sNoteIt_Omitted = "Omitted";

        /// <summary>
        /// Note it drop down option - Other
        /// </summary>
        public const string p_sNoteIt_Other = "Other";

        /// <summary>
        /// Name of the signature picture.
        /// </summary>
        public static readonly string p_sSignaturePicName = "Signature.jpg";

        /// <summary>
        /// Name of the profile picture.
        /// </summary>
        public static readonly string p_sProfilePicName = "ProfilePic.jpg";


        /// <summary>
        /// Confirmed, text to display.
        /// </summary>
        public static readonly string p_sConfirmedStatus_Yes = "Yes";

        /// <summary>
        /// Confirmed, text to display.
        /// </summary>
        public static readonly string p_sConfirmedStatus_No = "No";

        /// <summary>
        /// Any, text to display.
        /// </summary>
        public const string p_sAnyStatus = "Any";

        /// <summary>
        /// Generic Please Choose text.
        /// </summary>
        public const string p_sPleaseChoose = "Please Choose";

        /// <summary>
        /// Generic Not Required text.
        /// </summary>
        public const string p_sNotRequired = "Not Required";

        /// <summary>
        /// Set survey dates time picker drop down options.
        /// </summary>
        public const string p_sTime_AM = "AM";

        /// <summary>
        /// Set survey dates time picker drop down options.
        /// </summary>
        public const string p_sTime_PM = "PM";

        /// <summary>
        /// Set survey dates time picker drop down options.
        /// </summary>
        public const string p_sTime_Specific = "Specific Time";

        /// <summary>
        /// Units installed status
        /// </summary>
        public const int p_iUnits_InstalledStatus = 5;

        /// <summary>
        /// Installation - Install status - fully installed
        /// </summary>
        public const int p_iInstallStatus_InstalledFully = 11;

        /// <summary>
        /// Installation - Install status - installed part
        /// </summary>
        public const int p_iInstallStatus_InstalledPart = 10;

        /// <summary>
        /// Installation - Install status - installing
        /// </summary>
        public const int p_iInstallStatus_Installing = 8;

        /// <summary>
        /// v1.0.10 - Background list view color for an partially installed sub project.
        /// </summary>
        public const string p_sInstall_ListView_PartialInstall_Background = "#800080";

        /// <summary>
        /// v1.0.10 - Background list view color.
        /// </summary>
        public const string p_sInstall_ListView_Normal_Background = "#FF3A3838";


        /// <summary>
        /// v1.0.12 - Background list view colour for sub project on hold.
        /// </summary>
        public const string p_sSurvey_ListView_OnHold_Background = "#FFA80505";

        /// <summary>
        /// v1.0.12 - Background list view colour.
        /// </summary>
        public const string p_sSurvey_ListView_Normal_Background = "#FF3A3838";

        /// <summary>
        /// Colour to display for installation if on hold and partial install.
        /// </summary>
        public const string p_sInstall_ListView_OnHold_PartInstall_Background = "#FFC998EC";

        /// <summary>
        /// v1.0.12 - Project status - On Hold
        /// </summary>
        public const int p_iProjectStatus_OnHold = 5;

        /// <summary>
        /// v1.0.19 - Default database date.
        /// </summary>
        public static DateTime p_dDefaultDBDate = new DateTime(1900, 1, 1, 0, 0, 0);

        /// <summary>
        /// Yes no base enum values.
        /// </summary>
        public enum YesNoBaseEnum
        {

            No = 0
            , Yes = 1
        }


        /// <summary>
        /// Abort retry cancel enumerator
        /// </summary>
        public enum AbortRetryIgnore
        {
            Abort = 1
            ,
            Retry = 2
                , Ignore = 3
        }


        /// <summary>
        /// v1.0.2 - Yes No enumerator
        /// </summary>
        public enum YesNo
        {
            Yes = 1
            , No = 2

        }


        /// <summary>
        /// v1.0.2 - Yes No Cancel enumerator
        /// </summary>
        public enum YesNoCancel
        {
            Yes = 1
            ,
            No = 2
                , Cancel = 3

        }

        /// <summary>
        /// Structure for return data after applying survey date changes.
        /// </summary>
        public struct SurveyDatesApply
        {

            public List<cUpdatesTable> cUpdates;
            public cProjectTable cProjectData;

        }
        public static string GetMachineName()
        {
            return CrossDeviceInfo.Current.Model;
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v_sSearchText"></param>
        /// <returns></returns>
        public static string AddWildCardsToSearchString(string v_sSearchText)
        {

            try
            {

                //Add wild card to start if not already
                string sStart = string.Empty;
                if (v_sSearchText.StartsWith("*") == false)
                {
                    sStart = "*";
                }

                //Add wild card to end if not already
                string sEnd = string.Empty;
                if (v_sSearchText.EndsWith("*") == false)
                {
                    sEnd = "*";
                }

                //Put return string together.
                return sStart + v_sSearchText + sEnd;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - (" + v_sSearchText + ")");

            }

        }

        /// <summary>
        /// Return purpose type
        /// </summary>
        /// <returns></returns>
        public static string ReturnPurposeType()
        {

            try
            {

                if (DependencyService.Get<ISettings>().IsThisTheSurveyorApp() == true)
                {
                    return "Survey";

                }

                return "Install";

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }
        public async Task<bool> IsAXSystemAvailable(bool v_bShowMessageOnFailedConnection)
        {

            WcfExt116 cAX = null;
            SystemsAvailableResult rResult = null;
            string sSupportMsg = string.Empty;
            bool bConnected = true;

            try
            {


                try
                {
                    cAX = new WcfExt116();
                    rResult = await cAX.AreWeConnectedToAX();

                }
                catch (Exception ex)
                {
                    //We do not need to log.

                    //
                }

                if (cAX != null)
                {
                    await DependencyService.Get<IWcfExt116>().CloseAXConnection();
                }

                bool bDisplayNoConnection = false;

                if (rResult == null)
                {
                    bDisplayNoConnection = true;
                }
                else if (rResult.bSuccessfull == false)
                {
                    bDisplayNoConnection = true;
                }
                else if (rResult.SystemsAvailable == false)
                {

                    bConnected = false;

                    if (rResult.UserAccountOK == false) //v1.0.2 - Display different message for account disabled.
                    {


                        if (v_bShowMessageOnFailedConnection == true)
                        {
                            sSupportMsg = Settings.ReturnDisabledAccountMessage();
                            await DependencyService.Get<ISettings>().DisplayMessage(sSupportMsg, "Failed Connection");

                        }

                    }
                    else
                    {
                        bDisplayNoConnection = true;
                    }


                }



                //If we cannot connect and we need to display a message.
                if (bDisplayNoConnection == true)
                {

                    bConnected = false;

                    //Only display if flagged to.
                    if (v_bShowMessageOnFailedConnection == true)
                    {

                        sSupportMsg = Settings.ReturnNoConnectionMessage();

                        sSupportMsg += Environment.NewLine + Environment.NewLine;

                        sSupportMsg += Settings.p_sSupportMessage;

                        await DependencyService.Get<ISettings>().DisplayMessage(sSupportMsg, "No Connection");


                    }

                }


                return bConnected;


            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);

            }


        }
        /// <summary>
        /// Return a string object from a string that could be null.
        /// </summary>
        /// <param name="v_sString"></param>
        /// <returns></returns>
        public static string ReturnString(String v_sString)
        {

            try
            {
                if (string.IsNullOrEmpty(v_sString) == true)
                {
                    return String.Empty;
                }

                return v_sString.Trim();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - String(" + v_sString + ")");

            }

        }

        /// <summary>
        /// Return no connection message for displaying to users.
        /// </summary>
        /// <returns></returns>
        public static string ReturnNoConnectionMessage()
        {

            try
            {

                StringBuilder sbMessage = new StringBuilder();
                sbMessage.Append("A connection to the ABP head office system cannot be established, this could be for one of the following reasons:");
                sbMessage.Append(Environment.NewLine);
                sbMessage.Append(Environment.NewLine);
                sbMessage.Append("1. Your device is not connected to the Internet.");
                sbMessage.Append(Environment.NewLine);
                sbMessage.Append("2. There is a problem with the ABP back end system.");

                return sbMessage.ToString();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }
        /// <summary>
        /// v1.0.2 - Return disabled user account message.
        /// </summary>
        /// <returns></returns>
        public static string ReturnDisabledAccountMessage()
        {

            try
            {

                StringBuilder sbMessage = new StringBuilder();
                sbMessage.Append("A connection to head office has been established but your user account is disabled.");
                sbMessage.Append(Environment.NewLine);
                sbMessage.Append(Environment.NewLine);
                sbMessage.Append(Settings.p_sSupportMessage);


                return sbMessage.ToString();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }
        public static cProjectNotesTable ReturnNoteObject(string v_sSubProjectNo, string v_sNoteText, DateTime v_dNoteDate, string v_sNoteType)
        {

            cProjectNotesTable cNote = new cProjectNotesTable();
            try
            {


                cNote.AXRecID = -1;
                cNote.IDKey = -1;
                cNote.InputDateTime = v_dNoteDate;
                cNote.NoteText = v_sNoteText;
                cNote.NoteType = v_sNoteType;
                cNote.SubProjectNo = v_sSubProjectNo;
                cNote.UserName = Session.CurrentUserName;
                cNote.UserProfile = Session.CurrentUserName;

                return cNote;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// Add update to updates list and return. 
        /// Used for logging changes, saves a lot of extra repetitive coding.
        /// </summary>
        /// <param name="v_cUpdate"></param>
        /// <param name="v_sSubProjectNo"></param>
        /// <param name="v_sFieldName"></param>
        /// <param name="v_sFieldValue"></param>
        /// <returns></returns>
        public static List<cUpdatesTable> AddToUpdatesList(List<cUpdatesTable> v_cUpdates, string v_sSubProjectNo, string v_sFieldName, string v_sFieldValue)
        {

            try
            {

                //If not set then create new.
                if (v_cUpdates == null)
                {
                    v_cUpdates = new List<cUpdatesTable>();
                }

                //Create single entry.
                cUpdatesTable cUpdate = new cUpdatesTable();
                cUpdate.SubProjectNo = v_sSubProjectNo;
                cUpdate.FieldName = v_sFieldName;
                cUpdate.FieldValue = v_sFieldValue;

                //Add to collection
                v_cUpdates.Add(cUpdate);

                //Return
                return v_cUpdates;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - PARAMS(SubProjectNo=" + v_sSubProjectNo + ",FieldName=" + v_sFieldName + ",FieldValue=" + v_sFieldValue + ")");

            }
        }
        /// <summary>
        /// Apply survey dates to passed to passed objects.
        /// </summary>
        /// <param name="v_cProjectData"></param>
        /// <param name="v_cUpdates"></param>
        /// <returns></returns>
        public static Settings.SurveyDatesApply ApplySurveyDates(cProjectTable v_cProjectData, List<cUpdatesTable> v_cUpdates, DateTime v_dSurveyDate, string v_sSubProjectNo, string v_sSurveyorName, string v_sSurveyorProfile, string v_sMachineName)
        {

            Settings.SurveyDatesApply cReturn = new Settings.SurveyDatesApply();
            try
            {

                cReturn.cProjectData = v_cProjectData;
                cReturn.cUpdates = v_cUpdates;

                //AX sets the time one hour forward, so we take one hour off the date we save in the updates table for uploading to AX
                DateTime dAXAdjDateTime = v_dSurveyDate; //.AddHours(-1); v1.0.9 JM removed the add 1 hour from the AX date

                //Update class
                cReturn.cProjectData.MxmConfirmedAppointmentIndicator = (int)Settings.YesNoBaseEnum.Yes;
                cReturn.cUpdates = Settings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "MxmConfirmedAppointmentIndicator", ((int)Settings.YesNoBaseEnum.Yes).ToString());

                cReturn.cProjectData.EndDateTime = v_dSurveyDate;
                cReturn.cUpdates = Settings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "StartDateTime", dAXAdjDateTime.ToString());

                cReturn.cProjectData.StartDateTime = v_dSurveyDate;
                cReturn.cUpdates = Settings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "EndDateTime", dAXAdjDateTime.ToString());

                cReturn.cProjectData.SurveyorName = v_sSurveyorName;
                cReturn.cUpdates = Settings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "MxmSurveyorName", v_sSurveyorName);

                cReturn.cProjectData.SurveyorProfile = v_sSurveyorProfile;
                cReturn.cUpdates = Settings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "MxmSurveyorProfile", v_sSurveyorProfile);

                cReturn.cProjectData.SurveyorPCTag = v_sMachineName;
                cReturn.cUpdates = Settings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "MxmSurveyorPCTag", v_sMachineName);

                if (DependencyService.Get<ISettings>().IsThisTheSurveyorApp() == true)
                {


                    cReturn.cProjectData.MxmSurveyletterRequired = (int)Settings.YesNoBaseEnum.Yes;
                    cReturn.cUpdates = Settings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "MxmSurveyletterRequired", ((int)Settings.YesNoBaseEnum.Yes).ToString());

                    cReturn.cProjectData.MxmSMSSent = (int)Settings.YesNoBaseEnum.No;
                    cReturn.cUpdates = Settings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "MxmSMSSent", ((int)Settings.YesNoBaseEnum.No).ToString());

                    cReturn.cProjectData.MxmNextDaySMS = (int)Settings.YesNoBaseEnum.No;
                    cReturn.cUpdates = Settings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "MxmNextDaySMS", ((int)Settings.YesNoBaseEnum.No).ToString());

                }
                else
                {

                    cReturn.cProjectData.MxmSurveyletterRequired = (int)Settings.YesNoBaseEnum.Yes;
                    cReturn.cUpdates = Settings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "ABPAXInstallletterRequired", ((int)Settings.YesNoBaseEnum.Yes).ToString());

                    cReturn.cProjectData.MxmSMSSent = (int)Settings.YesNoBaseEnum.No;
                    cReturn.cUpdates = Settings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "ABPAXInstallSMSSent", ((int)Settings.YesNoBaseEnum.No).ToString());

                    cReturn.cProjectData.MxmNextDaySMS = (int)Settings.YesNoBaseEnum.No;
                    cReturn.cUpdates = Settings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "ABPAXInstallNextDaySMS", ((int)Settings.YesNoBaseEnum.No).ToString());

                    //v1.0.10 - Set the confirmed action date.
                    cReturn.cProjectData.ConfirmedActionDateTime = DateTime.Now;

                }

                //Return populated object.
                return cReturn;

            }
            catch (Exception ex)
            {

                StringBuilder sbParams = new StringBuilder();

                sbParams.Append("SurveyDate=" + v_dSurveyDate.ToString());
                sbParams.Append(",SubProjectNo=" + v_sSubProjectNo);
                sbParams.Append(",SurveyorName=" + v_sSurveyorName);
                sbParams.Append(",SurveyorProfile=" + v_sSurveyorProfile);
                sbParams.Append(",MachineName=" + v_sMachineName);

                throw new Exception(ex.Message + " - PARAMS(" + sbParams.ToString() + ")");

            }

        }
    }
}
