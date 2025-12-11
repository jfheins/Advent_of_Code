using Core;

namespace AoC_2025.Days;

public sealed class Day_11 : BaseDay
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
        var cache = new Dictionary<(string from, string to), long>();
        long part1;
        var part2 = Search("dac", "fft");
        long part3;

        if (part2 > 0)
        {
            part1 = Search("svr", "dac");
            part3 = Search("fft", "out");
            return (part1 * part2 * part3).ToString();
        }

        part1 = Search("svr", "fft");
        part2 = Search("fft", "dac");
        part3 = Search("dac", "out");

        return (part1 * part2 * part3).ToString();

        long Search(string node, string target)
        {
            if (node == target)
                return 1;

            return cache.GetOrAdd((node, target),
                _ => ex.GetValueOrDefault(node, []).Select(it => Search(it, target)).Sum());
        }
    }
}