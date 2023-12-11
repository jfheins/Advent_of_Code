using Core;
using Core.Combinatorics;

using System.Drawing;

namespace AoC_2023.Days;

public sealed partial class Day_11 : BaseDay
{
    private readonly string[] _input;

    public Day_11()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var grid = new FiniteGrid2D<char>(_input);
        var erows = grid.AllRows('.').Where(t => t.cells.AllEqual('.')).OrderByDescending(t => t.y).SelectList(it => it.y);
        foreach (var row in erows)
        {
            grid.InsertRow(row, '.');
        }

        var ecols = grid.AllCols('.').Where(t => t.cells.AllEqual('.')).OrderByDescending(t => t.x).SelectList(it => it.x);
        foreach (var col in ecols)
        {
            grid.InsertCol(col, '.');
        }

        var galaxies = grid.Where(t => t.value == '#').SelectList(it => it.pos);
        var allDistances = new TupleCombinations2<Point>(galaxies).Select(t => t.Item1.ManhattanDistTo(t.Item2));
        return allDistances.Sum().ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var grid = new FiniteGrid2D<char>(_input);
        grid.RemoveAll('.');
        var erows = grid.AllRows('.').Where(t => t.cells.AllEqual('.')).OrderByDescending(t => t.y).SelectList(it => it.y);
        foreach (var row in erows)
        {
            grid.InsertEmptyRows(row, 999_999);
        }

        var ecols = grid.AllCols('.').Where(t => t.cells.AllEqual('.')).OrderByDescending(t => t.x).SelectList(it => it.x);
        foreach (var col in ecols)
        {
            grid.InsertEmptyCols(col, 999_999);
        }

        var galaxies = grid.Where(t => t.value == '#').SelectList(it => it.pos);
        var allDistances = new TupleCombinations2<Point>(galaxies).Select(t => t.Item1.ManhattanDistTo(t.Item2));
        return allDistances.Sum(it => (long)it).ToString();
    }
}