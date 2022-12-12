using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;

using Core;

using Microsoft.VisualBasic.FileIO;

namespace AoC_2022.Days
{
    public sealed class Day_12 : BaseDay
    {
        private readonly List<string> _input;
        private readonly List<int[]> _ints;
        private readonly FiniteGrid2D<char> _grid;

        public Day_12()
        {
            _input = File.ReadAllLines(InputFilePath).ToList();
           // _ints = _input.SelectList(line => line.ParseInts());
            _grid = new FiniteGrid2D<char>(_input);
        }

        public override async ValueTask<string> Solve_1()
        {
            var s = new DijkstraSearch<Point>(null, Expand);
            var start = _grid.First(t => t.value == 'S');
            var res = s.FindFirst(start.pos, p => _grid[p] == 'E');
            return res.Length.ToString();
        }

        private IEnumerable<(Point node, float cost)> Expand(Point p)
        {
            var current = HeightOf(_grid[p]);
            return _grid.Get4NeighborsOf(p)
                .Where(n => HeightOf(_grid[n]) <= current + 1)
                .Select(n => (n, 1f));

            int HeightOf(char x) => x == 'S' ? 'a' : (x == 'E' ? 'z' : (int)x);
        }

        public override async ValueTask<string> Solve_2()
        {
            var s = new DijkstraSearch<Point>(null, ExpandP2);
            var start = _grid.First(t => t.value == 'E');
            var res = s.FindFirst(start.pos, p => _grid[p] == 'a');
            return res.Length.ToString();
        }

        private IEnumerable<(Point node, float cost)> ExpandP2(Point p)
        {
            var current = HeightOf(_grid[p]);
            return _grid.Get4NeighborsOf(p)
                .Where(n => HeightOf(_grid[n]) >= current - 1)
                .Select(n => (n, 1f));

            int HeightOf(char x) => x == 'S' ? 'a' : (x == 'E' ? 'z' : (int)x);
        }
    }
}