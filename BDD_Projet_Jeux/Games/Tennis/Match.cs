using System;

namespace BDD_Projet_Jeux.Tennis
{
    public class Match
    {
        public PlayerTennis Player1 { get; set; }
        public PlayerTennis Player2 { get; set; }
        public MatchState State { get; set; }
        public bool IsTieBreak { get; set; }
        public int CurrentSet { get; set; }
        public PlayerTennis Winner { get; set; }
        public bool GameJustWon { get; set; }
        public PlayerTennis GameWinner { get; set; }
        public bool SetJustWon { get; set; }
        public PlayerTennis SetWinner { get; set; }

        public Match()
        {
            Player1 = new PlayerTennis();
            Player2 = new PlayerTennis();
            State = MatchState.InProgress;
            CurrentSet = 1;
            IsTieBreak = false;
            GameJustWon = false;
            SetJustWon = false;
        }

        public void PlayerScores(int playerNumber)
        {
            if (State == MatchState.Completed)
                throw new InvalidOperationException("Match déjà terminé");

            var scoringPlayer = playerNumber == 1 ? Player1 : Player2;
            var otherPlayer = playerNumber == 1 ? Player2 : Player1;

            if (IsTieBreak)
            {
                HandleTieBreakPoint(scoringPlayer, otherPlayer);
            }
            else
            {
                HandleGamePoint(scoringPlayer, otherPlayer);
            }
        }

        public void StartTieBreak()
        {
            IsTieBreak = true;
            Player1.ResetTieBreak();
            Player2.ResetTieBreak();
        }

        private void HandleGamePoint(PlayerTennis scoringPlayerTennis, PlayerTennis otherPlayerTennis)
        {
            // Si un joueur est à 40 et l'autre pas à 40, le premier gagne
            if (scoringPlayerTennis.GameScore == GameScore.Forty && otherPlayerTennis.GameScore != GameScore.Forty && !scoringPlayerTennis.HasAdvantage && !otherPlayerTennis.HasAdvantage)
            {
                WinGame(scoringPlayerTennis, otherPlayerTennis);
            }
            // Si les deux joueurs sont à 40 (situation d'égalité)
            else if (scoringPlayerTennis.GameScore == GameScore.Forty && otherPlayerTennis.GameScore == GameScore.Forty)
            {
                if (!scoringPlayerTennis.HasAdvantage && !otherPlayerTennis.HasAdvantage)
                {
                    // Premier avantage
                    scoringPlayerTennis.HasAdvantage = true;
                }
                else if (otherPlayerTennis.HasAdvantage)
                {
                    // Retour à égalité
                    otherPlayerTennis.HasAdvantage = false;
                }
                else if (scoringPlayerTennis.HasAdvantage)
                {
                    // Victoire après avantage
                    WinGame(scoringPlayerTennis, otherPlayerTennis);
                }
            }
            else
            {
                // Progression normale des points
                AdvanceGameScore(scoringPlayerTennis);
            }
        }

        private void HandleTieBreakPoint(PlayerTennis scoringPlayerTennis, PlayerTennis otherPlayerTennis)
        {
            scoringPlayerTennis.TieBreakScore++;

            // Vérification de victoire du tie-break
            if (scoringPlayerTennis.TieBreakScore >= 7 && scoringPlayerTennis.TieBreakScore - otherPlayerTennis.TieBreakScore >= 2)
            {
                WinTieBreak(scoringPlayerTennis, otherPlayerTennis);
            }
        }

        private void AdvanceGameScore(PlayerTennis playerTennis)
        {
            switch (playerTennis.GameScore)
            {
                case GameScore.Zero:
                    playerTennis.GameScore = GameScore.Fifteen;
                    break;
                case GameScore.Fifteen:
                    playerTennis.GameScore = GameScore.Thirty;
                    break;
                case GameScore.Thirty:
                    playerTennis.GameScore = GameScore.Forty;
                    break;
            }
        }

        private void WinGame(PlayerTennis winner, PlayerTennis loser)
        {
            GameJustWon = true;
            GameWinner = winner;
            winner.GamesWon++;
            winner.ResetGame();
            loser.ResetGame();

            CheckSetWin(winner, loser);
        }

        private void WinTieBreak(PlayerTennis winner, PlayerTennis loser)
        {
            SetJustWon = true;
            SetWinner = winner;
            winner.SetsWon++; // Gagner un tie-break = gagner le set directement
            IsTieBreak = false;
            winner.ResetTieBreak();
            loser.ResetTieBreak();
            
            // Reset seulement les jeux (pas les sets !) pour le prochain set
            winner.GamesWon = 0;
            loser.GamesWon = 0;
            winner.ResetGame();
            loser.ResetGame();

            // Vérification de victoire du match (2 sets gagnants)
            if (winner.SetsWon == 2)
            {
                Winner = winner;
                State = MatchState.Completed;
            }
            else
            {
                CurrentSet++;
            }
        }

        private void CheckSetWin(PlayerTennis winner, PlayerTennis loser)
        {
            // Victoire normale du set (6 jeux avec au moins 2 d'écart)
            if (winner.GamesWon >= 6 && winner.GamesWon - loser.GamesWon >= 2)
            {
                WinSet(winner, loser);
            }
            // Tie-break nécessaire
            else if (winner.GamesWon == 6 && loser.GamesWon == 6)
            {
                StartTieBreak();
            }
            // Victoire du set 7-5
            else if (winner.GamesWon == 7 && loser.GamesWon == 5)
            {
                WinSet(winner, loser);
            }
        }

        private void WinSet(PlayerTennis winner, PlayerTennis loser)
        {
            SetJustWon = true;
            SetWinner = winner;
            winner.SetsWon++;
            winner.ResetSet();
            loser.ResetSet();

            // Vérification de victoire du match (2 sets gagnants)
            if (winner.SetsWon == 2)
            {
                Winner = winner;
                State = MatchState.Completed;
            }
            else
            {
                CurrentSet++;
            }
        }

        public string GetGameScore()
        {
            if (GameJustWon)
            {
                return "Jeu gagné";
            }
            
            if (IsTieBreak)
            {
                if (Player1.TieBreakScore >= 6 && Player2.TieBreakScore >= 6)
                {
                    if (Player1.TieBreakScore == Player2.TieBreakScore)
                        return $"{Player1.TieBreakScore}-{Player2.TieBreakScore} (Égalité)";
                    else if (Player1.TieBreakScore > Player2.TieBreakScore)
                        return $"{Player1.TieBreakScore}-{Player2.TieBreakScore} Avantage Joueur 1";
                    else
                        return $"{Player1.TieBreakScore}-{Player2.TieBreakScore} Avantage Joueur 2";
                }
                return $"{Player1.TieBreakScore}-{Player2.TieBreakScore}";
            }

            if (Player1.HasAdvantage)
                return "Avantage Joueur 1";
            if (Player2.HasAdvantage)
                return "Avantage Joueur 2";
            if (Player1.GameScore == GameScore.Forty && Player2.GameScore == GameScore.Forty)
                return "40-40 (Égalité)";

            return $"{GetScoreDisplay(Player1.GameScore)}-{GetScoreDisplay(Player2.GameScore)}";
        }

        public string GetGamesScore()
        {
            return $"{Player1.GamesWon}-{Player2.GamesWon}";
        }

        public string GetSetsScore()
        {
            return $"{Player1.SetsWon}-{Player2.SetsWon}";
        }

        private string GetScoreDisplay(GameScore score)
        {
            return score switch
            {
                GameScore.Zero => "0",
                GameScore.Fifteen => "15",
                GameScore.Thirty => "30",
                GameScore.Forty => "40",
                _ => "0"
            };
        }
    }
}