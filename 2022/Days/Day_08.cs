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
        }

        private bool isVisible(Point p)
        {
            var height = _input[p];
            return Directions.All4
                .Any(dir => _input.Line(p, dir.ToSize()).All(other => _input[other] < height));
        }

        public override async ValueTask<string> Solve_2()
        {
            return _input.Max(t => Score(t.pos)).ToString();
        }

        private long Score(Point p)
        {
            var height = _input[p];
            return Directions.All4.Select(dir =>
            _input.Line(p, dir.ToSize())
            .TakeUntil(other => _input[other] >= height)
            .Count()).Product();
        }
    }
}