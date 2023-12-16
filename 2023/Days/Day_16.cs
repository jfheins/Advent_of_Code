using Core;
using Spectre.Console;
using System.Drawing;

namespace AoC_2023.Days;

public sealed partial class Day_16 : BaseDay
{
    private readonly string[] _input;

    public Day_16()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var grid = new FiniteGrid2D<char>(_input);
        var beam = (new Point(-1, 0), Direction.Right);

        var beams = new HashSet<(Point pos, Direction dir)>();
        Trace(grid, beams, beam);
        beams.Remove(beam);

        var tiles = beams.Select(it => it.pos).ToHashSet();

        return tiles.Count.ToString();
    }

    private void Trace(
        FiniteGrid2D<char> grid,
        HashSet<(Point pos, Direction dir)> seen,
        (Point pos, Direction dir) beam)
    {
        while (seen.Add(beam))
        {
            var nextPos = beam.pos.MoveTo(beam.dir);

            if (!grid.Bounds.Contains(nextPos))
                return;

            var cell = grid.GetValueOrDefault(nextPos, '.');
            if (cell == '/')
            {
                var newDir = beam.dir.TurnCounterClockwise();
                if (Directions.Vertical.Contains(beam.dir))
                    newDir = beam.dir.TurnClockwise();
                beam = (nextPos, newDir);
            }
            else if (cell == '\\')
            {
                var newDir = beam.dir.TurnClockwise();
                if (Directions.Vertical.Contains(beam.dir))
                    newDir = beam.dir.TurnCounterClockwise();
                beam = (nextPos, newDir);
            }
            else if (cell == '|' && Directions.Horizontal.Contains(beam.dir))
            {
                Trace(grid, seen, (nextPos, Direction.Up));
                Trace(grid, seen, (nextPos, Direction.Down));
            }
            else if (cell == '-' && Directions.Vertical.Contains(beam.dir))
            {
                Trace(grid, seen, (nextPos, Direction.Left));
                Trace(grid, seen, (nextPos, Direction.Right));
            }
            else
            {
                beam = (nextPos, beam.dir);
            }
        }
    }

    public override async ValueTask<string> Solve_2()
    {
        var grid = new FiniteGrid2D<char>(_input);

        var beams = grid.GetRowTuple(0)
            .Select(r => (r.pos, Direction.Down))
            .Concat(grid.GetRowTuple(grid.Bounds.Bottom - 1).Select(r => (r.pos, Direction.Up)))
            .Concat(grid.GetColTuple(0).Select(r => (r.pos, Direction.Right)))
            .Concat(grid.GetColTuple(grid.Bounds.Right - 1).Select(r => (r.pos, Direction.Left)));



        return beams.Max(b => TraceOne(b.pos, b.Item2)).ToString();


        int TraceOne(Point p, Direction d)
        {
            var beams = new HashSet<(Point pos, Direction dir)>();
            var beam = (p.MoveTo(d.Opposite()), d);
            Trace(grid, beams, beam);
            beams.Remove(beam);
            return beams.Select(it => it.pos).ToHashSet().Count;
        }
    }
}