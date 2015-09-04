' Pour plus d'informations sur le modèle Application vide, consultez la page http://go.microsoft.com/fwlink/?LinkId=234227

''' <summary>
''' Fournit un comportement spécifique à l'application afin de compléter la classe Application par défaut.
''' </summary>
NotInheritable Class App
    Inherits Application

    ''' <summary>
    ''' Invoqué lorsque l'application est lancée normalement par l'utilisateur final.  D'autres points d'entrée
    ''' sont utilisés lorsque l'application est lancée pour ouvrir un fichier spécifique, pour afficher
    ''' des résultats de recherche, etc.
    ''' </summary>
    ''' <param name="e">Détails concernant la requête et le processus de lancement.</param>
    Protected Overrides Sub OnLaunched(e As Windows.ApplicationModel.Activation.LaunchActivatedEventArgs)
#If DEBUG Then
        ' Affichez des informations de profilage graphique lors du débogage.
        If System.Diagnostics.Debugger.IsAttached Then
            ' Afficher les compteurs de fréquence des trames actuels
            Me.DebugSettings.EnableFrameRateCounter = True
        End If
#End If

        Dim rootFrame As Frame = TryCast(Window.Current.Content, Frame)

        ' Ne répétez pas l'initialisation de l'application lorsque la fenêtre comporte déjà du contenu,
        ' assurez-vous juste que la fenêtre est active

        If rootFrame Is Nothing Then
            ' Créez un Frame utilisable comme contexte de navigation et naviguez jusqu'à la première page
            rootFrame = New Frame()
            ' Définir la page par défaut
            rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages(0)

            AddHandler rootFrame.NavigationFailed, AddressOf OnNavigationFailed

            If e.PreviousExecutionState = ApplicationExecutionState.Terminated Then
                ' TODO: chargez l'état de l'application précédemment suspendue
            End If
            ' Placez le frame dans la fenêtre active
            Window.Current.Content = rootFrame
        End If
        If rootFrame.Content Is Nothing Then
            ' Quand la pile de navigation n'est pas restaurée, accédez à la première page,
            ' puis configurez la nouvelle page en transmettant les informations requises en tant que
            ' paramètre
            rootFrame.Navigate(GetType(MainPage), e.Arguments)
        End If

        ' Vérifiez que la fenêtre actuelle est active
        Window.Current.Activate()
    End Sub

    ''' <summary>
    ''' Appelé lorsque la navigation vers une page donnée échoue
    ''' </summary>
    ''' <param name="sender">Frame à l'origine de l'échec de navigation.</param>
    ''' <param name="e">Détails relatifs à l'échec de navigation</param>
    Private Sub OnNavigationFailed(sender As Object, e As NavigationFailedEventArgs)
        Throw New Exception("Failed to load Page " + e.SourcePageType.FullName)
    End Sub

    ''' <summary>
    ''' Appelé lorsque l'exécution de l'application est suspendue.  L'état de l'application est enregistré
    ''' sans savoir si l'application pourra se fermer ou reprendre sans endommager
    ''' le contenu de la mémoire.
    ''' </summary>
    ''' <param name="sender">Source de la requête de suspension.</param>
    ''' <param name="e">Détails de la requête de suspension.</param>
    Private Sub OnSuspending(sender As Object, e As SuspendingEventArgs) Handles Me.Suspending
        Dim deferral As SuspendingDeferral = e.SuspendingOperation.GetDeferral()
        ' TODO: enregistrez l'état de l'application et arrêtez toute activité en arrière-plan
        deferral.Complete()
    End Sub

End Class
