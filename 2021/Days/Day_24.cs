using Core;
using Core.Combinatorics;
using MoreLinq.Extensions;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AoC_2021.Days
{
    public class Day_24 : BaseDay
    {
        private string[] _input;

        public Day_24()
        {
            _input = File.ReadAllLines(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            return Solve(99999999999999);
        }

        public override async ValueTask<string> Solve_2()
        {
            return Solve(11111111111111);
        }

        private string Solve(long start)
        {
            var digits = ToDigits(start);
            (int idx, int change)[] result;
            do
            {
                result = Calculate(digits);
                if (result.Length == 2)
                {
                    // As we start with either all 9 or all 1, only one suggestion will result in a valid digit
                    var (idx, change) = result.Single(it => IsValidDigit(digits[it.idx] + it.change));
                    digits[idx] += change;
                }
            } while (result.Length > 0);
            Debug.Assert(Interpret(digits) == 0);
            return ToNumber(digits);

            static bool IsValidDigit(int d) => d > 0 && d < 10;
        }

        private static int[] ToDigits(long l)
        {
            var digits = new int[14];
            for (int i = 13; i >= 0; i--)
            {
                digits[i] = (int)(l % 10);
                l /= 10;
            }
            return digits;
        }

        private static string ToNumber(int[] digits)
            => string.Concat(digits);

        /// <summary>
        /// Checks a model number. If it checks out, returns empty array.
        /// If model number is invalid, returns a suggestion on how to make it valid.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Emptyness for valid model numbers, two suggestions otherwise</returns>
        private (int idx, int change)[] Calculate(IList<int> input)
        {
            var data = _input.Chunk(18).Select(ParseBlock).ToList();
            var IdxStack = new Stack<int>();

            long x, w, z = 0;
            for (int i = 0; i < 14; i++)
            {
                w = input[i];
                x = z % 26 + data[i].xOff;
                z /= data[i].zDiv;
                if (x != w)
                {
                    z *= 26;
                    z += w + data[i].yOff;
                }

                if (data[i].zDiv == 1)
                {
                    IdxStack.Push(i); // This block can only increase. Save the index for later
                }
                else
                {
                    // This was meant to match.
                    var matchingIdx = IdxStack.Pop();
                    if (x != w)
                    {
                        // Did not happen
                        //Console.WriteLine($"Index {i} did not match, adjust {i} by {x - w} or {matchingIdx} by {w - x}");
                        return new[] { (i, (int)(x-w)), (matchingIdx, (int)(w-x)) };
                    }
                }
            }
            return Array.Empty<(int, int)>();
        }

        private (int xOff, int zDiv, int yOff) ParseBlock(string[] block)
        {
            var xOffset = block[5].Split(" ")[2];
            var zdiv = block[4].Split(" ")[2];
            var yOffset = block[15].Split(" ")[2];
            return (int.Parse(xOffset), int.Parse(zdiv), int.Parse(yOffset));
        }

        private long Interpret(IList<int> input)
        {
            var variables = new Dictionary<char, long>
            {
                ['x'] = 0,
                ['y'] = 0,
                ['z'] = 0,
                ['w'] = 0
            };
            var inputsTaken = 0;
            foreach (var line in _input)
            {                
                var args = line[3..].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var ret = args[0][0];
                switch (line[0..3])
                {
                    case "inp":
                        variables[ret] = input[inputsTaken++];
                        break;
                    case "add":
                        variables[ret] = Eval(args[0]) + Eval(args[1]);
                        break;
                    case "mul":
                        variables[ret] = Eval(args[0]) * Eval(args[1]);
                        break;
                    case "div":
                        variables[ret] = Eval(args[0]) / Eval(args[1]);
                        break;
                    case "mod":
                        variables[ret] = Eval(args[0]) % Eval(args[1]);
                        break;
                    case "eql":
                        variables[ret] = Eval(args[0]) == Eval(args[1]) ? 1 : 0;
                        break;
                    default:
                        break;
                }
            }
            return variables['z'];

            long Eval(string arg)
                => char.IsLetter(arg[0]) ? variables[arg[0]] : long.Parse(arg);
        }
    }
}
