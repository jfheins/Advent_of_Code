using System.IO;
using System.Linq;

using Core;
using Core.Combinatorics;

namespace AoC_2020.Days
{
    public class Day_01 : BaseDay
    {
        private readonly int[] _input;

        public Day_01()
        {
            _input = File.ReadAllLines(InputFilePath).Select(int.Parse).ToArray();
        }

        public override string Solve_1() => new ArrayCombinations<int>(_input, 2)
            .Where(x => x.Sum() == 2020)
            .Select(x => x.Product())
            .First().ToString();

        public override string Solve_2() => new ArrayCombinations<int>(_input, 3)
            .Where(x => x.Sum() == 2020)
            .Select(x => x.Product())
            .First().ToString();
    }
}
