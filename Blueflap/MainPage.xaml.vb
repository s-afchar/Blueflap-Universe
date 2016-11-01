﻿Imports Windows.UI.Notifications
Imports Windows.ApplicationModel.DataTransfer
Imports Windows.UI.Xaml.Controls
Imports Windows.ApplicationModel.Core
Imports Windows.Data.Json
Imports Windows.UI.Xaml.Documents
Imports Windows.Storage
Imports Windows.Web.Http
Imports Windows.Graphics.Imaging
Imports Windows.UI.StartScreen
''' <summary>
''' Page dédiée à la navigation web
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page
    Dim NotifPosition As String
    Dim History_SearchMode As Boolean
    Dim History_SearchKeywords As String
    Dim ItemCount As Integer
    Dim OpenSearchEngine As Boolean
    Dim EditMemo As Boolean
    Dim OpenSearch_A1 As String
    Dim OpenSearch_A2 As String
    Dim resourceLoader = New Resources.ResourceLoader()
    Dim favs_tagsearch As Boolean
    Dim AdressBoxEditing As Boolean
    Dim limitedOpenSearch As Boolean
    Dim showingfavbar As Boolean
    Dim favbarmodif As Boolean

#Region "HardwareBackButton"
    Public Sub New()
        Me.InitializeComponent()
        RemoveHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf MainPage_BackRequested
        AddHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf MainPage_BackRequested
    End Sub
    Private Sub MainPage_BackRequested(sender As Object, e As Windows.UI.Core.BackRequestedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        'Appui sur la touche retour "physique" d'un appareil Windows

        e.Handled = True
        If web.CanGoBack And MemoPanel.Visibility = Visibility.Collapsed And Ellipsis.Visibility = Visibility.Collapsed And AddFavView.Visibility = Visibility.Collapsed And Not MiniPlayer_Background.VerticalAlignment = VerticalAlignment.Stretch Then
            web.GoBack()
        ElseIf MemoPanel.Visibility = Visibility.Visible And Ellipsis.Visibility = Visibility.Collapsed And AddFavView.Visibility = Visibility.Collapsed Then
            MemoPopOut.Begin()
        ElseIf Ellipsis.Visibility = Visibility.Visible Then
            Ellipsis_Close.Begin()
        ElseIf AddFavView.Visibility = Visibility.Visible And Ellipsis.Visibility = Visibility.Collapsed Then
            AddFav_PopUp_Close.Begin()
        ElseIf MiniPlayer_Background.VerticalAlignment = VerticalAlignment.Stretch Then
            ReduceMiniPlayer()
        End If
    End Sub
#End Region
#Region "Page Loaded"
    Private Async Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings 'Permet l'accés aux paramètres

        'Animation d'ouverture de Blueflap
        EnterAnim.Begin()

        FirstLaunch()

        SmartSuggest.Visibility = Visibility.Collapsed

        Try
            If (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) Then
                Await Windows.UI.ViewManagement.StatusBar.GetForCurrentView.HideAsync()
            End If
        Catch
        End Try



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
            PhoneNavBar.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 21, 21, 21))
            PhoneNavBar.BorderBrush = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 70, 70, 70))
            PhoneNavBar.RequestedTheme = ElementTheme.Dark
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
            PhoneNavBar.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255))
            PhoneNavBar.BorderBrush = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 147, 147, 147))
            PhoneNavBar.RequestedTheme = ElementTheme.Light
        End If

        AdressBox.IsEnabled = False 'Autre solution provisoire (qui va sans doute rester) parce que sinon l'adressbox obtient le focus à l'ouverture allez savoir pourquoi...
        AdressBox.IsEnabled = True

        BackForward()

        If localSettings.Values("ShowLockScreen") = False Then 'Permet d'éviter un revérouillage systématique du navigateur
            LockTheBrowser.IsChecked = False
        End If
        localSettings.Values("ShowLockScreen") = True


        Try
            If localSettings.Values("VerrouillageEnabled") = True And LockTheBrowser.IsChecked = True And Not localSettings.Values("LoadPageFromBluestart") = True Then
                LockTheBrowser.IsChecked = True
                Me.Frame.Navigate(GetType(Verrouillage))
            ElseIf localSettings.Values("Bluestart") = True And AdressBox.Text = "about:blank" And Frame.CanGoBack = False And Not localSettings.Values("LoadPageFromBluestart") = True Then
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
                    'toeastElement(0).AppendChild(notificationXml.CreateTextNode(resourceLoader.GetString("Notification_Homepage_Header/Text")))
                    'toeastElement(1).AppendChild(notificationXml.CreateTextNode(resourceLoader.GetString("Notification_Homepage_Content/Text")))
                    toeastElement(0).AppendChild(notificationXml.CreateTextNode(Label_Notif_Home_Header.Text))
                    toeastElement(1).AppendChild(notificationXml.CreateTextNode(Label_Notif_Home_Content.Text))
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

        Ellipsis_Button.Background = LeftMenu.Background
        Ellipsis.Background = LeftMenu.Background
        Loader_MemoPanel.Foreground = LeftMenu.Background

        Try
            If localSettings.Values("LoadPageFromBluestart") = True Then
                web.Stop()
                AdressBox.Text = localSettings.Values("LoadPageFromBluestart_Adress")
                Rechercher()
            End If
        Catch
        End Try

        'Quelles icônes sont affichées
        ' TODO : utiliser du DataBinding pour ça

        If localSettings.Values("SearchFightIcon") = False Then
            Fight_Button.Visibility = Visibility.Collapsed
            Fight_Button_M.Visibility = Visibility.Collapsed
        Else
            Fight_Button.Visibility = Visibility.Visible
            Fight_Button_M.Visibility = Visibility.Visible
        End If

        If localSettings.Values("LockIcon") = False Then
            Lock_Button.Visibility = Visibility.Collapsed
            Lock_Button_M.Visibility = Visibility.Collapsed
        Else
            Lock_Button.Visibility = Visibility.Visible
            Lock_Button_M.Visibility = Visibility.Visible
        End If

        If localSettings.Values("NoteIcon") = False Then
            Memo_Button.Visibility = Visibility.Collapsed
            Memo_Button_M.Visibility = Visibility.Collapsed
        Else
            Memo_Button.Visibility = Visibility.Visible
            Memo_Button_M.Visibility = Visibility.Visible
        End If

        If localSettings.Values("ShareIcon") = False Then
            Share_Button.Visibility = Visibility.Collapsed
            Share_Button_M.Visibility = Visibility.Collapsed
        Else
            Share_Button.Visibility = Visibility.Visible
            Share_Button_M.Visibility = Visibility.Visible
        End If

        If localSettings.Values("WindowIcon") = False Then
            Window_Button.Visibility = Visibility.Collapsed
            Window_Button_M.Visibility = Visibility.Collapsed
        Else
            Window_Button.Visibility = Visibility.Visible
            Window_Button_M.Visibility = Visibility.Visible
        End If

        If localSettings.Values("GhostIcon") = True Then
            Gost_Button.Visibility = Visibility.Visible
        Else
            Gost_Button.Visibility = Visibility.Collapsed
        End If

        If MemoPanel.Visibility = Visibility.Visible And RightMenuPivot.SelectedIndex = 1 Then
            Try
                ShowFavorites()
            Catch
                If MemoPanel.Visibility = Visibility.Visible Then
                    MemoPopOut.Begin()
                End If
            End Try
        End If
        If MemoPanel.Visibility = Visibility.Visible And RightMenuPivot.SelectedIndex = 2 Then
            Try
                ShowHistory()
            Catch
                If MemoPanel.Visibility = Visibility.Visible Then
                    MemoPopOut.Begin()
                End If
            End Try
        End If

        If localSettings.Values("GhostMode") = True Then
            Gost_Button.BorderBrush = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255))
        Else
            Gost_Button.BorderBrush = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 255, 255, 255))
        End If

        History_SearchMode = False
        favs_tagsearch = False
        favbarmodif = True

    End Sub
#End Region
#Region "Webview : Navigation"
    Private Async Sub BackForward()
        'Blueflap gère ici le placement des boutons dans le menu latéral après chaque page chargée

        StopEnabled.Stop() 'Ce sont des animation pour le bouton stop/Refresh
        RefreshEnabled.Begin()
        MobileRefreshIcon.Text = ""
        'MobileRefreshLabel.Text = resourceLoader.GetString("Menu_Refresh/Text")
        MobileRefreshLabel.Text = Label_Refresh.Text

        If web.CanGoBack Then
            Back_Button.Visibility = Visibility.Visible
            Phone_Back.Opacity = 1
        Else
            Back_Button.Visibility = Visibility.Collapsed
            Phone_Back.Opacity = 0.2
        End If

        If web.CanGoForward Then
            Forward_Button.Visibility = Visibility.Visible
            Phone_Forward.Opacity = 1
        Else
            Forward_Button.Visibility = Visibility.Collapsed
            Phone_Forward.Opacity = 0.2
        End If

        AdressBox.IsEnabled = False 'Autre solution provisoire (qui va sans doute rester) parce que sinon l'adressbox obtient le focus à l'ouverture allez savoir pourquoi...
        AdressBox.IsEnabled = True
        NavigationFailed_Screen.Visibility = Visibility.Collapsed
        WebpageError.Stop()

    End Sub

    Private Sub web_NavigationStarting(sender As WebView, args As WebViewNavigationStartingEventArgs) Handles web.NavigationStarting
        loader.IsActive = True 'Les petites billes de chargement apparaissent quand une page se charge
        MobileProgressFinish.Stop()
        MobileProgressBegin.Begin()
        PCProgressFinish.Stop()
        PCProgressBegin.Begin()
        Favicon.Visibility = Visibility.Collapsed
        BackForward()
        RefreshEnabled.Stop()
        StopEnabled.Begin()
        MobileRefreshIcon.Text = ""

        'MobileRefreshLabel.Text = resourceLoader.GetString("Menu_Stop/Text")
        MobileRefreshLabel.Text = Label_Stop.Text

    End Sub

    Private Sub web_NavigationCompleted(sender As WebView, args As WebViewNavigationCompletedEventArgs) Handles web.NavigationCompleted
        PageReady(, True, True)
    End Sub

    Private Sub PageReady(Optional progressFinish As Boolean = True, Optional askFavicon As Boolean = False, Optional addHistory As Boolean = False)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        ' Try
        'If localSettings.Values("Adblock") = "En fonction" Then
        '       web.Navigate(New Uri(localSettings.Values("AdblockFonction")))
        '  End If
        ' Catch
        ' localSettings.Values("Adblock") = "Désactivé"
        ' End Try

        AdressBox.Text = web.Source.ToString
        Phone_URL.Text = web.Source.Host
        If web.Source.Scheme = "https" Then
            SecurityTag.Visibility = Visibility.Visible
        Else
            SecurityTag.Visibility = Visibility.Collapsed
        End If
        Titlebox.Text = web.DocumentTitle

        If progressFinish Then
            loader.IsActive = False
            MobileProgressFinish.Begin()
            PCProgressFinish.Begin()
        End If

        BackForward()

        If askFavicon Then
            SourceCode.Text = "..."
            localSettings.Values("LoadPageFromBluestart") = False

            If web.Source.HostNameType = UriHostNameType.Dns And Not localSettings.Values("Favicon") = False Then
                Favicon.Source = New BitmapImage(New Uri("http://" & web.Source.Host & "/favicon.ico", UriKind.Absolute))
                Favicon.Visibility = Visibility.Visible
            End If

            ContextNotification()
        End If

        If addHistory Then
            addHistoryEntry(localSettings)
        End If
    End Sub

    Private Async Sub addHistoryEntry(localSettings As Windows.Storage.ApplicationDataContainer)
        'Met à jour les stats dispos dans les paramètres de Blueflap
        Try
            localSettings.Values("Stat1") = localSettings.Values("Stat1") + 1
        Catch
            localSettings.Values("Stat1") = 1
        End Try

        ' Ajout de la page à l'historique

        Dim CurrentTitle As String = web.DocumentTitle
        Dim VisitDate As DateTime = DateTime.Now

        If localSettings.Values("GhostMode") = True Then
            GhostModeNotif.Stop()
            GhostModeNotif.Begin()
        Else
            Try
                Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("History"))
                Dim HistoryElem As JsonObject = New JsonObject
                HistoryElem.Add("url", JsonValue.CreateStringValue(web.Source.ToString))
                HistoryElem.Add("title", JsonValue.CreateStringValue(web.DocumentTitle))
                HistoryElem.Add("date", JsonValue.CreateNumberValue(DateTime.Now.ToBinary))
                root.Add(HistoryElem)
                WriteJsonFile(root, "History")
            Catch
                WriteJsonFile(JsonArray.Parse("[]"), "History")
            End Try
        End If
        Try
            Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("Favorites"))

            If root.Any(Function(x As JsonValue) x.GetObject.GetNamedString("url") = web.Source.ToString) Then
                Like_Anim.Begin()
                LikePageButton.Text = ""
                LikePageButton.Foreground = LeftMenu.Background
            Else
                Like_Anim.Stop()
                LikePageButton.Text = ""
                LikePageButton.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 166, 166, 166))
            End If
        Catch
        End Try

        If MemoPanel.Visibility = Visibility.Visible And RightMenuPivot.SelectedIndex = 1 Then
            Try
                ShowFavorites()
            Catch
                If MemoPanel.Visibility = Visibility.Visible Then
                    MemoPopOut.Begin()
                End If
            End Try
        End If
        If MemoPanel.Visibility = Visibility.Visible And RightMenuPivot.SelectedIndex = 2 Then
            Try
                ShowHistory()
            Catch
                If MemoPanel.Visibility = Visibility.Visible Then
                    MemoPopOut.Begin()
                End If
            End Try
        End If
    End Sub

    Private Sub web_LoadCompleted(sender As Object, e As NavigationEventArgs) Handles web.LoadCompleted
        'Page chargée

        PageReady(, True)
    End Sub
    Private Sub Go_Home()
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
                    'toeastElement(0).AppendChild(notificationXml.CreateTextNode(resourceLoader.GetString("Notification_Homepage_Header/Text")))
                    'toeastElement(1).AppendChild(notificationXml.CreateTextNode(resourceLoader.GetString("Notification_Homepage_Content/Text")))
                    toeastElement(0).AppendChild(notificationXml.CreateTextNode(Label_Notif_Home_Header.Text))
                    toeastElement(1).AppendChild(notificationXml.CreateTextNode(Label_Notif_Home_Content.Text))
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
    Private Sub Home_button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Home_button.Tapped
        Go_Home()
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
                If AdressBox.Text.ToLower.Contains("http://") OrElse AdressBox.Text.ToLower.Contains("https://") Then  'URL invalide si pas de http://
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
                'toeastElement(0).AppendChild(notificationXml.CreateTextNode(resourceLoader.GetString("Notification_Search_Header/Text")))
                'toeastElement(1).AppendChild(notificationXml.CreateTextNode(resourceLoader.GetString("Notification_Search_Content/Text")))
                toeastElement(0).AppendChild(notificationXml.CreateTextNode(Label_Notif_Search_Header.Text))
                toeastElement(1).AppendChild(notificationXml.CreateTextNode(Label_Notif_Search_Content.Text))
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
        PageReady()
    End Sub

    Private Sub Back_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Back_Button.Tapped
        If web.CanGoBack Then
            web.GoBack() 'Permet de revenir à la page précédente
        Else
            Back_Button.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Sub Forward_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Forward_Button.Tapped
        If web.CanGoForward Then
            web.GoForward() 'Revenir à la page suivante
        Else
            Forward_Button.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Sub OnNewWindowRequested(sender As WebView, e As WebViewNewWindowRequestedEventArgs) Handles web.NewWindowRequested
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        'Force l'ouverture dans Blueflap de liens censés s'ouvrir dans une nouvelle fenêtre
        If localSettings.Values("NewWin") = False Then
            e.Handled = True


            localSettings.Values("LoadPageFromBluestart") = True
            localSettings.Values("LoadPageFromBluestart_Adress") = e.Uri.ToString
            NewWindow()
        Else
            web.Navigate(e.Uri)
            e.Handled = True
        End If
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
            If (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) Then
                webcontainer.Margin = New Thickness(0, 0, 0, 0)
                Enterfullscreen_Mobile.Begin()
            Else
                Enterfullscreen.Begin()
                EchapFullScreen.Begin()
            End If


        Else
            Dim appView = ApplicationView.GetForCurrentView
            appView.ExitFullScreenMode()
            If (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) Then
                Enterfullscreen_Mobile.Stop()
                webcontainer.Margin = New Thickness(0, 0, 0, 50)
            Else
                Enterfullscreen.Stop()
                EchapFullScreen.Stop()
            End If

        End If
    End Sub
#End Region
#Region "First Launch"
    Private Sub FirstLaunch() 'Définit les paramètres par défaut
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        If Not localSettings.Values("Config") = True Then
            localSettings.Values("WallpaperName") = "Degrade.png"
            localSettings.Values("Homepage") = "https://www.youtube.com/playlist?list=PLTpU6RJ7jTGx2zaffbccbiNq0Rd2bPsOr"
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
            localSettings.Values("Fav_Confirmation") = True
        End If

        If Not localSettings.Values("FirstBoot") = "Non" Then
            WriteJsonFile(JsonArray.Parse("[]"), "History")
            WriteJsonFile(JsonArray.Parse("[]"), "Favorites")
            WriteJsonFile(JsonArray.Parse("[]"), "Memos")
            localSettings.Values("FirstBoot") = "Non"
            If PhoneNavBar.Visibility = Visibility.Visible Then
                Me.Frame.Navigate(GetType(FirstBootScreen_Mobile))
            Else
                Me.Frame.Navigate(GetType(FirstBootScreen))
            End If
        End If
    End Sub
#End Region
#Region "Right Panel (Memo, history, favorites, notifications)"
    Private Async Sub OpenRightMenu()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        MemoPopOut.Stop()
        MemoPopIN.Begin()

        LeftPanelShadow.Visibility = Visibility.Visible
        RightMenuCache.Visibility = Visibility.Visible
        Try
            If localSettings.Values("AncrageMemo") = True Then 'On  vérifie si l'utilisateur a ancré le volet des mémos
                webcontainer.Margin = New Thickness(webcontainer.Margin.Left, webcontainer.Margin.Top, 261, 0)
                LeftPanelShadow.Visibility = Visibility.Collapsed
                RightMenuCache.Visibility = Visibility.Collapsed
            End If
            MemoAncrageToggleButton.IsChecked = localSettings.Values("AncrageMemo")
        Catch
        End Try
        If RightMenuPivot.SelectedIndex = 0 Then

            If LeftMenu.Visibility = Visibility.Visible Then
                MemoPanel.Width = 261
                MemoPanel.Margin = New Thickness(0, 66, 0, 0)
            End If
            MemoPanel.HorizontalAlignment = HorizontalAlignment.Right
            Memo_ExpandButton.Visibility = Visibility.Collapsed
        End If
        PivotIndicatorPosition()
    End Sub
    Private Sub CloseRightMenu()
        MemoPopIN.Stop()
        MemoPopOut.Begin()
        webcontainer.Margin = New Thickness(webcontainer.Margin.Left, webcontainer.Margin.Top, 0, 0)
        RightMenuCache.Visibility = Visibility.Collapsed
    End Sub
    Private Sub Notifications_indicator_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Notifications_indicator.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        If MemoPanel.Visibility = Visibility.Visible And RightMenuPivot.SelectedIndex = 3 Then
            CloseRightMenu()
        Else
            If MemoPanel.Visibility = Visibility.Collapsed Then
                OpenRightMenu()
            End If
        End If
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
        webcontainer.Margin = New Thickness(webcontainer.Margin.Left, webcontainer.Margin.Top, 0, 0)
        Me.Frame.Navigate(GetType(Parametres))

    End Sub
    Private Sub Memo_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Memo_Button.Tapped
        If MemoPanel.Visibility = Visibility.Visible And RightMenuPivot.SelectedIndex = 0 Then
            CloseRightMenu()
        Else
            If MemoPanel.Visibility = Visibility.Collapsed Then
                OpenRightMenu()
            End If
        End If
        RightMenuPivot.SelectedIndex = 0
    End Sub
    Private Sub MemoAncrageToggleButton_Checked(sender As Object, e As RoutedEventArgs) Handles MemoAncrageToggleButton.Checked
        webcontainer.Margin = New Thickness(webcontainer.Margin.Left, webcontainer.Margin.Top, 261, 0)
        LeftPanelShadow.Visibility = Visibility.Collapsed
        RightMenuCache.Visibility = Visibility.Collapsed

        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("AncrageMemo") = True
    End Sub

    Private Sub MemoAncrageToggleButton_Unloaded(sender As Object, e As RoutedEventArgs) Handles MemoAncrageToggleButton.Unchecked
        webcontainer.Margin = New Thickness(webcontainer.Margin.Left, webcontainer.Margin.Top, 0, 0)
        LeftPanelShadow.Visibility = Visibility.Visible
        RightMenuCache.Visibility = Visibility.Visible
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("AncrageMemo") = False
    End Sub

    Private Sub Memo_ExpandButton_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Memo_ExpandButton.Tapped
        If MemoPanel.Width = 261 Then
            MemoPanel.Width = Double.NaN
            MemoPanel.Margin = New Thickness(48, 66, 0, 0)
            MemoPanel.HorizontalAlignment = HorizontalAlignment.Stretch
            LeftPanelShadow.Visibility = Visibility.Collapsed
            RightMenuCache.Visibility = Visibility.Collapsed
        Else
            MemoPanel.Width = 261
            MemoPanel.Margin = New Thickness(0, 66, 0, 0)
            MemoPanel.HorizontalAlignment = HorizontalAlignment.Right
            If MemoAncrageToggleButton.IsChecked Then
                LeftPanelShadow.Visibility = Visibility.Collapsed
                RightMenuCache.Visibility = Visibility.Collapsed
            Else
                LeftPanelShadow.Visibility = Visibility.Visible
                RightMenuCache.Visibility = Visibility.Visible
            End If
        End If

    End Sub
    Private Sub Notification()

    End Sub
    Private Sub ContextNotification()
        SmartSuggest_OpenSearch.Visibility = Visibility.Collapsed
        Try
            If Windows.Storage.ApplicationData.Current.LocalSettings.Values("Context_Notif") = "No" Then
                Notif_Diminutweet.Visibility = Visibility.Collapsed
                Notif_SearchEngineSuggestion.Visibility = Visibility.Collapsed

            Else
                OpenSearchEngine = False
                If web.Source.ToString.Contains("www.bing.com") Then
                    limitedOpenSearch = True
                    OpenSearchNotif()
                    SmartSuggest_OpenSearchIcon.Source = New BitmapImage(New Uri("http://" & web.Source.Host & "/favicon.ico", UriKind.Absolute))

                    If Notif_SearchEngineSuggestion.Visibility = Visibility.Collapsed Then
                        New_Notif.Begin()
                        Notifications_Counter.Text = Notifications_Counter.Text + 1
                        Notif_Home.Visibility = Visibility.Collapsed
                    End If
                    Notif_SearchEngineName.Text = "BING"
                    Notif_SearchEngineIcon.Source = New BitmapImage(New Uri("ms-appx:/Assets/Engine_Bing.png", UriKind.Absolute))
                    Notif_SearchEngineSuggestion.Visibility = Visibility.Visible
                ElseIf web.Source.ToString.Contains("www.qwant.com") Then
                    limitedOpenSearch = True
                    OpenSearchNotif()
                    SmartSuggest_OpenSearchIcon.Source = New BitmapImage(New Uri("http://" & web.Source.Host & "/favicon.ico", UriKind.Absolute))

                    If Notif_SearchEngineSuggestion.Visibility = Visibility.Collapsed Then
                        New_Notif.Begin()
                        Notifications_Counter.Text = Notifications_Counter.Text + 1
                        Notif_Home.Visibility = Visibility.Collapsed
                    End If
                    Notif_SearchEngineName.Text = "QWANT"
                    Notif_SearchEngineIcon.Source = New BitmapImage(New Uri("ms-appx:/Assets/Engine_Qwant.png", UriKind.Absolute))

                    Notif_SearchEngineSuggestion.Visibility = Visibility.Visible
                ElseIf web.Source.ToString.Contains("duckduckgo.com") Then
                    limitedOpenSearch = True
                    OpenSearchNotif()
                    SmartSuggest_OpenSearchIcon.Source = New BitmapImage(New Uri("http://" & web.Source.Host & "/favicon.ico", UriKind.Absolute))

                    If Notif_SearchEngineSuggestion.Visibility = Visibility.Collapsed Then
                        New_Notif.Begin()
                        Notifications_Counter.Text = Notifications_Counter.Text + 1
                        Notif_Home.Visibility = Visibility.Collapsed
                    End If
                    Notif_SearchEngineName.Text = "DUCKDUCKGO"
                    Notif_SearchEngineIcon.Source = New BitmapImage(New Uri("ms-appx:/Assets/Engine_Duck.png", UriKind.Absolute))

                    Notif_SearchEngineSuggestion.Visibility = Visibility.Visible
                ElseIf web.Source.ToString.Contains("yahoo.com") Then
                    limitedOpenSearch = True
                    OpenSearchNotif()
                    SmartSuggest_OpenSearchIcon.Source = New BitmapImage(New Uri("http://" & web.Source.Host & "/favicon.ico", UriKind.Absolute))

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
                    Try
                        limitedOpenSearch = False
                        OpenSearchNotif()
                        Notif_Home.Visibility = Visibility.Collapsed
                    Catch
                    End Try
                End If

                If web.Source.Host.ToString.Contains("twitter.com") Then
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

                If web.Source.ToString.Contains("vimeo.com/") Or web.Source.ToString.Contains("dailymotion.com/video/") Or web.Source.ToString.Contains("youtube.com/watch?v=") Then
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
                            Notif_Home.Visibility = Visibility.Collapsed
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


            End If
        Catch
            Windows.Storage.ApplicationData.Current.LocalSettings.Values("Context_Notif") = "Yes"
            ContextNotification()
        End Try
        Notification()
    End Sub
    Private Async Sub OpenSearchNotif()
        Try

            If limitedOpenSearch = False Then
                OpenSearchEngine = True
            End If
            Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
            ' Opensearch

            ' Detection du xml
            Dim xmlUri As Uri
            Dim html As String = Await (web.InvokeScriptAsync("eval", New String() {"document.documentElement.outerHTML;"}))
            Dim Found As Boolean = False

            Try
                While Not Found
                    Dim tagStart As Integer = html.IndexOf("<link")
                    Dim tagEnd As Integer = html.Substring(tagStart).IndexOf(">")
                    Dim tag As String = html.Substring(tagStart, tagEnd)
                    If tag.Contains("application/opensearchdescription+xml") Then
                        Found = True
                        Dim attStart As Integer = tag.IndexOf("href=""")
                        Dim attEnd As Integer = tag.Substring(attStart + 6).IndexOf("""")
                        Dim att As String = tag.Substring(attStart + 6, attEnd)
                        xmlUri = New Uri(web.Source, att)
                    Else
                        html = html.Substring(tagEnd)
                    End If
                End While

                ' Recuperation du XML
                Dim client As HttpClient = New HttpClient
                Dim xml As String
                Try
                    Dim res As HttpResponseMessage = Await client.GetAsync(xmlUri)
                    res.EnsureSuccessStatusCode()
                    xml = Await res.Content.ReadAsStringAsync
                Catch ex As Exception
                End Try

                ' Parsage (ce mot existe ?) du XML
                Dim doc As XDocument = XDocument.Parse(xml)
                Dim root As XElement = doc.Elements.FirstOrDefault
                Dim name As String
                Dim img As String
                Dim searchTemplate As String

                Try
                    name = root.Elements.First(Function(x) x.Name.LocalName = "ShortName").Value.ToUpperInvariant
                    img = root.Elements.First(Function(x) x.Name.LocalName = "Image").Value
                    Dim urlTag = root.Elements.Where(Function(x) x.Name.LocalName = "Url").First(Function(x) x.Attributes.Any(Function(y)
                                                                                                                                  Return y.Name.LocalName = "type" And y.Value = "text/html"
                                                                                                                              End Function))
                    searchTemplate = urlTag.Attributes.First(Function(x) x.Name.LocalName = "template").Value

                    If Not searchTemplate.Contains("{searchTerms}") Then
                        Dim param = urlTag.Elements.First(Function(x) x.Name.LocalName = "Param")
                        searchTemplate += "?" + param.Attributes.First(Function(x) x.Name.LocalName = "name").Value + "=" + param.Attributes.First(Function(x) x.Name.LocalName = "value").Value
                    End If

                Catch ex As Exception
                    If img Is Nothing Then
                        img = "http://" & web.Source.Host & "/favicon.ico"
                    End If
                    If name Is Nothing Then
                        name = "INCONNU"
                    End If
                    If searchTemplate Is Nothing Then
                        searchTemplate = "http://" + web.Source.Host + "/?q={searchTerms}"
                    End If
                End Try



                Dim splitter As String = "{searchTerms}"
                Dim A() As String = searchTemplate.Split(New String() {splitter}, StringSplitOptions.None)
                OpenSearch_A1 = A(0)

                If Not String.IsNullOrEmpty(A(1)) Then
                    OpenSearch_A2 = A(1)
                Else
                    OpenSearch_A2 = ""
                End If



                Notif_Home.Visibility = Visibility.Collapsed
                Notif_SearchEngineSuggestion.Visibility = Visibility.Visible

                Notification()

                name = name.Replace("Ã©", "É")

                If limitedOpenSearch = False Then
                    Notif_SearchEngineName.Text = name
                    Notif_SearchEngineIcon.Source = New BitmapImage(New Uri(img, UriKind.Absolute))
                    SmartSuggest_OpenSearchIcon.Source = New BitmapImage(New Uri(img, UriKind.Absolute))
                End If

                SmartSuggest_OpenSearch.Visibility = Visibility.Visible

            Catch
                OpenSearchEngine = False
            End Try
        Catch
        End Try

    End Sub
    Private Sub PivotIndicatorPosition()
        History_SearchMode = False

        If RightMenuPivot.SelectedIndex = 0 Then
            MemoIndexIndicator.Margin = New Thickness(4, 8, 0, 0)
            History_SearchBar.Visibility = Visibility.Collapsed
            History_ShowSearchBar.Visibility = Visibility.Collapsed

            Memo_ShowAll.Visibility = Visibility.Collapsed
            MemoEdit.Visibility = Visibility.Collapsed
            MemListScroll.Visibility = Visibility.Visible
            Try
                ShowMemoList()
            Catch
            End Try

        ElseIf RightMenuPivot.SelectedIndex = 1 Then
            MemoIndexIndicator.Margin = New Thickness(44, 8, 0, 0)
            Memo_ExpandButton.Visibility = Visibility.Visible
            History_SearchBar.Visibility = Visibility.Collapsed
            History_ShowSearchBar.Visibility = Visibility.Collapsed
            sorttag.Visibility = Visibility.Collapsed
            favs_tagsearch = False
            Try
                ShowFavorites()
            Catch ex As Exception
            End Try
        ElseIf RightMenuPivot.SelectedIndex = 2 Then
            MemoIndexIndicator.Margin = New Thickness(84, 8, 0, 0)
            Memo_ExpandButton.Visibility = Visibility.Visible
            History_ShowSearchBar.Visibility = Visibility.Visible
            Try
                ShowHistory()
            Catch ex As Exception
            End Try
        ElseIf RightMenuPivot.SelectedIndex = 3 Then
            MemoIndexIndicator.Margin = New Thickness(124, 8, 0, 0)


            If LeftMenu.Visibility = Visibility.Visible Then
                MemoPanel.Width = 261
                MemoPanel.Margin = New Thickness(0, 66, 0, 0)
            End If
            MemoPanel.HorizontalAlignment = HorizontalAlignment.Right
            Memo_ExpandButton.Visibility = Visibility.Collapsed
            History_SearchBar.Visibility = Visibility.Collapsed
            History_ShowSearchBar.Visibility = Visibility.Collapsed

        End If
        If LeftMenu.Visibility = Visibility.Collapsed Then
            MemoAncrageToggleButton.Visibility = Visibility.Collapsed
            Memo_ExpandButton.IsEnabled = False
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

        If OpenSearchEngine = True Then
            localSettings.Values("Custom_SearchEngine") = True
            localSettings.Values("A1") = OpenSearch_A1.ToString
            localSettings.Values("A2") = OpenSearch_A2.ToString
            localSettings.Values("Cust1") = OpenSearch_A1.ToString
            localSettings.Values("Cust2") = OpenSearch_A2.ToString
            localSettings.Values("SearchEngineIndex") = 12
        Else
            localSettings.Values("Custom_SearchEngine") = False
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
        End If

        Dim notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02)
        Dim toeastElement = notificationXml.GetElementsByTagName("text")
        'toeastElement(0).AppendChild(notificationXml.CreateTextNode(resourceLoader.GetString("Notification_SearchEngineSet_Header/Text")))
        'toeastElement(1).AppendChild(notificationXml.CreateTextNode(resourceLoader.GetString("Notification_SearchEngineSet_Content/Text")))
        toeastElement(0).AppendChild(notificationXml.CreateTextNode(Label_Notif_SearchSet_Header.Text))
        toeastElement(1).AppendChild(notificationXml.CreateTextNode(Label_Notif_SearchSet_Content.Text))
        Dim ToastNotification = New ToastNotification(notificationXml)
        ToastNotificationManager.CreateToastNotifier().Show(ToastNotification)
    End Sub
#End Region
#Region "Favorites"
    Private Async Function ShowFavorites() As Task
        FavList.Children.Clear()
        TagsContainer.Children.Clear()
        Dim PreventMultipleSameItems As New HashSet(Of String)()
        Loader_MemoPanel.IsActive = True
        For Each favsElem In JsonArray.Parse(Await ReadJsonFile("Favorites")).Reverse

            Dim elemContainer As StackPanel = New StackPanel
            elemContainer.Padding = New Thickness(8, 8, 0, 8)
            AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                        web.Navigate(New Uri(favsElem.GetObject.GetNamedString("url")))
                                                                    End Function)

            If PhoneNavBar.Visibility = Visibility.Collapsed Then
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
            End If

            Dim menu As MenuFlyout = New MenuFlyout
            Dim menuDelete As MenuFlyoutItem = New MenuFlyoutItem
            'menuDelete.Text = resourceLoader.GetString("Delete/Text")
            menuDelete.Text = Label_Delete.Text
            menu.Items.Add(menuDelete)
            Dim MenuCopy As MenuFlyoutItem = New MenuFlyoutItem
            'MenuCopy.Text = resourceLoader.GetString("CopyURL/Text")
            MenuCopy.Text = Label_CopyURL.Text
            menu.Items.Add(MenuCopy)
            Dim SetFavHomePage As MenuFlyoutItem = New MenuFlyoutItem
            'SetFavHomePage.Text = resourceLoader.GetString("SetAsMyHomepage/Text")
            SetFavHomePage.Text = Label_SetAsHomepage.Text
            menu.Items.Add(SetFavHomePage)

            AddHandler elemContainer.RightTapped, New RightTappedEventHandler(Function(sender As Object, e As RightTappedRoutedEventArgs)
                                                                                  menu.ShowAt(CType(sender, FrameworkElement))
                                                                              End Function)

            AddHandler menuDelete.Tapped, New TappedEventHandler(Async Sub(sender As Object, e As TappedRoutedEventArgs)
                                                                     Try
                                                                         Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("Favorites"))
                                                                         root.Remove(root.First(Function(x) x.GetObject.GetNamedString("url") = favsElem.GetObject.GetNamedString("url")))
                                                                         WriteJsonFile(root, "Favorites")
                                                                         Await ShowFavorites()
                                                                     Catch ex As Exception
                                                                     End Try
                                                                 End Sub)



            AddHandler MenuCopy.Tapped, New TappedEventHandler(Async Sub(sender As Object, e As TappedRoutedEventArgs)
                                                                   Dim DataPackage = New DataPackage
                                                                   DataPackage.SetText(favsElem.GetObject.GetNamedString("url").ToString)
                                                                   Clipboard.SetContent(DataPackage)
                                                               End Sub)

            AddHandler SetFavHomePage.Tapped, New TappedEventHandler(Async Sub(sender As Object, e As TappedRoutedEventArgs)
                                                                         Windows.Storage.ApplicationData.Current.LocalSettings.Values("Homepage") = favsElem.GetObject.GetNamedString("url")
                                                                         Windows.Storage.ApplicationData.Current.LocalSettings.Values("Bluestart") = False
                                                                     End Sub)

            Dim elemText As TextBlock = New TextBlock
            elemText.Text = favsElem.GetObject.GetNamedString("title")
            elemText.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 40, 40, 40))
            elemContainer.Children.Add(elemText)


            Dim UrlText As TextBlock = New TextBlock
            UrlText.Text = favsElem.GetObject.GetNamedString("url")
            UrlText.Foreground = LeftMenu.Background
            elemContainer.Children.Add(UrlText)

            Dim TagsText As TextBlock = New TextBlock

            For Each favTag As JsonValue In favsElem.GetObject.GetNamedArray("tags")
                TagsText.Text += "#" + favTag.GetString + " "

                If Not PreventMultipleSameItems.Contains("#" + favTag.GetString) Then
                    Dim taglabel As Grid = New Grid
                    taglabel.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(30, 150, 150, 150))
                    taglabel.CornerRadius = New CornerRadius(15)
                    taglabel.Margin = New Thickness(3, 3, 0, 0)
                    Dim taglabeltext As TextBlock = New TextBlock
                    taglabeltext.Text = "#" + favTag.GetString
                    taglabeltext.Margin = New Thickness(7, 6, 7, 4)
                    taglabel.Children.Add(taglabeltext)
                    taglabel.Height = 32
                    taglabel.Width = Double.NaN

                    If PhoneNavBar.Visibility = Visibility.Collapsed Then
                        AddHandler taglabel.PointerEntered, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                        taglabel.Background = LeftMenu.Background
                                                                                        taglabeltext.Foreground = New SolidColorBrush(Windows.UI.Colors.White)
                                                                                    End Function)

                        AddHandler taglabel.PointerExited, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                       taglabel.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(30, 150, 150, 150))
                                                                                       taglabeltext.Foreground = New SolidColorBrush(Windows.UI.Colors.Black)
                                                                                   End Function)

                    End If
                    AddHandler taglabel.Tapped, New TappedEventHandler(Async Function(sender As Object, e As TappedRoutedEventArgs)
                                                                           TagFilter.Text = taglabeltext.Text
                                                                           favs_tagsearch = True
                                                                           sorttag.Visibility = Visibility.Visible
                                                                           Await ShowFavorites()
                                                                       End Function)

                    TagsContainer.Children.Add(taglabel)
                End If
                PreventMultipleSameItems.Add("#" + favTag.GetString)
            Next


            TagsText.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 150, 150, 150))
            elemContainer.Children.Add(TagsText)


            If favs_tagsearch = True Then
                Dim tagsfilter As String = TagFilter.Text.ToLower.Replace("#", "")
                If favsElem.GetObject.GetNamedArray("tags").ToString.ToLower.Contains(tagsfilter) Then
                    FavList.Children.Add(elemContainer)
                End If
            Else
                FavList.Children.Add(elemContainer)
            End If
        Next
        favs_tagsearch = False
        Loader_MemoPanel.IsActive = False
    End Function
    Private Async Sub sorttag_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles sorttag.Tapped
        sorttag.Visibility = Visibility.Collapsed
        favs_tagsearch = False
        Await ShowFavorites()
    End Sub
    Private Sub sorttag_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles sorttag.PointerEntered
        sorttag.BorderThickness = New Thickness(2, 2, 2, 2)
    End Sub

    Private Sub sorttag_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles sorttag.PointerExited
        sorttag.BorderThickness = New Thickness(0, 1, 0, 1)
    End Sub

    Private Async Function AddToFavList() As Task
        Dim CurrentTitle As String = web.DocumentTitle
        Dim VisitDate As DateTime = DateTime.Now

        Try
            Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("Favorites"))

            If root.Any(Function(x As JsonValue) x.GetObject.GetNamedString("url") = web.Source.ToString) Then
                Return
            End If

            If Not Windows.Storage.ApplicationData.Current.LocalSettings.Values("Fav_Confirmation") = False Then
                Dim HistoryElem As JsonObject = New JsonObject
                HistoryElem.Add("url", JsonValue.CreateStringValue(Add_Fav_Url.Text))
                HistoryElem.Add("title", JsonValue.CreateStringValue(Add_Fav_Title.Text))

                If Add_Fav_Tags.Text = "" Then
                    Add_Fav_Tags.Text = Add_Fav_Title.Text
                End If

                Dim tags As JsonArray = JsonArray.Parse("[]")

                Add_Fav_Tags.Text = Add_Fav_Tags.Text.Replace(" , ", ",")
                Add_Fav_Tags.Text = Add_Fav_Tags.Text.Replace(", ", ",")
                Add_Fav_Tags.Text = Add_Fav_Tags.Text.ToLower

                For Each favTag As String In Add_Fav_Tags.Text.Split(New String() {","}, StringSplitOptions.None)
                    tags.Add(JsonValue.CreateStringValue(favTag))
                Next

                HistoryElem.Add("tags", tags)
                root.Add(HistoryElem)
                WriteJsonFile(root, "Favorites")
            Else
                Dim HistoryElem As JsonObject = New JsonObject
                HistoryElem.Add("url", JsonValue.CreateStringValue(web.Source.ToString))
                HistoryElem.Add("title", JsonValue.CreateStringValue(web.DocumentTitle))
                root.Add(HistoryElem)
                WriteJsonFile(root, "Favorites")
            End If
            favbarmodif = True
        Catch
            WriteJsonFile(JsonArray.Parse("[]"), "Favorites")
        End Try
    End Function
    Private Async Sub AddToFavorite_Ask()
        If Not Windows.Storage.ApplicationData.Current.LocalSettings.Values("Fav_Confirmation") = False Then
            Try
                Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("Favorites"))

                If root.Any(Function(x As JsonValue) x.GetObject.GetNamedString("url") = web.Source.ToString) Then
                    OpenRightMenu()
                    RightMenuPivot.SelectedIndex = 1
                Else
                    Add_Fav_Url.Text = web.Source.ToString
                    Add_Fav_Title.Text = web.DocumentTitle
                    Add_Fav_Tags.Text = ""
                    Add_Fav_Url.IsReadOnly = True
                    Add_Fav_Url.BorderThickness = New Thickness(0, 0, 0, 0)
                    AddFav_PopUp_Open.Begin()
                End If
            Catch ex As Exception
            End Try

        Else
            Await AddToFavList()
            If MemoPanel.Visibility = Visibility.Visible And RightMenuPivot.SelectedIndex = 1 Then
                Try
                    Await ShowFavorites()
                Catch ex As Exception
                End Try

            End If
            LikePageButton.Text = ""
            LikePageButton.Foreground = LeftMenu.Background
            Like_Anim.Begin()
        End If

    End Sub

    Private Sub LikePageButton_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles LikePageButton.Tapped
        AddToFavorite_Ask()
    End Sub

    Private Sub Button_Tapped_2(sender As Object, e As TappedRoutedEventArgs)
        Add_Fav_Url.IsReadOnly = False
        Add_Fav_Url.BorderThickness = New Thickness(0, 1, 0, 0)
    End Sub

    Private Async Sub AddFav_Confirm_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles AddFav_Confirm.Tapped
        Await AddToFavList()
        If MemoPanel.Visibility = Visibility.Visible And RightMenuPivot.SelectedIndex = 1 Then
            Try
                Await ShowFavorites()
            Catch ex As Exception
            End Try
        End If
        LikePageButton.Text = ""
        LikePageButton.Foreground = LeftMenu.Background
        Like_Anim.Begin()
        AddFav_PopUp_Close.Begin()

    End Sub

    Private Sub AddFav_Cancel_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles AddFav_Cancel.Tapped
        AddFav_PopUp_Close.Begin()
    End Sub
    Private Sub AddFav_Confirm_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles AddFav_Confirm.PointerEntered
        Addfav_Background.Opacity = 0.8
    End Sub

    Private Sub AddFav_Confirm_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles AddFav_Confirm.PointerExited
        Addfav_Background.Opacity = 1
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
        Sharing.Begin()
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


    Private Async Sub Info_Pin_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Info_Pin.Tapped

    End Sub
    Private Sub Info_Like_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Info_Like.Tapped
        AddToFavorite_Ask()
    End Sub
#End Region
#Region "SmartSuggest"
    Private Sub AdressBox_GotFocus(sender As Object, e As RoutedEventArgs) Handles AdressBox.GotFocus
        AdressBoxEditing = True
        LikePageButton.Visibility = Visibility.Collapsed
        SmartSuggest.Background = Adressbar.Background
        History_Suggestions.Background = SmartSuggest.Background

        Try
            AdressBox.Text = web.Source.ToString
            Titlebox.Text = web.DocumentTitle
        Catch
        End Try

        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        If localSettings.Values("DarkThemeEnabled") = True Then
            SmartSuggest.RequestedTheme = ElementTheme.Dark
            SmartSuggest_HeartIcon.RequestedTheme = ElementTheme.Dark
            DerniereRecherche.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(235, 255, 255, 255))
        Else
            SmartSuggest.RequestedTheme = ElementTheme.Light
            SmartSuggest_HeartIcon.RequestedTheme = ElementTheme.Light
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

        Try
            If web.Source.HostNameType = UriHostNameType.Dns And loader.IsActive = False And Not localSettings.Values("Favicon") = False Then
                Favicon.Source = New BitmapImage(New Uri("http://" & web.Source.Host & "/favicon.ico", UriKind.Absolute))
                Favicon.Visibility = Visibility.Visible
            End If
        Catch
        End Try

        SmartSuggest_History.Children.Clear()
        History_Suggestions.Height = 300
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
        LikePageButton.Visibility = Visibility.Visible
        If LeftMenu.Visibility = Visibility.Collapsed Then
            Adressbar.Visibility = Visibility.Collapsed
            SmartSuggest.Visibility = Visibility.Collapsed
        End If
        DarkBackground.Visibility = Visibility.Collapsed
        AdressBoxEditing = False
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
    Private Sub SmartSuggest_OpenSearch_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles SmartSuggest_OpenSearch.PointerEntered
        SmartSuggest_OpenSearch.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(70, 52, 152, 213))
    End Sub
    Private Sub SmartSuggest_OpenSearch_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles SmartSuggest_OpenSearch.PointerExited
        SmartSuggest_OpenSearch.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
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
    Private Sub SmartSuggest_OpenSearch_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles SmartSuggest_OpenSearch.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        Try
            Dim Rech As String
            Rech = AdressBox.Text
            localSettings.Values("textboxe") = AdressBox.Text
            Dim s As String
            s = Rech.ToString
            s = s.Replace("+", "%2B")

            web.Navigate(New Uri(OpenSearch_A1 + s + OpenSearch_A2))
        Catch
        End Try
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
        SmartSuggest_OpenSearch_Text.Text = s

        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        Try
            If localSettings.Values("SmartSuggest") = True Then
                If (AdressBox.Text.Contains(".") = True And AdressBox.Text.Contains(" ") = False And AdressBox.Text.Contains(" .") = False And AdressBox.Text.Contains(". ") = False) Or textArray(0).Contains(":/") = True Or textArray(0).Contains(":\") Then
                    SmartSuggest_URL.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(20, 52, 152, 213))
                    SmartSuggest_Search.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
                    If SmartSuggest.Height < 138 Then
                        If SmartSuggest_OpenSearch.Visibility = Visibility.Visible Then
                            SmartSuggest.Height = 184
                        Else
                            ExpandSuggestions.Begin()
                        End If

                    End If
                ElseIf AdressBox.Text = "" Then
                    SmartSuggest_URL.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
                    SmartSuggest_Search.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
                    HideSuggestions.Begin()
                Else
                    SmartSuggest_Search.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(20, 52, 152, 213))
                    SmartSuggest_URL.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
                    If SmartSuggest.Height < 138 Then
                        If SmartSuggest_OpenSearch.Visibility = Visibility.Visible Then
                            SmartSuggest.Height = 184
                        Else
                            ExpandSuggestions.Begin()
                        End If
                    End If
                End If
                Try
                    ItemCount = 0
                    SmartSuggest_Histo()
                Catch
                End Try
            End If
        Catch ex As Exception
        End Try


    End Sub
    Private Async Sub SmartSuggest_Histo()

        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        Try
            If localSettings.Values("SmartSuggest") = True Then



                History_Suggestions.Background = SmartSuggest.Background
                SmartSuggest_History.Children.Clear()
                Dim Json As String
                Dim PreventMultipleSameItems As New HashSet(Of String)()

                Try
                    Json = Await ReadJsonFile("Favorites")
                Catch ex As Exception
                    Json = "[]"
                End Try

                For Each histElem In JsonArray.Parse(Json).Reverse
                    Dim elemContainer As StackPanel = New StackPanel
                    elemContainer.Padding = New Thickness(34, 8, 0, 8)
                    AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                                web.Navigate(New Uri(histElem.GetObject.GetNamedString("url")))
                                                                            End Function)
                    If PhoneNavBar.Visibility = Visibility.Collapsed Then
                        AddHandler elemContainer.PointerEntered, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                             elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(70, 52, 152, 213))
                                                                                         End Function)

                        AddHandler elemContainer.PointerExited, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                            elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
                                                                                        End Function)

                    End If
                    Dim elemText As TextBlock = New TextBlock
                    elemText.Text = histElem.GetObject.GetNamedString("title")
                    elemText.Foreground = SmartSuggest_Search_Text.Foreground
                    elemContainer.Children.Add(elemText)

                    Dim UrlText As TextBlock = New TextBlock
                    UrlText.Text = histElem.GetObject.GetNamedString("url")
                    UrlText.Foreground = LeftMenu.Background
                    elemContainer.Children.Add(UrlText)



                    If Not PreventMultipleSameItems.Contains(histElem.GetObject.GetNamedString("url").ToLower) Then
                        If histElem.GetObject.GetNamedString("title").ToLower.Contains(AdressBox.Text.ToLower) Or histElem.GetObject.GetNamedString("url").ToLower.Contains(AdressBox.Text.ToLower) Then
                            SmartSuggest_History.Children.Add(elemContainer)

                            ItemCount = ItemCount + 1
                        End If
                    End If

                    PreventMultipleSameItems.Add(histElem.GetObject.GetNamedString("url").ToLower)

                Next

                Try
                    Json = Await ReadJsonFile("History")
                Catch ex As Exception
                    Json = "[]"
                End Try

                Try
                    For Each histElem In JsonArray.Parse(Json).Reverse
                        Dim elemContainer As StackPanel = New StackPanel
                        elemContainer.Padding = New Thickness(34, 8, 0, 8)
                        AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                                    web.Navigate(New Uri(histElem.GetObject.GetNamedString("url")))
                                                                                End Function)

                        If PhoneNavBar.Visibility = Visibility.Collapsed Then
                            AddHandler elemContainer.PointerEntered, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                                 elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(70, 52, 152, 213))
                                                                                             End Function)

                            AddHandler elemContainer.PointerExited, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                                elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
                                                                                            End Function)

                        End If
                        Dim elemText As TextBlock = New TextBlock
                        elemText.Text = histElem.GetObject.GetNamedString("title")
                        elemText.Foreground = SmartSuggest_Search_Text.Foreground
                        elemContainer.Children.Add(elemText)

                        Dim UrlText As TextBlock = New TextBlock
                        UrlText.Text = histElem.GetObject.GetNamedString("url")
                        UrlText.Foreground = LeftMenu.Background
                        elemContainer.Children.Add(UrlText)



                        If Not PreventMultipleSameItems.Contains(histElem.GetObject.GetNamedString("url").ToLower) Then
                            If histElem.GetObject.GetNamedString("title").ToLower.Contains(AdressBox.Text.ToLower) Or histElem.GetObject.GetNamedString("url").ToLower.Contains(AdressBox.Text.ToLower) Then
                                SmartSuggest_History.Children.Add(elemContainer)

                                ItemCount = ItemCount + 1
                            End If
                        End If

                        PreventMultipleSameItems.Add(histElem.GetObject.GetNamedString("url").ToLower)

                    Next

                    Dim h As Integer
                    If SmartSuggest_OpenSearch.Visibility = Visibility.Visible Then
                        h = 46
                    Else
                        h = 0
                    End If
                    If ItemCount = 0 Then
                        History_Suggestions.Height = 0 + 138 + h
                        SmartSuggest.Height = 138 + h
                        SmartSuggest_HeartIcon.Visibility = Visibility.Collapsed
                        SmartSuggest_History.Visibility = Visibility.Collapsed
                    ElseIf ItemCount = 1 Then
                        History_Suggestions.Height = 56 + 138 + h
                        SmartSuggest.Height = 194 + h
                        SmartSuggest_HeartIcon.Visibility = Visibility.Visible
                        SmartSuggest_History.Visibility = Visibility.Visible
                    ElseIf ItemCount = 2 Then
                        History_Suggestions.Height = 112 + 138 + h
                        SmartSuggest.Height = 250 + h
                        SmartSuggest_HeartIcon.Visibility = Visibility.Visible
                        SmartSuggest_History.Visibility = Visibility.Visible
                    ElseIf ItemCount > 2 Then
                        History_Suggestions.Height = 168 + 138 + h
                        SmartSuggest.Height = 306 + h
                        SmartSuggest_HeartIcon.Visibility = Visibility.Visible
                        SmartSuggest_History.Visibility = Visibility.Visible
                    End If
                Catch
                End Try

            End If
        Catch ex As Exception
        End Try
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
    End Sub

#End Region
#Region "JsonFileManagment"
    Private Async Sub WriteJsonFile(Json As JsonArray, FileName As String)
        Dim localFolder As StorageFolder = ApplicationData.Current.LocalFolder
        FileName += ".json"

        Try
            If Not Await localFolder.TryGetItemAsync(FileName) Is Nothing Then
                Dim textfile As StorageFile = Await localFolder.GetFileAsync(FileName)
                Await FileIO.WriteTextAsync(textfile, Json.ToString)
            Else
                Dim textFile As StorageFile = Await localFolder.CreateFileAsync(FileName)
                Await FileIO.WriteTextAsync(textFile, Json.ToString)
            End If
        Catch
        End Try

    End Sub

    Private Async Function ReadJsonFile(FileName As String) As Task(Of String)
        Dim localFolder As StorageFolder = ApplicationData.Current.LocalFolder
        FileName += ".json"
        Dim content As String = Nothing

        Dim textfile As StorageFile = Await localFolder.GetFileAsync(FileName)
        content = Await FileIO.ReadTextAsync(textfile)
        Return content
    End Function
#End Region
#Region "History"

    Private Async Sub ShowHistory()
        HistoryList.Children.Clear()
        Dim Json As String
        Loader_MemoPanel.IsActive = True
        Try
            Json = Await ReadJsonFile("History")
        Catch ex As Exception
            Json = "[]"
        End Try
        Try
            For Each histElem In JsonArray.Parse(Json).Reverse
                Dim elemContainer As StackPanel = New StackPanel
                elemContainer.Padding = New Thickness(8, 8, 0, 8)
                AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                            web.Navigate(New Uri(histElem.GetObject.GetNamedString("url")))
                                                                        End Function)

                If PhoneNavBar.Visibility = Visibility.Collapsed Then
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
                End If
                Dim menu As MenuFlyout = New MenuFlyout
                Dim menuDelete As MenuFlyoutItem = New MenuFlyoutItem
                'menuDelete.Text = resourceLoader.GetString("Delete/Text")
                menuDelete.Text = Label_Delete.Text
                menu.Items.Add(menuDelete)
                Dim MenuCopy As MenuFlyoutItem = New MenuFlyoutItem
                'MenuCopy.Text = resourceLoader.GetString("CopyURL/Text")
                MenuCopy.Text = Label_CopyURL.Text
                menu.Items.Add(MenuCopy)
                Dim SortByUrl As MenuFlyoutItem = New MenuFlyoutItem
                Dim HistUrl = New Uri(histElem.GetObject.GetNamedString("url"))
                'SortByUrl.Text = resourceLoader.GetString("HistorySortByUrl/Text") + " " + HistUrl.Host
                SortByUrl.Text = Label_SearchHistory.Text + " " + HistUrl.Host
                menu.Items.Add(SortByUrl)

                AddHandler elemContainer.RightTapped, New RightTappedEventHandler(Function(sender As Object, e As RightTappedRoutedEventArgs)
                                                                                      menu.ShowAt(CType(sender, FrameworkElement))
                                                                                  End Function)

                AddHandler MenuCopy.Tapped, New TappedEventHandler(Async Sub(sender As Object, e As TappedRoutedEventArgs)
                                                                       Dim DataPackage = New DataPackage
                                                                       DataPackage.SetText(histElem.GetObject.GetNamedString("url").ToString)
                                                                       Clipboard.SetContent(DataPackage)
                                                                   End Sub)

                AddHandler menuDelete.Tapped, New TappedEventHandler(Async Sub(sender As Object, e As TappedRoutedEventArgs)
                                                                         Try
                                                                             Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("History"))
                                                                             root.Remove(root.First(Function(x) x.GetObject.GetNamedString("url") = histElem.GetObject.GetNamedString("url")))
                                                                             WriteJsonFile(root, "History")
                                                                             ShowHistory()
                                                                         Catch
                                                                         End Try
                                                                     End Sub)

                AddHandler SortByUrl.Tapped, New TappedEventHandler(Async Sub(sender As Object, e As TappedRoutedEventArgs)
                                                                        History_SearchMode = True
                                                                        History_SearchKeywords = HistUrl.Host.ToString
                                                                        History_SearchBar.Visibility = Visibility.Visible
                                                                        SearchHistory.Text = HistUrl.Host.ToString
                                                                        ShowHistory()
                                                                    End Sub)

                Dim elemText As TextBlock = New TextBlock
                elemText.Text = histElem.GetObject.GetNamedString("title")
                elemText.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 40, 40, 40))
                elemContainer.Children.Add(elemText)

                Dim UrlText As TextBlock = New TextBlock
                UrlText.Text = histElem.GetObject.GetNamedString("url")
                UrlText.Foreground = LeftMenu.Background
                elemContainer.Children.Add(UrlText)

                Dim visitDate As TextBlock = New TextBlock
                'visitDate.Text = resourceLoader.GetString("DateList_1/Text") + DateTime.FromBinary(histElem.GetObject.GetNamedNumber("date")).ToString("dd MMMMMMMMMMMM yyyy ") + resourceLoader.GetString("DateList_2/Text") + DateTime.FromBinary(histElem.GetObject.GetNamedNumber("date")).ToString(" HH:mm")
                visitDate.Text = Label_Date1.Text + DateTime.FromBinary(histElem.GetObject.GetNamedNumber("date")).ToString("dd MMMMMMMMMMMM yyyy ") + Label_Date2.Text + DateTime.FromBinary(histElem.GetObject.GetNamedNumber("date")).ToString(" HH:mm")
                visitDate.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 150, 150, 150))
                elemContainer.Children.Add(visitDate)

                If History_SearchMode = True Then
                    If histElem.GetObject.GetNamedString("title").ToLower.Contains(History_SearchKeywords.ToLower) Or histElem.GetObject.GetNamedString("url").ToLower.Contains(History_SearchKeywords.ToLower) Then
                        HistoryList.Children.Add(elemContainer)
                    End If
                Else
                    HistoryList.Children.Add(elemContainer)
                End If

            Next
        Catch
            If MemoPanel.Visibility = Visibility.Visible Then
                MemoPopOut.Begin()
            End If
        End Try
        Loader_MemoPanel.IsActive = False
    End Sub
    Private Async Sub SearchHistory_TextChanged(sender As Object, e As TextChangedEventArgs) Handles SearchHistory.TextChanged
        History_SearchMode = True
        History_SearchKeywords = SearchHistory.Text
        Try
            ShowHistory()
        Catch
            If MemoPanel.Visibility = Visibility.Visible Then
                MemoPopOut.Begin()
            End If
        End Try
    End Sub

    Private Sub SearchHistory_LostFocus(sender As Object, e As RoutedEventArgs) Handles SearchHistory.LostFocus
        History_SearchMode = False
    End Sub

    Private Sub History_ShowSearchBar_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles History_ShowSearchBar.Tapped
        If History_SearchBar.Visibility = Visibility.Visible Then
            History_SearchBar.Visibility = Visibility.Collapsed
        Else
            History_SearchBar.Visibility = Visibility.Visible
        End If
        Try
            ShowHistory()
        Catch
            If MemoPanel.Visibility = Visibility.Visible Then
                MemoPopOut.Begin()
            End If
        End Try
    End Sub


#End Region
#Region "Memos"
    Private Async Sub Memo_New_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Memo_New.Tapped
        EditMemo = False

        Memo_ShowAll.Visibility = Visibility.Visible
        MemoEdit.Visibility = Visibility.Visible
        MemListScroll.Visibility = Visibility.Collapsed

        Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("Memos"))
        Dim MemoElem As JsonObject = New JsonObject
        MemoElem.Add("url", JsonValue.CreateStringValue(web.Source.Host.ToString))
        MemoElem.Add("title", JsonValue.CreateStringValue(web.DocumentTitle.ToString))

        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        Try
            localSettings.Values("MemosID") = localSettings.Values("MemosID") + 1
        Catch ex As Exception
            localSettings.Values("MemosID") = 1
        End Try

        MemoElem.Add("ID", JsonValue.CreateStringValue(localSettings.Values("MemosID")))

        MemoElem.Add("Text", JsonValue.CreateStringValue(localSettings.Values("MemosID")))
        root.Add(MemoElem)
        WriteJsonFile(root, "Memos")

        Memo_Edit_Text.Text = localSettings.Values("MemosID")
        Memo_Edit_Title.Text = web.DocumentTitle.ToString

    End Sub
    Private Async Function ShowMemoList() As Task
        MemoList.Children.Clear()
        Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("Memos"))
        Dim Json As String
        Try
            Json = Await ReadJsonFile("Memos")
        Catch ex As Exception
            Json = "[]"
        End Try
        Loader_MemoPanel.IsActive = True
        For Each MemoElem In root.Reverse

            Dim elemContainer As StackPanel = New StackPanel
            elemContainer.Padding = New Thickness(8, 8, 0, 8)

            Dim elemTitle As TextBlock = New TextBlock
            elemTitle.Text = MemoElem.GetObject.GetNamedString("title")
            elemTitle.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 40, 40, 40))
            elemContainer.Children.Add(elemTitle)


            Dim UrlText As TextBlock = New TextBlock
            UrlText.Text = MemoElem.GetObject.GetNamedString("url")
            UrlText.Foreground = LeftMenu.Background
            elemContainer.Children.Add(UrlText)

            AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                        Memo_ShowAll.Visibility = Visibility.Visible
                                                                        MemoEdit.Visibility = Visibility.Visible
                                                                        MemListScroll.Visibility = Visibility.Collapsed
                                                                        Memo_Edit_Text.Text = MemoElem.GetObject.GetNamedString("Text")
                                                                        Memo_Edit_Title.Text = MemoElem.GetObject.GetNamedString("title")
                                                                        Memo_Edit_URL.Text = MemoElem.GetObject.GetNamedString("url").ToUpper
                                                                        EditMemo = True
                                                                        AddHandler Memo_Edit_Title.LostFocus, New RoutedEventHandler(Function(textch As Object, te As RoutedEventArgs)
                                                                                                                                         If EditMemo = True Then
                                                                                                                                             MemoElem.GetObject.SetNamedValue("title", JsonValue.CreateStringValue(Memo_Edit_Title.Text))
                                                                                                                                             root.Remove(root.First(Function(x) x.GetObject.GetNamedString("ID") = MemoElem.GetObject.GetNamedString("ID")))
                                                                                                                                             root.Add(MemoElem)
                                                                                                                                             WriteJsonFile(root, "Memos")
                                                                                                                                         End If
                                                                                                                                     End Function)

                                                                        AddHandler Memo_Edit_Text.LostFocus, New RoutedEventHandler(Function(textch As Object, te As RoutedEventArgs)
                                                                                                                                        If EditMemo = True Then
                                                                                                                                            MemoElem.GetObject.SetNamedValue("Text", JsonValue.CreateStringValue(Memo_Edit_Text.Text))
                                                                                                                                            root.Remove(root.First(Function(x) x.GetObject.GetNamedString("ID") = MemoElem.GetObject.GetNamedString("ID")))
                                                                                                                                            root.Add(MemoElem)
                                                                                                                                            WriteJsonFile(root, "Memos")
                                                                                                                                        End If
                                                                                                                                    End Function)
                                                                    End Function)

            If PhoneNavBar.Visibility = Visibility.Collapsed Then
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
            End If
            Dim menu As MenuFlyout = New MenuFlyout
            Dim menuDelete As MenuFlyoutItem = New MenuFlyoutItem
            ' menuDelete.Text = resourceLoader.GetString("Delete/Text")
            menuDelete.Text = Label_Delete.Text
            menu.Items.Add(menuDelete)


            AddHandler elemContainer.RightTapped, New RightTappedEventHandler(Function(sender As Object, e As RightTappedRoutedEventArgs)
                                                                                  menu.ShowAt(CType(sender, FrameworkElement))
                                                                              End Function)

            AddHandler menuDelete.Tapped, New TappedEventHandler(Async Sub(sender As Object, e As TappedRoutedEventArgs)
                                                                     Try
                                                                         root.Remove(root.First(Function(x) x.GetObject.GetNamedString("ID") = MemoElem.GetObject.GetNamedString("ID")))
                                                                         WriteJsonFile(root, "Memos")
                                                                         Await ShowFavorites()
                                                                     Catch ex As Exception
                                                                     End Try
                                                                 End Sub)

            MemoList.Children.Add(elemContainer)

        Next
        Loader_MemoPanel.IsActive = False
    End Function
    Private Async Sub Memo_ShowAll_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Memo_ShowAll.Tapped
        Memo_ShowAll.Visibility = Visibility.Collapsed
        MemoEdit.Visibility = Visibility.Collapsed
        MemListScroll.Visibility = Visibility.Visible
        Try
            Await ShowMemoList()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Memo_Edit_Text_GotFocus(sender As Object, e As RoutedEventArgs) Handles Memo_Edit_Text.GotFocus
        Memo_Edit_Text.BorderThickness = New Thickness(2, 2, 2, 2)
    End Sub

    Private Sub Memo_Edit_Text_Loaded(sender As Object, e As RoutedEventArgs) Handles Memo_Edit_Text.LostFocus
        Memo_Edit_Text.BorderThickness = New Thickness(0, 0, 0, 0)
    End Sub


#End Region
#Region "PhoneControl"
    Private Sub Grid_Tapped(sender As Object, e As TappedRoutedEventArgs)


        Dim scope = New InputScope
        Dim scopeName = New InputScopeName
        scopeName.NameValue = InputScopeNameValue.Url
        scope.Names.Add(scopeName)
        AdressBox.InputScope = scope
        AdressBox.IsTextPredictionEnabled = True


        Adressbar.Visibility = Visibility.Visible
        AdressBox.Focus(Windows.UI.Xaml.FocusState.Keyboard)
        DarkBackground.Visibility = Visibility.Visible
        Ellipsis_Close.Begin()
    End Sub

    Private Sub Phone_Back_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Phone_Back.Tapped
        If web.CanGoBack Then
            web.GoBack()
        End If
    End Sub
    Private Sub Phone_Forward_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Phone_Forward.Tapped
        If web.CanGoForward Then
            web.GoForward()
        End If
    End Sub

    Private Sub Grid_PointerPressed(sender As Object, e As PointerRoutedEventArgs)
        Tilt_Out.Stop()
        Tilt_In.Stop()
        Tilt_In.Begin()
    End Sub

    Private Sub Grid_PointerReleased(sender As Object, e As PointerRoutedEventArgs)
        Tilt_Out.Begin()
    End Sub

    Private Sub Grid_PointerExited(sender As Object, e As PointerRoutedEventArgs)
        Tilt_Out.Begin()
    End Sub

    Private Sub Grid_Tapped_1(sender As Object, e As TappedRoutedEventArgs)
        If Ellipsis.Visibility = Visibility.Visible Then
            Ellipsis_Close.Begin()
        Else
            Ellipsis_Open.Begin()
        End If
    End Sub

    Private Sub grid_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles grid.SizeChanged
        If LeftMenu.Visibility = Visibility.Visible Then
            Ellipsis_Open.Stop()
        End If
        If Not PhoneNavBar.Opacity = 1 Then
            webcontainer.Margin = New Thickness(0, 0, 0, 0)
        End If
    End Sub

    Private Sub Settings_Button_M_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Settings_Button_M.Tapped
        ' Ellipsis_Close.Begin()
        Me.Frame.Navigate(GetType(Parametres))
    End Sub

    Private Sub Lock_Button_M_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Lock_Button_M.Tapped
        'Ellipsis_Close.Begin()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        LockTheBrowser.IsChecked = True
        Me.Frame.Navigate(GetType(Verrouillage))
    End Sub

    Private Sub Memo_Button_M_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Memo_Button_M.Tapped
        Ellipsis_Close.Begin()
        If MemoPanel.Visibility = Visibility.Collapsed Then
            MemoPopOut.Stop()
            MemoPopIN.Begin()
        End If

        RightMenuPivot.SelectedIndex = 0
    End Sub

    Private Sub Fight_Button_M_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Fight_Button_M.Tapped
        'Ellipsis_Close.Begin()
        Me.Frame.Navigate(GetType(SearchFight))
    End Sub

    Private Sub Notifications_Button_M_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Notifications_Button_M.Tapped
        Ellipsis_Close.Begin()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        If MemoPanel.Visibility = Visibility.Collapsed Then
            MemoPopOut.Stop()
            MemoPopIN.Begin()
        End If
        RightMenuPivot.SelectedIndex = 3
        New_Notif.Stop()
        Notifications_Counter.Text = "0"
        If NotifPosition = 0 Then
            Notif_Home.Visibility = Visibility.Visible
        End If
    End Sub

    Private Sub Share_Button_M_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Share_Button_M.Tapped
        Ellipsis_Close.Begin()
        Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI()
        Sharing.Begin()
    End Sub

    Private Sub Like_Button_M_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Like_Button_M.Tapped
        Ellipsis_Close.Begin()
        AddToFavorite_Ask()
    End Sub

    Private Sub Window_Button_M_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Window_Button_M.Tapped
        Ellipsis_Close.Begin()
        NewWindow()
    End Sub

    Private Sub Strefresh_Button_M_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Strefresh_Button_M.Tapped
        Ellipsis_Close.Begin()
        If MobileRefreshIcon.Text = "" Then
            web.Refresh()
            MobileRefreshIcon.Text = ""
            'MobileRefreshLabel.Text = resourceLoader.GetString("Menu_Stop/Text")
            MobileRefreshLabel.Text = Label_Stop.Text
        Else
            MobileRefreshIcon.Text = ""
            'MobileRefreshLabel.Text = resourceLoader.GetString("Menu_Refresh/Text")
            MobileRefreshLabel.Text = Label_Refresh.Text
            web.Stop()
        End If
        PageReady()
    End Sub

    Private Sub Home_button_M_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Home_button_M.Tapped
        Ellipsis_Close.Begin()
        Go_Home()
    End Sub

    Private Sub DarkBackground_Ellipsis_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles DarkBackground_Ellipsis.Tapped
        Ellipsis_Close.Begin()
    End Sub
    Private Async Sub web_UnviewableContentIdentified(sender As WebView, args As WebViewUnviewableContentIdentifiedEventArgs) Handles web.UnviewableContentIdentified
        Dim success = Await Windows.System.Launcher.LaunchUriAsync(web.Source)

        ' URI launched
        If success Then
            ' URI launch failed
        Else
        End If
    End Sub




#End Region
#Region "MiniPlayer"
    Private Async Sub MiniPlayer()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        Dim t As String
        t = web.Source.ToString
        t = t.Replace("vimeo.com/", "player.vimeo.com/video/")
        t = t.Replace("dailymotion.com/video/", "dailymotion.com/embed/video/")

        If Not (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) Then
            t = t.Replace("youtube.com/watch", "youtube.com/watch_popup")
        Else
            t = t.Replace("m.youtube.com", "www.youtube.com")
            t = t.Replace("youtube.com/watch?v=", "youtube.com/embed/")
            t = t + "?autoplay=0&showinfo=0&controls=0"
        End If

        localSettings.Values("MiniPlayerUri") = t



        If localSettings.Values("MiniPlayerDisplayMode") = 1 Then
            Dim newView As CoreApplicationView = CoreApplication.CreateNewView()
            Dim newViewId As Integer = 0
            Await newView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Function()
                                                                                                 Dim frame As New Frame()
                                                                                                 frame.Navigate(GetType(MiniPlayer), Nothing)
                                                                                                 Window.Current.Content = frame
                                                                                                 'You have to activate the window in order to show it later.
                                                                                                 Window.Current.Activate()

                                                                                                 newViewId = ApplicationView.GetForCurrentView().Id

                                                                                             End Function)
            Dim viewShown As Boolean = Await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId)
        Else
            If MemoPanel.Visibility = Visibility.Visible Then
                CloseRightMenu()
            End If
            If (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) Then
                Dim HttpRequestMessage = New Windows.Web.Http.HttpRequestMessage(Windows.Web.Http.HttpMethod.Get, New Uri(t))
                HttpRequestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.246")
                MiniPlayer_Player.NavigateWithHttpRequestMessage(HttpRequestMessage)
            Else
                MiniPlayer_Player.Source = New Uri(t)
            End If


            MiniPlayer_Close1.Stop()
            MiniPlayer_Background.Visibility = Visibility.Visible

            ReduceMiniPlayer()
        End If




    End Sub

    Private Sub OpenMiniP(sender As Object, e As TappedRoutedEventArgs)
        MiniPlayer()
    End Sub

    Private Sub MiniPlayer_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles MiniPlayer_Button.Tapped
        MiniPlayer()
    End Sub
    Private Sub ProgressBarre_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs) Handles ProgressBarre.ValueChanged

        Try
            If ProgressBarre.Value > 5 Then
                If AdressBoxEditing = False Then
                    AdressBox.Text = web.Source.ToString
                    Phone_URL.Text = web.Source.Host
                    Titlebox.Text = web.DocumentTitle
                End If
            End If
        Catch
        End Try
    End Sub
    Private Sub web_ContentLoading(sender As WebView, args As WebViewContentLoadingEventArgs) Handles web.ContentLoading
        InvisibleLoading.Begin()
    End Sub

    Private Sub web_FrameNavigationStarting(sender As WebView, args As WebViewNavigationStartingEventArgs) Handles web.FrameNavigationStarting
        InvisibleLoading.Begin()
    End Sub

    Private Sub web_Loading(sender As FrameworkElement, args As Object) Handles web.Loading
        InvisibleLoading.Begin()
    End Sub
    Private Sub BackgroundProgressBarre_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs) Handles BackgroundProgressBarre.ValueChanged

        Try
            If ProgressBarre.Value > 5 Then
                If AdressBoxEditing = False Then

                    If Not AdressBox.Text = web.Source.ToString Then
                        PageChanged()
                    End If
                    AdressBox.Text = web.Source.ToString
                    Phone_URL.Text = web.Source.Host
                    Titlebox.Text = web.DocumentTitle
                End If
            End If
        Catch
        End Try
    End Sub

    Private Sub PageChanged()
        PageReady(False, True, True)
    End Sub

    Private Sub MiniPlayer_Background_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles MiniPlayer_Background.SizeChanged
        Dim h = (1 / 16) * 9 * MiniPlayer_Resize.ActualWidth
        Dim s = MiniPlayer_Background.ActualHeight - 20
        If MiniPlayer_Background.VerticalAlignment = VerticalAlignment.Stretch Then

            If h < MiniPlayer_Background.ActualHeight Then
                MiniPlayer_Resize.Height = h
            Else
                MiniPlayer_Resize.Height = s
            End If
        End If
    End Sub

    Private Sub Button_Tapped_3(sender As Object, e As TappedRoutedEventArgs)
        ReduceMiniPlayer()
    End Sub
    Private Sub ReduceMiniPlayer()
        MiniPlayer_Background.VerticalAlignment = VerticalAlignment.Bottom
        MiniPlayer_Background.HorizontalAlignment = HorizontalAlignment.Right
        MiniPlayer_Background.Width = 280
        MiniPlayer_Background.Height = 210

        MiniPlayer_Background.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0))
        MiniPlayer_Resize.Height = 180
        If (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) Then
            MiniPlayer_Background.Margin = New Thickness(0, 0, -38, -35)
        End If
        MiniPlayer_Show.Visibility = Visibility.Visible
        MiniPlayer_PopOut.Begin()
        MiniPlayer_Reduce.Visibility = Visibility.Collapsed
        MiniPlayer_Close.Visibility = Visibility.Collapsed
    End Sub


    Private Sub MiniPlayer_Show_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles MiniPlayer_Show.Tapped
        MiniPlayer_Background.VerticalAlignment = VerticalAlignment.Stretch
        MiniPlayer_Background.HorizontalAlignment = HorizontalAlignment.Stretch
        MiniPlayer_Background.Width = Double.NaN
        MiniPlayer_Background.Height = Double.NaN

        MiniPlayer_Background.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(153, 0, 0, 0))
        MiniPlayer_Show.Visibility = Visibility.Collapsed
        MiniPlayer_PopIN.Begin()
        MiniPlayer_Reduce.Visibility = Visibility.Visible
        MiniPlayer_Close.Visibility = Visibility.Collapsed
        If (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) Then
            MiniPlayer_Close.Visibility = Visibility.Visible
            MiniPlayer_Background.Margin = New Thickness(0, 0, 0, 0)
        End If


    End Sub

    Private Sub MiniPlayer_Close_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles MiniPlayer_Close.Tapped
        MiniPlayer_Player.Source = New Uri("about:blank")
        MiniPlayer_Close1.Begin()
    End Sub

    Private Sub MiniPlayer_Show_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles MiniPlayer_Show.PointerEntered
        If Not (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) Then
            MiniPlayer_MouseHover.Begin()
        End If

    End Sub

    Private Sub MiniPlayer_Show_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles MiniPlayer_Show.PointerExited
        If Not (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) Then
            MiniPlayer_Mouseleave.Begin()
        End If

    End Sub

    Private Sub MiniPlayer_Background_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles MiniPlayer_Background.PointerEntered
        If Not (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) Then
            MiniPlayer_Close.Visibility = Visibility.Visible
        End If

    End Sub

    Private Sub MiniPlayer_Background_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles MiniPlayer_Background.PointerExited

        If Not (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) Then
            MiniPlayer_Close.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Sub MiniPlayer_Close_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles MiniPlayer_Close.PointerEntered
        MiniPlayer_CloseButHover.Begin()
    End Sub

    Private Sub MiniPlayer_Close_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles MiniPlayer_Close.PointerExited
        MiniPlayer_CloseButHover.Stop()
    End Sub

    Private Sub LikePageButton_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles LikePageButton.PointerEntered
        If LikePageButton.Text = "" Then
            LikePageButton.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 224, 139, 213))
        End If

    End Sub

    Private Sub LikePageButton_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles LikePageButton.PointerExited
        If LikePageButton.Text = "" Then
            LikePageButton.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 122, 122, 122))
        End If
    End Sub
#End Region
#Region "Divers"

    Private Sub Gost_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Gost_Button.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        If localSettings.Values("GhostMode") = True Then
            localSettings.Values("GhostMode") = False
            Gost_Button.BorderBrush = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 255, 255, 255))
        Else
            localSettings.Values("GhostMode") = True
            Gost_Button.BorderBrush = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255))
        End If
    End Sub

    Private Sub TagsContainer_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles TagsContainer.PointerEntered
        tagscrol.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
    End Sub
    Private Sub TagsContainer_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles TagsContainer.PointerExited
        tagscrol.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden
    End Sub

    Private Sub RightMenuCache_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles RightMenuCache.Tapped
        CloseRightMenu()
    End Sub
    Private Async Function ShowFavBar() As Task
        showingfavbar = True
        Try
            favbarstack.Children.Clear()
            FavoritesBar.Background = Adressbar.Background
            Dim count As String = 0
            For Each favsElem In JsonArray.Parse(Await ReadJsonFile("Favorites")).Reverse
                count = count + 1
                Dim elemContainer As Grid = New Grid
                elemContainer.Margin = New Thickness(0, 0, 2, 0)
                elemContainer.CornerRadius = New CornerRadius(2)
                elemContainer.VerticalAlignment = VerticalAlignment.Stretch
                elemContainer.HorizontalAlignment = HorizontalAlignment.Left
                elemContainer.Height = Double.NaN
                elemContainer.Width = Double.NaN
                elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0))

                Dim content As StackPanel = New StackPanel
                content.Orientation = Orientation.Horizontal
                content.Margin = New Thickness(5, 0, 5, 0)
                content.VerticalAlignment = VerticalAlignment.Stretch
                content.HorizontalAlignment = HorizontalAlignment.Stretch
                content.MaxWidth = 150


                Dim image As Image = New Image
                image.Width = 17
                image.Height = 17
                image.Margin = New Thickness(2, 0, 0, 0)
                image.HorizontalAlignment = HorizontalAlignment.Left
                image.VerticalAlignment = VerticalAlignment.Center
                content.Children.Add(image)

                Dim menu As MenuFlyout = New MenuFlyout
                Dim menuDelete As MenuFlyoutItem = New MenuFlyoutItem
                menuDelete.Text = Label_Delete.Text
                menu.Items.Add(menuDelete)
                Dim MenuCopy As MenuFlyoutItem = New MenuFlyoutItem
                MenuCopy.Text = Label_CopyURL.Text
                menu.Items.Add(MenuCopy)
                Dim SetFavHomePage As MenuFlyoutItem = New MenuFlyoutItem
                SetFavHomePage.Text = Label_SetAsHomepage.Text
                menu.Items.Add(SetFavHomePage)

                AddHandler elemContainer.RightTapped, New RightTappedEventHandler(Function(sender As Object, e As RightTappedRoutedEventArgs)
                                                                                      menu.ShowAt(CType(sender, FrameworkElement))
                                                                                  End Function)

                AddHandler menuDelete.Tapped, New TappedEventHandler(Async Sub(sender As Object, e As TappedRoutedEventArgs)
                                                                         Try
                                                                             Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("Favorites"))
                                                                             root.Remove(root.First(Function(x) x.GetObject.GetNamedString("url") = favsElem.GetObject.GetNamedString("url")))
                                                                             WriteJsonFile(root, "Favorites")
                                                                             Await ShowFavorites()
                                                                         Catch ex As Exception
                                                                         End Try
                                                                     End Sub)



                AddHandler MenuCopy.Tapped, New TappedEventHandler(Async Sub(sender As Object, e As TappedRoutedEventArgs)
                                                                       Dim DataPackage = New DataPackage
                                                                       DataPackage.SetText(favsElem.GetObject.GetNamedString("url").ToString)
                                                                       Clipboard.SetContent(DataPackage)
                                                                   End Sub)

                AddHandler SetFavHomePage.Tapped, New TappedEventHandler(Async Sub(sender As Object, e As TappedRoutedEventArgs)
                                                                             Windows.Storage.ApplicationData.Current.LocalSettings.Values("Homepage") = favsElem.GetObject.GetNamedString("url")
                                                                             Windows.Storage.ApplicationData.Current.LocalSettings.Values("Bluestart") = False
                                                                         End Sub)


                Try
                    Dim str As Uri = New Uri(favsElem.GetObject.GetNamedString("url"))

                    Dim request As Net.HttpWebRequest = DirectCast(Net.HttpWebRequest.Create("http://" & str.Host & "/favicon.ico"), Net.HttpWebRequest)
                    request.Method = "HEAD"
                    Try
                        Await request.GetResponseAsync()
                        image.Source = New BitmapImage(New Uri("http://" & str.Host & "/favicon.ico", UriKind.Absolute))
                    Catch
                        Dim Black1 As BitmapImage = New BitmapImage
                        Black1.UriSource = New Uri("ms-appx:/Assets/faviconNeutral.png", UriKind.Absolute)
                        image.Source = Black1

                    End Try



                Catch ex As Exception

                End Try


                Dim elemText As TextBlock = New TextBlock
                elemText.Text = favsElem.GetObject.GetNamedString("title")
                elemText.Margin = New Thickness(4, 0, 0, 0)
                elemText.FontSize = 12
                elemText.VerticalAlignment = VerticalAlignment.Center
                elemText.HorizontalAlignment = HorizontalAlignment.Left
                elemText.Foreground = AdressBox.Foreground
                content.Children.Add(elemText)

                elemContainer.Children.Add(content)

                AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                            web.Navigate(New Uri(favsElem.GetObject.GetNamedString("url")))
                                                                        End Function)

                If PhoneNavBar.Visibility = Visibility.Collapsed Then
                    AddHandler elemContainer.PointerEntered, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                         elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(100, 128, 128, 128))
                                                                                     End Function)

                    AddHandler elemContainer.PointerExited, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                        elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0))
                                                                                    End Function)

                    favbarstack.Children.Add(elemContainer)
                End If
            Next
            If count = 0 Then
                FavoritesBar.Visibility = Visibility.Collapsed
                showingfavbar = True
            Else
                FavoritesBar.Visibility = Visibility.Visible
                showingfavbar = False
            End If
        Catch
        End Try

    End Function

    Private Async Sub Adressbar_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles Adressbar.PointerEntered

        If FavoritesBar.Visibility = Visibility.Collapsed Then

            If showingfavbar = False Then
                ShowFavBarre.Begin()
                If favbarmodif = True Then
                    Await ShowFavBar()
                    favbarmodif = False
                End If

            End If


        End If
    End Sub

    Private Sub Adressbar_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles Adressbar.PointerExited
        hidefavbar.Begin()
    End Sub

    Private Sub FavMore_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles FavMore.Tapped

        OpenRightMenu()
        RightMenuPivot.SelectedIndex = 1
    End Sub

    Private Sub startshowmore_TextChanged(sender As Object, e As TextChangedEventArgs) Handles startshowmore.TextChanged
        If favbarstack.ActualWidth > FavoritesBar.ActualWidth Then
            FavMore.Background = Adressbar.Background
            FavMore.Visibility = Visibility.Visible
            AddHandler FavMore.PointerEntered, New PointerEventHandler(Function(machin As Object, truc As PointerRoutedEventArgs)
                                                                           FavMore.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(100, 128, 128, 128))
                                                                       End Function)

            AddHandler FavMore.PointerExited, New PointerEventHandler(Function(machin As Object, truc As PointerRoutedEventArgs)

                                                                          FavMore.Background = Adressbar.Background
                                                                      End Function)


            Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

            If localSettings.Values("DarkThemeEnabled") = True Then
                FavMore.RequestedTheme = ElementTheme.Dark
            Else
                FavMore.RequestedTheme = ElementTheme.Light
            End If
        Else
            FavMore.Visibility = Visibility.Collapsed
        End If
    End Sub
End Class
#End Region
