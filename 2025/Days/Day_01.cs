using Core;

namespace AoC_2025.Days;

public sealed class Day_01 : BaseDay
{
    private readonly int[] _changes;

    public Day_01()
    {
        _changes = File.ReadAllLines(InputFilePath)
            .SelectArray(ParseChange);
    }

    public override async ValueTask<string> Solve_1()
    {
        return _changes
            .RunningFold(50, (dial, change) => (dial + change).Modulo(100))
            .Count(dial => dial == 0)
            .ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        return _changes
            .Aggregate((dial: 50, zeroCrossings: 0), (state, change) =>
            {
                var sign = change < 0 ? -1 : 1;
                var absChange = Math.Abs(change);
                
                // Mirror dial if turning left
                var tempDial = (state.dial * sign).Modulo(100);
                var fullTurns = Math.DivRem(tempDial + absChange, 100, out var newDial);
                newDial = (newDial * sign).Modulo(100);
                
                return (dial: newDial, zeroCrossings: state.zeroCrossings + fullTurns);
            })
            .zeroCrossings
            .ToString();
    }

    private static int ParseChange(string line)
    {
        var change = int.Parse(line[1..]);
        return change * (line.StartsWith('L') ? -1 : 1);
    }
}