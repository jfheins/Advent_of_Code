using Core;

using static MoreLinq.Extensions.PartialSortByExtension;

namespace AoC_2022.Days
{
    public sealed class Day_11 : BaseDay
    {
        private readonly List<Monkey> _monkeys = new List<Monkey>
        {
            new Monkey()
            {
                Items = { 57 },
                Operation = it => it * 13,
                Destination = x => (x % 11 == 0) ? 3 : 2,
            },
            new Monkey()
            {
                Items = { 58, 93, 88, 81, 72, 73, 65 },
                Operation = old => old +2,
                Destination = x => (x % 7 == 0) ? 6 : 7,
            },
            new Monkey()
            {
                Items = { 65, 95 },
                Operation = old => old +6,
                Destination = x => (x % 13 == 0) ? 3 : 5,
            },
            new Monkey()
            {
                Items = { 58, 80, 81, 83 },
                Operation = old => old * old,
                Destination = x => (x % 5 == 0) ? 4 : 5,
            },
            new Monkey()
            {
                Items = { 58, 89, 90, 96, 55 }, // 4
                Operation = old => old + 3,
                Destination = x => (x % 3 == 0) ? 1 : 7,
            },
            new Monkey()
            {
                Items = { 66, 73, 87, 58, 62, 67 },
                Operation = old => old * 7,
                Destination = x => (x % 17 == 0) ? 4 : 1,
            },
            new Monkey()
            {
                Items = { 85, 55, 89 },
                Operation = old => old  + 4,
                Destination = x => (x % 2 == 0) ? 2 : 0,
            },
            new Monkey()
            {
                Items = { 73, 80, 54, 94, 90, 52, 69, 58 },
                Operation = old => old +7,
                Destination = x => (x % 19 == 0) ? 6 : 0,
            },
            // new Monkey()
            //{
            //    Items = { 79, 98 },
            //    Operation = it => it * 19,
            //    Destination = x => (x % 23 == 0) ? 2 : 3,
            //},
            //new Monkey()
            //{
            //    Items = {  54, 65, 75, 74 },
            //    Operation = old =>  old + 6,
            //    Destination = x => (x % 19 == 0) ? 2 : 0,
            //},
            //new Monkey()
            //{
            //    Items = {  79, 60, 97 },
            //    Operation = old => old * old,
            //    Destination = x => (x % 13 == 0) ? 1 : 3,
            //},
            //new Monkey()
            //{
            //    Items = {74 },
            //    Operation = old => old + 3,
            //    Destination = x => (x % 17 == 0) ? 0 : 1,
            //},
        };

        public Day_11()
        {
            //  _input = File.ReadAllLines(InputFilePath).Split("").ToList();
        }

        private class Monkey
        {
            public List<long> Items { get; set; } = new();
            public Func<long, long> Operation;
            public Func<long, int> Destination;

            public int ItemsInspected { get; private set; }

            public void TakeTurn(List<Monkey> monkeys)
            {
                var backup = Items.ToList();
                ItemsInspected += Items.Count;
                Items.Clear();
                foreach (var item in backup)
                {
                    var newLevel = Operation(item) / 3;
                    var dest = Destination(newLevel);
                    monkeys[dest].Items.Add(newLevel);
                }
            }

            public void TakeTurn2(List<Monkey> monkeys)
            {
                var backup = Items.ToList();
                ItemsInspected += Items.Count;
                Items.Clear();
                foreach (var item in backup)
                {
                    var newLevel = Operation(item) % 9699690;
                    var dest = Destination(newLevel);
                    monkeys[dest].Items.Add(newLevel);
                }
            }

            public Monkey Clone() => new Monkey
            {
                Items = Items.ToList(),
                Operation = Operation,
                Destination = Destination,
            };
        }

        public override async ValueTask<string> Solve_1()
        {
            var easyMonkeys = _monkeys.SelectList(x => x.Clone());
            for (int round = 0; round < 20; round++)
            {
                foreach (var m in easyMonkeys)
                {
                    m.TakeTurn(easyMonkeys);
                }
            }
            var ii = easyMonkeys.Select(m => m.ItemsInspected).ToList();
            return ii.PartialSortBy(2, it => -it).Product().ToString();
        }


        public override async ValueTask<string> Solve_2()
        {
            var busyMonkeys = _monkeys.SelectList(x => x.Clone());
            for (int round = 0; round < 10_000; round++)
            {
                foreach (var m in busyMonkeys)
                {
                    m.TakeTurn2(busyMonkeys);
                }
                if (round % 20 == 0)
                {

                    Console.Write($"Round\t{round}\t");
                    foreach (var m in busyMonkeys)
                    {
                        Console.Write($"{m.ItemsInspected}\t");

                    }
                    Console.WriteLine();
                }
            }// 28385994904
            var ii = busyMonkeys.Select(m => m.ItemsInspected).ToList();
            return ii.PartialSortBy(2, it => -it).Product().ToString();
        }
    }
}