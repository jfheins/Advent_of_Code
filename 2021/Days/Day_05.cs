using Core;
using System.Drawing;

namespace AoC_2021.Days
{
    public class Day_05 : BaseDay
    {
        private ILookup<bool, Line> _lines;
        private byte[] grid = new byte[1000 * 1000];

        public Day_05()
        {
            _lines = File.ReadAllLines(InputFilePath)
                .Select(Line.FromString).ToLookup(l => l.IsStraight);
        }

        public override async ValueTask<string> Solve_1()
        {
            foreach (var line in _lines[true])
                foreach (var p in line.AllPoints)
                    grid[p.X + p.Y * 1000]++;

            return grid.Count(x => x > 1).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            foreach (var line in _lines[false])
                foreach (var p in line.AllPoints)
                    grid[p.X + p.Y * 1000]++;

            return grid.Count(x => x > 1).ToString();
        }

        public sealed record Line
        {
            public Point A { get; set; }
            public Point B { get; set; }
            public IReadOnlyList<Point> AllPoints { get; }

            public bool IsStraight => A.X == B.X || A.Y == B.Y;

            public Line(int ax, int ay, int bx, int by)
            {
                A = new Point(ax, ay);
                B = new Point(bx, by);

                AllPoints = CalcPoints().ToList();
            }

            private IEnumerable<Point> CalcPoints()
            {
                var dx = Math.Sign(B.X - A.X);
                var dy = Math.Sign(B.Y - A.Y);
                var steps = Math.Max(Math.Abs(B.X - A.X), Math.Abs(B.Y - A.Y));

                yield return A;
                for (int i = 1; i <= steps; i++)
                {
                    yield return A + new Size(dx * i, dy * i);
                }
            }

            internal static Line FromString(string line)
            {
                // 767,159 -> 180,159
                var parts = line.Replace(" -> ", ",").Split(",").SelectArray(int.Parse);
                return new Line(parts[0], parts[1], parts[2], parts[3]);
            }
        }
    }
}
