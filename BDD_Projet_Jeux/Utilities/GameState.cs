using System.Collections.Generic;

namespace BDD_Projet_Jeux.Utilities
{
    public class GameState
    {
        public Dictionary<string, int> Players { get; set; } = new();
        public string CurrentPlayer { get; set; }
        public bool IsGameOver { get; set; }
    }
}