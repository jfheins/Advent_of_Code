using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

using Core;

using static MoreLinq.Extensions.SplitExtension;

namespace AoC_2020.Days
{
    public class Day_19 : BaseDay
    {
        private readonly string[] input;
        private readonly int[] numbers;
        private Dictionary<int, string> cache = new();

        //private FiniteGrid2D<char> grid;

        public Day_19()
        {
            input = File.ReadAllLines(InputFilePath);
            //numbers = input.Select(int.Parse).ToArray();
            //grid = Grid2D.FromFile(InputFilePath);
        }

        public override string Solve_1()
        {
            var blocks = input.Split("").Select(x => x.ToArray()).ToArray();
            var rules = blocks[0].ToDictionary(s => int.Parse(s.Split(":")[0]), s => s.Split(":")[1].Replace("\"", ""));

            var regex = Resolve(rules, 0);
            var x = new Regex("^" + regex.Replace(" ", "") + "$", RegexOptions.Compiled);

            var count = 0;
            for (int i = 0; i < blocks[1].Length; i++)
            {
                var line = blocks[1][i];
                if (x.IsMatch(line))
                {
                    count++;
                    Console.WriteLine($"{i} / {blocks[1].Length}");
                }
            }

            return blocks[1].Count(line => x.IsMatch(line)).ToString();
        }

        private string Resolve(Dictionary<int, string> rules, int index)
        {
            var value = rules[index];
            var orig = value;
            var addparens = value.Contains("|");
            if (!value.Any(c => char.IsDigit(c)))
            {
                return value;
            }
            var links = value.ParseInts();
            var repl = links.Distinct().ToDictionary(link => link, link => cache.GetOrAdd(link, i => Resolve(rules, i)));
            value = Regex.Replace(value, @"\d+", m => repl[int.Parse(m.Value)]);

            if (addparens)
            {
                value = "(" + value + ")";
            }
            return value.Replace(" ", "");
        }

        public override string Solve_2()
        {
            // 0: 8 11
            // 8: 42 | 42 8
            // 11: 42 31 | 42 11 31

            var blocks = input.Split("").Select(x => x.ToArray()).ToArray();
            var rules = blocks[0].ToDictionary(s => int.Parse(s.Split(":")[0]), s => s.Split(":")[1].Replace("\"", ""));

            var overall = new Regex("^(" + Resolve(rules, 42) + ")+(" + Resolve(rules, 31) + ")+$", RegexOptions.Compiled);
            var regex42 = new Regex(Resolve(rules, 42), RegexOptions.Compiled);
            var regex42start = new Regex("^" + Resolve(rules, 42), RegexOptions.Compiled);
            var regex31 = new Regex(Resolve(rules, 31), RegexOptions.Compiled);
            var regex31start = new Regex("^" + Resolve(rules, 31), RegexOptions.Compiled);

            var count = 0;
            for (int i = 0; i < blocks[1].Length; i++)
            {
                var line = blocks[1][i];
                var start42 = 0;
                while (regex42start.IsMatch(line))
                {
                    start42++;
                    line = regex42start.Replace(line, "");
                }

                var end31 = 0;
                while (regex31start.IsMatch(line))
                {
                    end31++;
                    line = regex31start.Replace(line, "");
                }
                if (line.Length > 0)
                    continue;
                
                if (end31 > 0 && end31 < start42)
                {
                    count++;
                    Console.WriteLine($"{i} / {blocks[1].Length}");
                }
            }

            return count.ToString();
        }
    }
}

