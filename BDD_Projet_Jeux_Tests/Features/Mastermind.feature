@mastermind
Feature: Gestion complète du Mastermind
Tests couvrant toutes les mécaniques du jeu

    Background:
    Etant donné une nouvelle partie de Mastermind avec le code secret "RGBY"

    # Cas standards
    Scenario: Proposition exacte
    Quand je propose "RGBY"
    Alors je devrais recevoir "4 pions bien placés"
    Et la partie doit être terminée
    Et je devrais être déclaré vainqueur

    Scenario: Proposition partielle
    Quand je propose "RGYB"
    Alors je devrais recevoir "2 pions bien placés, 2 couleurs correctes"

    # Cas limites
    Scenario: Tentative invalide
    Quand je propose "ABCD"
    Alors le système doit rejeter la tentative
    Et afficher "Couleurs valides: R,G,B,Y,O,P"

    Scenario: Longueur incorrecte
    Quand je propose "RGB"
    Alors le système doit rejeter la tentative
    Et afficher "Le code doit faire 4 caractères"

    # Fin de partie
    Scenario: Épuisement des tentatives
    Quand j'effectue 10 propositions incorrectes:
    | Proposition |
    | OOOO        |
    | PPPP        |
    | RRRR        |
    | GGGG        |
    | BBBB        |
    | YYYY        |
    | ROOO        |
    | GOOO        |
    | BOOO        |
    | YOOO        |
    Alors la partie doit être terminée
    Et le créateur du code doit gagner

    # Cas supplémentaires
    Scenario: Proposition avec caractères interdits
    Quand je propose "R@BY"
    Alors le système doit rejeter la tentative
    Et afficher "Couleurs valides: R,G,B,Y,O,P"

    Scenario: Proposition avec doublons valides
    Quand je propose "RRGG"
    Alors je devrais recevoir "2 pions bien placés, 2 couleurs correctes"

    Scenario: Victoire au dernier essai
    Quand j'effectue 9 propositions incorrectes:
    | Proposition |
    | OOOO        |
    | PPPP        |
    | RRRR        |
    | GGGG        |
    | BBBB        |
    | YYYY        |
    | ROOO        |
    | GOOO        |
    | BOOO        |
    Et je propose "RGBY"
    Alors la partie doit être terminée
    Et je devrais être déclaré vainqueur

    Scenario: Partie déjà terminée
    Quand j'effectue 10 propositions incorrectes:
    | Proposition |
    | OOOO        |
    | PPPP        |
    | RRRR        |
    | GGGG        |
    | BBBB        |
    | YYYY        |
    | ROOO        |
    | GOOO        |
    | BOOO        |
    | YOOO        |
    Et je propose "RGBY"
    Alors le système doit rejeter la tentative
    Et afficher "Partie terminée"

    Scenario: Code secret avec doublons
    Etant donné une nouvelle partie de Mastermind avec le code secret "RRGG"
    Quand je propose "RGRG"
    Alors je devrais recevoir "2 pions bien placés, 2 couleurs correctes"