using Core;

namespace AoC_2022.Days
{
    public sealed class Day_01 : BaseDay
    {
        private readonly int[] _input;

        public Day_01()
        {
            _input = File.ReadAllLines(InputFilePath).Select(int.Parse).ToArray();
        }

        public override async ValueTask<string> Solve_1()
        {
            return "";
        }

        public override async ValueTask<string> Solve_2()
        {
            return "";
        }
    }
}