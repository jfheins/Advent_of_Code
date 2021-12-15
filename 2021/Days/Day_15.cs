using Core;
using System.Drawing;

namespace AoC_2021.Days
{
    public class Day_15 : BaseDay
    {
        private FiniteGrid2D<char> _grid;

        public Day_15()
        {
            _grid = Grid2D.FromFile(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            var dest = _grid.BottomRight;
            var s = new AStarSearch<Point>(null, Expand);
            var path = s.FindFirst(new Point(0, 0), x => x == dest, x => x.ManhattanDistTo(dest))!;
            var totalRisk = path.Cost;
            return totalRisk.ToString();

            IEnumerable<(Point node, float cost)> Expand(Point p)
            {
                foreach (var n in _grid.Get4NeighborsOf(p))
                    yield return (n, _grid[n] - '0');
            }
        }

        public override async ValueTask<string> Solve_2()
        {
            var largerGrid = ExtendGrindFiveTimes();
            var dest = largerGrid.BottomRight;
            var s = new AStarSearch<Point>(null, Expand);
            var path = s.FindFirst(new Point(0, 0), x => x == dest, x => x.ManhattanDistTo(dest))!;
            var totalRisk = path.Cost;
            return totalRisk.ToString();

            IEnumerable<(Point node, float cost)> Expand(Point p)
            {
                foreach (var neighbor in largerGrid.Get4NeighborsOf(p))
                    yield return (neighbor, largerGrid[neighbor]);
            }
        }

        public FiniteGrid2D<short> ExtendGrindFiveTimes()
        {
            return new FiniteGrid2D<short>(_grid.Width * 5, _grid.Height * 5, Filler);

            short Filler(int x, int y)
            {
                var gridX = x / _grid.Width;
                var gridY = y / _grid.Height;
                var totalWrap = gridX + gridY;

                var originValue = _grid.GetValueWraparound(x, y) - '0';
                return (short)(originValue + totalWrap).OneBasedModulo(9);
            }
        }
    }
}
