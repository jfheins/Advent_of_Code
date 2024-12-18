using Core;
using System.Drawing;

namespace AoC_2024.Days;

public sealed class Day_18 : BaseDay
{
    private readonly string[] _input;
    private const int Part1Cutoff = 1024;
    private readonly FiniteGrid2D<char> _grid = new(71, 71);
    private readonly BreadthFirstSearch<Point> _bfs;
    private Point[] _path = [];

    public Day_18()
    {
        _input = File.ReadAllLines(InputFilePath);
        _bfs = new BreadthFirstSearch<Point>(null, Walk) { PerformParallelSearch = false };
    }

    public override void Clear()
    {
        _grid.Clear();
    }

    public override async ValueTask<string> Solve_1()
    {
        for (var i = 0; i < Part1Cutoff; i++) 
            AddByte(i);

        return FindPath()!.Value.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        for (var i = Part1Cutoff; i < _input.Length; i++)
        {
            var lastAdded = AddByte(i);
            if (_path.Contains(lastAdded))
            {
                if (FindPath() == null)
                    return $"{lastAdded.X},{lastAdded.Y}";
            }
        }

        return "Something went wrong!";
    }

    private Point AddByte(int idx)
    {
        var p = _input[idx].ParseInts(2).ToPoint();
        _grid[p] = '#';
        return p;
    }

    private int? FindPath()
    {
        var p = _bfs.FindFirst(_grid.TopLeft, it => it == _grid.BottomRight);
        _path = p?.Steps ?? [];
        return p?.Length;
    }

    private IEnumerable<Point> Walk(Point arg)
    {
        return _grid.Get4NeighborsOf(arg).Where(CanWalk);
        bool CanWalk(Point p) => _grid.GetValueOrDefault(p, '.') == '.';
    }
}