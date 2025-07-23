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
      | 0-0          | joueur 2 | 0-15        |
      | 0-15         | joueur 1 | 15-15       |
      | 15-30        | joueur 1 | 30-30       |
      | 30-40        | joueur 2 | Jeu gagné   |

  Scenario Outline: Situations d'égalité et avantage
    Given le score du jeu est <score_initial>
    When le <joueur> marque un point
    Then le score du jeu devient <score_final>

    Examples:
      | score_initial        | joueur   | score_final          |
      | 40-40 (Égalité)     | joueur 1 | Avantage Joueur 1    |
      | 40-40 (Égalité)     | joueur 2 | Avantage Joueur 2    |
      | Avantage Joueur 1   | joueur 2 | 40-40 (Égalité)      |
      | Avantage Joueur 2   | joueur 1 | 40-40 (Égalité)      |
      | Avantage Joueur 1   | joueur 1 | Jeu gagné            |
      | Avantage Joueur 2   | joueur 2 | Jeu gagné            |

  # Scénarios pour les sets
  Scenario Outline: Différents scores de sets gagnants
    Given le score des jeux est <jeux_avant>
    When le <gagnant> remporte le jeu suivant
    Then le <gagnant> remporte le set avec un score de <score_set>
    And le score des sets devient <nouveau_score_sets>
    And le score des jeux revient à 0-0

    Examples:
      | jeux_avant | gagnant  | score_set | nouveau_score_sets |
      | 5-0        | joueur 1 | 6-0       | 1-0                |
      | 5-1        | joueur 1 | 6-1       | 1-0                |
      | 5-2        | joueur 1 | 6-2       | 1-0                |
      | 5-3        | joueur 1 | 6-3       | 1-0                |
      | 5-4        | joueur 1 | 6-4       | 1-0                |
      | 6-5        | joueur 1 | 7-5       | 1-0                |
      | 0-5        | joueur 2 | 0-6       | 0-1                |
      | 1-5        | joueur 2 | 1-6       | 0-1                |
      | 4-5        | joueur 2 | 4-6       | 0-1                |
      | 5-6        | joueur 2 | 5-7       | 0-1                |

  Scenario: Set avec tie-break - déclenchement automatique
    Given le score des jeux est 6-6
    Then un tie-break commence
    And le score du tie-break est 0-0

  # Scénarios pour les tie-breaks
  Scenario Outline: Progression des points dans un tie-break
    Given un tie-break est en cours
    And le score du tie-break est <score_initial>
    When le <joueur> marque un point dans le tie-break
    Then le score du tie-break devient <score_final>

    Examples:
      | score_initial | joueur   | score_final |
      | 0-0          | joueur 1 | 1-0         |
      | 1-0          | joueur 2 | 1-1         |
      | 3-2          | joueur 1 | 4-2         |
      | 5-4          | joueur 2 | 5-5         |
      | 6-4          | joueur 1 | 7-4         |

  Scenario Outline: Fin de tie-break - victoires normales
    Given un tie-break est en cours
    And le score du tie-break est <score_avant>
    When le <gagnant> marque un point dans le tie-break
    Then le <gagnant> remporte le tie-break <score_final>
    And le <gagnant> remporte le set 7-6

    Examples:
      | score_avant | gagnant  | score_final |
      | 6-0         | joueur 1 | 7-0         |
      | 6-1         | joueur 1 | 7-1         |
      | 6-2         | joueur 1 | 7-2         |
      | 6-3         | joueur 1 | 7-3         |
      | 6-4         | joueur 1 | 7-4         |
      | 6-5         | joueur 1 | 7-5         |
      | 0-6         | joueur 2 | 0-7         |
      | 3-6         | joueur 2 | 3-7         |
      | 5-6         | joueur 2 | 5-7         |

  Scenario Outline: Tie-break avec égalité et avantage
    Given un tie-break est en cours
    And le score du tie-break est <score_initial>
    When le <joueur> marque un point dans le tie-break
    Then le score du tie-break devient "<score_final>"

    Examples:
      | score_initial | joueur   | score_final           |
      | 6-6          | joueur 1 | 7-6 Avantage Joueur 1 |
      | 6-6          | joueur 2 | 6-7 Avantage Joueur 2 |
      | 7-7          | joueur 1 | 8-7 Avantage Joueur 1 |
      | 7-7          | joueur 2 | 7-8 Avantage Joueur 2 |
      | 8-8          | joueur 1 | 9-8 Avantage Joueur 1 |

  Scenario Outline: Retour à l'égalité dans un tie-break
    Given un tie-break est en cours
    And le score du tie-break est <score_avec_avantage>
    When le <joueur_rattrape> marque un point dans le tie-break
    Then le score du tie-break revient à <score_egalite>

    Examples:
      | score_avec_avantage | joueur_rattrape | score_egalite     |
      | 7-6                | joueur 2        | 7-7 (Égalité)     |
      | 6-7                | joueur 1        | 7-7 (Égalité)     |
      | 8-7                | joueur 2        | 8-8 (Égalité)     |
      | 7-8                | joueur 1        | 8-8 (Égalité)     |

  Scenario Outline: Victoire après avantage dans un tie-break
    Given un tie-break est en cours
    And le score du tie-break est <score_avec_avantage>
    When le <gagnant> marque un point dans le tie-break
    Then le <gagnant> remporte le tie-break <score_final>

    Examples:
      | score_avec_avantage | gagnant  | score_final |
      | 7-6                | joueur 1 | 8-6         |
      | 6-7                | joueur 2 | 6-8         |
      | 8-7                | joueur 1 | 9-7         |
      | 7-8                | joueur 2 | 7-9         |
      | 9-8                | joueur 1 | 10-8        |

  # Scénarios pour les matchs
  Scenario Outline: Victoires de match selon différents scores
    Given le score des sets est <score_sets_avant>
    When le <gagnant> remporte le set <numero_set>
    Then le <gagnant> remporte le match
    And le score final du match est <score_final>

    Examples:
      | score_sets_avant | gagnant  | numero_set | score_final |
      | 1-0             | joueur 1 | 2          | 2-0         |
      | 0-1             | joueur 2 | 2          | 0-2         |
      | 1-1             | joueur 1 | 3          | 2-1         |
      | 1-1             | joueur 2 | 3          | 1-2         |

  # Scénarios d'erreur et cas limites
  Scenario Outline: Tentatives de marquer des points après la fin du match
    Given le match est terminé
    And le joueur <gagnant> a gagné <score_final>
    When on tente de faire marquer un point au joueur <tentative_joueur>
    Then une erreur "Match déjà terminé" est levée

    Examples:
      | gagnant  | score_final | tentative_joueur |
      | 1        | 2-0         | 1                |
      | 1        | 2-0         | 2                |
      | 2        | 0-2         | 1                |
      | 2        | 0-2         | 2                |
      | 1        | 2-1         | 1                |
      | 1        | 2-1         | 2                |

  Scenario Outline: Vérification de l'état du match en cours
    Given le score des sets est <score_sets>
    And le score des jeux est <score_jeux>
    When on demande l'état du match
    Then le match est en cours
    And le set actuel est le set numéro <numero_set>

    Examples:
      | score_sets | score_jeux | numero_set |
      | 0-0        | 0-0        | 1          |
      | 0-0        | 3-2        | 1          |
      | 1-0        | 2-4        | 2          |
      | 0-1        | 1-1        | 2          |
      | 1-1        | 3-2        | 3          |
      | 1-1        | 0-6        | 3          |


  # Scénarios spécifiques pour cas complexes
  Scenario: Jeu normal - victoire simple détaillée
    Given le score du jeu est 30-15
    When le joueur 1 marque un point
    Then le score du jeu devient 40-15
    When le joueur 1 marque un point
    Then le joueur 1 remporte le jeu
    And le score des jeux devient 1-0
    And le score des points revient à 0-0

  Scenario: Séquence complète d'égalité et avantage
    Given le score du jeu est 40-40 (Égalité)
    When le joueur 1 marque un point
    Then le score du jeu devient Avantage Joueur 1
    When le joueur 2 marque un point
    Then le score du jeu revient à 40-40 (Égalité)
    When le joueur 2 marque un point
    Then le score du jeu devient Avantage Joueur 2
    When le joueur 2 marque un point
    Then le joueur 2 remporte le jeu

  Scenario: Tie-break complet avec égalités multiples
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