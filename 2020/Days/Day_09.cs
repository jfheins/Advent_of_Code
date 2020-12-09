using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

using Core;
using Core.Combinatorics;

using MoreLinq.Extensions;

namespace AoC_2020.Days
{
    public class Day_09 : BaseDay
    {
        private readonly long[] input;
        private long xmasNumber = 0;
        private int xmasNumberIdx = 0;

        public Day_09()
        {
            input = File.ReadAllLines(InputFilePath).Select(long.Parse).ToArray();
        }

        public override string Solve_1()
        {
            var window = new HashSet<long>(input.Take(25));
            for (int i = 25; i < input.Length; i++)
            {
                var isSum = new FastCombinations<long>(window.ToList(), 2).Where(x => x.Sum() == input[i]).Any();
                if (!isSum)
                {
                    xmasNumber = input[i];
                    xmasNumberIdx = i;
                    return xmasNumber.ToString();
                }
                _ = window.Remove(input[i - 25]);
                _ = window.Add(input[i]);
            }
            return "?";
        }

        public override string Solve_2()
        {
            var haystack = input.Take(xmasNumberIdx).ToList();

            for (int lower = 0; lower < haystack.Count; lower++)
            {
                for (int len = 1; len < 100; len++)
                {
                    var sum = input.Skip(lower).Take(len).Sum();
                    if (sum == xmasNumber)
                    {
                        var (min, max) = input.Skip(lower).Take(len).MinMax()!.Value;
                        return (min + max).ToString();
                    }
                }
            }

            return "";
        }
    }
}
