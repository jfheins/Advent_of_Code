using Core;

using static MoreLinq.Extensions.SplitExtension;

using System.Diagnostics;
using System.Drawing;

namespace AoC_2023.Days;

public sealed partial class Day_13 : BaseDay
{
    private readonly string[] _input;

    public Day_13()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var grids = _input.Split("").SelectList(block => new FiniteGrid2D<char>(block));
        var res = 0L;
        foreach (var grid in grids)
        {
            res += TestVert(grid) + 100*TestHorz(grid);
        }
        return res.ToString();
    }

    private int TestVert(FiniteGrid2D<char> grid)
    {
        Console.WriteLine(grid.ToString());
        var c = grid.AllCols('.').PairwiseWithOverlap().Where(t => t.Item1.cells.Count(x => x == '#') == t.Item2.cells.Count(x => x == '#'))
            .Select(t => (t.Item1.x, t.Item2.x)).ToList();

        return c.Any() ? c.Max(TestCand) : 0;


        int TestCand((int, int) c)
        {
            var ismatch = true;
            var (l, r) = c;
            while(l >= grid.Bounds.Left && r < grid.Bounds.Right)
            {
                ismatch = ismatch && grid.GetCol(l, '.').SequenceEqual(grid.GetCol(r, '.'));
                l--;
                r++;
            }
            return ismatch ? c.Item1+1 : 0;
        }
    }

    private int TestHorz(FiniteGrid2D<char> grid)
    {
        //var c = grid.AllRows('.').Pairwise().Where(t => t.Item1.cells.Count(x => x == '#') == t.Item2.cells.Count(x => x == '#'))
        //    .Select(t => (t.Item1.y, t.Item2.y)).ToList();

        var cc = grid.AllRows('.').SelectList(x => (x.y, count: x.cells.Count(y => y == '#')));
        var c = cc.PairwiseWithOverlap().Where(x => x.Item1.count == x.Item2.count).SelectList(it => (it.Item1.y, it.Item2.y));

        return c.Count != 0 ? c.Max(TestCand) : 0;

        int TestCand((int, int) c)
        {
            var ismatch = true;
            var (t, b) = c;
            while (t >= grid.Bounds.Top && b < grid.Bounds.Bottom)
            {
                ismatch = ismatch && grid.GetRow(t, '.').SequenceEqual(grid.GetRow(b, '.'));
                t--;
                b++;
            }
            return ismatch ? c.Item1+1 : 0;
        }
    }

    public override async ValueTask<string> Solve_2()
    {
        var grids = _input.Split("").SelectList(block => new FiniteGrid2D<char>(block));
        var res = 0L;
        foreach (var grid in grids)
        {
            res += TestVert2(grid) + 100 * TestHorz2(grid);
        }
        return res.ToString();
    }

    private int TestVert2(FiniteGrid2D<char> grid)
    {
        Console.WriteLine(grid.ToString());
        var c = grid.AllCols('.').PairwiseWithOverlap()
            .Where(t => Math.Abs( t.Item1.cells.Count(x => x == '#') - t.Item2.cells.Count(x => x == '#')) < 2)
            .Select(t => (t.Item1.x, t.Item2.x)).ToList();

        return c.Any() ? c.Max(TestCand) : 0;


        int TestCand((int, int) c)
        {
            var diff = 0;
            var (l, r) = c;
            while (l >= grid.Bounds.Left && r < grid.Bounds.Right)
            {
                diff += CountDiff(grid.GetCol(l, '.'), grid.GetCol(r, '.'));
                l--;
                r++;
            }
            return diff == 1 ? c.Item1 + 1 : 0;
        }
    }

    private int CountDiff(IEnumerable<char> a, IEnumerable<char> b)
    {
        return a.Zip(b).Count(t => t.First != t.Second);
    }

    private int TestHorz2(FiniteGrid2D<char> grid)
    {
        //var c = grid.AllRows('.').Pairwise().Where(t => t.Item1.cells.Count(x => x == '#') == t.Item2.cells.Count(x => x == '#'))
        //    .Select(t => (t.Item1.y, t.Item2.y)).ToList();

        var cc = grid.AllRows('.').SelectList(x => (x.y, count: x.cells.Count(y => y == '#')));
        var c = cc.PairwiseWithOverlap()
            .Where(t => Math.Abs(t.Item1.count - t.Item2.count) < 2)
            .SelectList(it => (it.Item1.y, it.Item2.y));

        return c.Count != 0 ? c.Max(TestCand) : 0;

        int TestCand((int, int) c)
        {
            var diff = 0;
            var (t, b) = c;
            while (t >= grid.Bounds.Top && b < grid.Bounds.Bottom)
            {
                diff += CountDiff(grid.GetRow(t, '.'), grid.GetRow(b, '.'));
                t--;
                b++;
            }
            return diff == 1 ? c.Item1 + 1 : 0;
        }
    }
}