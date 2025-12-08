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

        var incoming = new Dictionary<Point3, List<Point3>>();
        var outgoing = new Dictionary<Point3, List<Point3>>();
        var bfs = new BreadthFirstSearch<Point3>(null, p => outgoing.GetValueOrDefault(p, [])) { PerformParallelSearch = false };

        foreach (var pair in new TupleCombinations2<Point3>(boxes).OrderBy(Distance).Take(1000))
        {
            if (AreConnected(pair.Item1, pair.Item2))
            {
                continue;
            }

            // connect
            outgoing.AddOrModifyAction(pair.Item1, () => [], list => list.Add(pair.Item2));
            outgoing.AddOrModifyAction(pair.Item2, () => [], list => list.Add(pair.Item1));
            incoming.AddOrModifyAction(pair.Item1, () => [], list => list.Add(pair.Item2));
            incoming.AddOrModifyAction(pair.Item2, () => [], list => list.Add(pair.Item1));
        }

        var circuits = new List<Point3[]>();
        var noted = new HashSet<Point3>();
        foreach (var box in outgoing.Keys)
        {
            if (noted.Contains(box))
                continue;
            // use bfs to find all connected boxes and put them into the list
            var circuit = bfs.FindReachable(box);
            circuits.Add(circuit.ToArray());
            foreach (var b in circuit) noted.Add(b);
        }

        var sizes = circuits.Select(c => c.Length).OrderDescending().ToArray();
        return sizes.Take(3).Product().ToString();

        bool AreConnected(Point3 a, Point3 b)
            => outgoing.ContainsKey(a) && bfs.FindFirst(a, p => p == b) != null;
    }


    private static double Distance((Point3, Point3) arg)
        => arg.Item1.EuklidDistTo(arg.Item2);

    public override async ValueTask<string> Solve_2()
    {
        
        var boxes = _input.SelectArray(it => it.ParseInts().ToPoint3());

        var incoming = new Dictionary<Point3, List<Point3>>();
        var outgoing = new Dictionary<Point3, List<Point3>>();
        var bfs = new BreadthFirstSearch<Point3>(null, p => outgoing.GetValueOrDefault(p, [])) { PerformParallelSearch = false };

        foreach (var pair in new TupleCombinations2<Point3>(boxes).OrderBy(Distance))
        {
            if (AreConnected(pair.Item1, pair.Item2))
            {
                continue;
            }

            // connect
            outgoing.AddOrModifyAction(pair.Item1, () => [], list => list.Add(pair.Item2));
            outgoing.AddOrModifyAction(pair.Item2, () => [], list => list.Add(pair.Item1));
            incoming.AddOrModifyAction(pair.Item1, () => [], list => list.Add(pair.Item2));
            incoming.AddOrModifyAction(pair.Item2, () => [], list => list.Add(pair.Item1));

            if (outgoing.Count == boxes.Length && bfs.FindReachable(pair.Item1).Count == boxes.Length)
            {
                return (pair.Item1.X * (long)pair.Item2.X).ToString();
            }
        }

        var circuits = new List<Point3[]>();
        var noted = new HashSet<Point3>();
        foreach (var box in outgoing.Keys)
        {
            if (noted.Contains(box))
                continue;
            // use bfs to find all connected boxes and put them into the list
            var circuit = bfs.FindReachable(box);
            circuits.Add(circuit.ToArray());
            foreach (var b in circuit) noted.Add(b);
        }

        var sizes = circuits.Select(c => c.Length).OrderDescending().ToArray();
        Console.WriteLine(string.Join(", ", sizes));

        return sizes.Take(3).Product().ToString();

        bool AreConnected(Point3 a, Point3 b)
            => outgoing.ContainsKey(a) && bfs.FindFirst(a, p => p == b) != null;
    }
}