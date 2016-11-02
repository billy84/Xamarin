Imports ANG_ABP_SURVEYOR_WP8.Common

' The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

''' <summary>
''' A basic page that provides characteristics common to most applications.
''' </summary>
Public NotInheritable Class SurveySuccessful
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

            Call Me.PopulateHealthSafetyQuestions()
            Call Me.PopulateCustomerQuestions()

        Catch ex As Exception

        End Try

    End Sub

    Public Sub PopulateHealthSafetyQuestions()

        Try

            Dim lsQuestions As New List(Of cQuestion)

            Dim cQuest As New cQuestion
            cQuest.Question = "Property Type"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Installation Type"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Access Equipment"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Services to Move"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Windowboard"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Dsbld Adaps Rqd"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Wrk Access Restrictions"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Floor Level"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Asbestos Presumed"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Door Choice Frm Recvd"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Permanent Gas Vent"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Structural Faults"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Internal Damage"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Public Protection"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            Me.lvHealthSafety.ItemsSource = lsQuestions

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PopulateCustomerQuestions()

        Try

            Dim lsQuestions As New List(Of cQuestion)

            Dim cQuest As New cQuestion
            cQuest.Question = "Resident Name"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Replacement Type"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Resident Tel No"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Resident Mobile No"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Alt Contact Tel No"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Alt Contact Name"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Alt Contact Mobile No"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            cQuest = New cQuestion
            cQuest.Question = "Special Resident Note"
            cQuest.Answer = String.Empty
            Call lsQuestions.Add(cQuest)

            Me.lvCustomer.ItemsSource = lsQuestions

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

#Region "Command Bar Events"

    Private Sub Photos_Click(sender As Object, e As RoutedEventArgs)

        Try

            Call Me.Frame.Navigate(GetType(SelectPhotos))

        Catch ex As Exception

        End Try

    End Sub

    Private Sub Notes_Click(sender As Object, e As RoutedEventArgs)


        Try

            Call Me.Frame.Navigate(GetType(Notes))

        Catch ex As Exception

        End Try

    End Sub

    Private Sub Save_Click(sender As Object, e As RoutedEventArgs)


        Try



        Catch ex As Exception

        End Try

    End Sub

#End Region
End Class
