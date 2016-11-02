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
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Shapes;
using Windows.Devices.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input;
using System.Threading.Tasks;
using ANG_ABP_SURVEYOR_APP_CLASS;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ANG_ABP_SURVEYOR_APP.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SignaturePage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// Ink Manager object.
        /// </summary>
        InkManager m_inkManager = new Windows.UI.Input.Inking.InkManager();


        const double STROKETHICKNESS = 5;
        Point _previousContactPt;
        uint _penID = 0;
        uint _touchID = 0;

        /// <summary>
        /// Sub project number passed by parameter.
        /// </summary>
        private string m_sSubProjectNo = String.Empty;

        /// <summary>
        /// Existing signature file.
        /// </summary>
        private StorageFile m_sfSignature = null;

        /// <summary>
        /// v1.0.2 - Flag to indicate that screen has been drawn on.
        /// </summary>
        private bool m_bScreenUpdated = false;


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


        public SignaturePage()
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

                    this.m_sSubProjectNo = e.Parameter.ToString();
                   
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
        /// Page load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {

                //Assign handlers to canvas
                this.cvSignature.PointerPressed += new PointerEventHandler(InkCanvas_PointerPressed);
                this.cvSignature.PointerMoved += new PointerEventHandler(InkCanvas_PointerMoved);
                this.cvSignature.PointerReleased += new PointerEventHandler(InkCanvas_PointerReleased);
                this.cvSignature.PointerExited += new PointerEventHandler(InkCanvas_PointerReleased);


                //Check for existing signature.
                this.CheckForExistingSignature();


                //v1.0.2 - Reset updated flag.
                this.m_bScreenUpdated = false;

               
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Check for existing signature.
        /// </summary>
        private async void CheckForExistingSignature()
        {

            try
            {

                this.m_sfSignature  = await cMain.ReturnSignatureFile(this.m_sSubProjectNo);
                if (this.m_sfSignature != null)
                {

                    IRandomAccessStream readStream = await this.m_sfSignature.OpenAsync(FileAccessMode.Read);
                    IInputStream inputStream = readStream.GetInputStreamAt(0);
                    await this.m_inkManager.LoadAsync(inputStream);

                    IReadOnlyList<InkStroke> strokes = this.m_inkManager.GetStrokes();
                    
                    if (strokes.Count > 0)
                    {                           
                        this.RenderAllStrokes();
                    }
                                
                }

            }
             catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Pointer pressed onto screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void InkCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

            try
            {

                // Get information about the pointer location.
                PointerPoint pt = e.GetCurrentPoint(this.cvSignature);
                _previousContactPt = pt.Position;

                // Accept input only from a pen or mouse with the left button pressed. 
                PointerDeviceType pointerDevType = e.Pointer.PointerDeviceType;
                if (pointerDevType == PointerDeviceType.Pen ||
                        pointerDevType == PointerDeviceType.Mouse &&
                        pt.Properties.IsLeftButtonPressed)
                {
                    // Pass the pointer information to the InkManager.
                    this.m_inkManager.ProcessPointerDown(pt);
                    _penID = pt.PointerId;

                    e.Handled = true;
                }

                else if (pointerDevType == PointerDeviceType.Touch)
                {
                    // Process touch input
                    // Pass the pointer information to the InkManager.
                    this.m_inkManager.ProcessPointerDown(pt);
                    _penID = pt.PointerId;

                    e.Handled = true;
                }



            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
            
        }

        /// <summary>
        /// Draw on the canvas and capture ink data as the pointer moves.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void InkCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {

            try
            {

                if (e.Pointer.PointerId == _penID)
                {
                    PointerPoint pt = e.GetCurrentPoint(this.cvSignature);

                    // Render a red line on the canvas as the pointer moves. 
                    // Distance() is an application-defined function that tests
                    // whether the pointer has moved far enough to justify 
                    // drawing a new line.
                    Point currentContactPt = pt.Position;
                    if (Distance(currentContactPt, _previousContactPt) > 2)
                    {
                        Line line = new Line()
                        {
                            X1 = _previousContactPt.X,
                            Y1 = _previousContactPt.Y,
                            X2 = currentContactPt.X,
                            Y2 = currentContactPt.Y,
                            StrokeThickness = STROKETHICKNESS,
                            Stroke = new SolidColorBrush(Windows.UI.Colors.Black)
                        };

                        _previousContactPt = currentContactPt;

                        // Draw the line on the canvas by adding the Line object as
                        // a child of the Canvas object.
                        this.cvSignature.Children.Add(line);

                        // Pass the pointer information to the InkManager.
                        this.m_inkManager.ProcessPointerUpdate(pt);
                    }
                }

                else if (e.Pointer.PointerId == _touchID)
                {
                    // Process touch input
                }

                e.Handled = true;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
           
        }

        /// <summary>
        /// Finish capturing ink data and use it to render ink strokes on the canvas. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void InkCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {

            try
            {

                if (e.Pointer.PointerId == _penID)
                {
                    PointerPoint pt = e.GetCurrentPoint(this.cvSignature);

                    // Pass the pointer information to the InkManager. 
                    this.m_inkManager.ProcessPointerUp(pt);

                    //v1.0.2 - Screen updated.
                    this.m_bScreenUpdated = true;
                }

                else if (e.Pointer.PointerId == _touchID)
                {
                    // Process touch input

                    //v1.0.2 - Screen updated.
                    this.m_bScreenUpdated = true;
                }

                _touchID = 0;
                _penID = 0;

                // Call an application-defined function to render the ink strokes.
                RenderAllStrokes();


              

                e.Handled = true;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
           
        }

 
        /// <summary>
        /// Render ink strokes as cubic bezier segments.
        /// </summary>
        private void RenderAllStrokes()
        {

            try
            {

                  this.cvSignature.Children.Clear();

                // Get the InkStroke objects.
                IReadOnlyList<InkStroke> inkStrokes = this.m_inkManager.GetStrokes();

                // Process each stroke.
                foreach (InkStroke inkStroke in inkStrokes)
                {
                    PathGeometry pathGeometry = new PathGeometry();
                    PathFigureCollection pathFigures = new PathFigureCollection();
                    PathFigure pathFigure = new PathFigure();
                    PathSegmentCollection pathSegments = new PathSegmentCollection();

                    // Create a path and define its attributes.
                    Windows.UI.Xaml.Shapes.Path path = new Windows.UI.Xaml.Shapes.Path();
                    path.Stroke = new SolidColorBrush(Colors.Black);
                    path.StrokeThickness = STROKETHICKNESS;

                    // Get the stroke segments.
                    IReadOnlyList<InkStrokeRenderingSegment> segments;
                    segments = inkStroke.GetRenderingSegments();

                    // Process each stroke segment.
                    bool first = true;
                    foreach (InkStrokeRenderingSegment segment in segments)
                    {
                        // The first segment is the starting point for the path.
                        if (first)
                        {
                            pathFigure.StartPoint = segment.BezierControlPoint1;
                            first = false;
                        }

                        // Copy each ink segment into a bezier segment.
                        BezierSegment bezSegment = new BezierSegment();
                        bezSegment.Point1 = segment.BezierControlPoint1;
                        bezSegment.Point2 = segment.BezierControlPoint2;
                        bezSegment.Point3 = segment.Position;

                        // Add the bezier segment to the path.
                        pathSegments.Add(bezSegment);
                    }

                    // Build the path geometerty object.
                    pathFigure.Segments = pathSegments;
                    pathFigures.Add(pathFigure);
                    pathGeometry.Figures = pathFigures;

                    // Assign the path geometry object as the path data.
                    path.Data = pathGeometry;

                    // Render the path by adding it as a child of the Canvas object.
                  this.cvSignature.Children.Add(path);
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
        /// <param name="currentContact"></param>
        /// <param name="previousContact"></param>
        /// <returns></returns>
        private double Distance(Point currentContact, Point previousContact)
        {

            try
            {
                return Math.Sqrt(Math.Pow(currentContact.X - previousContact.X, 2) +
                   Math.Pow(currentContact.Y - previousContact.Y, 2));

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
           
        }

        /// <summary>
        /// Reset signature panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResetSignature_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //v1.0.2 - If screen used 
                if (this.cvSignature.Children.Count > 0)
                {
                    this.m_bScreenUpdated = true;
                }
             
                this.cvSignature.Children.Clear();
                this.m_inkManager = null;
                this.m_inkManager = new InkManager();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Signature has been completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnSignatureCompleted_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (this.cvSignature.Visibility == Windows.UI.Xaml.Visibility.Visible)
                {

                    bool bSaveOK = await this.SaveSignature();
                    if (bSaveOK == true)
                    {

                        //Go back to the previous page.
                        this.navigationHelper.GoBack();

                    }

                  
                }
               
            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// v1.0.2 - Save signature
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SaveSignature()
        {

            try
            {

                if (this.cvSignature.Children.Count == 0)
                {

                    await cSettings.DisplayMessage("You need to provide a signature before you can complete, please amend and try again.", "Signature Required");
                    return false;

                }


                bool bSavedOK = await this.SaveSignatureFile();
                if (bSavedOK == false)
                {

                    await cSettings.DisplayMessage("A problem occurred when trying to save the signature, please try again.", "Error Saving");
                    return false;

                }

                return true;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }


        }

        /// <summary>
        /// Save signature file
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SaveSignatureFile()
        {

            IRandomAccessStream wsSignature = null;
            IOutputStream osSignature = null;
            try
            {            

                //Get the correct name for a signature file.
                string sSignatureFile = cMain.ReturnSignatureFileName(this.m_sSubProjectNo);

                //Fetch sub project folder.
                StorageFolder sfSubProject = await cSettings.ReturnSubProjectImagesFolder(this.m_sSubProjectNo);

                //Create file ready for signature.
                StorageFile sfSignature = await sfSubProject.CreateFileAsync(sSignatureFile, CreationCollisionOption.ReplaceExisting);

                //Open file for writing.
                wsSignature = await sfSignature.OpenAsync(FileAccessMode.ReadWrite);
                osSignature = wsSignature.GetOutputStreamAt(0);

                //Save signature details.
                await this.m_inkManager.SaveAsync(osSignature);

                //Flush all data out to file.
                await osSignature.FlushAsync();

                //Retrieve file properties, we need the modified date.
                Windows.Storage.FileProperties.BasicProperties bpSignatureFile = await sfSignature.GetBasicPropertiesAsync();

                //Save file record into files table.
                bool bSavesOK = cMain.p_cDataAccess.SaveSubProjectFile(this.m_sSubProjectNo, sSignatureFile, "Customer Signature File", bpSignatureFile.DateModified.LocalDateTime,true);
                if (bSavesOK == false)
                {
                    throw new Exception("Unable to save file details to database (" + sSignatureFile + ")");

                }
                
                return true;

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }
            finally
            {

                //Clean up.
                if (wsSignature != null)
                {
                    wsSignature.Dispose();
                }

                //Clean up/
                if (osSignature != null)
                {
                    osSignature.Dispose();
                }
                
            }

        }

        /// <summary>
        /// v1.0.2 - Back button clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void backButton_Click(object sender, RoutedEventArgs e)
        {

            bool bGoBack = true;
            try
            {

                if (this.m_bScreenUpdated == true)
                {

                    cSettings.YesNoCancel mResponse = await cSettings.PromptForUnSavedChanges();
                    if (mResponse == cSettings.YesNoCancel.Yes)
                    {
                        bool bSaveOK = await this.SaveSignature();
                        if (bSaveOK == false)
                        {
                            bGoBack = false;
                        }


                    }
                    else if (mResponse == cSettings.YesNoCancel.No)
                    {


                    }
                    else if (mResponse == cSettings.YesNoCancel.Cancel)
                    {
                        bGoBack = false;

                    }

                }
               

                if (bGoBack == true)
                {
                    this.navigationHelper.GoBack();

                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

    }
}
