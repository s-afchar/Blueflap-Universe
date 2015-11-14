' Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
''' </summary>
Public NotInheritable Class Verrouillage
    Inherits Page

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        Titlebar.Text = System.DateTime.Now.ToString("dddd dd MMMM yyyy")
    End Sub
End Class
