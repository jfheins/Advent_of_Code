using Core;

namespace AoC_2024.Days;

public sealed class Day_01 : BaseDay
{
    private readonly string[] _input;

    public Day_01()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var left = _input.Select(x => x.ParseInts(2)[0]).Order().ToList();
        var right = _input.Select(x => x.ParseInts(2)[1]).Order().ToList();
        var dist = left.Zip(right).Select(x => Math.Abs(x.First - x.Second)).Sum();

        return dist.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var left = _input.Select(x => x.ParseInts(2)[0]).Order().ToList();
        var right = _input.Select(x => x.ParseInts(2)[1]).Order().ToList();

        var score = left.Select(x => x*right.Count(r => r == x)).ToList();

        return score.Sum().ToString();
    }
}