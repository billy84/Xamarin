using ANG_ABP_SURVEYOR_APP.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ANG_ABP_SURVEYOR_APP_CLASS.wcfCalls;
using ANG_ABP_SURVEYOR_APP_CLASS.Model;
using Windows.Storage;
using System.Text;
using ANG_ABP_SURVEYOR_APP_CLASS;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ANG_ABP_SURVEYOR_APP.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class DownloadProjectPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// Flag to ignore reset on text changed.
        /// </summary>
        private bool m_bIgnoreReset = false;

        /// <summary>
        /// Flag to indicate if project is valid.
        /// </summary>
        private bool m_bProjectValid = false;

       

        /// <summary>
        /// Project search results.
        /// </summary>
        private ObservableCollection<ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.SearchResult> m_ocProjSearch = new ObservableCollection<ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.SearchResult>();

        /// <summary>
        /// Name of the validated project, used as parameter.
        /// </summary>
        private string m_sProjectName = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        Windows.UI.Core.CoreDispatcher m_dpDispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;

        /// <summary>
        /// 
        /// </summary>
        private enum ValidationStatus
        {

            Error = 0
            ,Invalid = 1
            ,Valid = 2
        }


        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public DownloadProjectPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            try
            {
                this.txtProjectNo.Focus(FocusState.Programmatic);
                this.gdDownload.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.tbValidationResult.Text = String.Empty;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        /// <summary>
        /// Validate project no.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnValidateProject_Click(object sender, RoutedEventArgs e)
        {

            cAXCalls cAX = null;

            try
            {

                this.pbDownload.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.tbDownloadStatus.Text = String.Empty;

                this.m_bIgnoreReset = true;
                this.txtProjectNo.Text = this.txtProjectNo.Text.Trim().ToUpper();               

                if (this.txtProjectNo.Text.Length == 0)
                {
                   await cSettings.DisplayMessage("You need to provide a project number before you can validate.", "Project no required.");
                   this.txtProjectNo.Focus(FocusState.Programmatic);
                   return;

                }


                //Check project is not already downloaded.
                bool bAlreadyDownloaded = await this.IsProjectAlreadyOnDevice(this.txtProjectNo.Text);
                if (bAlreadyDownloaded == true) { return; }


                bool bConnectedOK = await cMain.p_cSettings.IsAXSystemAvailable(true);
                if (bConnectedOK == true)
                {


                    await cMain.CheckForUpdates();
                           
                    cAX = new cAXCalls();
                    ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.ProjectValidationResult cResult = await cAX.ValidateProjectNo(this.txtProjectNo.Text);
                    if (cResult != null)
                    {
                        if (cResult.bSuccessfull == true)
                        {

                            if (cResult.bProjectFound == true)
                            {

                                //Mark project as valid.
                                this.m_bProjectValid = true;

                                //Fetch status name from base enum table.
                                string sStatus = cMain.p_cDataAccess.GetEnumValueName("ProjTable", "Status", Convert.ToInt32(cResult.ValidationResult.Status));

                                //Display project details.
                                this.DisplayProjectDetails(cResult.ValidationResult.ProjectName, sStatus);

                                //Display download section.
                                this.UpdateValidationStatus(ValidationStatus.Valid);
                                this.gdDownload.Visibility = Windows.UI.Xaml.Visibility.Visible;

                            }
                            else
                            {

                                //Mark project as invalid.
                                this.m_bProjectValid = false;

                                this.UpdateValidationStatus(ValidationStatus.Invalid);
                                this.gdDownload.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                            }
                                                                      
                        }
                        else
                        {

                            //Mark project as invalid.
                            this.m_bProjectValid = false;

                            this.UpdateValidationStatus(ValidationStatus.Error);
                            this.gdDownload.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                        }

                    }

                }                                         

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);


            }
        }


        /// <summary>
        /// Update the validation status.
        /// </summary>
        /// <param name="v_bValidationOK"></param>
        private void UpdateValidationStatus(ValidationStatus v_vStatus)
        {


            try
            {

                if (v_vStatus == ValidationStatus.Valid)
                {
                    this.tbValidationResult.Text = "Validation Successful.";
                    this.tbValidationResult.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);

                }
                else if (v_vStatus == ValidationStatus.Invalid)
                {
                    this.tbValidationResult.Text = "Project could not be found, please amend and try again.";
                    this.tbValidationResult.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                
                }
                else if (v_vStatus == ValidationStatus.Error)
                {

                    this.tbValidationResult.Text = "Something went wrong trying to validate, please try again.";
                    this.tbValidationResult.Foreground = new SolidColorBrush(Windows.UI.Colors.Orange);

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Check if project is already on device, if so lets user know.
        /// </summary>
        /// <param name="v_sProject"></param>
        /// <returns></returns>
        private async Task<bool> IsProjectAlreadyOnDevice(string v_sProjectNo)
        {
            try
            {

                //Check if project has already been downloaded
                cProjectTable cProject = cMain.p_cDataAccess.IsProjectAlreadyDownloaded(v_sProjectNo);
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

                    await cSettings.DisplayMessage(sbMsg.ToString(), "Project Already Downloaded");

                    return true;
                }

                return false;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;
            }

        }

        /// <summary>
        /// User wants to download project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDownloadProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                await this.EnableScreenControls(false);

                await this.DownloadProject();

                await this.EnableScreenControls(true);

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                
            }
           
        }

        /// <summary>
        /// Download project from AX
        /// </summary>
        /// <returns></returns>
        private async Task DownloadProject()
        {

            cAXCalls cAX = null;
            this.tbDownloadStatus.Visibility = Windows.UI.Xaml.Visibility.Visible;           
            cSettings.AbortRetryIgnore rResponse = cSettings.AbortRetryIgnore.Abort;

            cDataAccess.SaveSubProjectDataResult srSaved;

            bool bDownloadCompletedOK = false;

            try
            {

                await this.UpdateDownloadStatus("Checking if project already downloaded.");

                //Check project is not already downloaded.
                bool bAlreadyDownloaded = await this.IsProjectAlreadyOnDevice(this.txtProjectNo.Text);
                if (bAlreadyDownloaded == true) { return; }


                await this.UpdateDownloadStatus("Checking connection.");

                bool bConnected = await cMain.p_cSettings.IsAXSystemAvailable(true);
                if (bConnected == false)
                {
                    
                }
                else
                {

                    await this.UpdateDownloadStatus("Checking for configuration updates.");

                    await cMain.CheckForUpdates();
                                             
                    cAX = new cAXCalls();


                    ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.SubProjectData spData;

                    StorageFolder sfProject = null;

                    await this.UpdateDownloadStatus("Fetching list of sub projects.");

                    List<string> lSubProjects = await cAX.ReturnListOfSubProjectsToDownload(this.txtProjectNo.Text);
                    if (lSubProjects != null)
                    {

                        //If no sub projects, let user know.
                        if (lSubProjects.Count == 0)
                        {
                            await cSettings.DisplayMessage("This project does not have any sub projects to download.", "No SubProjects");                           
                            return;

                        }


                        this.pbDownload.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        this.pbDownload.IsIndeterminate = true;
                        int iSubProjectCount = 0;

                        bool bSubProjectOK = false;
                        bool bErrorOccured = false;
                        string sErrorMessage = String.Empty;


                        foreach (string sSubProjectNo in lSubProjects)
                        {

                            //Increment by one.
                            iSubProjectCount++;
                            bSubProjectOK = false;

                            //Update screen.
                            await this.UpdateDownloadStatus("Downloading Sub Project (" + iSubProjectCount.ToString() + " of " + lSubProjects.Count() + ") - " + sSubProjectNo);

                            do
                            {

                                bErrorOccured = false;
                                bSubProjectOK = false;
                                     
                                try
                                {

                                    //Fetch sub project data.
                                    spData = await cAX.DownloadSubProjectData(sSubProjectNo);
                                    if (spData.ProjId == null)
                                    {
                                        throw new Exception("No data returned for sub project.");
                                    }
                                    else
                                    {

                                        //Save sub project data
                                        srSaved = cMain.p_cDataAccess.SaveSubProjectData(this.txtProjectNo.Text, this.m_sProjectName, spData);
                                        if (srSaved.bSavedOK == false)
                                        {
                                            throw new Exception("Cannot save sub project to database.");
                                        }
                                        else
                                        {

                                            //Fetch list of files for sub project.
                                            List<ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.SubProjectFile> sfFiles = await cAX.ReturnListOfSubProjectFiles(spData.ProjId);
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
                                                            ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.SubProjectFileDownloadResult sfDownload = await cAX.ReturnFileData(sfFile.FileName);
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
                                                                bool bSavesOK = cMain.p_cDataAccess.SaveSubProjectFile(spData.ProjId, sfFile.FileName, sfFile.Comments, sfFile.ModifiedDate,false);
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
                                    rResponse = await this.PromptOnError(sSubProjectNo,sErrorMessage);
                                    if (rResponse == cSettings.AbortRetryIgnore.Abort)
                                    {

                                        await this.UpdateDownloadStatus("Removing previously downloaded data.");

                                        await cMain.p_cDataAccess.DeleteProjectFromDevice(this.txtProjectNo.Text);
                                        return; 
                                       
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

                    bDownloadCompletedOK = true;
                    this.UpdateDownloadStatus(true);

                }                
              
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
          
            }
            finally
            {
                this.pbDownload.IsIndeterminate = false;
                this.pbDownload.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                //Only clear if download not successful.
                if (bDownloadCompletedOK == false)
                {
                    this.tbDownloadStatus.Text = String.Empty;
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

        /// <summary>
        /// Update download status text block
        /// </summary>
        /// <param name="v_sMessage"></param>
        /// <returns></returns>
        private async Task UpdateDownloadStatus(string v_sMessage)
        {


            try
            {

                await this.m_dpDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    // Your UI update code goes here!
                    this.tbDownloadStatus.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                    this.tbDownloadStatus.Text = v_sMessage;

                });

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Enable screen controls.
        /// </summary>
        /// <param name="v_bEnable"></param>
        /// <returns></returns>
        private async Task EnableScreenControls(bool v_bEnable)
        {

            try
            {

                await this.m_dpDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    // Your UI update code goes here!
                    this.backButton.IsEnabled = v_bEnable;
                    this.btnDownloadProject.IsEnabled = v_bEnable;
                    this.btnProjectSearch.IsEnabled = v_bEnable;
                    this.btnValidateProject.IsEnabled = v_bEnable;
                    this.txtProjectNo.IsEnabled = v_bEnable;

                });

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.4 - Enable search screen controls.
        /// </summary>
        /// <param name="v_bEnable"></param>
        /// <returns></returns>
        private async Task EnableSearchScreenControls(bool v_bEnable)
        {

            try
            {

                await this.m_dpDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    // Your UI update code goes here!
                    this.txtProjectName.IsEnabled = v_bEnable;
                    this.btnSearchForProjects.IsEnabled = v_bEnable;
                    this.lvProjects.IsEnabled = v_bEnable;

                });

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Update the download status.
        /// </summary>
        /// <param name="v_bValidationOK"></param>
        private void UpdateDownloadStatus(bool v_bDownloadOK)
        {

            try
            {

                if (v_bDownloadOK == true)
                {
                    this.tbDownloadStatus.Text = "Download Successful.";
                    this.tbDownloadStatus.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);

                }
                else
                {
                    this.tbDownloadStatus.Text = "Download Failed.";
                    this.tbDownloadStatus.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtProjectNo_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {

                if (this.m_bIgnoreReset == false)
                {

                    this.tbDownloadStatus.Text = string.Empty;
                    this.tbProjectName.Text = string.Empty;
                    this.tbValidationResult.Text = string.Empty;
                    this.gdDownload.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    this.m_bProjectValid = false;

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
            
            }
            finally
            {
                this.m_bIgnoreReset = false;

            }

        }

        /// <summary>
        /// Show project search
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProjectSearch_Click(object sender, RoutedEventArgs e)
        {

            try
            {
             
                //Show search screen
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender); 
                            
            }
            catch (Exception ex)
            {
                //No need to report these errors, bogus error.

            }
        }

        /// <summary>
        /// Search for projects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnSearchForProjects_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                await this.ProcessProjectSearch();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Process project search.
        /// </summary>
        private async Task ProcessProjectSearch()
        {

            cAXCalls cAX = null;

            try
            {

                try
                {

                    await this.EnableSearchScreenControls(false);
                    this.prSearch.IsActive = true;
                    

                    bool bConnected = await cMain.p_cSettings.IsAXSystemAvailable(true);
                    if (bConnected == true)
                    {

                        cAX = new cAXCalls();

                        //v1.0.19 - Add wild cards to search text
                        string sSearchText = cSettings.AddWildCardsToSearchString(this.txtProjectName.Text);

                        ObservableCollection<ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.SearchResult> ocResult = await cAX.SearchForProject(sSearchText);
                        ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.SearchResult srResult;

                        List<cBaseEnumsTable> cEnums = cMain.p_cDataAccess.GetEnumsForField("Status");
                        cBaseEnumsTable cEnum = null;

                        //Clear out existing results.
                        this.m_ocProjSearch.Clear();

                        foreach (ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.SearchResult sResult in ocResult)
                        {

                            //Find matching enum.
                            cEnum =  cEnums.Find(mc => mc.EnumValue.Equals(Convert.ToInt32(sResult.Status)));

                            srResult = new ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.SearchResult();
                            srResult.ProjectName = sResult.ProjectName;
                            srResult.ProjectNo = sResult.ProjectNo;

                            if (cEnum != null)
                            {
                                srResult.Status = cEnum.EnumName;
                            }
                            else
                            {
                                srResult.Status = "N\\A";
                            }
                            

                            this.m_ocProjSearch.Add(srResult);
                           
                        }

                        this.lvProjects.ItemsSource = this.m_ocProjSearch;

                    }

                }
                catch (Exception ex)
                {


                }

                if (cAX != null)
                {
                    await cAX.CloseAXConnection();
                }

                await this.EnableSearchScreenControls(true);
           
            }
            catch (Exception ex)
            {                
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
            finally
            {
                this.prSearch.IsActive = false;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void txtProjectName_KeyUp(object sender, KeyRoutedEventArgs e)
        {

            try
            {

                if (e.Key == Windows.System.VirtualKey.Enter)
                {
                    await this.ProcessProjectSearch();
                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }
      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void lvProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {

                foreach (ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.SearchResult cResult in e.AddedItems)
                {

                     //Check project is not already downloaded.
                    bool bAlreadyDownloaded = await this.IsProjectAlreadyOnDevice(cResult.ProjectNo);
                    if (bAlreadyDownloaded == true)
                    {
                        return;                    
                    }


                    this.m_bIgnoreReset = true;
                    this.txtProjectNo.Text = cResult.ProjectNo;

                    this.DisplayProjectDetails(cResult.ProjectName, cResult.Status);
                  
                    this.foSearch.Hide();

                    this.UpdateValidationStatus(ValidationStatus.Valid);
                    this.gdDownload.Visibility = Windows.UI.Xaml.Visibility.Visible;

                    this.m_bProjectValid = true;
                    
                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
                       
        }

        /// <summary>
        /// Display project details.
        /// </summary>
        /// <param name="v_sProjectName"></param>
        /// <param name="v_sProjectStatus"></param>
        private void DisplayProjectDetails(string v_sProjectName, string v_sProjectStatus)
        {

            try
            {

                this.m_sProjectName = v_sProjectName;

                this.tbProjectName.Text = "Name: " + v_sProjectName;
                this.tbProjectStatus.Text = "Status: " + v_sProjectStatus;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Search button flyout opening.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void foSearch_Opening(object sender, object e)
        {

            try
            {

                //Remove previous selected.
                this.lvProjects.SelectedIndex = -1;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }


    }
}
