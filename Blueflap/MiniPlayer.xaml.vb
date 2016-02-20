' Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
''' </summary>
Public NotInheritable Class MiniPlayer
    Inherits Page

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        Dim titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView.TitleBar
        Dim v = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView()
        v.TitleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 27, 27, 27)
        v.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 27, 27, 27)
        v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.White
        v.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 27, 27, 27)
        v.TitleBar.InactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 27, 27, 27)
        Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = False
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        Player.Source = New Uri(localSettings.Values("MiniPlayerUri"))
    End Sub
End Class
