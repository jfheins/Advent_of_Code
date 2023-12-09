using Core;

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
        return _input.Select(ExtrapolateRight).Sum().ToString();
    }

    public override async ValueTask<string> Solve_2()
    {

        return _input.Select(ExtrapolateLeft).Sum().ToString();
    }

    private static int ExtrapolateRight(int[] sequence)
    {
        var lastDigits = new Stack<int>();
        while (!sequence.All(x => x == 0))
        {
            lastDigits.Push(sequence.Last());
            sequence = sequence.Diff().ToArray();
        }
        return lastDigits.Sum();
    }

    private static int ExtrapolateLeft(int[] sequence)
    {
        var firstDigits = new Stack<int>();
        while (!sequence.All(x => x == 0))
        {
            firstDigits.Push(sequence.First());
            sequence = sequence.Diff().ToArray();
        }
        return firstDigits.Aggregate((acc, next) => next - acc);
    }
}