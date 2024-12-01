using Core;

namespace AoC_2024.Days;

public sealed class Day_01 : BaseDay
{
    private readonly (int left, int right)[] _input;

    public Day_01()
    {
        _input = File.ReadAllLines(InputFilePath).SelectArray(line => line.ParseInts(2).ToTuple2());
    }

    public override async ValueTask<string> Solve_1()
    {
        var left = _input.Select2((a, b) => a).Order();
        var right = _input.Select2((a, b) => b).Order();
        return left.Zip(right).Sum(it => Math.Abs(it.First - it.Second)).ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var right = _input.Select2((a, b) => b).CountBy(x => x).ToDictionary();
        var left = _input.Select2((a, b) => a);
        return left.Sum(x => x * right.GetValueOrDefault(x)).ToString();
    }
}