Imports ANG_ABP_SURVEYOR_WP8.Common

' The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

''' <summary>
''' A basic page that provides characteristics common to most applications.
''' </summary>
Public NotInheritable Class SurveyDates
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

    Private Sub SurveyDates_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        Try

            Dim lSurveyDates As New List(Of cSurveyDate)

            Dim cSurvey As New cSurveyDate
            cSurvey.Address = "23 Flexway Lane, Norwich, Norfolk"
            cSurvey.PostCodeAppointment = "NR3 1OP - 26/10/2015 PM"
            cSurvey.ProgressStatus = "Able to progress"
            cSurvey.Confirmed = "Confirmed"
            Call lSurveyDates.Add(cSurvey)

            cSurvey = New cSurveyDate
            cSurvey.Address = "The Lodge, Hasingwell Estate, Norwich, Norfolk"
            cSurvey.PostCodeAppointment = "NR33 1OP - 26/10/2015 11:35"
            cSurvey.ProgressStatus = "No access to installation"
            cSurvey.Confirmed = "Not Confirmed"
            Call lSurveyDates.Add(cSurvey)

            Me.lvToday.ItemsSource = lSurveyDates

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Sub

#Region "Command Bar Events"


    Private Sub SelectAll_Click(sender As Object, e As RoutedEventArgs)

        Try



        Catch ex As Exception

        End Try

    End Sub

    Private Sub Clear_Click(sender As Object, e As RoutedEventArgs)


        Try



        Catch ex As Exception

        End Try

    End Sub

    Private Sub SetSurveyDates_Click(sender As Object, e As RoutedEventArgs)


        Try



        Catch ex As Exception

        End Try

    End Sub

    Private Sub UnConfirm_Click(sender As Object, e As RoutedEventArgs)


        Try



        Catch ex As Exception

        End Try

    End Sub

#End Region
End Class
