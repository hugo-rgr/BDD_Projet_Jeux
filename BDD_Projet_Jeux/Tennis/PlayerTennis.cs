namespace BDD_Projet_Jeux.Tennis
{
    public class PlayerTennis
    {
        public GameScore GameScore { get; set; }
        public int GamesWon { get; set; }
        public int SetsWon { get; set; }
        public int TieBreakScore { get; set; }
        public bool HasAdvantage { get; set; }

        public PlayerTennis()
        {
            ResetGame();
        }

        public void ResetGame()
        {
            GameScore = GameScore.Zero;
            HasAdvantage = false;
        }

        public void ResetSet()
        {
            // Gagner un set implique aussi gagner un jeu 
            GamesWon = 0;
            ResetGame();
        }

        public void ResetTieBreak()
        {
            TieBreakScore = 0;
        }
    }
}