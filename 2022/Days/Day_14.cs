using Core;

using Spectre.Console;

using System.Drawing;

namespace AoC_2022.Days
{
    public sealed class Day_14 : BaseDay
    {
        private readonly List<int[]> _numbers;
        private readonly FiniteGrid2D<char> _grid;

        public Day_14()
        {
            var input = File.ReadAllLines(InputFilePath);
            _numbers = input.SelectList(line => line.ParseInts());

            _grid = new FiniteGrid2D<char>(800, 200, ' ');
            foreach (var path in _numbers)
            {
                foreach (var (from, to) in path.Pairwise().PairwiseWithOverlap())
                {
                    var (minX, maxX) = LinqHelpers.MinMax(from.Item1, to.Item1);
                    var (minY, maxY) = LinqHelpers.MinMax(from.Item2, to.Item2);

                    for (int x = minX; x <= maxX; x++)
                    {
                        for (int y = minY; y <= maxY; y++)
                        {
                            _grid[x, y] = '#';
                        }
                    }
                }
            }
        }

        public override async ValueTask<string> Solve_1()
        {
            var sandSource = new Point(500, 0);
            var grid = new FiniteGrid2D<char>(_grid, 0, ' ');
            var units = 0;

            while (true)
            {
                var restPoint = DropSand(grid, sandSource);
                if (restPoint.HasValue)
                {
                    grid[restPoint.Value] = 'o';
                    units++;
                }
                else
                {
                    break;
                }
            }
            return units.ToString(); // 14826 too low
        }

        private static Point? DropSand(FiniteGrid2D<char> grid, Point sandSource)
        {
            bool atRest;
            do
            {
                atRest = true;
                if (grid[sandSource.MoveBy(0, 1)] == ' ')
                {
                    atRest = false;
                    sandSource = sandSource.MoveBy(0, 1);
                }
                else if (grid[sandSource.MoveBy(-1, 1)] == ' ')
                {
                    atRest = false;
                    sandSource = sandSource.MoveBy(-1, 1);
                }
                else if (grid[sandSource.MoveBy(1, 1)] == ' ')
                {
                    atRest = false;
                    sandSource = sandSource.MoveBy(1, 1);
                }
                if (sandSource.Y > 198)
                    return null;
            } while (!atRest);
            return sandSource;
        }

        public override async ValueTask<string> Solve_2()
        {
            var sandSource = new Point(500, 0);
            var gridWithFloor = new FiniteGrid2D<char>(_grid, 0, ' ');
            var maxy = gridWithFloor.Where(it => it.value == '#').Max(it => it.pos.Y);

            for (int x = 0; x < 800; x++)
            {
                gridWithFloor[x, maxy + 2] = '#';
            }

            var units = 0;

            while (true)
            {
                var restPoint = DropSand(gridWithFloor, sandSource);
                if(restPoint == sandSource)
                {
                    units++;
                    break;
                }
                else if (restPoint.HasValue)
                {
                    gridWithFloor[restPoint.Value] = 'o';
                    units++;
                }
                else
                {
                    break;
                }
            }
            return units.ToString();
        }
    }
}