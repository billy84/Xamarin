Imports ANG_ABP_SURVEYOR_WP8.Common
Imports Windows.UI.Popups

' The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

''' <summary>
''' A basic page that provides characteristics common to most applications.
''' </summary>
Public NotInheritable Class Downloads
    Inherits Page

    Private WithEvents _navigationHelper As New NavigationHelper(Me)
    Private ReadOnly _defaultViewModel As New ObservableDictionary()

    ''' <summary>
    ''' Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
    ''' </summary>
    Public ReadOnly Property NavigationHelper As NavigationHelper
        Get
            Return _navigationHelper
        End Get
    End Property

    ''' <summary>
    ''' Gets the view model for this <see cref="Page"/>.
    ''' This can be changed to a strongly typed view model.
    ''' </summary>
    Public ReadOnly Property DefaultViewModel As ObservableDictionary
        Get
            Return _defaultViewModel
        End Get
    End Property

    ''' <summary>
    ''' Populates the page with content passed during navigation. Any saved state is also
    ''' provided when recreating a page from a prior session.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event; typically <see cref="NavigationHelper"/>.
    ''' </param>
    ''' <param name="e">Event data that provides both the navigation parameter passed to
    ''' <see cref="Frame.Navigate"/> when this page was initially requested and
    ''' a dictionary of state preserved by this page during an earlier.
    ''' session. The state will be null the first time a page is visited.</param>
    Private Sub NavigationHelper_LoadState(sender As Object, e As LoadStateEventArgs) Handles _navigationHelper.LoadState
        ' TODO: Load the saved state of the page here.
    End Sub

    ''' <summary>
    ''' Preserves state associated with this page in case the application is suspended or the
    ''' page is discarded from the navigation cache.  Values must conform to the serialization
    ''' requirements of <see cref="SuspensionManager.SessionState"/>.
    ''' </summary>
    ''' <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
    ''' <param name="e">Event data that provides an empty dictionary to be populated with
    ''' serializable state.</param>
    Private Sub NavigationHelper_SaveState(sender As Object, e As SaveStateEventArgs) Handles _navigationHelper.SaveState
        ' TODO: Save the unique state of the page here.
    End Sub

#Region "NavigationHelper registration"

    ''' <summary>
    ''' The methods provided in this section are simply used to allow
    ''' NavigationHelper to respond to the page's navigation methods.
    ''' <para>
    ''' Page specific logic should be placed in event handlers for the
    ''' <see cref="NavigationHelper.LoadState"/>
    ''' and <see cref="NavigationHelper.SaveState"/>.
    ''' The navigation parameter is available in the LoadState method
    ''' in addition to page state preserved during an earlier session.
    ''' </para>
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        _navigationHelper.OnNavigatedTo(e)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        _navigationHelper.OnNavigatedFrom(e)
    End Sub

#End Region

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Try


            Dim lsDownloads As New List(Of cDownload)

            Dim cDown As New cDownload
            cDown.ProjectNoStatus = "01217308 - In Progress"
            cDown.ProjectName = "Hanover HA Runnymeade Mews"
            Call lsDownloads.Add(cDown)

            cDown = New cDownload
            cDown.ProjectNoStatus = "01217411 - On Hold"
            cDown.ProjectName = "HG Developments - Cherry Garden Lane"
            Call lsDownloads.Add(cDown)

            Me.lvDownloads.ItemsSource = lsDownloads

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Async Sub lvDownloads_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles lvDownloads.Tapped

        Try


            ' Create the message dialog and set its content and title
            Dim messageDialog = New MessageDialog("Download project:" & Environment.NewLine & Environment.NewLine & "01217308 - In Progress" & Environment.NewLine & "Hanover HA Runnymeade Mews")

            ' Add buttons and set their callbacks
            messageDialog.Commands.Add(New UICommand("Yes", Sub(command)
                                                                Call Me.Frame.Navigate(GetType(DownloadDetail))
                                                            End Sub))

            messageDialog.Commands.Add(New UICommand("No", Nothing))


            ' Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0

            ' Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 1

            ' Show the message dialog
            Await messageDialog.ShowAsync

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub
End Class
