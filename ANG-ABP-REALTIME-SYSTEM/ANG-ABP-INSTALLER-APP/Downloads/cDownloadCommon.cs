using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using ANG_ABP_SURVEYOR_APP_CLASS;

namespace ANG_ABP_INSTALLER_APP.Downloads
{
    class cDownloadCommon
    {

        /// <summary>
        /// Structure for return data after applying survey date changes.
        /// </summary>
        public struct DownloadResult
        {

            /// <summary>
            /// Bitmap
            /// </summary>
            public bool bSuccessful;

            /// <summary>
            /// Original width
            /// </summary>
            public int iSubProjectCount;

         
        }


        /// <summary>
        /// Event handler for displaying messages
        /// </summary>
        public event EventHandler<string> UpdateMessage;


        public async Task<DownloadResult> DownloadSubProjects(string v_sProjectNo, string v_sProjectName)
        {

            DownloadResult drResult = new DownloadResult();
            drResult.bSuccessful = false;
            drResult.iSubProjectCount = 0;

            ANG_ABP_SURVEYOR_APP_CLASS.wcfCalls.cAXCalls cAX_WCF = null;
            try
            {

                int iSubProjectCount = 0;
                bool bSubProjectOK = false;
                bool bErrorOccured = false;
                string sErrorMessage = string.Empty;
                cDataAccess.SaveSubProjectDataResult srSaved;
                cSettings.AbortRetryIgnore rResponse = cSettings.AbortRetryIgnore.Abort;


                ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.SubProjectData spData;

                StorageFolder sfProject = null;

                cAX_WCF = new ANG_ABP_SURVEYOR_APP_CLASS.wcfCalls.cAXCalls();

                List<string> lSubProjects = await cAX_WCF.ReturnListOfSubProjectsToDownload(v_sProjectNo);
                 if (lSubProjects != null)
                 {


                     foreach (string sSubProjectNo in lSubProjects)
                     {

                         //Increment by one.
                         iSubProjectCount++;
                         bSubProjectOK = false;

                         //Update screen.
                         this.UpdateMessage(this,"Downloading Sub Project (" + iSubProjectCount.ToString() + " of " + lSubProjects.Count() + ") - " + sSubProjectNo);


                         do
                         {

                             bErrorOccured = false;
                             bSubProjectOK = false;

                             try
                             {

                                 //Fetch sub project data.
                                 spData = await cAX_WCF.DownloadSubProjectData(sSubProjectNo);
                                 if (spData.ProjId == null)
                                 {
                                     throw new Exception("No data returned for sub project.");
                                 }
                                 else
                                 {

                                     //Save sub project data
                                     srSaved = cMain.p_cDataAccess.SaveSubProjectData(v_sProjectNo, v_sProjectName, spData);
                                     if (srSaved.bSavedOK == false)
                                     {
                                         throw new Exception("Cannot save sub project to database.");
                                     }
                                     else
                                     {

                                         //Increase sub project count.
                                         drResult.iSubProjectCount += 1;

                                         //Fetch list of files for sub project.
                                         List<ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.SubProjectFile> sfFiles = await cAX_WCF.ReturnListOfSubProjectFiles(spData.ProjId);
                                         if (sfFiles == null)
                                         {
                                             throw new Exception("Cannot retrieve list of files.");
                                         }
                                         else
                                         {

                                             //If files found.
                                             if (sfFiles.Count > 0)
                                             {

                                                 //Create- return local sub project image folder.
                                                 sfProject = await cSettings.ReturnSubProjectImagesFolder(spData.ProjId);
                                                 if (sfProject == null)
                                                 {
                                                     throw new Exception("Unable to retrieve local image folder.");

                                                 }
                                                 else
                                                 {

                                                     //Loop through list of files.
                                                     foreach (ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.SubProjectFile sfFile in sfFiles)
                                                     {

                                                         //Download file data
                                                         ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.SubProjectFileDownloadResult sfDownload = await cAX_WCF.ReturnFileData(sfFile.FileName);
                                                         if (sfDownload == null)
                                                         {
                                                             throw new Exception("Unable to download file data for (" + sfFile.FileName + ")");
                                                         }
                                                         else
                                                         {

                                                             //Save file to device.
                                                             bool bFileSaved = await cSettings.SaveFileLocally(sfProject, sfDownload.byFileData, sfFile.FileName);
                                                             if (bFileSaved == false)
                                                             {
                                                                 throw new Exception("Unable to save file to device (" + sfFile.FileName + ")");
                                                             }


                                                             //Save file record into files table.
                                                             bool bSavesOK = cMain.p_cDataAccess.SaveSubProjectFile(spData.ProjId, sfFile.FileName, sfFile.Comments, sfFile.ModifiedDate, false);
                                                             if (bSavesOK == false)
                                                             {
                                                                 throw new Exception("Unable to save file details to database (" + sfFile.FileName + ")");

                                                             }

                                                         }

                                                     }
                                                 }

                                             }

                                         }

                                     }

                                 }

                                 //If we get here then all OK.
                                 bSubProjectOK = true;

                             }
                             catch (Exception ex)
                             {

                                 bErrorOccured = true;
                                 sErrorMessage = ex.Message;

                                 cMain.ReportError(ex, cMain.GetCallerMethodName(), "Downloading sub project (" + sSubProjectNo + ")");

                             }

                             //If error occurred
                             if (bErrorOccured == true)
                             {

                                 //Ask user what they want to do.
                                 rResponse = await this.PromptOnError(sSubProjectNo, sErrorMessage);
                                 if (rResponse == cSettings.AbortRetryIgnore.Abort)
                                 {

                                     this.UpdateMessage(this, "Removing previously downloaded data.");

                                     await cMain.p_cDataAccess.DeleteProjectFromDevice(v_sProjectNo);
                                     return drResult;

                                 }
                                 else if (rResponse == cSettings.AbortRetryIgnore.Ignore)
                                 {
                                     bSubProjectOK = true;

                                 }
                                 else if (rResponse == cSettings.AbortRetryIgnore.Retry)
                                 {

                                     //Wait
                                 }

                             }


                         }
                         while (bSubProjectOK == false);

                     }
                     
                 }

                 drResult.bSuccessful = true;
                 return drResult;
                    
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return drResult;
          
            }
            finally
            {

                if (cAX_WCF != null)
                {
                    cAX_WCF.CloseAXConnection();
                }
            }


        }


        /// <summary>
        /// Prompt user what to do on error.
        /// </summary>
        /// <param name="v_sSubProject"></param>
        /// <param name="v_sInfo"></param>
        /// <returns></returns>
        private async Task<cSettings.AbortRetryIgnore> PromptOnError(string v_sSubProject, string v_sInfo)
        {

            try
            {

                //Check connection
                bool bConnected = await cMain.p_cSettings.IsAXSystemAvailable(false);

                StringBuilder sbMessage = new StringBuilder();

                //Depending on connection status, change message.
                if (bConnected == true)
                {
                    sbMessage.Append("A problem has occurred when downloading the sub project (" + v_sSubProject + ")");


                }
                else
                {
                    sbMessage.Append(cSettings.ReturnNoConnectionMessage());

                }

                sbMessage.Append(Environment.NewLine);
                sbMessage.Append(Environment.NewLine);


                sbMessage.Append("What do you want to do?");

                sbMessage.Append(Environment.NewLine);
                sbMessage.Append("** Abort - Abort the entire project download. (This will delete all project data downloaded so far).");

                sbMessage.Append(Environment.NewLine);
                sbMessage.Append("** Retry - Try and download the sub project again.");

                sbMessage.Append(Environment.NewLine);
                sbMessage.Append("** Ignore - Ignore this sub project and continue with the next.");

                sbMessage.Append(Environment.NewLine);
                sbMessage.Append(Environment.NewLine);

                sbMessage.Append("Info: " + v_sInfo);

                sbMessage.Append(Environment.NewLine);
                sbMessage.Append(Environment.NewLine);

                sbMessage.Append(cMain.GetAppResourceValue("Support_Message"));

                return await cSettings.DisplayAbortRetryIgnore(sbMessage.ToString(), "Download Issue");


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return cSettings.AbortRetryIgnore.Abort;

            }

        }


    }
}
