using Core;
using Spectre.Console;
using System.Drawing;

namespace AoC_2024.Days;

public sealed partial class Day_02 : BaseDay
{
    private readonly int[][] _input;

    public Day_02()
    {
        _input = File.ReadAllLines(InputFilePath).SelectArray(x => x.ParseInts());
    }

    public override async ValueTask<string> Solve_1()
    {
        var safeCount = _input.Count(IsSafe);
        return safeCount.ToString();
    }

    private static bool IsSafe(IEnumerable<int> it)
        => IsSafe(it.ToArray());

    private static bool IsSafe(int[] s)
        => s.Diff().Select(Math.Sign).AreAllEqual()
           && s.Diff().All(x => Math.Abs(x) is >= 1 and <= 3);

    private static IEnumerable<int>[] OmitAnyOne(int[] s)
        => Enumerable.Range(0, s.Length)
            .Select(s.OmitAt).ToArray();

    public override async ValueTask<string> Solve_2()
    {
        return _input.Count(it => IsSafe(it) || OmitAnyOne(it).Any(IsSafe)).ToString();
    }
}