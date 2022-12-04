using Core;

namespace AoC_2022.Days
{
    public sealed class Day_04 : BaseDay
    {
        private string[] _input;

        public Day_04()
        {
            _input = File.ReadAllLines(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            var xx = _input.Select(line => line.ParseNNInts(4))
                .Count(pair => (pair[2] >= pair[0] && pair[2] <= pair[1]
                && pair[3] >= pair[0] && pair[3] <= pair[1])
                || (pair[0] >= pair[2] && pair[0] <= pair[3]
                && pair[1] >= pair[2] && pair[1] <= pair[3]));

            return (xx).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var xx = _input.Select(line => line.ParseNNInts(4))
                .Count(pair => (pair[2] >= pair[0] && pair[2] <= pair[1])
                || (pair[3] >= pair[0] && pair[3] <= pair[1])
                || (pair[0] >= pair[2] && pair[0] <= pair[3])
                || (pair[1] >= pair[2] && pair[1] <= pair[3]));

            return (xx).ToString();
        }
    }
}