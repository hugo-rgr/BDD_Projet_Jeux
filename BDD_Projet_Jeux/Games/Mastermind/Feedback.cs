namespace BDD_Projet_Jeux.Games.Mastermind
{
    public class Feedback
    {
        public int CorrectPositions { get; }
        public int CorrectColors { get; }
        public bool IsPerfectMatch => CorrectPositions == 4;

        public Feedback(int correctPositions, int correctColors)
        {
            CorrectPositions = correctPositions;
            CorrectColors = correctColors;
        }

        public override string ToString()
        {
            return $"{CorrectPositions} bien placés, {CorrectColors} bonnes couleurs mal placées";
        }
    }
}