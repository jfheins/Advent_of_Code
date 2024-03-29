﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;

namespace AoC_2020.Days
{
    public class Day_10 : BaseDay
    {
        private readonly int[] input;
        private List<int> diffs = new();

        public Day_10()
        {
            input = File.ReadAllLines(InputFilePath).Select(int.Parse).ToArray();
        }

        public override string Solve_1()
        {
            var device = input.Max() + 3;
            diffs = input
                .Append(device)
                .Append(0)
                .OrderBy(x => x)
                .Diff()
                .ToList();

            return (diffs.Count(x => x == 1) * diffs.Count(x => x == 3)).ToString();
        }

        public override string Solve_2()
        {
            return diffs
                .Runs()
                .Where(run => run.Element == 1)
                .Select(run => NumberOfCompositionsOfN(run.Count))
                .Select(x => (long)x)
                .Product()
                .ToString();
        }
        private int NumberOfCompositionsOfN(int n) => Tribonacci(n);

        private readonly List<int> TribonacciCache = new() { 1, 1, 2 };

        private int Tribonacci(int x)
        {
            if (TribonacciCache.Count <= x)
                TribonacciCache.Add(Tribonacci(x - 1) + Tribonacci(x - 2) + Tribonacci(x - 3));

            return TribonacciCache[x];
        }
    }
}

