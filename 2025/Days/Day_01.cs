using Core;
using System.Linq;
using System.Drawing;

namespace AoC_2025.Days;

public sealed class Day_01 : BaseDay
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
            
            var result = DivMod(dial + change * sign, 100);
            var delta = Math.Abs(result.block);
            
            // Edge case: when moving left from 0, we already counted the 0 when we landed
            if (dial == 0 && sign < 0 && delta > 0)
                delta -= 1;
            
            // Edge case: when moving left and landing exactly on 0, we need to count that 0
            if (sign < 0 && result.remainder == 0 && dial != 0)
                delta += 1;

            solution += delta;
            dial = result.remainder;
        }

        return solution.ToString();
    }

    private static (int block, int remainder) DivMod(int num, int denom)
    {
        // calculates integer division and remainder, handling negative numbers correctly
        // so that -1 / 100 returns (-1, 99) instead of (0, -1)
        var block = num / denom;
        var remainder = num % denom;
        if (remainder < 0)
        {
            block--;
            remainder += denom;
        }

        return (block, remainder);
    }
}