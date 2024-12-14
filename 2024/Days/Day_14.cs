using System.Drawing;
using Core;

namespace AoC_2024.Days;

public sealed class Day_14 : BaseDay
{
    private readonly string[] _input;

    public Day_14()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    record Rob(Point Pos, Size V)
    {
        public Point Pos { get; set; } = Pos;
    }

    public override async ValueTask<string> Solve_1()
    {
        var grid = new FiniteGrid2D<char>(101, 103, '.');
        var robots = _input
            .Select(line => line.ParseInts(4))
            .Select(z => new Rob(new Point(z[0], z[1]), new Size(z[2], z[3])))
            .ToList();

        for (int i = 0; i < 100; i++)
        {
            foreach (var r in robots)
            {
                var x = (r.Pos.X + r.V.Width).Modulo(grid.Width);
                var y = (r.Pos.Y + r.V.Height).Modulo(grid.Height);
                r.Pos = new Point(x, y);
            }
        }

        var divided = robots.GroupBy(r => Quadrant(r, grid.Width, grid.Height));
        var res = divided.ExceptWhere(it => it.Key == 0).Select(group => group.Count()).Product();

        return res.ToString();
    }

    private static int Quadrant(Rob rob, int gridWidth, int gridHeight)
    {
        var vCenter = gridWidth / 2;
        var hCenter = gridHeight / 2;
        return (rob.Pos.X.CompareTo(vCenter), rob.Pos.Y.CompareTo(hCenter)) switch
        {
            (1, 1) => 1,
            (-1, 1) => 2,
            (-1, -1) => 3,
            (1, -1) => 4,
            _ => 0
        };
    }

    public override async ValueTask<string> Solve_2()
    {
        var grid = new FiniteGrid2D<char>(101, 103, '.');
        var robots = _input
            .Select(line => line.ParseInts(4))
            .Select(z => new Rob(new Point(z[0], z[1]), new Size(z[2], z[3])))
            .ToList();

        var iter = 0;
        for (var i = 0; i < 10000; i++)
        {
            foreach (var r in robots)
            {
                var x = (r.Pos.X + r.V.Width).Modulo(grid.Width);
                var y = (r.Pos.Y + r.V.Height).Modulo(grid.Height);
                r.Pos = new Point(x, y);
            }

            if (CouldBeTree(robots))
            {
                var g = new FiniteGrid2D<char>(grid);
                foreach (var r in robots)
                {
                    g[r.Pos] = '#';
                }

                Console.Write(g);
                iter = i + 1;
                Console.WriteLine(iter);
            }
        }

        return iter.ToString();
    }

    private static bool CouldBeTree(List<Rob> robots)
    {
        var pos = robots.Select(r => r.Pos).ToHashSet();
        // Searches for a pattern where a robot has 3 robots below
        // ..........#......
        // .........###.....
        // As this pattern is common in a xmas tree
        var treePoints = pos.Count(p => 
            pos.Contains(p + new Size(1, 1)) 
            && pos.Contains(p + new Size(0, 1)) 
            && pos.Contains(p + new Size(-1, 1)));
        return treePoints > 180;
    }
}