using Core;
using Spectre.Console;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AoC_2024.Days;

public sealed partial class Day_10 : BaseDay
{
    private readonly string[] _input;

    public Day_10()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var grid = new FiniteGrid2D<char>(_input);

        var heads = grid.Where(it => it.value == '0').ToList();
        var score =0;
        foreach (var h in heads)
        {
            var bfs = new BreadthFirstSearch<Point>(null, p => grid.Get4NeighborsOf(p)
                .Where(n => grid[n] == grid[p]+1));
            var dest = bfs.FindAll(h.pos, p => grid[p] == '9').Select(it => it.Target).ToList();
            score += dest.Count;
        }
        
        return score.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var grid = new FiniteGrid2D<char>(_input);

        var bfs = new BreadthFirstSearch<Point>(null, p => grid.Get4NeighborsOf(p)
            .Where(n => grid[n] == grid[p]+1));
        
        var heads = grid.Where(it => it.value == '0').ToList();
        var rating = 0;
        foreach (var h in heads)
        {
            rating += FindAllPaths(h.pos, '1', grid);
        }
        
        return rating.ToString();
    }

    private int FindAllPaths(Point p, char needle, FiniteGrid2D<char> grid)
    {
        if (grid[p] == '9')
        {
            return 1;
        }

        return grid.Get4NeighborsOf(p).Where(n => grid[n] == needle).Sum(n => FindAllPaths(n, (char)(needle+1), grid));
    }
}