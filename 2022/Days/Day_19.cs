using Core;

using Spectre.Console;

using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

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

        private Blueprint ParseLine(int[] ints)
        {
            var x = ints.SelectArray(it => (short)it);
            var ore = Vector64.Create(new short[] { x[1], 0, 0, 0 });
            var clay = Vector64.Create(new short[] { x[2], 0, 0, 0 });
            var obs = Vector64.Create(new short[] { x[3], x[4], 0, 0 });
            var geode = Vector64.Create(new short[] { x[5], 0, x[6], 0 });
            return new Blueprint(x[0], ore, clay, obs, geode);
        }

        private record Blueprint(int bpId,
            Vector64<short> OreCost,
            Vector64<short> ClayCost,
            Vector64<short> ObsidianCost,
            Vector64<short> GeodeCost);

        [Flags]
        private enum BuyOptions : byte
        {
            Ore = 1,
            Clay = 2,
            Obs = 4,
            Geode = 8,
            All = 15
        }

        private record struct State(Vector64<short> Robots, Vector64<short> Ress, int RemTime, Blueprint Bp, BuyOptions buyOptions)
        {
            public int GeodeBots() => Robots[3];
            public override int GetHashCode()
            {
                var r = Vector64.Narrow(Ress, Vector64<short>.Zero).AsInt32().GetElement(0);
                return r ^ (Bp.bpId << 28) ^ ((int)buyOptions << 22);
            }
        }

        public override async ValueTask<string> Solve_1()
        {
            return _input.AsParallel().Sum(CalcQuality).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            return _input.Take(3).AsParallel().Select(bp => CalcGeodes(bp, 32)).Product().ToString();
        }

        private int CalcQuality(Blueprint blueprint)
        {
            return blueprint.bpId * CalcGeodes(blueprint, 24);
        }

        private int CalcGeodes(Blueprint blueprint, int time)
        {
            var states = new HashSet<State>();

            var initRobots = Vector64.Create(new short[4] { 1, 0, 0, 0 });
            var ress = Vector64.Create(new short[4] { 0, 0, 0, 0 });

            _ = states.Add(new State(initRobots, ress, time, blueprint, BuyOptions.Ore | BuyOptions.Clay | BuyOptions.Obs));

            for (int i = 0; i < time; i++)
            {
                var next = new List<State>(states.Count * 3);
                next.AddRange(states.SelectMany(Expand));
                var maxBots = next.Max(it => it.GeodeBots());
                _ = next.RemoveAll(it => it.GeodeBots() < maxBots - 1);
                states = next.ToHashSet();
            }
            var maxGeodes = states.Max(s => s.Ress[3]);
            return maxGeodes;
        }

        private readonly Vector64<short> One = Vector64.Create(new short[] { 1, 0, 0, 0 });
        private readonly Vector64<short> Two = Vector64.Create(new short[] { 0, 1, 0, 0 });
        private readonly Vector64<short> Three = Vector64.Create(new short[] { 0, 0, 1, 0 });
        private readonly Vector64<short> Four = Vector64.Create(new short[] { 0, 0, 0, 1 });

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
            var True = Vector64<short>.AllBitsSet;

            var canAffordGeode = Vector64.GreaterThanOrEqual(s.Ress, s.Bp.GeodeCost) == True;
            var canAffordObs = Vector64.GreaterThanOrEqual(s.Ress, s.Bp.ObsidianCost) == True;
            var canAffordClay = Vector64.GreaterThanOrEqual(s.Ress, s.Bp.ClayCost) == True;
            var canAffordOre = Vector64.GreaterThanOrEqual(s.Ress, s.Bp.OreCost) == True;

            var maxCosts = Vector64.Max(Vector64.Max(s.Bp.OreCost, s.Bp.ClayCost), Vector64.Max(s.Bp.ObsidianCost, s.Bp.GeodeCost));
            var canUse = Vector64.LessThan(s.Robots, maxCosts); // if robots = max cost, no use for added bots

            if (canAffordGeode && s.buyOptions.HasFlag(BuyOptions.Geode))
                yield return s with { Ress = nextRess - s.Bp.GeodeCost, Robots = s.Robots + Four, RemTime = s.RemTime - 1, buyOptions = BuyOptions.All };

            if (canAffordObs && canUse[2] != 0 && s.buyOptions.HasFlag(BuyOptions.Obs)) // Obs robot
                yield return s with { Ress = nextRess - s.Bp.ObsidianCost, Robots = s.Robots + Three, RemTime = s.RemTime - 1, buyOptions = BuyOptions.All };

            if (canAffordClay && canUse[1] != 0 && s.buyOptions.HasFlag(BuyOptions.Clay)) // Clay robot
                yield return s with { Ress = nextRess - s.Bp.ClayCost, Robots = s.Robots + Two, RemTime = s.RemTime - 1, buyOptions = BuyOptions.All };

            if (canAffordOre && canUse[0] != 0 && s.buyOptions.HasFlag(BuyOptions.Ore)) // Build ore robot
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

ref struct TinyList<T> where T : struct
{
    private readonly Span<T> _buffer;
    public int Count { get; private set; }

    public TinyList(Span<T> buffer)
    {
        _buffer = buffer;
        Count = 0;
    }

    public void Add(T value) => _buffer[Count++] = value;

    public readonly T Get(int index) => _buffer[index];

    public readonly Span<T> AsSpan() => _buffer[..Count];

    public readonly T this[int i] => _buffer[i];
}