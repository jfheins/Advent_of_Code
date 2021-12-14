using Core;
using Core.Combinatorics;
using MoreLinq.Extensions;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AoC_2021.Days
{
    public class Day_14 : BaseDay
    {
        private string _template;
        private List<(string pattern, string elem)> _rules;

        public Day_14()
        {
            var input = File.ReadAllLines(InputFilePath).ToList();
            _template = input[0];
            _rules = input.Skip(2).Select(line => line.Split(" -> ").ToTuple2()).ToList();
        }

        public override async ValueTask<string> Solve_1()
        {
            var polymer = _template;
            var stats = polymer.ToLookup(x => x).MinMax(x => x.Count())!.Value;
            for (int i = 0; i < 10; i++)
            {
                polymer = polymer[0] + string.Concat(polymer.PairwiseWithOverlap().Select(NewChain));
                stats = polymer.ToLookup(x => x).MinMax(x => x.Count())!.Value;
            }

            return (stats.max - stats.min).ToString();

            string NewChain((char a, char b) pair)
            {
                var rule = _rules.FirstOrDefault(r => r.pattern[0] == pair.a && r.pattern[1] == pair.b);
                return string.Concat(rule.elem, pair.b);
            }
        }

        public override async ValueTask<string> Solve_2()
        {
            var polymer = _template;
            var ppp = polymer.PairwiseWithOverlap().Select(p => string.Concat(p.Item1.ToString(), p.Item2.ToString())).ToList();
            var pairs = ppp.ToLookup(x => x).ToDictionary(x => x.Key, x => x.LongCount());

            for (int i = 0; i < 40; i++)
            {
                var newpairs = pairs.Select(kvp => (chain: NewChain(kvp.Key), kvp.Value)).ToList();
                pairs.Clear();
                foreach (var (chain, Value) in newpairs)
                {
                    pairs.AddOrModify(chain[0..2], 0, x => x + Value);
                    pairs.AddOrModify(chain[1..3], 0, x => x + Value);
                }
            }
            // Every letter is counted twice because it will be in two pairs.
            // Except for the first and last letter, we need to bump them so the
            // division later produces the right count
            var stats = new Dictionary<char, long>();
            stats.AddOrModify(polymer[0], 0, x => x + 1);
            stats.AddOrModify(polymer[^1], 0, x => x + 1);

            foreach (var p in pairs)
            {
                stats.AddOrModify(p.Key[0], 0, x => x + p.Value);
                stats.AddOrModify(p.Key[1], 0, x => x + p.Value);
            }
            Debug.Assert(stats.Values.All(v => v % 2 == 0));
            // Divide by 2 as each letter was counted twice
            var (min, max) = stats.Values.MinMax(x => x/2)!.Value;
            return (max - min).ToString();

            string NewChain(string pair)
            {
                var (_, elem) = _rules.FirstOrDefault(r => r.pattern == pair);
                return string.Concat(pair[0], elem, pair[1]);
            }
        }
    }
}
