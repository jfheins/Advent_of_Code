using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using Core;

namespace AoC_2020.Days
{
    public class Day_03 : BaseDay
    {
        private readonly WrappingXGrid2D<char> _input;

        public Day_03()
        {
            _input = new WrappingXGrid2D<char>(File.ReadAllLines(InputFilePath));
        }

        public override string Solve_1()
        {
            return CountTreesInSlope(3, 1).ToString();
        }

        private int CountTreesInSlope(int dx, int dy)
        {
            var origin = new Point();
            var delta = new Size(dx, dy);
            return Enumerable.Range(0, int.MaxValue)
                .Select(x => origin + (x * delta))
                .TakeWhile(_input.Contains)
                .Count(p => _input[p] == '#');
        }

        public override string Solve_2()
        {
            var paths = new List<long>
            {
                CountTreesInSlope(1, 1),
                CountTreesInSlope(3, 1),
                CountTreesInSlope(5, 1),
                CountTreesInSlope(7, 1),
                CountTreesInSlope(1, 2)
            };
            return paths.Product().ToString();
        }
    }
}
