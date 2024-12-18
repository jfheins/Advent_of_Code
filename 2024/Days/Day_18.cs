using Core;
using System.Drawing;

namespace AoC_2024.Days;

public sealed class Day_18 : BaseDay
{
    private readonly string[] _input;

    public Day_18()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var grid = new FiniteGrid2D<char>(71, 71, '.');
        for (int i = 0; i < 1024; i++)
        {
            var b = _input[i].ParseInts();
            grid[b[0], b[1]] = '#';
        }

        var d = new DijkstraSearch<Point>(null, Expander);
        var path = d.FindFirst(Point.Empty, it => it == grid.BottomRight);
        
        Console.WriteLine(grid);
        
        return path!.Length.ToString();
        
        IEnumerable<(Point node, float cost)> Expander(Point arg)
        {
            return grid.Get4NeighborsOf(arg).Where(n => grid[n] != '#').Select(n => (n, 1f));
        }
    }

    public override async ValueTask<string> Solve_2()
    {
        var grid = new FiniteGrid2D<char>(71, 71, '.');
        for (int i = 0; i < 1024; i++)
        {
            var b = _input[i].ParseInts();
            grid[b[0], b[1]] = '#';
        }

        var d = new DijkstraSearch<Point>(null, Expander);
        var path = d.FindFirst(Point.Empty, it => it == grid.BottomRight);
        var byteIdx = 1024;
        var lastAdded = "";
        while (path is not null)
        {
            AddByte(byteIdx++);
            path = d.FindFirst(Point.Empty, it => it == grid.BottomRight);
        }
        
        return lastAdded;
        
        void AddByte(int i)
        {
            var b = _input[i].ParseInts();
            lastAdded = _input[i];
            grid[b[0], b[1]] = '#';
        }
        
        IEnumerable<(Point node, float cost)> Expander(Point arg)
        {
            return grid.Get4NeighborsOf(arg).Where(n => grid[n] != '#').Select(n => (n, 1f));
        }
    }
}