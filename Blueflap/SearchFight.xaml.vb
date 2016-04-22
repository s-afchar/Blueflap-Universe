' Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238
Imports Windows.UI.Notifications
Imports Windows.ApplicationModel.DataTransfer
Imports Windows.UI.Xaml.Controls
''' <summary>
''' Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
''' </summary>
Public NotInheritable Class SearchFight
    Inherits Page
    Dim PositionNumber As String
    Dim PositionCurseur As String
    Dim MousePress As Boolean
    Dim resourceLoader = New Resources.ResourceLoader()
#Region "Page Loaded"
    Private Sub Page_Loaded_1(sender As Object, e As RoutedEventArgs) Handles Page.Loaded
        AddHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf MainPage_BackRequested

        'Masque tout ce qui doit être masqué
        PositionNumber = 1
        AncrageShow.Stop()
        AncrageHide.Begin()
        AncreDroit.Stop()
        AncreGauche.Stop()
        Right_HideMenu.Begin()
        Left_HideMenu.Begin()
        RightMenu.Visibility = Visibility.Collapsed
        LeftMenu.Visibility = Visibility.Collapsed

        'Là on a tout ce qui concerne l'initialisation de Blueflap : thème, valeurs des combobox, animation d'arrivée...
        OpenAnim.Begin()
        W1.Width = (Page.ActualWidth / 2) - 2
        W2.Width = (Page.ActualWidth / 2) - 2
        If PhoneNavBar.Visibility = Visibility.Visible Then
            If Page.ActualWidth > Page.ActualHeight Then
                W1.Width = (Page.ActualWidth / 2) - 2
                W2.Width = (Page.ActualWidth / 2) - 2
                W1.Height = Double.NaN
                W2.Height = Double.NaN
            Else
                W1.Height = (WebContainer.ActualHeight / 2) - 2
                W2.Height = (WebContainer.ActualHeight / 2) - 2
                W1.VerticalAlignment = VerticalAlignment.Top
                W2.VerticalAlignment = VerticalAlignment.Bottom
                W1.HorizontalAlignment = HorizontalAlignment.Stretch
                W2.HorizontalAlignment = HorizontalAlignment.Stretch
                W1.Width = Double.NaN
                W2.Width = Double.NaN
            End If
        End If
        W1.Navigate(New Uri("about:blank"))
        W2.Navigate(New Uri("about:blank"))

        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        Try
            DerniereRecherche.Text = localSettings.Values("textboxe").ToString.ToUpper
            DerniereRecherche1.Text = localSettings.Values("textboxe").ToString.ToUpper
        Catch
        End Try

        If localSettings.Values("DarkThemeEnabled") = True Then 'Theme Sombre

            Fightbar.RequestedTheme = ElementTheme.Dark
            Fightbar.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 27, 27, 27))
            Window_Title.Opacity = 100
            Window_Title.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 187, 187, 187))

            Dim titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView.TitleBar
            Dim v = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView()
            v.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 27, 27, 27)
            v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.White
            v.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 27, 27, 27)
            SelectionPopup.RequestedTheme = ElementTheme.Dark
            PhoneNavBar.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 21, 21, 21))
            PhoneNavBar.BorderBrush = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 70, 70, 70))
            PhoneNavBar.RequestedTheme = ElementTheme.Dark
        Else 'Theme Clair

            Fightbar.RequestedTheme = ElementTheme.Light
            SelectionPopup.RequestedTheme = ElementTheme.Light
            Dim titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView.TitleBar
            Dim v = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView()
            v.TitleBar.ButtonBackgroundColor = Windows.UI.Colors.WhiteSmoke
            v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.DarkGray
            v.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.WhiteSmoke
            PhoneNavBar.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255))
            PhoneNavBar.BorderBrush = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 147, 147, 147))
            PhoneNavBar.RequestedTheme = ElementTheme.Light
        End If
        Try
            If localSettings.Values("CustomColorEnabled") = True Then
                button.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(localSettings.Values("CustomColorD"), localSettings.Values("CustomColorA"), localSettings.Values("CustomColorB"), localSettings.Values("CustomColorC")))
                DragEllipse.Fill = New SolidColorBrush(Windows.UI.Color.FromArgb(localSettings.Values("CustomColorD"), localSettings.Values("CustomColorA"), localSettings.Values("CustomColorB"), localSettings.Values("CustomColorC")))
                Fight_Butt.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(localSettings.Values("CustomColorD"), localSettings.Values("CustomColorA"), localSettings.Values("CustomColorB"), localSettings.Values("CustomColorC")))
                rectangle.Fill = New SolidColorBrush(Windows.UI.Color.FromArgb(localSettings.Values("CustomColorD"), localSettings.Values("CustomColorA"), localSettings.Values("CustomColorB"), localSettings.Values("CustomColorC")))
            Else
                button.Background = DefaultThemeColor.Background
                DragEllipse.Fill = DefaultThemeColor.Background
                Fight_Butt.Background = DefaultThemeColor.Background
                rectangle.Fill = DefaultThemeColor.Background
            End If
        Catch
        End Try
        Try
            Drop1.Text = localSettings.Values("F1Text").ToString.ToLower
            Drop2.Text = localSettings.Values("F2Text").ToString.ToLower
            Header1.Text = localSettings.Values("F1Text").ToString.ToUpper
            Header2.Text = localSettings.Values("F2Text").ToString.ToUpper
        Catch
        End Try

        If Page.ActualWidth > 600 OrElse Page.ActualHeight > 600 Then
            List_Right.Width = 300
            List_Left.Width = 300
        Else
            List_Right.Width = Page.ActualWidth / 2
            List_Left.Width = Page.ActualWidth / 2
        End If

        Ellipsis_Button.Background = rectangle.Fill
        If PhoneNavBar.Visibility = Visibility.Visible Then
            AdressBox.IsEnabled = False
            AdressBox.IsEnabled = True
            Resize.Visibility = Visibility.Collapsed
            ShowPopup()
        End If
        Phone_URL.Text = AdressBox.PlaceholderText.ToString
    End Sub
#End Region
#Region "Frame.GoBack"
    Public Sub New()
        Me.InitializeComponent()
        AddHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf MainPage_BackRequested
    End Sub
    Private Sub MainPage_BackRequested(sender As Object, e As Windows.UI.Core.BackRequestedEventArgs)
        'On retourne à la page principale quand le bouton retour "physique" est pressé
        If Selector_Background.Visibility = Visibility.Visible Then
            PopupClose.Begin()
            Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

            localSettings.Values("F1Text") = Header1.Text
            localSettings.Values("F2Text") = Header2.Text
            Drop1.Text = localSettings.Values("F1Text").ToString.ToLowerInvariant
            Drop2.Text = localSettings.Values("F2Text").ToString.ToLowerInvariant

        ElseIf Frame.CanGoBack And Selector_Background.Visibility = Visibility.Collapsed Then
            e.Handled = True
            RemoveHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf MainPage_BackRequested
            Frame.GoBack()
        End If
    End Sub
    Private Sub Button_Tapped(sender As Object, e As TappedRoutedEventArgs)
        'Retour à la page précédente avec appui sur le bouton Back
        If Frame.CanGoBack Then
            RemoveHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf MainPage_BackRequested
            Frame.GoBack()
        End If
    End Sub
#End Region
#Region "Begin a Fight"
    Private Sub Fight_Butt_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Fight_Butt.Tapped
        'Cette fois on lance le combar en appuyant sur un bouton rechercher
        Fight()
    End Sub
    Private Sub Fight()
        'On lance la navigation simultannée des deux WebView

        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("F1Text") = Header1.Text
        localSettings.Values("F2Text") = Header2.Text
        Drop1.Text = localSettings.Values("F1Text").ToString.ToLowerInvariant
        Drop2.Text = localSettings.Values("F2Text").ToString.ToLowerInvariant

        localSettings.Values("textboxe") = AdressBox.Text

        Phone_URL.Text = AdressBox.Text

        If localSettings.Values("F1Text") = "GOOGLE" Then
            W1.Navigate(New Uri("http://www.google.fr/search?q=" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F1Text") = "BING" Then
            W1.Navigate(New Uri("http://www.bing.com/search?q=" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F1Text") = "YAHOO" Then
            W1.Navigate(New Uri("http://fr.search.yahoo.com/search;_ylt=Ai38ykBDWJSAxF25NrTnjXxNhJp4?p=" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F1Text") = "DUCKDUCKGO" Then
            W1.Navigate(New Uri("http://duckduckgo.com/?q=" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F1Text") = "QWANT WEB" Then
            W1.Navigate(New Uri("http://www.qwant.com/?q=" + localSettings.Values("textboxe") + "&t=web"))

        ElseIf localSettings.Values("F1Text") = "QWANT" Then
            W1.Navigate(New Uri("http://www.qwant.com/?q=" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F1Text") = "ASK" Then
            W1.Navigate(New Uri("http://fr.ask.com/web?q=" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F1Text") = "YOUTUBE" Then
            W1.Navigate(New Uri("http://www.youtube.com/results?search_query=" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F1Text") = "DAILYMOTION" Then
            W1.Navigate(New Uri("http://www.dailymotion.com/fr/relevance/universal/search/" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F1Text") = "VIMEO" Then
            W1.Navigate(New Uri("http://vimeo.com/search?q=" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F1Text") = "WIKIPEDIA" Then
            W1.Navigate(New Uri("http://fr.wikipedia.org/w/index.php?search=" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F1Text") = "TWITTER" Then
            W1.Navigate(New Uri("http://twitter.com/search?q=" + localSettings.Values("textboxe")))
        End If



        If localSettings.Values("F2Text") = "GOOGLE" Then
            W2.Navigate(New Uri("http://www.google.fr/search?q=" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F2Text") = "BING" Then
            W2.Navigate(New Uri("http://www.bing.com/search?q=" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F2Text") = "YAHOO" Then
            W2.Navigate(New Uri("http://fr.search.yahoo.com/search;_ylt=Ai38ykBDWJSAxF25NrTnjXxNhJp4?p=" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F2Text") = "DUCKDUCKGO" Then
            W2.Navigate(New Uri("http://duckduckgo.com/?q=" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F2Text") = "QWANT WEB" Then
            W2.Navigate(New Uri("http://www.qwant.com/?q=" + localSettings.Values("textboxe") + "&t=web"))

        ElseIf localSettings.Values("F2Text") = "QWANT" Then
            W2.Navigate(New Uri("http://www.qwant.com/?q=" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F2Text") = "ASK" Then
            W2.Navigate(New Uri("http://fr.ask.com/web?q=" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F2Text") = "YOUTUBE" Then
            W2.Navigate(New Uri("http://www.youtube.com/results?search_query=" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F2Text") = "DAILYMOTION" Then
            W2.Navigate(New Uri("http://www.dailymotion.com/fr/relevance/universal/search/" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F2Text") = "VIMEO" Then
            W2.Navigate(New Uri("http://vimeo.com/search?q=" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F2Text") = "WIKIPEDIA" Then
            W2.Navigate(New Uri("http://fr.wikipedia.org/w/index.php?search=" + localSettings.Values("textboxe")))

        ElseIf localSettings.Values("F2Text") = "TWITTER" Then
            W2.Navigate(New Uri("http://twitter.com/search?q=" + localSettings.Values("textboxe")))
        End If

        'Incrémentation des statistiques concernant SearchFight dans les paramètres de Blueflap
        Try
            localSettings.Values("Stat2") = localSettings.Values("Stat2") + 1
        Catch
            localSettings.Values("Stat2") = 1
        End Try

        'Affichage des petites billes de chargement
        localSettings.Values("Loaded1") = "Chargement"
        localSettings.Values("Loaded2") = "Chargement"
        load.IsActive = True
        StartLoading.Begin()

        MenuBut.Visibility = Visibility.Visible
    End Sub
    Private Sub AdressBox_KeyDown(sender As Object, e As KeyRoutedEventArgs) Handles AdressBox.KeyDown
        'Lance un combat en appuyant sur la touche ENTREE dans la textbox
        If (e.Key = Windows.System.VirtualKey.Enter) Then
            Fight()
        End If
    End Sub

    Private Sub TextBox_KeyDown(sender As Object, e As KeyRoutedEventArgs)
        'Lance un combat en appuyant sur la touche ENTREE dans la textbox
        AdressBox.Text = Popup_Adressbox.Text
        If (e.Key = Windows.System.VirtualKey.Enter) Then
            Fight()
            PopupClose.Begin()

        End If
    End Sub

    Private Sub Popup_Adressbox_TextChanged(sender As Object, e As TextChangedEventArgs) Handles Popup_Adressbox.TextChanged

        AdressBox.Text = Popup_Adressbox.Text
    End Sub

    Private Sub Button_Tapped_1(sender As Object, e As TappedRoutedEventArgs)
        AdressBox.Text = Popup_Adressbox.Text
        Fight()
        PopupClose.Begin()
    End Sub
#End Region
#Region "Both pages loaded"
    Private Sub W1_NavigationCompleted(sender As WebView, args As WebViewNavigationCompletedEventArgs) Handles W1.NavigationCompleted
        'On affiche ou non les petites billes de chargement
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("Loaded1") = "Chargé"

        'Lorsque les deux webviews sont chargées
        If localSettings.Values("Loaded2") = "Chargé" Then
            If load.IsActive = True Then
                If PhoneNavBar.Visibility = Visibility.Visible Then
                    RightMenu.Visibility = Visibility.Collapsed
                    LeftMenu.Visibility = Visibility.Collapsed
                Else
                    If localSettings.Values("SearchFight_Menu") = False Then
                        RightMenu.Visibility = Visibility.Collapsed
                        LeftMenu.Visibility = Visibility.Collapsed
                    Else
                        RightMenu.Visibility = Visibility.Visible
                        LeftMenu.Visibility = Visibility.Visible
                    End If
                End If
            End If
            load.IsActive = False
            StartLoading.Stop()
        End If
    End Sub

    Private Sub W2_NavigationCompleted(sender As WebView, args As WebViewNavigationCompletedEventArgs) Handles W2.NavigationCompleted
        'On affiche ou non les petites billes de chargement
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("Loaded2") = "Chargé"

        'Lorsque les deux webviews sont chargées
        If localSettings.Values("Loaded1") = "Chargé" Then
            If load.IsActive = True Then
                If PhoneNavBar.Visibility = Visibility.Visible Then
                    RightMenu.Visibility = Visibility.Collapsed
                    LeftMenu.Visibility = Visibility.Collapsed
                Else
                    If localSettings.Values("SearchFight_Menu") = False Then
                        RightMenu.Visibility = Visibility.Collapsed
                        LeftMenu.Visibility = Visibility.Collapsed
                    Else
                        RightMenu.Visibility = Visibility.Visible
                        LeftMenu.Visibility = Visibility.Visible
                    End If
                End If
            End If
            load.IsActive = False
            StartLoading.Stop()
        End If
    End Sub
#End Region
#Region "Resize the split view"
    Private Sub Page_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        'Pour que chaque Webview prenne la moitié de la fenêtre
        W1.Height = Double.NaN
        W2.Height = Double.NaN
        W1.VerticalAlignment = VerticalAlignment.Stretch
        W2.VerticalAlignment = VerticalAlignment.Stretch
        W1.HorizontalAlignment = HorizontalAlignment.Left
        W2.HorizontalAlignment = HorizontalAlignment.Right

        If PositionNumber = 2 Then
            W2.Width = (Page.ActualWidth - 400) - 2
            W1.Width = 398
        ElseIf PositionNumber = 3 Then
            W1.Width = (Page.ActualWidth - 400) - 2
            W2.Width = 398
        Else
            W1.Width = (Page.ActualWidth / 2) - 2
            W2.Width = (Page.ActualWidth / 2) - 2
        End If

        List_Right.Width = SelectionPopup.ActualWidth / 2
            List_Left.Width = SelectionPopup.ActualWidth / 2

        If PhoneNavBar.Visibility = Visibility.Visible Then
            If Page.ActualWidth > Page.ActualHeight Then
                W1.Width = (Page.ActualWidth / 2) - 2
                W2.Width = (Page.ActualWidth / 2) - 2
                W1.Height = Double.NaN
                W2.Height = Double.NaN
            Else
                W1.Height = (WebContainer.ActualHeight / 2) - 2
                W2.Height = (WebContainer.ActualHeight / 2) - 2
                W1.VerticalAlignment = VerticalAlignment.Top
                W2.VerticalAlignment = VerticalAlignment.Bottom
                W1.HorizontalAlignment = HorizontalAlignment.Stretch
                W2.HorizontalAlignment = HorizontalAlignment.Stretch
                W1.Width = Double.NaN
                W2.Width = Double.NaN
            End If
        End If

    End Sub
    Private Sub SizeLeft_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles SizeLeft.Tapped
        If SizeLeft.Content = "" Then
            W2.Width = (Page.ActualWidth - 400) - 2
            W1.Width = 398
            AncreGauche.Begin()
            PositionNumber = 2
            SizeLeft.Content = ""
            SizeRight.Content = ""
        Else
            W1.Width = (Page.ActualWidth / 2) - 2
            W2.Width = (Page.ActualWidth / 2) - 2
            PositionNumber = 1
            SizeLeft.Content = ""
            AncreMilieu.Begin()
        End If
    End Sub

    Private Sub SizeRight_Tapped_1(sender As Object, e As TappedRoutedEventArgs) Handles SizeRight.Tapped
        If SizeRight.Content = "" Then
            W1.Width = (Page.ActualWidth - 400) - 2
            W2.Width = 398
            AncreDroit.Begin()
            PositionNumber = 3
            SizeRight.Content = ""
            SizeLeft.Content = ""
        Else
            W1.Width = (Page.ActualWidth / 2) - 2
            W2.Width = (Page.ActualWidth / 2) - 2
            PositionNumber = 1
            SizeRight.Content = ""
            AncreMilieu.Begin()
        End If
    End Sub

    Private Sub Resize_PointerPressed(sender As Object, e As PointerRoutedEventArgs) Handles Resize.PointerPressed
        'Quand on clique sur la boule de redimensionnement des webviews
        AncrageHide.Stop()
        AncrageShow.Begin()
        PositionCurseur = 1

        'On peut resizer par un glissement de souris uniquement si le bouton de la souris est maintenu. Quand il est relaché, la fonction se désactive
        MousePress = True
        'Cachebout est une solution faite un peu à l'arrache pour éviter que le bouton sous la souris se déclence par inadvertance à cause de l'événement OnMouseEnter du bouton
        CacheBout.Visibility = Visibility.Visible
        If PositionNumber = 3 Then
            CacheBout.Margin = New Thickness(0, 0, -35, 0)
        ElseIf PositionNumber = 2 Then
            CacheBout.Margin = New Thickness(-35, 0, 0, 0)
        Else
            CacheBout.Margin = New Thickness(0, 0, 0, 0)
        End If

    End Sub
    Private Sub Resize_PointerReleased(sender As Object, e As PointerRoutedEventArgs) Handles Resize.PointerReleased
        'On peut resizer par un glissement de souris uniquement si le bouton de la souris est maintenu. Quand il est relaché, la fonction se désactive
        MousePress = False
        CacheBout.Visibility = Visibility.Collapsed
    End Sub
    Private Sub Resize_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles Resize.PointerExited
        AncrageShow.Stop()
        AncrageHide.Begin()
        MousePress = False
        CacheBout.Visibility = Visibility.Collapsed
    End Sub
    Private Sub SizeRight_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles SizeRight.PointerEntered
        If MousePress = True Then
            If SizeRight.Content = "" Then
                W1.Width = (Page.ActualWidth - 400) - 2
                W2.Width = 398
                AncreDroit.Begin()
                PositionNumber = 3
                SizeRight.Content = ""
                SizeLeft.Content = ""
            Else
                W1.Width = (Page.ActualWidth / 2) - 2
                W2.Width = (Page.ActualWidth / 2) - 2
                PositionNumber = 1
                SizeRight.Content = ""
                AncreMilieu.Begin()
            End If
        End If
    End Sub

    Private Sub SizeLeft_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles SizeLeft.PointerEntered
        If MousePress = True Then
            If SizeLeft.Content = "" Then
                W2.Width = (Page.ActualWidth - 400) - 2
                W1.Width = 398
                AncreGauche.Begin()
                PositionNumber = 2
                SizeLeft.Content = ""
                SizeRight.Content = ""
            Else
                W1.Width = (Page.ActualWidth / 2) - 2
                W2.Width = (Page.ActualWidth / 2) - 2
                PositionNumber = 1
                SizeLeft.Content = ""
                AncreMilieu.Begin()
            End If
        End If
    End Sub

    Private Sub LeftMenu_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles LeftMenu.PointerEntered
        Left_HideMenu.Stop()
        Left_ShowMenu.Begin()
    End Sub
    Private Sub LeftMenu_PointerEx(sender As Object, e As PointerRoutedEventArgs) Handles LeftMenu.PointerExited
        Left_HideMenu.Begin()
    End Sub
    Private Sub RightMenu_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles RightMenu.PointerEntered
        Right_HideMenu.Stop()
        Right_ShowMenu.Begin()
    End Sub
    Private Sub RightMenu_PointerEx(sender As Object, e As PointerRoutedEventArgs) Handles RightMenu.PointerExited
        Right_HideMenu.Begin()
    End Sub
#End Region
#Region "Begin a fight with your last research as keywords"
    Private Sub Suggest_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles Suggest.PointerEntered
        'Couleur du rectangle de suggestion quand la souris est dessus
        Suggest.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 225, 225, 225))
    End Sub
    Private Sub Suggest_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles Suggest.PointerExited
        'Couleur du rectangle de suggestions quand la souris n'est pas dessus
        Suggest.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255))
    End Sub
    Private Sub Suggest_PointerPressed(sender As Object, e As PointerRoutedEventArgs) Handles Suggest.PointerPressed
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        'Clic sur la suggestion proposée
        Try
            AdressBox.Text = localSettings.Values("textboxe")
        Catch
        End Try
        Fight()
    End Sub

    Private Sub AdressBox_GotFocus(sender As Object, e As RoutedEventArgs) Handles AdressBox.GotFocus
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        Try
            DerniereRecherche.Text = localSettings.Values("textboxe").ToString.ToUpper
            DerniereRecherche1.Text = localSettings.Values("textboxe").ToString.ToUpper
        Catch
        End Try
        ShowSuggestions.Begin()
    End Sub
    Private Sub AdressBox_LostFocus(sender As Object, e As RoutedEventArgs) Handles AdressBox.LostFocus
        HideSuggestion.Begin()
        If PhoneNavBar.Visibility = Visibility.Visible Then
            Dragger.Margin = New Thickness(0, -81, 0, 0)
        Else
            Dragger.Margin = New Thickness(0, 0, 0, 0)
        End If
        BackgroundAdressMobile.Visibility = Visibility.Collapsed
    End Sub

    Private Sub Suggest_Copy_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles Suggest_Copy.PointerEntered
        'Couleur du rectangle de suggestion quand la souris est dessus
        Suggest_Copy.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 225, 225, 225))
    End Sub
    Private Sub Suggest_Copy_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles Suggest_Copy.PointerExited
        'Couleur du rectangle de suggestions quand la souris n'est pas dessus
        Suggest_Copy.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255))
    End Sub
    Private Sub Suggest_Copy_PointerPressed(sender As Object, e As PointerRoutedEventArgs) Handles Suggest_Copy.PointerPressed
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        'Clic sur la suggestion proposée
        Try
            AdressBox.Text = localSettings.Values("textboxe")
        Catch
        End Try
        Fight()
        PopupClose.Begin()
    End Sub

    Private Sub Popup_Adressbox_GotFocus(sender As Object, e As RoutedEventArgs) Handles Popup_Adressbox.GotFocus
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        Try
            DerniereRecherche.Text = localSettings.Values("textboxe").ToString.ToUpper
            DerniereRecherche1.Text = localSettings.Values("textboxe").ToString.ToUpper
        Catch
        End Try
        ShowPopupSuggestion.Begin()
    End Sub
    Private Sub Popup_Adressbox_LostFocus(sender As Object, e As RoutedEventArgs) Handles Popup_Adressbox.LostFocus
        HidePopipSuggestion.Begin()
    End Sub
#End Region
#Region "Show results in the main page"
    Private Sub LeftOpen()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("LoadPageFromBluestart") = True
        localSettings.Values("LoadPageFromBluestart_Adress") = W1.Source.ToString
        Me.Frame.Navigate(GetType(MainPage))
    End Sub
    Private Sub Left_OpenInBrowser_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Left_OpenInBrowser.Tapped
        LeftOpen()
    End Sub
    Private Sub RightOpen()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("LoadPageFromBluestart") = True
        localSettings.Values("LoadPageFromBluestart_Adress") = W2.Source.ToString
        Me.Frame.Navigate(GetType(MainPage))
    End Sub
    Private Sub Right_OpenInBrowser_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Right_OpenInBrowser.Tapped
        RightOpen()
    End Sub
#End Region
#Region "Set a new Search engine (Settings)"
    Private Sub LeftEngine()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        'Définit les valeurs du moteur de recherche tel que le navigateur navigue vers (A1 + Mots-clés + A2) = URI

        Header1.Text = localSettings.Values("F1Text")

        If Header1.Text = "QWANT" Then
            localSettings.Values("A1") = "http://www.qwant.com/?q="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 1
        ElseIf Header1.Text = "GOOGLE" Then
            localSettings.Values("A1") = "http://www.google.fr/search?q="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 4
        ElseIf Header1.Text = "BING" Then
            localSettings.Values("A1") = "http://www.bing.com/search?q="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 0
        ElseIf Header1.Text = "DUCKDUCKGO" Then
            localSettings.Values("A1") = "http://duckduckgo.com/?q="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 2
        ElseIf Header1.Text = "QWANT WEB" Then
            localSettings.Values("A1") = "http://www.qwant.com/?q="
            localSettings.Values("A2") = "&t=web"
            localSettings.Values("SearchEngineIndex") = 5
        ElseIf Header1.Text = "YAHOO" Then
            localSettings.Values("A1") = "http://fr.search.yahoo.com/search;_ylt=Ai38ykBDWJSAxF25NrTnjXxNhJp4?p="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 3
        ElseIf Header1.Text = "ASK" Then
            localSettings.Values("A1") = "http://fr.ask.com/web?q="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 6
        ElseIf Header1.Text = "YOUTUBE" Then
            localSettings.Values("A1") = "http://www.youtube.com/results?search_query="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 7
        ElseIf Header1.Text = "DAILYMOTION" Then
            localSettings.Values("A1") = "http://www.dailymotion.com/fr/relevance/universal/search/"
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 9
        ElseIf Header1.Text = "VIMEO" Then
            localSettings.Values("A1") = "http://vimeo.com/search?q="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 8
        ElseIf Header1.Text = "WIKIPEDIA" Then
            localSettings.Values("A1") = "http://fr.wikipedia.org/w/index.php?search="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 10
        ElseIf Header1.Text = "TWITTER" Then
            localSettings.Values("A1") = "http://twitter.com/search?q="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 11
        End If
        Dim notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02)
        Dim toeastElement = notificationXml.GetElementsByTagName("text")
        'toeastElement(0).AppendChild(notificationXml.CreateTextNode(resourceLoader.GetString("Notification_SearchEngineSet_Header/Text")))
        'toeastElement(1).AppendChild(notificationXml.CreateTextNode(resourceLoader.GetString("Notification_SearchEngineSet_Content/Text")))
        toeastElement(0).AppendChild(notificationXml.CreateTextNode(Label_Notif_Header.Text))
        toeastElement(1).AppendChild(notificationXml.CreateTextNode(Label_Notif_Content.Text))
        Dim ToastNotification = New ToastNotification(notificationXml)
        ToastNotificationManager.CreateToastNotifier().Show(ToastNotification)
    End Sub
    Private Sub Left_DefaultSearchEngine_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Left_DefaultSearchEngine.Tapped
        LeftEngine()
    End Sub
    Private Sub RightEngine()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        'Définit les valeurs du moteur de recherche tel que le navigateur navigue vers (A1 + Mots-clés + A2) = URI
        Header2.Text = localSettings.Values("F2Text")
        If Header2.Text = "QWANT" Then
            localSettings.Values("A1") = "http://www.qwant.com/?q="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 1
        ElseIf Header2.Text = "GOOGLE" Then
            localSettings.Values("A1") = "http://www.google.fr/search?q="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 4
        ElseIf Header2.Text = "BING" Then
            localSettings.Values("A1") = "http://www.bing.com/search?q="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 0
        ElseIf Header2.Text = "DUCKDUCKGO" Then
            localSettings.Values("A1") = "http://duckduckgo.com/?q="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 2
        ElseIf Header2.Text = "QWANT WEB" Then
            localSettings.Values("A1") = "http://www.qwant.com/?q="
            localSettings.Values("A2") = "&t=web"
            localSettings.Values("SearchEngineIndex") = 5
        ElseIf Header2.Text = "YAHOO" Then
            localSettings.Values("A1") = "http://fr.search.yahoo.com/search;_ylt=Ai38ykBDWJSAxF25NrTnjXxNhJp4?p="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 3
        ElseIf Header2.Text = "ASK" Then
            localSettings.Values("A1") = "http://fr.ask.com/web?q="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 6
        ElseIf Header2.Text = "YOUTUBE" Then
            localSettings.Values("A1") = "http://www.youtube.com/results?search_query="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 7
        ElseIf Header2.Text = "DAILYMOTION" Then
            localSettings.Values("A1") = "http://www.dailymotion.com/fr/relevance/universal/search/"
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 9
        ElseIf Header2.Text = "VIMEO" Then
            localSettings.Values("A1") = "http://vimeo.com/search?q="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 8
        ElseIf Header2.Text = "WIKIPEDIA" Then
            localSettings.Values("A1") = "http://fr.wikipedia.org/w/index.php?search="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 10
        ElseIf Header2.Text = "TWITTER" Then
            localSettings.Values("A1") = "http://twitter.com/search?q="
            localSettings.Values("A2") = ""
            localSettings.Values("SearchEngineIndex") = 11
        End If
        Dim notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02)
        Dim toeastElement = notificationXml.GetElementsByTagName("text")
        'toeastElement(0).AppendChild(notificationXml.CreateTextNode(resourceLoader.GetString("Notification_SearchEngineSet_Header/Text")))
        'toeastElement(1).AppendChild(notificationXml.CreateTextNode(resourceLoader.GetString("Notification_SearchEngineSet_Content/Text")))
        toeastElement(0).AppendChild(notificationXml.CreateTextNode(Label_Notif_Header.Text))
        toeastElement(1).AppendChild(notificationXml.CreateTextNode(Label_Notif_Content.Text))
        Dim ToastNotification = New ToastNotification(notificationXml)
        ToastNotificationManager.CreateToastNotifier().Show(ToastNotification)
    End Sub
    Private Sub Right_DefaultSearchEngine_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Right_DefaultSearchEngine.Tapped
        RightEngine()
    End Sub


#End Region
#Region "Popup"

    Private Sub Selector_Background_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Selector_Background.Tapped

    End Sub

    Private Sub Close_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Close.Tapped
        PopupClose.Begin()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("F1Text") = Header1.Text
        localSettings.Values("F2Text") = Header2.Text
        Drop1.Text = localSettings.Values("F1Text").ToString.ToLowerInvariant
        Drop2.Text = localSettings.Values("F2Text").ToString.ToLowerInvariant

    End Sub

    Private Sub Bing_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Bing.Tapped
        Header2.Text = "BING"
    End Sub

    Private Sub Qwant_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Qwant.Tapped
        Header2.Text = "QWANT"
    End Sub

    Private Sub DuckDuckGo_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles DuckDuckGo.Tapped
        Header2.Text = "DUCKDUCKGO"
    End Sub

    Private Sub Yahoo_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Yahoo.Tapped
        Header2.Text = "YAHOO"
    End Sub

    Private Sub Google_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Google.Tapped
        Header2.Text = "GOOGLE"
    End Sub

    Private Sub Qwant_W_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Qwant_W.Tapped
        Header2.Text = "QWANT WEB"
    End Sub

    Private Sub Ask_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Ask.Tapped
        Header2.Text = "ASK"
    End Sub

    Private Sub Youtube_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Youtube.Tapped
        Header2.Text = "YOUTUBE"
    End Sub

    Private Sub Vimeo_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Vimeo.Tapped
        Header2.Text = "VIMEO"
    End Sub

    Private Sub Dailymotion_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Dailymotion.Tapped
        Header2.Text = "DAILYMOTION"
    End Sub

    Private Sub Wiki_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Wiki.Tapped
        Header2.Text = "WIKIPEDIA"
    End Sub

    Private Sub Twitter_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Twitter.Tapped
        Header2.Text = "TWITTER"
    End Sub

    Private Sub Bing1_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Bing1.Tapped
        Header1.Text = "BING"
    End Sub

    Private Sub Qwant1_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Qwant1.Tapped
        Header1.Text = "QWANT"
    End Sub

    Private Sub DuckDuckGo1_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles DuckDuckGo1.Tapped
        Header1.Text = "DUCKDUCKGO"
    End Sub

    Private Sub Yahoo1_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Yahoo1.Tapped
        Header1.Text = "YAHOO"
    End Sub

    Private Sub Google1_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Google1.Tapped
        Header1.Text = "GOOGLE"
    End Sub

    Private Sub Qwant_W1_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Qwant_W1.Tapped
        Header1.Text = "QWANT WEB"
    End Sub

    Private Sub Ask1_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Ask1.Tapped
        Header1.Text = "ASK"
    End Sub

    Private Sub Youtube1_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Youtube1.Tapped
        Header1.Text = "YOUTUBE"
    End Sub

    Private Sub Vimeo1_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Vimeo1.Tapped
        Header1.Text = "VIMEO"
    End Sub

    Private Sub Dailymotion1_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Dailymotion1.Tapped
        Header1.Text = "DAILYMOTION"
    End Sub

    Private Sub Wiki1_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Wiki1.Tapped
        Header1.Text = "WIKIPEDIA"
    End Sub

    Private Sub Twitter1_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Twitter1.Tapped
        Header1.Text = "TWITTER"
    End Sub

    Private Sub DropDown_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles DropDown.Tapped
        ShowPopup()
    End Sub
    Private Sub ShowPopup()
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        PopupClose.Stop()
        PopupShow.Stop()
        PopupShow.Begin()
        Try
            Header1.Text = localSettings.Values("F1Text")
            Header2.Text = localSettings.Values("F2Text")
        Catch
            Header1.Text = "QWANT"
            Header2.Text = "BING"
        End Try
        Popup_Adressbox.Focus(Windows.UI.Xaml.FocusState.Keyboard)
        Popup_Adressbox.Text = AdressBox.Text
    End Sub

    Private Sub Selector_Background_PointerMoved(sender As Object, e As PointerRoutedEventArgs) Handles Selector_Background.PointerMoved
        List_Right.Width = SelectionPopup.ActualWidth / 2
        List_Left.Width = SelectionPopup.ActualWidth / 2
    End Sub

    Private Sub DropDown_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles DropDown.PointerEntered
        DropDownBackground.Visibility = Visibility.Visible
        DropDownBackground.Fill = rectangle.Fill
    End Sub
    Private Sub DropDown_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles DropDown.PointerExited
        DropDownBackground.Visibility = Visibility.Collapsed
    End Sub
#End Region
#Region "PhoneUI"
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

    Private Sub Grid_Tapped(sender As Object, e As TappedRoutedEventArgs)
        Dragger.Margin = New Thickness(0, -30, 0, 0)
        AdressBox.Focus(Windows.UI.Xaml.FocusState.Keyboard)
        BackgroundAdressMobile.Visibility = Visibility.Visible
    End Sub

    Private Sub Ellipsis_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Ellipsis_Button.Tapped
        ShowPopup()
        List_Right.Width = Page.ActualWidth / 2
        List_Left.Width = Page.ActualWidth / 2
    End Sub

    Private Sub W1_LoadCompleted(sender As Object, e As NavigationEventArgs) Handles W1.LoadCompleted

    End Sub

    Private Sub Menu_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles MenuBut.Tapped

        Dim menu As MenuFlyout = New MenuFlyout

        Dim LTitle As MenuFlyoutSubItem = New MenuFlyoutSubItem
        Try
            LTitle.Text = W1.Source.Host
        Catch
            LTitle.Text = "about:blank"
        End Try
        menu.Items.Add(LTitle)

        Dim LOpenInBrowser As MenuFlyoutItem = New MenuFlyoutItem
        LOpenInBrowser.Text = Label_OpenInBlueflap.Text
        LTitle.Items.Add(LOpenInBrowser)

        Dim LSetSearchENgine As MenuFlyoutItem = New MenuFlyoutItem
        LSetSearchENgine.Text = Label_SetSearchEngine.Text
        LTitle.Items.Add(LSetSearchENgine)

        Dim RTitle As MenuFlyoutSubItem = New MenuFlyoutSubItem
        Try
            RTitle.Text = W2.Source.Host
        Catch
            RTitle.Text = "about:blank"
        End Try
        menu.Items.Add(RTitle)

        Dim ROpenInBrowser As MenuFlyoutItem = New MenuFlyoutItem
        ROpenInBrowser.Text = Label_OpenInBlueflap.Text
        RTitle.Items.Add(ROpenInBrowser)

        Dim RSetSearchENgine As MenuFlyoutItem = New MenuFlyoutItem
        RSetSearchENgine.Text = Label_SetSearchEngine.Text
        RTitle.Items.Add(RSetSearchENgine)


        AddHandler ROpenInBrowser.Tapped, New TappedEventHandler(Sub(Machin As Object, Truc As TappedRoutedEventArgs)
                                                                     RightOpen()
                                                                 End Sub)

        AddHandler RSetSearchENgine.Tapped, New TappedEventHandler(Sub(Machin As Object, Truc As TappedRoutedEventArgs)
                                                                       RightEngine()
                                                                   End Sub)


        AddHandler LOpenInBrowser.Tapped, New TappedEventHandler(Sub(Machin As Object, Truc As TappedRoutedEventArgs)
                                                                     LeftOpen()
                                                                 End Sub)

        AddHandler LSetSearchENgine.Tapped, New TappedEventHandler(Sub(Machin As Object, Truc As TappedRoutedEventArgs)
                                                                       LeftEngine()
                                                                   End Sub)

        menu.ShowAt(MenuBut)

    End Sub
#End Region
End Class
