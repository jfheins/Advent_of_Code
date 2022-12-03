using Core;

using System.Diagnostics;

using static MoreLinq.Extensions.SplitExtension;

namespace AoC_2022.Days
{
    public sealed class Day_03 : BaseDay
    {
        private string[] _input;

        public Day_03()
        {
            _input = File.ReadAllLines(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            var prio = "_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            long sum = 0;
            foreach (var line in _input)
            {
                var half = line.Length / 2;
                var left = line.Take(half).ToHashSet();
                var right = line.Skip(half).ToHashSet();
                //Debug.Assert(left.Count() == right.Count());
                var common = left.Intersect(right);
                sum += prio.IndexOf(common.Single());
            }
            return sum.ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var prio = "_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var sum = 0;
            foreach (var group in _input.Chunk(3))
            {
                var common = group[0].Intersect(group[1]).Intersect(group[2]);
                sum += prio.IndexOf(common.Single());
            }
            return sum.ToString();
        }
    }
}