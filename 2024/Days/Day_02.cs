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
        // var grid = new FiniteGrid2D<char>(_input);
        var safeCount = _input.Count(IsSafe);
        return safeCount.ToString();
    }

    bool IsSafe(IEnumerable<int> s)
        => (s.Diff().All(x => x > 0)
            || s.Diff().All(x => x < 0))
            && s.Diff().All(x => Math.Abs(x) <= 3);

    static IEnumerable<int>[] OmitAnyOne(ICollection<int> s)
        => Enumerable.Range(0, s.Count)
        .Select(it => s.OmitAt(it)).ToArray();

    public override async ValueTask<string> Solve_2()
    {
        return _input.Count(it => IsSafe(it) || OmitAnyOne(it).Any(IsSafe)).ToString();
    }
}