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
        private readonly int[] input;
        private List<int> diffs;

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
                .Where(run => run.first == 1)
                .Select(run =>
                    run.count switch
                    {
                        1 => 1L,
                        2 => 2L,
                        3 => 4L,
                        4 => 7L,
                        _ => 0L
                    })
                .Product()
                .ToString();
        }
    }
}

