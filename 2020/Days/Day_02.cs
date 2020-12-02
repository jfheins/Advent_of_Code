using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


using Core;
using Core.Combinatorics;

namespace AoC_2020.Days
{
    public class Day_02 : BaseDay
    {
        private readonly ICollection<string[]> _input;

        public Day_02()
        {
            _input = File.ReadAllLines(InputFilePath).MatchRegexGroups(@"(\d+)-(\d+) (\w): (\w+)").ToList();
        }

        public override string Solve_1()
        {
            var isvalid = 0;
            foreach (var line in _input)
            {
                var min = int.Parse(line[1]);
                var max = int.Parse(line[2]);
                var requiredChar = line[3];
                var password = line[4];

                var count = password.Count(x => x == requiredChar[0]);
                if (count >= min && count <= max)
                    isvalid++;
            }
            return isvalid.ToString();
        }

        public override string Solve_2()
        {

            var isvalid = 0;
            foreach (var line in _input)
            {
                var min = int.Parse(line[1]);
                var max = int.Parse(line[2]);
                var need = line[3][0];
                var pwd = line[4];

                if (pwd[min - 1] == need ^ pwd[max - 1] == need)
                    isvalid++;

            }
            return isvalid.ToString();
        }
    }
}
