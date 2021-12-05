using System.IO;
using System.Linq;

using Core;
using Core.Combinatorics;

using static MoreLinq.Extensions.WindowExtension;

namespace AoC_2021.Days
{
    public class Day_01 : BaseDay
    {
        private readonly int[] _input;

        public Day_01()
        {
            _input = File.ReadAllLines(InputFilePath).Select(int.Parse).ToArray();
        }

        public override async ValueTask<string> Solve_1()
            => _input.Diff().Count(x => x > 0).ToString();

        public override async ValueTask<string> Solve_2()
        {
            return (GetSum(0) + GetSum(1) + GetSum(2)).ToString();

            int GetSum(int offset)
                => _input.StepBy(3, offset).Diff().Count(x => x > 0);
        }
    }
}
