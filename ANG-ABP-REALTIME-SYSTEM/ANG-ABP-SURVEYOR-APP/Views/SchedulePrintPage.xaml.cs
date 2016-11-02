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
using Windows.UI.Xaml.Printing;
using Windows.Graphics.Printing;
using Windows.UI;
using ANG_ABP_SURVEYOR_APP_CLASS;
using ANG_ABP_SURVEYOR_APP_CLASS.Model;
using Windows.UI.Xaml.Documents;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ANG_ABP_SURVEYOR_APP.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SchedulePrintPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        PrintDocument document = null;
        IPrintDocumentSource source = null;
        List<UIElement> pages = null;
        FrameworkElement page1;
        protected event EventHandler pagesCreated;
        protected const double left = 0.075;
        protected const double top = 0.03;

        private bool m_bHeaderCreated = false;

        List<cProjectTable> lWorksDB = null;



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


        public SchedulePrintPage()
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

                document = new PrintDocument();
                source = document.DocumentSource;

                document.Paginate += printDocument_Paginate;
                document.GetPreviewPage += printDocument_GetPreviewPage;
                document.AddPages += printDocument_AddPages;

                PrintManager manager = PrintManager.GetForCurrentView();
                manager.PrintTaskRequested += manager_PrintTaskRequested;

                pages = new List<UIElement>();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);

            try
            {


                if (document == null) return;

                document.Paginate -= printDocument_Paginate;
                document.GetPreviewPage -= printDocument_GetPreviewPage;
                document.AddPages -= printDocument_AddPages;

                // Remove the handler for printing initialization.
                PrintManager manager = PrintManager.GetForCurrentView();
                manager.PrintTaskRequested -= manager_PrintTaskRequested;

                PrintContainer.Children.Clear();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnPrint_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (this.lWorksDB.Count == 0)
                {
                    await cSettings.DisplayMessage("There are no appointments to print, make sure you have selected the correct date and try-again.", "No Appointments");
                    this.dpDate.Focus(FocusState.Pointer);
                }
                else
                {
                    PrepareContent();

                    await Windows.Graphics.Printing.PrintManager.ShowPrintUIAsync();

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }



        #region "Printing Code"

        /// <summary>
        /// 
        /// </summary>
        private void PrepareContent()
        {

            try
            {

                //Reset contents.
                page1 = null;
                PrintContainer.Children.Clear();

                this.m_bHeaderCreated = false;

                page1 = this.PreparePrintPage();

                StackPanel header = (StackPanel)page1.FindName("header");

                Paragraph pHeader = (Paragraph)page1.FindName("hdReport");
                pHeader.Inlines.Add(new Windows.UI.Xaml.Documents.Run { FontSize = 24, Foreground = new SolidColorBrush(Colors.Black), Text = "Surveyor Appointments for the " + this.dpDate.Date.Date.ToString("dd/MM/yyyy") });

                header.Visibility = Windows.UI.Xaml.Visibility.Visible;

                PrintContainer.Children.Add(page1);
                PrintContainer.InvalidateMeasure();
                PrintContainer.UpdateLayout();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private PageForPrinting PreparePrintPage()
        {

            try
            {

                PageForPrinting page = new PageForPrinting();

                RichTextBlock rtbMain = (RichTextBlock)page.FindName("textContent");
                if (rtbMain != null)
                {

                    int iMaxWidth = 650;
                    double dCol1Width = iMaxWidth * 0.1;
                    double dCol2Width = iMaxWidth * 0.6;
                    double dCol3Width = iMaxWidth * 0.1;
                    double dCol4Width = iMaxWidth * 0.11;

                    Color cForeColor = Colors.Black;

                    string sVal = string.Empty;
                    var para = new Windows.UI.Xaml.Documents.Paragraph();                   
                    InlineUIContainer icContainer = new InlineUIContainer();

                    StackPanel spPanel = new StackPanel();
                    spPanel.Width = iMaxWidth;
                    //spPanel.Background = new SolidColorBrush(Colors.Green);
                    spPanel.Orientation = Orientation.Horizontal;
                    int iFontHeaderSize = 16;

                    if (this.m_bHeaderCreated == false)
                    {

                        sVal = "Time";
                        spPanel.Children.Add(this.CreateTextBox(sVal, cForeColor, dCol1Width, iFontHeaderSize, true));

                        sVal = "Address";
                        spPanel.Children.Add(this.CreateTextBox(sVal, cForeColor, dCol2Width, iFontHeaderSize, true));

                        sVal = "Project";
                        spPanel.Children.Add(this.CreateTextBox(sVal, cForeColor, dCol3Width, iFontHeaderSize, true));

                        sVal = "Job No";
                        spPanel.Children.Add(this.CreateTextBox(sVal, cForeColor, dCol4Width, iFontHeaderSize, true));

                        para.LineHeight = this.Render(para);

                        icContainer.Child = spPanel;
                        para.Inlines.Add(icContainer);

                        rtbMain.Blocks.Add(para);

                        this.m_bHeaderCreated = true;

                    }


                    int iCount = 0;
                    int iFontItemSize = 12;
                    cProjectTable cWorkDB;
                    int iProcessed = 0;

                    for (iProcessed = 0; iProcessed <= lWorksDB.Count - 1; iProcessed++)
                    {

                        cWorkDB = lWorksDB[iProcessed];

                        para = new Windows.UI.Xaml.Documents.Paragraph();
                           
                        icContainer = new InlineUIContainer();

                        spPanel = new StackPanel();
                        //spPanel.Background = new SolidColorBrush(Colors.Purple);
                        spPanel.Width = iMaxWidth;
                        spPanel.Orientation = Orientation.Horizontal;

                        string[] sParts = cMain.CreateWorkDisplayTitle(cMain.ConvertNullableDateTimeToDateTime(cWorkDB.EndDateTime)).Split('@');

                        sVal = sParts[1].Trim();
                        spPanel.Children.Add(this.CreateTextBox(sVal, cForeColor, dCol1Width, iFontItemSize, false));

                        sVal = cMain.ReturnAddress(cWorkDB);
                        spPanel.Children.Add(this.CreateTextBox(sVal, cForeColor, dCol2Width, iFontItemSize, false));

                        sVal = cWorkDB.ProjectNo;
                        spPanel.Children.Add(this.CreateTextBox(sVal, cForeColor, dCol3Width, iFontItemSize, false));

                        sVal = this.ExtractJobNo(cWorkDB.SubProjectNo);
                        spPanel.Children.Add(this.CreateTextBox(sVal, cForeColor, dCol4Width, iFontItemSize, false));

                        para.LineHeight = this.Render(para);

                        icContainer.Child = spPanel;
                        para.Inlines.Add(icContainer);

                        rtbMain.Blocks.Add(para);

                        rtbMain.InvalidateMeasure();
                        rtbMain.UpdateLayout();

                        iCount++;


                    }
               
               
                }

                return page;

            }
            catch(Exception ex)
            {
                throw new Exception("Error:(" + ex.Message + ")");


            }


        }


        private double Render(Paragraph paragraph)
        {
            var measureRtb = new RichTextBlock();
            measureRtb.Blocks.Add(paragraph);
            this.printingRoot.Children.Clear();
            this.printingRoot.Children.Add(measureRtb);
            this.printingRoot.InvalidateMeasure();
            this.printingRoot.UpdateLayout();
            measureRtb.Blocks.Clear();

            return measureRtb.ActualHeight;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void manager_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
            {

                try
                {

                    PrintTask task = null;
                    task = args.Request.CreatePrintTask("ABP Surveyor Appointment Report", sourceRequested =>
                    {
                        sourceRequested.SetSource(source);
                    });

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
            void printDocument_AddPages(object sender, AddPagesEventArgs e)
            {

                try
                {

                    for (int i = 0; i < pages.Count; i++)
                    {
                        document.AddPage(pages[i]);
                    }

                    PrintDocument printDoc = (PrintDocument)sender;
                    printDoc.AddPagesComplete();

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
            void printDocument_GetPreviewPage(object sender, GetPreviewPageEventArgs e)
            {

                try
                {

                    PrintDocument printDoc = (PrintDocument)sender;

                    printDoc.SetPreviewPage(e.PageNumber, pages[e.PageNumber - 1]);

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
            void printDocument_Paginate(object sender, PaginateEventArgs e)
            {

                try
                {

                    pages.Clear();
                    PrintContainer.Children.Clear();

                    RichTextBlockOverflow lastRTBOOnPage;
                    PrintTaskOptions printingOptions = ((PrintTaskOptions)e.PrintTaskOptions);

                    PrintPageDescription pageDescription = printingOptions.GetPageDescription(0);

                    lastRTBOOnPage = AddOnePrintPreviewPage(null, pageDescription);

                    while (lastRTBOOnPage.HasOverflowContent && lastRTBOOnPage.Visibility == Windows.UI.Xaml.Visibility.Visible)
                    {
                        lastRTBOOnPage = AddOnePrintPreviewPage(lastRTBOOnPage, pageDescription);
                    }

                    if (pagesCreated != null)
                    {
                        pagesCreated.Invoke(pages, null);
                    }

                    PrintDocument printDoc = (PrintDocument)sender;

                    printDoc.SetPreviewPageCount(pages.Count, PreviewPageCountType.Intermediate);

                }
                catch (Exception ex)
                {
                    cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

                }
                
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="lastRTBOAdded"></param>
            /// <param name="printPageDescription"></param>
            /// <returns></returns>
            private RichTextBlockOverflow AddOnePrintPreviewPage(RichTextBlockOverflow lastRTBOAdded, PrintPageDescription printPageDescription)
            {

                try
                {
                
                    FrameworkElement page;
                    RichTextBlockOverflow link;

                    if (lastRTBOAdded == null)
                    {
                        page = page1;
                        StackPanel footer = (StackPanel)page.FindName("footer");
                        footer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }
                    else
                    {
                        page = new ContinuationPage(lastRTBOAdded);
                    }

                    page.Width = printPageDescription.PageSize.Width;
                    page.Height = printPageDescription.PageSize.Height;


                    Grid printableArea = (Grid)page.FindName("printableArea");

                    double marginWidth = Math.Max(printPageDescription.PageSize.Width - printPageDescription.ImageableRect.Width, printPageDescription.PageSize.Width * left * 2);
                    double marginHeight = Math.Max(printPageDescription.PageSize.Height - printPageDescription.ImageableRect.Height, printPageDescription.PageSize.Height * top * 2);

                    printableArea.Width = page1.Width - marginWidth;
                    printableArea.Height = page1.Height - marginHeight;

                    PrintContainer.Children.Add(page);

                    PrintContainer.InvalidateMeasure();
                    PrintContainer.UpdateLayout();
               

                    // Find the last text container and see if the content is overflowing
                    link = (RichTextBlockOverflow)page.FindName("continuationPageLinkedContainer");


                    // Add the page to the page preview collection
                    pages.Add(page);               

                    return link;

                }
                catch (Exception ex)
                {
                    cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                    return null;

                }
            }

        #endregion

        /// <summary>
        /// Display the work that is upcoming on the tiles.
        /// </summary>
        private async void DisplayWorkDetails()
        {

            try
            {
             
                int iInstall_Awaiting = Convert.ToInt32(cMain.GetAppResourceValue("InstallStatus_AwaitingSurvey"));
                string sUsername = await cSettings.GetUserName();

                int iMaxWidth = (int)this.ActualWidth - 100;
                double dCol1Width = iMaxWidth * 0.1;
                double dCol2Width = iMaxWidth * 0.65;
                double dCol3Width = iMaxWidth * 0.1;
                double dCol4Width = iMaxWidth * 0.15;

                DateTime? dtDate = this.dpDate.Date.Date;

                this.rtbMain.Blocks.Clear();

                lWorksDB = cMain.p_cDataAccess.GetUpComingWork_Surveyor(sUsername, iInstall_Awaiting, dtDate);
                if (lWorksDB != null)
                {

                    string sVal = string.Empty;
                    cDashboardWork cWork = null;

                    var para = new Windows.UI.Xaml.Documents.Paragraph();
                    InlineUIContainer icContainer = new InlineUIContainer();

                    StackPanel spPanel = new StackPanel();
                    spPanel.Width = iMaxWidth;
                    spPanel.Orientation = Orientation.Horizontal;

                    int iHeaderFontSize = 24;

                    sVal = "Time";
                    spPanel.Children.Add(this.CreateTextBox(sVal, Colors.White, dCol1Width, iHeaderFontSize, true));

                    sVal = "Address";
                    spPanel.Children.Add(this.CreateTextBox(sVal, Colors.White, dCol2Width, iHeaderFontSize, true));

                    sVal = "Project";
                    spPanel.Children.Add(this.CreateTextBox(sVal, Colors.White, dCol3Width, iHeaderFontSize, true));
                   
                    sVal = "Job No";
                    spPanel.Children.Add(this.CreateTextBox(sVal, Colors.White, dCol4Width, iHeaderFontSize, true));
                    
                    icContainer.Child = spPanel;
                    para.Inlines.Add(icContainer);

                    this.rtbMain.Blocks.Add(para);


                    int iItemFontSize = 20;
                    foreach (cProjectTable cWorkDB in lWorksDB)                                       
                    {                        

                        para = new Windows.UI.Xaml.Documents.Paragraph();                     
                        icContainer = new InlineUIContainer();

                        spPanel = new StackPanel();
                        spPanel.Width = iMaxWidth;
                        spPanel.Orientation = Orientation.Horizontal;

                        string[] sParts = cMain.CreateWorkDisplayTitle(cMain.ConvertNullableDateTimeToDateTime(cWorkDB.EndDateTime)).Split('@');
                        
                        sVal = sParts[1].Trim();
                        spPanel.Children.Add(this.CreateTextBox(sVal, Colors.White, dCol1Width, iItemFontSize, false));
                  
                        sVal = cMain.ReturnAddress(cWorkDB);
                        spPanel.Children.Add(this.CreateTextBox(sVal, Colors.White, dCol2Width, iItemFontSize, false));

                        sVal = cWorkDB.ProjectNo;
                        spPanel.Children.Add(this.CreateTextBox(sVal, Colors.White, dCol3Width, iItemFontSize, false));

                        sVal = this.ExtractJobNo(cWorkDB.SubProjectNo);
                        spPanel.Children.Add(this.CreateTextBox(sVal, Colors.White, dCol4Width, iItemFontSize, false));

                        icContainer.Child = spPanel;
                        para.Inlines.Add(icContainer);

                        this.rtbMain.Blocks.Add(para);                       
                    
                    }

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
        /// <returns></returns>
        private TextBlock CreateTextBox(string v_sText,Color v_cColor,double v_dWidth,int v_iFontSize,bool v_bBold)
        {

            try
            {
              
                TextBlock tbVal = new TextBlock();
                tbVal.TextWrapping = TextWrapping.WrapWholeWords;
                tbVal.Text = v_sText;
                tbVal.Foreground = new SolidColorBrush(v_cColor);
                tbVal.Width = v_dWidth;

                if (v_bBold == true){
                    tbVal.FontWeight = Windows.UI.Text.FontWeights.Bold;
                }
                
                tbVal.FontSize = v_iFontSize;
                
                return tbVal;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return null;

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v_sSubProjectID"></param>
        /// <returns></returns>
        private string ExtractJobNo(string v_sSubProjectID)
        {

            try
            {

                string[] sParts = v_sSubProjectID.Split('-');

                return sParts[2] + "-" + sParts[1];

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return v_sSubProjectID;

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {

                this.dpDate.Date = DateTime.Now;

                this.DisplayWorkDetails();


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
        private async void dpDate_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {

            try
            {

               this.DisplayWorkDetails();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

    }
}
