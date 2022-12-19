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

        private record State(Vector<int> Robots, Vector<int> Ress, int RemTime, Blueprint Bp, bool[] buyOptions)
        {
            public int GeodBots() => Robots[3];
        }

        public override async ValueTask<string> Solve_1()
        {
            return _input.AsParallel().Sum(CalcQuality).ToString();
        }

        private int CalcQuality(Blueprint blueprint)
        {
            var states = new HashSet<State>();

            var initRobots = new Vector<int>(new int[8] { 1, 0, 0, 0, 0, 0, 0, 0 });
            var ress = new Vector<int>(new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 });

            _ = states.Add(new State(initRobots, ress, 24, blueprint, new bool[] { true, true, true, false }));

            for (int i = 0; i < 24; i++)
            {
                states = states.SelectMany(Expand).ToHashSet();
            }
            var max = states.Max(s => s.Ress[3]);
            return blueprint.bpId * max;
        }

        private Vector<int> One = new Vector<int>(new int[] { 1, 0, 0, 0, 0, 0, 0, 0 });
        private Vector<int> Two = new Vector<int>(new int[] { 0, 1, 0, 0, 0, 0, 0, 0 });
        private Vector<int> Three = new Vector<int>(new int[] { 0, 0, 1, 0, 0, 0, 0, 0 });
        private Vector<int> Four = new Vector<int>(new int[] { 0, 0, 0, 1, 0, 0, 0, 0 });
        private bool[] All = new bool[4] { true, true, true, true };

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


            if (canAffordGeode && s.buyOptions[3])
                yield return s with { Ress = nextRess - s.Bp.geodeCost, Robots = s.Robots + Four, RemTime = s.RemTime - 1, buyOptions = All };

            if (canAffordObs && s.buyOptions[2]) // Obs robot
                yield return s with { Ress = nextRess - s.Bp.obsidianCost, Robots = s.Robots + Three, RemTime = s.RemTime - 1, buyOptions = All };

            if (canAffordClay && s.buyOptions[1]) // Clay robot
                yield return s with { Ress = nextRess - s.Bp.ClayCost, Robots = s.Robots + Two, RemTime = s.RemTime - 1, buyOptions = All };

            if (canAffordOre && s.buyOptions[0]) // Build ore robot
                yield return s with { Ress = nextRess - s.Bp.OreCost, Robots = s.Robots + One, RemTime = s.RemTime - 1, buyOptions = All };


            yield return s with { Ress = nextRess, RemTime = s.RemTime - 1, buyOptions = new bool[] { !canAffordOre, !canAffordClay, !canAffordObs, !canAffordGeode } }; // Wait

        }

        public override async ValueTask<string> Solve_2()
        {
            return "00";
        }
    }
}