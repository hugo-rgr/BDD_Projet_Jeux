using BDD_Projet_Jeux.Games;
using BDD_Projet_Jeux.Utilities;

public class TestContext
{
    public IGame CurrentGame { get; set; }
    public GameResult LastResult { get; set; }
    public Exception LastException { get; set; }
    
    public void Reset()
    {
        CurrentGame = null;
        LastResult = null;
        LastException = null;
    }
}