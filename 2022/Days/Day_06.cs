using Core;
using System.Diagnostics;
using System.Collections.Immutable;

using static MoreLinq.Extensions.SplitExtension;
using static MoreLinq.Extensions.TransposeExtension;

namespace AoC_2022.Days
{
    public sealed class Day_06 : BaseDay
    {
        private readonly string _input;

        public Day_06()
        {
            _input = File.ReadAllText(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            for (int i = 4; i < _input.Length; i++)
            {
                var last4 = _input.Substring(i - 4, 4);
                if (last4.Distinct().Count() == 4)
                    return i.ToString();
            }
            return "-";
        }

        public override async ValueTask<string> Solve_2()
        {
            for (int i = 14; i < _input.Length; i++)
            {
                var last4 = _input.Substring(i - 14, 14);
                if (last4.Distinct().Count() == 14)
                    return i.ToString();
            }
            return "-";
        }
    }
}