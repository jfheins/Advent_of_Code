using Core;

using static MoreLinq.Extensions.SplitExtension;

namespace AoC_2022.Days
{
    public sealed class Day_01 : BaseDay
    {
        private List<int> _input;

        public Day_01()
        {
            _input = File.ReadAllLines(InputFilePath)
                .Split("")
                .SelectList(block => block.Select(int.Parse).Sum());
        }

        public override async ValueTask<string> Solve_1()
        {                
            return _input.Max().ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            return _input.OrderDescending().Take(3).Sum().ToString();
        }
    }
}