Feature: TicTacToe
  Le morpion est un jeu simple avec les règles suivantes :
  - Grille 3x3
  - 2 joueurs (X et O)
  - Le joueur X commence toujours
  - Pour gagner : aligner 3 symboles (horizontalement, verticalement, ou diagonalement)
  - Match nul si toutes les cases sont remplies sans gagnant

  Background:
    Given une nouvelle partie de TicTacToe est créée
    And le joueur X commence la partie

  # Cas nominal - mouvement valide
  Scenario: Le joueur fait un mouvement valide
    When le joueur X joue en position (0,0)
    Then la case (0,0) contient le symbole X
    And c'est au tour du joueur O
    And la partie n'est pas terminée

  Scenario: Les joueurs alternent correctement
    Given le joueur X a joué en position (0,0)
    When le joueur O joue en position (1,1)
    Then la case (1,1) contient le symbole O
    And c'est au tour du joueur X

  # Cas d'erreur - mouvement invalide
  Scenario: Le joueur tente de jouer sur une case déjà occupée
    Given le joueur X a joué en position (0,0)
    When le joueur O tente de jouer en position (0,0)
    Then une erreur "Case déjà occupée" est levée
    And c'est toujours au tour du joueur O
    And la case (0,0) contient toujours le symbole X

  Scenario: Le joueur tente de jouer en dehors de la grille
    When le joueur X tente de jouer en position (3,3)
    Then une erreur "Position invalide" est levée
    And c'est toujours au tour du joueur X

  # Cas de victoire - Lignes horizontales
  Scenario: Le joueur gagne avec trois symboles alignés horizontalement
    Given les mouvements suivants ont été joués:
      | Joueur | Position |
      | X      | 0,0      |
      | O      | 1,0      |
      | X      | 0,1      |
      | O      | 1,1      |
      | X      | 0,2      |
    Then le joueur X a gagné
    And la partie est terminée
    And le motif gagnant est "ligne horizontale"

  # Cas de victoire - Lignes verticales
  Scenario: Le joueur O gagne avec une ligne verticale sur la colonne du milieu
    Given les mouvements suivants ont été joués:
      | Joueur | Position |
      | X      | 0,0      |
      | O      | 0,1      |
      | X      | 0,2      |
      | O      | 1,1      |
      | X      | 2,0      |
      | O      | 2,1      |
    Then le joueur O a gagné
    And la partie est terminée
    And le motif gagnant est "ligne verticale"

  # Cas de victoire - Diagonales
  Scenario: Le joueur X gagne avec la diagonale principale (haut-gauche vers bas-droite)
    Given les mouvements suivants ont été joués:
      | Joueur | Position |
      | X      | 0,0      |
      | O      | 0,1      |
      | X      | 1,1      |
      | O      | 0,2      |
      | X      | 2,2      |
    Then le joueur X a gagné
    And la partie est terminée
    And le motif gagnant est "diagonale principale"

  Scenario: Le joueur O gagne avec l'anti-diagonale (haut-droite vers bas-gauche)
    Given les mouvements suivants ont été joués:
      | Joueur | Position |
      | X      | 0,0      |
      | O      | 0,2      |
      | X      | 0,1      |
      | O      | 1,1      |
      | X      | 1,0      |
      | O      | 2,0      |
    Then le joueur O a gagné
    And la partie est terminée
    And le motif gagnant est "anti-diagonale"

  # Cas d'égalité
  Scenario: La partie se termine par une égalité quand la grille est pleine sans gagnant
    Given les mouvements suivants ont été joués:
      | Joueur | Position |
      | X      | 0,0      |
      | O      | 0,1      |
      | X      | 0,2      |
      | O      | 1,1      |
      | X      | 1,0      |
      | O      | 2,0      |
      | X      | 2,1      |
      | O      | 2,2      |
      | X      | 1,2      |
    Then la partie est terminée
    And le résultat est "match nul"
    
  # Cas d'erreur - partie terminée
  Scenario: Tentative de jouer après la fin de la partie
    Given le joueur X a gagné la partie
    When le joueur O tente de jouer en position (1,0)
    Then une erreur "Partie terminée" est levée
    And l'état de la grille n'a pas changé

  # Cas de récupération d'état
  Scenario: Récupération de l'état actuel de la partie
    Given les mouvements suivants ont été joués:
      | Joueur | Position |
      | X      | 0,0      |
      | O      | 1,1      |
      | X      | 0,1      |
    When je demande l'état de la partie
    Then la grille affiche:
      | col0 | col1 | col2 |
      | X    | X    |      |
      |      | O    |      |
      |      |      |      |
    And c'est au tour du joueur O
    And la partie n'est pas terminée