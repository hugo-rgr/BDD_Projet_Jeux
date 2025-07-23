using System;
using BDD_Projet_Jeux.Games;
using BDD_Projet_Jeux.Utilities;

namespace BDD_Projet_Jeux
{
    public class TicTacToeGame : IGame
    {
        private TicTacToe _game;

        public string GameName => "TicTacToe";

        public void Initialize(int playerCount)
        {
            if (playerCount != 2)
                throw new ArgumentException("Le TicTacToe nécessite exactement 2 joueurs");

            _game = new TicTacToe();
        }

        public UGameResult PlayTurn(params object[] inputs)
        {
            if (_game == null)
                throw new InvalidOperationException("Le jeu n'a pas été initialisé");

            if (_game.IsGameOver)
                throw new InvalidOperationException("Partie terminée");

            if (inputs.Length < 2)
                throw new ArgumentException("Position (row, col) requise");

            int row = (int)inputs[0];
            int col = (int)inputs[1];

            try
            {
                var currentPlayer = _game.CurrentPlayerTicTacToe;
                _game.MakeMove(row, col);

                var result = new UGameResult
                {
                    IsValid = true,
                    IsGameOver = _game.IsGameOver
                };

                if (_game.IsGameOver)
                {
                    switch (_game.Result)
                    {
                        case GameResult.XWins:
                            result.Winner = "Joueur X";
                            result.Message = "Joueur X a gagné";
                            break;
                        case GameResult.OWins:
                            result.Winner = "Joueur O";
                            result.Message = "Joueur O a gagné";
                            break;
                        case GameResult.Draw:
                            result.Winner = null;
                            result.Message = "Match nul";
                            break;
                    }
                }
                else
                {
                    result.Message = $"Tour du joueur {_game.CurrentPlayerTicTacToe}";
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
            if (_game == null)
                return new GameState { IsGameOver = true };

            var state = new GameState
            {
                CurrentPlayer = $"Joueur {_game.CurrentPlayerTicTacToe}",
                IsGameOver = _game.IsGameOver
            };
            
            state.Players.Add("Joueur X", _game.Result == GameResult.XWins ? 1 : 0);
            state.Players.Add("Joueur O", _game.Result == GameResult.OWins ? 1 : 0);

            return state;
        }
        
        public TicTacToe GetTicTacToe()
        {
            return _game;
        }

        // Méthode auxiliaires pour les tests nécessitant la configuration d'états de plateaux spécifiques
        public void SetupBoardState(params (int row, int col, PlayerTicTacToe player)[] moves)
        {
            foreach (var (row, col, player) in moves)
            {
                while (_game.CurrentPlayerTicTacToe != player && !_game.IsGameOver)
                {
                    break;
                }
                
                if (!_game.IsGameOver)
                {
                    _game.MakeMove(row, col);
                }
            }
        }
        
        public UGameResult PlayTurnAsPlayer(PlayerTicTacToe player, int row, int col)
        {
            if (_game.CurrentPlayerTicTacToe != player)
            {
                return new UGameResult
                {
                    IsValid = false,
                    Message = $"Ce n'est pas le tour du joueur {player}"
                };
            }

            return PlayTurn(row, col);
        }
    }
}