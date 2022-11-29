using Core;

namespace AoC_2021.Days
{
    public class Day_02 : BaseDay
    {
        private readonly (string d, int val)[] _input;

        public Day_02()
        {
            _input = File.ReadAllLines(InputFilePath)
                .MatchRegexGroups2<string, int>(@"(.+) (\d+)").ToArray();
        }

        public override async ValueTask<string> Solve_1()
        {
            var pos = 0;
            var depth = 0;

            foreach (var (dir, val) in _input)
            {
                if (dir == "forward")
                {
                    pos += val;
                }
                else if (dir == "down")
                {
                    depth += val;
                }
                else if (dir == "up")
                {
                    depth -= val;
                }
            }
            return (depth * pos).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var pos = 0;
            var depth = 0;
            var aim = 0;

            foreach (var (dir, val) in _input)
            {
                if (dir == "forward")
                {
                    pos += val;
                    depth += aim * val;
                }
                else if (dir == "down")
                {
                    aim += val;
                }
                else if (dir == "up")
                {
                    aim -= val;
                }
            }
            return (depth * pos).ToString();
        }
    }
}
