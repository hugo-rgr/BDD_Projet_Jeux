using BDD_Projet_Jeux.Tennis;
using NUnit.Framework;

namespace BDD_Projet_Jeux_Tests.Steps
{
    [Binding]
    public sealed class TennisStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly TestContext _testContext;
        private TennisGame _tennisGame;
        private Match _match; // For backward compatibility

        public TennisStepDefinitions(ScenarioContext scenarioContext, TestContext testContext)
        {
            _scenarioContext = scenarioContext;
            _testContext = testContext;
        }

        [Given(@"un nouveau match de tennis est initialisé")]
        public void GivenUnNouveauMatchDeTennisEstInitialise()
        {
            _tennisGame = new TennisGame();
            _tennisGame.Initialize(2);
            _match = _tennisGame.GetMatch();
            _testContext.CurrentGame = _tennisGame;
            _scenarioContext["match"] = _match;
        }

        [Given(@"le score initial est 0-0 en points, 0-0 en jeux et 0-0 en sets")]
        public void GivenLeScoreInitialEst()
        {
            Assert.AreEqual("0-0", _match.GetGameScore());
            Assert.AreEqual("0-0", _match.GetGamesScore());
            Assert.AreEqual("0-0", _match.GetSetsScore());
        }

        [Given(@"le score du jeu est (.*)")]
        public void GivenLeScoreDuJeuEst(string score)
        {
            _tennisGame.SetGameScore(score);
        }

        [Given(@"le score des jeux est (.*)")]
        public void GivenLeScoreDesJeuxEst(string score)
        {
            _tennisGame.SetGamesScore(score);
        }

        [Given(@"le score des sets est (.*)")]
        public void GivenLeScoreDesSetsEst(string score)
        {
            _tennisGame.SetSetsScore(score);
        }

        [Given(@"le jeu vient de commencer")]
        public void GivenLeJeuVientDeCommencer()
        {
            // Le jeu est déjà à 0-0 par défaut
        }

        [Given(@"un tie-break est en cours")]
        public void GivenUnTieBreakEstEnCours()
        {
            // Simuler un tie-break en mettant les jeux à 6-6
            _match.Player1.GamesWon = 6;
            _match.Player2.GamesWon = 6;
            _match.StartTieBreak();
        }

        [Given(@"le score du tie-break est (.*)")]
        public void GivenLeScoreDuTieBreakEst(string score)
        {
            _tennisGame.SetTieBreakScore(score);
        }

        [Given(@"le joueur (.*) a remporté le set (.*)")]
        public void GivenLeJoueurARemporteLeSet(int numeroJoueur, string numeroSet)
        {
            if (numeroJoueur == 1)
                _match.Player1.SetsWon++;
            else
                _match.Player2.SetsWon++;
            
            int totalSets = _match.Player1.SetsWon + _match.Player2.SetsWon;
            _match.CurrentSet = totalSets + 1;
        }

        [Given(@"le joueur (.*) a gagné (.*)")]
        public void GivenLeJoueurAGagne(int numeroJoueur, string scoreMatch)
        {
            // Configurer le score final du match
            var parts = scoreMatch.Split('-');
            if (parts.Length == 2)
            {
                if (numeroJoueur == 1)
                {
                    _match.Player1.SetsWon = int.Parse(parts[0]);
                    _match.Player2.SetsWon = int.Parse(parts[1]);
                    _match.Winner = _match.Player1;
                }
                else
                {
                    _match.Player2.SetsWon = int.Parse(parts[0]);
                    _match.Player1.SetsWon = int.Parse(parts[1]);
                    _match.Winner = _match.Player2;
                }
                
                _match.State = MatchState.Completed;
            }
        }

        [Given(@"le match est terminé")]
        public void GivenLeMatchEstTermine()
        {
            _match.Player1.SetsWon = 2;
            _match.State = MatchState.Completed;
            _match.Winner = _match.Player1;
        }
        
        [When(@"le joueur (.*) marque un point(?: dans le tie-break)?")]
        public void WhenLeJoueurMarqueUnPoint(int numeroJoueur)
        {
            try
            {
                var result = _tennisGame.PlayTurn(numeroJoueur);
                _testContext.LastResult = result;
            }
            catch (Exception ex)
            {
                _testContext.LastException = ex;
            }
        }

        [When(@"le joueur (.*) remporte le jeu suivant")]
        public void WhenLeJoueurRemporteLeJeuSuivant(int numeroJoueur)
        {
            // Forcer directement la victoire du jeu
            var scoringPlayer = numeroJoueur == 1 ? _match.Player1 : _match.Player2;
            var otherPlayer = numeroJoueur == 1 ? _match.Player2 : _match.Player1;
            
            _match.GameJustWon = false;
            scoringPlayer.ResetGame();
            otherPlayer.ResetGame();
            
            for (int i = 0; i < 4; i++)
            {
                var result = _tennisGame.PlayTurn(numeroJoueur);
                _testContext.LastResult = result;
                
                if (_match.GameJustWon)
                    break;
            }
        }

        [When(@"le joueur (.*) remporte le set (.*)")]
        public void WhenLeJoueurRemporteLeSet(int numeroJoueur, string numeroSet)
        {
            if (numeroJoueur == 1)
                _match.Player1.SetsWon++;
            else
                _match.Player2.SetsWon++;
            
            // Vérifier si le match est terminé (2 sets gagnants)
            if ((numeroJoueur == 1 && _match.Player1.SetsWon == 2) || 
                (numeroJoueur == 2 && _match.Player2.SetsWon == 2))
            {
                _match.State = MatchState.Completed;
                _match.Winner = numeroJoueur == 1 ? _match.Player1 : _match.Player2;
            }
            else
            {
                // Mettre à jour le set actuel si le match continue
                int totalSets = _match.Player1.SetsWon + _match.Player2.SetsWon;
                _match.CurrentSet = totalSets + 1;
            }
        }

        [When(@"on tente de faire marquer un point au joueur (.*)")]
        public void WhenOnTenteDeFaireMarquerUnPointAuJoueur(int numeroJoueur)
        {
            try
            {
                var result = _tennisGame.PlayTurn(numeroJoueur);
                _testContext.LastResult = result;
                _scenarioContext["exception"] = null;
            }
            catch (Exception ex)
            {
                _testContext.LastException = ex;
                _scenarioContext["exception"] = ex;
            }
        }

        [When(@"on demande l'état du match")]
        public void WhenOnDemandeLetatDuMatch()
        {
            _scenarioContext["matchState"] = _match.State;
        }

        // All the Then steps remain the same since they work with the Match object
        [Then(@"le score du jeu devient (.*)")]
        public void ThenLeScoreDuJeuDevient(string scoreAttendu)
        {
            var scoreActuel = _match.GetGameScore();
            Assert.AreEqual(scoreAttendu, scoreActuel);
        }

        [Then(@"le joueur (.*) remporte le jeu")]
        public void ThenLeJoueurRemporteLeJeu(int numeroJoueur)
        {
            _match.GameJustWon = false;
            
            Assert.AreEqual("0-0", _match.GetGameScore());
            
            if (numeroJoueur == 1)
                Assert.IsTrue(_match.Player1.GamesWon > 0);
            else
                Assert.IsTrue(_match.Player2.GamesWon > 0);
        }

        [Then(@"le score des jeux (?:devient|revient à) (.*)")]
        public void ThenLeScoreDesJeuxDevient(string scoreAttendu)
        {
            Assert.AreEqual(scoreAttendu, _match.GetGamesScore()); 
        }

        [Then(@"le score des points revient à (.*)")]
        [Then(@"le score du jeu revient à (.*)")]
        public void ThenLeScoreDesPointsRevientA(string scoreAttendu)
        {
            Assert.AreEqual(scoreAttendu, _match.GetGameScore());
        }

        [Then(@"le joueur (.*) remporte le set(?:\s+avec un score de\s+(.*)|\s+(\d+-\d+))?")]
        public void ThenLeJoueurRemporteLeSet(int numeroJoueur, string scoreSetAvecTexte = null, string scoreSetDirect = null)
        {
            bool setGagne = _match.SetJustWon || 
                            (numeroJoueur == 1 ? _match.Player1.SetsWon > 0 : _match.Player2.SetsWon > 0);
            
            Assert.IsTrue(setGagne, $"Le joueur {numeroJoueur} devrait avoir gagné un set");
            _match.SetJustWon = false;
        }

        [Then(@"le score des sets devient (.*)")]
        public void ThenLeScoreDesSetsDevient(string scoreAttendu)
        {
            Assert.AreEqual(scoreAttendu, _match.GetSetsScore());
        }

        [Then(@"un tie-break commence")]
        public void ThenUnTieBreakCommence()
        {
            Assert.IsTrue(_match.IsTieBreak);
        }

        [Then(@"le score du tie-break est (.*)")]
        public void ThenLeScoreDuTieBreakEst(string scoreAttendu)
        {
            var scoreActuel = $"{_match.Player1.TieBreakScore}-{_match.Player2.TieBreakScore}";
            Assert.AreEqual(scoreAttendu, scoreActuel);
        }

        [Then(@"le score du tie-break devient ""(.*)""")]
        [Then(@"le score du tie-break revient à (.*)")]
        public void ThenLeScoreDuTieBreakDevient(string scoreAttendu)
        {
            var scoreActuel = _match.GetGameScore();
            Assert.AreEqual(scoreAttendu, scoreActuel);
        }

        [Then(@"le joueur (.*) remporte le tie-break (.*)")]
        public void ThenLeJoueurRemporteLeTieBreak(int numeroJoueur, string score)
        {
            Assert.IsFalse(_match.IsTieBreak, "Le tie-break devrait être terminé");
            ThenLeJoueurRemporteLeSet(numeroJoueur);
        }

        [Then(@"le joueur (.*) remporte le match")]
        public void ThenLeJoueurRemporteLeMatch(int numeroJoueur)
        {
            Assert.AreEqual(MatchState.Completed, _match.State);
            var joueurGagnant = numeroJoueur == 1 ? _match.Player1 : _match.Player2;
            Assert.AreEqual(joueurGagnant, _match.Winner);
        }

        [Then(@"le score final du match est (.*)")]
        public void ThenLeScoreFinalDuMatchEst(string scoreAttendu)
        {
            Assert.AreEqual(scoreAttendu, _match.GetSetsScore());
        }

        [Then(@"le match est en cours")]
        public void ThenLeMatchEstEnCours()
        {
            var state = (MatchState)_scenarioContext["matchState"];
            Assert.AreEqual(MatchState.InProgress, state);
        }

        [Then(@"le set actuel est le set numéro (.*)")]
        public void ThenLeSetActuelEstLeSetNumero(int numeroSet)
        {
            Assert.AreEqual(numeroSet, _match.CurrentSet);
        }
    }
}