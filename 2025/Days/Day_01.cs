using Core;
using System.Linq;
using System.Drawing;

namespace AoC_2025.Days;

public sealed partial class Day_01 : BaseDay
{
    private readonly string[] _input;

    public Day_01()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var dial = 50;
        var solution = 0;
        foreach (var line in _input)
        {
            var change = line.ParseInts(1)[0];
            change *= line.StartsWith('L') ? 1 : -1;
            dial = (dial + change).Modulo(100);
            if (dial == 0)
            {
                solution++;
            }
        }

        return solution.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var dial = 50;
        var solution = 0;
        foreach (var line in _input)
        {
            var change = line.ParseInts(1)[0];
            var sign = line.StartsWith('L') ? 1 : -1;

            for (int i = 0; i < change; i++)
            {
                dial = (dial + sign).Modulo(100);
                if (dial == 0)
                {
                    solution++;
                }
            }
        }

        return solution.ToString();
    }
}