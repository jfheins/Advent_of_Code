using Core;
using Core.Combinatorics;
using MoreLinq.Extensions;
using System.IO;
using System.Linq;

namespace AoC_2021.Days
{
    public class Day_07 : BaseDay
    {
        private int[] _numbers;

        public Day_07()
        {
            var input = File.ReadAllText(InputFilePath).Split(",").ToArray();
            _numbers =  input.Select(int.Parse).ToArray();
        }

        public override async ValueTask<string> Solve_1()
        {
            var max = _numbers.Max();
            var cost = Enumerable.Range(0, max).Select(CalcCost1).Min();

            return cost.ToString();
        }

        private int CalcCost1(int pos) => _numbers.Sum(p => Math.Abs(p - pos));
        private long CalcCost2(int pos)
        {
            var c = _numbers.Sum(p => Fuel(Math.Abs(p - pos)));
            Console.WriteLine($"{pos} => {c}");
            return c;
            long Fuel(long dist) => (dist * (dist + 1)) / 2;
        }

        public override async ValueTask<string> Solve_2()
        {
            var max = _numbers.Max();
            var min = Enumerable.Range(0, max).Select(CalcCost2).Min();
            //var min = int.MaxValue;
            //for (int i = 0; i < max; i++)
            //{
            //    Console.WriteLine(i);
            //    var cost = CalcCost2(i);
            //    if (cost < min)
            //        min = cost;
            //}

            return min.ToString();
        }
    }
}
