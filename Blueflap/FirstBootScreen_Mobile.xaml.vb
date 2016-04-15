' Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
''' </summary>
Public NotInheritable Class FirstBootScreen_Mobile
    Inherits Page

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        Dim titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView.TitleBar
        Dim v = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView()
        v.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 240, 240, 240)
        v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.White
        v.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 240, 240, 240)
        Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = True
    End Sub

    Private Sub Button_Tapped(sender As Object, e As TappedRoutedEventArgs)
        Try
            StepByStep.SelectedIndex = StepByStep.SelectedIndex + 1
        Catch
            StepByStep.SelectedIndex = 0
        End Try
    End Sub

    Private Sub Button_2Tapped(sender As Object, e As TappedRoutedEventArgs)
        Me.Frame.Navigate(GetType(MainPage))
    End Sub

    Private Sub StepByStep_PivotItemLoaded(sender As Pivot, args As PivotItemEventArgs) Handles StepByStep.PivotItemLoaded
        If StepByStep.SelectedIndex = 0 Then
            IntroIndex1.Stop()
            IntroIndex1.Begin()
        ElseIf StepByStep.SelectedIndex = 5 Then
            LetsGo.Stop()
            LetsGo.Begin()
        End If
    End Sub

    Private Sub Button_Tapped_1(sender As Object, e As TappedRoutedEventArgs)
        Try
            StepByStep.SelectedIndex = StepByStep.SelectedIndex + 1
        Catch
            StepByStep.SelectedIndex = 0
        End Try
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("Bluestart") = False
    End Sub

    Private Sub Button_Tapped_2(sender As Object, e As TappedRoutedEventArgs)
        Settings_SearchEngine.Visibility = Visibility.Visible
        SuivBut.Visibility = Visibility.Visible
        UseQwant.Visibility = Visibility.Collapsed
        UseDuck.Visibility = Visibility.Collapsed
        UseGoogle.Visibility = Visibility.Collapsed
    End Sub

    Private Sub SuivBut_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles SuivBut.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        'Définit les valeurs du moteur de recherche tel que le navigateur navigue vers (A1 + Mots-clés + A2) = URI

        If Settings_SearchEngine.SelectedIndex = 1 Then
            localSettings.Values("A1") = "http://www.qwant.com/?q="
            localSettings.Values("A2") = ""
        ElseIf Settings_SearchEngine.SelectedIndex = 4 Then
            localSettings.Values("A1") = "http://www.google.fr/search?q="
            localSettings.Values("A2") = ""
        ElseIf Settings_SearchEngine.SelectedIndex = 0 Then
            localSettings.Values("A1") = "http://www.bing.com/search?q="
            localSettings.Values("A2") = ""
        ElseIf Settings_SearchEngine.SelectedIndex = 2 Then
            localSettings.Values("A1") = "http://duckduckgo.com/?q="
            localSettings.Values("A2") = ""
        ElseIf Settings_SearchEngine.SelectedIndex = 5 Then
            localSettings.Values("A1") = "http://www.qwant.com/?q="
            localSettings.Values("A2") = "&t=web"
        ElseIf Settings_SearchEngine.SelectedIndex = 3 Then
            localSettings.Values("A1") = "http://fr.search.yahoo.com/search;_ylt=Ai38ykBDWJSAxF25NrTnjXxNhJp4?p="
            localSettings.Values("A2") = ""
        ElseIf Settings_SearchEngine.SelectedIndex = 6 Then
            localSettings.Values("A1") = "http://fr.ask.com/web?q="
            localSettings.Values("A2") = ""
        ElseIf Settings_SearchEngine.SelectedIndex = 7 Then
            localSettings.Values("A1") = "http://www.youtube.com/results?search_query="
            localSettings.Values("A2") = ""
        ElseIf Settings_SearchEngine.SelectedIndex = 9 Then
            localSettings.Values("A1") = "http://www.dailymotion.com/fr/relevance/universal/search/"
            localSettings.Values("A2") = ""
        ElseIf Settings_SearchEngine.SelectedIndex = 8 Then
            localSettings.Values("A1") = "http://vimeo.com/search?q="
            localSettings.Values("A2") = ""
        ElseIf Settings_SearchEngine.SelectedIndex = 10 Then
            localSettings.Values("A1") = "http://fr.wikipedia.org/w/index.php?search="
            localSettings.Values("A2") = ""
        ElseIf Settings_SearchEngine.SelectedIndex = 11 Then
            localSettings.Values("A1") = "http://twitter.com/search?q="
            localSettings.Values("A2") = ""
        End If
        localSettings.Values("SearchEngineIndex") = Settings_SearchEngine.SelectedIndex
        Try
            StepByStep.SelectedIndex = StepByStep.SelectedIndex + 1
        Catch
            StepByStep.SelectedIndex = 0
        End Try
    End Sub

    Private Sub Qwant(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        'Définit les valeurs du moteur de recherche tel que le navigateur navigue vers (A1 + Mots-clés + A2) = URI

        localSettings.Values("A1") = "http://www.qwant.com/?q="
        localSettings.Values("A2") = "&t=web"
        localSettings.Values("SearchEngineIndex") = 5
        Try
            StepByStep.SelectedIndex = StepByStep.SelectedIndex + 1
        Catch
            StepByStep.SelectedIndex = 0
        End Try
    End Sub

    Private Sub DuckDuck(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        'Définit les valeurs du moteur de recherche tel que le navigateur navigue vers (A1 + Mots-clés + A2) = URI

        localSettings.Values("A1") = "http://duckduckgo.com/?q="
        localSettings.Values("A2") = ""
        localSettings.Values("SearchEngineIndex") = 2
        Try
            StepByStep.SelectedIndex = StepByStep.SelectedIndex + 1
        Catch
            StepByStep.SelectedIndex = 0
        End Try
    End Sub

    Private Sub Google(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        'Définit les valeurs du moteur de recherche tel que le navigateur navigue vers (A1 + Mots-clés + A2) = URI

        localSettings.Values("A1") = "http://www.google.fr/search?q="
        localSettings.Values("A2") = ""
        localSettings.Values("SearchEngineIndex") = 4
        Try
            StepByStep.SelectedIndex = StepByStep.SelectedIndex + 1
        Catch
            StepByStep.SelectedIndex = 0
        End Try
    End Sub

    Private Sub Button_Tapped_3(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("VerrouillageEnabled") = False
        Try
            StepByStep.SelectedIndex = StepByStep.SelectedIndex + 1
        Catch
            StepByStep.SelectedIndex = 0
        End Try
    End Sub

    Private Sub Verrouillage(sender As Object, e As TappedRoutedEventArgs)
        Verrouillageconfig.Visibility = Visibility.Visible
    End Sub

    Private Sub PasswordBox_KeyDown(sender As Object, e As KeyRoutedEventArgs)

    End Sub

    Private Sub Password3_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Password3.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("VerrouillageEnabled") = True
        localSettings.Values("Password") = Password1.Password
        Try
            StepByStep.SelectedIndex = StepByStep.SelectedIndex + 1
        Catch
            StepByStep.SelectedIndex = 0
        End Try
    End Sub

    Private Sub Password2_PasswordChanged(sender As Object, e As RoutedEventArgs) Handles Password2.PasswordChanged
        If Password2.Password = Password1.Password Then
            Password3.IsEnabled = True
        Else
            Password3.IsEnabled = False
        End If
    End Sub

    Private Sub Button_Tapped_4(sender As Object, e As TappedRoutedEventArgs)
        Me.Frame.Navigate(GetType(MainPage))
    End Sub

    Private Sub textBox_TextChanged(sender As Object, e As TextChangedEventArgs) Handles textBox.TextChanged
        If textBox.Text = "Navigate" Then
            Me.Frame.Navigate(GetType(MainPage))
        End If
    End Sub

    Private Sub Button_Tapped_5(sender As Object, e As TappedRoutedEventArgs)
        Try
            StepByStep.SelectedIndex = StepByStep.SelectedIndex + 1
        Catch
            StepByStep.SelectedIndex = 0
        End Try
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("Bluestart") = True
    End Sub
End Class
