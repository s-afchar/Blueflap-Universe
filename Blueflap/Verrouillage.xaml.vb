' Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
''' </summary>
Public NotInheritable Class Verrouillage
    Inherits Page
    Public Sub New()
        Me.InitializeComponent()
        AddHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf Parametres_BackRequested
    End Sub
    Private Sub Parametres_BackRequested(sender As Object, e As Windows.UI.Core.BackRequestedEventArgs)
        e.Handled = True
        Wrongpassword.Stop()
        Wrongpassword.Begin()
    End Sub
    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("ShowLockScreen") = True
        LoadAnim.Begin() 'Animation d'ouverture
        Dim v = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView()
        v.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 27, 27, 27)
        v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.White
        v.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 27, 27, 27)
        Try
            If localSettings.Values("WallpaperType") = "Custom" Then
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
        DateHours.Text = System.DateTime.Now.ToString("HH:mm")

        If localSettings.Values("KidsMode") = True Then
            KidsProfile.Visibility = Visibility.Visible
        Else
            KidsProfile.Visibility = Visibility.Collapsed
        End If

        Try
            profilpicture.ImageSource = New BitmapImage(New Uri(localSettings.Values("ProfilPictureSource"), UriKind.Absolute))
        Catch ex As Exception
            profilpicture.ImageSource = New BitmapImage(New Uri("ms-appx:/Assets/defaultprofilpic.jpg", UriKind.Absolute))
        End Try


        Try
            profilkidpicture.ImageSource = New BitmapImage(New Uri(localSettings.Values("KidProfilPictureSource"), UriKind.Absolute))
        Catch ex As Exception
            profilkidpicture.ImageSource = New BitmapImage(New Uri("ms-appx:/Assets/defaultkidprofilpic.jpg", UriKind.Absolute))
        End Try

    End Sub

    Private Sub PaswwordBox_KeyDown(sender As Object, e As KeyRoutedEventArgs) Handles PaswwordBox.KeyDown
        If (e.Key = Windows.System.VirtualKey.Enter) Then  'Permet de réagir à l'appui sur la touche entrée
            Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
            Try
                If PaswwordBox.Password = localSettings.Values("Password") Then
                    RemoveHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf Parametres_BackRequested
                    localSettings.Values("ShowLockScreen") = False
                    If Me.Frame.CanGoBack Then
                        Me.Frame.GoBack()
                    Else
                        Me.Frame.Navigate(GetType(MainPage))
                    End If
                Else
                    Wrongpassword.Stop()
                    Wrongpassword.Begin()
                End If
            Catch
            End Try
        End If
    End Sub

    Private Sub PaswwordBox_GotFocus(sender As Object, e As RoutedEventArgs) Handles PaswwordBox.GotFocus

    End Sub
    Private Sub PaswwordBox_LostFocus(sender As Object, e As RoutedEventArgs) Handles PaswwordBox.LostFocus

    End Sub

    Private Sub KidsProfile_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles KidsProfile.Tapped
        ProfilSwitch.Begin()
    End Sub

    Private Sub grid1_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles grid1.Tapped
        ProfilSwitchBack.Begin()
    End Sub

    Private Sub OpenKids_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles OpenKids.Tapped
        Me.Frame.Navigate(GetType(KidsMode))
    End Sub
End Class
