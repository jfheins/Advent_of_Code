using System.Collections.Generic;
using System.IO;
using System.Linq;

using Core;

namespace AoC_2020.Days
{
    public class Day_02 : BaseDay
    {
        private readonly ICollection<(int, int, char, string)> _input;

        public Day_02()
        {
            _input = File.ReadAllLines(InputFilePath).MatchRegexGroups4<int, int, char, string>(@"(\d+)-(\d+) (\w): (\w+)").ToList();
        }

        public override string Solve_1()
        {
            static bool IsValid((int min, int max, char c, string pwd) input)
            {
                var count = input.pwd.Count(x => x == input.c);
                return count >= input.min && count <= input.max;
            }

            return _input.Count(IsValid).ToString();
        }

        public override string Solve_2()
        {
            static bool IsValid((int firstIdx, int secondIdx, char c, string pwd) t)
                => t.pwd[t.firstIdx - 1] == t.c ^ t.pwd[t.secondIdx - 1] == t.c;

            return _input.Count(IsValid).ToString();
        }
    }
}
