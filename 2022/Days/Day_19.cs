using Core;

namespace AoC_2022.Days
{
    public sealed class Day_19 : BaseDay
    {
        private readonly IReadOnlyList<string> _input;

        public Day_19()
        {
            _input = File.ReadAllLines(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            return "-";
        }

        public override async ValueTask<string> Solve_2()
        {
            return "2";
        }
    }
}