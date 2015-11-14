﻿Imports Windows.UI.Core
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
        'On retourne à la page principale quand le bouton retour "physique" est pressé
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

        If localSettings.Values("Adblock") = "En fonction" Then 'Censé définit la bonne position du Toggle Switch Adblock
            Adblock_Switch.IsOn = True
        Else
            Adblock_Switch.IsOn = False
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

        Try 'Applique la couleur de thème personnalisée aux paramètres (si l'option est activée)
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

        'Là Blueflap affiche les statistiques d'utilisation (dans le volet à gauche)
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

        Try
            Dim package As Windows.ApplicationModel.Package = Windows.ApplicationModel.Package.Current
            Dim packageId As Windows.ApplicationModel.PackageId = package.Id
            Dim version As Windows.ApplicationModel.PackageVersion = packageId.Version
            Dim output As String = String.Format(
      "{0}.{1}.{2}.{3}",
    version.Major, version.Minor, version.Build, version.Revision)
            Version_Info.Text = output.ToString
        Catch
            Version_Info.Text = "ERROR"
        End Try

        Try
            If localSettings.Values("Bluestart") = True Then
                Bluestart_Checkbox.IsChecked = True
            ElseIf localSettings.Values("Bluestart") = False Then
                Bluestart_Checkbox.IsChecked = False
            End If
        Catch ex As Exception
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
        'Retour à la page principal quand on appui sur le bouton back en haut à gauche
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

        If Adblock_Switch.IsOn Then
            localSettings.Values("Adblock") = "En fonction"
        Else
            localSettings.Values("Adblock") = "Desactivé"
        End If
        localSettings.Values("AdblockFonction") = Adblock_Switch.Tag.ToString

        If Bluestart_Checkbox.IsChecked = True Then
            localSettings.Values("Bluestart") = True
        ElseIf Bluestart_Checkbox.IsChecked = False
            localSettings.Values("Bluestart") = False
        End If
    End Sub

    Private Sub Startpage_Settings_TextChanged(sender As Object, e As TextChangedEventArgs) Handles Startpage_Settings.TextChanged
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("Homepage") = Startpage_Settings.Text 'On enregistre la page d'accueil définie par l'utilisateur
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
        'On applique le thème personnalisé en temps réel
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        Color1_But.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(localSettings.Values("CustomColorD"), localSettings.Values("CustomColorA"), localSettings.Values("CustomColorB"), localSettings.Values("CustomColorC")))
        grid.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(localSettings.Values("CustomColorD"), localSettings.Values("CustomColorA"), localSettings.Values("CustomColorB"), localSettings.Values("CustomColorC")))
    End Sub

    Private Sub RedSlider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs) Handles RedSlider.ValueChanged
        'On applique le thème personnalisé en temps réel
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        If Not localSettings.Values("OppeningSettings") = True Then
            localSettings.Values("CustomColorA") = RedSlider.Value
            SetcolorPicker()
        End If
    End Sub

    Private Sub GreenSlider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs) Handles GreenSlider.ValueChanged
        'On applique le thème personnalisé en temps réel
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        If Not localSettings.Values("OppeningSettings") = True Then
            localSettings.Values("CustomColorB") = GreenSlider.Value
            SetcolorPicker()
        End If
    End Sub

    Private Sub BlueSlider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs) Handles BlueSlider.ValueChanged
        'On applique le thème personnalisé en temps réel
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        If Not localSettings.Values("OppeningSettings") = True Then
            localSettings.Values("CustomColorC") = BlueSlider.Value
            SetcolorPicker()
        End If
    End Sub

    Private Sub AlphaSlider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs) Handles AlphaSlider.ValueChanged
        'On applique le thème personnalisé en temps réel
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        If Not localSettings.Values("OppeningSettings") = True Then
            localSettings.Values("CustomColorD") = AlphaSlider.Value
            SetcolorPicker()
        End If
    End Sub

    Private Sub Adblock_Switch_Toggled(sender As Object, e As RoutedEventArgs) Handles Adblock_Switch.Toggled
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        'Activation du bloqueur de pubs
        If Adblock_Switch.IsOn Then
            localSettings.Values("Adblock") = "En fonction"

        Else
            localSettings.Values("Adblock") = "Desactivé"
        End If
        localSettings.Values("AdblockFonction") = Adblock_Switch.Tag.ToString
    End Sub

    Private Sub ShowHideLicense_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles ShowHideLicense.Tapped
        If textBox.Visibility = Visibility.Visible Then
            ShowHideLicense.Content = ""
            ShowOpenSourceLicence.Stop()
            HideOpenSourceLicense.Begin()
        Else
            ShowHideLicense.Content = ""
            HideOpenSourceLicense.Stop()
            ShowOpenSourceLicence.Begin()
        End If
        textBox.Margin = New Thickness(10, 483, 0, 0)
    End Sub

    Private Sub Bluestart_Checkbox_Unchecked(sender As Object, e As RoutedEventArgs) Handles Bluestart_Checkbox.Unchecked
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("Bluestart") = False
    End Sub

    Private Sub Bluestart_Checkbox_Checked(sender As Object, e As RoutedEventArgs) Handles Bluestart_Checkbox.Checked
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("Bluestart") = True
    End Sub

    Private Sub Button_Tapped_1(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("LoadPageFromBluestart") = True
        localSettings.Values("LoadPageFromBluestart_Adress") = "https://github.com/SimpleSoftwares/Blueflap-Universe"
        Me.Frame.Navigate(GetType(MainPage))
    End Sub

    Private Sub Button_Tapped_2(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("LoadPageFromBluestart") = True
        localSettings.Values("LoadPageFromBluestart_Adress") = "https://www.microsoft.com/fr-fr/store/apps/bluestart/9nblggh6241x"
        Me.Frame.Navigate(GetType(MainPage))
    End Sub

    Private Sub Button_Tapped_3(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("LoadPageFromBluestart") = True
        localSettings.Values("LoadPageFromBluestart_Adress") = "http://personali.zz.mu/"
        Me.Frame.Navigate(GetType(MainPage))
    End Sub

    Private Async Sub Button_Tapped_4(sender As Object, e As TappedRoutedEventArgs)
        Await Windows.System.Launcher.LaunchUriAsync(New Uri(("ms-windows-store:PDP?PFN=" + Package.Current.Id.FamilyName)))
    End Sub
End Class
