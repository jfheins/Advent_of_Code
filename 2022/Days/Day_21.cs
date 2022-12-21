using AoCHelper;

using Core;

using Microsoft.Z3;

using System.Data;
using System.Runtime.Intrinsics;

namespace AoC_2022.Days
{
    public sealed class Day_21 : BaseDay
    {
        private Dictionary<string, string> _input;

        public Day_21()
        {
            _input = File.ReadAllLines(InputFilePath).SelectList(ParseLine).ToDictionary(x => x.name, x => x.value);
        }

        private (string name, string value) ParseLine(string line)
        {
            return line.Split(":", StringSplitOptions.TrimEntries).ToTuple2();
        }

        public override async ValueTask<string> Solve_1()
        {
            Dictionary<string, long> cache = new();

            return Resolve("root").ToString();

            long Resolve(string name) => cache.GetOrAdd(name, n =>
                {
                    var value = _input[name];
                    if (long.TryParse(value, out var i))
                    {
                        return i;
                    }
                    var args = new string[2] { value[0..4], value[7..] };
                    return value[5] switch
                    {
                        '*' => Resolve(args[0]) * Resolve(args[1]),
                        '+' => Resolve(args[0]) + Resolve(args[1]),
                        '/' => Resolve(args[0]) / Resolve(args[1]),
                        '-' => Resolve(args[0]) - Resolve(args[1]),
                        _ => throw new Exception("Unknown op")
                    };
                });
        }

        public override async ValueTask<string> Solve_2()
        {
            var term = MakeTerm("root");
            return "Use CAS";

            string MakeTerm(string name)
            {
                if (name == "humn")
                    return "x";
                var value = _input[name];
                if (long.TryParse(value, out var i))
                {
                    return value;
                }
                var args = new string[2] { value[0..4], value[7..] };
                if (name == "root")
                    return $"{MakeTerm(args[0])} = {MakeTerm(args[1])}";

                return "(" + value[5] switch
                {
                    '*' => MakeTerm(args[0]) + "*" + MakeTerm(args[1]),
                    '+' => MakeTerm(args[0]) + "+" + MakeTerm(args[1]),
                    '/' => MakeTerm(args[0]) + "/" + MakeTerm(args[1]),
                    '-' => MakeTerm(args[0]) + "-" + MakeTerm(args[1]),
                    _ => throw new Exception("Unknown op")
                } + ")";
            }
        }
    }
}