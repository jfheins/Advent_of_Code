using Core;

namespace AoC_2022.Days
{
    public sealed class Day_02 : BaseDay
    {
        private readonly string[] _input;

        public Day_02()
        {
            _input = File.ReadAllLines(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            // X for Rock, Y for Paper, and Z for Scissors.
            var scores = _input.Select(line => PlayRound(ParseAbcMove(line[0]), ParseXyzMove(line[2])));
            return scores.Sum().ToString(); // 11449
        }

        private static Move ParseAbcMove(char c)
            => c switch { 'A' => Move.Rock, 'B' => Move.Paper, 'C' => Move.Scissors };

        private static Move ParseXyzMove(char c)
            => c switch { 'X' => Move.Rock, 'Y' => Move.Paper, 'Z' => Move.Scissors };

        private static Outcome ParseOutcome(char c) =>
            c switch { 'X' => Outcome.WeLoose, 'Y' => Outcome.Draw, 'Z' => Outcome.WeWin };

        public override async ValueTask<string> Solve_2()
        {
            // X means you need to lose, Y means you need to end the round in a draw, and Z means you need to win.
            var scores = _input.Select(line => PlayRound(ParseAbcMove(line[0]), ParseOutcome(line[2])));
            return scores.Sum().ToString(); // 13187
        }

        private enum Move
        {
            Rock = 1,
            Paper = 2,
            Scissors = 3
        };

        private enum Outcome
        {
            WeLoose = 0,
            Draw = 3,
            WeWin = 6
        };

        private static int PlayRound(Move opponent, Move ours)
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

        private static int PlayRound(Move opponent, Outcome outcome)
        {
            var ourMove = (Move)(outcome switch
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