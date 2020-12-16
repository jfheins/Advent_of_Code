using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Core;

using static MoreLinq.Extensions.SkipUntilExtension;

namespace AoC_2020.Days
{
    public class Day_16 : BaseDay
    {
        private readonly string[] input;
        //private readonly int[] numbers;
        //private FiniteGrid2D<char> grid;

        public Day_16()
        {
            input = File.ReadAllLines(InputFilePath);
            //numbers = input.Select(int.Parse).ToArray();
            //grid = Grid2D.FromFile(InputFilePath);
        }

        public override string Solve_1()
        {
            var rules = input.TakeWhile(line => line.Length > 0);
            var nearby = input.SkipUntil(line => line.Contains("nearby tickets:"));

            var allowedRange = new HashSet<(int min, int max)>();
            foreach (var line in rules)
            {
                var num = line.ParseNNInts().Pairwise();
                foreach (var range in num)
                {
                    _ = allowedRange.Add(range);
                }
            }

            long errorRate = 0;
            foreach (var line in nearby)
            {
                var vals = line.ParseInts();
                var invalid = vals.Where(v => !allowedRange
                    .Any(r => r.min <= v && v <= r.max));
                errorRate += invalid.Sum();
            }

            return errorRate.ToString();
        }

        private bool IsValid((int min, int max) rule, int value)
            => rule.min <= value && value <= rule.max;
        private bool IsValidForAll((int min, int max) rule, IEnumerable<int> values)
            => values.All(v => IsValid(rule, v));
        private bool IsValidForAny(IEnumerable<(int min, int max)> rules, int val)
            => rules.Any(r => IsValid(r, val));
        private bool IsValidForAll(IEnumerable<(int min, int max)> rule, IEnumerable<int> values)
            => values.All(v => IsValidForAny(rule, v));

        public override string Solve_2()
        {
            var rules = input.TakeWhile(line => line.Length > 0);
            var nearby = input.SkipUntil(line => line.Contains("nearby tickets:"));

            var allowedRange = new HashSet<(int min, int max)>();
            var fieldrules = new Dictionary<string, List<(int min, int max)>>();
            foreach (var line in rules)
            {
                var num = line.ParseNNInts().Pairwise();
                var name = line.Substring(0, line.IndexOf(':'));
                fieldrules[name] = new List<(int min, int max)>();
                foreach (var pair in num)
                {
                    _ = allowedRange.Add(pair);
                    fieldrules[name].Add(pair);
                }
            }

            var validTickets = new List<int[]>();
            foreach (var line in nearby)
            {
                var vals = line.ParseInts();
                var valid = vals.All(v => allowedRange
                    .Any(r => r.min <= v && v <= r.max));
                if (valid)
                {
                    validTickets.Add(line.ParseInts());
                }
            }

            var text = "";
            for (int i = 0; i < 20; i++)
            {
                var values = validTickets.Select(t => t[i]).ToList();
                var field = fieldrules.Select(r =>  IsValidForAll(r.Value, values) ? r.Key.Replace(" ", "-") : "_").ToList();
                var names = string.Join("\t", field);
                text += names + "\r\n";
                Console.WriteLine(names);
            }
            File.WriteAllText("./tickets.csv", text);

            return "_";
        }
    }
}

