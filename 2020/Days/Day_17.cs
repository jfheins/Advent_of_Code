using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;

namespace AoC_2020.Days
{
    public class Day_17 : BaseDay
    {
        private readonly IEnumerable<(int x, int y, char value)> points;

        public Day_17()
        {
            var input = File.ReadAllLines(InputFilePath);
            points = input.WithXY().Where(x => x.value == '#');
        }

        public override string Solve_1()
        {
            var grid = new HashSet<Point3>(points.Select(p => new Point3(p.x, p.y, 0)));

            for (int i = 0; i < 6; i++)
                grid = Iterate(grid);

            return grid.Count.ToString();
        }

        public override string Solve_2()
        {
            var grid = new HashSet<Point4>(points.Select(p => new Point4(p.x, p.y, 0, 0)));

            for (int i = 0; i < 6; i++)
                grid = Iterate(grid);

            return grid.Count.ToString();
        }

        private static HashSet<T> Iterate<T>(HashSet<T> state) where T : ICanEnumerateNeighbors<T>
        {
            var relevantPoints = new HashSet<T>(state);
            foreach (var point in state)
                relevantPoints.UnionWith(point.GetNeighborsDiag());

            return new HashSet<T>(relevantPoints.AsParallel().Where(p => Life(p, state)));
        }

        private static bool Life<T>(T point, HashSet<T> grid) where T : ICanEnumerateNeighbors<T>
        {
            var activeNeighbors = point.GetNeighborsDiag().Count(n => grid.Contains(n));
            return grid.Contains(point) ? activeNeighbors == 2 || activeNeighbors == 3 : activeNeighbors == 3;
        }
    }
}

