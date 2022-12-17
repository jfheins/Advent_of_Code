using Core;

using System.Diagnostics;
using System.Drawing;

namespace AoC_2022.Days
{
    public sealed class Day_17 : BaseDay
    {
        private string _input;

        private List<string> rocks = new List<string>
        {
            """
            ####
          """,
            """
             # 
            ###
             # 
          """,
            """
              #
              #
            ###
          """,
            """
            #
            #
            #
            #
          """,
            """
            ##
            ##
          """
        };

        public Day_17()
        {
            _input = File.ReadAllText(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            var rockIdx = 0;
            var grid = new FiniteGrid2D<char>(7, 3, ' ');

            for (int i = 0; i < 2022; i++)
            {
                var rock = rocks[rockIdx % 5].Replace('#', '@').Split("\r\n");
                grid.EnlargeTop(rock.Length);
                var y = grid.Bounds.Top;
                foreach (var line in rock)
                {
                    grid.SetRow(y++, line);
                }
                var shift = _input[rockIdx % _input.Length];
                Step(grid, shift);
                rockIdx++;
            }


            return "-";
        }

        Dictionary<char, Direction> dirmap = new() { { '>', Direction.Right }, { '<', Direction.Left } };

        private void Step(FiniteGrid2D<char> grid, char shift)
        {
            var points = grid.Where(kvp => kvp.value == '@').Select(kvp => kvp.pos).ToList();
            var shifted = points.SelectList(it => it.MoveTo(dirmap[shift]));

            var newPoints = points;
            if (shifted.All(FreeSpace))
            {
                newPoints = shifted;
            }
            var sunk = newPoints.SelectList(it => it.MoveTo(Direction.Down));
            char filler = '@';
            if (sunk.All(FreeSpace))
            {
                newPoints = sunk;
            }
            else
            {
                // arrest
                filler = '#';
            }
            foreach (var oldpoint in points)
            {
                grid[oldpoint] = ' ';
            }
            foreach (var p in newPoints)
            {
                grid[p] = filler;
            }

            bool FreeSpace(Point p) => grid.Bounds.Contains(p) && grid.GetValueOrDefault(p, ' ') != '#';
        }

        public override async ValueTask<string> Solve_2()
        {
            return "-";
        }
    }
}