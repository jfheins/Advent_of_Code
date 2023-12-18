using Core;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AoC_2023.Days;

public sealed partial class Day_18 : BaseDay
{
    private readonly string[] _input;

    public Day_18()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var ins = _input.SelectArray(ParseLine);
        var grid = new FiniteGrid2D<char>(0, 0, '.');

        var worker = Point.Empty;
        grid[worker] = '#';
        foreach (var (d, dist) in ins)
        {
            for (var i = 1; i <= dist; i++)
            {
                grid[worker.MoveTo(d, i)] = '#';
            }
            worker = worker.MoveTo(d, dist);
        }
        grid.SizeToFit();
        Console.WriteLine(grid.ToString(' '));

        var fill = new BreadthFirstSearch<Point>(null, FloodFill);
        var inner = fill.FindReachable(new Point(1, 1), null);

        return (inner.Count + grid.Count(it => it.value == '#')).ToString();

        IEnumerable<Point> FloodFill(Point point)
            => grid.Get4NeighborsOf(point).Where(p => grid.GetValueOrDefault(p, '.') != '#');
    }

    private (Direction d, int dist) ParseLine(string l)
    {
        var split = l.Split(' ');
        var d = split[0][0] switch
        {
            'L' => Direction.Left,
            'U' => Direction.Up,
            'R' => Direction.Right,
            'D' => Direction.Down,
        };
        return (d, int.Parse(split[1]));
    }

    public override async ValueTask<string> Solve_2()
    {
        var ins = _input.SelectArray(ParseLine2);
        var grid = new FiniteGrid2D<char>(0, 0, '.');

        var path = new List<Point>();
        var cursor = Point.Empty;
        var outside = Direction.Up;
        var last = (Direction)11;
        var area = 0L;
        foreach (var (d, dist) in ins)
        {
            var edgeDist = dist;
            // On convex corner, add one to last move
            // on concave, deduct current move
            if (d == outside.Opposite())
            {
                cursor = cursor.MoveTo(last, 1);
                path.Add(cursor);
            }
            else if (d == outside)
            {
                edgeDist--;
            }
            cursor = cursor.MoveTo(d, edgeDist);
            path.Add(cursor);
            if (last == (Direction)11)
                outside = Direction.Down;
            else
                outside = (Direction) ((int)outside + (int)d - (int)last).Modulo(4);
            last = d;
        }

        foreach (var p in path.PairwiseWithOverlap())
        {
            if (p.Item1.Y == p.Item2.Y)
            {
                long dx = p.Item1.X - p.Item2.X;
                area += checked(dx * p.Item2.Y);
            }
        }

        return area.ToString(); // 122109642109349 too low
        //                         122109860712709

        IEnumerable<Point> FloodFill(Point point)
            => grid.Get4NeighborsOf(point).Where(p => grid.GetValueOrDefault(p, '.') != '#');
    }

    private (Direction d, int dist) ParseLine2(string l)
    {
        var split = l.Split(' ')[2];
        var d = split[7] switch
        {
            '2' => Direction.Left,
            '3' => Direction.Up,
            '0' => Direction.Right,
            '1' => Direction.Down,
        };
        var hex = split[2..^2];
        return (d, Convert.ToInt32(hex, 16));
    }
}