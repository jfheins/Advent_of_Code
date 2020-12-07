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
        private readonly string[] _input;

        public Day_07()
        {
            _input = File.ReadAllLines(InputFilePath);
        }

        public override string Solve_1()
        {
            var bags = _input.Select(ParseBag).ToList();

            var possibleContainers = bags.Where(b => b.Item2.Contains("shiny gold")).Select(x => x.Item1).ToHashSet();

            for (int i = 0; i < 1000; i++)
            {
                var add = bags.Where(b => b.Item2.Any(bag => possibleContainers.Contains(bag))).Select(x => x.Item1);
                possibleContainers.UnionWith(add);
            }
            return "";
        }

        private (string, string[]) ParseBag(string b)
        {
            var container = new Regex(@"(\w+ \w+) bags contain");
            var content = new Regex(@"\d (\w+ \w+) bag");

            var c = container.Match(b).Groups[1].Value;
            var cn = content.Matches(b).Select(match => match.Groups[1].Value).ToArray();

            return (c, cn);
        }

        private (string, (int count, string color)[]) ParseBag2(string b)
        {
            var container = new Regex(@"(\w+ \w+) bags contain");
            var content = new Regex(@"(\d) (\w+ \w+) bag");

            var c = container.Match(b).Groups[1].Value;
            var cn = content.Matches(b).Select(match => (int.Parse(match.Groups[1].Value), match.Groups[2].Value)).ToArray();

            return (c, cn);
        }

        public override string Solve_2()
        {
            var bags = _input.Select(ParseBag2).ToDictionary(t => t.Item1);
            var content = new Dictionary<string, int>();
            var newcontent = new Dictionary<string, int>();

            foreach (var (count, color) in bags["shiny gold"].Item2)
            {
                newcontent.Add(color, count);
            }


            for (int i = 0; i < 1000; i++)
            {
                foreach (var kvp in newcontent)
                    content.AddOrModify(kvp.Key, 0, c => c + kvp.Value);

                var toadd = newcontent.SelectMany(bag => bags[bag.Key].Item2.Select(child => new { parentcount = bag.Value, child })).ToList();
                newcontent = new Dictionary<string, int>();
                foreach (var newBag in toadd)
                {
                    newcontent.AddOrModify(newBag.child.color, 0, c => c + newBag.parentcount * newBag.child.count);
                }
            }

            return content.Sum(x => x.Value).ToString();
        }
    }
}
