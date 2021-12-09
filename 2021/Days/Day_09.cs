using Core;
using Core.Combinatorics;
using MoreLinq.Extensions;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AoC_2021.Days
{
    public class Day_09 : BaseDay
    {
        private FiniteGrid2D<char> _input;
        private List<(Point pos, char value)> _minima;

        public Day_09()
        {
            _input = Grid2D.FromFile(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            _minima = _input.Where(p => _input.Get4NeighborsOf(p.pos).All(n => _input[n] > p.value)).ToList();
            return _minima.Sum(p => int.Parse(p.value.ToString()) + 1).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var results = new List<int>();
            var s = new BreadthFirstSearch<Point>(null, x => _input.Get4NeighborsOf(x).Where(n => _input[n] < '9'));
            foreach (var min in _minima)
            {
                var basin = s.FindLeafs2(min.pos);
                results.Add( basin);
            }
            //var n = _minima.SelectMany(p => _input.Get4NeighborsOf(p.pos)).Where(n => _input[n] < 9);
            return results.OrderByDescending(x => x).Take(3).Product().ToString();
        }
    }
}
