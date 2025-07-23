using TechTalk.SpecFlow;
using FluentAssertions;
using BDD_Projet_Jeux.Games.Mastermind;
using BDD_Projet_Jeux.Utilities;

[Binding]
public class MastermindSteps
{
    private MastermindGame _game;
    private UGameResult _lastResult;
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
        try
        {
            _lastResult = _game.PlayTurn(guess);
        }
        catch (Exception ex)
        {
            _lastResult = new UGameResult { IsValid = false, Message = ex.Message };
        }
        _testContext.LastResult = _lastResult;
    }

    [When(@"j'effectue (\d+) propositions incorrectes:")]
    public void MultipleGuesses(int attemptCount, Table table)
    {
        foreach (var row in table.Rows)
        {
            try
            {
                _lastResult = _game.PlayTurn(row["Proposition"]);
            }
            catch (Exception ex)
            {
                _lastResult = new UGameResult { IsValid = false, Message = ex.Message };
                break;
            }
            if (_lastResult.IsGameOver) break;
        }
        _testContext.LastResult = _lastResult;
    }

    [When(@"il fait (\d+) propositions incorrectes:")]
    public void MultipleGuessesSimple(int attemptCount, Table table)
    {
        MultipleGuesses(attemptCount, table);
    }

    [When(@"il fait (\d+) propositions de ([A-Z]+)")]
    public void MultipleGuessesSame(int count, string guess)
    {
        for (int i = 0; i < count; i++)
        {
            try
            {
                _lastResult = _game.PlayTurn(guess);
            }
            catch (Exception ex)
            {
                _lastResult = new UGameResult { IsValid = false, Message = ex.Message };
                break;
            }
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

    [Then(@"je devrais être déclaré vainqueur")]
    public void VerifyCodeBreakerWins()
    {
        _lastResult.Winner.Should().Be("CodeBreaker");
    }

    [Then(@"le système doit rejeter la tentative")]
    public void VerifyRejectedAttempt()
    {
        _lastResult.IsValid.Should().BeFalse();
    }

    [Then(@"afficher ""([^""]*)""")]
    public void VerifyErrorMessage(string expectedMessage)
    {
        _lastResult.Message.Should().Be(expectedMessage);
    }
}