Imports Windows.UI.Notifications
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
Public NotInheritable Class KidsMode
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
    Dim NotCompleted As Boolean

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
        If web.CanGoBack And MemoPanel.Visibility = Visibility.Collapsed And Ellipsis.Visibility = Visibility.Collapsed And AddFavView.Visibility = Visibility.Collapsed Then
            web.GoBack()
        ElseIf MemoPanel.Visibility = Visibility.Visible And Ellipsis.Visibility = Visibility.Collapsed And AddFavView.Visibility = Visibility.Collapsed Then
            MemoPopOut.Begin()
        ElseIf Ellipsis.Visibility = Visibility.Visible Then
            Ellipsis_Close.Begin()
        ElseIf AddFavView.Visibility = Visibility.Visible And Ellipsis.Visibility = Visibility.Collapsed Then
            AddFav_PopUp_Close.Begin()
        End If
    End Sub
#End Region
#Region "Page Loaded"
    Private Async Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings 'Permet l'accés aux paramètres
        Await ShowFavorites2()


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


        History_SearchMode = False
        favs_tagsearch = False

        'Animation d'ouverture de Blueflap
        EnterAnim.Begin()
        Go_Home()
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
        ShowCache.Begin()
        Loading2.Begin()
        Danger.Stop()
        MobileProgressFinish.Stop()
        MobileProgressBegin.Begin()
        PCProgressFinish.Stop()
        PCProgressBegin.Begin()
        Favicon.Visibility = Visibility.Collapsed
        BackForward()
        RefreshEnabled.Stop()
        StopEnabled.Begin()
        MobileRefreshIcon.Text = ""
        NotCompleted = True
        'MobileRefreshLabel.Text = resourceLoader.GetString("Menu_Stop/Text")
        MobileRefreshLabel.Text = Label_Stop.Text

    End Sub

    Private Async Sub web_NavigationCompleted(sender As WebView, args As WebViewNavigationCompletedEventArgs) Handles web.NavigationCompleted
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        'Navigation terminée
        AdressBox.Text = web.Source.ToString
        Phone_URL.Text = web.Source.Host
        Titlebox.Text = web.DocumentTitle
        loader.IsActive = False
        Loading2.Stop()
        MobileProgressFinish.Begin()
        PCProgressFinish.Begin()
        NotCompleted = False

        BackForward()

        If web.Source.HostNameType = UriHostNameType.Dns And Not localSettings.Values("Favicon") = False Then
            Favicon.Source = New BitmapImage(New Uri("http://" & web.Source.Host & "/favicon.ico", UriKind.Absolute))
            Favicon.Visibility = Visibility.Visible
        End If



        ' Ajout de la page à l'historique

        Dim CurrentTitle As String = web.DocumentTitle
        Dim VisitDate As DateTime = DateTime.Now
        If Not web.Source.ToString = "about:blank" Then
            Try
                Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("KidsHistory"))
                Dim HistoryElem As JsonObject = New JsonObject
                HistoryElem.Add("url", JsonValue.CreateStringValue(web.Source.ToString))
                HistoryElem.Add("title", JsonValue.CreateStringValue(web.DocumentTitle))
                HistoryElem.Add("date", JsonValue.CreateNumberValue(DateTime.Now.ToBinary))
                root.Add(HistoryElem)
                WriteJsonFile(root, "KidsHistory")
            Catch
                WriteJsonFile(JsonArray.Parse("[]"), "KidsHistory")
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

        If Not web.Source.ToString = "about:blank" Then
            Try
                localSettings.Values("StatKid") = localSettings.Values("StatKid") + 1
            Catch
                localSettings.Values("StatKid") = 1
            End Try
        End If


        CheckPage()


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
        Phone_URL.Text = web.Source.Host

        Titlebox.Text = web.DocumentTitle
        loader.IsActive = False
        Loading2.Stop()
        MobileProgressFinish.Begin()
        PCProgressFinish.Begin()

        BackForward()

        If web.Source.HostNameType = UriHostNameType.Dns And Not localSettings.Values("Favicon") = False Then
            Favicon.Source = New BitmapImage(New Uri("http://" & web.Source.Host & "/favicon.ico", UriKind.Absolute))
            Favicon.Visibility = Visibility.Visible
        End If


    End Sub
    Private Sub Go_Home()
        'Clic sur le bouton home
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings




        'Vérification de l'existence d'une page d'accueil valide
        Try
                web.Navigate(New Uri("https://www.qwantjunior.com"))

            Catch

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
        Try
            AdressBox.Text = web.Source.ToString
            Phone_URL.Text = web.Source.Host
        Catch
        End Try

        Titlebox.Text = web.DocumentTitle
        loader.IsActive = False
        Loading2.Stop()
        MobileProgressFinish.Begin()
        PCProgressFinish.Begin()
        BackForward()
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
            If (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) Then
                webcontainer.Margin = New Thickness(0, 0, 0, 0)
                Enterfullscreen_Mobile.Begin()
            Else
            End If


        Else
            Dim appView = ApplicationView.GetForCurrentView
            appView.ExitFullScreenMode()
            If (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) Then
                Enterfullscreen_Mobile.Stop()
                webcontainer.Margin = New Thickness(0, 0, 0, 50)
            Else
            End If

        End If
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


    Private Async Sub PageChanged()
        Cache.Visibility = Visibility.Visible
        ShowCache.Begin()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        'Navigation terminée
        AdressBox.Text = web.Source.ToString
        Phone_URL.Text = web.Source.Host

        Titlebox.Text = web.DocumentTitle

        'Met à jour les stats dispos dans les paramètres de Blueflap



        BackForward()


        If web.Source.HostNameType = UriHostNameType.Dns And Not localSettings.Values("Favicon") = False Then
            Favicon.Source = New BitmapImage(New Uri("http://" & web.Source.Host & "/favicon.ico", UriKind.Absolute))
            Favicon.Visibility = Visibility.Visible
        End If


        ' Ajout de la page à l'historique

        Dim CurrentTitle As String = web.DocumentTitle
        Dim VisitDate As DateTime = DateTime.Now


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
        If Not web.Source.ToString = "about:blank" Then
            Try
                localSettings.Values("StatKid") = localSettings.Values("StatKid") + 1
            Catch
                localSettings.Values("StatKid") = 1
            End Try
        End If
        CheckPage()
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

    Private Async Sub Button_Tapped_1(sender As Object, e As TappedRoutedEventArgs)
        Await Windows.System.Launcher.LaunchUriAsync(New Uri(("ms-windows-store://pdp/?ProductId=9nblggh316xh")))
    End Sub

    Private Sub ChangSearchEngine(sender As Object, e As TappedRoutedEventArgs)

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
        Catch
            WriteJsonFile(JsonArray.Parse("[]"), "Favorites")
        End Try
    End Function
    Private Async Sub AddToFavorite_Ask()
        If Not Windows.Storage.ApplicationData.Current.LocalSettings.Values("Fav_Confirmation") = False Then
            Try
                Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("Favorites"))

                If root.Any(Function(x As JsonValue) x.GetObject.GetNamedString("url") = web.Source.ToString) Then
                    RightMenuPivot.SelectedIndex = 1
                    MemoPopOut.Stop()
                    MemoPopIN.Begin()
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
                    Json = Await ReadJsonFile("KidsHistory")
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
            Json = Await ReadJsonFile("KidsHistory")
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
                                                                             Dim root As JsonArray = JsonArray.Parse(Await ReadJsonFile("KidsHistory"))
                                                                             root.Remove(root.First(Function(x) x.GetObject.GetNamedString("url") = histElem.GetObject.GetNamedString("url")))
                                                                             WriteJsonFile(root, "KidsHistory")
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


    Private Sub Lock_Button_M_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Lock_Button_M.Tapped
        'Ellipsis_Close.Begin()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        LockTheBrowser.IsChecked = True
        Me.Frame.Navigate(GetType(Verrouillage))
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

    Private Sub Like_Button_M_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Like_Button_M.Tapped
        Ellipsis_Close.Begin()
        AddToFavorite_Ask()
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
        AdressBox.Text = web.Source.ToString
        Phone_URL.Text = web.Source.Host
        Titlebox.Text = web.DocumentTitle
        loader.IsActive = False
        Loading2.Stop()
        MobileProgressFinish.Begin()
        PCProgressFinish.Begin()
        BackForward()
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
#Region "Kids"
    Private Async Sub CheckPage()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        If Not web.Source.ToString = "about:blank" Then
            Cache.Visibility = Visibility.Visible
            ShowCache.Begin()
            Dim Allow As Boolean


            If localSettings.Values("ProtectionLevel") = 0 Then
                Allow = True

            ElseIf localSettings.Values("ProtectionLevel") = 1 Then
                Allow = True

                Try
                    Dim Json As String
                    Try
                        Json = Await ReadJsonFile("KidsMode_ForbiddenLinks")
                    Catch ex As Exception
                        Json = "[]"
                    End Try
                    For Each KidsElem In JsonArray.Parse(Json).Reverse
                        Dim url As String
                        url = KidsElem.GetObject.GetNamedString("word").ToLower
                        url = url.Replace("http://", "")
                        url = url.Replace("https://", "")

                        If web.Source.ToString.ToLower.Contains(url) Then
                            Allow = False
                        End If
                    Next
                Catch ex As Exception
                End Try


                Try
                    Dim Json As String
                    Try
                        Json = Await ReadJsonFile("KidsMode_ForbiddenWords")
                    Catch ex As Exception
                        Json = "[]"
                    End Try
                    For Each KidsElem In JsonArray.Parse(Json).Reverse

                        If web.DocumentTitle.ToLower.Contains(KidsElem.GetObject.GetNamedString("word").ToLower) Then
                            Allow = False
                        End If
                    Next
                Catch ex As Exception
                End Try








            ElseIf localSettings.Values("ProtectionLevel") = 2 Then
                Allow = False

                Try
                    Dim Json As String

                    Try
                        Json = Await ReadJsonFile("KidsMode_AllowedLinks")
                    Catch ex As Exception
                        Json = "[]"
                    End Try

                    For Each KidsElem In JsonArray.Parse(Json).Reverse
                        Dim url As String
                        url = KidsElem.GetObject.GetNamedString("word").ToLower
                        url = url.Replace("http://", "")
                        url = url.Replace("https://", "")

                        If web.Source.ToString.ToLower.Contains(url) Then
                            Allow = True
                        End If


                    Next
                Catch ex As Exception
                End Try

                Try
                    Dim Json As String
                    Try
                        Json = Await ReadJsonFile("KidsMode_ForbiddenLinks")
                    Catch ex As Exception
                        Json = "[]"
                    End Try
                    For Each KidsElem In JsonArray.Parse(Json).Reverse
                        Dim url As String
                        url = KidsElem.GetObject.GetNamedString("word").ToLower
                        url = url.Replace("http://", "")
                        url = url.Replace("https://", "")

                        If web.Source.ToString.ToLower.Contains(url) Then
                            Allow = False
                        End If
                    Next
                Catch ex As Exception
                End Try


                Try
                    Dim Json As String
                    Try
                        Json = Await ReadJsonFile("KidsMode_ForbiddenWords")
                    Catch ex As Exception
                        Json = "[]"
                    End Try
                    For Each KidsElem In JsonArray.Parse(Json).Reverse

                        If web.DocumentTitle.ToLower.Contains(KidsElem.GetObject.GetNamedString("word").ToLower) Then
                            Allow = False
                        End If
                    Next
                Catch ex As Exception
                End Try







            ElseIf localSettings.Values("ProtectionLevel") = 3 Then

                Allow = False

                Try
                    Dim Json As String

                    Try
                        Json = Await ReadJsonFile("KidsMode_AllowedLinks")
                    Catch ex As Exception
                        Json = "[]"
                    End Try

                    For Each KidsElem In JsonArray.Parse(Json).Reverse
                        Dim url As String
                        url = KidsElem.GetObject.GetNamedString("word").ToLower
                        url = url.Replace("http://", "")
                        url = url.Replace("https://", "")

                        If web.Source.ToString.ToLower.Contains(url) Then
                            Allow = True
                        End If


                    Next
                Catch ex As Exception
                End Try

                Try
                    Dim Json As String
                    Try
                        Json = Await ReadJsonFile("KidsMode_ForbiddenLinks")
                    Catch ex As Exception
                        Json = "[]"
                    End Try
                    For Each KidsElem In JsonArray.Parse(Json).Reverse
                        Dim url As String
                        url = KidsElem.GetObject.GetNamedString("word").ToLower
                        url = url.Replace("http://", "")
                        url = url.Replace("https://", "")

                        If web.Source.ToString.ToLower.Contains(url) Then
                            Allow = False
                        End If
                    Next
                Catch ex As Exception
                End Try

                Try
                    Dim Json As String
                    Try
                        Json = Await ReadJsonFile("KidsMode_ForbiddenWords")
                    Catch ex As Exception
                        Json = "[]"
                    End Try
                    For Each KidsElem In JsonArray.Parse(Json).Reverse
                        If web.DocumentTitle.ToLower.Contains(KidsElem.GetObject.GetNamedString("word").ToLower) Then
                            Allow = False
                        End If
                    Next
                Catch ex As Exception
                End Try

                If NotCompleted = False Then
                    Try
                        Dim Json As String
                        Try
                            Json = Await ReadJsonFile("KidsMode_Words")
                        Catch ex As Exception
                            Json = "[]"
                        End Try
                        For Each KidsElem In JsonArray.Parse(Json).Reverse
                            If web.DocumentTitle.ToLower.Contains(KidsElem.GetObject.GetNamedString("word").ToLower) Then
                                Allow = True
                            End If
                        Next
                    Catch ex As Exception
                    End Try

                End If
            End If




            If localSettings.Values("ProtectionLevel") = 0 Then
                Allow = True
            End If
            If Allow = True Then
                ShowCache.Stop()

            Else
                web.Source = New Uri("about:blank")
                ShowCache.Begin()
                Loading2.Stop()
                Danger.Begin()
            End If
        End If
    End Sub

    Private Sub BackToMain_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles BackToMain_Button.Tapped
        Frame.GoBack()
    End Sub
#End Region

    Private Async Function ShowFavorites2() As Task

        Try
            Springboard.Children.Clear()

            For Each favsElem In JsonArray.Parse(Await ReadJsonFile("Favorites")).Reverse

                Dim elemContainer As Grid = New Grid
                elemContainer.Margin = New Thickness(0, 0, 0, 0)
                elemContainer.Height = 80
                elemContainer.Width = 80
                elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255))


                Dim image As Image = New Image
                image.Width = Double.NaN
                image.Opacity = 0.5
                image.Height = Double.NaN
                image.HorizontalAlignment = HorizontalAlignment.Stretch
                image.HorizontalAlignment = HorizontalAlignment.Stretch

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
                elemContainer.Children.Add(image)

                Dim image1 As Image = New Image
                image1.Width = Double.NaN
                image1.Height = Double.NaN
                image1.HorizontalAlignment = HorizontalAlignment.Stretch
                image1.HorizontalAlignment = HorizontalAlignment.Stretch

                Try

                    Dim Black1 As BitmapImage = New BitmapImage
                    Black1.UriSource = New Uri("ms-appx:/Assets/kidsicon.png", UriKind.Absolute)
                    image1.Source = Black1
                Catch ex As Exception

                End Try
                elemContainer.Children.Add(image1)

                Dim image2 As Image = New Image
                image2.Width = 23
                image2.Height = 23
                image2.HorizontalAlignment = HorizontalAlignment.Center
                image2.HorizontalAlignment = HorizontalAlignment.Center

                Try
                    Dim str As Uri = New Uri(favsElem.GetObject.GetNamedString("url"))

                    Dim request As Net.HttpWebRequest = DirectCast(Net.HttpWebRequest.Create("http://" & str.Host & "/favicon.ico"), Net.HttpWebRequest)
                    request.Method = "HEAD"
                    Try
                        Await request.GetResponseAsync()
                        image2.Source = New BitmapImage(New Uri("http://" & str.Host & "/favicon.ico", UriKind.Absolute))
                    Catch
                        Dim Black1 As BitmapImage = New BitmapImage
                        Black1.UriSource = New Uri("ms-appx:/Assets/faviconNeutral.png", UriKind.Absolute)
                        image2.Source = Black1

                    End Try

                Catch ex As Exception

                End Try
                elemContainer.Children.Add(image2)

                AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                            Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
                                                                            web.Source = New Uri(favsElem.GetObject.GetNamedString("url"))
                                                                            grid2.Visibility = Visibility.Collapsed
                                                                        End Function)

                If PhoneNavBar.Visibility = Visibility.Collapsed Then
                    AddHandler elemContainer.PointerEntered, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                         elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 100, 80, 200))
                                                                                     End Function)

                    AddHandler elemContainer.PointerExited, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                        elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255))
                                                                                    End Function)
                End If
                Springboard.Children.Add(elemContainer)
            Next
        Catch
        End Try

    End Function

    Private Sub Lock_Button_Copy_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Lock_Button_Copy.Tapped
        If grid2.Visibility = Visibility.Collapsed Then
            grid2.Visibility = Visibility.Visible


        Else
            grid2.Visibility = Visibility.Collapsed
        End If
    End Sub
End Class
