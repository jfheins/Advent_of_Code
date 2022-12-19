using Core;

using Spectre.Console;

using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

using static MoreLinq.Extensions.ZipLongestExtension;

namespace AoC_2022.Days
{
    public sealed class Day_19 : BaseDay
    {
        private IReadOnlyList<Blueprint> _input;

        public Day_19()
        {
            var input = File.ReadAllLines(InputFilePath).SelectList(line => line.ParseInts(7));
            _input = input.SelectList(ParseLine);
        }

        private Blueprint ParseLine(int[] x)
        {
            var ore = new Vector<int>(new int[] { x[1], 0, 0, 0, 0, 0, 0, 0 });
            var clay = new Vector<int>(new int[] { x[2], 0, 0, 0, 0, 0, 0, 0 });
            var obs = new Vector<int>(new int[] { x[3], x[4], 0, 0, 0, 0, 0, 0 });
            var geode = new Vector<int>(new int[] { x[5], 0, x[6], 0, 0, 0, 0, 0 });
            return new Blueprint(x[0], ore, clay, obs, geode);
        }

        private record Blueprint(int bpId,
            Vector<int> OreCost,
            Vector<int> ClayCost,
            Vector<int> obsidianCost,
            Vector<int> geodeCost);

        [Flags]
        private enum BuyOptions : byte
        {
            Ore = 1,
            Clay = 2,
            Obs = 4,
            Geode = 8,
            All = 15
        }

        private record struct State(Vector<int> Robots, Vector<int> Ress, int RemTime, Blueprint Bp, BuyOptions buyOptions)
        {
            public int GeodeBots() => Robots[3];
        }

        public override async ValueTask<string> Solve_1()
        {
            return _input.AsParallel().Sum(CalcQuality).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            return _input.Take(3).Select(bp => CalcGeodes(bp, 32)).Product().ToString();
        }

        private int CalcQuality(Blueprint blueprint)
        {
            return blueprint.bpId * CalcGeodes(blueprint, 24);
        }

        private int CalcGeodes(Blueprint blueprint, int time)
        {
            var states = new HashSet<State>();

            var initRobots = new Vector<int>(new int[8] { 1, 0, 0, 0, 0, 0, 0, 0 });
            var ress = new Vector<int>(new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 });

            _ = states.Add(new State(initRobots, ress, time, blueprint, BuyOptions.Ore | BuyOptions.Clay | BuyOptions.Obs));

            for (int i = 0; i < time; i++)
            {
                var next = new List<State>(states.Count * 2);
                next.AddRange(states.SelectMany(Expand));
                var maxBots = next.Max(it => it.GeodeBots());
                _ = next.RemoveAll(it => it.GeodeBots() < maxBots - 1);
                states = next.ToHashSet();
                Console.WriteLine(i);
            }
            var maxGeodes = states.Max(s => s.Ress[3]);
            return maxGeodes;
        }

        private Vector<int> One = new Vector<int>(new int[] { 1, 0, 0, 0, 0, 0, 0, 0 });
        private Vector<int> Two = new Vector<int>(new int[] { 0, 1, 0, 0, 0, 0, 0, 0 });
        private Vector<int> Three = new Vector<int>(new int[] { 0, 0, 1, 0, 0, 0, 0, 0 });
        private Vector<int> Four = new Vector<int>(new int[] { 0, 0, 0, 1, 0, 0, 0, 0 });

        private IEnumerable<State> Expand(State s)
        {
            if (s.RemTime == 0)
            {
                yield return s;
                yield break;
            }

            var nextRess = s.Ress + s.Robots;

            if (s.RemTime == 1)
            {
                yield return s with { Ress = nextRess, RemTime = 0 };
                yield break;
            }

            var canAffordGeode = s.Ress[0] >= s.Bp.geodeCost[0] && s.Ress[2] >= s.Bp.geodeCost[2];
            var canAffordObs = s.Ress[0] >= s.Bp.obsidianCost[0] && s.Ress[1] >= s.Bp.obsidianCost[1];
            var canAffordClay = s.Ress[0] >= s.Bp.ClayCost[0];
            var canAffordOre = s.Ress[0] >= s.Bp.OreCost[0];

            var maxCosts = Vector.Max(Vector.Max(s.Bp.OreCost, s.Bp.ClayCost), Vector.Max(s.Bp.obsidianCost, s.Bp.geodeCost));
            var canUse = Vector.LessThan(s.Robots, maxCosts);

            if (canAffordGeode && s.buyOptions.HasFlag(BuyOptions.Geode))
                yield return s with { Ress = nextRess - s.Bp.geodeCost, Robots = s.Robots + Four, RemTime = s.RemTime - 1, buyOptions = BuyOptions.All };

            if (canAffordObs && s.buyOptions.HasFlag(BuyOptions.Obs) && canUse[2] != 0) // Obs robot
                yield return s with { Ress = nextRess - s.Bp.obsidianCost, Robots = s.Robots + Three, RemTime = s.RemTime - 1, buyOptions = BuyOptions.All };

            if (canAffordClay && s.buyOptions.HasFlag(BuyOptions.Clay) && canUse[1] != 0) // Clay robot
                yield return s with { Ress = nextRess - s.Bp.ClayCost, Robots = s.Robots + Two, RemTime = s.RemTime - 1, buyOptions = BuyOptions.All };

            if (canAffordOre && s.buyOptions.HasFlag(BuyOptions.Ore) && canUse[0] != 0) // Build ore robot
                yield return s with { Ress = nextRess - s.Bp.OreCost, Robots = s.Robots + One, RemTime = s.RemTime - 1, buyOptions = BuyOptions.All };

            var optionsAfterWait = MakeOption(!canAffordGeode, BuyOptions.Geode)
                | MakeOption(!canAffordObs, BuyOptions.Obs)
                | MakeOption(!canAffordClay, BuyOptions.Clay)
                | MakeOption(!canAffordOre, BuyOptions.Ore);
            yield return s with { Ress = nextRess, RemTime = s.RemTime - 1, buyOptions = optionsAfterWait }; // Wait

            static BuyOptions MakeOption(bool enable, BuyOptions o) => enable ? o : 0;
        }
    }
}