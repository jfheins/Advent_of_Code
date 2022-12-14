using Core;

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

            _grid = new FiniteGrid2D<char>(600, 200, ' ');
            foreach (var path in _numbers)
            {
                foreach (var (from, to) in path.Pairwise().PairwiseWithOverlap())
                {
                    var xx = new[] { from.Item1, to.Item1 }.MinMax()!.Value;
                    var yy = new[] { from.Item2, to.Item2 }.MinMax()!.Value;
                    for (int x = xx.min; x <= xx.max; x++)
                    {
                        for (int y = yy.min; y <= yy.max; y++)
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

            while(true)
            {
                try
                {
                    var restPoint = DropSand(grid, sandSource);
                    if (restPoint.HasValue)
                    {
                        grid[restPoint.Value] = 'o';
                        units++;
                        Console.WriteLine($"Sand {units} @ {restPoint}");
                    }
                    else
                        break;
                }
                catch (Exception)
                {
                    break;
                }
            }
            var x = grid.Count(it => it.value == 'o');

            return units.ToString();
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
                else if(grid[sandSource.MoveBy(-1, 1)] == ' ')
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
            return "-";
        }
    }
}