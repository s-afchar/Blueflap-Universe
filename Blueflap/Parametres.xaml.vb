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

        Parametres_Theme() 'Permet d'appliquer le theme

        If localSettings.Values("DarkThemeEnabled") = True Then 'Definit la bonne position du toggleswitch theme
            Theme_switch.IsOn = True
        Else
            Theme_switch.IsOn = False
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
            Startpage_Settings.Text = localSettings.Values("Homepage") 'Défini la valeur de la textbox Homepage
        Catch
        End Try

        localSettings.Values("OppeningSettings") = True 'Regle les curseurs de la couleur personnalisée
        Try
            AlphaSlider.Value = localSettings.Values("CustomColorD")
            RedSlider.Value = localSettings.Values("CustomColorA")
            GreenSlider.Value = localSettings.Values("CustomColorB")
            BlueSlider.Value = localSettings.Values("CustomColorC")
            Color1_But.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(localSettings.Values("CustomColorD"), localSettings.Values("CustomColorA"), localSettings.Values("CustomColorB"), localSettings.Values("CustomColorC")))
        Catch
        End Try
        localSettings.Values("OppeningSettings") = False

        Try
            If localSettings.Values("CustomColorEnabled") = True Then
                Color_Switch.IsOn = True
                PersonnalizeColorGrid.Visibility = Visibility.Visible
                grid.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(localSettings.Values("CustomColorD"), localSettings.Values("CustomColorA"), localSettings.Values("CustomColorB"), localSettings.Values("CustomColorC")))
            Else
                Color_Switch.IsOn = False
                PersonnalizeColorGrid.Visibility = Visibility.Collapsed
            End If
        Catch
        End Try
        Try
            Stat1.Text = localSettings.Values("Stat1")
        Catch
            localSettings.Values("Stat1") = 0
        End Try

        Try
            Stat2.Text = localSettings.Values("Stat2")
        Catch
            localSettings.Values("Stat2") = 0
        End Try

        If Stat1.Text < 50 Then
            Stat3.Text = "Novice"
        ElseIf Stat1.Text < 100 Then
            Stat3.Text = "Explorer"
        ElseIf Stat1.Text < 250 Then
            Stat3.Text = "Hard Searcher"
        ElseIf Stat1.Text < 500 Then
            Stat3.Text = "Web Addict"
        ElseIf Stat1.Text < 700 Then
            Stat3.Text = "Geek"
        ElseIf Stat1.Text < 900 Then
            Stat3.Text = "Outsider from real life"
        ElseIf Stat1.Text < 5000 Then
            Stat3.Text = "Blueflap Lover"
        ElseIf 5000 < Stat1.Text Then
            Stat3.Text = "Would you marry Blueflap ?"
        End If

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

        'Règle le thème en fonction du toogleswitch
        If Color_Switch.IsOn Then
            PersonnalizeColorGrid.Visibility = Visibility.Visible
            localSettings.Values("CustomColorEnabled") = True
            grid.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(localSettings.Values("CustomColorD"), localSettings.Values("CustomColorA"), localSettings.Values("CustomColorB"), localSettings.Values("CustomColorC")))
        Else
            PersonnalizeColorGrid.Visibility = Visibility.Collapsed
            localSettings.Values("CustomColorEnabled") = False
            grid.Background = DefaultThemeColor.Background
        End If

    End Sub

    Private Sub SetcolorPicker()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        Color1_But.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(localSettings.Values("CustomColorD"), localSettings.Values("CustomColorA"), localSettings.Values("CustomColorB"), localSettings.Values("CustomColorC")))
        grid.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(localSettings.Values("CustomColorD"), localSettings.Values("CustomColorA"), localSettings.Values("CustomColorB"), localSettings.Values("CustomColorC")))
    End Sub

    Private Sub RedSlider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs) Handles RedSlider.ValueChanged
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        If Not localSettings.Values("OppeningSettings") = True Then
            localSettings.Values("CustomColorA") = RedSlider.Value
            SetcolorPicker()
        End If
    End Sub

    Private Sub GreenSlider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs) Handles GreenSlider.ValueChanged
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        If Not localSettings.Values("OppeningSettings") = True Then
            localSettings.Values("CustomColorB") = GreenSlider.Value
            SetcolorPicker()
        End If
    End Sub

    Private Sub BlueSlider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs) Handles BlueSlider.ValueChanged
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        If Not localSettings.Values("OppeningSettings") = True Then
            localSettings.Values("CustomColorC") = BlueSlider.Value
            SetcolorPicker()
        End If
    End Sub

    Private Sub AlphaSlider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs) Handles AlphaSlider.ValueChanged
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        If Not localSettings.Values("OppeningSettings") = True Then
            localSettings.Values("CustomColorD") = AlphaSlider.Value
            SetcolorPicker()
        End If
    End Sub
End Class
