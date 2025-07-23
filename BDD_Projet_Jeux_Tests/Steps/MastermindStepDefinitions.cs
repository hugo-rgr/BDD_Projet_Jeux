using TechTalk.SpecFlow;
using FluentAssertions;
using BDD_Projet_Jeux.Games.Mastermind;
using BDD_Projet_Jeux.Utilities;

[Binding]
public class MastermindSteps
{
    private MastermindGame _game;
    private GameResult _lastResult;
    private TestContext _testContext;

    public MastermindSteps(TestContext testContext)
    {
        _testContext = testContext;
    }

    [Given(@"une nouvelle partie de Mastermind avec le code secret ""([^""]*)""")]
    public void NewGameWithCode(string secretCode)
    {
        _game = new MastermindGame();
        _game.Initialize(1);
        _game.SetSecretCode(new Code(secretCode));
        _testContext.CurrentGame = _game;
    }

    [When(@"je propose ""([^""]*)""")]
    public void SubmitGuess(string guess)
    {
        _lastResult = _game.PlayTurn(guess);
        _testContext.LastResult = _lastResult;
    }

    [When(@"j'effectue (\d+) propositions incorrectes:")]
    public void MultipleGuesses(int attemptCount, Table table)
    {
        foreach (var row in table.Rows)
        {
            _lastResult = _game.PlayTurn(row["Proposition"]);
            if (_lastResult.IsGameOver) break;
        }
        _testContext.LastResult = _lastResult;
    }

    [Then(@"je devrais recevoir ""([^""]*)""")]
    public void VerifyFeedback(string expectedFeedback)
    {
        _lastResult.Message.Should().Be(expectedFeedback);
    }

    [Then(@"le système doit rejeter la tentative")]
    public void VerifyInvalidAttempt()
    {
        _lastResult.IsValid.Should().BeFalse();
    }

    [Then(@"la partie doit être terminée")]
    public void VerifyGameOver()
    {
        _lastResult.IsGameOver.Should().BeTrue();
    }

    [Then(@"le créateur du code doit gagner")]
    public void VerifyCodeMakerWins()
    {
        _lastResult.Winner.Should().Be("CodeMaker");
    }
}