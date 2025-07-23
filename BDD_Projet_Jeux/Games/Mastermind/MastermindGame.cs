using System;
using System.Collections.Generic;
using BDD_Projet_Jeux.Utilities;

namespace BDD_Projet_Jeux.Games.Mastermind
{
    public class MastermindGame : IGame
    {
        private Code _secretCode;
        private List<Feedback> _feedbacks;
        private int _remainingAttempts;
        private bool _isSolved;

        public string GameName => "Mastermind";

        public void Initialize(int playerCount)
        {
            _secretCode = Code.GenerateRandom();
            _feedbacks = new List<Feedback>();
            _remainingAttempts = 10;
            _isSolved = false;
        }
        public void SetSecretCode(Code code)
        {
            _secretCode = code;
        }
        public UGameResult PlayTurn(params object[] inputs)
        {
            if (_remainingAttempts <= 0 || _isSolved)
                throw new InvalidOperationException("Partie terminée");

            var guess = new Code((string)inputs[0]);
            var feedback = _secretCode.evaluate(guess);
            _feedbacks.Add(feedback);
            _remainingAttempts--;

            if (feedback.IsPerfectMatch)
            {
                _isSolved = true;
                return new UGameResult {
                    IsGameOver = true,
                    Winner = "CodeBreaker",
                    Message = "Code trouvé!"
                };
            }

            if (_remainingAttempts == 0)
            {
                return new UGameResult {
                    IsGameOver = true,
                    Winner = "CodeMaker",
                    Message = "Tentatives épuisées"
                };
            }

            return new UGameResult {
                Message = feedback.ToString()
            };
        }

        public GameState GetCurrentState()
        {
            return new GameState {
                CurrentPlayer = "CodeBreaker",
                IsGameOver = _isSolved || _remainingAttempts == 0
            };
        }
    }
}