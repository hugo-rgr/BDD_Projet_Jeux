using System;
using System.Collections.Generic;
using BDD_Projet_Jeux.Utilities;


namespace BDD_Projet_Jeux.Games.Flechettes
{
    public class FlechettesGame : IGame
    {
        private List<FlechettesPlayer> _players;
        private FlechettesRules _rules;
        private int _currentPlayerIndex;
        private bool _isGameOver;
        private const int TargetScore = 501;

        public string GameName => "Fléchettes 501";

        public FlechettesGame(FlechettesRules rules)
        {
            _players = new List<FlechettesPlayer>();
            _rules = rules;
        }

        public void Initialize(int playerCount)
        {
            if (playerCount < 2)
                throw new ArgumentException("Minimum 2 joueurs requis");

            for (int i = 0; i < playerCount; i++)
                _players.Add(new FlechettesPlayer($"Joueur {i+1}", TargetScore));

            _currentPlayerIndex = 0;
            _isGameOver = false;
        }

        public GameResult PlayTurn(params object[] inputs)
        {
            if (_isGameOver)
                throw new InvalidOperationException("Partie terminée");

            int score = (int)inputs[0];
            var currentPlayer = _players[_currentPlayerIndex];

            if (!_rules.IsValidScore(currentPlayer, score))
                return new GameResult { IsValid = false, Message = "Score invalide" };

            currentPlayer.Score -= score;

            if (_rules.CheckVictory(currentPlayer))
            {
                _isGameOver = true;
                return new GameResult { 
                    IsValid = true, 
                    IsGameOver = true, 
                    Winner = currentPlayer.Name 
                };
            }

            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
            return new GameResult { IsValid = true };
        }
        public void SetPlayerScore(string playerName, int score)
        {
            var player = _players.First(p => p.Name == playerName);
            player.Score = score;
        }
        public GameState GetCurrentState()
        {
            var state = new GameState();
            foreach (var player in _players)
                state.Players.Add(player.Name, player.Score);
            
            state.CurrentPlayer = _players[_currentPlayerIndex].Name;
            state.IsGameOver = _isGameOver;
            return state;
        }
    }
}