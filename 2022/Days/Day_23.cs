using Core;

using System.Collections.Concurrent;
using System.Drawing;

namespace AoC_2022.Days
{
    public sealed class Day_23 : BaseDay
    {
        private readonly FiniteGrid2D<char> _map;
        private readonly List<Direction> _directions;
        private readonly string[] _input;

        public Day_23()
        {
            _input = File.ReadAllLines(InputFilePath);
            _map = new FiniteGrid2D<char>(_input);
            _map.RemoveWhere(p => !HasElf(p));
            _directions = new List<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
        }

        public override async ValueTask<string> Solve_1()
        {
            for (var i = 0; i < 10; i++)
                _ = PlayRound();

            _map.SizeToFit();
            _map.FillGaps('.');

            return _map.Count(t => t.value == '.')
                .ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            _map.RemoveWhere(p => !HasElf(p));
            var rounds = 11;
            while (PlayRound())
            {
                rounds++;
            }
            return rounds.ToString();
        }

        private bool PlayRound()
        {
            var proposed = new ConcurrentBag<(Point from, Point to)>();
            _ = Parallel.ForEach(_map.Where(t => t.value == '#'), elf =>
            {
                if (elf.pos.MoveLURDDiag().Any(it => HasElf(it)))
                {
                    foreach (var dir in _directions)
                    {
                        var one = elf.pos.MoveTo(dir);
                        var two = one.MoveTo(dir.TurnClockwise());
                        var three = one.MoveTo(dir.TurnCounterClockwise());

                        if (!HasElf(one) && !HasElf(two) && !HasElf(three))
                        {
                            proposed.Add((elf.pos, one));
                            break;
                        }
                    }
                }
            });
            var validMoves = proposed.ToLookup(it => it.to).Where(it => it.Count() == 1).Select(it => it.First());
            foreach (var (from, to) in validMoves)
            {
                _map.RemoveAt(from);
                _map[to] = '#';
            }
            _directions.Add(_directions.First());
            _directions.RemoveAt(0);

            return proposed.Any();
        }
        bool HasElf(Point p) => _map.GetValueOrDefault(p, '.') == '#';
    }
}