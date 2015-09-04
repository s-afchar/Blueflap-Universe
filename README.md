Blueflap
========

![Alt text](Images/Bluewindow.png "Représentation de la fenêtre de Blueflap")

Blueflap est un navigateur internet en VB.NET.
Blueflap est un projet français, il est actuellement uniquement Francophone.
Des traductions en Anglais et en Allemand sont néanmoins prêtes et seront publiées quand Blueflap 3.0 sera finalisé.

Pourquoi contribuer au projet :
Je ne parviens pas à résoudre plusieurs problèmes, ainsi, grâce à votre aide j'espère arriver à sortir une V3 sans Bugs.

-----------------

![Alt text](Images/Texte0.png "Important")

_Il est nécessaire d'ajouter tous les .dll du dossier "DLL" dans les références de votre projet.
Pour cela, dans l'explorateur de solution, faites un clic-droit sur l'item Blueflap. Cliquez sur "Ajouter une référence" puis sur "parcourir" pour ajouter les fichiers du dossier DLL._

-----------------
-----------------
![Alt text](Images/Texte1.png "Design")

Déterminer l'interface et le design de Blueflap 3.0 a demandé une très grande reflexion.
D'abord, nous voulions garder l'idée de volet latéral (sinon le nom "Blueflap", littéralement "Volet bleu" n'avait plus aucun sens...)

Ensuite, au niveau du design, nous avons travaillé sur plusieurs pistes. Le design "skeuomorphique" de Blueflap 1.0 faisait un peu vieillot. Nous nous sommes alors penché sur les designs tendance du moment.

Premier constat : nous DEVIONS faire du flat design

Second constat : Les deux designs "flat" du moment sont ModernUI (Windows 8.x, Windows Phone) et iOS 7 (iPhone...).

Nous avons alors décidé de créer une interface qui combinerait les deux.

C'est ainsi qu'est néé le design de Blueflap 3 :

- Une interface fine type iOS 7 laissant place entière au contenu sur la partie navigation internet.
- Cette interface est couplée à des éléments ModernUI, comme le volet des favoris par exemple.

- Une interface reprenant les codes de Windows 8 dans les paramètres et les infos de la page par exemple.

Faire cohabiter ces deux interfaces opposées était un petit défi, et nous espérons l'avoir réussi... :smile:

-----------------

![Alt text](Images/Texte3.png "Moteur Web")

Blueflap utilisait jusqu'à 3.x un moteur Trident, le même que celui d'internet Explorer 7 : un moteur peu performant et peu respectueux des normes actuelles du web.

C'est pour cela que depuis 3.x, Blueflap utilise le moteur Awesomium, qui est un fork de Chromium 18.

Sur 4.x, Blueflap utilise le tout nouveau moteur Awesomium 1.7.5 !

-----------------
![Alt text](Images/Texte4.png "Fonctions sup")

Pourquoi choisir Blueflap plutôt qu'un autre navigateur ?

Tout d'abord, pour privilégier un projet Français... Bon... d'accord, ce n'est pas un argument...

- Sur Blueflap, vous avez "SearchFight", un comparateur de moteur de recherche qui vous permettra de trouver votre favoris !
- Blueflap intègre d'ailleurs par défaut de nombreux moteurs de recherches !
- Le verrouillage par code est peut-être la meilleure innovation de Blueflap : elle vous permet de laisser votre ordinateur allumé tout en bloquant l'accès au navigateur.
- Un système de Post-it virtuels vous permet d'éviter de coller des post-it sur votre magnifique écran tactile 4K Super-Amoled antitrace antireflets antirayures à la technogie Power-Ion à projection holographique !

-----------------
![Alt text](Images/Texte6.png "Le nom")

Blueflap signifie littéralement "Volet Bleu". Pourquoi un tel nom ? C'est très simple : La première version de Blueflap (0.7) avait pour élément caractéristique un volet latéral regroupant toutes les fonctions utiles d'un navigateur. Dans ce volet, tous les éléments étaient bleus, c'est ainsi que ce navigateur qui devait s'appeler "SimpleBrowser" se fit finalement appeler "Blueflap".

Anecdote : Jusqu'à la version 3.1, les éléments du menu latéral étaient seulement bleus sur fond blanc, sans laisser la possibilité à l'utilisateur de changer la couleur d'accent. Depuis 3.1, choisir une couleur d'accent est possible.
