' Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
''' </summary>
Public NotInheritable Class Customization
    Inherits Page
#Region "Frame.GoBack"
    Public Sub New()
        Me.InitializeComponent()
        AddHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf MainPage_BackRequested
    End Sub
    Private Sub MainPage_BackRequested(sender As Object, e As Windows.UI.Core.BackRequestedEventArgs)
        'On retourne à la page principale quand le bouton retour "physique" est pressé
        If Frame.CanGoBack Then
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
#Region "Page Loaded"
    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        'LoadAnim.Begin() 'Animation d'ouverture

        Dim v = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView()
        v.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 27, 27, 27)
        v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.White
        v.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 27, 27, 27)

        Try
            If localSettings.Values("WallpaperType") = "Custom" Then 'Définit l'image de fond : image prédéfinie ou image en ligne
                Wallpaper.Source = New BitmapImage(New Uri(localSettings.Values("WallpaperSource"), UriKind.Absolute))
                OnlineImagePath.Visibility = Visibility.Visible
                OnlineImagePath.Text = localSettings.Values("WallpaperSource")
            Else
                Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
                OnlineImagePath.Visibility = Visibility.Collapsed
            End If
        Catch ex As Exception

        End Try

    End Sub
#End Region
#Region "Wallpaper"
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

    Private Sub OnlineImagePath_TextChanged(sender As Object, e As TextChangedEventArgs) Handles OnlineImagePath.TextChanged
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperSource") = OnlineImagePath.Text
        localSettings.Values("WallpaperType") = "Custom"
    End Sub


    Private Sub Button_Tapped_1(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Wallpaper_Bird.jpg"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub

    Private Sub Button_Tapped_2(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Wallpaper_bokeh.jpg"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub

    Private Sub Button_Tapped_3(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Wallpaper_building.jpg"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub

    Private Sub Button_Tapped_4(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Wallpaper_Clouds.jpg"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub

    Private Sub Button_Tapped_5(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Wallpaper_Clouds2.jpg"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub

    Private Sub Button_Tapped_6(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Wallpaper_Door.jpg"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub

    Private Sub Button_Tapped_7(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Wallpaper_leap.jpg"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub

    Private Sub Button_Tapped_8(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Wallpaper_Moon.jpg"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub

    Private Sub Button_Tapped_9(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Wallpaper_Morning.jpg"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub

    Private Sub Button_Tapped_10(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Wallpaper_sky.jpg"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub

    Private Sub Button_Tapped_11(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Wallpaper_Street.jpg"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub

    Private Sub Button_Tapped_12(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Wallpaper_tree.jpg"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub

    Private Sub Button_Tapped_13(sender As Object, e As TappedRoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("WallpaperName") = "Wallpaper_Wall.jpg"
        Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
        localSettings.Values("WallpaperType") = "Default"
    End Sub
#End Region
End Class
