using TechTalk.SpecFlow;
using FluentAssertions;
using BDD_Projet_Jeux.Games.Bowling;
using BDD_Projet_Jeux.Utilities;

[Binding]
public class BowlingSteps
{
    private BowlingGame _game;
    private UGameResult _lastResult;
    private TestContext _testContext;

    public BowlingSteps(TestContext testContext)
    {
        _testContext = testContext;
    }

    [Given(@"une nouvelle partie de bowling avec (\d+) joueurs")]
    public void NewGame(int playerCount)
    {
        _game = new BowlingGame();
        _game.Initialize(playerCount);
        _testContext.CurrentGame = _game;
    }

    [Given(@"le joueur (\d+) est au (\d+)ème frame")]
    public void SetCurrentFrame(int playerNumber, int frameNumber)
    {
        _game.ForceCurrentFrame($"Joueur {playerNumber}", frameNumber);
    }

    [When(@"le joueur (\d+) fait un lancer de (\d+)")]
    public void PlayerRolls(int playerNumber, int pins)
    {
        try
        {
            while (_game.GetCurrentState().CurrentPlayer != $"Joueur {playerNumber}")
            {
                _game.PlayTurn(0); // Simuler les autres joueurs
            }
            
            _lastResult = _game.PlayTurn(pins);
            _testContext.LastResult = _lastResult;
        }
        catch (Exception ex)
        {
            _testContext.LastException = ex;
            _lastResult = new UGameResult { IsValid = false, Message = ex.Message };
        }
    }

    [When(@"le joueur (\d+) fait (\d+) lancers de (\d+)")]
    public void PlayerRollsMultiple(int playerNumber, int count, int pins)
    {
        for (int i = 0; i < count; i++)
        {
            PlayerRolls(playerNumber, pins);
            if (_lastResult.IsGameOver) break;
        }
    }

    [When(@"il fait (\d+) puis (\d+)")]
    public void PlayerRollsSequence(int first, int second)
    {
        var player = _game.GetCurrentState().CurrentPlayer;
        PlayerRolls(int.Parse(player.Split(' ')[1]), first);
        PlayerRolls(int.Parse(player.Split(' ')[1]), second);
    }

    [When(@"il fait (\d+)")]
    public void PlayerRollsSingle(int pins)
    {
        var player = _game.GetCurrentState().CurrentPlayer;
        PlayerRolls(int.Parse(player.Split(' ')[1]), pins);
    }

    [Then(@"son score pour ce frame doit être (\d+)")]
    public void VerifyFrameScore(int expectedScore)
    {
        var player = _game.GetCurrentState().CurrentPlayer;
        var frameScore = _game.GetFrameScore(player, _game.CurrentFrame);
        frameScore.Should().Be(expectedScore);
    }

    [Then(@"il doit avoir droit à (\d+) lancers supplémentaires")]
    public void VerifyExtraRolls(int rollCount)
    {
        var player = _game.GetCurrentState().CurrentPlayer;
        var remainingRolls = _game.GetRemainingRolls(player);
        remainingRolls.Should().Be(rollCount);
    }

    [Then(@"son score total doit être (\d+)")]
    public void VerifyTotalScore(int expectedScore)
    {
        var player = _game.GetCurrentState().CurrentPlayer;
        _game.GetCurrentState().Players[player].Should().Be(expectedScore);
    }

    [Then(@"le système doit rejeter le lancer")]
    public void VerifyInvalidRoll()
    {
        _lastResult.IsValid.Should().BeFalse();
    }

    [Then(@"le prochain joueur doit être le joueur (\d+)")]
    public void VerifyNextPlayer(int playerNumber)
    {
        _game.GetCurrentState().CurrentPlayer.Should().Be($"Joueur {playerNumber}");
    }

    [Then(@"afficher ""([^\""]*)""")]
    public void VerifyErrorMessage(string expectedMessage)
    {
        if (_testContext.LastException != null)
        {
            _testContext.LastException.Message.Should().Be(expectedMessage);
        }
        else
        {
            _lastResult.Message.Should().Be(expectedMessage);
        }
    }
}