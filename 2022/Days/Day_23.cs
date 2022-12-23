using Core;

namespace AoC_2022.Days
{
    public sealed class Day_23 : BaseDay
    {
        private readonly FiniteGrid2D<char> _map;
        private readonly string[] _input;

        public Day_23()
        {
            _input = File.ReadAllLines(InputFilePath);
          //  _map = new FiniteGrid2D<char>(_input[0..^1]);
        }

        public override async ValueTask<string> Solve_1()
        {
            return "-";
        }

        public override async ValueTask<string> Solve_2()
        {
            return "--";
        }
    }
}