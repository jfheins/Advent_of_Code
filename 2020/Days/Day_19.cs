using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Core;

using static MoreLinq.Extensions.SplitExtension;

namespace AoC_2020.Days
{
    public class Day_19 : BaseDay
    {
        private readonly Dictionary<int, string> cache = new();
        private readonly string[] rules;
        private readonly string[] messages;
        private Dictionary<int, string> ruleDict;
        private long time;

        public Day_19()
        {
            var input = File.ReadAllLines(InputFilePath).Split("").ToArray();
            rules = input[0].Select(l => l.Replace("\"", "")).ToArray();
            messages = input[1].ToArray();
        }

        public override string Solve_1()
        {
            static KeyValuePair<int, string> ParseRule(string rule)
            {
                var parts = rule.Split(": ");
                return KeyValuePair.Create(int.Parse(parts[0]), parts[1]);
            }
            ruleDict = new Dictionary<int, string>(rules.Select(ParseRule));
            var regex = new Regex("^" + Resolve(0) + "$", RegexOptions.ExplicitCapture);
            return messages.Count(line => regex.IsMatch(line)).ToString();
        }

        public override string Solve_2()
        {
            // 0: 8 11
            // 8: 42 | 42 8
            // 11: 42 31 | 42 11 31
            // => 42*m 42*n 31*n where m, n > 0

            var p42 = Resolve(42);
            var p31 = Resolve(31);
            var overall = new Regex(@$"^{p42}+(?'n'{p42})+(?'-n'{p31})+(?(n)(?!))$", RegexOptions.ExplicitCapture);
            return messages.Count(line => overall.IsMatch(line)).ToString();
        }

        private string Resolve(int index)
        {
            var value = ruleDict[index];
            var addparens = value.Contains("|");
            if (!value.Any(c => char.IsDigit(c)))
            {
                return value;
            }
            var links = value.ParseInts();
            var repl = links.Distinct()
                .ToDictionary(link => link, link => cache.GetOrAdd(link, i => Resolve(i)));
            value = Regex.Replace(value, @"\d+", m => repl[int.Parse(m.Value)]);

            if (addparens)
            {
                value = "(" + value + ")";
            }
            return value.Replace(" ", "");
        }
    }
}

