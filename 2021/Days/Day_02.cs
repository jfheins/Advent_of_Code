using System.IO;
using System.Linq;

using Core;
using Core.Combinatorics;

using MoreLinq.Extensions;

namespace AoC_2021.Days
{
    public class Day_02 : BaseDay
    {
        private readonly string[] _input;

        public Day_02()
        {
            _input = File.ReadAllLines(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            var pos = 0;
            var depth = 0;

            foreach (var line in _input)
            {
                var delta = line.ParseInts(1)[0];
                if (line.StartsWith("forward"))
                {
                    pos += delta;
                }
                else if (line.StartsWith("down"))
                {
                    depth += delta;
                }
                else if (line.StartsWith("up"))
                {
                    depth -= delta;
                }
            }
            return (depth*pos).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var pos = 0;
            var depth = 0;
            var aim = 0;

            foreach (var line in _input)
            {
                var delta = line.ParseInts(1)[0];
                if (line.StartsWith("forward"))
                {
                    pos += delta;
                    depth += aim * delta;
                }
                else if (line.StartsWith("down"))
                {
                    aim += delta;
                }
                else if (line.StartsWith("up"))
                {
                    aim -= delta;
                }
            }
            return (depth * pos).ToString();
        }
    }
}
