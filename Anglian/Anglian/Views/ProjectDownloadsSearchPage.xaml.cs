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
                txtProjectNo.Text = txtProjectNo.Text.Trim().ToUpper();
                if (txtProjectNo.Text.Length == 0)
                {
                    await DisplayAlert("Project Number Required.", "You need to provide a project number before you can validate.", "Retry");
                    txtProjectNo.Focus();
                    return;
                }
                bool bAlreadyDownloaded = await IsProjectAlreadyOnDevice(txtProjectNo.Text);
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
                    UserDialogs.Instance.ShowLoading("Searching Projects...", MaskType.Black);


                    bool bConnected = await Main.p_cSettings.IsAXSystemAvailable(true);
                    if (bConnected == true)
                    {

                        cAX = new WcfExt116();

                        //v1.0.19 - Add wild cards to search text
                        string sSearchText = Settings.AddWildCardsToSearchString(txtProjectNo.Text);

                        ObservableCollection<SearchResult> ocResult = await cAX.SearchForProject(sSearchText);
                        SearchResult srResult;

                        List<cBaseEnumsTable> cEnums = Main.p_cDataAccess.GetEnumsForField("Status");
                        cBaseEnumsTable cEnum = null;

                        //Clear out existing results.
                        this.m_ocProjSearch.Clear();

                        foreach (SearchResult sResult in ocResult)
                        {

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


                            this.m_ocProjSearch.Add(srResult);

                        }

                        lvProjects.ItemsSource = m_ocProjSearch;

                    }

                }
                catch (Exception ex)
                {


                }

                if (cAX != null)
                {
                    await DependencyService.Get<IWcfExt116>().CloseAXConnection();
                }

                //await this.EnableSearchScreenControls(true);

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }

        }
    }
}
