using System.Drawing;
using Core;

using static MoreLinq.Extensions.TakeUntilExtension;

namespace AoC_2022.Days
{
    public sealed class Day_08 : BaseDay
    {
        private readonly FiniteGrid2D<char> _input;

        public Day_08()
        {
            _input = new FiniteGrid2D<char>(File.ReadAllLines(InputFilePath));
        }

        public override async ValueTask<string> Solve_1()
        {
            return _input.Count(t => isVisible(t.pos)).ToString();


            return "-";
        }

        private bool isVisible(Point p)
        {
            var height = _input[p];
            foreach (var dir in Directions.All4)
            {
                var line = _input.Line(p, dir.ToSize()).ToList();

                if (line.Count == 0)
                    return true;

                if (line.All(other => _input[other] < height))
                    return true;
            }
            return false;
        }

        public override async ValueTask<string> Solve_2()
        {
            return _input.Max(t => Score(t.pos)).ToString();
        }

        private int Score(Point p)
        {
            var height = _input[p];
            var score = 1;
            foreach (var dir in Directions.All4)
            {
                var line = _input.Line(p, dir.ToSize()).ToList();

                score *= line.TakeUntil(other => _input[other] >= height).Count();
            }
            return score;
        }
    }
}