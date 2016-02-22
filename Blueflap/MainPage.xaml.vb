﻿Imports Windows.UI.Notifications
Imports Windows.ApplicationModel.DataTransfer
Imports Windows.UI.Xaml.Controls
Imports Windows.ApplicationModel.Core
Imports Windows.Data.Json
Imports Windows.UI.Xaml.Documents
''' <summary>
''' Page dédiée à la navigation web
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page
    Dim NotifPosition As String
#Region "HardwareBackButton"
    Public Sub New()
        Me.InitializeComponent()
        AddHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf MainPage_BackRequested
    End Sub
    Private Sub MainPage_BackRequested(sender As Object, e As Windows.UI.Core.BackRequestedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        'Appui sur la touche retour "physique" d'un appareil Windows
        e.Handled = True
        If web.CanGoBack Then
            e.Handled = True
            web.GoBack()
        End If
    End Sub
#End Region
#Region "Page Loaded"
    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        AddHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf MainPage_BackRequested
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings 'Permet l'accés aux paramètres

        FirstLaunch()

        SmartSuggest.Visibility = Visibility.Collapsed

        If localSettings.Values("DarkThemeEnabled") = True Then 'Theme Sombre

            LightThemeMainPage.Stop() 'Solution provisoire parce que j'avais la flemme de chercher le code Argb
            BlackThemeMainPage.Begin() 'Idem (Je suis sûr qu'au final je ne le changerai pas parce que j'aurai des trucs plus importants à faire

            'Supprime la titlebar
            Dim titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView.TitleBar
            Dim v = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView()
            v.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 27, 27, 27)
            v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.White
            v.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 27, 27, 27)
            Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = True

            AdressBox.Foreground = New SolidColorBrush(Windows.UI.Colors.White)

        Else 'Theme Clair

            'Bon... Je vais pas recommenter la même chose... Débrouillez vous avec ce qu'il y a avant...
            BlackThemeMainPage.Stop()
            LightThemeMainPage.Begin()

            Dim titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView.TitleBar
            Dim v = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView()
            v.TitleBar.ButtonBackgroundColor = Windows.UI.Colors.WhiteSmoke
            v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.DarkGray
            v.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.WhiteSmoke
            Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = True

            Adressbar.Background = New SolidColorBrush(Windows.UI.Colors.WhiteSmoke)
            AdressBox.Foreground = New SolidColorBrush(Windows.UI.Colors.DimGray)
        End If

        AdressBox.IsEnabled = False 'Autre solution provisoire (qui va sans doute rester) parce que sinon l'adressbox obtient le focus à l'ouverture allez savoir pourquoi...
        AdressBox.IsEnabled = True

        BackForward()

        If localSettings.Values("ShowLockScreen") = False Then 'Permet d'éviter un revérouillage systématique du navigateur
            LockTheBrowser.IsChecked = False
        End If
        localSettings.Values("ShowLockScreen") = True

        Try
            If localSettings.Values("VerrouillageEnabled") = True And LockTheBrowser.IsChecked = True Then
                LockTheBrowser.IsChecked = True
                Me.Frame.Navigate(GetType(Verrouillage))
            ElseIf localSettings.Values("Bluestart") = True And AdressBox.Text = "about:blank" And Frame.CanGoBack = False Then
                Me.Frame.Navigate(GetType(Bluestart))
            ElseIf AdressBox.Text = "about:blank" Then
                'Vérification de l'existence d'une page d'accueil valide
                Try
                    web.Navigate(New Uri(localSettings.Values("Homepage")))

                    'Met à jour les éléments du centre de messages systèmes
                    If Notif_HomePageError.Visibility = Visibility.Visible Then
                        Notif_HomePageError.Visibility = Visibility.Collapsed
                        New_Notif.Stop()
                        Notification()
                        Notif_SearchEngineError.Margin = New Thickness(0, NotifPosition, 0, 0)
                    End If

                Catch
                    Dim notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02)
                    Dim toeastElement = notificationXml.GetElementsByTagName("text")
                    toeastElement(0).AppendChild(notificationXml.CreateTextNode("Erreur page d'accueil"))
                    toeastElement(1).AppendChild(notificationXml.CreateTextNode("La page d'accueil définie est invalide. Rendez-vous dans les paramètres et vérifiez la configuration de votre page d'accueil."))
                    Dim ToastNotification = New ToastNotification(notificationXml)
                    ToastNotificationManager.CreateToastNotifier().Show(ToastNotification)

                    'Met à jour les éléments du centre de messages systèmes
                    If Not Notif_HomePageError.Visibility = Visibility.Visible Then

                        New_Notif.Begin()
                        Notifications_Counter.Text = Notifications_Counter.Text + 1
                        Notif_HomePageError.Visibility = Visibility.Visible
                        Notification()


                        Notif_Home.Visibility = Visibility.Collapsed
                    End If
                End Try
            End If
        Catch
            localSettings.Values("Bluestart") = True
        End Try

        'Définition du thème avec couleur personnalisée
        Try
            If localSettings.Values("CustomColorEnabled") = True Then
                LeftMenu.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(localSettings.Values("CustomColorD"), localSettings.Values("CustomColorA"), localSettings.Values("CustomColorB"), localSettings.Values("CustomColorC")))
            Else
                LeftMenu.Background = DefaultThemeColor.Background
            End If
        Catch
        End Try

        Try
            If localSettings.Values("LoadPageFromBluestart") = True Then
                web.Stop()
                AdressBox.Text = localSettings.Values("LoadPageFromBluestart_Adress")
                Rechercher()
            End If
        Catch
        End Try

        'Quelles icônes sont affichées

        If localSettings.Values("SearchFightIcon") = False Then
            Fight_Button.Visibility = Visibility.Collapsed
        Else
            Fight_Button.Visibility = Visibility.Visible
        End If

        If localSettings.Values("LockIcon") = False Then
            Lock_Button.Visibility = Visibility.Collapsed
        Else
            Lock_Button.Visibility = Visibility.Visible
        End If

        If localSettings.Values("NoteIcon") = False Then
            Memo_Button.Visibility = Visibility.Collapsed
        Else
            Memo_Button.Visibility = Visibility.Visible
        End If

        If localSettings.Values("ShareIcon") = False Then
            Share_Button.Visibility = Visibility.Collapsed
        Else
            Share_Button.Visibility = Visibility.Visible
        End If

        If localSettings.Values("WindowIcon") = False Then
            Window_Button.Visibility = Visibility.Collapsed
        Else
            Window_Button.Visibility = Visibility.Visible
        End If

        If MemoPanel.Visibility = Visibility.Visible And RightMenuPivot.SelectedIndex = 1 Then
            ShowFavorites()
        End If
        If MemoPanel.Visibility = Visibility.Visible And RightMenuPivot.SelectedIndex = 2 Then
            ShowHistory()
        End If

        'Animation d'ouverture de Blueflap
        EnterAnim.Begin()
    End Sub
#End Region
#Region "Webview : Navigation"
    Private Sub BackForward()
        'Blueflap gère ici le placement des boutons dans le menu latéral après chaque page chargée

        StopEnabled.Stop() 'Ce sont des animation pour le bouton stop/Refresh
        RefreshEnabled.Begin()

        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        'A terme ceci sera dynamique en fonction des éléments que l'utilisateur a choisi pour le menu

        localSettings.Values("FightBut_Top") = 242
        localSettings.Values("lockBut_Top") = 286
        localSettings.Values("memoBut_Top") = 330
        localSettings.Values("ShareBut_Top") = 374
        localSettings.Values("WindowBut_Top") = 418

        Try
            If localSettings.Values("SearchFightIcon") = False Then
                localSettings.Values("lockBut_Top") = localSettings.Values("lockBut_Top") - 44
                localSettings.Values("memoBut_Top") = localSettings.Values("memoBut_Top") - 44
                localSettings.Values("ShareBut_Top") = localSettings.Values("ShareBut_Top") - 44
                localSettings.Values("WindowBut_Top") = localSettings.Values("WindowBut_Top") - 44
            End If

            If localSettings.Values("LockIcon") = False Then
                localSettings.Values("memoBut_Top") = localSettings.Values("memoBut_Top") - 44
                localSettings.Values("ShareBut_Top") = localSettings.Values("ShareBut_Top") - 44
                localSettings.Values("WindowBut_Top") = localSettings.Values("WindowBut_Top") - 44
            End If

            If localSettings.Values("NoteIcon") = False Then
                localSettings.Values("ShareBut_Top") = localSettings.Values("ShareBut_Top") - 44
                localSettings.Values("WindowBut_Top") = localSettings.Values("WindowBut_Top") - 44
            End If

            If localSettings.Values("ShareIcon") = False Then
                localSettings.Values("WindowBut_Top") = localSettings.Values("WindowBut_Top") - 44
            End If

            If localSettings.Values("WindowIcon") = False Then

            End If
        Catch
        End Try


        If web.CanGoBack And web.CanGoForward Then
            Back_Button.Visibility = Visibility.Visible
            Forward_Button.Visibility = Visibility.Visible
            Fight_Button.Margin = New Thickness(3, localSettings.Values("FightBut_Top"), -1, 0)
            Lock_Button.Margin = New Thickness(3, localSettings.Values("lockBut_Top"), -1, 0)
            Memo_Button.Margin = New Thickness(3, localSettings.Values("memoBut_Top"), -1, 0)
            Share_Button.Margin = New Thickness(3, localSettings.Values("ShareBut_Top"), -1, 0)
            Window_Button.Margin = New Thickness(3, localSettings.Values("WindowBut_Top"), -1, 0)
            Forward_Button.Margin = New Thickness(3, 198, -1, 0)

        ElseIf web.CanGoBack And web.CanGoForward = False Then
            Back_Button.Visibility = Visibility.Visible
            Forward_Button.Visibility = Visibility.Collapsed
            Fight_Button.Margin = New Thickness(3, localSettings.Values("FightBut_Top") - 44, -1, 0)
            Lock_Button.Margin = New Thickness(3, localSettings.Values("lockBut_Top") - 44, -1, 0)
            Memo_Button.Margin = New Thickness(3, localSettings.Values("memoBut_Top") - 44, -1, 0)
            Share_Button.Margin = New Thickness(3, localSettings.Values("ShareBut_Top") - 44, -1, 0)
            Window_Button.Margin = New Thickness(3, localSettings.Values("WindowBut_Top") - 44, -1, 0)

        ElseIf web.CanGoBack = False And web.CanGoForward Then
            Back_Button.Visibility = Visibility.Collapsed
            Forward_Button.Visibility = Visibility.Visible
            Fight_Button.Margin = New Thickness(3, localSettings.Values("FightBut_Top") - 44, -1, 0)
            Lock_Button.Margin = New Thickness(3, localSettings.Values("lockBut_Top") - 44, -1, 0)
            Memo_Button.Margin = New Thickness(3, localSettings.Values("memoBut_Top") - 44, -1, 0)
            Share_Button.Margin = New Thickness(3, localSettings.Values("ShareBut_Top") - 44, -1, 0)
            Window_Button.Margin = New Thickness(3, localSettings.Values("WindowBut_Top") - 44, -1, 0)
            Forward_Button.Margin = New Thickness(3, 154, -1, 0)

        ElseIf web.CanGoBack = False And web.CanGoForward = False Then
            Back_Button.Visibility = Visibility.Collapsed
            Forward_Button.Visibility = Visibility.Collapsed
            Fight_Button.Margin = New Thickness(3, localSettings.Values("FightBut_Top") - 88, -1, 0)
            Lock_Button.Margin = New Thickness(3, localSettings.Values("lockBut_Top") - 88, -1, 0)
            Memo_Button.Margin = New Thickness(3, localSettings.Values("memoBut_Top") - 88, -1, 0)
            Share_Button.Margin = New Thickness(3, localSettings.Values("ShareBut_Top") - 88, -1, 0)
            Window_Button.Margin = New Thickness(3, localSettings.Values("WindowBut_Top") - 88, -1, 0)
        End If

        AdressBox.IsEnabled = False 'Autre solution provisoire (qui va sans doute rester) parce que sinon l'adressbox obtient le focus à l'ouverture allez savoir pourquoi...
        AdressBox.IsEnabled = True
        NavigationFailed_Screen.Visibility = Visibility.Collapsed
        WebpageError.Stop()
    End Sub

    Private Sub web_NavigationStarting(sender As WebView, args As WebViewNavigationStartingEventArgs) Handles web.NavigationStarting
        loader.IsActive = True 'Les petites billes de chargement apparaissent quand une page se charge
        Favicon.Visibility = Visibility.Collapsed
        BackForward()
        RefreshEnabled.Stop()
        StopEnabled.Begin()

    End Sub

    Private Sub web_NavigationCompleted(sender As WebView, args As WebViewNavigationCompletedEventArgs) Handles web.NavigationCompleted
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        'Navigation terminée
        AdressBox.Text = web.Source.ToString
        If web.Source.ToString.Contains("https://") Then
            SecurityTag.Visibility = Visibility.Visible
        Else
            SecurityTag.Visibility = Visibility.Collapsed
        End If
        Titlebox.Text = web.DocumentTitle
        loader.IsActive = False

        'Met à jour les stats dispos dans les paramètres de Blueflap
        Try
            localSettings.Values("Stat1") = localSettings.Values("Stat1") + 1
        Catch
            localSettings.Values("Stat1") = 1
        End Try

        SourceCode.Text = "..."

        BackForward()
        localSettings.Values("LoadPageFromBluestart") = False

        If web.Source.HostNameType = UriHostNameType.Dns And Not localSettings.Values("Favicon") = False Then
            Favicon.Source = New BitmapImage(New Uri("http://" & web.Source.Host & "/favicon.ico", UriKind.Absolute))
            Favicon.Visibility = Visibility.Visible
        End If

        ContextNotification()

        ' Ajout de la page à l'historique

        Dim CurrentTitle As String = web.DocumentTitle
        Dim VisitDate As DateTime = DateTime.Now

        Dim root As JsonArray = JsonArray.Parse(localSettings.Values("History"))
        Dim HistoryElem As JsonObject = New JsonObject
        HistoryElem.Add("url", JsonValue.CreateStringValue(web.Source.ToString))
        HistoryElem.Add("title", JsonValue.CreateStringValue(web.DocumentTitle))
        HistoryElem.Add("date", JsonValue.CreateNumberValue(DateTime.Now.ToBinary))
        root.Add(HistoryElem)
        localSettings.Values("History") = root.ToString

        If MemoPanel.Visibility = Visibility.Visible And RightMenuPivot.SelectedIndex = 1 Then
            ShowFavorites()
        End If
        If MemoPanel.Visibility = Visibility.Visible And RightMenuPivot.SelectedIndex = 2 Then
            ShowHistory()
        End If

    End Sub

    Private Sub web_LoadCompleted(sender As Object, e As NavigationEventArgs) Handles web.LoadCompleted
        'Page chargée

        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        ' Try
        'If localSettings.Values("Adblock") = "En fonction" Then
        '       web.Navigate(New Uri(localSettings.Values("AdblockFonction")))
        '  End If
        ' Catch
        ' localSettings.Values("Adblock") = "Désactivé"
        ' End Try

        AdressBox.Text = web.Source.ToString
        If web.Source.ToString.Contains("https://") Then
            SecurityTag.Visibility = Visibility.Visible
        Else
            SecurityTag.Visibility = Visibility.Collapsed
        End If
        Titlebox.Text = web.DocumentTitle
        loader.IsActive = False
        SourceCode.Text = "..."
        BackForward()
        localSettings.Values("LoadPageFromBluestart") = False

        If web.Source.HostNameType = UriHostNameType.Dns And Not localSettings.Values("Favicon") = False Then
            Favicon.Source = New BitmapImage(New Uri("http://" & web.Source.Host & "/favicon.ico", UriKind.Absolute))
            Favicon.Visibility = Visibility.Visible
        End If

        ContextNotification()
    End Sub

    Private Sub Home_button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Home_button.Tapped
        'Clic sur le bouton home
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        Try
            If localSettings.Values("Bluestart") = True Then
                Me.Frame.Navigate(GetType(Bluestart))
            Else

                'Vérification de l'existence d'une page d'accueil valide
                Try
                    web.Navigate(New Uri(localSettings.Values("Homepage")))

                    'Met à jour les éléments du centre de messages systèmes
                    If Notif_HomePageError.Visibility = Visibility.Visible Then
                        Notif_HomePageError.Visibility = Visibility.Collapsed
                        New_Notif.Stop()
                        Notification()

                    End If

                Catch
                    Dim notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02)
                    Dim toeastElement = notificationXml.GetElementsByTagName("text")
                    toeastElement(0).AppendChild(notificationXml.CreateTextNode("Erreur page d'accueil"))
                    toeastElement(1).AppendChild(notificationXml.CreateTextNode("La page d'accueil définie est invalide. Rendez-vous dans les paramètres et vérifiez la configuration de votre page d'accueil."))
                    Dim ToastNotification = New ToastNotification(notificationXml)
                    ToastNotificationManager.CreateToastNotifier().Show(ToastNotification)

                    'Met à jour les éléments du centre de messages systèmes
                    If Not Notif_HomePageError.Visibility = Visibility.Visible Then
                        Notif_HomePageError.Visibility = Visibility.Visible
                        Notification()


                        New_Notif.Begin()
                        Notifications_Counter.Text = Notifications_Counter.Text + 1
                        Notif_Home.Visibility = Visibility.Collapsed
                    End If
                End Try
            End If
        Catch
            localSettings.Values("Bluestart") = True
        End Try
    End Sub

    Private Sub AdressBox_KeyDown(sender As Object, e As KeyRoutedEventArgs) Handles AdressBox.KeyDown
        If (e.Key = Windows.System.VirtualKey.Enter) Then  'Permet de réagir à l'appui sur la touche entrée
            Rechercher()
        End If
    End Sub
    Private Sub Rechercher()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        SecurityTag.Visibility = Visibility.Collapsed

        Dim textArray = AdressBox.Text.Split(" ")

        'Détermine si il s'agit d'une URL ou d'une recherche

        If (AdressBox.Text.Contains(".") = True And AdressBox.Text.Contains(" ") = False And AdressBox.Text.Contains(" .") = False And AdressBox.Text.Contains(". ") = False) Or textArray(0).Contains(":/") = True Or textArray(0).Contains(":\") Then
            Try
                If AdressBox.Text.Contains("http://") OrElse AdressBox.Text.Contains("https://") Then  'URL invalide si pas de http://
                    web.Navigate(New Uri(AdressBox.Text))
                Else
                    web.Navigate(New Uri("http://" + AdressBox.Text))
                End If
            Catch
            End Try

        Else
            Try 'On teste si le moteur de recherche défini par l'utilisateur est valide
                Dim Rech As String
                Rech = AdressBox.Text
                localSettings.Values("textboxe") = AdressBox.Text
                Dim s As String
                s = Rech.ToString
                s = s.Replace("+", "%2B")

                If localSettings.Values("Custom_SearchEngine") = True Then
                    web.Navigate(New Uri(localSettings.Values("Cust1") + s + localSettings.Values("Cust2")))
                Else
                    web.Navigate(New Uri(localSettings.Values("A1") + s + localSettings.Values("A2"))) 'Rechercher avec moteurs de recherche
                End If

                'Met à jour les éléments du centre de messages systèmes


                If Notif_SearchEngineError.Visibility = Visibility.Visible Then
                    Notif_SearchEngineError.Visibility = Visibility.Collapsed
                    New_Notif.Stop()
                End If

            Catch
                Dim notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02)
                Dim toeastElement = notificationXml.GetElementsByTagName("text")
                toeastElement(0).AppendChild(notificationXml.CreateTextNode("Erreur moteur de recherche"))
                toeastElement(1).AppendChild(notificationXml.CreateTextNode("Le moteur de recherche défini est invalide. Rendez-vous dans les paramètres et vérifiez la configuration de votre moteur de recherche."))
                Dim ToastNotification = New ToastNotification(notificationXml)
                ToastNotificationManager.CreateToastNotifier().Show(ToastNotification)

                'Met à jour les éléments du centre de messages systèmes
                If Not Notif_SearchEngineError.Visibility = Visibility.Visible Then
                    Notif_SearchEngineError.Visibility = Visibility.Visible
                    Notification()

                    New_Notif.Begin()
                    Notifications_Counter.Text = Notifications_Counter.Text + 1
                    Notif_Home.Visibility = Visibility.Collapsed
                End If
            End Try
        End If

        AdressBox.IsEnabled = False 'PROVISOIRE : Faire perdre le focus à la textbox
        AdressBox.IsEnabled = True

    End Sub
    Private Sub Strefresh_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Strefresh_Button.Tapped
        'C'est ici que notre bouton va devoir faire un choix... Devenir un bouton stop... Ou un bouton Refresh... LE SUSPENS EST A SON COMBLE !

        If Strefresh_Button.Content = "" Then
            web.Refresh() 'Permet d'activer le ventilateur pour que votre PC ait moins chaud... Ahaha...Je suis trop drôle... En vrai ça sert juste à actualiser la page
            StopEnabled.Stop()
            RefreshEnabled.Begin()
        Else
            RefreshEnabled.Stop()
            StopEnabled.Begin()
            web.Stop()
        End If
        AdressBox.Text = web.Source.ToString
        If web.Source.ToString.Contains("https://") Then
            SecurityTag.Visibility = Visibility.Visible
        Else
            SecurityTag.Visibility = Visibility.Collapsed
        End If
        Titlebox.Text = web.DocumentTitle
        loader.IsActive = False
        BackForward()
    End Sub

    Private Sub Back_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Back_Button.Tapped
        web.GoBack() 'Permet de revenir à la page précédente
    End Sub

    Private Sub Forward_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Forward_Button.Tapped
        web.GoForward() 'Revenir à la page suivante
    End Sub

    Private Sub OnNewWindowRequested(sender As WebView, e As WebViewNewWindowRequestedEventArgs) Handles web.NewWindowRequested
        'Force l'ouverture dans Blueflap de liens censés s'ouvrir dans une nouvelle fenêtre
        web.Navigate(e.Uri)
        e.Handled = True
    End Sub
    Private Sub web_NavigationFailed(sender As Object, e As WebViewNavigationFailedEventArgs) Handles web.NavigationFailed
        NavigationFailed_Screen.Visibility = Visibility.Visible
        WebpageError.Begin()
        Titlebox.Text = "I hate this page"
    End Sub
    Private Sub web_ContainsFullScreenElementChanged(sender As WebView, args As Object) Handles web.ContainsFullScreenElementChanged
        If web.ContainsFullScreenElement Then
            Dim appView = ApplicationView.GetForCurrentView
            appView.TryEnterFullScreenMode()
            Enterfullscreen.Begin()
            EchapFullScreen.Begin()

        Else
            Dim appView = ApplicationView.GetForCurrentView
            appView.ExitFullScreenMode()
            Enterfullscreen.Stop()
            EchapFullScreen.Stop()

        End If
    End Sub
#End Region
#Region "First Launch"
    Private Sub FirstLaunch() 'Définit les paramètres par défaut
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        If Not localSettings.Values("Config") = True Then
            localSettings.Values("A1") = "http://www.qwant.com/?q="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 1
            localSettings.Values("WallpaperName") = "Degrade.png"
            localSettings.Values("Bluestart") = True
            localSettings.Values("Homepage") = "http://personali.zz.mu"
            localSettings.Values("CustomColorA") = 52
            localSettings.Values("CustomColorB") = 152
            localSettings.Values("CustomColorC") = 219
            localSettings.Values("SearchFight_Menu") = True
            localSettings.Values("SearchFightIcon") = True
            localSettings.Values("LockIcon") = True
            localSettings.Values("NoteIcon") = True
            localSettings.Values("ShareIcon") = True
            localSettings.Values("WindowIcon") = True
            localSettings.Values("SmartSuggest") = True
            localSettings.Values("Favicon") = True
            localSettings.Values("History") = JsonArray.Parse("[]").ToString
            localSettings.Values("Favorites") = JsonArray.Parse("[]").ToString
        End If

        If Not localSettings.Values("FirstBoot") = "Non" Then
            localSettings.Values("FirstBoot") = "Non"
            Me.Frame.Navigate(GetType(FirstBootScreen))
        End If

    End Sub
#End Region
#Region "Right Panel (Memo, history, favorites, notifications)"
    Private Sub OpenRightMenu()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        'Soit on ouvre le volet des mémos, soit on le ferme (s'il est déjà ouvert)
        If MemoPanel.Visibility = Visibility.Collapsed Then
            MemoPopOut.Stop()
            MemoPopIN.Begin()
            Try
                MemoText.Text = localSettings.Values("MemoText")
            Catch
            End Try
            LeftPanelShadow.Visibility = Visibility.Visible
            Try
                If localSettings.Values("AncrageMemo") = True Then 'On  vérifie si l'utilisateur a ancré le volet des mémos
                    webcontainer.Margin = New Thickness(48, 66, 261, 0)
                    LeftPanelShadow.Visibility = Visibility.Collapsed
                End If
                MemoAncrageToggle.IsOn = localSettings.Values("AncrageMemo")
            Catch
            End Try

        Else
            MemoPopIN.Stop()
            MemoPopOut.Begin()
            webcontainer.Margin = New Thickness(48, 66, 0, 0)
        End If
        PivotIndicatorPosition()
    End Sub
    Private Sub Notifications_indicator_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Notifications_indicator.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        OpenRightMenu()
        RightMenuPivot.SelectedIndex = 3
        New_Notif.Stop()
        Notifications_Counter.Text = "0"
        If NotifPosition = 0 Then
            Notif_Home.Visibility = Visibility.Visible
        End If
    End Sub

    Private Sub Button_Tapped(sender As Object, e As TappedRoutedEventArgs)
        'Clic sur le bouton d'accès au paramètres présent sur certaines notifications
        MemoPopIN.Stop()
        MemoPopOut.Begin()
        webcontainer.Margin = New Thickness(48, 66, 0, 0)
        Me.Frame.Navigate(GetType(Parametres))
    End Sub
    Private Sub Memo_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Memo_Button.Tapped
        OpenRightMenu()
        RightMenuPivot.SelectedIndex = 0
    End Sub

    Private Sub MemoText_TextChanged(sender As Object, e As TextChangedEventArgs) Handles MemoText.TextChanged
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("MemoText") = MemoText.Text 'Là on enregistre le texte des mémos en continu
    End Sub

    Private Sub MemoPanel_LostFocus(sender As Object, e As RoutedEventArgs) Handles MemoText.LostFocus
        'Pour aller plus vite, Quand on a fini d'écrire, le volet se referme tout seul
        If webcontainer.Margin.Right = 0 Then
            MemoPopIN.Stop()
            MemoPopOut.Begin()
        End If
    End Sub

    Private Sub MemoAncrageToggle_Toggled(sender As Object, e As RoutedEventArgs) Handles MemoAncrageToggle.Toggled
        'L'utilisateur choisi d'ancrer ou non le volet des mémos
        If MemoAncrageToggle.IsOn Then
            webcontainer.Margin = New Thickness(48, 66, 261, 0)
            LeftPanelShadow.Visibility = Visibility.Collapsed
        Else
            webcontainer.Margin = New Thickness(48, 66, 0, 0)
            LeftPanelShadow.Visibility = Visibility.Visible
        End If
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("AncrageMemo") = MemoAncrageToggle.IsOn
    End Sub
    Private Sub Notification()
        Dim Notifsvisible As String
        Notifsvisible = 0
        If Notif_MiniPlayer.Visibility = Visibility.Visible Then
            NotifPosition = 110 * Notifsvisible
            Notif_MiniPlayer.Margin = New Thickness(0, NotifPosition, 0, 0)
            Notifsvisible = Notifsvisible + 1
        End If
        If Notif_HomePageError.Visibility = Visibility.Visible Then
            NotifPosition = 110 * Notifsvisible
            Notif_HomePageError.Margin = New Thickness(0, NotifPosition, 0, 0)
            Notifsvisible = Notifsvisible + 1
        End If
        If Notif_SearchEngineError.Visibility = Visibility.Visible Then
            NotifPosition = 110 * Notifsvisible
            Notif_SearchEngineError.Margin = New Thickness(0, NotifPosition, 0, 0)
            Notifsvisible = Notifsvisible + 1
        End If
        If Notif_SearchEngineSuggestion.Visibility = Visibility.Visible Then
            NotifPosition = 110 * Notifsvisible
            Notif_SearchEngineSuggestion.Margin = New Thickness(0, NotifPosition, 0, 0)
            Notifsvisible = Notifsvisible + 1
        End If
        If Notif_Diminutweet.Visibility = Visibility.Visible Then
            NotifPosition = 110 * Notifsvisible
            Notif_Diminutweet.Margin = New Thickness(0, NotifPosition, 0, 0)
            Notifsvisible = Notifsvisible + 1
        End If

        NotifPosition = 110 * Notifsvisible
    End Sub
    Private Sub ContextNotification()
        If web.Source.ToString.Contains("www.bing.com") Then
            If Notif_SearchEngineSuggestion.Visibility = Visibility.Collapsed Then
                New_Notif.Begin()
                Notifications_Counter.Text = Notifications_Counter.Text + 1
                Notif_Home.Visibility = Visibility.Collapsed
            End If
            Notif_SearchEngineName.Text = "BING"
            Notif_SearchEngineIcon.Source = New BitmapImage(New Uri("ms-appx:/Assets/Engine_Bing.png", UriKind.Absolute))
            Notif_SearchEngineSuggestion.Visibility = Visibility.Visible
        ElseIf web.Source.ToString.Contains("www.qwant.com") Then
            If Notif_SearchEngineSuggestion.Visibility = Visibility.Collapsed Then
                New_Notif.Begin()
                Notifications_Counter.Text = Notifications_Counter.Text + 1
                Notif_Home.Visibility = Visibility.Collapsed
            End If
            Notif_SearchEngineName.Text = "QWANT"
            Notif_SearchEngineIcon.Source = New BitmapImage(New Uri("ms-appx:/Assets/Engine_Qwant.png", UriKind.Absolute))
            Notif_SearchEngineSuggestion.Visibility = Visibility.Visible
        ElseIf web.Source.ToString.Contains("duckduckgo.com") Then
            If Notif_SearchEngineSuggestion.Visibility = Visibility.Collapsed Then
                New_Notif.Begin()
                Notifications_Counter.Text = Notifications_Counter.Text + 1
                Notif_Home.Visibility = Visibility.Collapsed
            End If
            Notif_SearchEngineName.Text = "DUCKDUCKGO"
            Notif_SearchEngineIcon.Source = New BitmapImage(New Uri("ms-appx:/Assets/Engine_Duck.png", UriKind.Absolute))
            Notif_SearchEngineSuggestion.Visibility = Visibility.Visible
        ElseIf web.Source.ToString.Contains("yahoo.com") Then
            If Notif_SearchEngineSuggestion.Visibility = Visibility.Collapsed Then
                New_Notif.Begin()
                Notifications_Counter.Text = Notifications_Counter.Text + 1
                Notif_Home.Visibility = Visibility.Collapsed
            End If
            Notif_SearchEngineName.Text = "YAHOO"
            Notif_SearchEngineIcon.Source = New BitmapImage(New Uri("ms-appx:/Assets/Engine_yahoo.png", UriKind.Absolute))
            Notif_SearchEngineSuggestion.Visibility = Visibility.Visible
        Else
            Notif_SearchEngineSuggestion.Visibility = Visibility.Collapsed
            If Notifications_Counter.Text > 1 Then
                Notifications_Counter.Text = Notifications_Counter.Text - 1
            End If
        End If

        If web.Source.ToString.Contains("twitter.com") Then
            If Notif_Diminutweet.Visibility = Visibility.Collapsed Then
                New_Notif.Begin()
                Notifications_Counter.Text = Notifications_Counter.Text + 1
                Notif_Home.Visibility = Visibility.Collapsed
            End If

            Notif_Diminutweet.Visibility = Visibility.Visible
        Else
            Notif_Diminutweet.Visibility = Visibility.Collapsed
            If Notifications_Counter.Text > 1 Then
                Notifications_Counter.Text = Notifications_Counter.Text - 1
            End If
        End If

        If web.Source.ToString.Contains("vimeo.com/") Or web.Source.ToString.Contains("dailymotion.com/video/") Or web.Source.ToString.Contains("youtube.com/watch") Then
            If Notif_MiniPlayer.Visibility = Visibility.Collapsed Then
                New_Notif.Begin()
                Notifications_Counter.Text = Notifications_Counter.Text + 1
                Notif_Home.Visibility = Visibility.Collapsed
            End If

            Notif_MiniPlayer.Visibility = Visibility.Visible
            ShowMiniPlayerIcon.Begin()

            If web.Source.ToString.Contains("vimeo.com/") Then
                If web.Source.ToString.Contains("vimeo.com/0") Or web.Source.ToString.Contains("vimeo.com/1") Or web.Source.ToString.Contains("vimeo.com/2") Or web.Source.ToString.Contains("vimeo.com/3") Or web.Source.ToString.Contains("vimeo.com/4") Or web.Source.ToString.Contains("vimeo.com/5") Or web.Source.ToString.Contains("vimeo.com/6") Or web.Source.ToString.Contains("vimeo.com/7") Or web.Source.ToString.Contains("vimeo.com/8") Or web.Source.ToString.Contains("vimeo.com/9") Then
                    Notif_MiniPlayer.Visibility = Visibility.Visible
                    ShowMiniPlayerIcon.Begin()
                Else
                    Notifications_Counter.Text = Notifications_Counter.Text - 1
                    Notif_MiniPlayer.Visibility = Visibility.Collapsed
                    ShowMiniPlayerIcon.Stop()
                End If
            End If
            Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
            If localSettings.Values("DarkThemeEnabled") = True Then
                MiniPlayer_Button.RequestedTheme = ElementTheme.Dark
            Else
                MiniPlayer_Button.RequestedTheme = ElementTheme.Light
            End If
        Else
            Notif_MiniPlayer.Visibility = Visibility.Collapsed
            ShowMiniPlayerIcon.Stop()
        End If

        Notification()
    End Sub
    Private Sub PivotIndicatorPosition()
        If RightMenuPivot.SelectedIndex = 0 Then
            MemoIndexIndicator.Margin = New Thickness(4, 8, 0, 0)
        ElseIf RightMenuPivot.SelectedIndex = 1 Then
            MemoIndexIndicator.Margin = New Thickness(44, 8, 0, 0)
            ShowFavorites()
        ElseIf RightMenuPivot.SelectedIndex = 2 Then
            MemoIndexIndicator.Margin = New Thickness(84, 8, 0, 0)
            ShowHistory()
        ElseIf RightMenuPivot.SelectedIndex = 3 Then
            MemoIndexIndicator.Margin = New Thickness(124, 8, 0, 0)
        End If
    End Sub
    Private Sub RightMenuPivot_PivotItemLoaded(sender As Pivot, args As PivotItemEventArgs) Handles RightMenuPivot.PivotItemLoaded
        PivotIndicatorPosition()
        MemoIndicator.Begin()
    End Sub
    Private Async Sub Button_Tapped_1(sender As Object, e As TappedRoutedEventArgs)
        Await Windows.System.Launcher.LaunchUriAsync(New Uri(("ms-windows-store://pdp/?ProductId=9nblggh316xh")))
    End Sub

    Private Sub ChangSearchEngine(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        'Définit les valeurs du moteur de recherche tel que le navigateur navigue vers (A1 + Mots-clés + A2) = URI

        If Notif_SearchEngineName.Text.ToLower = "qwant" Then
            localSettings.Values("A1") = "http://www.qwant.com/?q="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 1
        ElseIf Notif_SearchEngineName.Text.ToLower = "bing" Then
            localSettings.Values("A1") = "http://www.bing.com/search?q="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 0
        ElseIf Notif_SearchEngineName.Text.ToLower = "duckduckgo" Then
            localSettings.Values("A1") = "http://duckduckgo.com/?q="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 2
        ElseIf Notif_SearchEngineName.Text.ToLower = "yahoo" Then
            localSettings.Values("A1") = "http://fr.search.yahoo.com/search;_ylt=Ai38ykBDWJSAxF25NrTnjXxNhJp4?p="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 3
        End If

        Dim notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02)
        Dim toeastElement = notificationXml.GetElementsByTagName("text")
        toeastElement(0).AppendChild(notificationXml.CreateTextNode("Moteur de recherche"))
        toeastElement(1).AppendChild(notificationXml.CreateTextNode("Le moteur de recherche a été modifié"))
        Dim ToastNotification = New ToastNotification(notificationXml)
        ToastNotificationManager.CreateToastNotifier().Show(ToastNotification)
    End Sub

    Private Sub ShowHistory()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        HistoryList.Children.Clear()

        For Each histElem In JsonArray.Parse(localSettings.Values("History")).Reverse

            Dim elemContainer As StackPanel = New StackPanel
            elemContainer.Padding = New Thickness(8, 8, 0, 8)
            AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                        web.Navigate(New Uri(histElem.GetObject.GetNamedString("url")))
                                                                    End Function)

            AddHandler elemContainer.PointerEntered, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                 elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(10, 0, 0, 0))
                                                                                 elemContainer.BorderThickness = New Thickness(2, 0, 0, 0)
                                                                                 elemContainer.Padding = New Thickness(6, 8, 0, 8)
                                                                                 elemContainer.BorderBrush = LeftMenu.Background
                                                                             End Function)

            AddHandler elemContainer.PointerExited, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
                                                                                elemContainer.BorderThickness = New Thickness(0, 0, 0, 0)
                                                                                elemContainer.Padding = New Thickness(8, 8, 0, 8)
                                                                            End Function)

            Dim elemText As TextBlock = New TextBlock
            elemText.Text = histElem.GetObject.GetNamedString("title")
            elemText.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 40, 40, 40))
            elemContainer.Children.Add(elemText)

            Dim UrlText As TextBlock = New TextBlock
            UrlText.Text = histElem.GetObject.GetNamedString("url")
            UrlText.Foreground = LeftMenu.Background
            elemContainer.Children.Add(UrlText)

            Dim visitDate As TextBlock = New TextBlock
            visitDate.Text = DateTime.FromBinary(histElem.GetObject.GetNamedNumber("date")).ToString("Le dd MMMMMMMMMMMM yyyy à HH:mm")
            visitDate.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 150, 150, 150))
            elemContainer.Children.Add(visitDate)

            HistoryList.Children.Add(elemContainer)

        Next
    End Sub

    Sub ShowFavorites()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        FavList.Children.Clear()

        For Each favsElem In JsonArray.Parse(localSettings.Values("Favorites")).Reverse

            Dim elemContainer As StackPanel = New StackPanel
            elemContainer.Padding = New Thickness(8, 8, 0, 8)
            AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                        web.Navigate(New Uri(favsElem.GetObject.GetNamedString("url")))
                                                                    End Function)

            AddHandler elemContainer.PointerEntered, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                 elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(10, 0, 0, 0))
                                                                                 elemContainer.BorderThickness = New Thickness(2, 0, 0, 0)
                                                                                 elemContainer.Padding = New Thickness(6, 8, 0, 8)
                                                                                 elemContainer.BorderBrush = LeftMenu.Background
                                                                             End Function)

            AddHandler elemContainer.PointerExited, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
                                                                                elemContainer.BorderThickness = New Thickness(0, 0, 0, 0)
                                                                                elemContainer.Padding = New Thickness(8, 8, 0, 8)
                                                                            End Function)

            Dim elemText As TextBlock = New TextBlock
            elemText.Text = favsElem.GetObject.GetNamedString("title")
            elemText.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 40, 40, 40))
            elemContainer.Children.Add(elemText)


            Dim UrlText As TextBlock = New TextBlock
            UrlText.Text = favsElem.GetObject.GetNamedString("url")
            UrlText.Foreground = LeftMenu.Background
            elemContainer.Children.Add(UrlText)

            FavList.Children.Add(elemContainer)

        Next
    End Sub
    Private Sub AddToFavList()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        Dim CurrentTitle As String = web.DocumentTitle
        Dim VisitDate As DateTime = DateTime.Now

        Dim root As JsonArray = JsonArray.Parse(localSettings.Values("Favorites"))
        Dim HistoryElem As JsonObject = New JsonObject
        HistoryElem.Add("url", JsonValue.CreateStringValue(web.Source.ToString))
        HistoryElem.Add("title", JsonValue.CreateStringValue(web.DocumentTitle))
        root.Add(HistoryElem)
        localSettings.Values("Favorites") = root.ToString
        Debug.WriteLine(localSettings.Values("Favorites"))

    End Sub

    Private Sub AddToFavs_Tapped(sender As Object, e As TappedRoutedEventArgs)
        AddToFavList()
        ShowFavorites()
    End Sub
#End Region
#Region "Share/Source code Menu"
    Private Sub Fight_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Fight_Button.Tapped
        'Ouvre SearchFight
        Me.Frame.Navigate(GetType(SearchFight))
    End Sub
    Private Sub Share_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Share_Button.Tapped
        'Affichage de l'interface de partage
        SourceCode.Text = "..."
        ViewSourceCode.Stop()
        HideSourceCode.Stop()
        SourceCode.Visibility = Visibility.Collapsed
        web.Visibility = Visibility.Visible

        If Infos_grid.Visibility = Visibility.Visible Then
            Share.Stop()
            CancelShare.Stop()
            CancelShare.Begin()
        Else
            Share.Stop()
            CancelShare.Stop()
            Share.Begin()
        End If
    End Sub

    Private Sub Web_Share_Save_Tapped(sender As Object, e As TappedRoutedEventArgs)

    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        Dim dtManager As DataTransferManager = DataTransferManager.GetForCurrentView()
        AddHandler dtManager.DataRequested, AddressOf dtManager_DataRequested
    End Sub
    Private Sub dtManager_DataRequested(sender As DataTransferManager, e As DataRequestedEventArgs)
        e.Request.Data.Properties.Title = "Partage d'une page internet : " + web.DocumentTitle.ToString + " - Via Blueflap"
        e.Request.Data.SetText(web.DocumentTitle.ToString + " (" + web.Source.ToString + ") - Via #Blueflap - https://www.microsoft.com/store/apps/9nblggh5xcvz")
    End Sub
    Private Sub Web_Share_Share_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Web_Share_Share.Tapped
        Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI()
    End Sub

    Private Async Sub Web_Share_Source_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Web_Share_Source.Tapped

        'Récupère et affiche le code source de la page
        If SourceCode.Visibility = Visibility.Collapsed Then
            ViewSourceCode.Begin()

            If SourceCode.Text = "..." Then
                Dim html As String = Await (web.InvokeScriptAsync("eval", New String() {"document.documentElement.outerHTML;"}))
                SourceCode.Text = html
            End If
        Else
            HideSourceCode.Begin()
        End If
    End Sub
    Private Sub Info_Pin_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Info_Pin.Tapped

    End Sub
    Private Sub Info_Like_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Info_Like.Tapped
        Like_Anim.Stop()
        Like_Anim.Begin()
        AddToFavList()
        If MemoPanel.Visibility = Visibility.Visible And RightMenuPivot.SelectedIndex = 1 Then
            ShowFavorites()
        End If
    End Sub
#End Region
#Region "SmartSuggest"
    Private Sub AdressBox_GotFocus(sender As Object, e As RoutedEventArgs) Handles AdressBox.GotFocus
        Try
            AdressBox.Text = web.Source.ToString
            Titlebox.Text = web.DocumentTitle
        Catch
        End Try
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        If localSettings.Values("DarkThemeEnabled") = True Then
            SmartSuggest.RequestedTheme = ElementTheme.Dark
            DerniereRecherche.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(235, 255, 255, 255))
        Else
            SmartSuggest.RequestedTheme = ElementTheme.Light
            DerniereRecherche.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(170, 0, 0, 0))
        End If
        Try
            If localSettings.Values("SmartSuggest") = True Then
                HideSuggestions.Stop()
                ShowSuggestions.Begin()
                SmartSuggest.Background = Adressbar.Background
            End If
        Catch ex As Exception
        End Try
        Try
            DerniereRecherche.Text = localSettings.Values("textboxe").ToString.ToUpper
        Catch
        End Try
        Dim textArray = AdressBox.Text.Split(" ")
        If (AdressBox.Text.Contains(".") = True And AdressBox.Text.Contains(" ") = False And AdressBox.Text.Contains(" .") = False And AdressBox.Text.Contains(". ") = False) Or textArray(0).Contains(":/") = True Or textArray(0).Contains(":\") Then
            Dim t As String
            t = AdressBox.Text.ToLower
            t = t.Replace("https://", "")
            t = t.Replace("http://", "")
            t = t.Replace(" ", "%20")
            SmartSuggest_URL_Text.Text = "http://" + t
            SmartSuggest_Search_Text.Text = ""
        Else
            Try
                SmartSuggest_URL_Text.Text = localSettings.Values("A1")
            Catch
            End Try
            SmartSuggest_Search_Text.Text = ""
        End If
        AdressBox.SelectAll()

        If web.Source.HostNameType = UriHostNameType.Dns And loader.IsActive = False And Not localSettings.Values("Favicon") = False Then
            Favicon.Source = New BitmapImage(New Uri("http://" & web.Source.Host & "/favicon.ico", UriKind.Absolute))
            Favicon.Visibility = Visibility.Visible
        End If

    End Sub

    Private Sub AdressBox_LostFocus(sender As Object, e As RoutedEventArgs) Handles AdressBox.LostFocus
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        Try
            If localSettings.Values("SmartSuggest") = True Then
                ShowSuggestions.Stop()
                SmartSuggest.Visibility = Visibility.Collapsed
            End If
        Catch ex As Exception
        End Try
        AdressBox.SelectionStart = AdressBox.Text.Length
    End Sub

    Private Sub SmartSuggest_LastOne_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles SmartSuggest_LastOne.PointerEntered
        SmartSuggest_LastOne.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(70, 52, 152, 213))
    End Sub
    Private Sub SmartSuggest_LastOne_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles SmartSuggest_LastOne.PointerExited
        SmartSuggest_LastOne.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
    End Sub
    Private Sub SmartSuggest_Search_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles SmartSuggest_Search.PointerEntered
        SmartSuggest_Search.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(70, 52, 152, 213))
    End Sub
    Private Sub SmartSuggest_Search_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles SmartSuggest_Search.PointerExited
        SmartSuggest_Search.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
    End Sub
    Private Sub SmartSuggest_URL_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles SmartSuggest_URL.PointerEntered
        SmartSuggest_URL.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(70, 52, 152, 213))
    End Sub
    Private Sub SmartSuggest_URL_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles SmartSuggest_URL.PointerExited
        SmartSuggest_URL.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
    End Sub

    Private Sub SmartSuggest_LastOne_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles SmartSuggest_LastOne.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        Try
            AdressBox.Text = localSettings.Values("textboxe")
        Catch
        End Try
        Rechercher()
    End Sub
    Private Sub SmartSuggest_Search_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles SmartSuggest_Search.Tapped
        AdressBox.Text = SmartSuggest_Search_Text.Text + " "
        Rechercher()
    End Sub
    Private Sub SmartSuggest_Url_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles SmartSuggest_URL.Tapped
        AdressBox.Text = SmartSuggest_URL_Text.Text
        Rechercher()
    End Sub

    Private Sub AdressBox_TextChanged(sender As Object, e As TextChangedEventArgs) Handles AdressBox.TextChanged
        Dim t As String
        t = AdressBox.Text.ToLower
        t = t.Replace("https://", "")
        t = t.Replace("http://", "")
        t = t.Replace(" ", "%20")
        SmartSuggest_URL_Text.Text = "http://" + t
        Dim s As String
        s = AdressBox.Text.ToLower
        s = s.Replace("https://", "")
        s = s.Replace("http://", "")
        Dim textArray = AdressBox.Text.Split(" ")
        If (AdressBox.Text.Contains(".") = True And AdressBox.Text.Contains(" ") = False And AdressBox.Text.Contains(" .") = False And AdressBox.Text.Contains(". ") = False) Or textArray(0).Contains(":/") = True Or textArray(0).Contains(":\") Then
            s = s.Replace("/", " ")
            s = s.Replace("search?q", " recherche ")
            s = s.Replace("?q", "")
            s = s.Replace("_", " ")
            s = s.Replace("-", " ")
            s = s.Replace("www.", "")
            s = s.Replace(".com", "")
            s = s.Replace(".fr", "")
            s = s.Replace(".tv", " tv")
            s = s.Replace(".org", "")
            s = s.Replace(".eu", "")
            s = s.Replace(".it", "")
            s = s.Replace(".de", "")
            s = s.Replace(".co.uk", "")
            s = s.Replace(".co", "")
            s = s.Replace(".edu", " éducation")
            s = s.Replace(".net", "")
            s = s.Replace(".io", "")
            s = s.Replace(".tk", "")
            s = s.Replace("?", "")
            s = s.Replace("=", " ")
            s = s.Replace("personnalisa.bl.ee", "Blueflap")
            s = s.Replace("personali.zz.mu", "Blueflap")
            s = s.Replace(".php", "")
            s = s.Replace(".html", "")
            s = s.Replace(".htm", "")
            s = s.Replace(".aspx", "")
        End If
        SmartSuggest_Search_Text.Text = s

        If (AdressBox.Text.Contains(".") = True And AdressBox.Text.Contains(" ") = False And AdressBox.Text.Contains(" .") = False And AdressBox.Text.Contains(". ") = False) Or textArray(0).Contains(":/") = True Or textArray(0).Contains(":\") Then
            SmartSuggest_URL.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(20, 52, 152, 213))
            SmartSuggest_Search.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
            ExpandSuggestions.Begin()
        ElseIf AdressBox.Text = "" Then
            SmartSuggest_URL.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
            SmartSuggest_Search.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
            HideSuggestions.Begin()
        Else
            SmartSuggest_Search.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(20, 52, 152, 213))
            SmartSuggest_URL.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
            ExpandSuggestions.Begin()
        End If
    End Sub
#End Region
#Region "Go to (other frame)"
    Private Sub Lock_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Lock_Button.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        LockTheBrowser.IsChecked = True
        Me.Frame.Navigate(GetType(Verrouillage))
    End Sub

    Private Sub Paramètres_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Paramètres_Button.Tapped
        Me.Frame.Navigate(GetType(Parametres)) 'Aller sur la page "Paramètres"
    End Sub

    Private Async Sub NewWindow()
        Dim newView = CoreApplication.CreateNewView()
        Dim appView = ApplicationView.GetForCurrentView()
        Dim newViewId As Integer = 0
        Await newView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Function()
                                                                                             Dim window__1 = Window.Current
                                                                                             Dim newAppView = ApplicationView.GetForCurrentView()

                                                                                             Dim frame = New Frame()
                                                                                             window__1.Content = frame

                                                                                             frame.Navigate(GetType(MainPage))
                                                                                             window__1.Activate()

                                                                                             newViewId = ApplicationView.GetForCurrentView().Id

                                                                                         End Function)
        Dim viewShown As Boolean = Await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId)

    End Sub

    Private Sub Window_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Window_Button.Tapped
        NewWindow()
        'MiniPlayer()
    End Sub
    Private Async Sub MiniPlayer()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        Dim t As String
        t = web.Source.ToString
        t = t.Replace("vimeo.com/", "player.vimeo.com/video/")
        t = t.Replace("dailymotion.com/video/", "dailymotion.com/embed/video/")
        t = t.Replace("youtube.com/watch", "youtube.com/watch_popup")
        localSettings.Values("MiniPlayerUri") = t

        Dim newView As CoreApplicationView = CoreApplication.CreateNewView()
        Dim newViewId As Integer = 0
        Await newView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Function()
                                                                                             Dim frame As New Frame()
                                                                                             frame.Navigate(GetType(MiniPlayer), Nothing)
                                                                                             Window.Current.Content = frame
                                                                                             ' You have to activate the window in order to show it later.
                                                                                             Window.Current.Activate()

                                                                                             newViewId = ApplicationView.GetForCurrentView().Id

                                                                                         End Function)
        Dim viewShown As Boolean = Await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId)

    End Sub

    Private Sub OpenMiniP(sender As Object, e As TappedRoutedEventArgs)
        MiniPlayer()
    End Sub

    Private Sub MiniPlayer_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles MiniPlayer_Button.Tapped
        MiniPlayer()
    End Sub
#End Region

End Class
