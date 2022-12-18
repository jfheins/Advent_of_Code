using Core;
using Core.Combinatorics;

using System.Diagnostics;

namespace AoC_2022.Days
{
    public sealed class Day_18 : BaseDay
    {
        private readonly IReadOnlyList<int[]> _input;

        public Day_18()
        {
            _input = File.ReadAllLines(InputFilePath).SelectList(l => l.ParseInts(3));
        }

        public override async ValueTask<string> Solve_1()
        {
            rockSides = _input.Count * 6;
            foreach (var pair in new FastCombinations<int[]>(_input, 2))
            {
                var dx = pair[0][0] - pair[1][0];
                var dy = pair[0][1] - pair[1][1];
                var dz = pair[0][2] - pair[1][2];
                if (dx * dx + dy * dy + dz * dz == 1)
                {
                    rockSides -= 2;
                }
            }

            return rockSides.ToString();
        }

        int rockSides = 0;


        public override async ValueTask<string> Solve_2()
        {
            var rockCubes = _input.SelectList(it => new Point3(it[0], it[1], it[2]));

            var xrange = rockCubes.MinMax(c => c.X)!.Value;
            var yrange = rockCubes.MinMax(c => c.Y)!.Value;
            var zrange = rockCubes.MinMax(c => c.Z)!.Value;
            var xlen = (xrange.max - xrange.min + 2);
            var ylen = (yrange.max - yrange.min + 2);
            var zlen = (zrange.max - zrange.min + 2);

            var cubes = new bool[xlen + xrange.min, ylen + yrange.min, zlen + zrange.min]; // true = rock or outside, false = unknown
            foreach (var c in rockCubes)
            {
                cubes[c.X, c.Y, c.Z] = true;
            }
            Debug.Assert(cubes[xrange.min, yrange.min, zrange.min] == false);

            var bfs = new BreadthFirstSearch<Point3>(null, p =>
            {
                cubes[p.X, p.Y, p.Z] = true;
                return Get6N(p).Where(InRange).Where(p => !cubes[p.X, p.Y, p.Z]);
            })
            { PerformParallelSearch = false };
            bfs.FindLeafs(new Point3(0, 0, 0));

            var inner = new List<Point3>();
            for (int x = xrange.min; x <= xrange.max; x++)
                for (int y = yrange.min; y <= yrange.max; y++)
                    for (int z = zrange.min; z <= zrange.max; z++)
                    {
                        if (!cubes[x, y, z])
                            inner.Add(new Point3(x, y, z));
                    }
            var innerFaces = inner.Count * 6;
            if (inner.Count >= 3)
                foreach (var pair in new FastCombinations<Point3>(inner, 2))
                {
                    var dx = pair[0].X - pair[1].X;
                    var dy = pair[0].Y - pair[1].Y;
                    var dz = pair[0].Z - pair[1].Z;
                    if (dx * dx + dy * dy + dz * dz == 1)
                    {
                        innerFaces -= 2;
                    }
                }

            return (rockSides - innerFaces).ToString();

            bool InRange(Point3 p)
            {
                return p.X >= 0 && p.Y >= 0 && p.Z >= 0
                    && p.X <= xrange.max + 1 && p.Y <= yrange.max + 1 && p.Z <= zrange.max + 1;
            }
        }

        private IEnumerable<Point3> Get6N(Point3 p)
        {

            yield return p.TranslateBy(1, 0, 0);
            yield return p.TranslateBy(-1, 0, 0);
            yield return p.TranslateBy(0, 1, 0);
            yield return p.TranslateBy(0, -1, 0);
            yield return p.TranslateBy(0, 0, 1);
            yield return p.TranslateBy(0, 0, -1);

        }
    }
}