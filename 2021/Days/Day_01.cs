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
            return _input.Window(3)
                       .Select(x => x.Sum())
                       .Diff()
                       .Count(x => x > 0).ToString();
        }
    }
}
