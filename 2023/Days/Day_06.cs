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
        var times = _input[0].ParseInts();
        var dist = _input[1].ParseInts();

        List<int> ways = new();
        for (var raceIdx = 0; raceIdx < times.Length; raceIdx++)
        {
            var recordDist = dist[raceIdx];
            var wayss = 0;
            for (var holdTime = 1; holdTime < times[raceIdx]; holdTime++)
            {
                var travelDist = holdTime * (times[raceIdx] - holdTime);
                if (travelDist > recordDist)
                {
                    wayss++;
                }
            }
            ways.Add(wayss);
        }

        return ways.Product().ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        long time = 60808676;
        long recordDist = 601116315591300;
        var ways = 0L;

        for (long holdTime = 1; holdTime < time; holdTime++)
        {
            var travelDist = holdTime * (time - holdTime);
            if (travelDist > recordDist)
            {
                ways++;
            }
        }

        return ways.ToString();
    }
}