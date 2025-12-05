using Core;
using System.Linq;
using System.Drawing;

namespace AoC_2025.Days;

public sealed partial class Day_04 : BaseDay
{
    private readonly string[] _input;

    public Day_04()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var movable = 0;
        var grid = new FiniteGrid2D<char>(_input);
        foreach (var cell in grid)
        {
            var neighbors = grid.Get8NeighborsOf(cell.pos);
            if (cell.value == '@' && neighbors.Count(p => grid[p] == '@') < 4)
                movable++;
        }
        return movable.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var removed = 0;
        var grid = new FiniteGrid2D<char>(_input);

        var removable = GetRemovable();
        while (removable.Count > 0)
        {
            Console.WriteLine("Removing " + removable.Count + " cells, total: " + removed);
            removed += removable.Count;
            foreach (var point in removable)
            {
                grid[point] = '.';
            }

            removable = GetRemovable();
        }
        
        
        return removed.ToString();

        IReadOnlyCollection<Point> GetRemovable()
        {
            var movable = from cell in grid
                where cell.value == '@'
                let neighbors = grid.Get8NeighborsOf(cell.pos)
                where neighbors.Count(p => grid[p] == '@') < 4
                select cell.pos;
            return movable.ToList();
        }
    }
}