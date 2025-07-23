namespace BDD_Projet_Jeux;

public class TicTacToe
{
    private readonly PlayerTicTacToe?[,] _board; // matrice du plateau où chaque case peut avoir soit X, soit O
    private PlayerTicTacToe _currentPlayerTicTacToe;
    private GameResult _result;
    private WinPattern _winPattern;

    public TicTacToe()
    {
        _board = new PlayerTicTacToe?[3, 3];
        _currentPlayerTicTacToe = PlayerTicTacToe.X; // X commence toujours
        _result = GameResult.InProgress;
        _winPattern = WinPattern.None;
    }

    public bool IsGameOver => _result != GameResult.InProgress;
    public GameResult Result => _result;
    public PlayerTicTacToe CurrentPlayerTicTacToe => _currentPlayerTicTacToe;
    public WinPattern WinPattern => _winPattern;

    public PlayerTicTacToe? GetCell(int row, int col)
    {
        ValidatePosition(row, col);
        return _board[row, col];
    }

    public void MakeMove(int row, int col)
    {
        if (IsGameOver)
            throw new InvalidOperationException("Partie terminée");

        ValidatePosition(row, col);

        if (_board[row, col] != null)
            throw new InvalidOperationException("Case déjà occupée");

        _board[row, col] = _currentPlayerTicTacToe;
        
        CheckForWin();
        if (_result == GameResult.InProgress)
        {
            CheckForDraw();
        }

        if (_result == GameResult.InProgress)
        {
            _currentPlayerTicTacToe = _currentPlayerTicTacToe == PlayerTicTacToe.X ? PlayerTicTacToe.O : PlayerTicTacToe.X;
        }
    }

    public string[,] GetBoardState()
    {
        var state = new string[3, 3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                state[i, j] = _board[i, j]?.ToString() ?? "";
            }
        }
        return state;
    }

    private void ValidatePosition(int row, int col)
    {
        if (row < 0 || row > 2 || col < 0 || col > 2)
            throw new ArgumentException("Position invalide");
    }

    private void CheckForWin()
    {
        // Vérifier les lignes horizontales
        for (int row = 0; row < 3; row++)
        {
            if (CheckLine(_board[row, 0], _board[row, 1], _board[row, 2]))
            {
                SetWinner(_board[row, 0]!.Value, WinPattern.LigneHorizontale);
                return;
            }
        }

        // Vérifier les lignes verticales
        for (int col = 0; col < 3; col++)
        {
            if (CheckLine(_board[0, col], _board[1, col], _board[2, col]))
            {
                SetWinner(_board[0, col]!.Value, WinPattern.LigneVerticale);
                return;
            }
        }

        // Vérifier la diagonale principale
        if (CheckLine(_board[0, 0], _board[1, 1], _board[2, 2]))
        {
            SetWinner(_board[0, 0]!.Value, WinPattern.DiagonalePrincipale);
            return;
        }

        // Vérifier l'anti-diagonale
        if (CheckLine(_board[0, 2], _board[1, 1], _board[2, 0]))
        {
            SetWinner(_board[0, 2]!.Value, WinPattern.AntiDiagonale);
            return;
        }
    }

    private bool CheckLine(PlayerTicTacToe? a, PlayerTicTacToe? b, PlayerTicTacToe? c)
    {
        return a != null && a == b && b == c;
    }

    private void SetWinner(PlayerTicTacToe winner, WinPattern pattern)
    {
        _result = winner == PlayerTicTacToe.X ? GameResult.XWins : GameResult.OWins;
        _winPattern = pattern;
    }

    private void CheckForDraw()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (_board[i, j] == null)
                    return; // Il y a encore des cases vides
            }
        }
        _result = GameResult.Draw;
    }
}