using Core;
using System.Linq;
using System.Drawing;

namespace AoC_2025.Days;

public sealed partial class Day_11 : BaseDay
{
    private readonly (string node, string[] outputs)[] _input;

    public Day_11()
    {
        _input = File.ReadAllLines(InputFilePath).SelectArray(Parse);
    }

    private (string node, string[] outputs) Parse(string arg)
    {
        var x = arg.Split(':');
        return (x[0], x[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
    }

    public override async ValueTask<string> Solve_1()
    {
        var ex = _input.ToDictionary(it => it.node, it => it.outputs);
        var paths = Search("you");

        return paths.ToString();

        int Search(string node)
        {
            if (node == "out")
                return 1;

            return ex.GetValueOrDefault(node, []).Select(Search).Sum();
        }
    }

    public override async ValueTask<string> Solve_2()
    {
        var ex = _input.ToDictionary(it => it.node, it => it.outputs);

        // Dictionary to merge nodes at current level: (node, hasDac, hasFft) -> pathCount
        var currentLevel = new Dictionary<(string device, bool dac, bool fft), long>
        {
            [("svr", false, false)] = 1
        };

        long totalPaths = 0;

        while (currentLevel.Count > 0)
        {
            var nextLevel = new Dictionary<(string device, bool dac, bool fft), long>();

            foreach (var (state, pathCount) in currentLevel)
            {
                var (device, hasDac, hasFft) = state;
                if (device == "out" && hasDac && hasFft)
                {
                    Console.WriteLine($"Found path with count: {pathCount}");
                    totalPaths += pathCount;
                    continue;
                }

                foreach (var nextNode in ex.GetValueOrDefault(device, []))
                {
                    var newState = (
                        device: nextNode,
                        dac: hasDac || nextNode == "dac",
                        fft: hasFft || nextNode == "fft"
                    );

                    var existingCount = nextLevel.GetValueOrDefault(newState);
                    nextLevel[newState] = existingCount + pathCount;
                }
            }

            currentLevel = nextLevel;
        }

        return totalPaths.ToString(); // wrong: 1409877312320
    }
}

