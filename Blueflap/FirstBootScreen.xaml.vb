' Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
''' </summary>
Public NotInheritable Class FirstBootScreen
    Inherits Page

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        Dim titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView.TitleBar
        Dim v = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView()
        v.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 41, 27, 33)
        v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.White
        v.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 41, 27, 33)
        Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = True
    End Sub

    Private Sub Button_Tapped(sender As Object, e As TappedRoutedEventArgs)
        StepByStep.SelectedIndex = StepByStep.SelectedIndex + 1
    End Sub

    Private Sub Button_2Tapped(sender As Object, e As TappedRoutedEventArgs)
        Me.Frame.Navigate(GetType(MainPage))
    End Sub
End Class
