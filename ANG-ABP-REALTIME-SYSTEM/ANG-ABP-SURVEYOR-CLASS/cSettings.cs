using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANG_ABP_SURVEYOR_APP_CLASS.Classes;
using ANG_ABP_SURVEYOR_APP_CLASS.Model;
using Windows.Networking;
using Windows.Networking.Connectivity;
using ANG_ABP_SURVEYOR_APP_CLASS.wcfCalls;
using Windows.Storage;
using System.IO;
using Windows.Storage.Streams;
using Windows.UI.Popups;

namespace ANG_ABP_SURVEYOR_APP_CLASS
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
        public const string p_sSetting_AXCompany= "abpl";


        /// <summary>
        /// Authority ID for web calls.
        /// </summary>
        public const string p_sSetting_AuthID = "eCQLH6SDmWFS";

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

        /// <summary>
        /// Checks if we are connected to the Internet.
        /// </summary>
        public static bool AreWeOnline
        {
            get
            {
                return NetworkInformation.GetInternetConnectionProfile() != null;
            }
        }

        /// <summary>
        /// Returns the logged on users name.
        /// </summary>
        /// <returns></returns>
        public async static Task<string> GetUserName()
        {

            try
            {

                string sUserName = await Windows.System.UserProfile.UserInformation.GetDomainNameAsync();
                string[] sParts = sUserName.Split('\\');

                if (sParts.Length > 1)
                {
                    return sParts[1].ToUpper();

                }

                return await Windows.System.UserProfile.UserInformation.GetDisplayNameAsync();

            }
            catch (Exception ex)
            {
                return ex.Message;

            }

        }

        /// <summary>
        /// Get logged on users display name.
        /// </summary>
        /// <returns></returns>
        public async static Task<string> GetUserDisplayName()
        {

            try
            {

                return await Windows.System.UserProfile.UserInformation.GetDisplayNameAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        /// <summary>
        /// Return the name of the machine.
        /// </summary>
        /// <returns></returns>
        public static string GetMachineName()
        {

            try
            {

                
                

                IReadOnlyList<HostName> HostNames = NetworkInformation.GetHostNames();
                foreach (HostName hsName in HostNames)
                {

                    if (hsName.Type == HostNameType.DomainName)
                    {

                        string[] sNameParts = hsName.RawName.Split('.');
                        return sNameParts[0];

                    }

                }

                return "N\\A";

            }
            catch (Exception ex)
            {
                return string.Empty;

            }

        }

        /// <summary>
        /// Apply survey dates to passed to passed objects.
        /// </summary>
        /// <param name="v_cProjectData"></param>
        /// <param name="v_cUpdates"></param>
        /// <returns></returns>
        public static cSettings.SurveyDatesApply ApplySurveyDates(cProjectTable v_cProjectData, List<cUpdatesTable> v_cUpdates, DateTime v_dSurveyDate, string v_sSubProjectNo, string v_sSurveyorName, string v_sSurveyorProfile, string v_sMachineName)
        {

            cSettings.SurveyDatesApply cReturn = new cSettings.SurveyDatesApply();
            try
            {

                cReturn.cProjectData = v_cProjectData;
                cReturn.cUpdates = v_cUpdates;

                //AX sets the time one hour forward, so we take one hour off the date we save in the updates table for uploading to AX
                DateTime dAXAdjDateTime = v_dSurveyDate; //.AddHours(-1); v1.0.9 JM removed the add 1 hour from the AX date

                //Update class
                cReturn.cProjectData.MxmConfirmedAppointmentIndicator = (int)cSettings.YesNoBaseEnum.Yes;
                cReturn.cUpdates = cSettings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "MxmConfirmedAppointmentIndicator", ((int)cSettings.YesNoBaseEnum.Yes).ToString());

                cReturn.cProjectData.EndDateTime = v_dSurveyDate;
                cReturn.cUpdates = cSettings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "StartDateTime", dAXAdjDateTime.ToString());

                cReturn.cProjectData.StartDateTime = v_dSurveyDate;
                cReturn.cUpdates = cSettings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "EndDateTime", dAXAdjDateTime.ToString());

                cReturn.cProjectData.SurveyorName = v_sSurveyorName;
                cReturn.cUpdates = cSettings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "MxmSurveyorName", v_sSurveyorName);

                cReturn.cProjectData.SurveyorProfile = v_sSurveyorProfile;
                cReturn.cUpdates = cSettings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "MxmSurveyorProfile", v_sSurveyorProfile);

                cReturn.cProjectData.SurveyorPCTag = v_sMachineName;
                cReturn.cUpdates = cSettings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "MxmSurveyorPCTag", v_sMachineName);   

                if (cSettings.IsThisTheSurveyorApp() == true) 
                { 
             

                    cReturn.cProjectData.MxmSurveyletterRequired = (int)cSettings.YesNoBaseEnum.Yes;
                    cReturn.cUpdates = cSettings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "MxmSurveyletterRequired", ((int)cSettings.YesNoBaseEnum.Yes).ToString());

                    cReturn.cProjectData.MxmSMSSent = (int)cSettings.YesNoBaseEnum.No;
                    cReturn.cUpdates = cSettings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "MxmSMSSent", ((int)cSettings.YesNoBaseEnum.No).ToString());

                    cReturn.cProjectData.MxmNextDaySMS = (int)cSettings.YesNoBaseEnum.No;
                    cReturn.cUpdates = cSettings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "MxmNextDaySMS", ((int)cSettings.YesNoBaseEnum.No).ToString());

                }
                else
                {

                    cReturn.cProjectData.MxmSurveyletterRequired = (int)cSettings.YesNoBaseEnum.Yes;
                    cReturn.cUpdates = cSettings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "ABPAXInstallletterRequired", ((int)cSettings.YesNoBaseEnum.Yes).ToString());

                    cReturn.cProjectData.MxmSMSSent = (int)cSettings.YesNoBaseEnum.No;
                    cReturn.cUpdates = cSettings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "ABPAXInstallSMSSent", ((int)cSettings.YesNoBaseEnum.No).ToString());

                    cReturn.cProjectData.MxmNextDaySMS = (int)cSettings.YesNoBaseEnum.No;
                    cReturn.cUpdates = cSettings.AddToUpdatesList(cReturn.cUpdates, v_sSubProjectNo, "ABPAXInstallNextDaySMS", ((int)cSettings.YesNoBaseEnum.No).ToString());

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
        /// Check too see if AX system is available.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsAXSystemAvailable(bool v_bShowMessageOnFailedConnection)
        {

            cAXCalls cAX = null;            
            wcfAX.SystemsAvailableResult rResult = null;
            string sSupportMsg = string.Empty;
            bool bConnected = true;

            try
            {


                try
                {
                    cAX = new cAXCalls();
                    rResult = await cAX.AreWeConnectedToAX();

                }
                catch (Exception ex)
                {
                    //We do not need to log.

                    //
                }

                if (cAX != null)
                {
                    await cAX.CloseAXConnection();
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
                            sSupportMsg = cSettings.ReturnDisabledAccountMessage();
                            await cSettings.DisplayMessage(sSupportMsg,"Failed Connection");

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

                        sSupportMsg = cSettings.ReturnNoConnectionMessage();

                        sSupportMsg += Environment.NewLine + Environment.NewLine;

                        sSupportMsg += cSettings.p_sSupportMessage;

                        await cSettings.DisplayMessage(sSupportMsg,"No Connection");


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
                sbMessage.Append(cSettings.p_sSupportMessage);
                

                return sbMessage.ToString();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        /// <summary>
        /// Return sub project storage file.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <param name="v_sFileName"></param>
        /// <returns></returns>
        public static async Task<StorageFile> ReturnStorageFileForSubProject(string v_sSubProjectNo, string v_sFileName)
        {

            StorageFile sfReturn = null;
            try
            {

                StorageFolder sfSubProject = await cSettings.ReturnSubProjectImagesFolder(v_sSubProjectNo);
                if (sfSubProject != null)
                {
                    sfReturn = await cSettings.ReturnStorageFile(sfSubProject, v_sFileName);

                }

                return sfReturn;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SubProjectNo(" + v_sSubProjectNo + "),FileName(" + v_sFileName + ")");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async static Task<StorageFolder> ReturnImageRooFolder()
        {

            StorageFolder sfRootFolder = null;
            try
            {

                string sFolderName = cSettings.p_sImageStoreRootFolderName;

                IStorageItem siItem = await Windows.Storage.KnownFolders.PicturesLibrary.TryGetItemAsync(sFolderName);
                if (siItem == null)
                {
                    sfRootFolder = await Windows.Storage.KnownFolders.PicturesLibrary.CreateFolderAsync(sFolderName);

                }
                else
                {
                    sfRootFolder = await Windows.Storage.KnownFolders.PicturesLibrary.GetFolderAsync(sFolderName);

                }
               
                return sfRootFolder;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// Return sub project image folder.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public async static Task<StorageFolder> ReturnSubProjectImagesFolder(string v_sSubProjectNo)
        {

            StorageFolder sfProject = null;
            try
            {
                //Retrieve the image root folder.
                StorageFolder sfRoot = await cSettings.ReturnImageRooFolder();
                if (sfRoot != null)
                {

                    //Check if folder exists
                    IStorageItem siItem = await sfRoot.TryGetItemAsync(v_sSubProjectNo);
                    if (siItem == null)
                    {
                        sfProject = await sfRoot.CreateFolderAsync(v_sSubProjectNo);

                    }
                    else
                    {
                        sfProject = await sfRoot.GetFolderAsync(v_sSubProjectNo);

                    }


                    return sfProject;

                }

                return sfProject;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - SubProjectNo(" + v_sSubProjectNo + ")");

            }

        }

        /// <summary>
        /// Loads the byte data from a StorageFile
        /// </summary>
        /// <param name="file">The file to read</param>
        public static async Task<byte[]> ConvertFileToByteArray(StorageFile file)
        {

            try
            {

                byte[] fileBytes = null;
                using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
                {
                    fileBytes = new byte[stream.Size];
                    using (DataReader reader = new DataReader(stream))
                    {
                        await reader.LoadAsync((uint)stream.Size);
                        reader.ReadBytes(fileBytes);

                    }
                }
                return fileBytes;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// Return storage file.
        /// </summary>
        /// <param name="v_sfFolder"></param>
        /// <param name="v_sFileName"></param>
        /// <returns></returns>
        public async static Task<StorageFile> ReturnStorageFile(StorageFolder v_sfFolder, string v_sFileName)
        {
            try
            {

                //Check if signature file exists
                IStorageItem siItem = await v_sfFolder.TryGetItemAsync(v_sFileName);
                if (siItem != null)
                {
                    return await v_sfFolder.GetFileAsync(v_sFileName);

                }

                return null;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - FILENAME(" + v_sFileName + ")");

            }

        }

        /// <summary>
        /// Save file locally.
        /// </summary>
        /// <param name="v_sfFolder"></param>
        /// <param name="v_bFileData"></param>
        /// <param name="v_sFileName"></param>
        /// <returns></returns>
        public async static Task<bool> SaveFileLocally(string v_sSubProjectNo, byte[] v_bFileData, string v_sFileName)
        {

            try
            {

                //Fetch sub project image folder.
                StorageFolder sfProject = await cSettings.ReturnSubProjectImagesFolder(v_sSubProjectNo);

                //Save file.
                return await cSettings.SaveFileLocally(sfProject, v_bFileData, v_sFileName);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - FileName(" + v_sFileName + ")");

            }

        }

        /// <summary>
        /// Save file locally.
        /// </summary>
        /// <param name="v_sfFolder"></param>
        /// <param name="v_bFileData"></param>
        /// <param name="v_sFileName"></param>
        /// <returns></returns>
        public async static Task<bool> SaveFileLocally(StorageFolder v_sfFolder, byte[] v_bFileData, string v_sFileName)
        {

            try
            {

                //Save file data to local file.
                using (Stream f = await v_sfFolder.OpenStreamForWriteAsync(v_sFileName, CreationCollisionOption.ReplaceExisting))
                {
                    f.Seek(0, SeekOrigin.End);
                    await f.WriteAsync(v_bFileData, 0, v_bFileData.Length);
                }

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - FileName(" + v_sFileName + ")");

            }
        }

        /// <summary>
        /// v1.0.8 - Check which application is calling the class.
        /// </summary>
        /// <returns></returns>
        public static bool IsThisTheSurveyorApp()
        {
            try
            {

                string sAppName = Windows.ApplicationModel.Package.Current.DisplayName;
                if (sAppName.Equals("ANG-ABP-INSTALLER-APP") == true)
                {
                    return false;
                }
                else if (sAppName.Equals("ANG-ABP-SURVEYOR-APP") == true)
                {
                    return true;

                }
                else
                {
                    throw new Exception("Unidentified application name (" + sAppName + ")");
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

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

                if (cSettings.IsThisTheSurveyorApp() == true)
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

        /// <summary>
        /// Display message, common routine.
        /// </summary>
        /// <param name="v_sMessage"></param>
        /// <param name="v_sTitle"></param>
        public async static Task DisplayMessage(string v_sMessage, string v_sTitle)
        {

            try
            {
                MessageDialog mdMessage = new MessageDialog(v_sMessage, v_sTitle);
                await mdMessage.ShowAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " MESSAGE(" + v_sMessage + ") TITLE(" + v_sTitle + ")");

            }

        }

        /// <summary>
        /// v1.0.2 - Display Yes No.
        /// </summary>
        /// <param name="v_sMessage"></param>
        /// <param name="v_sTitle"></param>
        /// <returns></returns>
        public async static Task<cSettings.YesNo> DisplayYesNo(string v_sMessage, string v_sTitle)
        {
            cSettings.YesNo mResponse = YesNo.No;
            try
            {
                MessageDialog mdMessage = new MessageDialog(v_sMessage, v_sTitle);

                UICommand ucYes = new UICommand("Yes", (UICommandInvokedHandler) => { mResponse = YesNo.Yes; });
                mdMessage.Commands.Add(ucYes);

                UICommand ucNo = new UICommand("No", (UICommandInvokedHandler) => { mResponse = YesNo.No; });
                mdMessage.Commands.Add(ucNo);

                await mdMessage.ShowAsync();

                return mResponse;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " MESSAGE(" + v_sMessage + ") TITLE(" + v_sTitle + ")");
            }

        }

        /// <summary>
        /// v1.0.2 - Display Yes No Cancel.
        /// </summary>
        /// <param name="v_sMessage"></param>
        /// <param name="v_sTitle"></param>
        /// <returns></returns>
        public async static Task<cSettings.YesNoCancel> DisplayYesNoCancel(string v_sMessage, string v_sTitle)
        {
            cSettings.YesNoCancel mResponse = YesNoCancel.No;
            try
            {
                MessageDialog mdMessage = new MessageDialog(v_sMessage, v_sTitle);

                UICommand ucYes = new UICommand("Yes", (UICommandInvokedHandler) => { mResponse = YesNoCancel.Yes; });
                mdMessage.Commands.Add(ucYes);

                UICommand ucNo = new UICommand("No", (UICommandInvokedHandler) => { mResponse = YesNoCancel.No; });
                mdMessage.Commands.Add(ucNo);

                UICommand ucCancel = new UICommand("Cancel", (UICommandInvokedHandler) => { mResponse = YesNoCancel.Cancel; });
                mdMessage.Commands.Add(ucCancel);

                await mdMessage.ShowAsync();

                return mResponse;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " MESSAGE(" + v_sMessage + ") TITLE(" + v_sTitle + ")");
            }

        }


        /// <summary>
        /// v1.0.2 - Prompt user to save changes.
        /// </summary>
        /// <returns></returns>
        public async static Task<cSettings.YesNoCancel> PromptForUnSavedChanges()
        {
           
            try
            {
                StringBuilder sbMessage = new StringBuilder();
                sbMessage.Append("You have made changes that have not been saved, would you like to save now?");

                return await cSettings.DisplayYesNoCancel(sbMessage.ToString(), "Save Changes");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        /// <summary>
        /// Display abort retry ignore.
        /// </summary>
        /// <param name="v_sMessage"></param>
        /// <param name="v_sTitle"></param>
        /// <returns></returns>
        public async static Task<cSettings.AbortRetryIgnore> DisplayAbortRetryIgnore(string v_sMessage, string v_sTitle)
        {
            cSettings.AbortRetryIgnore mResponse = AbortRetryIgnore.Abort;
            try
            {
                MessageDialog mdMessage = new MessageDialog(v_sMessage, v_sTitle);

                UICommand ucAbort = new UICommand("Abort", (UICommandInvokedHandler) => { mResponse = AbortRetryIgnore.Abort; });
                mdMessage.Commands.Add(ucAbort);

                UICommand ucRetry = new UICommand("Retry", (UICommandInvokedHandler) => { mResponse = AbortRetryIgnore.Retry; });
                mdMessage.Commands.Add(ucRetry);

                UICommand ucIgnore = new UICommand("Ignore", (UICommandInvokedHandler) => { mResponse = AbortRetryIgnore.Ignore; });
                mdMessage.Commands.Add(ucIgnore);

                await mdMessage.ShowAsync();

                return mResponse;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " MESSAGE(" + v_sMessage + ") TITLE(" + v_sTitle + ")");
            }

        }

        /// <summary>
        /// v1.0.1 - Return new notes object.
        /// </summary>
        /// <param name="v_sNoteText"></param>
        /// <returns></returns>
        public async static Task<cProjectNotesTable> ReturnNoteObject(string v_sSubProjectNo, string v_sNoteText, DateTime v_dNoteDate, string v_sNoteType)
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
                cNote.UserName = await cSettings.GetUserDisplayName();
                cNote.UserProfile = await cSettings.GetUserName();

                return cNote;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// Make thread sleep.
        /// </summary>
        /// <param name="v_iMilliseconds"></param>
        public static void Sleep(int v_iMilliseconds)
        {

            try
            {

                new System.Threading.ManualResetEvent(false).WaitOne(v_iMilliseconds);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// v1.0.18 - Fetch setting value, has to created DB instance first.
        /// </summary>
        /// <param name="v_sSettingName"></param>
        /// <returns></returns>
        public static string FetchSettingValue(string v_sSettingName)
        {

            cDataAccess cData = null;
            try
            {

                cData = new cDataAccess();
                cData.CheckDB();
                return cData.GetSettingValue(v_sSettingName);                

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - v_sSettingName=" + v_sSettingName);

            }
            finally
            {
                if (cData != null)
                {
                    cData.CleanUp();
                }
            }
        }




        /// <summary>
        /// Delete sub project images folder from device.
        /// </summary>
        /// <param name="v_sSubProjectNo"></param>
        /// <returns></returns>
        public async static Task<bool> DeleteSubProjectImageFolder(string v_sSubProjectNo)
        {

            try
            {
                //Fetch image root folder.
                StorageFolder sfRoot = await cSettings.ReturnImageRooFolder();

                //See if folder exists.
                IStorageItem siItem = await sfRoot.TryGetItemAsync(v_sSubProjectNo);
                if (siItem != null)
                {
                    //If folder exists then create a storage folder object for that folder.
                    StorageFolder sfSubProject = await sfRoot.GetFolderAsync(v_sSubProjectNo);

                    //Delete folder.
                    await sfSubProject.DeleteAsync(StorageDeleteOption.Default);

                }

                //If we get here then all OK.
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - (" + v_sSubProjectNo + ")");

            }

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
        /// v1.0.20 - Clear out temp folder.
        /// </summary>
        /// <returns></returns>
        public static async Task ClearTempFolder()        
        {

            try
            {

                //Clear out temp folder.
                await ApplicationData.Current.ClearAsync(ApplicationDataLocality.Temporary);

            }
            catch(Exception ex)
            {
                //v1.0.23.
                //throw new Exception(ex.Message);

            }


        }

        /// <summary>
        /// v1.0.20 - Copy files to temp folder
        /// </summary>
        /// <param name="r_sfFiles"></param>
        /// <returns></returns>
        public static async Task<List<StorageFile>> CopyFilesToTemp(IReadOnlyList<StorageFile> r_sfFiles)
        {

            string sCurrentFile = string.Empty;
            StorageFile sfiCopyFile = null;
            try
            {

               
                //Return object
                List<StorageFile> lsFiles = new List<StorageFile>();

                //Create new temp folder for storing the files.
                StorageFolder sfoTempFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync(DateTime.Now.ToString("yyyyMMddHHmmss"));
              
                foreach (StorageFile sfFile in r_sfFiles)
                {

                    sCurrentFile = sfFile.Path;
                                                       
                    // Copy the file to the destination folder.                  
                    sfiCopyFile = await sfFile.CopyAsync(sfoTempFolder);

                    //Add to return list.
                    lsFiles.Add(sfiCopyFile);

                }

                return lsFiles; 

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - (" + sCurrentFile + ")");
            }

        }

   
    }
}
