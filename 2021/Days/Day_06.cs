using Core;
using Core.Combinatorics;
using MoreLinq.Extensions;
using System.IO;
using System.Linq;

namespace AoC_2021.Days
{
    public class Day_06 : BaseDay
    {
        private string _input;
        private int[] _numbers;

        public Day_06()
        {
            _input = File.ReadAllText(InputFilePath);
            _numbers = _input.Split(",").Select(int.Parse).ToArray();
        }
            
        public override async ValueTask<string> Solve_1()
        {
            var allFish = _numbers.ToLookup(x => x)
                            .Select(g => (age: g.Key, count: g.LongCount())).ToList();
            for (int i = 0; i < 80; i++)
            {
                allFish = allFish.SelectMany(Grow).ToList();
                if (allFish.Count > 9)
                {
                    allFish = allFish.ToLookup(x => x.age)
                            .Select(g => (age: g.Key, count: g.Sum(x => x.count))).ToList();
                }
            }
            return allFish.Sum(x => x.count).ToString();
        }

        private IEnumerable<int> Grow(int x)
        {
            if (x == 0)
            {
                yield return 6;
                yield return 8;
            }
            else
                yield return x - 1;
        }

        private IEnumerable<(int age, long count)> Grow((int age, long count) group)
        {
            if (group.age == 0)
            {
                yield return (6, group.count);
                yield return (8, group.count);
            }
            else
                yield return (group.age - 1, group.count);
        }

        public override async ValueTask<string> Solve_2()
        {
            var allFish = _numbers.ToLookup(x => x)
                 .Select(g => (age: g.Key, count: g.LongCount())).ToList();
            for (int i = 0; i < 256; i++)
            {
                allFish = allFish.SelectMany(Grow).ToList();
                if (allFish.Count > 9)
                {
                    allFish = allFish.ToLookup(x => x.age)
                            .Select(g => (age: g.Key, count: g.Sum(x => x.count))).ToList();
                }
            }
            return allFish.Sum(x => x.count).ToString();
        }
    }
}
