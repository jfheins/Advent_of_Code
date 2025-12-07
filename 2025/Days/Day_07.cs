using Core;
using System.Drawing;

namespace AoC_2025.Days;

public sealed class Day_07 : BaseDay
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
        var totalSplits = 0L;
        var grid = new FiniteGrid2D<char>(_input);
        var s = grid.FindFirst('S');
        var beams = new List<(Point pos, long timelines)> { (s.MoveTo(Direction.Down), 1) };

        while (beams.Count > 0)
        {
            foreach (var beam in beams) grid[beam.pos] = '|';

            var newbeams = new Dictionary<Point, long>();
            foreach (var (pos, timelines) in beams)
            {
                var next = grid.GetValueOrDefault(pos.MoveTo(Direction.Down), 'x');
                if (next == '.')
                {
                    newbeams.AddOrModify(pos.MoveTo(Direction.Down), 0, count => count + timelines);
                }

                if (next == '^')
                {
                    var left = pos.MoveBy(-1, 1);
                    newbeams.AddOrModify(left, 0, count => count + timelines);
                    var right = pos.MoveBy(1, 1);
                    newbeams.AddOrModify(right, 0, count => count + timelines);
                }

                if (next == 'x') // bottom
                {
                    totalSplits += timelines;
                }
            }

            // Console.WriteLine(grid.ToString());
            // Console.WriteLine($"Beams: {newbeams.Count}, timelines: {newbeams.Values.Sum()}");
            beams = newbeams.Select(kvp => (kvp.Key, kvp.Value)).ToList();
        }

        return totalSplits.ToString(); // 13181222377
    }
}