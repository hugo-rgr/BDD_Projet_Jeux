@bowling
Feature: Gestion complète des parties de bowling
Tests couvrant toutes les règles du bowling 10 quilles

    Background:
    Etant donné une nouvelle partie de bowling avec 2 joueurs

    # Cas standards
    Scenario: Frame simple
    Quand le joueur 1 fait un lancer de 7
    Et un lancer de 2
    Alors son score pour ce frame doit être 9
    Et son score total doit être 9

    Scenario: Spare simple
    Quand le joueur 1 fait un lancer de 7
    Et un lancer de 3 (spare)
    Et au frame suivant il fait 5
    Alors son score pour le premier frame doit être 15
    Et son score total doit être 20

    # Cas spéciaux
    Scenario: Strike parfait
    Quand le joueur 1 fait 12 strikes consécutifs
    Alors son score total doit être 300
    Et la partie doit être terminée

    Scenario: Strike au dernier frame
    Etant donné le joueur 1 est au 10ème frame
    Quand il fait un strike
    Alors il doit avoir droit à 2 lancers supplémentaires
    Quand il fait 7 puis 2
    Alors son score pour ce frame doit être 19

    # Gestion des erreurs
    Scenario: Lancer invalide
    Quand le joueur 1 fait un lancer de 11
    Alors le système doit rejeter le lancer
    Et afficher "Nombre de quilles invalide"

    # Cas supplémentaires
    Scenario: Partie à un seul joueur
    Etant donné une nouvelle partie de bowling avec 1 joueurs
    Quand le joueur 1 fait un lancer de 5
    Alors son score total doit être 5

    Scenario: Score minimum
    Quand le joueur 1 fait 20 lancers de 0
    Alors son score total doit être 0
    Et la partie doit être terminée

    Scenario: Spare au dernier frame
    Etant donné le joueur 1 est au 10ème frame
    Quand il fait 5 puis 5
    Et il fait 7
    Alors son score pour ce frame doit être 17

    Scenario: Lancer négatif
    Quand le joueur 1 fait un lancer de -1
    Alors le système doit rejeter le lancer
    Et afficher "Nombre de quilles invalide"

    Scenario: Lancer après la fin de la partie
    Quand le joueur 1 fait 20 lancers de 0
    Et il fait un lancer de 5
    Alors le système doit rejeter le lancer
    Et afficher "Partie terminée"

    Scenario: Changement de joueur correct
    Quand le joueur 1 fait un lancer de 3
    Et le joueur 2 fait un lancer de 4
    Alors le prochain joueur doit être le joueur 1