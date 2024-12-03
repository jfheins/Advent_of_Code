using Core;
using System.Text.RegularExpressions;

namespace AoC_2024.Days;

public sealed partial class Day_03 : BaseDay
{
    private readonly string _input;

    public Day_03()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        long res = 0;
        foreach (var match in Part1Regex().EnumerateMatches(_input))
        {
            res += _input.AsSpan(match).ParseInts(2).Product();
        }
        return res.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        long res = 0;
        bool active = true;
        foreach (var match in Part2Regex().EnumerateMatches(_input))
        {
            var matchValue = _input.AsSpan(match);
            if (matchValue is "do()")
                active = true;
            else if (matchValue is "don't()")
                active = false;
            else if (active)
                res += matchValue.ParseInts(2).Product();
        }
        return res.ToString();
    }

    [GeneratedRegex(@"mul\([0-9]{1,3},[0-9]{1,3}\)")]
    private static partial Regex Part1Regex();

    [GeneratedRegex(@"mul\([0-9]{1,3},[0-9]{1,3}\)|do\(\)|don't\(\)")]
    private static partial Regex Part2Regex();
}