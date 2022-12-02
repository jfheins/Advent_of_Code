using Core;

using static MoreLinq.Extensions.SplitExtension;

namespace AoC_2022.Days
{
    public sealed class Day_02 : BaseDay
    {
        private string[] _input;

        public Day_02()
        {
            _input = File.ReadAllLines(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            // A for Rock, B for Paper, and C for Scissors.
            //  X for Rock, Y for Paper, and Z for Scissors.
            /* The score for a single round is the score for the shape you selected
             * (1 for Rock, 2 for Paper, and 3 for Scissors)
             * plus the score for the outcome of the round 
             * (0 if you lost, 3 if the round was a draw, and 6 if you won). */
            var scores = _input.Select(line => line switch
            {
                "A X" => 1+3,
                "B X" => 1+0,
                "C X" => 1+6,
                "A Y" => 2+6,
                "B Y" => 2+3,
                "C Y" => 2+0,
                "A Z" => 3+0,
                "B Z" => 3+6,
                "C Z" => 3+3,

            });

            return scores.Sum().ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            // X means you need to lose, Y means you need to end the round in a draw, and
            // Z means you need to win. Good luck!"
            var scores = _input.Select(line => line switch
            {
                "A X" => 3 + 0, // Rock / sc
                "B X" => 1 + 0, // Paper / Rock
                "C X" => 2 + 0, // Sc / Paper
                "A Y" => 1 + 3, // Rock / rock
                "B Y" => 2 + 3, // Paper / paper
                "C Y" => 3 + 3, // Sc / Sc
                "A Z" => 2 + 6, // Rock / Paper
                "B Z" => 3 + 6, // Paper / Sc
                "C Z" => 1 + 6, // Sc / Rock

            });

            return scores.Sum().ToString();
        }
    }
}