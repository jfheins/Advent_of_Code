using Core;
using Core.Combinatorics;
using MoreLinq.Extensions;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AoC_2021.Days
{
    public class Day_05 : BaseDay
    {
        private string[] _input;

        public Day_05()
        {
            _input = File.ReadAllLines(InputFilePath).ToArray();
        }

        public override async ValueTask<string> Solve_1()
        {
            var lines = _input.Select(l => new Line(l.ParseInts(4))).ToList();

            var allpoints = lines.Where(line => line.A.X == line.B.X || line.A.Y == line.B.Y)
                .SelectMany(l => l.AllPoints()).ToList();
            var manyPoints = allpoints.GroupBy(p => p).Where(grp => grp.Count() > 1);

            return manyPoints.Count().ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var lines = _input.Select(l => new Line(l.ParseInts(4))).ToList();

            var allpoints = lines.SelectMany(l => l.AllPoints()).ToList();
            var manyPoints = allpoints.GroupBy(p => p).Where(grp => grp.Count() > 1);

            return manyPoints.Count().ToString();
        }

        public record Line
        {
            public Point A { get; set; }
            public Point B { get; set; }

            public Line(int[] numbers)
            {
                // 767,159 -> 180,159
                A = new Point(numbers[0], numbers[1]);
                B = new Point(numbers[2], numbers[3]);
            }

            public IEnumerable<Point> AllPoints()
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
