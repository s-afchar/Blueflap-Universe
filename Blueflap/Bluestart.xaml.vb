' Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
''' </summary>
Public NotInheritable Class Bluestart
    Inherits Page

#Region "Page Loaded"
    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("LoadPageFromBluestart") = False

        Titlebar.Text = System.DateTime.Now.ToString("dddd dd MMMM yyyy") 'Affiche la date du jour dans la titlebar

        LoadAnim.Begin() 'Animation d'ouverture

        Dim v = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView()
        v.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 27, 27, 27)
        v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.White
        v.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 27, 27, 27)
        If RechercheBox.Text = "" Then
            textBlock.Visibility = Visibility.Visible
            LoopSuggestions.Stop()
            LoopSuggestions.Begin()
        End If

        Try
            If localSettings.Values("WallpaperType") = "Custom" Then 'Définit l'image de fond : image prédéfinie ou image en ligne
                Loader.Visibility = Visibility.Visible
                DownloadingWallpaper.Begin()
                Wallpaper.Source = New BitmapImage(New Uri(localSettings.Values("WallpaperSource"), UriKind.Absolute))
            Else
                DownloadingWallpaper.Stop()
                Loader.Visibility = Visibility.Collapsed

                Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
            End If
        Catch ex As Exception

        End Try

        grid1.Visibility = Visibility.Collapsed
        grid3.Visibility = Visibility.Collapsed

    End Sub
#End Region
#Region "Good Looking"
    'Ce qui suit est d'ordre visuel
    '<Visuel>
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
    '</Visuel>
#End Region
#Region "SearchBox"
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
    Private Sub RechercheBox_TextChanged(sender As Object, e As TextChangedEventArgs) Handles RechercheBox.TextChanged
        LoopSuggestions.Stop()
        textBlock.Visibility = Visibility.Collapsed
    End Sub
#End Region
#Region "Go to (other frame)"
    Private Sub Button_Tapped(sender As Object, e As TappedRoutedEventArgs)
        Me.Frame.Navigate(GetType(MainPage))
    End Sub

    Private Sub Fight_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Fight_Button.Tapped
        Me.Frame.Navigate(GetType(SearchFight))
    End Sub

    Private Sub Lock_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Lock_Button.Tapped
        Me.Frame.Navigate(GetType(Verrouillage))
    End Sub
#End Region
#Region "Personnalization panel"
    ' Private Async Sub Button_Tapped_1(sender As Object, e As TappedRoutedEventArgs)
    'Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings


    'Dim picker = New Windows.Storage.Pickers.FileOpenPicker()
    '        picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail
    '        picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
    '        picker.FileTypeFilter.Add(".jpg")
    '        picker.FileTypeFilter.Add(".jpeg")
    '        picker.FileTypeFilter.Add(".png")
    '
    '    Dim FichierSelectionne As Windows.Storage.StorageFile = Await picker.PickSingleFileAsync()
    '    If FichierSelectionne IsNot Nothing Then
    '    Dim Chemin = Await FichierSelectionne.OpenAsync(Windows.Storage.FileAccessMode.Read)
    '    Dim bitmapImage = New Windows.UI.Xaml.Media.Imaging.BitmapImage()
    '            Await bitmapImage.SetSourceAsync(Chemin)
    '            Wallpaper.Source = bitmapImage
    '
    '            localSettings.Values("WallpaperImagePath") = FichierSelectionne.Path
    '
    '    End If
    '    End Sub

    Private Sub Personnalisation_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Personnalisation_Button.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        If grid1.Visibility = Visibility.Visible Then
            grid1.Visibility = Visibility.Collapsed
        Else
            Try
                If localSettings.Values("WallpaperType") = "Custom" Then
                    OnlineImagePath.Visibility = Visibility.Visible
                    OnlineImagePath.Text = localSettings.Values("WallpaperSource")
                Else
                    OnlineImagePath.Visibility = Visibility.Collapsed
                End If
            Catch ex As Exception

            End Try

            OpenPersonnalize.Stop()
            OpenPersonnalize.Begin()
            grid3.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Sub OnlineImagePath_TextChanged(sender As Object, e As TextChangedEventArgs) Handles OnlineImagePath.TextChanged
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperSource") = OnlineImagePath.Text
        localSettings.Values("WallpaperType") = "Custom"
    End Sub

    Private Sub SetWallpaper1(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Degrade.png"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub
    Private Sub SetWallpaper2(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Degrade2.png"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub
    Private Sub SetWallpaper3(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Ciel.JPG"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub
    Private Sub SetWallpaper4(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Fleurs.jpg"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub
    Private Sub SetWallpaper5(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Montagne.jpg"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub
    Private Sub SetWallpaper6(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Neige.jpg"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub
    Private Sub SetWallpaper7(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Plage.jpg"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub
    Private Sub SetWallpaper8(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Soleil.JPG"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub
    Private Sub SetWallpaper9(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Cailloux.jpg"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub

    Private Sub SetCustomWallpaper(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        If OnlineImagePath.Visibility = Visibility.Visible Then
            OnlineImagePath.Visibility = Visibility.Collapsed
            localSettings.Values("WallpaperType") = "Default"
        Else
            OnlineImagePath.Visibility = Visibility.Visible
            localSettings.Values("WallpaperType") = "Custom"
            Try
                OnlineImagePath.Text = localSettings.Values("WallpaperSource")
            Catch
            End Try
        End If
    End Sub


#End Region
#Region "Memo"
    Private Sub Memo_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Memo_Button.Tapped
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        If grid3.Visibility = Visibility.Visible Then
            grid3.Visibility = Visibility.Collapsed
        Else
            Try
                MemoText.Text = localSettings.Values("MemoText")
            Catch
            End Try

            OpenMemo.Stop()
            OpenMemo.Begin()
            grid1.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Sub memotext_TextChanged(sender As Object, e As TextChangedEventArgs) Handles memotext.TextChanged
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("MemoText") = memotext.Text 'Là on enregistre le texte des mémos en continu
    End Sub
#End Region
End Class
