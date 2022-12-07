using Core;

using static MoreLinq.Extensions.WindowExtension;

namespace AoC_2022.Days
{
    public sealed class Day_06 : BaseDay
    {
        private readonly byte[] _input;

        public Day_06()
        {
            _input = File.ReadAllBytes(InputFilePath);
        }

        public override async ValueTask<string> Solve_1() => Solve(4).ToString();
        public override async ValueTask<string> Solve_2() => Solve(14).ToString();
        private int Solve(int wndSize) => _input.WindowArr(wndSize).IndexWhere(x => x.AreAllDistinct2()).First() + wndSize;
    }
}