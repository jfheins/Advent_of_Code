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
        //while (currentNode != "ZZZ")
        //{
        //    var i = instr[steps.Modulo(instr.Length)];
        //    currentNode = i == 'L' ? nodes[currentNode].Item1 : nodes[currentNode].Item2;
        //    steps++;
        //}

        return steps.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var instructions = _input[0];

        var nodes = _input[2..].ToDictionary(line => line[0..3], line => (line[7..10], line[12..15]));

        var loopStart = new Dictionary<string, long>();
        var loopLen = new Dictionary<string, long>();

        var currentNodes = new List<string>(nodes.Keys.Where(n => n.EndsWith("A")));
        var steps = 0L;
        while (currentNodes.Any(it => !it.EndsWith("Z")))
        {
            var instr = instructions[(int)steps.Modulo(instructions.Length)];

            for (int i = 0; i < currentNodes.Count; i++)
            {
                var n = nodes[currentNodes[i]];
                currentNodes[i] = instr == 'L' ? n.Item1 : n.Item2;
            }

            foreach (var n in currentNodes.Where(it => it.EndsWith("Z")))
            {
                Console.WriteLine($"Node \t{n}\t reached final at \t{steps}");
                if (!loopLen.ContainsKey(n))
                    if (loopStart.TryGetValue(n, out var start))
                        loopLen[n] = steps - start;
                    else
                        loopStart[n] = steps;

            }
            if (loopLen.Count == 6)
                break;
            steps += 1;
        }



        var maxStart = loopStart.Values.Max();
        var xxx = loopStart.Keys.Select(k =>
        {
            var loops = (maxStart - loopStart[k]) / loopLen[k];
            var thisloopS = loopStart[k] + loops * loopLen[k];
            var done = maxStart - thisloopS;
            return (loopLen[k], loopLen[k] - done, start: loopStart[k]);
        }).ToList();
        var primes = xxx.Select((x, i) => i == 0 ? x.Item1 : x.Item1 / 271).ToArray();
        var remain = xxx.Select(x => (int)x.Item2).ToArray();

        var multi = ChineseRemainderTheorem.Solve(primes, remain);
        var res = maxStart + multi;

        foreach (var item in xxx)
        {
            var loopp = (res - item.start) % item.Item1;
            Console.WriteLine(item.Item1 + "  " + loopp);
        }

        return res.ToString(); // 13830919117338 too low
    }
}