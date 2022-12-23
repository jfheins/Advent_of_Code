using Core;

using System.Drawing;

namespace AoC_2022.Days
{
    public sealed class Day_23 : BaseDay
    {
        private FiniteGrid2D<char> _map;
        private readonly string[] _input;

        public Day_23()
        {
            _input = File.ReadAllLines(InputFilePath);
            _map = new FiniteGrid2D<char>(_input);
        }

        public override async ValueTask<string> Solve_1()
        {
            _map.RemoveWhere(p => !HasElf(p));
            var directions = new List<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
            for (int i = 0; i < 10; i++)
            {
                var proposed = new List<(Point from, Point to)>();
                foreach (var elf in _map.Where(t => t.value == '#'))
                {
                    var n = _map.Get8NeighborsOf(elf.pos);
                    if(n.Any(HasElf))
                    {
                        foreach (var dir in directions)
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
                }
                foreach (var (from, to) in proposed.ToLookup(it => it.to).Where(it => it.Count() == 1).Select(it => it.First()))
                {
                    _map.RemoveAt(from);
                    _map[to] = '#';
                }
                directions.Add(directions.First());
                directions.RemoveAt(0);
                _map.SizeToFit();
                proposed.Clear();
            }
            var c1 = _map.Count;
            _map.SizeToFit();
            _map.FillGaps('.');
            var c = _map.Count;

            return _map
                .Count(t => t.value == '.')
                .ToString();

            bool HasElf(Point p) => _map.GetValueOrDefault(p, '.') == '#';
        }

        public override async ValueTask<string> Solve_2()
        {
            _map = new FiniteGrid2D<char>(_input);
            _map.RemoveWhere(p => !HasElf(p));
            var directions = new List<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
            var rounds = 1;
            while(true)
            {
                var proposed = new List<(Point from, Point to)>();
                foreach (var elf in _map.Where(t => t.value == '#'))
                {
                    var n = _map.Get8NeighborsOf(elf.pos);
                    if (n.Any(HasElf))
                    {
                        foreach (var dir in directions)
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
                }
                foreach (var (from, to) in proposed.ToLookup(it => it.to).Where(it => it.Count() == 1).Select(it => it.First()))
                {
                    _map.RemoveAt(from);
                    _map[to] = '#';
                }
                directions.Add(directions.First());
                directions.RemoveAt(0);
                _map.SizeToFit();
                if (proposed.Any())
                    proposed.Clear();
                else
                    break;
                rounds++;
            }
           
            return rounds.ToString();

            bool HasElf(Point p) => _map.GetValueOrDefault(p, '.') == '#';
        }
    }
}