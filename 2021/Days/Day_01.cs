using System.IO;
using System.Linq;

using Core;
using Core.Combinatorics;

namespace AoC_2021.Days
{
    public class Day_01 : BaseDay
    {
        private readonly int[] _input;

        public Day_01()
        {
            _input = File.ReadAllLines(InputFilePath).Select(int.Parse).ToArray();
        }

        public override async ValueTask<string> Solve_1() => new FastCombinations<int>(_input, 2)
            .Where(x => x.Sum() == 2020)
            .Select(x => x.Product())
            .First().ToString();

        public override async ValueTask<string> Solve_2() => new FastCombinations<int>(_input, 3)
            .Where(x => x.Sum() == 2020)
            .Select(x => x.Product())
            .First().ToString();
    }
}
