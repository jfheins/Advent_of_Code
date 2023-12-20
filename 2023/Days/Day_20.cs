using Core;

namespace AoC_2023.Days;

public sealed class Day_20 : BaseDay
{
    private readonly Dictionary<string, Module> _modules;


    public Day_20()
    {
        _modules = File.ReadAllLines(InputFilePath).Select(ParseLine).ToDictionary(x => x.Name);
        _modules.Add("output", new Broadcast("output", Array.Empty<string>()));
        _modules.Add("rx", new Broadcast("rx", Array.Empty<string>()));
        foreach (var conj in _modules.Values.OfType<Conjunction>())
        {
            conj.InitSources(_modules.Values.Where(it => it.Dest.Contains(conj.Name)));
        }
    }

    private long[] _counts = new long[2];

    public override async ValueTask<string> Solve_1()
    {
        for (int i = 0; i < 1000; i++)
            PushTheButton();
        return _counts.Product().ToString();
    }

    private void PushTheButton()
    {
        var pulses = new Queue<(Module src, string dest, Pulse pulse)>();
        pulses.Enqueue((_modules["broadcaster"], "broadcaster", Pulse.Low));

        while (pulses.TryDequeue(out var pulse))
        {
            _counts[(int)pulse.pulse]++;
            //Console.WriteLine(ToStr(pulse));
            var module = _modules[pulse.dest];
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
                    Send(c.LastIn.Values.All(x => x == Pulse.High) ? Pulse.Low : Pulse.High);
                    break;
            }

            void Send(Pulse p)
            {
                foreach (var dest in module.Dest)
                    pulses.Enqueue((module, dest, p));
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
    }

    public override async ValueTask<string> Solve_2()
    {
        return "-";
    }
}