using Core;

namespace AoC_2025.Days;

public sealed class Day_02 : BaseDay
{
    private readonly LongInterval[] _input;

    /// <summary>
    /// Parses input as comma-separated ranges (e.g., "1-10,20-30")
    /// </summary>
    public Day_02()
    {
        _input = File.ReadAllText(InputFilePath)
            .Split(",")
            .SelectArray(ParseRange);

        LongInterval ParseRange(string range)
        {
            var ends = range.Split("-").SelectArray(long.Parse);
            return LongInterval.FromInclusiveEnd(ends[0], ends[1]);
        }
    }

    /// <summary>
    /// Splits intervals that cross powers of 10 into constant-length sub-intervals.
    /// Example: 998..1005 becomes [998..1000) and [1000..1005)
    /// </summary>
    private static IEnumerable<LongInterval> YieldConstantLengthIntervals(LongInterval interval)
    {
        var startDigits = interval.Start.DigitCount();
        var endDigits = (interval.End - 1).DigitCount();
        
        var boundaries = Enumerable.Range(startDigits, endDigits - startDigits)
            .Select(Pow10)
            .Prepend(interval.Start)
            .Append(interval.End);
        
        return boundaries.PairwiseWithOverlap()
            .Select2((start, end) => new LongInterval(start, end));
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

    /// <summary>
    /// Maps digit count to valid pattern lengths for finding repeating patterns.
    /// For example, a 4-digit number can have patterns of length 1 or 2 (e.g., 1111, 1212).
    /// </summary>
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


    /// <summary>
    /// Part 1: Finds IDs with repeating patterns where the pattern length is half the total digits.
    /// Only processes even-length numbers (e.g., 1212, 123123).
    /// </summary>
    private static IEnumerable<long> EnumerateInvalidIds1(LongInterval interval)
    {
        var length = interval.Start.DigitCount();
        return length % 2 > 0 ? [] : EnumerateInvalidIds(interval, [length / 2]);
    }
    
    /// <summary>
    /// Part 2: Finds IDs with any repeating pattern based on the PatternLengths lookup.
    /// </summary>
    private static IEnumerable<long> EnumerateInvalidIds2(LongInterval interval)
    {
        var length = interval.Start.DigitCount();
        return EnumerateInvalidIds(interval, PatternLengths[length]);
    }

    /// <summary>
    /// Generates IDs with repeating patterns within the given interval.
    /// For each pattern length, creates numbers by repeating the pattern across all digits.
    /// </summary>
    private static HashSet<long> EnumerateInvalidIds(LongInterval interval, int[] patternLengths)
    {
        var startStr = interval.Start.ToString();
        var endStr = (interval.End - 1).ToString(); // inclusive end
        var digitCount = startStr.Length;
        var invalidIds = new HashSet<long>();

        foreach (var patternLength in patternLengths)
        {
            if (patternLength == 0 || digitCount % patternLength > 0)
                continue;

            var patternStart = int.Parse(startStr[..patternLength]);
            var patternEnd = int.Parse(endStr[..patternLength]);
            var repetitions = digitCount / patternLength;
            var multiplier = Pow10(patternLength);
            
            for (var pattern = patternStart; pattern <= patternEnd; pattern++)
            {
                var id = BuildRepeatingId(pattern, repetitions, multiplier);
                if (interval.Contains(id))
                    invalidIds.Add(id);
            }
        }

        return invalidIds;
    }

    /// <summary>
    /// Builds a number by repeating the pattern the specified number of times.
    /// Example: BuildRepeatingId(123, 3, 1000) returns 123123123
    /// </summary>
    private static long BuildRepeatingId(int pattern, int repetitions, long multiplier)
    {
        long id = pattern;
        for (var i = 1; i < repetitions; i++)
            id = id * multiplier + pattern;
        return id;
    }

    /// <summary>
    /// Efficiently calculates 10^exponent without floating-point operations.
    /// </summary>
    private static long Pow10(int exponent)
    {
        long result = 1;
        for (var i = 0; i < exponent; i++)
            result *= 10;
        return result;
    }
}