# Projet BDD_Projet_Jeux

## Introduction générale

Ce projet vise à illustrer la puissance de la démarche BDD (Behavior Driven Development) à travers la conception, la modélisation et l’implémentation de cinq jeux classiques : Bowling, Fléchettes, Mastermind, TicTacToe et Tennis. L’objectif est de démontrer que la réflexion sur les règles, la qualité des scénarios de tests et la robustesse de la logique métier priment sur la simple quantité de code produit. Le projet est réalisé en .NET Core/.NET Standard, avec des tests automatisés via SpecFlow (Gherkin), garantissant portabilité, maintenabilité et évolutivité.

---

## 1. Présentation détaillée des jeux et analyse des règles

### Bowling
Le bowling se joue en 10 frames, chaque joueur lançant deux boules par frame (sauf strike). Les règles incluent la gestion des strikes, spares, bonus au dernier frame, et la détection des lancers invalides (plus de 10 quilles, score négatif, etc.).

**Modélisation :**
- Classe `BowlingGame` gérant la liste des joueurs, le frame courant, la détection de la fin de partie.
- Classe `BowlingPlayer` pour le score individuel, la gestion des frames et des lancers.
- Gestion des cas limites (strike/spare au dernier frame, score parfait, score minimum, etc.).

**Exemple de scénario Gherkin :**
```gherkin
Scenario: Spare au dernier frame
  Etant donné le joueur 1 est au 10ème frame
  Quand il fait 5 puis 5
  Et il fait 7
  Alors son score pour ce frame doit être 17
```

**StepDefinition associée :**
```csharp
[When("il fait (\\d+) puis (\\d+)")]
public void PlayerRollsSequence(int first, int second)
{
    var player = _game.GetCurrentState().CurrentPlayer;
    PlayerRolls(int.Parse(player.Split(' ')[1]), first);
    PlayerRolls(int.Parse(player.Split(' ')[1]), second);
}
```

### Fléchettes
Le jeu de fléchettes (501) consiste à réduire son score de 501 à 0, en respectant certaines règles (fin sur double, pas de score négatif, dépassement interdit). Plusieurs joueurs peuvent participer.

**Modélisation :**
- Classe `FlechettesGame` gérant la liste des joueurs, le score, la règle "fin sur double".
- Classe `FlechettesPlayer` pour le score individuel.
- Gestion des cas limites (victoire pile à 0, dépassement, score négatif, etc.).

**Exemple de scénario Gherkin :**
```gherkin
Scenario: Règle "fin sur double" non respectée
  Etant donné une partie avec la règle "Fin sur double"
  Et le joueur 1 a 20 points restants
  Quand il marque 20 points (simple)
  Alors son score doit rester à 20
  Et la partie ne doit pas être terminée
```

**StepDefinition associée :**
```csharp
[When("le joueur (\\d+) marque (\\d+) points (simple)")]
public void PlayerScoresSimple(int playerNumber, int points)
{
    PlayerScores(playerNumber, points);
}
```

### Mastermind
Jeu de déduction où un joueur doit deviner un code secret en un nombre limité de tentatives. Chaque proposition reçoit un feedback (bien placés, mal placés). Gestion des entrées invalides, des doublons, et de la victoire/défaite.

**Modélisation :**
- Classe `MastermindGame` gérant le code secret, les tentatives, le feedback.
- Classe `Code` pour la génération et la validation du code secret.
- Gestion des cas limites (proposition invalide, doublons, victoire au dernier essai, etc.).

**Exemple de scénario Gherkin :**
```gherkin
Scenario: Proposition avec caractères interdits
  Quand je propose "R@BY"
  Alors le système doit rejeter la tentative
  Et afficher "Couleurs valides: R,G,B,Y,O,P"
```

**StepDefinition associée :**
```csharp
[Then("afficher \"([^\"]*)\"")]
public void VerifyErrorMessage(string expectedMessage)
{
    _lastResult.Message.Should().Be(expectedMessage);
}
```

### TicTacToe
Jeu de morpion classique sur une grille 3x3. Deux joueurs s’affrontent pour aligner trois symboles. Gestion des égalités, des victoires, des coups invalides (case occupée, hors grille), et de la détection de la fin de partie.

**Modélisation :**
- Classe `TicTacToe` gérant la grille, le joueur courant, la détection de victoire ou d’égalité.
- Enum `GameResult` pour l’état de la partie (en cours, X gagne, O gagne, nul).
- Gestion des coups invalides, de la fin de partie, et de la réinitialisation.

**Exemple de scénario Gherkin :**
```gherkin
Scenario: Coup sur case déjà occupée
  Quand X joue en (0,0)
  Et O joue en (0,0)
  Alors le système doit rejeter la tentative
  Et afficher "Case déjà occupée"
```

**StepDefinition associée :**
```csharp
[When("([XO]) joue en \((\\d+),(\\d+)\)")]
public void Jouer(string joueur, int x, int y)
{
    try { _lastResult = _game.PlayTurn(joueur, x, y); }
    catch (Exception ex) { _lastResult = new UGameResult { IsValid = false, Message = ex.Message }; }
}
```

### Tennis
Simulation d’un match de tennis avec gestion du score (15/30/40/Avantage), des sets, tie-break, et des règles de victoire. Gestion des changements de serveur, des égalités, et des cas d’erreur (score impossible, partie terminée).

**Modélisation :**
- Classe `Match` gérant les joueurs, le score, les sets, le tie-break.
- Classe `PlayerTennis` pour le score individuel.
- Gestion des cas limites (tie-break, avantage, changement de serveur, etc.).

**Exemple de scénario Gherkin :**
```gherkin
Scenario: Tie-break
  Quand le score est 6-6
  Et le joueur 1 marque un point
  Alors le score du tie-break doit être 1-0
```

**StepDefinition associée :**
```csharp
[When("le joueur (\\d+) marque un point")]
public void PlayerScores(int playerNumber)
{
    _lastResult = _game.PlayTurn(playerNumber);
}
```

---

## 2. Démarche BDD appliquée et stratégie de tests

### Analyse des règles et identification des comportements
Pour chaque jeu, une analyse détaillée des règles officielles a été menée afin d’identifier :
- Les comportements attendus (victoire, déroulement normal, erreurs)
- Les cas limites et situations exceptionnelles
- Les interactions entre joueurs

### Rédaction des scénarios Gherkin
Des scénarios Gherkin exhaustifs ont été rédigés pour chaque jeu, couvrant :
- Les cas d’usage standards
- Les cas limites et d’erreur
- Les conditions de victoire, d’égalité, de fin de partie

**Conseil pédagogique :**
> Avant d’implémenter, relisez et enrichissez vos scénarios. Un bon scénario BDD doit être compréhensible par un non-développeur et couvrir un comportement métier précis.

### Stratégie de couverture des tests
L’objectif a été de garantir que chaque règle métier, chaque cas limite, et chaque erreur possible soit couvert par au moins un scénario. Les scénarios sont relus et enrichis avant toute implémentation.

**Encadré :**
> La qualité des scénarios prime sur la quantité de code. Un projet BDD réussi se mesure à la pertinence et à la complétude de ses tests.

---

## 3. Architecture technique, organisation du code et validation métier

### Structure des dossiers
- Un dossier par jeu, contenant la logique métier, les entités et les règles spécifiques
- Un dossier `Utilities` pour les classes partagées (résultat de partie, état, etc.)
- Un projet de tests BDD séparé, avec un dossier `Features` (scénarios Gherkin) et un dossier `Steps` (StepDefinitions)

### Interface commune et extensibilité
Tous les jeux implémentent l’interface `IGame`, garantissant une intégration homogène et facilitant l’écriture de tests génériques. L’architecture permet d’ajouter facilement de nouveaux jeux ou de nouvelles règles, chaque jeu étant isolé dans son propre espace de noms.

### Choix techniques
- **.NET Core/Standard** : portabilité, simplicité, intégration facile des tests
- **SpecFlow** : rédaction des scénarios en Gherkin, automatisation des tests
- **FluentAssertions** : assertions lisibles et robustes dans les tests

### Validation métier et gestion des erreurs
Chaque méthode métier valide systématiquement les entrées (ex : score négatif, coup hors grille, code invalide) et lève des exceptions ou retourne des résultats invalides avec un message explicite. Les StepDefinitions capturent ces erreurs et les vérifient dans les assertions.

**Exemple :**
```csharp
[Then("le système doit rejeter la tentative")]
public void VerifyInvalidAttempt()
{
    _lastResult.IsValid.Should().BeFalse();
}
```

---

## 4. Collaboration, gestion de projet et amélioration continue

### Travail en groupe
Le projet est conçu pour être réalisé en groupe (2-3 personnes). Chaque membre peut se spécialiser sur un jeu, ce qui permet :
- Une meilleure répartition des tâches
- Une expertise approfondie sur les règles de chaque jeu
- Une gestion efficace des merges et des conflits

### Gestion des conflits et relecture
La communication et la relecture des scénarios sont essentielles pour garantir la cohérence et la qualité globale. Les conflits de merge sont résolus en priorisant la logique métier et la clarté des scénarios.

### Amélioration continue
Le projet encourage l’ajout régulier de nouveaux scénarios, la refactorisation du code métier, et l’extension à de nouveaux jeux ou variantes de règles. Les tests BDD servent de filet de sécurité pour toute évolution.

**Conseil :**
> N’hésitez pas à enrichir vos scénarios au fil du développement. Toute règle métier doit être testée !

---

## 5. Perspectives et axes d’amélioration

### Extensions possibles
- Ajout de nouveaux jeux ou variantes de règles (ex : variantes du morpion, bowling à 5 quilles, etc.)
- Génération automatique de rapports de tests BDD
- Renforcement de la couverture de tests sur les cas extrêmes
- Documentation technique et fonctionnelle enrichie

### Conseils pour la suite
- Continuez à enrichir vos scénarios et à tester chaque règle métier
- Privilégiez la clarté, la robustesse et la maintenabilité du code
- Documentez vos choix et vos arbitrages pour faciliter la reprise du projet

---

## Conclusion

Ce projet est un exemple concret de l’apport du BDD dans la conception logicielle, où la réflexion et la qualité des scénarios priment sur la quantité de code. Il offre une base solide pour tout développement logiciel centré sur l’utilisateur et la robustesse métier. Le respect du cycle BDD garantit un code fiable, testé et conforme aux attentes des utilisateurs finaux.

**En résumé :**
- Analyse approfondie des règles de chaque jeu
- Rédaction de scénarios Gherkin exhaustifs
- Implémentation métier robuste et validée par les tests
- Collaboration efficace et amélioration continue

> La réussite d’un projet BDD ne se mesure pas au nombre de lignes de code, mais à la pertinence et à la couverture de ses scénarios ! 