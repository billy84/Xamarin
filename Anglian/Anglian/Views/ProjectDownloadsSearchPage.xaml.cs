using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Classes;
using Anglian.Models;
using Anglian.Service;
using Xamarin.Forms;
using Anglian.Engine;
using Acr.UserDialogs;
namespace Anglian.Views
{
    public partial class ProjectDownloadsSearchPage : ContentPage
    {
        private ObservableCollection<SearchResult> m_ocProjSearch = new ObservableCollection<SearchResult>();
        private string m_sProjectName = string.Empty;
        private List<BaseEnumField> bcFields = null;
        private enum ValidationStatus
        {
            Error = 0,
            Invalid = 1,
            Valid = 2
        }
        public ProjectDownloadsSearchPage()
        {
            InitializeComponent();
            Title = "Project Downloads Search";
            cmbProjectType.SelectedIndex = 0;
            txtProjectNo.Keyboard = Keyboard.Numeric;
            SearchProjectBtn.Source = ImageSource.FromFile(String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "find"));
            SearchProjectBtn.HeightRequest = 30;
            SearchProjectBtn.WidthRequest = 30;
            var tapSearchBtn = new TapGestureRecognizer();
            cmbProjectType.SelectedIndexChanged += CmbProjectType_SelectedIndexChanged;
            tapSearchBtn.Tapped += TapSearchBtn_Tapped;
            SearchProjectBtn.GestureRecognizers.Add(tapSearchBtn);
            //lvProjects.ItemsSource = new string[0];
            lvProjects.ItemTapped += LvProjects_ItemTapped;
        }
        private async void LvProjects_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
            {

            }
            else
            {
                //e.SelectedItem
                SearchResult oResult = (SearchResult)e.Item;
                bool bAlreadyDownloaded = await IsProjectAlreadyOnDevice(oResult.ProjectNo);
                if (bAlreadyDownloaded == true)
                {
                    return;
                }
                else
                {
                    //lvProjects.ItemSelected += null;
                    Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new ProjectDownloadStatusPage(oResult)));
                }
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
        private async void TapSearchBtn_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text == null || txtProjectNo.Text.Length == 0)
                {
                    await DisplayAlert("Project Number Required.", "You need to provide a project number before you can validate.", "Retry");
                    txtProjectNo.Focus();
                    return;
                }
                bool bAlreadyDownloaded = await IsProjectAlreadyOnDevice(txtProjectNo.Text.Trim().ToUpper());
                if (bAlreadyDownloaded == true)
                {
                    return;
                }
                if (cmbProjectType.SelectedIndex == 1)
                {
                    await ProcessProjectSearch();
                }
                else
                {
                    await btnValidateProject();
                }


            }
            catch (Exception ex)
            {

            }
            //throw new NotImplementedException();
        }
        /// <summary>
        /// Process project search.
        /// </summary>
        private async Task ProcessProjectSearch()
        {

            WcfExt116 cAX = null;

            try
            {

                try
                {

                    //await this.EnableSearchScreenControls(false);
                    //UserDialogs.Instance.ShowLoading("Searching Projects...", MaskType.Black);
                    SetProgress(.0);
                    //lvProjects.ItemsSource = m_ocProjSearch;
                    bool bConnected = await Main.p_cSettings.IsAXSystemAvailable(true);
                    SetProgress(.1);
                    if (bConnected == true)
                    {

                        cAX = new WcfExt116();

                        //v1.0.19 - Add wild cards to search text
                        string sSearchText = Settings.AddWildCardsToSearchString(txtProjectNo.Text.Trim().ToUpper());
                        SetProgress(.2);
                        ObservableCollection<SearchResult> ocResult = await cAX.SearchForProject(sSearchText);
                        SearchResult srResult;
                        SetProgress(.3);
                        List<cBaseEnumsTable> cEnums = Main.p_cDataAccess.GetEnumsForField("Status");
                        cBaseEnumsTable cEnum = null;
                        //Clear out existing results.
                        try
                        {
                            m_ocProjSearch.Clear();
                        }
                        catch (Exception ex)
                        {
                            lvProjects.ItemsSource = null;
                        }
                        int count = ocResult.Count;
                        int index = 0;
                        //lvProjects.ItemsSource = "";
                        lvProjects.ItemsSource = m_ocProjSearch;
                        foreach (SearchResult sResult in ocResult)
                        {
                            double percent = .3 + .7 / count * index;
                            SetProgress(percent);
                            //Find matching enum.
                            cEnum = cEnums.Find(mc => mc.EnumValue.Equals(Convert.ToInt32(sResult.Status)));

                            srResult = new SearchResult();
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

                            index++;
                            m_ocProjSearch.Add(srResult);

                        }

                        //lvProjects.ItemsSource = m_ocProjSearch;

                    }

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message);
                }

                if (cAX != null)
                {
                    await DependencyService.Get<IWcfExt116>().CloseAXConnection();
                    SetProgress(.9);
                }

                //await this.EnableSearchScreenControls(true);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
            finally
            {
                SetProgress(1.0);
                //UserDialogs.Instance.HideLoading();
            }

        }
        private async void SetProgress(double percent)
        {
            await Progress.ProgressTo(percent, 250, Easing.Linear);
        }

        private async Task btnValidateProject()
        {

            WcfExt116 cAX = null;

            try
            {
                SetProgress(.0);
                //lvProjects.ItemsSource = m_ocProjSearch;
                //this.pbDownload.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                //this.tbDownloadStatus.Text = String.Empty;

                //this.m_bIgnoreReset = true;
                this.txtProjectNo.Text = this.txtProjectNo.Text.Trim().ToUpper();

                if (this.txtProjectNo.Text.Length == 0)
                {
                    await DisplayAlert("Project no required.", "You need to provide a project number before you can validate.", "OK");
                    this.txtProjectNo.Focus();
                    return;

                }


                //Check project is not already downloaded.
                bool bAlreadyDownloaded = await this.IsProjectAlreadyOnDevice(this.txtProjectNo.Text);
                SetProgress(.1);
                if (bAlreadyDownloaded == true) { return; }


                bool bConnectedOK = await Main.p_cSettings.IsAXSystemAvailable(true);
                if (bConnectedOK == true)
                {

                    SetProgress(.2);
                    await Main.CheckForUpdates();
                    SetProgress(.4);
                    cAX = new WcfExt116();
                    ProjectValidationResult cResult = await cAX.ValidateProjectNo(this.txtProjectNo.Text);
                    if (cResult != null)
                    {
                        if (cResult.bSuccessfull == true)
                        {
                            SetProgress(.6);
                            if (cResult.bProjectFound == true)
                            {

                                //Mark project as valid.
                                //this.m_bProjectValid = true;

                                //Fetch status name from base enum table.
                                string sStatus = Main.p_cDataAccess.GetEnumValueName("ProjTable", "Status", Convert.ToInt32(cResult.ValidationResult.Status));
                                SearchResult srResult = new SearchResult();
                                SetProgress(.8);
                                srResult.ProjectName = cResult.ValidationResult.ProjectName;
                                srResult.ProjectNo = cResult.ValidationResult.ProjectNo;
                                srResult.Status = sStatus;
                                try
                                {
                                    m_ocProjSearch.Clear();
                                }
                                catch (Exception ex)
                                {
                                    lvProjects.ItemsSource = null;
                                }
                                //m_ocProjSearch.Clear();
                                m_ocProjSearch.Add(srResult);
                                lvProjects.ItemsSource = m_ocProjSearch;
                                SetProgress(.9);

                                //Display project details.
                                //this.DisplayProjectDetails(cResult.ValidationResult.ProjectName, sStatus);

                                //Display download section.
                                //this.UpdateValidationStatus(ValidationStatus.Valid);
                                //this.gdDownload.Visibility = Windows.UI.Xaml.Visibility.Visible;

                            }
                            else
                            {

                                SearchResult srResult = new SearchResult();
                                SetProgress(.8);
                                srResult.ProjectName = "N/A";
                                srResult.ProjectNo = "N/A";
                                srResult.Status = "N/A";
                                try
                                {
                                    m_ocProjSearch.Clear();
                                }
                                catch (Exception ex)
                                {
                                    lvProjects.ItemsSource = null;
                                }
                                m_ocProjSearch.Add(srResult);
                                lvProjects.ItemsSource = m_ocProjSearch;
                                SetProgress(.9);
                                //Mark project as invalid.
                                //this.m_bProjectValid = false;

                                //this.UpdateValidationStatus(ValidationStatus.Invalid);
                                //this.gdDownload.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                            }

                        }
                        else
                        {

                            //Mark project as invalid.
                            //this.m_bProjectValid = false;

                            //this.UpdateValidationStatus(ValidationStatus.Error);
                            //this.gdDownload.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                        }

                    }

                }
                SetProgress(1.0);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
            }
        }
    }
}
