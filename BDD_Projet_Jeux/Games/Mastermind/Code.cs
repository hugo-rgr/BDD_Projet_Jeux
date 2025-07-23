using System;
using System.Linq;

namespace BDD_Projet_Jeux.Games.Mastermind
{
    public class Code
    {
        public char[] Pegs { get; }
        private static readonly char[] ValidColors = { 'R', 'G', 'B', 'Y', 'O', 'P' };

        public Code(string code)
        {
            if (code.Length != 4 || !code.All(c => ValidColors.Contains(c)))
                throw new ArgumentException("Code invalide");
            Pegs = code.ToCharArray();
        }

        public static Code GenerateRandom()
        {
            var random = new Random();
            return new Code(new string(
                Enumerable.Range(0, 4)
                    .Select(_ => ValidColors[random.Next(ValidColors.Length)])
                    .ToArray()));
        }

        public Feedback evaluate(Code guess)
        {
            int correctPositions = 0;
            int correctColors = 0;

            // Check correct positions
            for (int i = 0; i < 4; i++)
            {
                if (Pegs[i] == guess.Pegs[i])
                    correctPositions++;
            }

            // Check correct colors
            var secretGroups = Pegs.GroupBy(c => c);
            var guessGroups = guess.Pegs.GroupBy(c => c);

            foreach (var group in guessGroups)
            {
                var secretGroup = secretGroups.FirstOrDefault(g => g.Key == group.Key);
                if (secretGroup != null)
                    correctColors += Math.Min(secretGroup.Count(), group.Count());
            }

            correctColors -= correctPositions; // Remove already counted positions

            return new Feedback(correctPositions, correctColors);
        }
    }
}