using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Core;

namespace AoC_2020.Days
{
    public class Day_17 : BaseDay
    {
        private Dictionary<Point3, char> grid = new();
        private Dictionary<Point4, char> grid2 = new();

        public Day_17()
        {
            var content = File.ReadAllLines(InputFilePath);
            foreach (var (x, y, value) in content.WithXY())
                grid[new Point3(x, y, 0)] = value;

            foreach (var (x, y, value) in content.WithXY())
                grid2[new Point4(x, y, 0, 0)] = value;
        }

        private IEnumerable<Point3> Neighbors(Point3 p)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        if (dx != 0 || dy != 0 || dz != 0)
                        {
                            yield return p.TranslateBy(dx, dy, dz);
                        }
                    }
                }
            }
        }

        private char Life(Point3 p)
        {
            /*
If a cube is active and exactly 2 or 3 of its neighbors are also active,
            the cube remains active. Otherwise, the cube becomes inactive.
If a cube is inactive but exactly 3 of its neighbors are active, the cube becomes active. Otherwise, the cube remains inactive.
             */
            var activeNeighb = Neighbors(p).Count(n => grid.GetValueOrDefault(n, '.') == '#');
            if (grid.GetValueOrDefault(p, '.') == '#')
                return (activeNeighb == 2 || activeNeighb == 3) ? '#' : '.';
            else
                return activeNeighb == 3 ? '#' : '.';
        }

        public override string Solve_1()
        {
            var newgrid = new Dictionary<Point3, char>();
            var lim = 20;

            for (int i = 0; i < 6; i++)
            {
                for (int x = -lim; x <= lim; x++)
                {
                    for (int y = -lim; y <= lim; y++)
                    {
                        for (int z = -lim; z <= lim; z++)
                        {
                            var newCell = Life(new Point3(x, y, z));
                            if (newCell == '#')
                                newgrid[new Point3(x, y, z)] = '#';
                        }
                    }
                }


                grid = newgrid;
                newgrid = new Dictionary<Point3, char>();
            }


            return grid.Count(x => x.Value == '#').ToString();
        }
        private IEnumerable<Point4> Neighbors2(Point4 p)
        {
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                    for (int dz = -1; dz <= 1; dz++)
                        for (int dw = -1; dw <= 1; dw++)
                            if (dx != 0 || dy != 0 || dz != 0 || dw != 0)
                                yield return p.TranslateBy(dx, dy, dz, dw);
        }

        private char Life2(Point4 p)
        {
            /*
If a cube is active and exactly 2 or 3 of its neighbors are also active,
            the cube remains active. Otherwise, the cube becomes inactive.
If a cube is inactive but exactly 3 of its neighbors are active, the cube becomes active. Otherwise, the cube remains inactive.
             */
            var activeNeighb = Neighbors2(p).Count(n => grid2.GetValueOrDefault(n, '.') == '#');
            if (grid2.GetValueOrDefault(p, '.') == '#')
                return (activeNeighb == 2 || activeNeighb == 3) ? '#' : '.';
            else
                return activeNeighb == 3 ? '#' : '.';
        }

        public override string Solve_2()
        {
            var newgrid = new Dictionary<Point4, char>();
            var lim = 15;

            for (int i = 0; i < 6; i++)
            {
                for (int x = -lim; x <= lim; x++)
                {
                    for (int y = -lim; y <= lim; y++)
                    {
                        for (int z = -lim; z <= lim; z++)
                        {
                            for (int w = -lim; w <= lim; w++)
                            {
                                var newCell = Life2(new Point4(x, y, z, w));
                                if (newCell == '#')
                                    newgrid[new Point4(x, y, z, w)] = '#';
                            }
                        }
                    }
                    Console.Write(x.ToString() + " ");
                }
                Console.WriteLine();

                grid2 = newgrid;
                newgrid = new Dictionary<Point4, char>();
            }


            return grid2.Count(x => x.Value == '#').ToString();
        }
    }
}

