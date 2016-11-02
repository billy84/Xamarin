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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ANG_ABP_SURVEYOR_APP.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ConnectionTest : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

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


        public ConnectionTest()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnConnectionTest_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                this.lbOutput.Items.Clear();

                //service.Proxy = System.Net.WebRequest.DefaultWebProxy;
                //service.Credentials = System.Net.CredentialCache.DefaultCredentials; ;
                //service.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

                this.lbOutput.Items.Add("Initiating wcf object");


                //System.Net.WebRequest.DefaultWebProxy = System.Net.WebRequest.DefaultWebProxy;
                ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.ServiceClient cClient = new ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.ServiceClient();
               
                string sUserName = await ANG_ABP_SURVEYOR_APP_CLASS.cSettings.GetUserName();
                string sAXCompany = cMain.GetAppResourceValue("AX_Company");


                this.lbOutput.Items.Add("Calling WCF function.");
                ANG_ABP_SURVEYOR_APP_CLASS.wcfAX.SystemsAvailableResult rResult = await cClient.ReturnAreSystemsAvailableAsync(sAXCompany, sUserName, ANG_ABP_SURVEYOR_APP_CLASS.cSettings.p_sSetting_AuthID);

                this.lbOutput.Items.Add("WCF Responded.");

                if (rResult != null)
                {
                    this.lbOutput.Items.Add("Result object not null.");

                    if (rResult.bSuccessfull == true)
                    {
                        this.lbOutput.Items.Add("Success flag is true.");
                    }
                    else
                    {
                        this.lbOutput.Items.Add("Success flag is false.");

                    }

                    if (rResult.SystemsAvailable == true)
                    {
                        this.lbOutput.Items.Add("System available flag is true.");

                    }
                    else
                    {
                        this.lbOutput.Items.Add("System available flag is false.");

                    }

                    if (rResult.UserAccountOK == true)
                    {
                        this.lbOutput.Items.Add("User account flag is true.");
                    }
                    else
                    {
                        this.lbOutput.Items.Add("User account flag is false.");

                    }
                }
                else
                {
                    this.lbOutput.Items.Add("Result object is null.");

                }

            }
            catch (Exception ex)
            {

                this.lbOutput.Items.Add("ERROR: " + ex.Message);

            }
        }

        /// <summary>
        /// Generate test error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnCreateTestError_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                this.lbOutput.Items.Clear();

                this.lbOutput.Items.Add("Initiating wcf object");

                //System.Net.WebRequest.DefaultWebProxy = System.Net.WebRequest.DefaultWebProxy;
                ANG_ABP_SURVEYOR_APP_CLASS.wcfErrorLog.ServiceClient cClient = new ANG_ABP_SURVEYOR_APP_CLASS.wcfErrorLog.ServiceClient();

                string sUserName = await ANG_ABP_SURVEYOR_APP_CLASS.cSettings.GetUserName();
                string sStatus = cMain.GetAppResourceValue("STATUS");


                this.lbOutput.Items.Add("Calling WCF function.");

                ANG_ABP_SURVEYOR_APP_CLASS.wcfErrorLog.ErrorInfo ei = new ANG_ABP_SURVEYOR_APP_CLASS.wcfErrorLog.ErrorInfo();
                ei.iPriority = 10;
                ei.sClassName = "Test Class";
                ei.sEx_Message = "Test Exception Message";
                ei.sEx_Source = "Test Exception Source";
                ei.sEx_StackTrace = "Test Exception Stack Trace";
                ei.sInformation = "Test Information";
                ei.sMachineName = ANG_ABP_SURVEYOR_APP_CLASS.cSettings.GetMachineName();
                ei.sMethodName = "TEST METHOD NAME";
                ei.sProductName = cMain.GetAppResourceValue("ProductName");
                ei.sProductVersion = cMain.GetAppResourceValue("ProductVersion");
                ei.sUserName = sUserName;

                bool bOK = await cClient.ReportErrorAsync(sStatus, ei);

                this.lbOutput.Items.Add("WCF Responded.");

                if (bOK != null)
                {
                    if (bOK == true)
                    {
                        this.lbOutput.Items.Add("WCF Responded (True)");
                    }
                    else
                    {
                        this.lbOutput.Items.Add("WCF Responded (False)");
                    }
                }
                else
                {
                    this.lbOutput.Items.Add("WCF Responded (Null)");
                }
                        

            }
            catch (Exception ex)
            {

                this.lbOutput.Items.Add("ERROR: " + ex.Message);
            }

        }
    }
}
