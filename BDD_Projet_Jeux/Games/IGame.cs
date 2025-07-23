using BDD_Projet_Jeux.Utilities;

namespace BDD_Projet_Jeux.Games
{
    public interface IGame
    {
        string GameName { get; }
        void Initialize(int playerCount);
        GameResult PlayTurn(params object[] inputs);
        GameState GetCurrentState();
    }
}