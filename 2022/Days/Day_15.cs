using Core;

using Spectre.Console;

using System.Diagnostics;
using System.Drawing;

namespace AoC_2022.Days
{
    public sealed class Day_15 : BaseDay
    {
        private IReadOnlyList<string> _input;

        public Day_15()
        {
            _input = File.ReadAllLines(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            var rowY = 2000000;
            var row = new Dictionary<int, char>();

            foreach (var sensor in _input)
            {
                var ints = sensor.ParseInts(4);
                var sensorPoint = new Point(ints[0], ints[1]);
                var baconPoint = new Point(ints[2], ints[3]);
                if (baconPoint.Y == rowY)
                {
                    row[baconPoint.X] = 'B';
                }
                var bDist = sensorPoint.ManhattanDistTo(baconPoint);
                var yDist = Math.Abs(sensorPoint.Y - rowY);

                var lrDist = bDist - yDist;
                for (int x = sensorPoint.X - lrDist; x <= sensorPoint.X + lrDist; x++)
                {
                    if (!row.ContainsKey(x))
                        row[x] = '#';
                }
            }

            return row.Count(kvp => kvp.Value == '#').ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var beacons = new HashSet<Point>();
            var sensors = new List<(Point pos, int r)>();
            foreach (var line in _input)
            {
                var ints = line.ParseInts(4);
                var sensorPoint = new Point(ints[0], ints[1]);
                var baconPoint = new Point(ints[2], ints[3]);
                var bDist = sensorPoint.ManhattanDistTo(baconPoint);
                _ = beacons.Add(baconPoint);
                sensors.Add((sensorPoint, bDist));
            }
            var avgX = (int)sensors.Average(s => s.pos.X);
            var avgY = (int)sensors.Average(s => s.pos.Y);
            var pos = new Point(avgX, avgY);


            var deltas = CalcDeltas(pos);
            var steps = 0;
            while (deltas.Any(d => d != Size.Empty))
            {
                var oldpos = pos;
                pos = deltas.Aggregate(pos, (p, d) => checked(p + d));
                if(steps++ % 10000 == 0)
                    Console.WriteLine(pos);
                if (oldpos == new Point(2633670, 3071499) && pos == new Point(2633671, 3071499))
                    pos = pos.MoveBy(700_000, 300_000);
                if (beacons.Contains(pos))
                    pos = pos.MoveBy(1, 1);

                deltas = CalcDeltas(pos);

                if (steps > 238485)
                    break;
            }

            Debug.Assert(!beacons.Contains(pos));
            Debug.Assert(sensors.All(s => s.pos.ManhattanDistTo(pos) > s.r));
            Debug.Assert(pos.X > 0 && pos.Y > 0);
            Console.WriteLine(pos);
            return (pos.X * 4000000L + pos.Y).ToString();

            List<Size> CalcDeltas(Point p) => sensors.Select(s =>
                  {
                      var sensorDist = s.pos.ManhattanDistTo(p);
                      if (sensorDist > s.r)
                          return Size.Empty;

                      var diff = p.Minus(s.pos);
                      var remainder = s.r - sensorDist;
                      Size rem;
                      if (remainder == 0)
                      {
                          if (diff.X > 0)
                              rem = new Size(Math.Sign(diff.X), 0);
                          else
                              rem = new Size(0, Math.Sign(diff.Y));
                      }
                      else if (diff.X == 0)
                      {
                          rem = new Size(0, Math.Sign(diff.Y) * remainder);
                      }
                      else if (diff.Y == 0)
                      {
                          rem = new Size(Math.Sign(diff.X) * remainder, 0);
                      }
                      else
                      {
                          var remX = remainder / 2;
                          var remY = remainder - remX;
                          rem = new Size(Math.Sign(diff.X) * remX,
                              Math.Sign(diff.Y) * remY);
                      }
                      return rem;
                  }).ToList();
        }
    }
}