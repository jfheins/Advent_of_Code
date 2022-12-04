using Core;

using System.Collections;
using System.Diagnostics;

namespace AoC_2022.Days
{
    public sealed class Day_04 : BaseDay
    {
        private List<(Interval left, Interval right)> _input;

        public Day_04()
        {
            _input = File.ReadAllLines(InputFilePath)
                .Select(line => line.ParseNNInts())
                .SelectList(it =>
                (Interval.FromInclusiveBounds(it[0], it[1]),
                Interval.FromInclusiveBounds(it[2], it[3])));
        }

        public override async ValueTask<string> Solve_1()
        {
            return _input.Count(t => t.left.Contains(t.right) || t.right.Contains(t.left)).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            return _input.Count(t => t.left.OverlapsWith(t.right)).ToString();
        }
    }
}