using System.Collections.Generic;

namespace BDD_Projet_Jeux.Games.Bowling
{
    public class Frame
    {
        public List<int> Rolls { get; } = new List<int>();
        public bool IsComplete => Rolls.Count == 2 || (Rolls.Count == 1 && Rolls[0] == 10);
        public bool IsStrike => Rolls.Count >= 1 && Rolls[0] == 10;
        public bool IsSpare => Rolls.Count == 2 && Rolls.Sum() == 10;

        public void AddRoll(int pins)
        {
            if (pins < 0 || pins > 10)
                throw new ArgumentException("Nombre de quilles invalide");
            if (IsComplete)
                throw new InvalidOperationException("Frame déjà complété");
            Rolls.Add(pins);
        }
    }
}