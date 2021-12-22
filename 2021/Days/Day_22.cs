using Core;
using Core.Combinatorics;
using MoreLinq.Extensions;
using System.IO;
using System.Linq;
using System.Numerics;

namespace AoC_2021.Days
{
    public class Day_22 : BaseDay
    {
        private string[] _input;
        private (string cmd, int[] area)[] _steps;

        public Day_22()
        {
            _input = File.ReadAllLines(InputFilePath).ToArray();
            _steps = _input.Select(line => (cmd: line[0..2], area: line.ParseInts(6))).ToArray();
        }

        public override async ValueTask<string> Solve_1()
        {
            var aoi = new Dictionary<Point3, bool>();
            var area2 = new Cube(new Point3(-50, -50, -50), 101);

            foreach (var (cmd, area) in _steps)
            {
                var minx = Math.Max(area[0], area2.BottomLeft.X);
                var maxx = Math.Min(area[1], area2.TopRight.X);
                for (int x = minx; x <= maxx; x++)
                {
                    var miny = Math.Max(area[2], area2.BottomLeft.Y);
                    var maxy = Math.Min(area[3], area2.TopRight.Y);
                    for (int y = miny; y <= maxy; y++)
                    {
                        var minz = Math.Max(area[4], area2.BottomLeft.Z);
                        var maxz = Math.Min(area[5], area2.TopRight.Z);
                        for (int z = minz; z <= maxz; z++)
                        {
                            var p = new Point3(x, y, z);
                            if (cmd == "on")
                                aoi[p] = true;
                            else
                                aoi[p] = false;
                        }
                    }
                }
            }
            return aoi.Where(kvp => area2.Contains(kvp.Key)).Count(kvp => kvp.Value).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var volumes = new List<(Cuboid p, int sign)>();
            foreach (var (cmd, area) in _steps)
            {
                var changeArea = new Cuboid
                {
                    Location = new Point3(area[0], area[2], area[4]),
                    Width = area[1] - area[0] + 1,
                    Height = area[3] - area[2] + 1,
                    Depth = area[5] - area[4] + 1
                };

                // Level the playing field and "remove" any existing on areas by adding
                // them with opposite sign.
                // in case some areas have also been switched off later, we need to cancel that as well
                foreach (var (p, sign) in volumes.ToList())
                {
                    var intersection = p.Intersect(changeArea);
                    if (intersection != null)
                    {
                        var cancelSign = -sign;
                        volumes.Add((intersection, cancelSign));
                    }
                }
                if (cmd == "on")
                    volumes.Add((changeArea, 1));
            }

            var sum = volumes.Aggregate(BigInteger.Zero, (sum, x) => sum + x.p.Size * x.sign);
            return sum.ToString();
        }
    }
}
