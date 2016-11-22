using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ABP.TableModels;
using ABP.Interfaces;

namespace ABP.WcfProxys
{
    public class cSettings
    {
        ///// <summary>
        ///// Event handler for displaying messages
        ///// </summary>
        //public event EventHandler<string> DisplayMessage;

        /// <summary>
        /// AX Company
        /// </summary>
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

        public static bool IsThisTheSurveyorApp()
        {
            try
            {
                //string sAppName = Windows.ApplicationModel.Package.Current.DisplayName;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public static bool AreWeOnline
        {
            get
            {
                return DependencyService.Get<ISettings>().AreWeOnline();
            }
        }
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

    }
}
