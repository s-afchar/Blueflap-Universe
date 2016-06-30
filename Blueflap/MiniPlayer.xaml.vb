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

        If (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) Then
            Dim HttpRequestMessage = New Windows.Web.Http.HttpRequestMessage(Windows.Web.Http.HttpMethod.Get, New Uri(localSettings.Values("MiniPlayerUri")))
            HttpRequestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.246")
            Player.NavigateWithHttpRequestMessage(HttpRequestMessage)
        Else
            Player.Source = New Uri(localSettings.Values("MiniPlayerUri"))
        End If


    End Sub
End Class
