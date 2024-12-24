using Core;
using System.Linq;
using System.Drawing;

namespace AoC_2024.Days;

public sealed partial class Day_24 : BaseDay
{
    private readonly string[] _input;

    public Day_24()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var par = _input.SplitBy("");

        var known = par[0].Select(line => line.Split(":", StringSplitOptions.TrimEntries))
            .ToDictionary(t => t[0], t => int.Parse(t[1]));

        var todo = par[1].Select(ParseGate).ToList();

        while (todo.Any())
        {
            var next = todo.First(it => known.ContainsKey(it.left) && known.ContainsKey(it.right));
            todo.Remove(next);
            (string var, int value) res = Calculate(next, known);
            known.Add(res.var, res.value);
        }
// 0011111101000
        var z = known.Where(it => it.Key.StartsWith('z')).OrderBy(it => it.Key).ToArray();

        return known.Where(it => it.Key.StartsWith('z')).OrderByDescending(it => it.Key)
            .Aggregate(0L, (a, b) => (a << 1) | (b.Value & 1L)).ToString();
    }

    private (string var, int value) Calculate(
        (string left, string op, string right, string dest) x,
        Dictionary<string, int> known)
    {
        return x.op switch
        {
            "XOR" => (x.dest, Left() ^ Right()),
            "OR" => (x.dest, Left() | Right()),
            "AND" => (x.dest, Left() & Right()),
            _ => throw new ArgumentOutOfRangeException()
        };

        int Left() => known[x.left];
        int Right() => known[x.right];
    }

    private (string left, string op, string right, string dest) ParseGate(string x)
    {
        var s = x.Split("->", StringSplitOptions.TrimEntries);
        var result = s[1];
        var operands = s[0].Split(" ");
        return (operands[0], operands[1], operands[2], result);
    }

    public override async ValueTask<string> Solve_2()
    {
        return "-";
    }
}