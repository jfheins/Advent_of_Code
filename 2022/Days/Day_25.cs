using static MoreLinq.Extensions.ZipLongestExtension;

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
            return _input.Aggregate(SumQuinary);
        }

        private static string SumQuinary(string num1, string num2)
        {
            var sequence = num1.Reverse().ZipLongest(num2.Reverse(), (a, b) => (a, b));

            var result = new List<char>();
            var carry = 0;
            foreach (var (a, b) in sequence)
            {
                var res = SumOfDigits(a, b, carry);
                result.Add(ConvertDigit(res.sum));
                carry = res.carry;
            }
            result.Add(ConvertDigit(carry));
            result.Reverse();
            return string.Concat(result.SkipWhile(c => c == '0'));
        }

        static (int sum, int carry) SumOfDigits(char a, char b, int carry = 0)
            => (ConvertDigit(a) + ConvertDigit(b) + carry) switch
            {
                -5 => (0, -1),
                -4 => (1, -1),
                -3 => (2, -1),
                -2 => (-2, 0),
                -1 => (-1, 0),
                0 => (0, 0),
                1 => (1, 0),
                2 => (2, 0),
                3 => (-2, 1),
                4 => (-1, 1),
                5 => (0, 1),
                _ => throw new NotImplementedException()
            };

        static int ConvertDigit(char digit) => digit switch
        {
            '=' => -2,
            '-' => -1,
            '0' => 0,
            '1' => 1,
            '2' => 2,
            '\0' => 0,
            _ => throw new NotImplementedException()
        };

        static char ConvertDigit(int value) => value switch
        {
            -2 => '=',
            -1 => '-',
            0 => '0',
            1 => '1',
            2 => '2',
            _ => throw new NotImplementedException()
        };


        public override async ValueTask<string> Solve_2()
        {
            return "Start the Blender";
        }
    }
}