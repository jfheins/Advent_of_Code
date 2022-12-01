using Core;

using MoreLinq;

namespace AoC_2022.Days
{
    public sealed class Day_01 : BaseDay
    {
        private readonly string[] _input;

        public Day_01()
        {
            _input = File.ReadAllLines(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            var blocks = _input.Split("");
            var summed = blocks.Select(block => block.Select(line => int.Parse(line)).Sum());
            return summed.Max().ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var blocks = _input.Split("");
            var summed = blocks.Select(block => block.Select(line => int.Parse(line)).Sum());
            var topThree = summed.OrderDescending().Take(3).ToList();
            return topThree.Sum().ToString();
        }
    }
}