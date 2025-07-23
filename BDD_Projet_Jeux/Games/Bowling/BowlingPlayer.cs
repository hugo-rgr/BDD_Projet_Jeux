using System.Collections.Generic;
using System.Linq;

namespace BDD_Projet_Jeux.Games.Bowling
{
    public class BowlingPlayer
    {
        public string Name { get; }
        public List<Frame> Frames { get; }
        public int TotalScore { get; private set; }

        public BowlingPlayer(string name)
        {
            Name = name;
            Frames = new List<Frame>();
            for (int i = 0; i < 10; i++)
                Frames.Add(new Frame());
        }

        public void RecordRoll(int pins)
        {
            var currentFrame = GetCurrentFrame();
            if (currentFrame != null)
                currentFrame.AddRoll(pins);
        }

        public bool IsFrameComplete(int frameNumber)
        {
            if (frameNumber < 1 || frameNumber > 10) return false;
            return Frames[frameNumber - 1].IsComplete;
        }

        public void CalculateFinalScore()
        {
            TotalScore = 0;
            for (int i = 0; i < 10; i++)
            {
                var frame = Frames[i];
                TotalScore += frame.Rolls.Sum();

                // Handle strikes
                if (frame.IsStrike && i < 9)
                {
                    TotalScore += Frames[i + 1].Rolls.Take(2).Sum();
                }
                // Handle spares
                else if (frame.IsSpare && i < 9)
                {
                    TotalScore += Frames[i + 1].Rolls.FirstOrDefault();
                }
            }
        }

        private Frame GetCurrentFrame()
        {
            return Frames.FirstOrDefault(f => !f.IsComplete);
        }
    }
}