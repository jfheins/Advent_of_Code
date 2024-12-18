using Core;
using System.Linq;
using System.Drawing;

namespace AoC_2024.Days;

public sealed partial class Day_00 : BaseDay
{
    private readonly string[] _input;

    public Day_00()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        // var grid = new FiniteGrid2D<char>(_input);
        return "-";
    }

    public override async ValueTask<string> Solve_2()
    {
        return "-";
    }
}