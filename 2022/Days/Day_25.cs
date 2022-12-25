using Core;

using System.Collections.Concurrent;
using System.Drawing;

namespace AoC_2022.Days
{
    public sealed class Day_25 : BaseDay
    {
        private readonly IReadOnlyList<string> _input;

        public Day_25()
        {
            _input = File.ReadAllLines(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            return _input.SelectList(ConvertLine).Sum().ToString(); // Use excel to convert
        }

        private long ConvertLine(string line)
        {
            // the digits are 2, 1, 0, minus (written -), and double-minus (written =). Minus is worth -1, and double-minus is worth -2."
            var digitWorth = 1L;
            var result = 0L;
            foreach (var digit in line.Reverse())
            {
                result += digitWorth * ConvertDigit(digit);
                digitWorth *= 5;
            }
            return result;

            static int ConvertDigit(char digit) => digit switch
            {
                '=' => -2,
                '-' => -1,
                '0' => 0,
                '1' => 1,
                '2' => 2,
                _ => throw new NotImplementedException()
            };
        }


        public override async ValueTask<string> Solve_2()
        {
            return "Start the Blender";
        }
    }
}