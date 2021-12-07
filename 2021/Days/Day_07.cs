using Core;

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
            var s = new BinarySearchInt(Descending);
            var minIdx = s.FindLast(0, _numbers.Max());
            return OverallFuel1(minIdx).ToString();
        }

        private bool Descending(int x)
        {// descending if the number of points smaller than x is less than half
            var pointsBelow = _numbers.Count(it => it < x);
            return 2 * pointsBelow <= _numbers.Count;
        }

        private int OverallFuel1(int pos)
        {
            return _numbers.Sum(p => Math.Abs(p - pos));
        }

        private long OverallFuel2(int pos)
        {
            return _numbers.Sum(p => Fuel(Math.Abs(p - pos)));

            static long Fuel(long dist) => (dist * (dist + 1)) / 2;
        }

        public override async ValueTask<string> Solve_2()
        {
            var avg = (int)Math.Round(_numbers.Average());
            return OverallFuel2(avg).ToString();
        }
    }
}
