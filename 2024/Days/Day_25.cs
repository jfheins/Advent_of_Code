using System.Diagnostics;
using Core;
using static MoreLinq.Extensions.TransposeExtension;

namespace AoC_2024.Days;

public sealed class Day_25 : BaseDay
{
    private readonly string[] _input;

    public Day_25()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        // var grid = new FiniteGrid2D<char>(_input);
        var grids = _input.SplitBy("");

        var keys = new List<int[]>();
        var locks = new List<int[]>();

        foreach (var grid in grids)
        {
            if (grid[0].All(x => x == '#')) // lock
            {
                var t = grid.Transpose().Select(col => col.Count('#'));
                locks.Add(t.ToArray());
            }
            else
            {
                Debug.Assert(grid.Last().All(x => x == '#'));
                var t = grid.Transpose().Select(col => col.Count('#'));
                keys.Add(t.ToArray());
            }
        }

        int fit = 0;
        foreach (var l in locks)
        {
            fit += keys.Count(k => KindaFits(k, l));
        }
        
        return fit.ToString();
        
        bool KindaFits(int[] key, int[] l)
        {
            Debug.Assert(key.Length == 5);
            Debug.Assert(l.Length == 5);
            return key.Zip(l).All(t => t.First + t.Second <= 7);
        }
    }

    public override async ValueTask<string> Solve_2()
    {
        return "-";
    }
}