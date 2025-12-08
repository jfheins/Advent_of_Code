using Core;
using Core.Combinatorics;

namespace AoC_2025.Days;

public sealed class Day_08 : BaseDay
{
    private readonly string[] _input;

    public Day_08()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var boxes = _input.SelectArray(it => it.ParseInts().ToPoint3());

        var ds = new DisjointSet<Point3>(boxes.Length);
        foreach (var (a, b) in new TupleCombinations2<Point3>(boxes).OrderBy(Distance).Take(1000))
        {
            ds.AddConnection(a, b);
        }

        var circuits = ds.GetAllConnectedSets();
        var sizes = circuits.Select(c => c.Count).OrderDescending().Take(3).ToArray();
        return sizes.Product().ToString();
    }


    private static double Distance((Point3, Point3) arg)
        => arg.Item1.EuklidDistTo(arg.Item2);

    public override async ValueTask<string> Solve_2()
    {
        var boxes = _input.SelectArray(it => it.ParseInts().ToPoint3());

        var ds = new DisjointSet<Point3>(boxes.Length);
        
        foreach (var pair in new TupleCombinations2<Point3>(boxes).OrderBy(Distance))
        {
            ds.AddConnection(pair.Item1, pair.Item2);

            if (ds.DisjointSetCount == 1 && ds.TotalItemCount == boxes.Length)
                return (pair.Item1.X * (long)pair.Item2.X).ToString();
        }

        return "Logic error";
    }
}