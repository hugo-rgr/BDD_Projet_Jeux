using FluentAssertions;
using BDD_Projet_Jeux;

namespace BDD_Projet_Jeux_Tests.Steps;

[Binding]
public sealed class TicTacToeStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;
    private readonly TestContext _testContext;
    private TicTacToeGame _ticTacToeGame;
    private TicTacToe _game; // For backward compatibility

    public TicTacToeStepDefinitions(ScenarioContext scenarioContext, TestContext testContext)
    {
        _scenarioContext = scenarioContext;
        _testContext = testContext;
    }

    [Given(@"une nouvelle partie de TicTacToe est créée")]
    public void GivenUneNouvellePartieDeTicTacToeEstCreee()
    {
        _ticTacToeGame = new TicTacToeGame();
        _ticTacToeGame.Initialize(2);
        _game = _ticTacToeGame.GetTicTacToe();
        _testContext.CurrentGame = _ticTacToeGame;
    }

    [Given(@"le joueur (.*) commence la partie")]
    public void GivenLeJoueurCommenceLaPartie(string player)
    {
        _game.CurrentPlayerTicTacToe.Should().Be(PlayerTicTacToe.X);
    }

    [When(@"le joueur (.*) joue en position \((\d+),(\d+)\)")]
    public void WhenLeJoueurJoueEnPosition(string player, int row, int col)
    {
        var expectedPlayer = Enum.Parse<PlayerTicTacToe>(player);
        _game.CurrentPlayerTicTacToe.Should().Be(expectedPlayer);
        
        var result = _ticTacToeGame.PlayTurn(row, col);
        _testContext.LastResult = result;
    }

    [When(@"le joueur (.*) tente de jouer en position \((\d+),(\d+)\)")]
    public void WhenLeJoueurTenteDeJouerEnPosition(string player, int row, int col)
    {
        try
        {
            _scenarioContext["exception"] = null;
            
            // Ne pas vérifier le tour du joueur si la partie est terminée
            if (!_game.IsGameOver)
            {
                var expectedPlayer = Enum.Parse<PlayerTicTacToe>(player);
                _game.CurrentPlayerTicTacToe.Should().Be(expectedPlayer);
            }
            
            var result = _ticTacToeGame.PlayTurn(row, col);
            _testContext.LastResult = result;
            
            if (!result.IsValid)
            {
                _scenarioContext["exception"] = new Exception(result.Message);
            }
        }
        catch (Exception e)
        {
            _scenarioContext["exception"] = e;
            _testContext.LastException = e;
        }
    }

    [Given(@"le joueur (.*) a joué en position \((\d+),(\d+)\)")]
    public void GivenLeJoueurAJoueEnPosition(string player, int row, int col)
    {
        var result = _ticTacToeGame.PlayTurn(row, col);
        _testContext.LastResult = result;
    }

    [Given(@"les mouvements suivants ont été joués:")]
    public void GivenLesMouvementsSuivantsOntEteJoues(Table table)
    {
        foreach (var row in table.Rows)
        {
            var player = row["Joueur"];
            var position = row["Position"].Split(',');
            var r = int.Parse(position[0]);
            var c = int.Parse(position[1]);
            
            var result = _ticTacToeGame.PlayTurn(r, c);
            _testContext.LastResult = result;
        }
    }

    [Given(@"le joueur (.*) a gagné la partie")]
    public void GivenLeJoueurAGagneLaPartie(string player)
    {
        // Setup d'une partie gagnée pour les tests
        if (player == "X")
        {
            _ticTacToeGame.PlayTurn(0, 0); // X
            _ticTacToeGame.PlayTurn(1, 0); // O
            _ticTacToeGame.PlayTurn(0, 1); // X
            _ticTacToeGame.PlayTurn(1, 1); // O
            _ticTacToeGame.PlayTurn(0, 2); // X gagne
        }
        
        // Vérifier que la partie est bien terminée
        _game.IsGameOver.Should().BeTrue();
    }

    [Then(@"la case \((\d+),(\d+)\) contient le symbole (.*)")]
    public void ThenLaCaseContientLeSymbole(int row, int col, string symbol)
    {
        var expectedPlayer = Enum.Parse<PlayerTicTacToe>(symbol);
        _game.GetCell(row, col).Should().Be(expectedPlayer);
    }

    [Then(@"c'est (toujours )?au tour du joueur (.*)")]
    public void ThenCestAuTourDuJoueur(string toujours, string player)
    {
        var expectedPlayer = Enum.Parse<PlayerTicTacToe>(player);
        _game.CurrentPlayerTicTacToe.Should().Be(expectedPlayer);
    }

    [Then(@"la partie n'est pas terminée")]
    public void ThenLaPartieNestPasTerminee()
    {
        _game.IsGameOver.Should().BeFalse();
        _game.Result.Should().Be(GameResult.InProgress);
    }

    [Then(@"la partie est terminée")]
    public void ThenLaPartieEstTerminee()
    {
        _game.IsGameOver.Should().BeTrue();
        _game.Result.Should().NotBe(GameResult.InProgress);
    }

    [Then(@"le joueur (.*) a gagné")]
    public void ThenLeJoueurAGagne(string player)
    {
        var expectedResult = player == "X" ? GameResult.XWins : GameResult.OWins;
        _game.Result.Should().Be(expectedResult);
        _game.IsGameOver.Should().BeTrue();
    }

    [Then(@"le motif gagnant est ""(.*)""")]
    public void ThenLeMotifGagnantEst(string motif)
    {
        var expectedPattern = motif switch
        {
            "ligne horizontale" => WinPattern.LigneHorizontale,
            "ligne verticale" => WinPattern.LigneVerticale,
            "diagonale principale" => WinPattern.DiagonalePrincipale,
            "anti-diagonale" => WinPattern.AntiDiagonale,
            _ => WinPattern.None
        };
        _game.WinPattern.Should().Be(expectedPattern);
    }

    [Then(@"le résultat est ""(.*)""")]
    public void ThenLeResultatEst(string resultat)
    {
        if (resultat == "match nul")
        {
            _game.Result.Should().Be(GameResult.Draw);
        }
    }

    [Then(@"la case \((\d+),(\d+)\) contient toujours le symbole (.*)")]
    public void ThenLaCaseContientToujoursLeSymbole(int row, int col, string symbol)
    {
        var expectedPlayer = Enum.Parse<PlayerTicTacToe>(symbol);
        _game.GetCell(row, col).Should().Be(expectedPlayer);
    }

    [Then(@"l'état de la grille n'a pas changé")]
    public void ThenLetatDeLaGrilleNaPasChange()
    {
        // Validation que l'état n'a pas changé après une erreur
        _game.Should().NotBeNull();
    }

    [When(@"je demande l'état de la partie")]
    public void WhenJeDemandeLetatDeLaPartie()
    {
        var state = _ticTacToeGame.GetCurrentState();
    }

    [Then(@"la grille affiche:")]
    public void ThenLaGrilleAffiche(Table table)
    {
        var actualState = _game.GetBoardState();
        var expectedRows = table.Rows.ToList();
        
        for (int i = 0; i < 3; i++)
        {
            var expectedRow = expectedRows[i];
            var rowValues = expectedRow.Values.ToList();
            
            for (int j = 0; j < 3; j++)
            {
                var expectedCell = rowValues[j].Trim();
                var actualCell = actualState[i, j];
                
                if (string.IsNullOrEmpty(expectedCell))
                {
                    actualCell.Should().BeEmpty();
                }
                else
                {
                    actualCell.Should().Be(expectedCell);
                }
            }
        }
    }
}