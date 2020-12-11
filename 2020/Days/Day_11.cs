using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;
using System.Transactions;

using Core;

namespace AoC_2020.Days
{
    public class Day_11 : BaseDay
    {
        private FiniteGrid2D<char> grid;

        public Day_11()
        {
            grid = Grid2D.FromFile(InputFilePath);
        }

        public override string Solve_1()
        {
            var hasChanged = false;
            do
            {
                hasChanged = false;
                var newgrid = new FiniteGrid2D<char>(grid);
                foreach (var (pos, value) in grid.Where(cell => cell.value != '.'))
                {
                    var newv = Life(pos);
                    if (newv != null)
                    {
                        newgrid[pos] = newv.Value;
                        hasChanged = true;
                    }
                }
                grid = newgrid;
            } while (hasChanged);

            return grid.Count(cell => cell.value == '#').ToString();
        }

        public override string Solve_2()
        {
            grid = Grid2D.FromFile(InputFilePath);
            var hasChanged = false;
            do
            {
                hasChanged = false;
                var newgrid = new FiniteGrid2D<char>(grid);
                foreach (var (pos, value) in grid.Where(cell => cell.value != '.'))
                {
                    var newv = Life2(pos);
                    if (newv != null)
                    {
                        newgrid[pos] = newv.Value;
                        hasChanged = true;
                    }
                }
                grid = newgrid;
            } while (hasChanged);

            return grid.Count(cell => cell.value == '#').ToString();
        }

        private char? Life(Point pos)
        {   
            /*
            If a seat is empty (L) and there are no occupied seats adjacent to it, the seat becomes occupied.
            If a seat is occupied (#) and four or more seats adjacent to it are also occupied, the seat becomes empty.
            Otherwise, the seat's state does not change.
            */
            var occupiedN = grid.Get8NeighborsOf(pos).Count(c => grid[c] == '#');
            if (grid[pos] == 'L' && occupiedN == 0)
            {
                return '#';
            }
            if (grid[pos] == '#' && occupiedN >= 4)
            {
                return 'L';
            }
            return null;
        }

        private Dictionary<Point, Point[]> NeighborCache = new();

        private Point[] GetLineOfSightNeighbors(Point p)
        {
            var sizes = new[] {  new Size(-1, -1), new Size(0, -1),  new Size(1, -1),
                new Size(-1, 0), new Size(1, 0),
                new Size(-1, 1), new Size(0, 1),  new Size(1, 1),
            };

            return sizes.Select(dir =>
                    grid.Line(p, dir)
                        .Cast<Point?>()
                        .FirstOrDefault(p => grid[p!.Value] != '.'))
                    .WhereNotNull().ToArray();
        }

        private char? Life2(Point pos)
        {
            /*
            it now takes five or more visible occupied seats for an occupied seat to become empty
            (rather than four or more from the previous rules). The other rules still apply:
            empty seats that see no occupied seats become occupied, seats matching no rule don't change, and floor never changes.
            */
            var occupiedN = NeighborCache.GetOrAdd(pos, GetLineOfSightNeighbors).Count(p => grid[p] == '#');

            if (grid[pos] == 'L' && occupiedN == 0)
            {
                return '#';
            }
            if (grid[pos] == '#' && occupiedN >= 5)
            {
                return 'L';
            }
            return null;
        }
    }
}

