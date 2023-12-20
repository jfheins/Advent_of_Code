using Core;

namespace AoC_2023.Days;

public sealed class Day_20 : BaseDay
{
    private readonly string[] _input;

    public Day_20()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    private long[] _counts = new long[2];
    private readonly Queue<(Module src, string dest, Pulse pulse)> _pulses = new(20);

    public override async ValueTask<string> Solve_1()
    {
        var modules = BuildModules();
        for (var i = 0; i < 1000; i++)
            PushTheButton(modules);
        return _counts.Product().ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var modules = BuildModules();
        var preSink = modules.Values.Single(it => it.Dest.Contains("rx"));
        var inputs = modules.Values.Where(it => it.Dest.Contains(preSink.Name)).Cast<Conjunction>().ToList();
        var loops = inputs.SelectList(_ => new LoopDetector(4100) { MinLoopCheckSize = 3000 });

        var buttonPresses = 0;
        while (loops.Any(it => !it.HasLoop))
        {
            PushTheButton(modules);
            buttonPresses++;
            for (var i = 0; i < loops.Count; i++)
            {
                if (!loops[i].HasLoop)
                    loops[i].Feed(buttonPresses, inputs[i].SentHigh ? 1 : 2);
            }
        }

        var cycles = loops.SelectList(it => (long)it.LoopSize);
        return cycles.LowestCommonMulti().ToString();
    }

    private Dictionary<string, Module> BuildModules()
    {
        var modules = _input.Select(ParseLine).ToDictionary(x => x.Name);
        modules.Add("output", new Broadcast("output", Array.Empty<string>()));
        modules.Add("rx", new Broadcast("rx", Array.Empty<string>()));
        foreach (var conj in modules.Values.OfType<Conjunction>())
        {
            conj.InitSources(modules.Values.Where(it => it.Dest.Contains(conj.Name)));
        }

        return modules;
    }

    private void PushTheButton(IReadOnlyDictionary<string, Module> modules)
    {
        foreach (var c in modules.Values.OfType<Conjunction>()) c.Reset();
        _pulses.Enqueue((modules["broadcaster"], "broadcaster", Pulse.Low));

        while (_pulses.TryDequeue(out var pulse))
        {
            _counts[(int)pulse.pulse]++;
            //Console.WriteLine(ToStr(pulse));
            var module = modules[pulse.dest];
            switch (module)
            {
                case Broadcast:
                    Send(Pulse.Low);
                    break;
                case FlipFlop ff:
                {
                    if (pulse.pulse == Pulse.Low)
                        Send(ff.Toggle());
                    break;
                }
                case Conjunction c:
                    c.LastIn[pulse.src] = pulse.pulse;
                    Send(c.CalcOut());
                    break;
            }

            void Send(Pulse p)
            {
                foreach (var dest in module.Dest)
                    _pulses.Enqueue((module, dest, p));
            }
        }

        // string ToStr((Module src, string dest, Pulse pulse) t) => $"{t.src.Name} ({t.pulse})=> {t.dest}";
    }

    private static Module ParseLine(string line)
    {
        var (name, dest) =
            line.Split(new[] { '-', '>' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .ToTuple2();
        var destArr = dest.Split(',', StringSplitOptions.TrimEntries);
        return line[0] switch
        {
            'b' => new Broadcast(name, destArr),
            '%' => new FlipFlop(name[1..], destArr),
            '&' => new Conjunction(name[1..], destArr),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    enum Pulse
    {
        Low,
        High
    }

    abstract record Module(string Name, string[] Dest);

    record Broadcast(string Name, string[] Dest) : Module(Name, Dest);

    record FlipFlop(string Name, string[] Dest) : Module(Name, Dest)
    {
        public bool Active { get; private set; }

        public Pulse Toggle() => (Active = !Active) == false ? Pulse.Low : Pulse.High;
    }

    record Conjunction(string Name, string[] Dest) : Module(Name, Dest)
    {
        public Dictionary<Module, Pulse> LastIn { get; } = new(new ByNameComparer());

        public bool SentHigh;

        public void Reset() => SentHigh = false;

        private class ByNameComparer : IEqualityComparer<Module>
        {
            public bool Equals(Module? x, Module? y)
            {
                if (ReferenceEquals(x, y)) return true;
                return x?.Name == y?.Name;
            }

            public int GetHashCode(Module obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        public void InitSources(IEnumerable<Module> sources)
        {
            foreach (var src in sources) LastIn.Add(src, Pulse.Low);
        }

        public Pulse CalcOut()
        {
            var result = LastIn.Values.All(x => x == Pulse.High) ? Pulse.Low : Pulse.High;
            SentHigh |= result == Pulse.High;
            return result;
        }
    }
}