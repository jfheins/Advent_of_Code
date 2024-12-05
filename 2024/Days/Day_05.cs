using Core;
using Spectre.Console;
using System.Drawing;
using System.Runtime.CompilerServices;
using static MoreLinq.Extensions.SplitExtension;

namespace AoC_2024.Days;

public sealed partial class Day_05 : BaseDay
{
    private readonly string[] _input;

    public Day_05()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var parts = _input.Split("").ToArray();
        var rules = parts[0].Select(x => x.ParseInts(2)).ToArray();
        var updates = parts[1].Select(x => x.ParseInts()).ToArray();

        var sum = 0;
        foreach(var u in updates)
        {
            if (inOrder(u, rules))
                sum += u.CenterItem();
        }

        return sum.ToString();
    }

    private bool inOrder(int[] u, int[][] rules)
    {
        return rules.All(r =>
        {
            var idx = Array.IndexOf(u, r[0]);
            if (idx == -1)
                return true;
            else
            {
                var si = Array.IndexOf(u, r[1]);
                return si == -1 ? true : idx < si;
            }
        });
    }

    public override async ValueTask<string> Solve_2()
    {
        var parts = _input.Split("").ToArray();
        var rules = parts[0].Select(x => x.ParseInts(2)).ToArray();
        var updates = parts[1].Select(x => x.ParseInts()).ToArray();

        var sum = 0;
        foreach (var u in updates.ExceptWhere(u => inOrder(u, rules)))
        {
            var corrected = Fix(u, rules);
                sum += corrected.CenterItem();
        }

        return sum.ToString();
    }

    private int[] Fix(int[] u, int[][] rules)
    {

        do
        {
            var brokenRule = rules.FirstOrDefault(r =>
            {
                var left = Array.IndexOf(u, r[0]);
                var right = Array.IndexOf(u, r[1]);
                return left != -1 && right != -1 && left > right;
            });
            if (brokenRule != null)
            {

                var leftIdx = Array.IndexOf(u, brokenRule[0]);
                var rightIdx = Array.IndexOf(u, brokenRule[1]);
                (u[leftIdx], u[rightIdx]) = (u[rightIdx], u[leftIdx]);
            }
            else break;

        } while (true);
        return u;
    }
}