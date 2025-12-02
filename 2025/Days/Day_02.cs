using Core;
using System.Linq;
using System.Drawing;
using System.Xml;
using Flurl.Util;

namespace AoC_2025.Days;

public sealed partial class Day_02 : BaseDay
{
    private readonly LongInterval[] _input;

    public Day_02()
    {
        _input = File.ReadAllText(InputFilePath).Split(",").SelectArray(ParseRange);

        LongInterval ParseRange(string range)
        {
            var ends = range.Split("-").SelectArray(long.Parse);
            return LongInterval.FromInclusiveEnd(ends[0], ends[1]);
        }
    }

    public override async ValueTask<string> Solve_1()
    {
        var invalid = _input.SelectMany(it => it).Where(id => IsInvalid1(id.ToString())).Sum();
        return invalid.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var invalid = _input.SelectMany(it => it).Where(id => IsInvalid2(id.ToString())).Sum();
        return invalid.ToString();
    }


    private static bool IsInvalid1(ReadOnlySpan<char> strId)
    {
        if (strId.Length % 2 == 1)
            return false;
        var halfLen = strId.Length / 2;
        return strId[..halfLen].SequenceEqual(strId[halfLen..]);
    }

    private static bool IsInvalid2(ReadOnlySpan<char> strId)
    {
        for (var prefixLen = 1; prefixLen <= strId.Length / 2; prefixLen++)
        {
            if (strId.Length % prefixLen > 0)
                continue;
            var prefix = strId[..prefixLen];
            var allMatch = true;
            for (var i = 1; i < strId.Length / prefixLen; i++)
            {
                var part = strId[(prefixLen * i)..(prefixLen * (i + 1))];
                if (!prefix.SequenceEqual(part))
                {
                    allMatch = false;
                    break;
                }
            }

            if (allMatch)
                return true;
        }

        return false;
    }
}