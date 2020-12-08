using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

using Core;

using MoreLinq.Extensions;

namespace AoC_2020.Days
{
    public class Day_08 : BaseDay
    {
        private readonly string[] input;

        public Day_08()
        {
            input = File.ReadAllLines(InputFilePath);
        }

        private static (TerminaionReason reason, int acc) RunCPU(string[] program)
        {
            var seenIdx = new HashSet<int>();
            var current = 0;
            var acc = 0;

            while (seenIdx.Add(current) && current < program.Length)
            {
                var op = program[current];

                if (op.StartsWith("acc"))
                {
                    acc += op.ParseInts(1)[0];
                }
                else if (op.StartsWith("jmp"))
                {
                    current += op.ParseInts(1)[0];
                    continue;
                }
                current++;
            }
            var reason = current >= program.Length ? TerminaionReason.ReadOverflow : TerminaionReason.LoopDetected;
            return (reason, acc);
        }

        public override string Solve_1()
        {
            var (_, acc) = RunCPU(input);
            return acc.ToString();
        }

        public override string Solve_2()
        {
            for (int i = 0; i < input.Length; i++)
            {
                var program = input.ToArray();
                var op1 = input[i];
                if (op1.StartsWith("acc"))
                {
                    continue;
                }
                if (op1.StartsWith("jmp"))
                {
                    program[i] = program[i].Replace("jmp", "nop");
                }
                if (op1.StartsWith("nop"))
                {
                    program[i] = program[i].Replace("nop", "jmp");
                }

                var (reason, acc) = RunCPU(program);
                if (reason == TerminaionReason.ReadOverflow)
                    return acc.ToString();
            }

            return "(?)";
        }

        private enum TerminaionReason { LoopDetected, ReadOverflow }
    }
}
