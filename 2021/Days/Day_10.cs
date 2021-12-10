using Core;
using Core.Combinatorics;
using MoreLinq.Extensions;
using System.IO;
using System.Linq;

namespace AoC_2021.Days
{
    public class Day_10 : BaseDay
    {
        private string[] _input;
        private int[] _numbers;

        public Day_10()
        {
            _input = File.ReadAllLines(InputFilePath).ToArray();

            //Grid2D.FromFile(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            int score = 0;
            foreach (var line in _input)
            {
                var stack = new Stack<char>();
                char? illegalChar = null;
                foreach (var chr in line)
                {
                    if ("([{<".Contains(chr))
                        stack.Push(chr);
                    else
                    {
                        var expected = GetMatching(stack.Pop());
                        if (chr != expected)
                        {
                            illegalChar = chr;
                            break;
                        }
                    }
                }
                /*
                 ): 3 points.
                ]: 57 points.
                }: 1197 points.
                >: 25137 points.*/

                score += illegalChar switch {
                ')' => 3,
                ']' => 57,
                '}' => 1197,
                '>' => 25137,
                null => 0
                };
            }
            return score.ToString();
        }

        private char GetMatching(char v)
        {
            return v switch
            {
                '(' => ')',
                '[' => ']',
                '{' => '}',
                '<' => '>',
                _ => throw new NotImplementedException()
            };
        }

        public override async ValueTask<string> Solve_2()
        {
            var allscore = new List<long>();
            foreach (var line in _input)
            {
                var stack = new Stack<char>();
                bool isLegal = true;
                foreach (var chr in line)
                {
                    if ("([{<".Contains(chr))
                        stack.Push(chr);
                    else
                    {
                        var expected = GetMatching(stack.Pop());
                        if (chr != expected)
                        {
                            isLegal = false;
                            break;
                        }
                    }
                }
                if (isLegal)
                {
                    // likely incomplete
                    var suffix = string.Concat(stack.Select(GetMatching));
                    long score = 0L;
                    for (int i = 0; i < suffix.Length; i++)
                    {
                        score *= 5;
                        score += suffix[i] switch
                        {
                            ')' => 1,
                            ']' => 2,
                            '}' => 3,
                            '>' => 4,
                            _ => throw new NotImplementedException(),
                        };
                    }
                    allscore.Add(score);
                }
            }
            allscore.Sort();
            return allscore.Median().ToString();
        }
    }
}
