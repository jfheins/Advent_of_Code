using Core;
using static MoreLinq.Extensions.PartialSortByExtension;

namespace AoC_2022.Days
{
    public sealed class Day_11 : BaseDay
    {
        private readonly List<(int monkey, int worryLevel)> _initialItems = new();

        public Day_11()
        {
            var input = File.ReadAllLines(InputFilePath);
            for (int i = 0; i < input.Length - 1; i++)
            {
                if (!input[i].StartsWith("Monkey")) continue;
                var monkeyIdx = input[i].ParseInts(1).First();
                var items = input[i + 1].ParseInts();
                foreach (var item in items)
                {
                    _initialItems.Add((monkeyIdx, item));
                }
            }
        }

        private sealed class Part1Item : MonkeyItem
        {
            protected override long Simplify(long x) => x / 3;
        }

        private sealed class Part2Item : MonkeyItem
        {
            protected override long Simplify(long x) => x % 9699690;
        }

        public override async ValueTask<string> Solve_1()
        {
            var items = MakeItems<Part1Item>();

            foreach (var item in items)
                item.PlayRounds(20);

            return MonkeyBusinessLevel(items).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var items = MakeItems<Part2Item>();
            _ = Parallel.ForEach(items, item => item.PlayRounds(10_000));
            return MonkeyBusinessLevel(items).ToString();
        }

        private List<T> MakeItems<T>() where T : MonkeyItem, new()
        {
            return _initialItems
                .Select(t => new T() { CurrentMonkey = t.monkey, WorryLevel = t.worryLevel })
                .ToList();
        }

        private long MonkeyBusinessLevel(IEnumerable<MonkeyItem> items)
        {
            var sums = items
                .Select(it => it.Inspections)
                .Aggregate((a, b) => a.Zip(b, (x, y) => x + y).ToArray());
            return sums.PartialSortBy(2, it => -it).Product();
        }
    }
}