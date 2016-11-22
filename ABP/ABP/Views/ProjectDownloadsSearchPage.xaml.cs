using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ABP.WcfProxys;
using ABP.TableModels;
using ABP.Models;
using Acr.UserDialogs;

namespace ABP.Views
{
    public partial class ProjectDownloadsSearchPage : ContentPage
    {
        private ObservableCollection<ServiceExt.SearchResult> m_ocProjSearch = new ObservableCollection<ServiceExt.SearchResult>();
        private string m_sProjectName = string.Empty;
        private List<ServiceExt.BaseEnumField> bcFields = null;
        private cAXCalls cAX = new cAXCalls();
        private bool m_bProjectValid = false;
        private enum ValidationStatus
        {
            Error = 0,
            Invalid = 1,
            Valid = 2
        }

        public ProjectDownloadsSearchPage()
        {
            InitializeComponent();
            this.Title = "Project Downloads Search";
            cmbProjectType.SelectedIndex = 0;
            txtProjectNo.Keyboard = Keyboard.Numeric;
            SearchProjectBtn.Source = ImageSource.FromFile(String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "find"));
            var tapSearchBtn = new TapGestureRecognizer();
            cmbProjectType.SelectedIndexChanged += CmbProjectType_SelectedIndexChanged;
            tapSearchBtn.Tapped += TapSearchBtn_Tapped;
            SearchProjectBtn.GestureRecognizers.Add(tapSearchBtn);
            lvProjects.ItemSelected += LvProjects_ItemSelected;
        }
        private void LvProjects_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {

            }
            else
            {
                //e.SelectedItem
                ServiceExt.SearchResult oResult = (ServiceExt.SearchResult)e.SelectedItem;
                Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new ProjectDownloadStatusPage(oResult)));
            }

            //throw new NotImplementedException();
        }

        private void CmbProjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProjectType.SelectedIndex == 0)
            {
                txtProjectNo.Keyboard = Keyboard.Numeric;
            }
            else if (cmbProjectType.SelectedIndex == 1)
            {
                txtProjectNo.Keyboard = Keyboard.Text;
            }
            else
            {
                txtProjectNo.Keyboard = Keyboard.Default;
            }
            //throw new NotImplementedException();
        }

        private void TapSearchBtn_Tapped(object sender, EventArgs e)
        {
            try
            {
                txtProjectNo.Text = txtProjectNo.Text.Trim().ToUpper();
                if (txtProjectNo.Text.Length == 0)
                {
                    DisplayAlert("Project Number Required.", "You need to provide a project number before you can validate.", "Retry");
                    txtProjectNo.Focus();
                    return;
                }
                bool bAlreadyDownloaded = IsProjectAlreadyOnDevice(txtProjectNo.Text);
                if (bAlreadyDownloaded == true)
                {
                    return;
                }
                if (cSettings.AreWeOnline == true)
                {
                    //string sUserProfile = await DependencyService.Get<ISettings>().GetUserName();
                    string sUserProfile = WcfLogin.m_instance.LoggedUserName;
                    cAX.m_wcfClient.ReturnAreSystemsAvailableCompleted += M_wcfClient_ReturnAreSystemsAvailableCompleted;
                    cAX.m_wcfClient.ReturnAreSystemsAvailableAsync(cAX.m_cCompanyName, sUserProfile, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);
                    UserDialogs.Instance.ShowLoading("Checking System Available...", MaskType.Black);
                }
                else
                {
                    DisplayAlert("Warning", "Internet connection failed.", "OK");
                }

            }
            catch (Exception ex)
            {

            }
            //throw new NotImplementedException();
        }

        private void M_wcfClient_ReturnAreSystemsAvailableCompleted(object sender, ServiceExt.ReturnAreSystemsAvailableCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            UserDialogs.Instance.HideLoading();
            if (e.Error != null)
            {
                DisplayAlert("Error", e.Error.Message, "OK");
            }
            else if (e.Cancelled == true)
            {
                DisplayAlert("Error", "Request was canceled.", "OK");
            }
            else
            {
                if (e.Result.SystemsAvailable == true)
                {
                    if (cmbProjectType.SelectedIndex == 1)
                    {
                        // while search
                        UserDialogs.Instance.ShowLoading("Downloading Projects by name...", MaskType.Black);
                        string sSearchText = cSettings.AddWildCardsToSearchString(txtProjectNo.Text);
                        cAX.m_wcfClient.SearchForContractCompleted += M_wcfClient_SearchForContractCompleted;
                        cAX.m_wcfClient.SearchForContractAsync(cAX.m_cCompanyName, sSearchText, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);
                    }
                    else
                    {
                        // display work details
                        if (cMain.m_bCheckingBaseEnums == true)
                        {

                        }
                        else
                        {
                            bool bCheck = cMain.ShouldICheckForBaseEnums();
                            if (bCheck == true)
                            {
                                List<ServiceExt.BaseEnumField> bFields = cMain.p_cDataAccess.GetBaseEnumUpdates();
                                ObservableCollection<ServiceExt.BaseEnumField> ocFields = new ObservableCollection<ServiceExt.BaseEnumField>();
                                //List<ServiceExt.BaseEnumField> bFieldsUpdate = 
                                cAX.m_wcfClient.ReturnBaseEnumsAsync(cAX.m_cCompanyName, ocFields, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);
                                cAX.m_wcfClient.ReturnBaseEnumsCompleted += M_wcfClient_ReturnBaseEnumsCompleted;
                            }
                            else
                            {
                                if (cAX != null)
                                {
                                    if (cAX.m_wcfClient != null)
                                    {
                                        cAX.m_wcfClient.CloseCompleted += M_wcfClient_CloseCompleted;
                                        cAX.m_wcfClient.CloseAsync();
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    DisplayAlert("Error", "System non Available.", "OK");
                }
            }
        }

        private void M_wcfClient_SearchForContractCompleted(object sender, ServiceExt.SearchForContractCompletedEventArgs e)
        {
            UserDialogs.Instance.HideLoading();
            if (e.Error != null)
            {

            }
            else if (e.Cancelled == true)
            {

            }
            else
            {
                if (e.Result.bSuccessfull == true)
                {
                    UserDialogs.Instance.ShowLoading("Loading Projects...", MaskType.Black);
                    ObservableCollection<ServiceExt.SearchResult> oResults = new ObservableCollection<ServiceExt.SearchResult>();
                    oResults = e.Result.SearchResults;
                    ServiceExt.SearchResult srResult;
                    List<cBaseEnumsTable> cEums = cMain.p_cDataAccess.GetEnumsForField("Status");
                    cBaseEnumsTable cEum = null;
                    try
                    {
                        m_ocProjSearch.Clear();
                    }
                    catch (Exception ex)
                    {

                    }
                    foreach (ServiceExt.SearchResult sResult in oResults)
                    {
                        cEum = cEums.Find(mc => mc.EnumValue.Equals(Convert.ToInt32(sResult.Status)));
                        srResult = new ServiceExt.SearchResult();
                        srResult.ProjectName = sResult.ProjectName;
                        srResult.ProjectNo = sResult.ProjectNo;
                        if (cEum != null)
                        {
                            srResult.Status = cEum.EnumName;
                        }
                        else
                        {
                            srResult.Status = "N\\A";
                        }
                        m_ocProjSearch.Add(srResult);

                    }
                    lvProjects.ItemsSource = m_ocProjSearch;
                    UserDialogs.Instance.HideLoading();

                }
            }
            //throw new NotImplementedException();
        }

        private void M_wcfClient_ReturnBaseEnumsCompleted(object sender, ServiceExt.ReturnBaseEnumsCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                DisplayAlert("Error", e.Error.Message, "OK");
            }
            else if (e.Cancelled == true)
            {
                DisplayAlert("Error", "Request was canceled.", "OK");
            }
            else
            {

                if (e.Result.bSuccessfull == true)
                {
                    bcFields = new List<ServiceExt.BaseEnumField>(e.Result.BaseEnumResults);
                }
                if (cAX != null)
                {
                    if (cAX.m_wcfClient != null)
                    {
                        cAX.m_wcfClient.CloseCompleted += M_wcfClient_CloseCompleted;
                        cAX.m_wcfClient.CloseAsync();
                    }
                }
                
            }
            //throw new NotImplementedException();
        }

        private void M_wcfClient_CloseCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                DisplayAlert("Error", e.Error.Message, "OK");
            }
            else if (e.Cancelled == true)
            {
                DisplayAlert("Error", "Request was canceled.", "OK");
            }
            else
            {
                cAX.m_wcfClient = null;
                if (bcFields != null)
                {
                    cMain.p_cDataAccess.ProcessUpdatedBaseEnums(bcFields);
                }
                cMain.m_bCheckingBaseEnums = false;

                // start check for setting
                if (cMain.m_bCheckingSetings == true)
                {

                }
                else
                {
                    bool bCheck = cMain.ShouldICheckForSettings();
                    if (bCheck == true)
                    {
                        cAX = new cAXCalls();
                        List<ServiceExt.SettingDetails> v_sdSettings = cMain.p_cDataAccess.GetSettingsUpdates();
                        ObservableCollection<ServiceExt.SettingDetails> ocSettings = new ObservableCollection<ServiceExt.SettingDetails>(v_sdSettings);
                        cAX.m_wcfClient.CheckForUpdatedSettingsCompleted += M_wcfClient_CheckForUpdatedSettingsCompleted;
                        cAX.m_wcfClient.CheckForUpdatedSettingsAsync(cAX.m_cCompanyName, ocSettings, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);
                    }
                }

                cAX = new cAXCalls();
                UserDialogs.Instance.ShowLoading("Downloading Project by number...", MaskType.Black);
                cAX.m_wcfClient.ValidateProjectCompleted += M_wcfClient_ValidateProjectCompleted;
                cAX.m_wcfClient.ValidateProjectAsync(cAX.m_cCompanyName, txtProjectNo.Text, cSettings.p_sSetting_AuthID, WcfLogin.m_instance.Token);
            }
            //throw new NotImplementedException();
        }

        private void M_wcfClient_CheckForUpdatedSettingsCompleted(object sender, ServiceExt.CheckForUpdatedSettingsCompletedEventArgs e)
        {
            List<ServiceExt.SettingDetails> sdSettings = null;
            if (e.Error != null)
            {

            }
            else if (e.Cancelled == true)
            {

            }
            else
            {
                if (e.Result.bSuccessfull == true)
                {
                    sdSettings = new List<ServiceExt.SettingDetails>(e.Result.Settings);

                }
            }
            //throw new NotImplementedException();
        }

        private void M_wcfClient_ValidateProjectCompleted(object sender, ServiceExt.ValidateProjectCompletedEventArgs e)
        {
            UserDialogs.Instance.HideLoading();
            if (e.Error != null)
            {
                DisplayAlert("Error", e.Error.Message, "OK");
            }
            else if (e.Cancelled == true)
            {
                DisplayAlert("Error", "Request was canceled.", "OK");
            }
            else
            {
                if (e.Result.bSuccessfull == true)
                {
                    if (e.Result.bProjectFound == true)
                    {
                        m_bProjectValid = true;
                        string sStatus = cMain.p_cDataAccess.GetEnumValueName("ProjTable", "Status", Convert.ToInt32(e.Result.ValidationResult.Status));
                        ServiceExt.SearchResult oResult = new ServiceExt.SearchResult();
                        oResult.ProjectName = e.Result.ValidationResult.ProjectName;
                        oResult.ProjectNo = e.Result.ValidationResult.ProjectNo;
                        oResult.Status = sStatus;
                        //if (m_ocProjSearch.Count() > 0)
                        try
                        {
                            m_ocProjSearch.Clear();
                        }
                        catch (Exception ex)
                        {
                            string err = ex.Message;
                        }
                        
                        m_ocProjSearch.Add(oResult);
                        lvProjects.ItemsSource = m_ocProjSearch;
                    }
                    else
                    {
                        DisplayAlert("Warning", "Please enter valid Project No.", "Retry");
                        txtProjectNo.Focus();
                    }
                }
            }
            //throw new NotImplementedException();
        }

        private bool IsProjectAlreadyOnDevice(string v_sProjectNo)
        {
            try
            {
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
                    DisplayAlert("Project Already Downloaded", sbMsg.ToString(), "OK");

                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
