using Core;
using System.Linq;
using System.Drawing;
using System.Xml;
using Flurl.Util;

namespace AoC_2025.Days;

public sealed class Day_02 : BaseDay
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
        var invalid = _input.SelectMany(it => it).Where(IsInvalid1).Sum();
        return invalid.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var invalid = _input.SelectMany(it => it).Where(IsInvalid2).Sum();
        return invalid.ToString();
    }


    private static bool IsInvalid1(ReadOnlySpan<char> strId)
    {
        if (strId.Length % 2 == 1)
            return false;
        var halfLen = strId.Length / 2;
        return strId[..halfLen].SequenceEqual(strId[halfLen..]);
    }

    private static bool IsInvalid1(long id)
    {
        // A number that has 2 halves will be of the form xx * 101 or xxx * 1001 or xxxx * 10001 etc.
        // With xx being the prefix and 1001 is called magic number.
        // So the prefix needs to be half the number and the magic number has to divide id cleanly.
        var patternLength = id.DigitCount() / 2;
        var magicNumber = (long)Math.Pow(10, patternLength) + 1;
        return id % magicNumber == 0 && (id / magicNumber).DigitCount() == patternLength;
    }

    private static readonly long[] MagicNumbers1 = [11, 111, 1111, 11111, 111111, 1111111, 11111111, 111111111, 1111111111];
    private static readonly long[] MagicNumbers2 = [101, 10101, 1010101, 101010101];
    private static readonly long[] MagicNumbers3 = [1001, 1001001];
    private static readonly long[] MagicNumbers4 = [10001];
    private static readonly long[] MagicNumbers5 = [100001];

    private static bool IsInvalid2(long id)
    {
        if (MagicNumbers1.Any(l => Divides(l, 1)))
            return true;
        if (MagicNumbers2.Any(l => Divides(l, 2)))
            return true;
        if (MagicNumbers3.Any(l => Divides(l, 3)))
            return true;
        if (MagicNumbers4.Any(l => Divides(l, 4)))
            return true;
        if (MagicNumbers5.Any(l => Divides(l, 5)))
            return true;

        return false;

        bool Divides(long magicNumber, int patternLength)
            => id % magicNumber == 0 && (id / magicNumber).DigitCount() == patternLength;
    }

    private static bool IsInvalid2(ReadOnlySpan<char> strId)
    {
        for (var patternLen = 1; patternLen <= strId.Length / 2; patternLen++)
        {
            if (strId.Length % patternLen > 0)
                continue;
            var pattern = strId[..patternLen];
            var allMatch = true;
            for (var i = 1; i < strId.Length / patternLen; i++)
            {
                var part = strId[(patternLen * i)..(patternLen * (i + 1))];
                if (!pattern.SequenceEqual(part))
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