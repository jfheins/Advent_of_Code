using Core;

using System.Drawing;

namespace AoC_2016.Days
{
    public class Day_02 : BaseDay
    {
        private readonly List<ICollection<Direction>> _input;

        public Day_02()
        {
            _input = File.ReadAllLines(InputFilePath).Select(ParseLine).ToList();
        }

        private static ICollection<Direction> ParseLine(string line)
            => line.Select(move => move switch
                {
                    'L' => Direction.Left,
                    'U' => Direction.Up,
                    'R' => Direction.Right,
                    'D' => Direction.Down,
                    _ => throw new NotImplementedException()
                }).ToList();

        public override async ValueTask<string> Solve_1()
        {
            var combination = "";
            var pos = new Point(1, 1);
            var guessedKeypad = new FiniteGrid2D<int>(Enumerable.Range(1, 9).Chunk(3));
            foreach (var line in _input)
            {
                foreach (var move in line)
                {
                    var newpos = pos.MoveTo(move);
                    pos = guessedKeypad.Contains(newpos) ? newpos : pos;
                }
                combination += guessedKeypad[pos];
            }
            return combination;
        }

        public override async ValueTask<string> Solve_2()
        {
            var combination = "";
            var pos = new Point(2, 2);
            var keypad = new FiniteGrid2D<char>(new[] {
                "    1    ",
                "   234   ",
                "  56789  ",
                "   ABC   ",
                "    D    "});

            foreach (var line in _input)
            {
                foreach (var move in line)
                {
                    var newpos = pos.MoveTo(move);
                    pos = keypad.GetValueOrDefault(newpos, ' ') != ' ' ? newpos : pos;
                }
                combination += keypad[pos];
            }
            return combination;
        }
    }
}
