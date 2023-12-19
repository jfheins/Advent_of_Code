using System.Collections.Immutable;
using Core;
using static MoreLinq.Extensions.SplitExtension;

namespace AoC_2023.Days;

public sealed class Day_19 : BaseDay
{
    private readonly Dictionary<string, Workflow> _workflows;
    private readonly List<Part> _parts;

    public Day_19()
    {
        var (workflows, parts) = File.ReadAllLines(InputFilePath).Split("").ToTuple2();

        _workflows = workflows.Select(ParseWorkflow).ToDictionary(it => it.Name);
        _parts = parts.SelectList(ParsePart);
    }

    private static Part ParsePart(string arg)
    {
        var i = arg.ParseInts(4);
        return new Part(new Dictionary<char, int>()
        {
            { 'x', i[0] },
            { 'm', i[1] },
            { 'a', i[2] },
            { 's', i[3] },
        });
    }

    private static Workflow ParseWorkflow(string line)
    {
        var parts = line.Split(new[] { '{', ',', '}' }, StringSplitOptions.RemoveEmptyEntries);
        var result = new Workflow(parts[0]);
        foreach (var rule in parts.Skip(1))
        {
            if (rule.Contains(':'))
            {
                var (threshold, dest) = rule.Split(':').ToTuple2();
                var parsed = int.Parse(threshold[2..]);
                var interval = threshold[1] switch
                {
                    '<' => new Interval(int.MinValue, parsed),
                    '>' => new Interval(parsed + 1, int.MaxValue),
                    _ => throw new ArgumentOutOfRangeException()
                };
                result.Rules.Add(new Rule(threshold[0], interval, dest));
            }
            else
            {
                result.Rules.Add(new Rule('x', Interval.Whole, rule));
            }
        }

        return result;
    }

    private record Workflow(string Name)
    {
        public List<Rule> Rules { get; } = new();

        public string Process(Part part)
            => Rules.First(r => r.Matches(part)).Dest;
    }

    private record Rule(char Prop, Interval Range, string Dest)
    {
        public bool Matches(Part p) => Range.Contains(p.Props[Prop]);
    }

    private record Part(Dictionary<char, int> Props)
    {
        public long Rating => Props['x'] + Props['m'] + Props['a'] + Props['s'];
    }

    public override async ValueTask<string> Solve_1()
    {
        var result = _parts.Where(p => Process(p) == 'A').Sum(p => p.Rating);
        return result.ToString();
    }

    private char Process(Part part)
    {
        var next = "in";
        while (_workflows.TryGetValue(next, out var workflow))
            next = workflow.Process(part);
        return next[0];
    }

    public override async ValueTask<string> Solve_2()
    {
        var possibleRatings = Interval.FromInclusiveBounds(1, 4000);
        var all = ImmutableDictionary<char, Interval>.Empty
            .Add('x', possibleRatings)
            .Add('m', possibleRatings)
            .Add('a', possibleRatings)
            .Add('s', possibleRatings);

        return CountCombinations(all, "in").ToString();
    }

    private long CountCombinations(ImmutableDictionary<char, Interval> intervals, string name)
    {
        if (!_workflows.TryGetValue(name, out var wf))
            return name == "A" ? intervals.Values.Select(it => it.Length).Product() : 0;

        var combinations = 0L;
        foreach (var rule in wf.Rules)
        {
            var propInterval = intervals[rule.Prop];
            var (a, b, c) = propInterval.Intersect(rule.Range);
            if (b.HasValue)
                combinations += CountCombinations(intervals.SetItem(rule.Prop, b.Value), rule.Dest);
            intervals = intervals.SetItem(rule.Prop, a ?? c ?? propInterval);
        }

        return combinations;
    }
}