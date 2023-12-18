using Core;
using System.Drawing;

namespace AoC_2023.Days;

public sealed class Day_18 : BaseDay
{
    private readonly string[] _input;

    public Day_18()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        return Solve(_input.SelectArray(ParseLine1));
    }

    public override async ValueTask<string> Solve_2()
    {
        return Solve(_input.SelectArray(ParseLine2));
    }

    private static string Solve(IEnumerable<(Direction d, int dist)> instructions)
    {
        var borderPoints = instructions
            .RunningFold(Point.Empty, (point, step) => point.MoveTo(step.d, step.dist))
            .Prepend(Point.Empty).ToList();
        var info =  borderPoints.PickTheorem();
        return (info.edge + info.inside).ToString();
    }

    private static (Direction d, int dist) ParseLine1(string l)
    {
        var split = l.Split(' ');
        var d = split[0][0] switch
        {
            'L' => Direction.Left,
            'U' => Direction.Up,
            'R' => Direction.Right,
            'D' => Direction.Down,
        };
        return (d, int.Parse(split[1]));
    }

    private static (Direction d, int dist) ParseLine2(string l)
    {
        var split = l.Split(' ')[2];
        var d = split[7] switch
        {
            '2' => Direction.Left,
            '3' => Direction.Up,
            '0' => Direction.Right,
            '1' => Direction.Down,
        };
        var hex = split[2..^2];
        return (d, Convert.ToInt32(hex, 16));
    }
}