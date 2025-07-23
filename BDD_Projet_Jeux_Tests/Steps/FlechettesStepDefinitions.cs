using TechTalk.SpecFlow;
using FluentAssertions;
using BDD_Projet_Jeux.Games.Flechettes;
using BDD_Projet_Jeux.Utilities;

[Binding]
public class FlechettesSteps
{
    private FlechettesGame _game;
    private UGameResult _lastResult;
    private Exception _exception;
    private GameState _currentState;

    [Given(@"une nouvelle partie de fléchettes avec (\d+) joueurs")]
    public void NewGame(int playerCount)
    {
        _game = new FlechettesGame(new FlechettesRules());
        _game.Initialize(playerCount);
        _currentState = _game.GetCurrentState();
    }

    [Given(@"une partie avec la règle ""([^""]*)""")]
    public void WithSpecialRule(string rule)
    {
        var rules = new FlechettesRules();
        if (rule == "Fin sur double")
        {
            rules = new FlechettesRules { FinishOnDouble = true };
        }
        _game = new FlechettesGame(rules);
        _game.Initialize(2); // Par défaut 2 joueurs
    }

    [Given(@"le joueur (\d+) a (\d+) points restants")]
    public void SetPlayerScore(int playerNumber, int score)
    {
        var player = _game.GetCurrentState().Players.Keys.First(p => p == $"Joueur {playerNumber}");
        _game.SetPlayerScore(player, score);
    }

    [When(@"le joueur (\d+) marque (\d+) points")]
    public void PlayerScores(int playerNumber, int points)
    {
        try
        {
            // Simuler le tour du bon joueur
            while (_game.GetCurrentState().CurrentPlayer != $"Joueur {playerNumber}")
            {
                _game.PlayTurn(0);
            }
            
            _lastResult = _game.PlayTurn(points);
            _currentState = _game.GetCurrentState();
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Then(@"son score doit être (\d+)")]
    public void VerifyScore(int expectedScore)
    {
        var currentPlayer = _currentState.CurrentPlayer;
        _currentState.Players[currentPlayer].Should().Be(expectedScore);
    }

    [Then(@"la partie doit être terminée")]
    public void VerifyGameOver()
    {
        _lastResult.IsGameOver.Should().BeTrue();
    }

    [Then(@"le joueur (\d+) doit être déclaré vainqueur")]
    public void VerifyWinner(int playerNumber)
    {
        _lastResult.Winner.Should().Be($"Joueur {playerNumber}");
    }

    [Then(@"le système doit rejeter le score")]
    public void VerifyScoreRejected()
    {
        _lastResult.IsValid.Should().BeFalse();
    }

    [Then(@"le tour doit passer au joueur (\d+)")]
    public void VerifyNextPlayer(int playerNumber)
    {
        _currentState.CurrentPlayer.Should().Be($"Joueur {playerNumber}");
    }

    [Then(@"son score final doit être (\d+)")]
    public void VerifyFinalScore(int expectedScore)
    {
        var winner = _lastResult.Winner;
        _currentState.Players[winner].Should().Be(expectedScore);
    }
}