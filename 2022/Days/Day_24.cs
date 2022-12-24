using Core;

using System.Collections.Concurrent;
using System.Drawing;

namespace AoC_2022.Days
{
    public sealed class Day_24 : BaseDay
    {
        private readonly FiniteGrid2D<char> _map;
        private List<(Point pos, Direction d)> _blizzards;
        private int steps = 0;
        private readonly int _gridInnerWidth;
        private readonly int _gridInnerHeight;
        private readonly Point startPoint;
        private readonly string[] _input;
        private readonly Point goalPoint;

        public Day_24()
        {
            _input = File.ReadAllLines(InputFilePath);
            _map = new FiniteGrid2D<char>(_input);
            _blizzards = _map.Where(it => "^v<>".Contains(it.value)).SelectList(x => (x.pos, Directions.Parse(x.value)));
            _gridInnerWidth = _map.Width - 2;
            _gridInnerHeight = _map.Height - 2;

            startPoint = new Point(1, 0);
            var goalY = _map.GetRowIndices().Last();
            var goalX = _map.GetRow(goalY, ' ').IndexWhere(x => x == '.').First();
            goalPoint = new Point(goalX, goalY);
        }

        public override async ValueTask<string> Solve_1()
        {
            var nodes = new HashSet<Point>() { startPoint};
            while (!nodes.Contains(goalPoint))
            {
                StepBlizzards(ref _blizzards, _map);
                nodes = nodes.SelectMany(Expander).ToHashSet();
                steps++;
            }

            return steps.ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var nodes = new HashSet<Point>() { goalPoint };
            while (!nodes.Contains(startPoint))
            {
                StepBlizzards(ref _blizzards, _map);
                nodes = nodes.SelectMany(Expander).ToHashSet();
                steps++;
            }
            nodes = new HashSet<Point>() { startPoint };
            while (!nodes.Contains(goalPoint))
            {
                StepBlizzards(ref _blizzards, _map);
                nodes = nodes.SelectMany(Expander).ToHashSet();
                steps++;
            }

            return steps.ToString();
        }

        private IEnumerable<Point> Expander(Point arg)
        {
            var n = _map.Get4NeighborsOf(arg);
            foreach (var n2 in n)
            {
                if (_map.GetValueOrDefault(n2, '.') == '.')
                {
                    yield return n2;
                }
            }
            if (_map.GetValueOrDefault(arg, '.') == '.')
                yield return arg; // Wait
        }

        private void StepBlizzards(ref List<(Point pos, Direction d)> blizzards, FiniteGrid2D<char> map)
        {
            foreach (var b in blizzards)
            {
                map.RemoveAt(b.pos);
            }

            blizzards = blizzards.Select(b =>
            {
                var pos = b.pos.MoveTo(b.d);
                if (map.GetValueOrDefault(pos, ' ') == '#')
                {
                    pos = new Point(pos.X.OneBasedModulo(_gridInnerWidth),
                        pos.Y.OneBasedModulo(_gridInnerHeight));
                }
                return (pos, b.d);
            }).ToList();

            foreach (var (pos, d) in blizzards)
            {
                map[pos] = 'x';
            }
        }
    }
}