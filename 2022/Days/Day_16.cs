using Core;
using Core.Combinatorics;

using Spectre.Console;

using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AoC_2022.Days
{
    public sealed class Day_16 : BaseDay
    {
        private IReadOnlyList<string> _input;
        private IReadOnlyList<(string name, int rate, string[] neighbors)> valves;
        private Dictionary<string, List<(string name, int dist)>> dist;
        private readonly Dictionary<string, string[]> neighbors;
        private static Dictionary<string, int> rates;
        private static HashSet<string> allValves = new();
        private static List<string> usefulValves = new();

        public Day_16()
        {
            _input = File.ReadAllLines(InputFilePath);
            valves = _input.SelectList(ParseLine);
            neighbors = valves.ToDictionary(v => v.name, v => v.neighbors);
            rates = valves.ToDictionary(v => v.name, v => v.rate);
            allValves = new HashSet<string>(rates.Keys);
        }

        private (string name, int rate, string[] neighbors) ParseLine(string arg)
        {
            var rate = arg.ParseInts(1).First();
            var name = arg[6..8];
            var n = arg.Split("to valve")[1];
            if (n[0] == 's')
                n = n.Substring(2);
            var ne = n.Split(",", StringSplitOptions.TrimEntries);
            return (name, rate, ne);
        }

        public override async ValueTask<string> Solve_1()
        {
            usefulValves = valves.Where(v => v.rate > 0).SelectList(v => v.name);
            valveCount = usefulValves.Count;
            var bfs = new BreadthFirstSearch<string>(null, it => neighbors[it]);
            dist = new Dictionary<string, List<(string, int)>>();

            // From aa to all
            dist.Add("AA", bfs.FindAll("AA", usefulValves.Contains).SelectList(o => (o.Target, o.Length)));

            foreach (var v in usefulValves)
            {
                foreach (var o in bfs.FindAll(v, usefulValves.Contains))
                {
                    var li = dist.GetOrAdd(v, _ => new List<(string, int)>());
                    li.Add((o.Target, o.Length));
                }
            }

            var queue = new PriorityQueue<Node, int>();
            var start = new Node("AA", ImmutableList<string>.Empty, 0, 0);
            queue.Enqueue(start, -start.MaximumSum);
            Node? result = null;
            var closedSet = new HashSet<Node>(Node.Comparer);
            var ii = 0;
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (closedSet.Add(current))
                {
                    if (ii++ % 4000 == 0)
                        Console.WriteLine($"Max sum: {current.MaximumSum} after { current.Time }");

                    var nodes = Expander(current).Where(it => !closedSet.Contains(it)).ToList();
                    foreach (var node in nodes)
                    {
                        queue.Enqueue(node, -node.MaximumSum);
                        if (node.Name == "Z")
                        {
                            result = node;
                            queue.Clear();
                            break;
                        }
                    }
                }
            }
            return result!.Pressure.ToString();
        }

        int valveCount = 0;

        private IEnumerable<Node> Expander(Node node)
        {
            var rate = node.Rate;

            if (node.Open.Any() && "DDBBJJHHEECC".StartsWith(string.Concat(node.Open)))
            {
                ;
            }

            if (node.Open.Count == valveCount || node.Time == 30)
            {
                var remTime = 30 - node.Time;
                var finalPres = node.Pressure + rate * remTime;
                yield return new Node("Z", node.Open, finalPres, 30);
                yield break;
            }

            foreach (var n in dist[node.Name])
            {
                var destSteps = node.Time + n.dist;
                if (destSteps <= 30 && n.name != node.Name)
                {
                    var futurePressure = node.Pressure + rate * n.dist;
                    var r = new Node(n.name, node.Open, futurePressure, destSteps);
                    yield return r;
                }
            }
            if (!node.Open.Contains(node.Name) && rates[node.Name] > 0)
            {
                var futurePressure = node.Pressure + rate;
                var dest = new Node(node.Name, node.Open.Add(node.Name), futurePressure,
                    node.Time + 1);
                yield return dest;
            }
        }

        private IEnumerable<Node2> Expander2(Node2 node)
        {
            var rate = node.Rate;

            if (node.Open.Count == valveCount || node.Time == 26)
            {
                var remTime = 26 - node.Time;
                var finalPres = node.Pressure + rate * remTime;
                yield return new Node2("Z", "Z", node.Open, finalPres, 26);
                yield break;
            }

            var myOptions = neighbors[node.Name].ToList();
            if (usefulValves.Contains(node.Name) && !node.Open.Contains(node.Name))
                myOptions.Add(node.Name);

            var eleOptions = neighbors[node.EleName].ToList();
            if (usefulValves.Contains(node.EleName) && !node.Open.Contains(node.EleName))
                eleOptions.Add(node.EleName);

            var futurePressure = node.Pressure + rate;
            foreach (var a in myOptions)
            {
                foreach (var b in eleOptions)
                {
                    var newOpen = node.Open;
                    if(a == node.Name)
                        newOpen = node.Open.Add(node.Name);
                    if (b == node.EleName)
                        newOpen = node.Open.Add(node.EleName);
                    yield return new Node2(a, b, newOpen, futurePressure, node.Time + 1);
                }
            }
        }

        private record Node2(string Name, string EleName, ImmutableList<string> Open, int Pressure, int Time) : Node(Name, Open, Pressure, Time);

        private record Node(string Name, ImmutableList<string> Open, int Pressure, int Time)
        {
            public static IEqualityComparer<Node>? Comparer { get; } = new CompareIgnoreSteps();

            public int Rate = Open.Select(n => rates[n]).Sum();

            public int MaximumSum { get
                {
                    if (Time == 30)
                        return Pressure;
                    var t = Time;
                    var rate = Rate;
                    var sum = Pressure;
                    var leftValves = usefulValves.Except(Open).OrderByDescending(v => rates[v]);
                    foreach (var valve in leftValves)
                    {
                        if (valve != Name)
                        {
                            t++;
                            sum += rate;
                            if (t == 30)
                                return sum;
                        }
                        t++;
                        sum += rate;
                        rate += rates[valve];
                        if (t == 30)
                            return sum;
                    }
                    Debug.Assert(rate == rates.Values.Sum());
                    return sum + (30 - t) * rate;
                } }

            public override string ToString()
            {
                return $"@{Name}, p={Pressure}, r={Rate}, t={Time}, o:{string.Concat(Open)}";
            }
        }
        private class CompareIgnoreSteps : IEqualityComparer<Node>
        {
            public bool Equals(Node? x, Node? y)
            {

                return x.Name == y.Name
                    && x.Pressure == y.Pressure
                    && x.Rate == y.Rate;
            }

            public int GetHashCode([DisallowNull] Node obj)
            {
                return HashCode.Combine(obj.Name, obj.Pressure, obj.Open.Count);
            }
        }
        private class CompareIgnoreSteps2 : IEqualityComparer<Node2>
        {
            public bool Equals(Node2? x, Node2? y)
            {

                return x.Name == y.Name
                    && x.EleName == y.EleName
                    && x.Pressure == y.Pressure
                    && x.Rate == y.Rate;
            }

            public int GetHashCode([DisallowNull] Node2 obj)
            {
                return HashCode.Combine(obj.Name, obj.EleName, obj.Pressure, obj.Rate);
            }
        }

        //private long CalcFlow(ReadOnlyCollection<string> path)
        //{
        //    var pos = "AA";
        //    var time = 0;
        //    var open = new List<string>();
        //    var psum = 0L;
        //    var rate = 0L;
        //    foreach (var node in path)
        //    {
        //        var dist = bfs.FindFirst(pos, x => x == node)!.Length;
        //        time += dist;
        //        open.Add(pos);
        //        time++;
        //        rate += rates[node];
        //        pos = node;
        //    }
        //}

        //private IEnumerable<(string pos, string[] open, int pressure, int steps)> Expander((string pos, string[] open, int pressure, int steps) node)
        //{
        //    if (node.steps == 30)
        //        yield break;
        //    var rate = node.open.Select(n => rates[n]).Sum();
        //    var futurePressure = node.pressure + rate;
        //    // Option 1: walk
        //    foreach (var neighbor in neighbors[node.pos])
        //    {
        //        yield return (neighbor, node.open, futurePressure, node.steps+1);
        //    }
        //    // Option 2: work
        //    if(!node.open.Contains(node.pos) && rates[node.pos] > 0)
        //        yield return (node.pos, node.open.Append(node.pos).ToArray(), futurePressure, node.steps + 1);
        //}

        public override async ValueTask<string> Solve_2()
        {
            valveCount = usefulValves.Count;

            var queue = new PriorityQueue<Node2, int>();
            var start = new Node2("AA", "AA", ImmutableList<string>.Empty, 0, 0);
            queue.Enqueue(start, -start.MaximumSum);
            Node2? result = null;
            var closedSet = new HashSet<Node2>(new CompareIgnoreSteps2());
            var ii = 0;
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (closedSet.Add(current))
                {
                    if (ii++ % 40 == 0)
                        Console.WriteLine($"Max sum: {current.MaximumSum} after {current.Time}");

                    var nodes = Expander2(current).Where(it => !closedSet.Contains(it)).ToList();
                    foreach (var node in nodes)
                    {
                        queue.Enqueue(node, -node.MaximumSum);
                        if (node.Name == "Z")
                        {
                            result = node;
                            queue.Clear();
                            break;
                        }
                    }
                }
            }
            return result!.Pressure.ToString(); // 2634 too low
        }
    }
}