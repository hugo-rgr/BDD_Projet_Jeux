Feature: Tennis (deux sets gagnants)
  En tant que système de gestion de match de tennis
  Je veux pouvoir gérer une partie de tennis avec les règles officielles
  Afin de suivre correctement le score et déterminer le vainqueur
  Règles :
  - Deux joueurs
  - Le match est initialisé à 0 points, 0 jeux et 0 sets pour les deux joueurs
  Un jeu :
  - Le premier point est 15, deuxième est 30, le troisième est 40
  - Le joueur gagne le jeu s'il marque un quatrième point et que l'autre joueur est à moins de 40
  - Si les deux joueurs sont à 40, ils sont dits à Egalité
  - S'il y a Egalité, alors le quatrième point est appelé Avantage
  - S'il y a Avantage, alors le cinquième point permet de gagner le jeu
  - S'il y a Avantage et que l'autre joueur marque un point, alors on revient à Egalité 
  Un set :
  - Pour remporter un set, il faut gagner 6 jeux
  - Si un des joueurs à remporté 5 jeux et à condition d'en avoir deux d'avance sur son adversaire
  - Si les deux joueurs sont à 6 jeux, on entre dans un jeu pour départager : le Tie Break.
  Tie break :    
  - Les points sont comptés de 0 à 7
  - Il faut 7 points pour remporter le jeu et le set
  - Si les deux joueurs ont 6 points, il y a Egalité
  - S'il y a Egalité, alors le septième point est appelé Avantage
  - S'il y a Avantage, alors le huitième point permet de gagner le jeu
  - S'il y a Avantage et que l'autre joueur marque un point, alors on revient à Egalité 
  Un match :
  - Gagner 2 sets (maximum de 3 sets dans le match entier) 

  Background:
    Given un nouveau match de tennis est initialisé
    And le score initial est 0-0 en points, 0-0 en jeux et 0-0 en sets

  # Scénarios pour les jeux
  Scenario: Jeu normal - victoire simple
    Given le score du jeu est 30-15
    When le joueur 1 marque un point
    Then le score du jeu devient 40-15
    When le joueur 1 marque un point
    Then le joueur 1 remporte le jeu
    And le score des jeux devient 1-0
    And le score des points revient à 0-0

  Scenario: Jeu avec égalité et avantage
    Given le score du jeu est 40-40 (Égalité)
    When le joueur 1 marque un point
    Then le score du jeu devient Avantage Joueur 1
    When le joueur 2 marque un point
    Then le score du jeu revient à 40-40 (Égalité)
    When le joueur 2 marque un point
    Then le score du jeu devient Avantage Joueur 2
    When le joueur 2 marque un point
    Then le joueur 2 remporte le jeu

  Scenario: Évolution des points dans un jeu
    Given le jeu vient de commencer
    When le joueur 1 marque un point
    Then le score du jeu devient 15-0
    When le joueur 2 marque un point
    Then le score du jeu devient 15-15
    When le joueur 1 marque un point
    Then le score du jeu devient 30-15
    When le joueur 2 marque un point
    Then le score du jeu devient 30-30

  # Scénarios pour les sets
  Scenario: Set gagné normalement (6-4)
    Given le score des jeux est 5-4
    When le joueur 1 remporte le jeu suivant
    Then le joueur 1 remporte le set
    And le score des sets devient 1-0
    And le score des jeux revient à 0-0

  Scenario: Set gagné avec un écart de 2 jeux (7-5)
    Given le score des jeux est 6-5
    When le joueur 1 remporte le jeu suivant
    Then le joueur 1 remporte le set avec un score de 7-5

  Scenario: Set avec tie-break
    Given le score des jeux est 6-6
    Then un tie-break commence
    And le score du tie-break est 0-0

  Scenario: Tie-break normal
    Given un tie-break est en cours
    And le score du tie-break est 6-5
    When le joueur 1 marque un point dans le tie-break
    Then le joueur 1 remporte le tie-break 7-5
    And le joueur 1 remporte le set 7-6

  Scenario: Tie-break avec égalité et avantage
    Given un tie-break est en cours
    And le score du tie-break est 6-6
    When le joueur 1 marque un point dans le tie-break
    Then le score du tie-break devient "7-6 Avantage Joueur 1"
    When le joueur 2 marque un point dans le tie-break
    Then le score du tie-break revient à 7-7 (Égalité)
    When le joueur 1 marque un point dans le tie-break
    Then le score du tie-break devient "8-7 Avantage Joueur 1"
    When le joueur 1 marque un point dans le tie-break
    Then le joueur 1 remporte le tie-break 9-7

  # Scénarios pour les matchs
  Scenario: Match gagné 2-0
    Given le joueur 1 a remporté le set 1
    And le score des sets est 1-0
    When le joueur 1 remporte le set 2
    Then le joueur 1 remporte le match
    And le score final du match est 2-0

  Scenario: Match gagné 2-1
    Given le joueur 1 a remporté le set 1
    And le joueur 2 a remporté le set 2
    And le score des sets est 1-1
    When le joueur 1 remporte le set 3
    Then le joueur 1 remporte le match
    And le score final du match est 2-1

  # Scénarios d'erreur et cas limites
  Scenario: Tentative de marquer un point après la fin du match
    Given le match est terminé
    And le joueur 1 a gagné 2-0
    When on tente de faire marquer un point au joueur 2
    Then une erreur "Match déjà terminé" est levée

  Scenario: Vérification de l'état du match en cours
    Given le score des sets est 1-1
    And le score des jeux est 3-2
    When on demande l'état du match
    Then le match est en cours
    And le set actuel est le set numéro 3

  # Exemples avec tables de données
  Scenario Outline: Progression des points dans un jeu
    Given le score du jeu est <score_initial>
    When le <joueur> marque un point
    Then le score du jeu devient <score_final>

    Examples:
      | score_initial | joueur   | score_final |
      | 0-0          | joueur 1 | 15-0        |
      | 15-0         | joueur 2 | 15-15       |
      | 15-15        | joueur 1 | 30-15       |
      | 30-15        | joueur 2 | 30-30       |
      | 30-30        | joueur 1 | 40-30       |
      | 40-30        | joueur 1 | Jeu gagné   |

  Scenario Outline: Différents scores de sets gagnants
    Given le score des jeux est <jeux_avant>
    When le <gagnant> remporte le jeu suivant
    Then le <gagnant> remporte le set avec un score de <score_set>

    Examples:
      | jeux_avant | gagnant  | score_set |
      | 5-0        | joueur 1 | 6-0       |
      | 5-1        | joueur 1 | 6-1       |
      | 5-4        | joueur 1 | 6-4       |
      | 6-5        | joueur 1 | 7-5       |