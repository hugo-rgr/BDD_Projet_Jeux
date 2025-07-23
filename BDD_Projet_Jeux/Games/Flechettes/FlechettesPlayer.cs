namespace BDD_Projet_Jeux.Games.Flechettes
{
    public class FlechettesPlayer
    {
        public string Name { get; }
        public int Score { get; set; }

        public FlechettesPlayer(string name, int initialScore)
        {
            Name = name;
            Score = initialScore;
        }
    }
}