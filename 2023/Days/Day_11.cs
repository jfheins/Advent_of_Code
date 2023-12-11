using Core;
using Core.Combinatorics;

using System.Diagnostics;
using System.Drawing;

namespace AoC_2023.Days;

public sealed partial class Day_11 : BaseDay
{
    private readonly string[] _input;
    private readonly FiniteGrid2D<char> _grid;

    public Day_11()
    {
        _input = File.ReadAllLines(InputFilePath);
        _grid = new FiniteGrid2D<char>(_input);
    }

    public override async ValueTask<string> Solve_1()
    {
        var s = _grid.ToString();

        // find empty rows
        var erows = _grid.AllRows('.').Where(t => t.cells.AllEqual('.')).OrderByDescending(t => t.y).SelectList(it => it.y);
        foreach (var row in erows)
        {
            _grid.InsertRow(row, '.');
        }
        s = _grid.ToString();

        var ecols = _grid.AllCols('.').Where(t => t.cells.AllEqual('.')).OrderByDescending(t => t.x).SelectList(it => it.x);
        foreach (var col in ecols)
        {
            _grid.InsertCol(col, '.');
        }
        s = _grid.ToString();

        var galaxies = _grid.Where(t => t.value == '#').SelectList(it => it.pos);
        var allDistances = new TupleCombinations2<Point>(galaxies).Select(t => t.Item1.ManhattanDistTo(t.Item2));

        return allDistances.Sum().ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        return "-";
    }
}