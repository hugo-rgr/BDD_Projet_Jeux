using System;
using BDD_Projet_Jeux.Games;
using BDD_Projet_Jeux.Utilities;

namespace BDD_Projet_Jeux.Tennis
{
    public class TennisGame : IGame
    {
        private Match _match;

        public string GameName => "Tennis";

        public void Initialize(int playerCount)
        {
            if (playerCount != 2)
                throw new ArgumentException("Le tennis nécessite exactement 2 joueurs");

            _match = new Match();
        }

        public UGameResult PlayTurn(params object[] inputs)
        {
            if (_match == null)
                throw new InvalidOperationException("Le jeu n'a pas été initialisé");

            if (_match.State == MatchState.Completed)
                throw new InvalidOperationException("Match déjà terminé");

            int playerNumber = (int)inputs[0];
            
            try
            {
                _match.PlayerScores(playerNumber);

                var result = new UGameResult
                {
                    IsValid = true,
                    IsGameOver = _match.State == MatchState.Completed
                };

                if (result.IsGameOver)
                {
                    result.Winner = _match.Winner == _match.Player1 ? "Joueur 1" : "Joueur 2";
                    result.Message = $"Match terminé - {result.Winner} gagne {_match.GetSetsScore()}";
                }
                else
                {
                    result.Message = $"Score: {_match.GetGameScore()} | Jeux: {_match.GetGamesScore()} | Sets: {_match.GetSetsScore()}";
                }

                return result;
            }
            catch (Exception ex)
            {
                return new UGameResult
                {
                    IsValid = false,
                    Message = ex.Message
                };
            }
        }

        public GameState GetCurrentState()
        {
            if (_match == null)
                return new GameState { IsGameOver = true };

            var state = new GameState
            {
                CurrentPlayer = "Joueur 1", // Tennis doesn't have turns in the same way
                IsGameOver = _match.State == MatchState.Completed
            };

            // Add player scores (sets won)
            state.Players.Add("Joueur 1", _match.Player1.SetsWon);
            state.Players.Add("Joueur 2", _match.Player2.SetsWon);

            return state;
        }

        // Expose the internal Match for backward compatibility with existing tests
        public Match GetMatch()
        {
            return _match;
        }

        // Helper methods for tests
        public void SetGameScore(string score)
        {
            // Implementation for setting specific game scores for testing
            if (score.Contains("Égalité") || score == "40-40 (Égalité)")
            {
                _match.Player1.GameScore = GameScore.Forty;
                _match.Player2.GameScore = GameScore.Forty;
                _match.Player1.HasAdvantage = false;
                _match.Player2.HasAdvantage = false;
            }
            else if (score.Contains("Avantage"))
            {
                _match.Player1.GameScore = GameScore.Forty;
                _match.Player2.GameScore = GameScore.Forty;
                if (score.Contains("Joueur 1"))
                {
                    _match.Player1.HasAdvantage = true;
                    _match.Player2.HasAdvantage = false;
                }
                else if (score.Contains("Joueur 2"))
                {
                    _match.Player1.HasAdvantage = false;
                    _match.Player2.HasAdvantage = true;
                }
            }
            else
            {
                var parts = score.Split('-');
                if (parts.Length == 2)
                {
                    SetPlayerGameScore(_match.Player1, parts[0]);
                    SetPlayerGameScore(_match.Player2, parts[1]);
                    _match.Player1.HasAdvantage = false;
                    _match.Player2.HasAdvantage = false;
                }
            }
        }

        private void SetPlayerGameScore(PlayerTennis player, string score)
        {
            switch (score)
            {
                case "0":
                    player.GameScore = GameScore.Zero;
                    break;
                case "15":
                    player.GameScore = GameScore.Fifteen;
                    break;
                case "30":
                    player.GameScore = GameScore.Thirty;
                    break;
                case "40":
                    player.GameScore = GameScore.Forty;
                    break;
            }
        }

        public void SetGamesScore(string score)
        {
            var parts = score.Split('-');
            if (parts.Length == 2)
            {
                _match.Player1.GamesWon = int.Parse(parts[0]);
                _match.Player2.GamesWon = int.Parse(parts[1]);
                
                // Auto-trigger tie-break if 6-6
                if (score == "6-6")
                {
                    _match.StartTieBreak();
                }
            }
        }

        public void SetSetsScore(string score)
        {
            var parts = score.Split('-');
            if (parts.Length == 2)
            {
                _match.Player1.SetsWon = int.Parse(parts[0]);
                _match.Player2.SetsWon = int.Parse(parts[1]);
                
                int totalSets = _match.Player1.SetsWon + _match.Player2.SetsWon;
                _match.CurrentSet = totalSets + 1;
            }
        }

        public void SetTieBreakScore(string score)
        {
            var parts = score.Split('-');
            if (parts.Length == 2)
            {
                _match.Player1.TieBreakScore = int.Parse(parts[0]);
                _match.Player2.TieBreakScore = int.Parse(parts[1]);
            }
        }
    }
}