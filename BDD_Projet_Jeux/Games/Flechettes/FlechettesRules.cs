namespace BDD_Projet_Jeux.Games.Flechettes
{
    public class FlechettesRules
    {
        public bool FinishOnDouble { get; set; }

        public bool IsValidScore(FlechettesPlayer player, int score)
        {
            if (FinishOnDouble && player.Score - score == 0)
                return score % 2 == 0; 
            
            return player.Score - score >= 0;
        }

        public bool CheckVictory(FlechettesPlayer player)
        {
            return player.Score == 0;
        }
    }
}