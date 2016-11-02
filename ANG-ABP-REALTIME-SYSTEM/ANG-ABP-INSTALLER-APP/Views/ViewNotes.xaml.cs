using ANG_ABP_INSTALLER_APP.Common;
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
using ANG_ABP_SURVEYOR_APP_CLASS;
using ANG_ABP_SURVEYOR_APP_CLASS.Model;
using ANG_ABP_SURVEYOR_APP_CLASS.Classes;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ANG_ABP_INSTALLER_APP.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ViewNotes : Page
    {


        /// <summary>
        /// Flag to indicate page has been loaded.
        /// </summary>
        private bool m_bPageLoaded = false;

        /// <summary>
        /// Holds project details.
        /// </summary>
        private cProjectTable m_cProject = null;

        /// <summary>
        /// Store the project notes.
        /// </summary>
        private List<cProjectNotesTable> m_cProjectNotes = null;


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


        public ViewNotes()
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


            try
            {
            
                if (e.Parameter != null)
                {
                    if (e.Parameter.GetType() == typeof(string))
                    {

                        this.m_cProject = cMain.p_cDataAccess.GetSubProjectProjectData(e.Parameter.ToString());
                        this.m_cProjectNotes = cMain.p_cDataAccess.GetSubProjectNotesData(e.Parameter.ToString()); //v1.0.1 - Fetch notes.

                    }

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

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
        private void ViewNotes_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {

                //Display sub project details.
                this.tbSubProjectTitle.Text = this.m_cProject.SubProjectNo + " / " + cMain.RemoveNewLinesFromString(this.m_cProject.DeliveryStreet);

                //Display notes on screen.
                this.RefreshNotesList();              

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
            
        }


        /// <summary>
        /// Refresh notes list.
        /// </summary>
        private void RefreshNotesList()
        {

            try
            {

                //Create new instance, we need to bind to list view.
                List<cNotesHistory> cNotes = new List<cNotesHistory>();
                cNotesHistory cNote = null;

                //v1.0.1 - Order by input date time.
                var oResult = (from oCols in this.m_cProjectNotes
                               orderby oCols.InputDateTime descending
                               select oCols);


                foreach (cProjectNotesTable cProjNote in oResult)
                {
                    cNote = new cNotesHistory();
                    cNote.InputDateTime = cProjNote.InputDateTime;
                    cNote.NoteText = cProjNote.NoteText;
                    cNote.UserName = cProjNote.UserName;

                    cNote.ListViewWidth = (this.txtNewNote.ActualWidth + this.btnSaveNote.ActualWidth);                   

                    cNotes.Add(cNote);

                }

                //v1.0.1 - Update notes.
                this.lvNotes.ItemsSource = null;
                this.lvNotes.ItemsSource = cNotes;
                this.lvNotes.UpdateLayout();

                this.tbNoteCount.Text = cNotes.Count.ToString() + " notes listed";

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
        private async void btnSaveNote_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                this.txtNewNote.Text = this.txtNewNote.Text.Trim();
                if (this.txtNewNote.Text.Length > 0)
                {
                    string sNoteText = this.txtNewNote.Text;

                    //v1.0.1 - Add notes the notes collection
                    cProjectNotesTable cNote = await cSettings.ReturnNoteObject(this.m_cProject.SubProjectNo, sNoteText, DateTime.Now, cSettings.p_sProjectNoteType_General);
                    cMain.p_cDataAccess.SaveSubProjectNote(cNote);
                    this.m_cProjectNotes.Add(cNote);

                    this.txtNewNote.Text = String.Empty;
                    this.txtNewNote.Focus(FocusState.Programmatic);

                    this.RefreshNotesList();
                }


            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }
    }
}
