using Core;
using Spectre.Console;
using System.Drawing;

namespace AoC_2023.Days;

public sealed partial class Day_06 : BaseDay
{
    private readonly string[] _input;

    public Day_06()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        IEnumerable<(int Time, int RecordDist)> races =
            _input[0].ParseInts().Zip(_input[1].ParseInts());
        
        return races
            .Select(t => WinningHoldTimes(t.Time, t.RecordDist).Length)
            .Product().ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var time = _input[0].ReadOneLong();
        var dist = _input[1].ReadOneLong();
        return WinningHoldTimes(time, dist).Length.ToString();
    }

    private static LongInterval WinningHoldTimes(long raceTime, long recordDist)
    {
        var d = Math.Sqrt(raceTime * raceTime - 4 * (recordDist + 1));
        var t1 = Math.Ceiling((raceTime - d) / 2);
        var t2 = Math.Floor((raceTime + d) / 2);
        return LongInterval.FromInclusiveEnd((long)t1, (long)t2);
    }
}