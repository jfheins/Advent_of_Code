using Core;

namespace AoC_2025.Days;

public sealed class Day_11 : BaseDay
{
    private readonly Dictionary<(string from, string to), long> _cache = new(1200);
    private readonly Dictionary<string, string[]> _graph;

    public Day_11()
    {
        var input = File.ReadAllLines(InputFilePath).SelectArray(Parse2);
        _graph = input.ToDictionary(it => it[0], it => it[1..]);
        
        string[] Parse2(string it)
            => it.Split([':', ' '], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    }

    public override async ValueTask<string> Solve_1()
        => Search("you", "out").ToString();

    public override async ValueTask<string> Solve_2()
    {
        var passDacFirst = Search("dac", "fft") > 0;

        return passDacFirst
            ? Search("svr", "dac", "fft", "out").ToString()
            : Search("svr", "fft", "dac", "out").ToString();
    }

    private long Search(params IEnumerable<string> nodes)
        => nodes.PairwiseWithOverlap().Select2(Search).Product();

    private long Search(string node, string target)
    {
        if (node == target)
            return 1;

        return _cache.GetOrAdd((node, target),
            _ => _graph.GetValueOrDefault(node, []).Select(it => Search(it, target)).Sum());
    }
}