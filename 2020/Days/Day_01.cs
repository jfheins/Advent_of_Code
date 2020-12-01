using System.IO;
using System.Linq;

using Core;


namespace AoC_2020.Days
{
    public class Day_01 : BaseDay
    {
        private readonly int[] _input;

        public Day_01()
        {
            _input = File.ReadAllLines(InputFilePath).Select(int.Parse).ToArray();
        }

        public override string Solve_1() => _input.CartesianProduct(_input).Where(x => x.Item1 + x.Item2 == 2020).Select(x => x.Item1 * x.Item2).First().ToString();

        public override string Solve_2() => _input
            .CartesianProduct(_input)
            .CartesianProduct(_input)
            .Select(x => (x.Item1.Item1, x.Item1.Item2, x.Item2))
            .Where(x => x.Item1 + x.Item2 + x.Item3 == 2020)
            .Select(x => x.Item1 * x.Item2 * x.Item3).First().ToString();
    }
}
