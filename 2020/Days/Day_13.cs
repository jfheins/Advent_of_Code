﻿using System.IO;
using System.Linq;

using Core;

namespace AoC_2020.Days
{
    public class Day_13 : BaseDay
    {
        private readonly string[] input;
        private readonly int[] ids;

        public Day_13()
        {
            input = File.ReadAllLines(InputFilePath);
            ids = input[1].ParseInts();
        }

        public override string Solve_1()
        {
            var time = int.Parse(input[0]);

            for (int i = time; i < int.MaxValue; i++)
            {
                if (ids.Any(id => i % id == 0))
                {
                    return (ids.First(id => i % id == 0) * (i - time)).ToString();
                }
            }

            return "_";
        }

        public override string Solve_2()
        {
            var busses = input[1].Split(",")
                .Select((busid, idx) => (idx, busid))
                .Where(t => t.busid != "x")
                .Select(t => (t.idx, busid: long.Parse(t.busid)))
                .ToList();

            var ids = busses.Select(x => x.busid).ToArray();
            var remainders = busses.Select(x => (int)x.busid - x.idx).ToArray();

            var bestTime = ChineseRemainderTheorem.Solve(ids, remainders);

            return bestTime.ToString();
        }
    }

    public static class ChineseRemainderTheorem
    {
        public static long Solve(long[] n, int[] a)
        {
            long prod = n.Aggregate(1L, (i, j) => i * j);
            long p;
            long sm = 0;
            for (int i = 0; i < n.Length; i++)
            {
                p = prod / n[i];
                sm += a[i] * ModularMultiplicativeInverse(p, n[i]) * p;
            }
            return sm % prod;
        }

        private static long ModularMultiplicativeInverse(long a, long mod)
        {
            long b = a % mod;
            for (int x = 1; x < mod; x++)
            {
                if ((b * x) % mod == 1)
                {
                    return x;
                }
            }
            return 1L;
        }
    }
}
