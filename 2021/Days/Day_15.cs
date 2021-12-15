using Core;
using Core.Combinatorics;
using MoreLinq.Extensions;
using System.Drawing;
using System.IO;
using System.Linq;

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
            var s = new DijkstraSearch<Point>(null, Expand);
            var dest = new Point(_grid.Width - 1, _grid.Height - 1);
            var path = s.FindFirst(new Point(0, 0), x => x == dest);
            var risk = path.Cost;
            return risk.ToString();
        }

        private IEnumerable<(Point node, float cost)> Expand(Point p)
        {
            foreach (var neighbor in _grid.Get4NeighborsOf(p))
            {
                var digit = _grid[neighbor];
                yield return (neighbor, float.Parse(digit.ToString()));
            }
        }


        public override async ValueTask<string> Solve_2()
        {
            var largerGrid = new FiniteGrid2D<int>(_grid.Width * 5, _grid.Height * 5, Filler);
            var s = new DijkstraSearch<Point>(null, Expand2);
            var dest = new Point(largerGrid.Width - 1, largerGrid.Height - 1);
            var path = s.FindFirst(new Point(0, 0), x => x == dest);
            var risk = path.Cost;
            return risk.ToString();

            IEnumerable<(Point node, float cost)> Expand2(Point p)
            {
                foreach (var neighbor in largerGrid.Get4NeighborsOf(p))
                {
                    var digit = largerGrid[neighbor];
                    yield return (neighbor, digit);
                }
            }
        }

        private int Filler(int x, int y)
        {
            var xWrap = x / _grid.Width;
            var yWrap = y / _grid.Height;
            if (xWrap >= 1)
            {
                ;
            }
            var totalWrap = xWrap + yWrap;
            var originValue = int.Parse(_grid[x % _grid.Width, y % _grid.Height].ToString());
            return (originValue-1 + totalWrap) % 9 +1;
        }
    }
}
