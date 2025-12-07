using Core;
using System.Linq;
using System.Drawing;

namespace AoC_2025.Days;

public sealed partial class Day_07 : BaseDay
{
    private readonly string[] _input;

    public Day_07()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var totalSplits = 0;
        var grid = new FiniteGrid2D<char>(_input);
        var s = grid.FindFirst('S');
        var beams = new List<Point> { s.MoveTo(Direction.Down) };

        while (beams.Count > 0)
        {
            foreach (var p in beams) grid[p] = '|';

            var newbeams = new HashSet<Point>(beams.Capacity);
            var splits = 0;

            var xx = beams.Select(it => it.MoveTo(Direction.Down)).Select(p => (p, grid.GetValueOrDefault(p, 'x')));

            foreach (var beam in xx.Where(it => it.Item2 == '.'))
                newbeams.Add(beam.p);

            foreach (var (pos, _) in xx.Where(it => it.Item2 == '^'))
            {
                if (newbeams.Add(pos.MoveBy(1, 0)))
                    splits++;
                if (newbeams.Add(pos.MoveBy(-1, 0)))
                    splits++;
            }

            splits = xx.Count(it => it.Item2 == '^');
            totalSplits += splits;
            beams = newbeams.ToList();
        }

        return totalSplits.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var totalSplits = 0;
        var grid = new FiniteGrid2D<char>(_input);
        var s = grid.FindFirst('S');
        var beams = new List<Point> { s.MoveTo(Direction.Down) };

        while (beams.Count > 0)
        {
            foreach (var p in beams) grid[p] = '|';

            var newbeams = new HashSet<Point>(beams.Capacity);
            var splits = 0;

            var xx = beams.Select(it => it.MoveTo(Direction.Down)).Select(p => (p, grid.GetValueOrDefault(p, 'x')));

            foreach (var beam in xx.Where(it => it.Item2 == '.'))
                newbeams.Add(beam.p);

            foreach (var (pos, _) in xx.Where(it => it.Item2 == '^'))
            {
                if (newbeams.Add(pos.MoveBy(1, 0)))
                    splits++;
                if (newbeams.Add(pos.MoveBy(-1, 0)))
                    splits++;
            }

            splits = xx.Count(it => it.Item2 == '^');
            totalSplits += splits;
            beams = newbeams.ToList();
        }

        return totalSplits.ToString();
    }
}