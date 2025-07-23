@flechettes
Feature: Gestion complète des parties de fléchettes
Tests couvrant toutes les règles du jeu 501 points

    Background:
    Etant donné une nouvelle partie de fléchettes avec 2 joueurs

    # Cas standards
    Scenario: Tour simple sans victoire
    Quand le joueur 1 marque 20 points
    Alors son score doit être 481
    Et le prochain joueur doit être le joueur 2

    Scenario: Victoire par score exact
    Quand le joueur 1 marque 500 points
    Et le joueur 2 marque 400 points
    Et le joueur 1 marque 1 point
    Alors la partie doit être terminée
    Et le joueur 1 doit être déclaré vainqueur

    # Cas limites
    Scenario: Score excédentaire simple
    Etant donné le joueur 1 a 10 points restants
    Quand il marque 15 points
    Alors son score doit rester à 10
    Et le tour doit passer au joueur 2

    Scenario: Double score excédentaire
    Etant donné le joueur 1 a 5 points restants
    Quand il marque 20 points
    Et il marque 3 points
    Alors son score doit rester à 5
    Et le tour doit passer au joueur 2

    # Règles spéciales
    Scenario: Fin sur double activée
    Etant donné une partie avec la règle "Fin sur double"
    Quand le joueur 1 a 40 points restants
    Et il marque 20 points (simple)
    Et il marque 20 points (double)
    Alors son score final doit être 0
    Et la partie doit être terminée

    # Gestion d'erreur
    Scenario: Score négatif
    Quand le joueur 1 marque -5 points
    Alors le système doit rejeter le score
    Et afficher "Score invalide"

    # Cas supplémentaires
    Scenario: Victoire pile à 0
    Etant donné le joueur 1 a 10 points restants
    Quand il marque 10 points
    Alors la partie doit être terminée
    Et le joueur 1 doit être déclaré vainqueur

    Scenario: Score qui dépasse 0
    Etant donné le joueur 1 a 5 points restants
    Quand il marque 6 points
    Alors son score doit rester à 5
    Et le tour doit passer au joueur 2

    Scenario: Score de 0 dès le début
    Etant donné le joueur 1 a 0 points restants
    Alors la partie doit être terminée
    Et le joueur 1 doit être déclaré vainqueur

    Scenario: Règle "fin sur double" non respectée
    Etant donné une partie avec la règle "Fin sur double"
    Et le joueur 1 a 20 points restants
    Quand il marque 20 points (simple)
    Alors son score doit rester à 20
    Et la partie ne doit pas être terminée

    Scenario: Partie à 3 joueurs
    Etant donné une nouvelle partie de fléchettes avec 3 joueurs
    Quand le joueur 1 marque 100 points
    Et le joueur 2 marque 150 points
    Et le joueur 3 marque 200 points
    Alors le tour doit passer au joueur 1