using Core;
using System.Linq;
using System.Drawing;

namespace AoC_2025.Days;

public sealed partial class Day_05 : BaseDay
{
    private readonly string[] _input;

    public Day_05()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var (ranges, ingred) = _input.SplitBy("").ToTuple2();

        var r2 = ranges.Select(it => it.Split("-").ToTuple2())
            .Select2((start, end) => LongInterval.FromInclusiveEnd(long.Parse(start), long.Parse(end))).ToList();
        var i = ingred.Select(it => long.Parse(it)).ToList();

        var freshCount = i.Count(it => r2.Any(r => r.Contains(it)));
        return freshCount.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var (ranges, _) = _input.SplitBy("").ToTuple2();

        var r2 = ranges.Select(it => it.Split("-").ToTuple2())
            .Select2((start, end) => LongInterval.FromInclusiveEnd(long.Parse(start), long.Parse(end))).ToList();

        var r3 = new List<LongInterval>();
        while (r2.Count > 0)
        {
            var cand = r2[0];
            r2.Remove(cand);
            LongInterval other;
            while ((other = r2.FirstOrDefault(o => o.OverlapsWith(cand))) != default)
            {
                cand = cand.Union(other);
                r2.Remove(other);
            }

            r3.Add(cand);
        }

        return r3.Select(it => it.Length).Sum().ToString();
    }
}