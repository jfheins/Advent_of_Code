using Core;

using System.Diagnostics;
using System.Drawing;

namespace AoC_2023.Days;

public sealed partial class Day_09 : BaseDay
{
    private readonly string[] _input;

    public Day_09()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var res = new List<int>();
        foreach (var line in _input)
        {
            var d = line.ParseInts();
            var lastDigit = new Stack<int>();
            while (!d.All(x => x == 0))
            {
                lastDigit.Push(d.Last());
                d = d.Diff().ToArray();
            }
            var newDigit = lastDigit.Sum();
            res.Add(newDigit);
        }
        return res.Sum().ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var res = new List<int>();
        foreach (var line in _input)
        {
            var d = line.ParseInts();
            var lastDigit = new Stack<int>();
            while (!d.All(x => x == 0))
            {
                lastDigit.Push(d.First());
                d = d.Diff().ToArray();
            }
            var newDigit = lastDigit.Aggregate((a, b) => b-a);
            res.Add(newDigit);
        }
        return res.Sum().ToString();
    }
}