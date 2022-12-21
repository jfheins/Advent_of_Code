using Core;

using System.Collections.Specialized;

namespace AoC_2022.Days
{
    public sealed class Day_16 : BaseDay
    {
        private static readonly List<Valve> _valves = new();
        private readonly int StartPoint;

        public Day_16()
        {
            var allValves = File.ReadAllLines(InputFilePath).SelectList(ParseLine);
            var neighbors = allValves.ToDictionary(v => v.name, v => v.neighbors);
            var rates = allValves.ToDictionary(v => v.name, v => v.rate);
            var relevantValves = allValves.Where(v => v.rate > 0)
                .Select(v => v.name)
                .Append("AA")
                .Select(name => new Valve(name, rates[name]))
                .ToDictionary(x => x.Name);

            foreach (var valve in relevantValves.Values.OrderBy(it => it.Name))
                _valves.Add(valve);

            StartPoint = _valves.IndexWhere(v => v.Name == "AA").First();

            var bfs = new BreadthFirstSearch<string>(null, it => neighbors[it]) { PerformParallelSearch = false };
            foreach (var valve in relevantValves.Values)
            {
                var paths = bfs.FindAll(valve.Name, x => x != valve.Name && x != "AA" && relevantValves.ContainsKey(x));

                foreach (var path in paths)
                {
                    var otherIdx = _valves.IndexOf(relevantValves[path.Target]);
                    valve.Distances.Add((otherIdx, path.Length));
                }
            }
        }

        public override async ValueTask<string> Solve_1()
        {
            var bfs = new BreadthFirstSearch<State>(null, Expander) { PerformParallelSearch = false };
            var timeIsUp = bfs.FindAll(new State(StartPoint, new BitVector32(), 0, 0, 30), n => n.RemainingTime == 0);

            return timeIsUp.Max(path => path.Target.Pressure).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var bfs = new BreadthFirstSearch<State>(null, Expander) { PerformParallelSearch = false };
            var myValves = bfs.FindAll(new State(StartPoint, new BitVector32(), 0, 0, 26), n => n.RemainingTime == 0).MaxBy(it => it.Target.Pressure)!.Target;
            var eleValves = bfs.FindAll(new State(StartPoint, myValves.Open, 0, 0, 26), n => n.RemainingTime == 0).MaxBy(it => it.Target.Pressure)!.Target;

            var totalPressure = myValves.Pressure + eleValves.Pressure;

            return totalPressure.ToString();
        }

        private record Valve(string Name, int Rate)
        {
            public List<(int valveIdx, int distance)> Distances { get; } = new();
            public int Key => Name[0] << 16 | Name[1];
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

        private record State(int Pos, BitVector32 Open, int Pressure, int Rate, int RemainingTime);

        private IEnumerable<State> Expander(State s)
        {
            if (s.RemainingTime == 0)
                yield break;

            if (s.Open.PopCount() > 5) // Just wait until time is up
            {
                var finalPres = s.Pressure + s.Rate * s.RemainingTime;
                yield return s with { RemainingTime = 0, Pressure = finalPres };
            }

            foreach (var (target, distance) in _valves[s.Pos].Distances)
            {
                var deltaTime = distance + 1;
                var isOpen = s.Open[1 << target];
                if (!isOpen && deltaTime <= s.RemainingTime)
                {
                    // Move to target and open the valve there
                    var futurePressure = s.Pressure + s.Rate * deltaTime;
                    var nowOpen = new BitVector32(s.Open);
                    nowOpen[1 << target] = true;
                    var futureRate = s.Rate + _valves[target].Rate;
                    yield return new State(target, nowOpen, futurePressure, futureRate, s.RemainingTime - deltaTime);
                }
            }
        }
    }
}