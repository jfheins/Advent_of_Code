using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

using Core;

namespace AoC_2020.Days
{
    public class Day_10 : BaseDay
    {
        private readonly long[] inputNum;
        private readonly string[] input;

        public Day_10()
        {
            inputNum = File.ReadAllLines(InputFilePath).Select(long.Parse).ToArray();
            input = File.ReadAllLines(InputFilePath);
        }

        public override string Solve_1()
        {
            var device = inputNum.Max() + 3;
            var c = inputNum
                .Append(device)
                .Append(0)
                .OrderBy(x => x)
                .PairwiseWithOverlap()
                .Select(x => x.Diff()).ToList();

            return (c.Count(x => x == 1) * c.Count(x => x == 3)).ToString();
        }

        public override string Solve_2()
        {
            var device = inputNum.Max() + 3;
            var sorted = inputNum
                .Append(device)
                .Append(0)
                .OrderBy(x => x)
                .PairwiseWithOverlap()
                .Select(x => x.Diff()).ToList();

            var runs = sorted.Chunks().Where(c => c[0] == 1).Select(c => c.Count)
                .Select(len =>
            len switch
            {
                1 => 1L,
                2 => 2L,
                3 => 4L,
                4 => 7L,
                _ => 0L
            }).Product();

            return runs.ToString();
        }
    }
}

