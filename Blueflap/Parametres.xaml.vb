Imports Windows.UI.Core
Imports Windows.UI.Notifications
Imports Windows.Data.Json
Imports Windows.Storage
Imports Windows.UI.Xaml.Documents
Imports Windows.Web.Http
''' <summary>
''' Comme son nom l'indique, Page de paramètres
''' </summary>
Public NotInheritable Class Parametres
    Inherits Page
    Dim resourceLoader = New Resources.ResourceLoader()
#Region "Frame.GoBack"
    Public Sub New()
        Me.InitializeComponent()
        AddHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf Parametres_BackRequested
    End Sub
    Private Sub Parametres_BackRequested(sender As Object, e As Windows.UI.Core.BackRequestedEventArgs)
        'On retourne à la page principale quand le bouton retour "physique" est pressé
        If Frame.CanGoBack Then
            e.Handled = True
            SaveSettings()
            RemoveHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf Parametres_BackRequested
            Frame.GoBack()
        End If
    End Sub
    Private Sub GoBackViaButton(sender As Object, e As TappedRoutedEventArgs)
        'Retour à la page principal quand on appui sur le bouton back en haut à gauche
        If Frame.CanGoBack Then
            Frame.GoBack()
            SaveSettings()
        End If
    End Sub
#End Region
#Region "Page Loaded"
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

        If localSettings.Values("GhostMode") = False Then
            Ghost_Switch.IsOn = False
        Else
            Ghost_Switch.IsOn = True
        End If


        If localSettings.Values("KidsMode") = True Then
            Kids_Switch.IsOn = True
        Else
            Kids_Switch.IsOn = False
        End If

        If localSettings.Values("NewWin") = True Then
            NewWin_Switch.IsOn = True
        Else
            NewWin_Switch.IsOn = False
        End If

        Try
            Settings_MiniPlayer.SelectedIndex = localSettings.Values("MiniPlayerDisplayMode")
        Catch
        End Try

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
                grid.Background = DefaultThemeColor.Background
            End If
        Catch
        End Try

        Kids_Button_AllowedKeywords.Background = grid.Background
        Kids_Button_AllowedUrls.Background = grid.Background
        Kids_Button_ForbiddenKeywords.Background = grid.Background
        Kids_Button_ForbiddenUrls.Background = grid.Background
        Kids_Button_History.Background = grid.Background
        Kids_Button_ProtectionLevel.Background = grid.Background

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
            KidsStat.Text = localSettings.Values("StatKid")
            KidsStatMini.Text = localSettings.Values("StatKid")
        Catch
            localSettings.Values("StatKid") = 0
        End Try
        Kids_HistoryTile.Begin()

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


        FaviconCheck.IsChecked = localSettings.Values("Favicon")


        Try
            If localSettings.Values("SmartSuggest") = True Then
                Suggest_Switch.IsOn = True
            Else
                Suggest_Switch.IsOn = False
            End If
        Catch ex As Exception
        End Try

        Try
            If localSettings.Values("SearchFight_Menu") = False Then
                SearchFight_Menu_Switch.IsOn = False
            Else
                SearchFight_Menu_Switch.IsOn = True
            End If
        Catch ex As Exception
        End Try

        Try
            If localSettings.Values("Fav_Confirmation") = False Then
                Favorites_Confirm_Switch.IsOn = False
            Else
                Favorites_Confirm_Switch.IsOn = True
            End If
        Catch ex As Exception
        End Try

        Try
            If localSettings.Values("Context_Notif") = "No" Then
                Switch_ContextNotifs.IsOn = False
            Else
                Switch_ContextNotifs.IsOn = True
            End If
        Catch ex As Exception
        End Try

        Try

            Kids_ProtectionLevel.Text = localSettings.Values("ProtectionLevel")
            Kids_ProtectionLevelSelect.SelectedIndex = localSettings.Values("ProtectionLevel")
        Catch
            localSettings.Values("ProtectionLevel") = 0
            Kids_ProtectionLevel.Text = localSettings.Values("ProtectionLevel")
        End Try

        ParamOpen.Stop()
        ParamOpen.Begin()

        Icon_SF.IsChecked = localSettings.Values("SearchFightIcon")
        Icon_Lock.IsChecked = localSettings.Values("LockIcon")
        Icon_Note.IsChecked = localSettings.Values("NoteIcon")
        Icon_Share.IsChecked = localSettings.Values("ShareIcon")
        Icon_Window.IsChecked = localSettings.Values("WindowIcon")

        If localSettings.Values("GhostIcon") = True Then
            Icon_Ghost.IsChecked = True
        Else
            Icon_Ghost.IsChecked = False
        End If

        Try
            If localSettings.Values("WallpaperType") = "Custom" Then 'Définit l'image de fond : image prédéfinie ou image en ligne
                Wallpaper_Preview.Source = New BitmapImage(New Uri(localSettings.Values("WallpaperSource"), UriKind.Absolute))
            Else
                Wallpaper_Preview.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
            End If
        Catch ex As Exception
        End Try

        If localSettings.Values("KidsMode") = True Then

        Else
            pivot.Items.Remove(kidsDashboard)
        End If

        Try
            viewprofilpic2.ImageSource = New BitmapImage(New Uri(localSettings.Values("KidProfilPictureSource"), UriKind.Absolute))
            viewprofilpic3.ImageSource = New BitmapImage(New Uri(localSettings.Values("KidProfilPictureSource"), UriKind.Absolute))
        Catch ex As Exception
            viewprofilpic2.ImageSource = New BitmapImage(New Uri("ms-appx:/Assets/defaultkidprofilpic.jpg", UriKind.Absolute))
            viewprofilpic3.ImageSource = New BitmapImage(New Uri("ms-appx:/Assets/defaultkidprofilpic.jpg", UriKind.Absolute))
        End Try

    End Sub

    Private Sub Grid_PointerEntered(sender As Object, e As PointerRoutedEventArgs)
        scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto
    End Sub

    Private Sub Grid_PointerExited(sender As Object, e As PointerRoutedEventArgs)
        scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden
    End Sub
#End Region
#Region "Theme"
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

        Kids_Button_AllowedKeywords.Background = grid.Background
        Kids_Button_AllowedUrls.Background = grid.Background
        Kids_Button_ForbiddenKeywords.Background = grid.Background
        Kids_Button_ForbiddenUrls.Background = grid.Background
        Kids_Button_History.Background = grid.Background
        Kids_Button_ProtectionLevel.Background = grid.Background

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
    Private Sub Color1_But_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Color1_But.Tapped
        'Thème/Couleur personnalisée : Affiche/Masque les options de personnalisation avancée de la couleur de thème
        If scrollViewer1.Visibility = Visibility.Visible Then
            HideCustomColor.Begin()
        Else
            ShowCustomColor.Begin()
            Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
            RedSlider.Value = localSettings.Values("CustomColorA")
            GreenSlider.Value = localSettings.Values("CustomColorB")
            BlueSlider.Value = localSettings.Values("CustomColorC")
        End If
    End Sub

    Private Sub Color_Bleu_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Color_Bleu.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("CustomColorA") = 52
        localSettings.Values("CustomColorB") = 152
        localSettings.Values("CustomColorC") = 219
        SetcolorPicker()
    End Sub
    Private Sub Color_Rose_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Color_Rose.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("CustomColorA") = 255
        localSettings.Values("CustomColorB") = 58
        localSettings.Values("CustomColorC") = 156
        SetcolorPicker()
    End Sub
    Private Sub Color_Jaune_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Color_Jaune.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("CustomColorA") = 255
        localSettings.Values("CustomColorB") = 185
        localSettings.Values("CustomColorC") = 0
        SetcolorPicker()
    End Sub
    Private Sub Color_Violet_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Color_Violet.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("CustomColorA") = 107
        localSettings.Values("CustomColorB") = 86
        localSettings.Values("CustomColorC") = 140
        SetcolorPicker()
    End Sub
    Private Sub Color_Vert_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Color_Vert.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("CustomColorA") = 54
        localSettings.Values("CustomColorB") = 160
        localSettings.Values("CustomColorC") = 50
        SetcolorPicker()
    End Sub
    Private Sub Color_Saumon_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Color_Saumon.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("CustomColorA") = 255
        localSettings.Values("CustomColorB") = 131
        localSettings.Values("CustomColorC") = 131
        SetcolorPicker()
    End Sub
    Private Sub Color_Rose1_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Color_Rose1.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("CustomColorA") = 255
        localSettings.Values("CustomColorB") = 0
        localSettings.Values("CustomColorC") = 131
        SetcolorPicker()
    End Sub
    Private Sub Color_Rouge_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Color_Rouge.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("CustomColorA") = 234
        localSettings.Values("CustomColorB") = 17
        localSettings.Values("CustomColorC") = 29
        SetcolorPicker()
    End Sub
    Private Sub Color_Orange_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Color_Orange.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("CustomColorA") = 255
        localSettings.Values("CustomColorB") = 99
        localSettings.Values("CustomColorC") = 0
        SetcolorPicker()
    End Sub
    Private Sub Color_Gris_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Color_Gris.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("CustomColorA") = 82
        localSettings.Values("CustomColorB") = 82
        localSettings.Values("CustomColorC") = 84
        SetcolorPicker()
    End Sub

    Private Sub Color1_But_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles Color1_But.PointerEntered
        Color1_But.BorderThickness = New Thickness(2, 2, 2, 2)
    End Sub

    Private Sub Color1_But_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles Color1_But.PointerExited
        Color1_But.BorderThickness = New Thickness(0, 0, 0, 0)
    End Sub
#End Region
#Region "Search Engine"
    Private Sub ChangeSearchEngine()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        'Définit les valeurs du moteur de recherche tel que le navigateur navigue vers (A1 + Mots-clés + A2) = URI

        If Settings_SearchEngine.SelectedIndex = 1 Then
            localSettings.Values("A1") = "http://www.qwant.com/?q="
            localSettings.Values("A2") = ""
        ElseIf Settings_SearchEngine.SelectedIndex = 4 Then
            localSettings.Values("A1") = "http://www.google.com/search?q="
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
            localSettings.Values("A1") = "http://vimeo.com/search?q="
            localSettings.Values("A2") = ""
        ElseIf Settings_SearchEngine.SelectedIndex = 9 Then
            localSettings.Values("A1") = "http://www.dailymotion.com/fr/relevance/universal/search/"
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
    Private Sub Settings_SearchEngine_DropDownClosed(sender As Object, e As Object) Handles Settings_SearchEngine.DropDownClosed
        ChangeSearchEngine()
    End Sub
#End Region
#Region "Save settings"
    Private Sub SaveSettings() 'Sauvegarde les paramètres

        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("Config") = True
        ChangeSearchEngine()
        localSettings.Values("SearchEngineIndex") = Settings_SearchEngine.SelectedIndex
        localSettings.Values("MiniPlayerDisplayMode") = Settings_MiniPlayer.SelectedIndex
        localSettings.Values("Homepage") = Startpage_Settings.Text

        If Adblock_Switch.IsOn Then
            localSettings.Values("Adblock") = "En fonction"
        Else
            localSettings.Values("Adblock") = "Desactivé"
        End If
        localSettings.Values("AdblockFonction") = Adblock_Switch.Tag.ToString

        If Ghost_Switch.IsOn = True Then
            localSettings.Values("GhostMode") = True
        Else
            localSettings.Values("GhostMode") = False
        End If

        If Kids_Switch.IsOn = True Then
            localSettings.Values("KidsMode") = True
        Else
            localSettings.Values("KidsMode") = False
        End If


        If NewWin_Switch.IsOn = True Then
            localSettings.Values("NewWin") = True
        Else
            localSettings.Values("NewWin") = False
        End If

        If Suggest_Switch.IsOn Then
            localSettings.Values("SmartSuggest") = True
        Else
            localSettings.Values("SmartSuggest") = False
        End If

        If SearchFight_Menu_Switch.IsOn Then
            localSettings.Values("SearchFight_Menu") = True
        Else
            localSettings.Values("SearchFight_Menu") = False
        End If

        If Favorites_Confirm_Switch.IsOn Then
            localSettings.Values("Fav_Confirmation") = True
        Else
            localSettings.Values("Fav_Confirmation") = False
        End If

        If Switch_ContextNotifs.IsOn Then
            localSettings.Values("Context_Notif") = "Yes"
        Else
            localSettings.Values("Context_Notif") = "No"
        End If

        If Bluestart_Checkbox.IsChecked = True Then
            localSettings.Values("Bluestart") = True
        ElseIf Bluestart_Checkbox.IsChecked = False Then
            localSettings.Values("Bluestart") = False
        End If
        localSettings.Values("Favicon") = FaviconCheck.IsChecked
        localSettings.Values("SearchFightIcon") = Icon_SF.IsChecked
        localSettings.Values("LockIcon") = Icon_Lock.IsChecked
        localSettings.Values("NoteIcon") = Icon_Note.IsChecked
        localSettings.Values("ShareIcon") = Icon_Share.IsChecked
        localSettings.Values("WindowIcon") = Icon_Window.IsChecked
        localSettings.Values("GhostIcon") = Icon_Ghost.IsChecked


    End Sub
#End Region
#Region "Homepage"
    Private Sub Startpage_Settings_TextChanged(sender As Object, e As TextChangedEventArgs) Handles Startpage_Settings.TextChanged
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("Homepage") = Startpage_Settings.Text 'On enregistre la page d'accueil définie par l'utilisateur
    End Sub

    Private Sub Bluestart_Checkbox_Unchecked(sender As Object, e As RoutedEventArgs) Handles Bluestart_Checkbox.Unchecked
        'Désactivation de Bluestart
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("Bluestart") = False
    End Sub

    Private Sub Bluestart_Checkbox_Checked(sender As Object, e As RoutedEventArgs) Handles Bluestart_Checkbox.Checked
        'Activation de Bluestart
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("Bluestart") = True
    End Sub
#End Region
#Region "AdBlock"
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
#End Region
#Region "Open Source"
    Private Sub ShowHideLicense_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles ShowHideLicense.Tapped
        'Affichage de la license OpenSource avec effet d'animation
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
    Private Sub Button_Tapped_1(sender As Object, e As TappedRoutedEventArgs)
        'Ouverture du repertoire GitHub dans la fenêtre principale
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("LoadPageFromBluestart") = True
        localSettings.Values("LoadPageFromBluestart_Adress") = "https://github.com/SimpleSoftwares/Blueflap-Universe"
        RemoveHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf Parametres_BackRequested
        SaveSettings()
        Me.Frame.Navigate(GetType(MainPage))
    End Sub
#End Region
#Region "External Links"

    Private Sub Button_Tapped_2(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("LoadPageFromBluestart") = True
        localSettings.Values("LoadPageFromBluestart_Adress") = "https://www.microsoft.com/fr-fr/store/apps/bluestart/9nblggh6241x"
        RemoveHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf Parametres_BackRequested
        SaveSettings()
        Me.Frame.Navigate(GetType(MainPage))
    End Sub
    Private Async Sub Button_Tapped_4(sender As Object, e As TappedRoutedEventArgs)
        'Ouverture de la page du store associée à Blueflap
        Await Windows.System.Launcher.LaunchUriAsync(New Uri(("ms-windows-store:PDP?PFN=" + Package.Current.Id.FamilyName)))
    End Sub

    Private Sub Button_Tapped(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("LoadPageFromBluestart") = True
        localSettings.Values("LoadPageFromBluestart_Adress") = "http://goo.gl/forms/0KNjt488i5"
        RemoveHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf Parametres_BackRequested
        SaveSettings()
        Me.Frame.Navigate(GetType(MainPage))
    End Sub
    Private Sub Buton_Customize_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Buton_Customize_Button.Tapped
        Me.Frame.Navigate(GetType(Customization))
    End Sub
#End Region
#Region "Password and security"
    Private Sub Password_SaveSettings(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        ShowVerrouillagePopup.Stop()
        CloseVerrouillagePopup.Begin()

        If Not NewPasswordBox.Password = "" Then
            localSettings.Values("Password") = NewPasswordBox.Password
        End If
        NewPasswordBox.IsEnabled = True

        'TextBlockNewPassword.Text = resourceLoader.GetString("Settings_editpassword/text")
        TextBlockNewPassword.Text = Label_ChangePassword.Text
        localSettings.Values("VerrouillageEnabled") = VerrouillageSwitch.IsOn
        localSettings.Values("ShowLockScreen") = False
    End Sub
    Private Sub Password_CancelConfiguration(sender As Object, e As TappedRoutedEventArgs)
        ShowVerrouillagePopup.Stop()
        CloseVerrouillagePopup.Begin()
        NewPasswordBox.Password = ""
        NewPasswordBox.IsEnabled = True
        'TextBlockNewPassword.Text = resourceLoader.GetString("Settings_editpassword/text")
        TextBlockNewPassword.Text = Label_ChangePassword.Text
    End Sub
    Private Sub ShowPasswordConfigurationPopup(sender As Object, e As TappedRoutedEventArgs) Handles Buton_LockSetings.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        Try
            If Not localSettings.Values("Password") = "" Then
                Passwordbox.Visibility = Visibility.Visible
                Buton_LockSetings.IsEnabled = False
            Else
                CloseVerrouillagePopup.Stop()
                ShowVerrouillagePopup.Begin()
                'TextBlockNewPassword.Text = resourceLoader.GetString("Settings_newpassword/text")
                TextBlockNewPassword.Text = Label_NewPassword.Text
            End If
        Catch
            CloseVerrouillagePopup.Stop()
            ShowVerrouillagePopup.Begin()
        End Try
        Try
            If localSettings.Values("VerrouillageEnabled") = True Then 'Definit la bonne position du toggleswitch
                VerrouillageSwitch.IsOn = True
            Else
                VerrouillageSwitch.IsOn = False
            End If
        Catch
        End Try

        Try
            profilpicuri.Text = localSettings.Values("ProfilPictureSource")
        Catch ex As Exception
        End Try
        Try
            viewprofilpic.ImageSource = New BitmapImage(New Uri(localSettings.Values("ProfilPictureSource"), UriKind.Absolute))
        Catch ex As Exception
            viewprofilpic.ImageSource = New BitmapImage(New Uri("ms-appx:/Assets/defaultprofilpic.jpg", UriKind.Absolute))
        End Try
    End Sub

    Private Sub Passwordbox_KeyDown(sender As Object, e As KeyRoutedEventArgs) Handles Passwordbox.KeyDown
        If (e.Key = Windows.System.VirtualKey.Enter) Then  'Permet de réagir à l'appui sur la touche entrée
            Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
            Try
                If Passwordbox.Password = localSettings.Values("Password") Then
                    CloseVerrouillagePopup.Stop()
                    ShowVerrouillagePopup.Begin()
                    Passwordbox.Visibility = Visibility.Collapsed
                    Buton_LockSetings.IsEnabled = True
                    Passwordbox.Password = ""
                    NewPasswordBox.Password = ""
                Else
                    WrongPassword.Stop()
                    WrongPassword.Begin()
                End If
            Catch
            End Try
        End If
    End Sub

    Private Sub ResetPassword(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        NewPasswordBox.IsEnabled = False
        localSettings.Values("Password") = ""
    End Sub


#End Region
#Region "History"
    Private Sub Buton_ClearHistory_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Buton_ClearHistory.Tapped

        FlyoutBase.ShowAttachedFlyout(DirectCast(sender, FrameworkElement))

        Flyout_Confirmbut.Content = Favorites_Confirm_Switch.OnContent.ToString
        Flyout_Confirm_Label.Text = Buton_ClearHistory.Content.ToString + " ?"
        Flyout_Confirmbut.IsEnabled = True

    End Sub
    Private Async Sub WriteJsonFile(Json As JsonArray, FileName As String)
        Dim localFolder As StorageFolder = ApplicationData.Current.LocalFolder
        FileName += ".json"

        If Not Await localFolder.TryGetItemAsync(FileName) Is Nothing Then
            Dim textfile As StorageFile = Await localFolder.GetFileAsync(FileName)
            Await FileIO.WriteTextAsync(textfile, Json.ToString)
        Else
            Dim textFile As StorageFile = Await localFolder.CreateFileAsync(FileName)
            Await FileIO.WriteTextAsync(textFile, Json.ToString)
        End If

    End Sub

    Private Sub Flyout_Confirmbut_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Flyout_Confirmbut.Tapped
        Flyout_Confirmbut.IsEnabled = False
        Try
            WriteJsonFile(JsonArray.Parse("[]"), "History")
        Catch
        End Try
        Stat1.Text = 0
        Windows.Storage.ApplicationData.Current.LocalSettings.Values("Stat1") = 0
    End Sub




#End Region
#Region "JsonFileManagment"

    Private Async Function ReadJsonFile(FileName As String) As Task(Of String)
        Dim localFolder As StorageFolder = ApplicationData.Current.LocalFolder
        FileName += ".json"
        Dim content As String = Nothing

        Dim textfile As StorageFile = Await localFolder.GetFileAsync(FileName)
        content = Await FileIO.ReadTextAsync(textfile)
        Return content
    End Function


#End Region
#Region "Kids"
    Private Async Sub Button_Restrict1_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Button_Restrict1.Tapped
        If Not TextBox_Restrict1.Text = "" Then
            Try
                Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("KidsMode_ForbiddenWords"))

                Dim KidsElem As JsonObject = New JsonObject
                KidsElem.Add("word", JsonValue.CreateStringValue(TextBox_Restrict1.Text))

                root.Add(KidsElem)
                WriteJsonFile(root, "KidsMode_ForbiddenWords")

            Catch
                WriteJsonFile(JsonArray.Parse("[]"), "KidsMode_ForbiddenWords")
            End Try
        End If

        Await ShowForbiddenWords()
    End Sub
    Private Async Sub Button_Restrict_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Button_Restrict.Tapped

        If Not TextBox_Restrict.Text = "" Then
            Try
                Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("KidsMode_Words"))

                Dim KidsElem As JsonObject = New JsonObject
                KidsElem.Add("word", JsonValue.CreateStringValue(TextBox_Restrict.Text))

                root.Add(KidsElem)
                WriteJsonFile(root, "KidsMode_Words")

            Catch
                WriteJsonFile(JsonArray.Parse("[]"), "KidsMode_Words")
            End Try
        End If

        Await ShowAllowedWords()
    End Sub
    Private Sub Refresh_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Refresh.Tapped
        ShowAllowedWords()
    End Sub
    Private Sub Refresh1_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Refresh1.Tapped
        ShowForbiddenWords()
    End Sub
    Private Async Function ShowForbiddenWords() As Task


        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        Try
            StackPanel_Restrict1.Children.Clear()
            Dim Json As String

            Try
                Json = Await ReadJsonFile("KidsMode_ForbiddenWords")
            Catch ex As Exception
                Json = "[]"
            End Try

            For Each KidsElem In JsonArray.Parse(Json).Reverse

                Dim elemContainer As StackPanel = New StackPanel
                elemContainer.Padding = New Thickness(8, 8, 0, 8)

                Dim menu As MenuFlyout = New MenuFlyout
                Dim menuDelete As MenuFlyoutItem = New MenuFlyoutItem
                menuDelete.FontFamily = New FontFamily("Segoe MDL2 Assets")
                menuDelete.FontSize = 25
                menuDelete.Text = ""
                menu.Items.Add(menuDelete)


                AddHandler elemContainer.RightTapped, New RightTappedEventHandler(Function(sender As Object, e As RightTappedRoutedEventArgs)
                                                                                      menu.ShowAt(CType(sender, FrameworkElement))
                                                                                  End Function)

                AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                            menu.ShowAt(CType(sender, FrameworkElement))
                                                                        End Function)

                AddHandler menuDelete.Tapped, New TappedEventHandler(Async Sub(sender As Object, e As TappedRoutedEventArgs)
                                                                         Try
                                                                             Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("KidsMode_ForbiddenWords"))
                                                                             root.Remove(root.First(Function(x) x.GetObject.GetNamedString("word") = KidsElem.GetObject.GetNamedString("word")))
                                                                             WriteJsonFile(root, "KidsMode_ForbiddenWords")
                                                                             Await ShowForbiddenWords()
                                                                         Catch
                                                                         End Try
                                                                     End Sub)

                AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                            menu.ShowAt(CType(sender, FrameworkElement))
                                                                        End Function)



                AddHandler elemContainer.PointerEntered, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                     elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(10, 0, 0, 0))
                                                                                     elemContainer.BorderBrush = rectangle.Fill
                                                                                 End Function)

                AddHandler elemContainer.PointerExited, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                    elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
                                                                                End Function)




                Dim Name As TextBlock = New TextBlock
                Name.Text = KidsElem.GetObject.GetNamedString("word")
                elemContainer.Children.Add(Name)


                StackPanel_Restrict1.Children.Add(elemContainer)

            Next
        Catch ex As Exception
        End Try



    End Function
    Private Async Function ShowAllowedWords() As Task


        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        Try
            StackPanel_Restrict.Children.Clear()
            Dim Json As String

            Try
                Json = Await ReadJsonFile("KidsMode_Words")
            Catch ex As Exception
                Json = "[]"
            End Try

            For Each KidsElem In JsonArray.Parse(Json).Reverse

                Dim elemContainer As StackPanel = New StackPanel
                elemContainer.Padding = New Thickness(8, 8, 0, 8)

                Dim menu As MenuFlyout = New MenuFlyout
                Dim menuDelete As MenuFlyoutItem = New MenuFlyoutItem
                menuDelete.FontFamily = New FontFamily("Segoe MDL2 Assets")
                menuDelete.FontSize = 25
                menuDelete.Text = ""
                menu.Items.Add(menuDelete)


                AddHandler elemContainer.RightTapped, New RightTappedEventHandler(Function(sender As Object, e As RightTappedRoutedEventArgs)
                                                                                      menu.ShowAt(CType(sender, FrameworkElement))
                                                                                  End Function)

                AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                            menu.ShowAt(CType(sender, FrameworkElement))
                                                                        End Function)

                AddHandler menuDelete.Tapped, New TappedEventHandler(Async Sub(sender As Object, e As TappedRoutedEventArgs)
                                                                         Try
                                                                             Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("KidsMode_Words"))
                                                                             root.Remove(root.First(Function(x) x.GetObject.GetNamedString("word") = KidsElem.GetObject.GetNamedString("word")))
                                                                             WriteJsonFile(root, "KidsMode_Words")
                                                                             Await ShowAllowedWords()
                                                                         Catch
                                                                         End Try
                                                                     End Sub)

                AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                            menu.ShowAt(CType(sender, FrameworkElement))
                                                                        End Function)



                AddHandler elemContainer.PointerEntered, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                     elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(10, 0, 0, 0))
                                                                                     elemContainer.BorderBrush = rectangle.Fill
                                                                                 End Function)

                AddHandler elemContainer.PointerExited, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                    elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
                                                                                End Function)




                Dim Name As TextBlock = New TextBlock
                Name.Text = KidsElem.GetObject.GetNamedString("word")
                elemContainer.Children.Add(Name)


                StackPanel_Restrict.Children.Add(elemContainer)

            Next
        Catch ex As Exception
        End Try

    End Function


    Private Sub HideKidsElem()
        Kids_Words_Allowed.Visibility = Visibility.Collapsed
        Kids_Words_Forbidden.Visibility = Visibility.Collapsed
        Kids_link_Allowed.Visibility = Visibility.Collapsed
        Kids_link_Forbidden.Visibility = Visibility.Collapsed
        Kids_History.Visibility = Visibility.Collapsed
        Kids_SecurityLevel.Visibility = Visibility.Collapsed
        Kids_ProfilePic.Visibility = Visibility.Collapsed
    End Sub
    Private Async Sub Button_Tapped_3(sender As Object, e As TappedRoutedEventArgs)
        HideKidsElem()
        Await ShowAllowedWords()
        Kids_Words_Allowed.Visibility = Visibility.Visible
    End Sub

    Private Async Sub Kids_Button_Picture_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Kids_Button_Picture.Tapped
        HideKidsElem()
        Await ShowAllowedWords()
        Kids_ProfilePic.Visibility = Visibility.Visible
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        Try
            profilpicuri1.Text = localSettings.Values("KidProfilPictureSource")
        Catch
        End Try

        Try
            viewprofilpic1.ImageSource = New BitmapImage(New Uri(localSettings.Values("KidProfilPictureSource"), UriKind.Absolute))
        Catch ex As Exception
            viewprofilpic1.ImageSource = New BitmapImage(New Uri("ms-appx:/Assets/defaultkidprofilpic.jpg", UriKind.Absolute))
        End Try
    End Sub

    Private Async Sub Button_Tapped_5(sender As Object, e As TappedRoutedEventArgs)
        HideKidsElem()
        Await ShowForbiddenWords()
        Kids_Words_Forbidden.Visibility = Visibility.Visible
    End Sub

    Private Async Sub Button_Tapped_6(sender As Object, e As TappedRoutedEventArgs)
        HideKidsElem()
        Await ShowAllowedLinks()
        Kids_link_Allowed.Visibility = Visibility.Visible
    End Sub

    Private Async Sub Button_Tapped_7(sender As Object, e As TappedRoutedEventArgs)
        HideKidsElem()
        Await ShowForbiddenLinks()
        Kids_link_Forbidden.Visibility = Visibility.Visible
    End Sub

    Private Async Sub Button_Tapped_8(sender As Object, e As TappedRoutedEventArgs)
        HideKidsElem()
        Await ShowKidsHistory
        Kids_History.Visibility = Visibility.Visible
    End Sub

    Private Async Sub Kids_Button_ProtectionLevel_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Kids_Button_ProtectionLevel.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        HideKidsElem()
        Await ShowKidsHistory()
        Kids_SecurityLevel.Visibility = Visibility.Visible
        Kids_ProtectionLevelSelect.SelectedIndex = localSettings.Values("ProtectionLevel")

    End Sub
    Private Async Sub Button_Restrict2_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Button_Restrict2.Tapped
        If Not TextBox_Restrict2.Text = "" Then
            Try

                Dim Url = New Uri(TextBox_Restrict2.Text)

                If Url.HostNameType = UriHostNameType.Dns Then

                    Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("KidsMode_AllowedLinks"))

                    Dim KidsElem As JsonObject = New JsonObject
                    KidsElem.Add("word", JsonValue.CreateStringValue(TextBox_Restrict2.Text))

                    root.Add(KidsElem)
                    WriteJsonFile(root, "KidsMode_AllowedLinks")
                End If

            Catch
                WriteJsonFile(JsonArray.Parse("[]"), "KidsMode_AllowedLinks")
            End Try
        End If

        Await ShowAllowedLinks()
    End Sub
    Private Sub Refresh2_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Refresh2.Tapped
        ShowAllowedLinks()
    End Sub
    Private Async Function ShowAllowedLinks() As Task


        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        Try
            StackPanel_Restrict2.Children.Clear()
            Dim Json As String

            Try
                Json = Await ReadJsonFile("KidsMode_AllowedLinks")
            Catch ex As Exception
                Json = "[]"
            End Try

            For Each KidsElem In JsonArray.Parse(Json).Reverse

                Dim elemContainer As StackPanel = New StackPanel
                elemContainer.Padding = New Thickness(8, 8, 0, 8)

                Dim menu As MenuFlyout = New MenuFlyout
                Dim menuDelete As MenuFlyoutItem = New MenuFlyoutItem
                menuDelete.FontFamily = New FontFamily("Segoe MDL2 Assets")
                menuDelete.FontSize = 25
                menuDelete.Text = ""
                menu.Items.Add(menuDelete)


                AddHandler elemContainer.RightTapped, New RightTappedEventHandler(Function(sender As Object, e As RightTappedRoutedEventArgs)
                                                                                      menu.ShowAt(CType(sender, FrameworkElement))
                                                                                  End Function)

                AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                            menu.ShowAt(CType(sender, FrameworkElement))
                                                                        End Function)

                AddHandler menuDelete.Tapped, New TappedEventHandler(Async Sub(sender As Object, e As TappedRoutedEventArgs)
                                                                         Try
                                                                             Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("KidsMode_AllowedLinks"))
                                                                             root.Remove(root.First(Function(x) x.GetObject.GetNamedString("word") = KidsElem.GetObject.GetNamedString("word")))
                                                                             WriteJsonFile(root, "KidsMode_AllowedLinks")
                                                                             Await ShowForbiddenWords()
                                                                         Catch
                                                                         End Try
                                                                     End Sub)

                AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                            menu.ShowAt(CType(sender, FrameworkElement))
                                                                        End Function)



                AddHandler elemContainer.PointerEntered, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                     elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(10, 0, 0, 0))
                                                                                     elemContainer.BorderBrush = rectangle.Fill
                                                                                 End Function)

                AddHandler elemContainer.PointerExited, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                    elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
                                                                                End Function)




                Dim Name As TextBlock = New TextBlock
                Name.Text = KidsElem.GetObject.GetNamedString("word")
                elemContainer.Children.Add(Name)


                StackPanel_Restrict2.Children.Add(elemContainer)

            Next
        Catch ex As Exception
        End Try

    End Function
    Private Async Sub Button_Restrict3_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Button_Restrict3.Tapped
        If Not TextBox_Restrict3.Text = "" Then
            Try

                Dim Url = New Uri(TextBox_Restrict3.Text)

                If Url.HostNameType = UriHostNameType.Dns Then

                    Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("KidsMode_ForbiddenLinks"))

                    Dim KidsElem As JsonObject = New JsonObject
                    KidsElem.Add("word", JsonValue.CreateStringValue(TextBox_Restrict3.Text))

                    root.Add(KidsElem)
                    WriteJsonFile(root, "KidsMode_ForbiddenLinks")
                End If

            Catch
                WriteJsonFile(JsonArray.Parse("[]"), "KidsMode_ForbiddenLinks")
            End Try
        End If

        Await ShowAllowedLinks()
    End Sub
    Private Sub Refresh3_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Refresh3.Tapped
        ShowAllowedLinks()
    End Sub
    Private Async Function ShowForbiddenLinks() As Task


        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        Try
            StackPanel_Restrict3.Children.Clear()
            Dim Json As String

            Try
                Json = Await ReadJsonFile("KidsMode_ForbiddenLinks")
            Catch ex As Exception
                Json = "[]"
            End Try

            For Each KidsElem In JsonArray.Parse(Json).Reverse

                Dim elemContainer As StackPanel = New StackPanel
                elemContainer.Padding = New Thickness(8, 8, 0, 8)

                Dim menu As MenuFlyout = New MenuFlyout
                Dim menuDelete As MenuFlyoutItem = New MenuFlyoutItem
                menuDelete.FontFamily = New FontFamily("Segoe MDL2 Assets")
                menuDelete.FontSize = 25
                menuDelete.Text = ""
                menu.Items.Add(menuDelete)


                AddHandler elemContainer.RightTapped, New RightTappedEventHandler(Function(sender As Object, e As RightTappedRoutedEventArgs)
                                                                                      menu.ShowAt(CType(sender, FrameworkElement))
                                                                                  End Function)

                AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                            menu.ShowAt(CType(sender, FrameworkElement))
                                                                        End Function)

                AddHandler menuDelete.Tapped, New TappedEventHandler(Async Sub(sender As Object, e As TappedRoutedEventArgs)
                                                                         Try
                                                                             Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("KidsMode_ForbiddenLinks"))
                                                                             root.Remove(root.First(Function(x) x.GetObject.GetNamedString("word") = KidsElem.GetObject.GetNamedString("word")))
                                                                             WriteJsonFile(root, "KidsMode_ForbiddenLinks")
                                                                             Await ShowForbiddenWords()
                                                                         Catch
                                                                         End Try
                                                                     End Sub)

                AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                            menu.ShowAt(CType(sender, FrameworkElement))
                                                                        End Function)



                AddHandler elemContainer.PointerEntered, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                     elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(10, 0, 0, 0))
                                                                                     elemContainer.BorderBrush = rectangle.Fill
                                                                                 End Function)

                AddHandler elemContainer.PointerExited, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                    elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
                                                                                End Function)




                Dim Name As TextBlock = New TextBlock
                Name.Text = KidsElem.GetObject.GetNamedString("word")
                elemContainer.Children.Add(Name)


                StackPanel_Restrict3.Children.Add(elemContainer)

            Next
        Catch ex As Exception
        End Try

    End Function
    Private Async Function ShowKidsHistory() As Task

        Kids_HistoryList.Children.Clear()
        Dim Json As String
        Try
            Json = Await ReadJsonFile("KidsHistory")
        Catch ex As Exception
            Json = "[]"
        End Try
        Try
            For Each histElem In JsonArray.Parse(Json).Reverse
                Dim elemContainer As StackPanel = New StackPanel
                elemContainer.Padding = New Thickness(8, 8, 0, 8)



                Dim elemText As TextBlock = New TextBlock
                elemText.Text = histElem.GetObject.GetNamedString("title")
                elemText.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 40, 40, 40))
                elemContainer.Children.Add(elemText)

                Dim UrlText As TextBlock = New TextBlock
                UrlText.Text = histElem.GetObject.GetNamedString("url")
                UrlText.Foreground = grid.Background
                elemContainer.Children.Add(UrlText)

                Dim visitDate As TextBlock = New TextBlock
                'visitDate.Text = resourceLoader.GetString("DateList_1/Text") + DateTime.FromBinary(histElem.GetObject.GetNamedNumber("date")).ToString("dd MMMMMMMMMMMM yyyy ") + resourceLoader.GetString("DateList_2/Text") + DateTime.FromBinary(histElem.GetObject.GetNamedNumber("date")).ToString(" HH:mm")
                visitDate.Text = Label_Date1.Text + DateTime.FromBinary(histElem.GetObject.GetNamedNumber("date")).ToString("dd MMMMMMMMMMMM yyyy ") + Label_Date2.Text + DateTime.FromBinary(histElem.GetObject.GetNamedNumber("date")).ToString(" HH:mm")
                visitDate.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 150, 150, 150))
                elemContainer.Children.Add(visitDate)


                Kids_HistoryList.Children.Add(elemContainer)


            Next
        Catch

        End Try


    End Function



    Private Sub Kids_ProtectionLevelSelect_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles Kids_ProtectionLevelSelect.SelectionChanged

        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        Try
            If Kids_ProtectionLevelSelect.SelectedIndex = 0 Then
                Kids_ProtectionLevelSelect_Label1.Opacity = 1.0
                Kids_ProtectionLevelSelect_Label2.Opacity = 0.5
                Kids_ProtectionLevelSelect_Label3.Opacity = 0.5
                Kids_ProtectionLevelSelect_Label4.Opacity = 0.5

                Kids_Button_AllowedKeywords.IsEnabled = False
                Kids_Button_AllowedUrls.IsEnabled = False
                Kids_Button_ForbiddenKeywords.IsEnabled = False
                Kids_Button_ForbiddenUrls.IsEnabled = False

            ElseIf Kids_ProtectionLevelSelect.SelectedIndex = 1 Then
                Kids_ProtectionLevelSelect_Label1.Opacity = 1.0
                Kids_ProtectionLevelSelect_Label2.Opacity = 1.0
                Kids_ProtectionLevelSelect_Label3.Opacity = 0.5
                Kids_ProtectionLevelSelect_Label4.Opacity = 0.5


                Kids_Button_AllowedKeywords.IsEnabled = False
                Kids_Button_AllowedUrls.IsEnabled = False
                Kids_Button_ForbiddenKeywords.IsEnabled = True
                Kids_Button_ForbiddenUrls.IsEnabled = True

            ElseIf Kids_ProtectionLevelSelect.SelectedIndex = 2 Then
                Kids_ProtectionLevelSelect_Label1.Opacity = 1.0
                Kids_ProtectionLevelSelect_Label2.Opacity = 1.0
                Kids_ProtectionLevelSelect_Label3.Opacity = 1.0
                Kids_ProtectionLevelSelect_Label4.Opacity = 0.5

                Kids_Button_AllowedKeywords.IsEnabled = False
                Kids_Button_AllowedUrls.IsEnabled = True
                Kids_Button_ForbiddenKeywords.IsEnabled = True
                Kids_Button_ForbiddenUrls.IsEnabled = True

            ElseIf Kids_ProtectionLevelSelect.SelectedIndex = 3 Then
                Kids_ProtectionLevelSelect_Label1.Opacity = 1.0
                Kids_ProtectionLevelSelect_Label2.Opacity = 1.0
                Kids_ProtectionLevelSelect_Label3.Opacity = 1.0
                Kids_ProtectionLevelSelect_Label4.Opacity = 1.0


                Kids_Button_AllowedKeywords.IsEnabled = True
                Kids_Button_AllowedUrls.IsEnabled = True
                Kids_Button_ForbiddenKeywords.IsEnabled = True
                Kids_Button_ForbiddenUrls.IsEnabled = True

            End If

            localSettings.Values("ProtectionLevel") = Kids_ProtectionLevelSelect.SelectedIndex
            Kids_ProtectionLevel.Text = localSettings.Values("ProtectionLevel")
        Catch
        End Try
    End Sub

    Private Sub Kids_Switch_Toggled(sender As Object, e As RoutedEventArgs) Handles Kids_Switch.Toggled
        Try
            If Kids_Switch.IsOn = True Then
                pivot.Items.Add(kidsDashboard)
            Else
                pivot.Items.Remove(kidsDashboard)
            End If
        Catch
        End Try
    End Sub




#End Region

    Private Sub Button_Tapped_9(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("ProfilPictureSource") = profilpicuri.Text
        Try
            viewprofilpic.ImageSource = New BitmapImage(New Uri(localSettings.Values("ProfilPictureSource"), UriKind.Absolute))
        Catch ex As Exception
            viewprofilpic.ImageSource = New BitmapImage(New Uri("ms-appx:/Assets/defaultprofilpic.jpg", UriKind.Absolute))
        End Try
    End Sub

    Private Sub Button_Tapped_10(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("KidProfilPictureSource") = profilpicuri1.Text
        Try
            viewprofilpic1.ImageSource = New BitmapImage(New Uri(localSettings.Values("KidProfilPictureSource"), UriKind.Absolute))
            viewprofilpic2.ImageSource = New BitmapImage(New Uri(localSettings.Values("KidProfilPictureSource"), UriKind.Absolute))
            viewprofilpic3.ImageSource = New BitmapImage(New Uri(localSettings.Values("KidProfilPictureSource"), UriKind.Absolute))
        Catch ex As Exception
            viewprofilpic1.ImageSource = New BitmapImage(New Uri("ms-appx:/Assets/defaultkidprofilpic.jpg", UriKind.Absolute))
            viewprofilpic2.ImageSource = New BitmapImage(New Uri("ms-appx:/Assets/defaultkidprofilpic.jpg", UriKind.Absolute))
            viewprofilpic3.ImageSource = New BitmapImage(New Uri("ms-appx:/Assets/defaultkidprofilpic.jpg", UriKind.Absolute))
        End Try
    End Sub

End Class
