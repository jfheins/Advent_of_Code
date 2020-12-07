using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Core;

namespace AoC_2020.Days
{
    public class Day_07 : BaseDay
    {
        private readonly Dictionary<string, (string color, int count)[]> contentsOf = new();
        private readonly Dictionary<string, List<string>> containersOf = new();

        public Day_07()
        {
            var input = File.ReadAllLines(InputFilePath);

            var containerRegex = new Regex(@"(\w+ \w+) bags contain");
            var contentRegex = new Regex(@"(\d) (\w+ \w+) bag");

            foreach (var line in input)
            {
                var container = containerRegex.Match(line).Groups[1].Value;
                var content = contentRegex.Matches(line).Select(match => (color: match.Groups[2].Value, count: int.Parse(match.Groups[1].Value))).ToArray();

                contentsOf[container] = content;
                foreach (var (color, count) in content)
                    if (containersOf.TryGetValue(color, out var val))
                        val.Add(container);
                    else
                        containersOf[color] = new List<string> { container };
            }
        }

        public override string Solve_1()
        {
            var emptyList = new List<string>();
            var search = new BreadthFirstSearch<string>(StringComparer.Ordinal, bag => containersOf.GetValueOrDefault(bag, emptyList))
            {
                PerformParallelSearch = false
            };
            return search.FindAll("shiny gold", bag => bag != "shiny gold").Count.ToString();
        }

        public override string Solve_2()
        {
            var content = new Dictionary<string, int>();
            var newcontent = new Dictionary<string, int>();

            foreach (var (color, count) in contentsOf["shiny gold"])
                newcontent.Add(color, count);

            while (newcontent.Any())
            {
                foreach (var kvp in newcontent)
                    content.AddOrModify(kvp.Key, 0, c => c + kvp.Value);

                var nextItems = newcontent
                    .SelectMany(bag => contentsOf[bag.Key].Select(child => (child.color, count: bag.Value * child.count)))
                    .ToList();
                newcontent.Clear();
                foreach (var (color, count) in nextItems)
                    newcontent.AddOrModify(color, 0, c => c + count);
            }

            return content.Values.Sum().ToString();
        }
    }
}
