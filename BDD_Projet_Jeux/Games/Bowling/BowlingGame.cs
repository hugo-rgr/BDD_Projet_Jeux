using System.Collections.Generic;
using BDD_Projet_Jeux.Utilities;

namespace BDD_Projet_Jeux.Games.Bowling
{
    public class BowlingGame : IGame
    {
        private List<BowlingPlayer> _players;
        private int _currentPlayerIndex;
        private int _currentFrame;
        private bool _isGameOver;
        public int CurrentFrame => _currentFrame; // Ajout de la propriété


        public string GameName => "Bowling 10 Frames";

        public void Initialize(int playerCount)
        {
            _players = new List<BowlingPlayer>();
            for (int i = 0; i < playerCount; i++)
                _players.Add(new BowlingPlayer($"Joueur {i+1}"));

            _currentPlayerIndex = 0;
            _currentFrame = 1;
            _isGameOver = false;
        }

        public UGameResult PlayTurn(params object[] inputs)
        {
            if (_isGameOver)
                throw new InvalidOperationException("Partie terminée");

            int pins = (int)inputs[0];
            var currentPlayer = _players[_currentPlayerIndex];

            currentPlayer.RecordRoll(pins);

            if (currentPlayer.IsFrameComplete(_currentFrame))
            {
                _currentPlayerIndex++;
                if (_currentPlayerIndex >= _players.Count)
                {
                    _currentPlayerIndex = 0;
                    _currentFrame++;
                    if (_currentFrame > 10)
                    {
                        _isGameOver = true;
                        return new UGameResult {
                            IsGameOver = true,
                            Winner = GetWinner()
                        };
                    }
                }
            }

            return new UGameResult {
                Message = $"Frame {_currentFrame}, Joueur {_currentPlayerIndex + 1}"
            };
        }

        public void ForceCurrentFrame(string playerName, int frameNumber)
        {
            _currentPlayerIndex = _players.FindIndex(p => p.Name == playerName);
            _currentFrame = frameNumber;
        }

        public int GetFrameScore(string playerName, int frameNumber)
        {
            var player = _players.First(p => p.Name == playerName);
            return player.Frames[frameNumber - 1].Rolls.Sum();
        }

        public int GetRemainingRolls(string playerName)
        {
            var player = _players.First(p => p.Name == playerName);
            var frame = player.Frames[_currentFrame - 1];
            return frame.IsComplete ? 0 : 2 - frame.Rolls.Count;
        }
        private string GetWinner()
        {
            BowlingPlayer winner = null;
            int maxScore = -1;
            foreach (var player in _players)
            {
                player.CalculateFinalScore();
                if (player.TotalScore > maxScore)
                {
                    maxScore = player.TotalScore;
                    winner = player;
                }
            }
            return winner?.Name;
        }

        public GameState GetCurrentState()
        {
            var state = new GameState();
            foreach (var player in _players)
                state.Players.Add(player.Name, player.TotalScore);
            
            state.CurrentPlayer = _players[_currentPlayerIndex].Name;
            state.IsGameOver = _isGameOver;
            return state;
        }
    }
}