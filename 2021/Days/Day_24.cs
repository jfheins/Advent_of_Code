using Core;
using Core.Combinatorics;
using MoreLinq.Extensions;
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
            var res = Interpret(ToDigits(36969794979199));
            Console.WriteLine("=> " + res['z']);
            return "36969794979199";
        }

        public override async ValueTask<string> Solve_2()
        {
            var res = Interpret(ToDigits(11419161313147));
            Console.WriteLine("=> " + res['z']);
            return "11419161313147";
        }

        private IList<int> ToDigits(long l)
        {
            var digits = new int[14];
            for (int i = 13; i >= 0; i--)
            {
                digits[i] = (int)(l % 10);
                l /= 10;
            }
            return digits;
        }

        private Dictionary<char, long> Interpret(IList<int> input)
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
            return variables;

            long Eval(string arg)
            {
                return char.IsLetter(arg[0])
                    ? variables[arg[0]] : long.Parse(arg);
            }
        }
    }
}
