using Core;
using Spectre.Console;
using System.Drawing;

namespace AoC_2024.Days;

public sealed partial class Day_02 : BaseDay
{
    private readonly int[][] _input;

    public Day_02()
    {
        _input = File.ReadAllLines(InputFilePath).SelectArray(x => x.ParseInts());
    }

    public override async ValueTask<string> Solve_1()
    {
        // var grid = new FiniteGrid2D<char>(_input);
        var safeCount = _input.Count(IsSafe);
        return safeCount.ToString();
    }

    bool IsSafe(int[] s)
        => (s.Diff().All(x => x > 0)
        || s.Diff().All(x => x < 0))
        && s.Diff().All(x => Math.Abs(x) <= 3);

    public override async ValueTask<string> Solve_2()
    {
        var sc = 0;
        foreach (var s in _input)
        {
            if (IsSafe(s))
                sc++;
            else
            {
                for (var i = 0; i < s.Length; i++)
                {
                    int[] alter = [..s[0..i], ..s[(i+1)..]];
                    if(IsSafe(alter))
                    {
                        sc++;
                        break;
                    }
                }
            }
        }

        return sc.ToString();
    }
}