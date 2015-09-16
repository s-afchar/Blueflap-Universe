Imports Windows.UI.Core
Imports Windows.UI.Notifications
''' <summary>
''' Comme son nom l'indique, Page de paramètres
''' </summary>
Public NotInheritable Class Parametres
    Inherits Page
    Public Sub New()
        Me.InitializeComponent()
        AddHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf MainPage_BackRequested
    End Sub
    Private Sub MainPage_BackRequested(sender As Object, e As Windows.UI.Core.BackRequestedEventArgs)
        If Frame.CanGoBack Then
            e.Handled = True
            Frame.GoBack()
            SaveSettings()
        End If
    End Sub
    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("CustomColorA") = 0
        localSettings.Values("CustomColorB") = 0
        localSettings.Values("CustomColorC") = 10

        Parametres_Theme() 'Permet d'appliquer le theme

        If localSettings.Values("DarkThemeEnabled") = True Then 'Definit la bonne position du toggleswitch theme
            Theme_switch.IsOn = True
        Else
            Theme_switch.IsOn = False
        End If

        If localSettings.Values("CustomColorEnabled") = True Then 'Definit la bonne position du toggleswitch theme
            Color_Switch.IsOn = True
        Else
            Color_Switch.IsOn = False
        End If

        Settings_SearchEngine.SelectedIndex = localSettings.Values("SearchEngineIndex") 'Définit la bonne valeur pour la combobox moteurs de recherches
        If Settings_SearchEngine.SelectedIndex = 12 Then
            SearchEngine_1.Visibility = Visibility.Visible
            SearchEngine_2.Visibility = Visibility.Visible
            searchengine3.Visibility = Visibility.Visible
            SearchEngine_1.Text = localSettings.Values("Cust1")
            SearchEngine_2.Text = localSettings.Values("Cust2")
        Else
            SearchEngine_1.Visibility = Visibility.Collapsed
            SearchEngine_2.Visibility = Visibility.Collapsed
            searchengine3.Visibility = Visibility.Collapsed
        End If

        Try
            Startpage_Settings.Text = localSettings.Values("Homepage")
        Catch
        End Try

        ParamOpen.Stop()
        ParamOpen.Begin()

    End Sub
    Private Sub Theme_switch_Toggled(sender As Object, e As RoutedEventArgs) Handles Theme_switch.Toggled
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("DarkThemeEnabled") = Theme_switch.IsOn 'Règle le thème en fonction du toogleswitch
        Parametres_Theme() 'Applique le theme
    End Sub
    Private Sub Parametres_Theme() 'Applique le thème
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        If localSettings.Values("DarkThemeEnabled") = True Then
            ParaPage.RequestedTheme = ElementTheme.Dark
            Dim v = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView()
            v.TitleBar.ButtonBackgroundColor = Windows.UI.Colors.Black
            v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.White
            v.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Black
        Else
            ParaPage.RequestedTheme = ElementTheme.Light
            Dim v = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView()
            v.TitleBar.ButtonBackgroundColor = Windows.UI.Colors.White
            v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.DarkGray
            v.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.White
        End If
    End Sub

    Private Sub Button_Tapped(sender As Object, e As TappedRoutedEventArgs)
        If Frame.CanGoBack Then
            Frame.GoBack()
            SaveSettings()
        End If
    End Sub

    Private Sub ChangeSearchEngine()
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
        ElseIf Settings_SearchEngine.SelectedIndex = 8 Then
            localSettings.Values("A1") = "http://www.dailymotion.com/fr/relevance/universal/search/"
            localSettings.Values("A2") = ""
        ElseIf Settings_SearchEngine.SelectedIndex = 9 Then
            localSettings.Values("A1") = "http://dreamvids.fr/search/&q"
            localSettings.Values("A2") = ""
        ElseIf Settings_SearchEngine.SelectedIndex = 10 Then
            localSettings.Values("A1") = "http://fr.wikipedia.org/w/index.php?search="
            localSettings.Values("A2") = ""
        ElseIf Settings_SearchEngine.SelectedIndex = 11 Then
            localSettings.Values("A1") = "http://twitter.com/search?q="
            localSettings.Values("A2") = ""
        ElseIf Settings_SearchEngine.SelectedIndex = 12 Then
            localSettings.Values("A1") = SearchEngine_1.Text
            localSettings.Values("A2") = SearchEngine_2.Text
            localSettings.Values("Cust1") = SearchEngine_1.Text
            localSettings.Values("Cust2") = SearchEngine_2.Text
        End If
        If Settings_SearchEngine.SelectedIndex = 12 Then
            localSettings.Values("Custom_SearchEngine") = True
            SearchEngine_1.Visibility = Visibility.Visible
            SearchEngine_2.Visibility = Visibility.Visible
            searchengine3.Visibility = Visibility.Visible
            SearchEngine_1.Text = localSettings.Values("Cust1")
            SearchEngine_2.Text = localSettings.Values("Cust2")
        Else
            localSettings.Values("Custom_SearchEngine") = False
            SearchEngine_1.Visibility = Visibility.Collapsed
            SearchEngine_2.Visibility = Visibility.Collapsed
            searchengine3.Visibility = Visibility.Collapsed
        End If

    End Sub
    Private Sub SaveSettings() 'Sauvegarde les paramètres

        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("Config") = True
        ChangeSearchEngine()
        localSettings.Values("SearchEngineIndex") = Settings_SearchEngine.SelectedIndex
        localSettings.Values("Homepage") = Startpage_Settings.Text
    End Sub

    Private Sub Startpage_Settings_TextChanged(sender As Object, e As TextChangedEventArgs) Handles Startpage_Settings.TextChanged
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("Homepage") = Startpage_Settings.Text
    End Sub

    Private Sub Settings_SearchEngine_DropDownClosed(sender As Object, e As Object) Handles Settings_SearchEngine.DropDownClosed
        ChangeSearchEngine()
    End Sub

    Private Sub Color_Switch_Toggled(sender As Object, e As RoutedEventArgs) Handles Color_Switch.Toggled
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("CustomColorEnabled") = Color_Switch.IsOn 'Règle le thème en fonction du toogleswitch
    End Sub

    Private Sub Color2_But_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Color2_But.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("CustomColorA") = localSettings.Values("CustomColorB") + 5
        localSettings.Values("CustomColorB") = localSettings.Values("CustomColorB") + 10
        localSettings.Values("CustomColorC") = localSettings.Values("CustomColorC") + 20
        SetcolorPicker()
    End Sub

    Private Sub Color3_But_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Color3_But.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("CustomColorA") = localSettings.Values("CustomColorA") + 10
        localSettings.Values("CustomColorB") = localSettings.Values("CustomColorB") + 30
        localSettings.Values("CustomColorC") = localSettings.Values("CustomColorC") + 55
        SetcolorPicker()
    End Sub

    Private Sub Color4_But_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Color4_But.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("CustomColorA") = localSettings.Values("CustomColorA") + 30
        localSettings.Values("CustomColorB") = localSettings.Values("CustomColorB") + 60
        localSettings.Values("CustomColorC") = localSettings.Values("CustomColorC") + 110
        SetcolorPicker()
    End Sub
    Private Sub SetcolorPicker()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        Dim ColA1 As String = localSettings.Values("CustomColorA") + 5
        Dim ColA2 As String = localSettings.Values("CustomColorB") + 10
        Dim ColA3 As String = localSettings.Values("CustomColorC") + 20

        If ColA1 > 254 Then
            ColA1 = ColA1 - 253
        End If
        If ColA2 > 254 Then
            ColA2 = ColA2 - 253
        End If
        If ColA3 > 254 Then
            ColA3 = ColA3 - 253
        End If

        Dim ColB1 As String = ColA1 + 5
        Dim ColB2 As String = ColA2 + 20
        Dim ColB3 As String = ColA3 + 35

        If ColB1 > 255 Then
            ColB1 = ColA1
        End If
        If ColB2 > 255 Then
            ColB2 = ColA2
        End If
        If ColB3 > 255 Then
            ColB3 = ColA3
        End If

        Dim ColC1 As String = ColB1 + 20
        Dim ColC2 As String = ColB2 + 30
        Dim ColC3 As String = ColB3 + 55

        If ColC1 > 255 Then
            ColC1 = ColB1
        End If
        If ColC2 > 255 Then
            ColC2 = ColB2
        End If
        If ColC3 > 255 Then
            ColC3 = ColB3
        End If

        Try
            Color1_But.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(ColA1 - 5, ColA2 - 10, ColA3 - 20, 100))
        Catch
            Color1_But.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(ColA1, ColA2, ColA3, 100))
        End Try

        Color2_But.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(ColA1, ColA2, ColA3, 100))
            Color3_But.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(ColB1, ColB2, ColB3, 100))
            Color4_But.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(ColC1, ColC2, ColC3, 100))

    End Sub
End Class
