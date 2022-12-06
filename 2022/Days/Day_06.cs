using Core;

using static MoreLinq.Extensions.WindowExtension;

namespace AoC_2022.Days
{
    public sealed class Day_06 : BaseDay
    {
        private readonly string _input;

        public Day_06()
        {
            _input = File.ReadAllText(InputFilePath);
        }

        public override async ValueTask<string> Solve_1() => Solve(4).ToString();
        public override async ValueTask<string> Solve_2() => Solve(14).ToString();
        private int Solve(int wndSize) => _input.Window(wndSize).IndexWhere(x => x.AreAllDistinct()).First() + wndSize;
    }
}