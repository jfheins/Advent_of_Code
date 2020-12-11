using System;
using System.IO;
using System.Linq;


namespace AoC_2020.Days
{
    public class Day_05 : BaseDay
    {
        private readonly string[] _input;

        public Day_05()
        {
            _input = File.ReadAllLines(InputFilePath);
        }

        public override string Solve_1()
        {
            var binary = _input.Select(s => s.Replace("R", "1").Replace("B", "1").Replace('F', '0').Replace('L', '0'));
            var seatIds = binary.Select(x => Convert.ToInt32(x, 2));
            return seatIds.Max().ToString();
        }

        public override string Solve_2()
        {
            var binary = _input.Select(s => s.Replace("R", "1").Replace("B", "1").Replace('F', '0').Replace('L', '0'));
            var seatIds = binary.Select(x => Convert.ToInt32(x, 2)).OrderBy(x => x).ToList();
            var offset = seatIds.First();
            return (seatIds.Where((seatId, idx) => idx + offset != seatId).First() - 1).ToString();
        }
    }
}
