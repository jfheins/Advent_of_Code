using Core;
using Core.Combinatorics;
using MoreLinq.Extensions;
using System.IO;
using System.Linq;

namespace AoC_2021.Days
{
    public class Day_06 : BaseDay
    {
        private string[] _input;
        private int[] _numbers;

        public Day_06()
        {
            _input = File.ReadAllLines(InputFilePath).ToArray();
            _numbers = _input.Select(int.Parse).ToArray();
        }

        public override async ValueTask<string> Solve_1()
        {
            return "";
        }

        public override async ValueTask<string> Solve_2()
        {
            return "";
        }
    }
}
