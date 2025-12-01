using Core;

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
        return _input
            .Select(ParseChange)
            .RunningFold(50, (dial, change) => (dial + change).Modulo(100))
            .Count(dial => dial == 0)
            .ToString();

        int ParseChange(string line)
        {
            var change = line.ParseInts(1)[0];
            return change * (line.StartsWith('L') ? -1 : 1);
        }
    }

    public override async ValueTask<string> Solve_2()
    {
        var dial = 50;
        var solution = 0;
        foreach (var line in _input)
        {
            var change = line.ParseInts(1)[0];
            var sign = line.StartsWith('L') ? -1 : 1;

            // Mirror dial if turning left
            var tempDial = (dial * sign).Modulo(100);
            var fullTurns = Math.DivRem(tempDial + change, 100, out var newDial);

            solution += fullTurns;
            dial = (newDial * sign).Modulo(100); // Mirror back
        }

        return solution.ToString();
    }
}