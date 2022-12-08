using System.Drawing;

using Core;

using static MoreLinq.Extensions.TakeUntilExtension;

namespace AoC_2022.Days
{
    public sealed class Day_08 : BaseDay
    {
        private readonly FiniteGrid2D<char> _grid;
        public Day_08()
        {
            _grid = new FiniteGrid2D<char>(File.ReadAllLines(InputFilePath));
        }

        public override async ValueTask<string> Solve_1()
        {
            return _grid.Count(t => IsVisible(t.pos)).ToString();
        }

        private bool IsVisible(Point p)
        {
            var height = _grid[p];
            return _grid.Lines(p, Directions.All4).Any(line => line.All(other => _grid[other] < height));
        }

        public override async ValueTask<string> Solve_2()
        {
            return _grid.Max(t => Score(t.pos)).ToString();
        }

        private long Score(Point p)
        {
            var height = _grid[p];
            return _grid.Lines(p, Directions.All4)
                .Select(line => line.TakeUntil(other => _grid[other] >= height).Count())                
                .Product();
        }
    }
}