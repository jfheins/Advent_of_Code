using Core;
using System.Drawing;

namespace AoC_2021.Days
{
    public class Day_20 : BaseDay
    {
        private string _algo;
        private FiniteGrid2D<char> _img;

        public Day_20()
        {
            var input = File.ReadAllLines(InputFilePath).ToArray();
            _algo = input[0];
            _img = Grid2D.FromArray(input, 2.., ..);
        }

        public override async ValueTask<string> Solve_1()
        {
            var grid = _img;
            var infChar = '.';
            var infCharToggle = _algo[0] == '#';
            for (int i = 0; i < 2; i++)
            {
                grid = new FiniteGrid2D<char>(grid, 1, infChar);
                var nextgrid = new FiniteGrid2D<char>(grid);
                foreach (var (pos, _) in grid)
                {
                    var area = AreaToIndex(grid.GetPointWith8Neighbors(pos, infChar));
                    nextgrid[pos] = _algo[area];
                }
                grid = nextgrid;
                if (infCharToggle)
                    infChar = infChar == '.' ? '#' : '.';
            }
            //Console.WriteLine(grid.ToString());
            
            return grid.Count(cell => cell.value == '#').ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var grid = _img;
            var infChar = '.';
            var infCharToggle = _algo[0] == '#';
            for (int i = 0; i < 50; i++)
            {
                var inflated = grid.Bounds.InflatedCopy(1, 1);
                grid = new FiniteGrid2D<char>(inflated, Step);

                if (infCharToggle)
                    infChar = infChar == '.' ? '#' : '.';
            }
            //Console.WriteLine(grid.ToString());

            return grid.Count(cell => cell.value == '#').ToString();

            char Step(Point p)
            {
                var area = grid.GetPointWith8Neighbors(p, infChar);
                return _algo[AreaToIndex(area)];
            }
        }

        private static int AreaToIndex(IEnumerable<char> pixels)
        {
            var idx = 0;
            foreach (var chr in pixels)
            {
                idx = idx << 1 | (chr == '#' ? 1 : 0);
            }
            return idx;
        }
    }
}
