using Core;

using NoAlloq;

using System.Diagnostics;
using System.Drawing;

namespace AoC_2023.Days;

public sealed partial class Day_09 : BaseDay
{
    private readonly int[][] _input;

    public Day_09()
    {
        _input = File.ReadAllLines(InputFilePath).SelectArray(line => line.ParseInts());
    }

    public override async ValueTask<string> Solve_1()
    {
        return _input.SelectArray(ExtrapolateRight).Sum().ToString();
    }

    public override async ValueTask<string> Solve_2()
    {

        return _input.SelectArray(ExtrapolateLeft).Sum().ToString();
    }

    private static int ExtrapolateRight(int[] sequence)
    {
        Span<int> seq = stackalloc int[sequence.Length];
        sequence.CopyTo(seq);
        Span<int> diff = seq;
        while (diff.ContainsAnyExcept(0))
        {
            for (var i = 0; i < diff.Length - 1; i++)
                diff[i] = diff[i + 1] - diff[i];
            diff = diff[..^1];
        }
        return seq.Sum();
    }

    private static int ExtrapolateLeft(int[] sequence)
    {
        Span<int> seq = stackalloc int[sequence.Length];
        sequence.CopyTo(seq);
        Span<int> diff = seq;
        while (diff.ContainsAnyExcept(0))
        {
            for (int i = diff.Length - 1; i >= 1; i--)
                diff[i] = diff[i] - diff[i - 1];
            diff = diff[1..];
        }

        for (var i = 1; i < seq.Length; i += 2)
            seq[i] = -seq[i];
        return seq.Sum();
    }
}