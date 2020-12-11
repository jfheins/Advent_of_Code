using System.Collections.Generic;
using System.IO;
using System.Linq;

using MoreLinq.Extensions;

namespace AoC_2020.Days
{
    public class Day_06 : BaseDay
    {
        private readonly List<IEnumerable<string>> _input;

        public Day_06()
        {
            _input = File.ReadAllLines(InputFilePath).Split("").ToList();
        }

        public override string Solve_1()
        {
            return _input.Select(block => string.Concat(block).Distinct().Count()).Sum().ToString();
        }

        public override string Solve_2()
        {
            return _input.Select(block => block.Aggregate((set, elem) => string.Concat(set.Intersect(elem))).Length).Sum().ToString();
        }
    }
}
