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
        return _input[0];
    }

    public override async ValueTask<string> Solve_2()
    {
        return "_2_";
    }
}