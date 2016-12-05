using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Anglian.Classes;
using Anglian.Engine;
using Anglian.Models;
using Anglian.Service;
using Acr.UserDialogs;

namespace Anglian.Views
{
    public class Log
    {
        public string Status { get; set; }
        public Color Color { get; set; }
    }
    public partial class ProjectDownloadStatusPage : ContentPage
    {
        private static Task backgroundTask = null;
        public struct Percent
        {
            public const double START = .0;//start 
            public const double S1 = .1;//Fetch sub project data.
            public const double S2 = .2;//Save sub project data
            public const double S3 = .3;//Fetch list of files for sub project.
            public const double S4 = .4;//Create- return local sub project image folder.
            public const double END = 1.0;//end 
        }
        ObservableCollection<Log> logs = new ObservableCollection<Log>();
        public ProjectDownloadStatusPage(SearchResult oResult)
        {
            InitializeComponent();
            this.Title = "Project Download";
            txtProjectNo.Text = oResult.ProjectNo;
            txtProjectName.Text = oResult.ProjectName;
            txtSubProjectNo.Text = "---";
            txtSubTask.Text = "---";
            //lvProgressStatus.ItemsSource = logs;
        }
        private void LoggingSubProject(Color color, string status, double percent)
        {
            txtSubProjectNo.Text = status;
            txtSubProjectNo.TextColor = color;
            SetTotalProgress(percent);
            //Log log = new Log() { Color = color, Status = status };
            //logs.Add(log);
        }
        private async void SetTotalProgress(double percent)
        {
            await totalProgress.ProgressTo(percent, 250, Easing.Linear);
        }
        private async void SetSubProgress(double percent)
        {
            await SubProgress.ProgressTo(percent, 250, Easing.Linear);
        }
        private void LoggingStatus(Color color, string status, double percent)
        {
            txtSubTask.Text = status;
            txtSubTask.TextColor = color;
            SetSubProgress(percent);
            //Log log = new Log() { Color = color, Status = status };
            //logs.Add(log);
        }
        private void Clicked_Download(object sender, EventArgs args)
        {
            if (backgroundTask != null && backgroundTask.Status == TaskStatus.Running)
            {
                DisplayAlert("Warning", "Downloading another Project", "OK");
                return;
            }
            if (backgroundTask == null || backgroundTask.Status == TaskStatus.RanToCompletion)
            {
                backgroundTask = new Task(DownloadProject);
                backgroundTask.RunSynchronously();
            }
        }
        private async Task<bool> IsProjectAlreadyOnDevice(string v_sProjectNo)
        {
            try
            {

                //Check if project has already been downloaded
                cProjectTable cProject = Main.p_cDataAccess.IsProjectAlreadyDownloaded(v_sProjectNo);
                if (cProject != null)
                {

                    StringBuilder sbMsg = new StringBuilder();
                    sbMsg.Append("This project has already been downloaded onto your device:");
                    sbMsg.Append(Environment.NewLine);
                    sbMsg.Append(Environment.NewLine);
                    sbMsg.Append("Project No: " + v_sProjectNo);
                    sbMsg.Append(Environment.NewLine);
                    sbMsg.Append("Project Name: " + cProject.ProjectName);
                    sbMsg.Append(Environment.NewLine);
                    sbMsg.Append(Environment.NewLine);
                    sbMsg.Append("Any changes to the project or sub projects will be applied in the syncing process.");

                    await DisplayAlert("Project Already Downloaded", sbMsg.ToString(), "OK");

                    return true;
                }

                return false;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;
            }

        }
        private void SetBarColor(Color color)
        {
            var navigationPage = Application.Current.MainPage as NavigationPage;
            navigationPage.BarBackgroundColor = color;
        }
        private async void DownloadProject()
        {
            WcfExt116 cAX = null;
            //this.tbDownloadStatus.Visibility = Windows.UI.Xaml.Visibility.Visible;
            string rResponse = "Abort";

            DataAccess.SaveSubProjectDataResult srSaved;

            bool bDownloadCompletedOK = false;

            try
            {

                LoggingSubProject(Color.White, "Checking if project already downloaded.", .0);
                
                //await this.UpdateDownloadStatus("Checking if project already downloaded.");

                //Check project is not already downloaded.
                bool bAlreadyDownloaded = await IsProjectAlreadyOnDevice(this.txtProjectNo.Text);
                if (bAlreadyDownloaded == true) { return; }
                LoggingSubProject(Color.White, "Checking connection.", .3);
                //await this.UpdateDownloadStatus("Checking connection.");

                bool bConnected = await Main.p_cSettings.IsAXSystemAvailable(true);
                if (bConnected == false)
                {
                    //LoggingSubProject(Color.Red, "Failed Checking connection.", .3);
                }
                else
                {
                    LoggingSubProject(Color.White, "Checking for configuration updates.", .6);
                    //await this.UpdateDownloadStatus("Checking for configuration updates.");

                    await Main.CheckForUpdates();

                    LoggingSubProject(Color.White, "Completed Checking.", 1.0);

                    cAX = new WcfExt116();


                    SubProjectData spData;

                    object sfProject = null;
                    LoggingSubProject(Color.White, "Fetching list of sub projects.", .0);
                    //await this.UpdateDownloadStatus("Fetching list of sub projects.");

                    List<string> lSubProjects = await cAX.ReturnListOfSubProjectsToDownload(txtProjectNo.Text);
                    if (lSubProjects != null)
                    {

                        //If no sub projects, let user know.
                        if (lSubProjects.Count == 0)
                        {
                            await DisplayAlert("No SubProjects", "This project does not have any sub projects to download.", "OK");
                            return;

                        }


                        //this.pbDownload.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        //this.pbDownload.IsIndeterminate = true;
                        int iSubProjectCount = 0;

                        bool bSubProjectOK = false;
                        bool bErrorOccured = false;
                        string sErrorMessage = String.Empty;

                        double percent = .0;
                        foreach (string sSubProjectNo in lSubProjects)
                        {
                            Random rnd = new Random();
                            SetBarColor(Color.FromRgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255)));
                            //Increment by one.
                            iSubProjectCount++;
                            bSubProjectOK = false;

                            //Update screen.
                            LoggingSubProject(Color.White, "Downloading Sub Project (" + iSubProjectCount.ToString() + " of " + lSubProjects.Count() + ") - " + sSubProjectNo, percent);
                            //await this.UpdateDownloadStatus("Downloading Sub Project (" + iSubProjectCount.ToString() + " of " + lSubProjects.Count() + ") - " + sSubProjectNo);
                            percent = Convert.ToDouble(iSubProjectCount) / Convert.ToDouble(lSubProjects.Count);
                            do
                            {

                                bErrorOccured = false;
                                bSubProjectOK = false;

                                try
                                {

                                    //Fetch sub project data.
                                    LoggingStatus(Color.White, "Fetch sub project data", Percent.START);
                                    spData = await cAX.DownloadSubProjectData(sSubProjectNo);
                                    if (spData.ProjId == null)
                                    {
                                        throw new Exception("No data returned for sub project.");
                                    }
                                    else
                                    {

                                        //Save sub project data
                                        LoggingStatus(Color.White, "Save sub project data", Percent.S1);
                                        srSaved = Main.p_cDataAccess.SaveSubProjectData(txtProjectNo.Text, txtProjectName.Text, spData);
                                        if (srSaved.bSavedOK == false)
                                        {
                                            throw new Exception("Cannot save sub project to database.");
                                        }
                                        else
                                        {
                                            LoggingStatus(Color.White, "Fetch list of files for sub project", Percent.S2);
                                            //Fetch list of files for sub project.
                                            List<SubProjectFile> sfFiles = await cAX.ReturnListOfSubProjectFiles(spData.ProjId);
                                            if (sfFiles == null)
                                            {
                                                throw new Exception("Cannot retrieve list of files.");
                                            }
                                            else
                                            {

                                                //If files found.
                                                LoggingStatus(Color.Yellow, "No files for sub project", Percent.S3);
                                                if (sfFiles.Count > 0)
                                                {

                                                    LoggingStatus(Color.White, "Return local sub project image folder.", Percent.S3);
                                                    //Create- return local sub project image folder.
                                                    sfProject = await DependencyService.Get<ISettings>().ReturnSubProjectImagesFolder(spData.ProjId);
                                                    if (sfProject == null)
                                                    {
                                                        throw new Exception("Unable to retrieve local image folder.");

                                                    }
                                                    else
                                                    {

                                                        //Loop through list of files.
                                                        int fileCount = 0;
                                                        
                                                        LoggingStatus(Color.White, "Loop through list of files", Percent.S4);
                                                        foreach (SubProjectFile sfFile in sfFiles)
                                                        {
                                                            LoggingStatus(
                                                                Color.White, 
                                                                "Download file data for (" + sfFile.FileName + ")",
                                                                Percent.S4 + (.6 / sfFiles.Count) / 3 + (.6 / sfFiles.Count) * fileCount);
                                                            //Download file data
                                                            SubProjectFileDownloadResult sfDownload = await cAX.ReturnFileData(sfFile.FileName);
                                                            if (sfDownload == null)
                                                            {
                                                                throw new Exception("Unable to download file data for (" + sfFile.FileName + ")");
                                                            }
                                                            else
                                                            {
                                                                LoggingStatus(
                                                                    Color.White, 
                                                                    "Save file to device (" + sfFile.FileName + ")",
                                                                    Percent.S4 + (.6 / sfFiles.Count) / 3 * 2 + (.6 / sfFiles.Count) * fileCount);
                                                                //Save file to device.
                                                                bool bFileSaved = await DependencyService.Get<ISettings>().SaveFileLocally(sSubProjectNo, sfDownload.byFileData, sfFile.FileName);
                                                                if (bFileSaved == false)
                                                                {
                                                                    throw new Exception("Unable to save file to device (" + sfFile.FileName + ")");
                                                                }

                                                                LoggingStatus(
                                                                    Color.White,
                                                                    "Save file details to database (" + sfFile.FileName + ")",
                                                                    Percent.S4 + .6 / sfFiles.Count + (.6 / sfFiles.Count) * fileCount);
                                                                //Save file record into files table.
                                                                bool bSavesOK = Main.p_cDataAccess.SaveSubProjectFile(
                                                                    spData.ProjId, 
                                                                    sfFile.FileName, 
                                                                    sfFile.Comments, 
                                                                    sfFile.ModifiedDate, 
                                                                    false);
                                                                if (bSavesOK == false)
                                                                {
                                                                    throw new Exception("Unable to save file details to database (" + sfFile.FileName + ")");

                                                                }

                                                            }
                                                            fileCount++;

                                                        }
                                                    }

                                                }
                                                

                                            }

                                        }

                                    }
                                    LoggingStatus(Color.White, "Downloading this file was done", Percent.END);
                                    //If we get here then all OK.
                                    bSubProjectOK = true;

                                }
                                catch (Exception ex)
                                {

                                    bErrorOccured = true;
                                    sErrorMessage = ex.Message;

                                    //cMain.ReportError(ex, cMain.GetCallerMethodName(), "Downloading sub project (" + sSubProjectNo + ")");

                                }

                                //If error occurred
                                if (bErrorOccured == true)
                                {

                                    //Ask user what they want to do.
                                    rResponse = await PromptOnError(sSubProjectNo, sErrorMessage);
                                    if (rResponse == "Abort")
                                    {
                                        LoggingStatus(Color.Red, "Removing previously downloaded data.", .0);
                                        //await UpdateDownloadStatus("Removing previously downloaded data.");

                                        await Main.p_cDataAccess.DeleteProjectFromDevice(this.txtProjectNo.Text);
                                        return;

                                    }
                                    else if (rResponse == "Ignore")
                                    {
                                        bSubProjectOK = true;

                                    }
                                    else if (rResponse == "Retry")
                                    {

                                        //Wait
                                    }

                                }


                            }
                            while (bSubProjectOK == false);

                        }

                    }

                    bDownloadCompletedOK = true;
                    LoggingSubProject(Color.White, "OK Downloading.", 1.0);
                    //this.UpdateDownloadStatus(true);

                }

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
            finally
            {
                //this.pbDownload.IsIndeterminate = false;
                //this.pbDownload.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                //Only clear if download not successful.
                if (bDownloadCompletedOK == false)
                {
                    LoggingStatus(Color.Red, "Failed Downloading.", .0);
                    //this.tbDownloadStatus.Text = String.Empty;
                }
                SetBarColor(Color.Black);
            }

        }
        /// <summary>
        /// Prompt user what to do on error.
        /// </summary>
        /// <param name="v_sSubProject"></param>
        /// <param name="v_sInfo"></param>
        /// <returns></returns>
        private async Task<string> PromptOnError(string v_sSubProject, string v_sInfo)
        {

            try
            {

                //Check connection
                bool bConnected = await Main.p_cSettings.IsAXSystemAvailable(false);

                StringBuilder sbMessage = new StringBuilder();

                //Depending on connection status, change message.
                if (bConnected == true)
                {
                    sbMessage.Append("A problem has occurred when downloading the sub project (" + v_sSubProject + ")");


                }
                else
                {
                    sbMessage.Append(Settings.ReturnNoConnectionMessage());

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

                sbMessage.Append(DependencyService.Get<IMain>().GetAppResourceValue("Support_Message"));

                //return await Settings.DisplayAbortRetryIgnore(sbMessage.ToString(), "Download Issue");
                return await DisplayActionSheet(sbMessage.ToString(), "Cancel", null, "Abort", "Retry", "Ignore");


            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return "Abort";

            }

        }
    }
}
