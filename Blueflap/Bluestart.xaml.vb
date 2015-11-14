' Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
''' </summary>
Public NotInheritable Class Bluestart
    Inherits Page


    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        Titlebar.Text = System.DateTime.Now.ToString("dddd dd MMMM yyyy")
        LoadAnim.Begin()
        RechercheBox.IsEnabled = False
        RechercheBox.IsEnabled = True

        Dim v = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView()
        v.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 27, 27, 27)
        v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.White
        v.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 27, 27, 27)
        If RechercheBox.Text = "" Then
            textBlock.Visibility = Visibility.Visible
            LoopSuggestions.Begin()
        End If
    End Sub

    Private Sub TextBox_GotFocus(sender As Object, e As RoutedEventArgs)
        Textboxclick.Begin()
    End Sub

    Private Sub TextBox_LostFocus(sender As Object, e As RoutedEventArgs)
        Textboxleave.Begin()
    End Sub

    Private Sub grid_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles grid.Tapped
        RechercheBox.Focus(Windows.UI.Xaml.FocusState.Keyboard)
    End Sub

    Private Sub HoverRecherche_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles HoverRecherche.PointerEntered
        HoverRecherche.Fill = New SolidColorBrush(Windows.UI.Color.FromArgb(20, 0, 0, 0))
    End Sub

    Private Sub HoverRecherche_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles HoverRecherche.PointerExited
        HoverRecherche.Fill = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0))
    End Sub

    Private Sub HoverRecherche_PointerPressed(sender As Object, e As PointerRoutedEventArgs) Handles HoverRecherche.PointerPressed
        HoverRecherche.Fill = New SolidColorBrush(Windows.UI.Color.FromArgb(40, 0, 0, 0))
    End Sub

    Private Sub HoverRecherche_PointerReleased(sender As Object, e As PointerRoutedEventArgs) Handles HoverRecherche.PointerReleased
        HoverRecherche.Fill = New SolidColorBrush(Windows.UI.Color.FromArgb(20, 0, 0, 0))
    End Sub

    Private Sub HoverRecherche_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles HoverRecherche.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

        localSettings.Values("LoadPageFromBluestart") = True
        localSettings.Values("LoadPageFromBluestart_Adress") = RechercheBox.Text
        Me.Frame.Navigate(GetType(MainPage))

    End Sub

    Private Sub RechercheBox_KeyDown(sender As Object, e As KeyRoutedEventArgs) Handles RechercheBox.KeyDown
        If (e.Key = Windows.System.VirtualKey.Enter) Then  'Permet de réagir à l'appui sur la touche entrée
            Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

            localSettings.Values("LoadPageFromBluestart") = True
            localSettings.Values("LoadPageFromBluestart_Adress") = RechercheBox.Text
            Me.Frame.Navigate(GetType(MainPage))
        End If
    End Sub

    Private Sub Button_Tapped(sender As Object, e As TappedRoutedEventArgs)
        Me.Frame.Navigate(GetType(MainPage))
    End Sub

    Private Sub Fight_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Fight_Button.Tapped
        Me.Frame.Navigate(GetType(SearchFight))
    End Sub

    Private Sub RechercheBox_TextChanged(sender As Object, e As TextChangedEventArgs) Handles RechercheBox.TextChanged
        LoopSuggestions.Stop()
        textBlock.Visibility = Visibility.Collapsed
    End Sub
End Class
