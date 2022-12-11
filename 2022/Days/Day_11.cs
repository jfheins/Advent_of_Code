using Core;

using System.Diagnostics;

using static MoreLinq.Extensions.PartialSortByExtension;

namespace AoC_2022.Days
{
    public sealed class Day_11 : BaseDay
    {
        private readonly List<Monkey> _initialMonkeySetup = new()
        {
            new Monkey()
            {
                Items = { 57 },
                Operation = it => it * 13,
                Test = 11,
                Destinations = (3, 2),
            },
            new Monkey()
            {
                Items = { 58, 93, 88, 81, 72, 73, 65 },
                Operation = old => old + 2,
                Test = 7,
                Destinations = (6, 7),
            },
            new Monkey()
            {
                Items = { 65, 95 },
                Operation = old => old + 6,
                Test = 13,
                Destinations = (3, 5),
            },
            new Monkey()
            {
                Items = { 58, 80, 81, 83 },
                Operation = old => checked (old * old),
                Test = 5,
                Destinations = (4, 5),
            },
            new Monkey()
            {
                Items = { 58, 89, 90, 96, 55 },
                Operation = old => old + 3,
                Test = 3,
                Destinations = (1, 7),
            },
            new Monkey()
            {
                Items = { 66, 73, 87, 58, 62, 67 },
                Operation = old => old * 7,
                Test = 17,
                Destinations = (4, 1),
            },
            new Monkey()
            {
                Items = { 85, 55, 89 },
                Operation = old => old + 4,
                Test = 2,
                Destinations = (2, 0),
            },
            new Monkey()
            {
                Items = { 73, 80, 54, 94, 90, 52, 69, 58 },
                Operation = old => old + 7,
                Test = 19,
                Destinations = (6, 0),
            },
        };

        public Day_11()
        {
        }

        private class Monkey
        {
            public List<long> Items { get; set; } = new();
            public required Func<long, long> Operation { get; init; }
            public required int Test { get; init; }
            public required (int, int) Destinations { get; init; }

            public int ItemsInspected { get; private set; }

            public void TakeTurn(List<Monkey> monkeys, Func<long, long> simplifier)
            {
                ItemsInspected += Items.Count;
                foreach (var item in Items)
                {
                    var newLevel = simplifier(Operation(item));
                    var dest = (newLevel % Test == 0) ? Destinations.Item1 : Destinations.Item2;
                    Debug.Assert(this != monkeys[dest]);
                    monkeys[dest].Items.Add(newLevel);
                }
                Items.Clear();
            }

            public Monkey Clone() => new Monkey
            {
                Items = Items.ToList(),
                Operation = Operation,
                Test = Test,
                Destinations = Destinations,
            };
        }

        public override async ValueTask<string> Solve_1()
        {
            var monkeys = _initialMonkeySetup.SelectList(x => x.Clone());
            for (var round = 0; round < 20; round++)
            {
                foreach (var m in monkeys)
                    m.TakeTurn(monkeys, static x => x / 3);
            }
            return monkeys.Select(m => m.ItemsInspected)
                .PartialSortBy(2, it => -it).Product().ToString();
        }


        public override async ValueTask<string> Solve_2()
        {
            var monkeys = _initialMonkeySetup.SelectList(x => x.Clone());
            for (var round = 0; round < 10_000; round++)
            {
                foreach (var m in monkeys)
                    m.TakeTurn(monkeys, static x => x % 9699690);
            }
            return monkeys.Select(m => m.ItemsInspected)
                .PartialSortBy(2, it => -it).Product().ToString();
        }
    }
}