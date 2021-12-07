using Core;
using System.Diagnostics;

namespace AoC_2021.Days
{
    public class Day_07 : BaseDay
    {
        private List<int> _numbers;

        public Day_07()
        {
            var input = File.ReadAllText(InputFilePath).Split(",");
            _numbers =  input.Select(int.Parse).ToList();
        }

        public override async ValueTask<string> Solve_1()
        {
            _numbers.Sort();
            var minimum = OverallFuel1(_numbers.Median());
            return minimum.ToString();
        }

        private int OverallFuel1(int pos) => _numbers.Sum(p => Math.Abs(p - pos));

        private long OverallFuel2(int pos)
        {
            return _numbers.Sum(p => Fuel(Math.Abs(p - pos)));
            static long Fuel(long dist) => (dist * (dist + 1)) / 2;
        }

        public override async ValueTask<string> Solve_2()
        {
            var avg = _numbers.Average();
            var lowerBound = (int)Math.Floor(avg);
            var upperBound = lowerBound + 1;
            var result = Math.Min(OverallFuel2(lowerBound), OverallFuel2(upperBound));
            return result.ToString();
        }
    }
}
