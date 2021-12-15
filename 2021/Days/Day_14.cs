using Core;

namespace AoC_2021.Days
{
    public class Day_14 : BaseDay
    {
        private string _template;
        private Dictionary<(char, char), char> _rules;

        public Day_14()
        {
            var input = File.ReadAllLines(InputFilePath).ToList();
            _template = input[0];
            _rules = input.Skip(2).Select(line => line.Split(" -> ")).ToDictionary(x => x[0].ToTuple2(), x => x[1][0]);
        }

        public override async ValueTask<string> Solve_1()
        {
            return Solve(10).ToString();
        }
        public override async ValueTask<string> Solve_2()
        {
            return Solve(40).ToString();
        }

        private long Solve(int steps)
        {
            var monomer = _template;
            var pairs = monomer.PairwiseWithOverlap().Histogram().ToDictionary(x => x.item, x => (long)x.count);
            for (int i = 0; i < steps; i++)
            {
                var newpairs = pairs.SelectMany(NewPairs).ToList();
                pairs.Clear();
                foreach (var (pair, count) in newpairs)
                {
                    pairs.AddOrModify(pair, 0, x => x + count);
                }
            }
            // If we count only the right side of each pair, we miss the first letter
            var stats = new Dictionary<char, long>
            {
                { monomer[0], 1 }
            };
            foreach (var p in pairs)
            {
                stats.AddOrModify(p.Key.Item2, 0, x => x + p.Value);
            }

            var (min, max) = stats.Values.MinMax()!.Value;
            return max - min;

            IEnumerable<((char, char), long)> NewPairs(KeyValuePair<(char, char), long> pairWithCount)
            {
                var left = pairWithCount.Key.Item1;
                var right = pairWithCount.Key.Item2;
                var addedMonomer = _rules[pairWithCount.Key];
                yield return ((left, addedMonomer), pairWithCount.Value);
                yield return ((addedMonomer, right), pairWithCount.Value);
            }
        }
    }
}
