using Core;
using System.Drawing;

namespace AoC_2021.Days
{
    public class Day_05 : BaseDay
    {
        private List<Line> _lines;

        public Day_05()
        {
            _lines = File.ReadAllLines(InputFilePath)
                .Select(l => new Line(l.ParseInts(4))).ToList();
        }

        public override async ValueTask<string> Solve_1()
        {
            var allpoints = _lines
                .Where(line => line.IsStraight)
                .SelectMany(l => l.AllPoints);
            var manyPoints = allpoints.GroupBy(p => p);

            return manyPoints.Count(grp => grp.Count() > 1).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var allpoints = _lines.SelectMany(l => l.AllPoints);
            var manyPoints = allpoints.GroupBy(p => p);

            return manyPoints.Count(grp => grp.Count() > 1).ToString();
        }

        public record Line
        {
            public Point A { get; set; }
            public Point B { get; set; }
            public IReadOnlyList<Point> AllPoints { get; }

            public bool IsStraight => A.X == B.X || A.Y == B.Y;

            public Line(int[] numbers)
            {
                // 767,159 -> 180,159
                A = new Point(numbers[0], numbers[1]);
                B = new Point(numbers[2], numbers[3]);

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
        }
    }
}
