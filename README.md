# Documentation et Justification des Choix Techniques
## Projet BDD - Librairie de Jeux

## Introduction

Ce document présente l'analyse et la justification des choix techniques pour une librairie de jeux BDD implémentant TicTacToe, Tennis, Fléchettes, Mastermind et Bowling. L'approche BDD a guidé le développement en plaçant les comportements métier au centre de la conception.

---

## 1. Analyse et Justification des Scénarios

### 1.1 Identification des Cas de Test

**Cas Nominaux :** Déroulement standard des parties
- TicTacToe : Mouvements valides, alternance des joueurs
- Tennis : Progression 0-15-30-40, jeux et sets
- Fléchettes : Réduction de 501 vers zéro
- Mastermind : Propositions avec feedback approprié
- Bowling : Lancers standards, calcul par frame

**Cas Limites :** Situations particulières mais légales
- TicTacToe : Match nul, grille pleine
- Tennis : Tie-breaks, égalités, avantages
- Fléchettes : Score exact zéro, "fin sur double"
- Mastermind : Victoire au dernier essai, codes doublons
- Bowling : Strikes/spares, 10ème frame spécial

**Cas d'Erreur :** Validation des règles métier
- Transversaux : Jeu après fin de partie
- TicTacToe : Case occupée, position invalide
- Fléchettes : Score négatif/excédentaire
- Mastermind : Code invalide, longueur incorrecte
- Bowling : Quilles invalides (<0 ou >10)

**Justifications Spécifiques :**
- **Tennis** : Complexité règlementaire élevée, situations d'égalité critiques modifiant la logique de score
- **Mastermind** : Algorithme d'évaluation complexe (pions placés vs couleurs correctes)
- **Bowling** : Interdépendances entre frames, strikes/spares affectent les calculs suivants

### 1.2 Priorisation des Scénarios

**Priorité 1 (Critiques) :**
1. Cas nominaux fondamentaux
2. Logique de fin de partie
3. Gestion des erreurs essentielles

**Priorité 2 (Secondaires) :**
1. Cas limites complexes
2. Cas spéciaux et optimisations
3. Extensions fonctionnelles

Cette priorisation permet de se concentrer sur les cas principaux avant ceux plus complexes et exceptionnels.

---

## 2. Architecture et Représentation des Données

### 2.1 Lisibilité des Données de Test

**Background pour l'Initialisation :**
```gherkin
Background:
    Etant donné une nouvelle partie de bowling avec 2 joueurs
```
Évite la répétition, améliore la lisibilité et garantit une configuration cohérente.

**Tables pour Séquences Complexes :**
```gherkin
Quand j'effectue 10 propositions incorrectes:
| Proposition |
| OOOO        |
| PPPP        |
```
Rendent explicites les séquences d'actions, particulièrement utiles pour Mastermind et TicTacToe.

**Scenario Outline pour Généralisation :**
```gherkin
Examples:
    | score_initial | joueur   | score_final |
    | 0-0          | joueur 1 | 15-0        |
    | 15-0         | joueur 2 | 15-15       |
```
Évite la duplication tout en maintenant une couverture exhaustive.

### 2.2 Extensibilité

**Interface IGame Commune :**
```csharp
public interface IGame
{
    string GameName { get; }
    void Initialize(int playerCount);
    UGameResult PlayTurn(params object[] inputs);
    GameState GetCurrentState();
}
```

Bénéfices :
- Ajout facile de nouveaux jeux
- Polymorphisme et traitement uniforme

**Couche Métier Pure** : Classes dédiées (TicTacToe, Match, Code) sans dépendances

Facilite :
- Réutilisabilité

---

## 3. Stratégie BDD et Bonnes Pratiques

### 3.1 Langage Ubiquitaire

**Vocabulaire Métier :**

Tennis :
```gherkin
Alors le score du jeu devient Avantage Joueur 1
Et un tie-break commence
```

Bowling :
```gherkin
Quand le joueur 1 fait un lancer de 7
Et un lancer de 3 (spare)
```

Mastermind :
```gherkin
Alors je devrais recevoir "4 pions bien placés"
```

Cette approche garantit que les tests servent de documentation compréhensible.

### 3.2 Réutilisabilité

**Step Definitions Communes :**
```csharp
[Then(@"une erreur ""(.*)"" est levée")]
public void ThenUneErreurEstLevee(string messageErreur)
```
Centralise des comportements transversaux via `SharedStepDefinitions`.

**Step Definitions Spécifiques :**
Chaque jeu maintient ses définitions pour :
- Précision du vocabulaire métier
- Logique métier complexe
- Maintenabilité optimisée

### 3.3 Maintenance

**Organisation Modulaire :**

Dossier :
- Features
- Steps
- Games avec un dossier pour chaque jeu contenant toutes les classes nécessaires
- Utilities pour des classes utilitaires centralisées (validité, gagnants, état du jeu)

**Avantages :**
1. Isolation des changements
2. Responsabilités claires

---

## Conclusion

Cette approche garantit une librairie robuste, évolutive et alignée sur les besoins métier, servant de documentation pour les règles de chaque jeu. L'utilisation des principes BDD assure la correspondance entre les attentes métier et l'implémentation technique.
