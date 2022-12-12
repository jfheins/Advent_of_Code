using System.Drawing;

using Core;

namespace AoC_2022.Days
{
    public sealed class Day_12 : BaseDay
    {
        private readonly FiniteGrid2D<char> _grid;

        public Day_12()
        {
            _grid = new FiniteGrid2D<char>(File.ReadAllLines(InputFilePath));
        }

        public override async ValueTask<string> Solve_1()
        {
            var start = _grid.FindFirst('S');
            var res = new BreadthFirstSearch<Point>(null, Expand) { PerformParallelSearch = false }
                .FindFirst(start, p => _grid[p] == 'E');
            return res!.Length.ToString();
        }

        private IEnumerable<Point> Expand(Point p)
            => _grid.Get4NeighborsOf(p)
                .Where(n => HeightOf(n) <= HeightOf(p) + 1);

        private int HeightOf(Point p) => _grid[p] switch
        {
            'S' => 'a',
            'E' => 'z',
            char c => c
        };

        public override async ValueTask<string> Solve_2()
        {
            var start = _grid.FindFirst('E');
            var res = new BreadthFirstSearch<Point>(null, ExpandP2) { PerformParallelSearch = false }
                .FindFirst(start, p => HeightOf(p) == 'a');
            return res!.Length.ToString();
        }

        private IEnumerable<Point> ExpandP2(Point p)
            => _grid.Get4NeighborsOf(p)
                .Where(n => HeightOf(p) <= HeightOf(n) + 1);
    }
}