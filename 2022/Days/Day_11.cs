using Core;

using System.Diagnostics;
using System.Runtime.CompilerServices;

using static MoreLinq.Extensions.PartialSortByExtension;

namespace AoC_2022.Days
{
    public sealed partial class Day_11 : BaseDay
    {
        private readonly List<(int m, int[] items)> _initialItems = new() {
            (0, new[] { 57 }),
            (1, new[] { 58, 93, 88, 81, 72, 73, 65 }),
            (2, new[] { 65, 95 }),
            (3, new[] { 58, 80, 81, 83 }),
            (4, new[] { 58, 89, 90, 96, 55 }),
            (5, new[] { 66, 73, 87, 58, 62, 67 }),
            (6, new[] { 85, 55, 89 }),
            (7, new[] { 73, 80, 54, 94, 90, 52, 69, 58 }),
        };

        public Day_11()
        {
        }

        private sealed class Part1Item : MonkeyItem
        {

            protected override long Simplify(long x) => x / 3;
        }

        private sealed class Part2Item : MonkeyItem
        {

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        private IEnumerable<T> MakeItems<T>() where T: MonkeyItem, new()
        {
            return _initialItems
               .SelectMany(t => t.items.Select(wl => new T()
               { CurrentMonkey = t.m, WorryLevel = wl })).ToList();
        }

        private long MonkeyBusinessLevel(IEnumerable<MonkeyItem> items)
        {
            var sums = items
                .Select(it => it.Inspections)
                .Aggregate((a, b) => a.Zip(b, (x, y) => x + y)
                .ToArray());
            return sums.PartialSortBy(2, it => -it).Product();
        }
    }
}