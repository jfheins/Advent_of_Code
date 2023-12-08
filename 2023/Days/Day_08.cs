using AoC_2020.Days;

using Core;

using MoreLinq;

using System.Diagnostics;
using System.Drawing;

namespace AoC_2023.Days;

public sealed partial class Day_08 : BaseDay
{
    private readonly string[] _input;

    public Day_08()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var instr = _input[0];

        var nodes = _input[2..].ToDictionary(line => line[0..3], line => (line[7..10], line[12..15]));

        var currentNode = "AAA";
        var steps = 0;
        while (currentNode != "ZZZ")
        {
            var i = instr[steps.Modulo(instr.Length)];
            currentNode = i == 'L' ? nodes[currentNode].Item1 : nodes[currentNode].Item2;
            steps++;
        }

        return steps.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var instructions = _input[0];

        var nodes = _input[2..].ToDictionary(line => line[0..3], line => (line[7..10], line[12..15]));

        var loops = new Dictionary<string, (int start, long len)>();
        var loopCount = 0;

        var currentNodes = new List<string>(nodes.Keys.Where(n => n.EndsWith('A')));
        var steps = 0;
        while (true)
        {
            var instr = instructions[steps.Modulo(instructions.Length)];

            for (int i = 0; i < currentNodes.Count; i++)
            {
                var n = nodes[currentNodes[i]];
                currentNodes[i] = instr == 'L' ? n.Item1 : n.Item2;
            }
            steps++;

            foreach (var n in currentNodes)
            {
                if (n.EndsWith('Z'))
                {
                    if (loops.TryGetValue(n, out var l))
                    {
                        if (l.len == 0)
                        {
                            loops[n] = l with { len = steps - l.start };
                            loopCount++;
                        }
                    }
                    else
                    {
                        loops.Add(n, (steps, 0));
                    }
                }
            }
            if (loopCount == currentNodes.Count)
                break;
        }

        var maxStart = loops.Values.Max(t => t.start);
        var tuples = loops.Values.Select(it => (it.len, (int)(it.len - ((maxStart - it.start) % it.len)))).ToList();
        var coprimes = tuples.Select((x, i) => i == 0 ? x.len : x.len / 271).ToArray();
        var remain = tuples.SelectArray(x => x.Item2).ToArray();

        var remainder = ChineseRemainderTheorem.Solve(coprimes, remain);
        return (maxStart + remainder).ToString();
    }
}