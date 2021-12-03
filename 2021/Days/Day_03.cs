using System.IO;
using System.Linq;

using Core;
using Core.Combinatorics;

using MoreLinq.Extensions;

namespace AoC_2021.Days
{
    public class Day_03 : BaseDay
    {
        private string[] _input;
        private long[] _numbers;

        public Day_03()
        {
            _input = File.ReadAllLines(InputFilePath).ToArray();
            _numbers = _input.Select(long.Parse).ToArray();
        }

        private int PopCount(int idx, IEnumerable<string> arr)
        {
            return arr.Select(x => x[idx]).Count(x => x == '1');
        }

        public override async ValueTask<string> Solve_1()
        {
            var digits = _input[0].Length;
            var overallCount = _input.Length;
            var gammaRateStr = new string('0', digits).ToCharArray();
            for (int idx = 0; idx < digits; idx++)
            {
                var popcount = PopCount(idx, _input);
                if (popcount > overallCount / 2)
                {
                    gammaRateStr[idx] = '1';
                }
            }
            var gamma = Convert.ToInt32(new string(gammaRateStr), 2);
            var epsilon = (~gamma) & 0b111111111111;
            return (gamma * epsilon).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var digits = _input[0].Length;
            var num = _input.ToList();
            // oxygen generator rating, determine the most common 
            for (int idx = 0; idx < digits; idx++)
            {
                var popcount = PopCount(idx, num);
                if (2*popcount >= num.Count)
                { // Keep 1
                    num = num.Where(s => s[idx] == '1').ToList();
                }
                else
                {
                    num = num.Where(s => s[idx] == '0').ToList();
                }
                if (num.Count == 1)
                {
                    break;
                }
            }
            var oxygenRating = num.First();
            num = _input.ToList();

            // co2 generator rating, determine the least common 
            for (int idx = 0; idx < digits; idx++)
            {
                var popcount = PopCount(idx, num);
                if (2*popcount >= num.Count)
                { // Keep 1
                    num = num.Where(s => s[idx] == '0').ToList();
                }
                else
                {
                    num = num.Where(s => s[idx] == '1').ToList();
                }
                if (num.Count == 1)
                {
                    break;
                }
            }
            var co2Rating = num.First();

            var a = Convert.ToInt32(oxygenRating, 2);
            var b = Convert.ToInt32(co2Rating, 2);
            return (a*b).ToString();

        }
    }
}
