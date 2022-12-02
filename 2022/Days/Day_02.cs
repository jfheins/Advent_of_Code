using Core;

using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;

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
            // X for Rock, Y for Paper, and Z for Scissors.
            var scores = _input.Select(line => PlayRound(ParseOtherMove(line[0]), ParseOurMove(line[2])));
            return scores.Sum().ToString(); // 11449

            static Move ParseOtherMove(char c) => c switch { 'A' => Move.Rock, 'B' => Move.Paper, 'C' => Move.Scissors };
            static Move ParseOurMove(char c) => c switch { 'X' => Move.Rock, 'Y' => Move.Paper, 'Z' => Move.Scissors };
        }

        public override async ValueTask<string> Solve_2()
        {
            // X means you need to lose, Y means you need to end the round in a draw, and Z means you need to win.
            var scores = _input.Select(line => PlayRound(OtherMove(line[0]), ParseOutcome(line[2])));
            return scores.Sum().ToString(); // 13187

            static Move OtherMove(char c) => c switch { 'A' => Move.Rock, 'B' => Move.Paper, 'C' => Move.Scissors };
            static Outcome ParseOutcome(char c) => c switch { 'X' => Outcome.WeLoose, 'Y' => Outcome.Draw, 'Z' => Outcome.WeWin };
        }

        private enum Move { Rock = 1, Paper = 2, Scissors = 3 };
        private enum Outcome { WeLoose= 0, Draw = 3, WeWin = 6 };

        private int PlayRound(Move opponent, Move ours)
        {
            var outcome = (opponent - ours).Modulo(3) switch
            {
                0 => Outcome.Draw,
                1 => Outcome.WeLoose,
                2 => Outcome.WeWin,
                _ => throw new NotImplementedException(),
            };
            return GetScore(outcome, ours);
        }

        private int PlayRound(Move opponent, Outcome outcome)
        {
            var ourMove = (Move) (outcome switch
            {
                Outcome.WeLoose => ((int)opponent + 2).OneBasedModulo(3),
                Outcome.Draw => (int)opponent,
                Outcome.WeWin => ((int)opponent + 1).OneBasedModulo(3),
                _ => throw new NotImplementedException(),
            });
            return GetScore(outcome, ourMove);
        }

        private static int GetScore(Outcome outcome, Move ourMove) => (int)outcome + (int)ourMove;
    }
}