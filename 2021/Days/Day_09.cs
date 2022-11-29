using Core;
using System.Drawing;

namespace AoC_2021.Days
{
    public class Day_09 : BaseDay
    {
        private FiniteGrid2D<char> _input;
        private IEnumerable<(Point pos, int value)> _minima = Array.Empty<(Point, int)>();

        public Day_09()
        {
            _input = Grid2D.FromFile(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            _minima = _input.Where(p => _input.Get4NeighborsOf(p.pos).All(n => _input[n] > p.value))
                .Select(t => (t.pos, t.value - '0')).ToList();
            return _minima.Sum(p => p.value + 1).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var results = new List<int>();
            var s = new BreadthFirstSearch<Point>(null, x => _input.Get4NeighborsOf(x).Where(n => _input[n] < '9'));
            foreach (var (pos, value) in _minima)
            {
                results.Add(s.FindReachable(pos).Count);
            }
            return results.OrderByDescending(x => x).Take(3).Product().ToString();
        }
    }
}
