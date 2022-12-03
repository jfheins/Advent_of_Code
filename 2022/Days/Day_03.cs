using Core;

namespace AoC_2022.Days
{
    public sealed class Day_03 : BaseDay
    {
        private string[] _input;
        private readonly string prio = " " + Constants.LowerCaseAbc + Constants.UpperCaseAbc;

        public Day_03()
        {
            _input = File.ReadAllLines(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            return _input.Sum(CalcPriority).ToString();

            int CalcPriority(string line)
            {
                var half = line.Length / 2;
                var left = line[..half];
                var right = line[half..];
                var common = left.Intersect(right);
                return prio.IndexOfOrThrow(common.Single());
            }
        }

        public override async ValueTask<string> Solve_2()
        {
            return _input.Chunk(3).Sum(CalcPriority).ToString();

            int CalcPriority(string[] group)
            {
                var common = group[0].Intersect(group[1]).Intersect(group[2]);
                return prio.IndexOfOrThrow(common.Single());
            }
        }
    }
}