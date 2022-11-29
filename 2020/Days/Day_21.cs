using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC_2020.Days
{
    public class Day_21 : BaseDay
    {
        private readonly string[] input;
        private readonly int[] numbers;
        //private FiniteGrid2D<char> grid;

        public Day_21()
        {
            input = File.ReadAllLines(InputFilePath);
            //numbers = input.Select(int.Parse).ToArray();
            //grid = Grid2D.FromFile(InputFilePath);
        }

        private (string[] ing, string[] allerg) ParseLine (string line)
        {
            var halfs = line.Split("(contains ");
            var i = halfs[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var a = halfs[1][..^1].Split(", ");
            return (i, a);
        }

        public override string Solve_1()
        {
            var all = input.Select(ParseLine).ToList();
            var allin = new HashSet<string>(all.SelectMany(x => x.ing));
            var allal = new HashSet<string>(all.SelectMany(x => x.allerg));

            var count = 0;
            foreach (var mying in allin)
            {
                var possible = new HashSet<string>(allal);
                foreach (var (ing, allerg) in all)
                {
                    if (!ing.Contains(mying))
                        possible.ExceptWith(allerg);
                }
                if (possible.Count == 0)
                {
                    count += all.Count(x => x.ing.Contains(mying));
                }
            }

            return count.ToString();
        }

        public override string Solve_2()
        {
            var all = input.Select(ParseLine).ToList();
            var allin = new HashSet<string>(all.SelectMany(x => x.ing));
            var allal = new HashSet<string>(all.SelectMany(x => x.allerg));

            var possible = new Dictionary<string, HashSet<string>>();
            foreach (var mying in allin)
            {
                possible[mying] = new HashSet<string>(allal);
                foreach (var (ing, allerg) in all)
                {
                    if (!ing.Contains(mying))
                        possible[mying].ExceptWith(allerg);
                }
            }

            var known = new List<(string i, string a)>();
            while(possible.Any(x => x.Value.Count > 0))
            {
                var ele = possible.Where(x => x.Value.Count == 1).First();
                known.Add((ele.Key, ele.Value.First()));
                _=possible.Remove(ele.Key);
                foreach (var other in possible)
                {
                    other.Value.ExceptWith(ele.Value);
                }
            }

            return string.Join(",", known.OrderBy(x => x.a).Select(x => x.i));
        }
    }
}

