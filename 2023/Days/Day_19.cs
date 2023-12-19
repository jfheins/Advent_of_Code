using System.Collections.Immutable;
using Core;
using System.Drawing;
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

    record Workflow(string Name)
    {
        public List<Rule> Rules { get; } = new();

        public string Process(Part part)
            => Rules.First(r => r.Matches(part)).Dest;
    }

    record Rule(char Prop, Interval Range, string Dest)
    {
        public bool Matches(Part p) => Range.Contains(p.Props[Prop]);
    }

    record Part(Dictionary<char, int> Props)
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
        {
            next = workflow.Process(part);
        }
        return next[0];
    }

    public override async ValueTask<string> Solve_2()
    {
        var all = new Dictionary<char, Interval>
        {
            { 'x', Interval.FromInclusiveBounds(1, 4000) },
            { 'm', Interval.FromInclusiveBounds(1, 4000) },
            { 'a', Interval.FromInclusiveBounds(1, 4000) },
            { 's', Interval.FromInclusiveBounds(1, 4000) },
        };
        return CountCombinations(all.ToImmutableDictionary(), "in").ToString();
    }

    public long CountCombinations(ImmutableDictionary<char, Interval> intervals, string name)
    {
        if (!_workflows.TryGetValue(name, out var wf))
            return name == "A" ? intervals.Values.Select(it => it.Length).Product() : 0;
        
        var rules = wf.Rules;
        var combinations = 0L;
        foreach (var rule in rules)
        {
            if (rule.Range == Interval.Whole)
            {
                combinations += CountCombinations(intervals, rule.Dest);
            }
            else
            {
                var propInterval = intervals[rule.Prop];
                var (a, b, c) = propInterval.Intersect(rule.Range);
                if (b.HasValue)
                    combinations += CountCombinations(intervals.SetItem(rule.Prop, b.Value), rule.Dest);
                intervals = intervals.SetItem(rule.Prop, a ?? c ?? propInterval);
            }
        }
        return combinations;
    }
}