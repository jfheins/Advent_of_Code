using Core;
using Core.Combinatorics;

using System.Drawing;

using static MoreLinq.Extensions.IndexExtension;

namespace AoC_2022.Days
{
    public sealed class Day_18 : BaseDay
    {
        private readonly IReadOnlyList<int[]> _input;

        public Day_18()
        {
            _input = File.ReadAllLines(InputFilePath).SelectList(l => l.ParseInts(3));
        }

        public override async ValueTask<string> Solve_1()
        {
            var sides = _input.Count * 6;
            foreach (var pair in new FastCombinations<int[]>(_input, 2))
            {
                var dx = pair[0][0] - pair[1][0];
                var dy = pair[0][1] - pair[1][1];
                var dz = pair[0][2] - pair[1][2];
                if(dx*dx + dy*dy + dz*dz == 1)
                {
                    sides -= 2;
                }
            }

            return sides.ToString();
        }


        public override async ValueTask<string> Solve_2()
        {
            return "-";
        }
    }
}