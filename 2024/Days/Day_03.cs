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

    [GeneratedRegex(@"mul\([0-9]{1,3},[0-9]{1,3}\)")]
    private static partial Regex Part1Regex();

    public override ValueTask<string> Solve_1()
    {
        long res = 0;
        foreach (var match in Part1Regex().EnumerateMatches(_input))
        {
            res += _input.AsSpan(match).ParseInts(2).Product();
        }

        return ValueTask.FromResult(res.ToString());
    }

    [GeneratedRegex(@"mul\([0-9]{1,3},[0-9]{1,3}\)|do\(\)|don't\(\)")]
    private static partial Regex Part2Regex();

    public override ValueTask<string> Solve_2()
    {
        long res = 0;
        var active = true;
        foreach (var match in Part2Regex().EnumerateMatches(_input))
        {
            var instruction = _input.AsSpan(match);
            if (instruction is "do()")
                active = true;
            else if (instruction is "don't()")
                active = false;
            else if (active)
                res += instruction.ParseInts(2).Product();
        }

        return ValueTask.FromResult(res.ToString());
    }
}