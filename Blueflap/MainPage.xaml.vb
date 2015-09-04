' Pour en savoir plus sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page

    Private Sub textBlock_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles textBlock.Tapped
        Anim.Begin()
    End Sub
End Class
