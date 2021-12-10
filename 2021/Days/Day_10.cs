using Core;

using LanguageExt;
using static LanguageExt.Prelude;

namespace AoC_2021.Days
{
    public class Day_10 : BaseDay
    {
        private readonly IReadOnlyCollection<string> _input;

        public Day_10()
        {
            _input = File.ReadAllLines(InputFilePath).ToList();
        }

        // Return illegal char or missing chunk
        private Either<char, string> ParseLine(string line)
        {
            var expectedChars = new Stack<char>();
            foreach (var chr in line)
            {
                if ("([{<".Contains(chr))
                    expectedChars.Push(GetClosing(chr));
                else
                {
                    if (chr != expectedChars.Pop())
                        return Left(chr);
                }
            }
            return Right(string.Concat(expectedChars));

            static char GetClosing(char v) => v switch
            {
                '(' => ')',
                '[' => ']',
                '{' => '}',
                '<' => '>',
                _ => throw new NotImplementedException()
            };
        }

        public override async ValueTask<string> Solve_1()
        {
            return _input.Select(ParseLine)
                .SelectMany(it => it.LeftAsEnumerable())
                .Sum(GetScore)
                .ToString();

            static int GetScore(char c) => c switch
            {
                ')' => 3,
                ']' => 57,
                '}' => 1197,
                '>' => 25137,
                _ => throw new NotImplementedException(),
            };
        }

        public override async ValueTask<string> Solve_2()
        {
            var scores = _input.Select(ParseLine)
                .SelectMany(it => it.RightAsEnumerable())
                .Select(GetScore)
                .ToList();

            scores.Sort();
            return scores.CenterItem().ToString();

            static long GetScore(string s)
            {
                const string chars = "=)]}>";
                return s.Aggregate(0L, (sum, c) => sum * 5 + chars.IndexOf(c));
            }
        }
    }
}
