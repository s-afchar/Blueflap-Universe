Imports Windows.UI.Notifications
Imports Windows.ApplicationModel.DataTransfer
Imports Windows.UI.Xaml.Controls
''' <summary>
''' Page dédiée à la navigation web
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page
    Public Sub New()
        Me.InitializeComponent()
        AddHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf MainPage_BackRequested
    End Sub
    Private Sub MainPage_BackRequested(sender As Object, e As Windows.UI.Core.BackRequestedEventArgs)
        'Appui sur la touche retour "physique" d'un appareil Windows
        If Not Frame.CanGoBack Then
            If web.CanGoBack Then
                e.Handled = True
                web.GoBack()
            End If
        End If
    End Sub
    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)

        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings 'Permet l'accés aux paramètres

        If Notif_Home.Visibility = Visibility.Visible Then
            localSettings.Values("Organise_notifs") = 0 'Initialisation de la valeur qui définit le positionnement des éléments dans les paramètres
        End If

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
        FirstLaunch()

        'Vérification de la page d'accueil
        If AdressBox.Text = "about:blank" Then
            Try
                web.Navigate(New Uri(localSettings.Values("Homepage")))

                'Met à jour les éléments du centre de messages systèmes
                If Notif_HomePageError.Visibility = Visibility.Visible Then
                    Notif_HomePageError.Visibility = Visibility.Collapsed
                    New_Notif.Stop()
                    If Notif_SearchEngineError.Visibility = Visibility.Visible Then
                        If Notif_SearchEngineError.Margin.Top > Notif_HomePageError.Margin.Top Then
                            Notif_SearchEngineError.Margin = New Thickness(0, Notif_SearchEngineError.Margin.Top - 110, 0, 0)
                            localSettings.Values("Organise_notifs") = localSettings.Values("Organise_notifs") - 110
                        End If
                    End If
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
                    New_Notif.Begin()
                    Notifications_Counter.Text = Notifications_Counter.Text + 1
                    Notif_HomePageError.Margin = New Thickness(0, localSettings.Values("Organise_notifs"), 0, 0)
                    localSettings.Values("Organise_notifs") = localSettings.Values("Organise_notifs") + 110
                    Notif_Home.Visibility = Visibility.Collapsed
                End If

        End Try

        End If

        'Définition du thème avec couleur personnalisée
        Try
            If localSettings.Values("CustomColorEnabled") = True Then
                LeftMenu.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(localSettings.Values("CustomColorD"), localSettings.Values("CustomColorA"), localSettings.Values("CustomColorB"), localSettings.Values("CustomColorC")))
            Else
                LeftMenu.Background = DefaultThemeColor.Background
            End If
        Catch
        End Try

        'Animation d'ouverture de Blueflap
        EnterAnim.Begin()
    End Sub

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

        If web.CanGoBack And web.CanGoForward Then
            Back_Button.Visibility = Visibility.Visible
            Forward_Button.Visibility = Visibility.Visible
            Fight_Button.Margin = New Thickness(3, localSettings.Values("FightBut_Top"), -1, 0)
            Lock_Button.Margin = New Thickness(3, localSettings.Values("lockBut_Top"), -1, 0)
            Memo_Button.Margin = New Thickness(3, localSettings.Values("memoBut_Top"), -1, 0)
            Share_Button.Margin = New Thickness(3, localSettings.Values("ShareBut_Top"), -1, 0)
            Forward_Button.Margin = New Thickness(3, 198, -1, 0)

        ElseIf web.CanGoBack And web.CanGoForward = False Then
            Back_Button.Visibility = Visibility.Visible
            Forward_Button.Visibility = Visibility.Collapsed
            Fight_Button.Margin = New Thickness(3, localSettings.Values("FightBut_Top") - 44, -1, 0)
            Lock_Button.Margin = New Thickness(3, localSettings.Values("lockBut_Top") - 44, -1, 0)
            Memo_Button.Margin = New Thickness(3, localSettings.Values("memoBut_Top") - 44, -1, 0)
            Share_Button.Margin = New Thickness(3, localSettings.Values("ShareBut_Top") - 44, -1, 0)

        ElseIf web.CanGoBack = False And web.CanGoForward Then
            Back_Button.Visibility = Visibility.Collapsed
            Forward_Button.Visibility = Visibility.Visible
            Fight_Button.Margin = New Thickness(3, localSettings.Values("FightBut_Top") - 44, -1, 0)
            Lock_Button.Margin = New Thickness(3, localSettings.Values("lockBut_Top") - 44, -1, 0)
            Memo_Button.Margin = New Thickness(3, localSettings.Values("memoBut_Top") - 44, -1, 0)
            Share_Button.Margin = New Thickness(3, localSettings.Values("ShareBut_Top") - 44, -1, 0)
            Forward_Button.Margin = New Thickness(3, 154, -1, 0)

        ElseIf web.CanGoBack = False And web.CanGoForward = False Then
            Back_Button.Visibility = Visibility.Collapsed
            Forward_Button.Visibility = Visibility.Collapsed
            Fight_Button.Margin = New Thickness(3, localSettings.Values("FightBut_Top") - 88, -1, 0)
            Lock_Button.Margin = New Thickness(3, localSettings.Values("lockBut_Top") - 88, -1, 0)
            Memo_Button.Margin = New Thickness(3, localSettings.Values("memoBut_Top") - 88, -1, 0)
            Share_Button.Margin = New Thickness(3, localSettings.Values("ShareBut_Top") - 88, -1, 0)
        End If

        AdressBox.IsEnabled = False 'Autre solution provisoire (qui va sans doute rester) parce que sinon l'adressbox obtient le focus à l'ouverture allez savoir pourquoi...
        AdressBox.IsEnabled = True

    End Sub

    Private Sub FirstLaunch() 'Définit les paramètres par défaut
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        If Not localSettings.Values("Config") = True Then
            localSettings.Values("A1") = "http://www.qwant.com/?q="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 1
        End If

    End Sub

    Private Sub web_NavigationStarting(sender As WebView, args As WebViewNavigationStartingEventArgs) Handles web.NavigationStarting
        loader.IsActive = True 'Les petites billes de chargement apparaissent quand une page se charge
        BackForward()
        RefreshEnabled.Stop()
        StopEnabled.Begin()
    End Sub

    Private Sub web_NavigationCompleted(sender As WebView, args As WebViewNavigationCompletedEventArgs) Handles web.NavigationCompleted
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        'Navigation terminée
        AdressBox.Text = web.Source.ToString
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

    End Sub

    Private Sub web_LoadCompleted(sender As Object, e As NavigationEventArgs) Handles web.LoadCompleted
        'Page chargée

        ' Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        ' Try
        'If localSettings.Values("Adblock") = "En fonction" Then
        '       web.Navigate(New Uri(localSettings.Values("AdblockFonction")))
        '  End If
        ' Catch
        ' localSettings.Values("Adblock") = "Désactivé"
        ' End Try

        AdressBox.Text = web.Source.ToString
        Titlebox.Text = web.DocumentTitle
        loader.IsActive = False
        SourceCode.Text = "..."
        BackForward()
    End Sub

    Private Sub Home_button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Home_button.Tapped
        'Clic sur le bouton home
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        'Vérification de l'existence d'une page d'accueil valide
        Try
            web.Navigate(New Uri(localSettings.Values("Homepage")))

            'Met à jour les éléments du centre de messages systèmes
            If Notif_HomePageError.Visibility = Visibility.Visible Then
                Notif_HomePageError.Visibility = Visibility.Collapsed
                New_Notif.Stop()
                If Notif_SearchEngineError.Visibility = Visibility.Visible Then
                    If Notif_SearchEngineError.Margin.Top > Notif_HomePageError.Margin.Top Then
                        Notif_SearchEngineError.Margin = New Thickness(0, Notif_SearchEngineError.Margin.Top - 110, 0, 0)
                        localSettings.Values("Organise_notifs") = localSettings.Values("Organise_notifs") - 110
                    End If
                End If
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
                New_Notif.Begin()
                Notifications_Counter.Text = Notifications_Counter.Text + 1
                Notif_HomePageError.Margin = New Thickness(0, localSettings.Values("Organise_notifs"), 0, 0)
                localSettings.Values("Organise_notifs") = localSettings.Values("Organise_notifs") + 110
                Notif_Home.Visibility = Visibility.Collapsed
            End If
        End Try
    End Sub

    Private Sub AdressBox_KeyDown(sender As Object, e As KeyRoutedEventArgs) Handles AdressBox.KeyDown
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        If (e.Key = Windows.System.VirtualKey.Enter) Then  'Permet de réagir à l'appui sur la touche entrée
            Dim textArray = AdressBox.Text.Split(" ")

            'Détermine si il s'agit d'une URL ou d'une recherche

            If (AdressBox.Text.Contains(".") = True And AdressBox.Text.Contains(" ") = False And AdressBox.Text.Contains(" .") = False And AdressBox.Text.Contains(". ") = False) Or textArray(0).Contains(":/") = True Or textArray(0).Contains(":\") Then
                If AdressBox.Text.Contains("http://") OrElse AdressBox.Text.Contains("https://") Then  'URL invalide si pas de http://
                    web.Navigate(New Uri(AdressBox.Text))
                Else
                    web.Navigate(New Uri("http://" + AdressBox.Text))
                End If

            Else
                Try 'On teste si le moteur de recherche défini par l'utilisateur est valide
                    Dim Rech As String
                    Rech = AdressBox.Text
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
                        If Notif_HomePageError.Visibility = Visibility.Visible Then
                            If Notif_HomePageError.Margin.Top.ToString > Notif_SearchEngineError.Margin.Top.ToString Then
                                Notif_HomePageError.Margin = New Thickness(0, Notif_HomePageError.Margin.Top - 110, 0, 0)
                                localSettings.Values("Organise_notifs") = localSettings.Values("Organise_notifs") - 110
                            End If
                        End If
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
                        New_Notif.Begin()
                        Notifications_Counter.Text = Notifications_Counter.Text + 1
                        Notif_SearchEngineError.Margin = New Thickness(0, localSettings.Values("Organise_notifs"), 0, 0)
                        localSettings.Values("Organise_notifs") = localSettings.Values("Organise_notifs") + 110
                        Notif_Home.Visibility = Visibility.Collapsed
                    End If
                End Try
            End If

            AdressBox.IsEnabled = False 'PROVISOIRE : Faire perdre le focus à la textbox
            AdressBox.IsEnabled = True
        End If
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


    Private Sub Paramètres_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Paramètres_Button.Tapped
        Me.Frame.Navigate(GetType(Parametres)) 'Aller sur la page "Paramètres"
    End Sub
    Private Sub OnNewWindowRequested(sender As WebView, e As WebViewNewWindowRequestedEventArgs) Handles web.NewWindowRequested
        'Force l'ouverture dans Blueflap de liens censés s'ouvrir dans une nouvelle fenêtre
        web.Navigate(e.Uri)
        e.Handled = True
    End Sub
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
    End Sub
    Private Sub Notifications_indicator_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Notifications_indicator.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        OpenRightMenu()
        RightMenuPivot.SelectedIndex = 3
        New_Notif.Stop()
        Notifications_Counter.Text = "0"
        If Notif_HomePageError.Visibility = Visibility.Collapsed And Notif_SearchEngineError.Visibility = Visibility.Collapsed Then
            Notif_Home.Visibility = Visibility.Visible
            localSettings.Values("Organise_notifs") = 0
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

    Private Sub Web_Share_Save_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Web_Share_Save.Tapped

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
End Class
