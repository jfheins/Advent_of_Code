using Core;

namespace AoC_2021.Days
{
    public class Day_08 : BaseDay
    {
        private List<Row> _input;

        public Day_08()
        {
            _input = File.ReadAllLines(InputFilePath).Select(Row.FromLine).ToList();
        }

        public override async ValueTask<string> Solve_1()
        {
            var relevantLengths = new HashSet<int> { 2, 3, 4, 7 };
            return _input.Sum(CountEasyDigits).ToString();

            int CountEasyDigits(Row line)
                => line.OutputValues.Count(s => relevantLengths.Contains(s.Length));
        }

        public override async ValueTask<string> Solve_2()
        {
            return _input.Sum(SolveLine).ToString();
        }

        private int SolveLine(Row line)
        {
            /*   0000
             * 1      2
               1      2 
                 3333
               4      5
               4      5 
                 6666    */
            var possibleChars = Enumerable.Range(0, 7).Select(_ => new HashSet<char>("abcdefg")).ToArray();
            var mapLengthToKnownSegments = new Dictionary<int, IList<int>>
            {
                {2, new[]{ 2, 5 } },     // must be 1
                {3, new[]{ 0, 2, 5 } },  // 7
                {4, new[]{ 1, 2, 3, 5 } }, // 4
                {5, new[]{ 0, 3, 6 } }, // can be 2, 3 or 5
                {6, new[]{ 0, 1, 5, 6 } } // can be 0, 6 or 9
            };

            foreach (var pattern in line.Patterns)
            {
                if (mapLengthToKnownSegments.TryGetValue(pattern.Length, out var knownSegments))
                {
                    foreach (var segment in knownSegments)
                        possibleChars[segment].IntersectWith(pattern);
                }
            }

            var solved = GetSolved().ToList();

            for (int i = 0; i < 7; i++)
            {
                foreach (var (_, c) in solved)
                    Apply(set => set.Remove(c));
                
                solved.AddRange(GetSolved());
                if (solved.Count == 7)
                    break;
            }
            var dict = solved.ToDictionary(t => t.c, t => t.segment);

            var digits = line.OutputValues.Select(code => MapToDigit(code, dict));
            return int.Parse(string.Concat(digits.Select(d => d.ToString())));


            IEnumerable<(int segment, char c)> GetSolved()
            {
                for (int i = 0; i < possibleChars.Length; i++)
                    if (possibleChars[i].Count == 1)
                        yield return (i, possibleChars[i].First());
            }
            void Apply(Action<HashSet<char>> action)
            {
                foreach (var set in possibleChars)
                    action(set);
            }
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
        private record Row(string[] Patterns, string[] OutputValues)
        {
            public static Row FromLine(string line)
            {
                var parts = line.Split("|").SelectArray(p => p.Split(" ", StringSplitOptions.RemoveEmptyEntries));
                return new Row(parts[0], parts[1]);
            }
        };
    }
}
