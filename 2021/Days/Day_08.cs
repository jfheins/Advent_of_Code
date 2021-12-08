using Core;
using Core.Combinatorics;
using MoreLinq.Extensions;
using System.IO;
using System.Linq;

namespace AoC_2021.Days
{
    public class Day_08 : BaseDay
    {
        private List<(string[] patterns, string[] code)> _input;

        public Day_08()
        {
            _input = File.ReadAllLines(InputFilePath).Select(Parse).ToList();
        }

        private (string[] patterns, string[] code) Parse(string line)
        {
            var parts = line.Split("|");
            var p = parts[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var d = parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
            return (p, d);
        }

        public override async ValueTask<string> Solve_1()
        {
            // gits 1, 4, 7, or 8 appear?
            var ones = _input.Sum(line => line.code.Count(p => p.Length == 2));
            var fours = _input.Sum(line => line.code.Count(p => p.Length == 4));
            var sevens = _input.Sum(line => line.code.Count(p => p.Length == 3));
            var eigths = _input.Sum(line => line.code.Count(p => p.Length == 7));
            return (ones+fours+sevens+eigths).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            return _input.Sum(SolveLine).ToString();
        }

        private int SolveLine((string[] patterns, string[] code) line)
        {
            /*  000000
             * 1       2
               1       2 
                  3333
               4       5
               4       5 
                  6666  */
            var possibleChars = Enumerable.Range(0, 7).Select(_ => new HashSet<char>("abcdefg")).ToArray();

            foreach (var pattern in line.patterns)
            {
                switch (pattern.Length)
                {
                    case 2: // must be 1
                        possibleChars[2].IntersectWith(pattern);
                        possibleChars[5].IntersectWith(pattern);
                        break;
                    case 3: // 7
                        possibleChars[0].IntersectWith(pattern);
                        possibleChars[2].IntersectWith(pattern);
                        possibleChars[5].IntersectWith(pattern);
                        break;
                    case 4: // 4
                        possibleChars[1].IntersectWith(pattern);
                        possibleChars[3].IntersectWith(pattern);
                        possibleChars[2].IntersectWith(pattern);
                        possibleChars[5].IntersectWith(pattern);
                        break;
                    case 5: // can be 2 or 3 or 5
                        possibleChars[0].IntersectWith(pattern);
                        possibleChars[3].IntersectWith(pattern);
                        possibleChars[6].IntersectWith(pattern);
                        break;
                    case 6: // can be 0 6 or 9
                        possibleChars[0].IntersectWith(pattern);
                        possibleChars[1].IntersectWith(pattern);
                        possibleChars[5].IntersectWith(pattern);
                        possibleChars[6].IntersectWith(pattern);
                        break;
                    default:
                        break;
                }
            }
            while (possibleChars.Any(x => x.Count > 1))
            {
                var clear = possibleChars.Select((s, i) => (s, i))
                    .Where(x => x.s.Count == 1).Select(x => (x.s.First(), x.i)).ToList();
                foreach (var item in clear)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        if (i != item.i)
                        {
                            possibleChars[i].Remove(item.Item1);
                        }
                    }
                }
            }
            var dict = possibleChars.Select((s, i) => (s.First(), i))
                .ToDictionary(t => t.Item1, t => t.i);

            var digits = line.code.Select(code => MapToDigit(code, dict));
            return int.Parse(string.Concat(digits.Select(d => d.ToString())));
        }

        private int MapToDigit(string code, Dictionary<char, int> dict)
        {
            /*  000000
             * 1       2
               1       2 
                  3333
               4       5
               4       5 
                  6666  */
            var segments = string.Concat(code.Select(c => dict[c])
                .OrderBy(c => c).Select(c => c.ToString()));
            return segments switch
            {
                "012456" => 0,
                "25" => 1,
                "02346" => 2,
                "02356" => 3,
                "1235" => 4,
                "01356" => 5,
                "013456" => 6,
                "025" => 7,
                "0123456" => 8,
                "012356" => 9,
                _ => throw new NotImplementedException()
            };
        }
    }
}
