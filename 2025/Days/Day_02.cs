using System.Collections;
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

    private static IEnumerable<LongInterval> YieldConstantLengthIntervals(LongInterval interval)
    {
        // Any interval that crosses a power of 10 needs to be split. So 998..1005 becomes 998..999 and 1000..1005
        var startDigits = interval.Start.DigitCount();
        var endDigits = (interval.End - 1).DigitCount();
        var lower = interval.Start;
        for (var length = startDigits; length < endDigits; length++)
        {
            var split = (long)Math.Pow(10, length);
            yield return new LongInterval(lower, split);
            lower = split;
        }

        yield return new LongInterval(lower, interval.End);
    }

    public override async ValueTask<string> Solve_1()
    {
        return _input
            .SelectMany(YieldConstantLengthIntervals)
            .SelectMany(EnumerateInvalidIds1)
            .Sum()
            .ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        return _input
            .SelectMany(YieldConstantLengthIntervals)
            .SelectMany(EnumerateInvalidIds2)
            .Sum()
            .ToString();
    }

    private static readonly Dictionary<int, int[]> PatternLengths = new()
    {
        { 1, [] },
        { 2, [1] },
        { 3, [1] },
        { 4, [1, 2] },
        { 5, [1] },
        { 6, [1, 2, 3] },
        { 7, [1] },
        { 8, [1, 2, 4] },
        { 9, [1, 3] },
        { 10, [1, 2, 5] },
    };


    private static IEnumerable<long> EnumerateInvalidIds1(LongInterval interval)
    {
        var length = interval.Start.DigitCount();
        return length % 2 > 0 ? [] : EnumerateInvalidIds(interval, [length / 2]);
    }
    
    private static IEnumerable<long> EnumerateInvalidIds2(LongInterval interval)
    {
        var length = interval.Start.DigitCount();
        return EnumerateInvalidIds(interval, PatternLengths[length]);
    }

    private static HashSet<long> EnumerateInvalidIds(LongInterval interval, int[] patternLengths)
    {
        var startStr = interval.Start.ToString();
        var endStr = (interval.End - 1).ToString(); // inclusive end
        var invalidIds = new HashSet<long>();

        foreach (var patternLength in patternLengths)
        {
            if (patternLength == 0 || startStr.Length % patternLength > 0)
                continue;

            var start = int.Parse(startStr[..patternLength]);
            var end = int.Parse(endStr[..patternLength]);
            for (var i = start; i <= end; i++)
            {
                var id = MakeId(i);
                if (interval.Contains(id))
                    invalidIds.Add(id);
            }

            long MakeId(int pattern)
            {
                long id = pattern;
                var multiplier = (long)Math.Pow(10, patternLength);
                for (var i = 1; i < startStr.Length / patternLength; i++)
                    id = id * multiplier + pattern;

                return id;
            }
        }

        return invalidIds;
    }
}