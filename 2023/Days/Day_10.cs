using Core;

using System.Drawing;

namespace AoC_2023.Days;

public sealed partial class Day_10 : BaseDay
{
    private readonly string[] _input;
    private readonly FiniteGrid2D<char> _grid;
    private Point _farPoint;
    private Point _usedNeighbor;
    private Point[] _loop;

    public Day_10()
    {
        _input = File.ReadAllLines(InputFilePath);
        _grid = new FiniteGrid2D<char>(_input);
    }

    public override async ValueTask<string> Solve_1()
    {
        var bfs = new BreadthFirstSearch<Point>(null, Expand1);
        var s = _grid.FindFirst('S');
        var target = bfs.FindLeafs(s).First();
        _farPoint = target.Target;
        _usedNeighbor = target.Steps[1];

        return target.Length.ToString();
    }


    private IEnumerable<Point> Expand2(Point p)
    {
        if (_grid[p] == 'S')
        {
            // neighb that can be walked into
            var alln = _grid.Get4NeighborsOf(p).ExceptFor(_usedNeighbor).ToList();
            var validN = alln.Where(n => Expand1(n).Any(nn => nn == p));
            return validN;
        }
        else
            return Expand1(p);
    }

    private IEnumerable<Point> Expand1(Point p)
    {
        if (_grid[p] == 'S')
        {
            // neighb that can be walked into
            var alln = _grid.Get4NeighborsOf(p).ToList();
            var validN = alln.Where(n => Expand1(n).Any(nn => nn == p));
            return validN;
        }
        else
        {
            IEnumerable<Direction> dir = _grid[p] switch
            {
                /*
                | is a vertical pipe connecting north and south.
                - is a horizontal pipe connecting east and west.
                L is a 90-degree bend connecting north and east.
                J is a 90-degree bend connecting north and west.
                7 is a 90-degree bend connecting south and west.
                F is a 90-degree bend connecting south and east.
                . is ground; there is no pipe in this tile.
                 */
                '|' => Directions.Vertical,
                '-' => Directions.Horizontal,
                'L' => [Direction.Up, Direction.Right],
                'J' => [Direction.Up, Direction.Left],
                '7' => [Direction.Down, Direction.Left],
                'F' => [Direction.Down, Direction.Right],
                '.' => [],
                'S' => Directions.All4,
                _ => throw new NotImplementedException(),
            };
            return dir.Select(d => p.MoveTo(d)).Where(_grid.Contains);
        }
    }

    public override async ValueTask<string> Solve_2()
    {
        var bfs = new BreadthFirstSearch<Point>(null, Expand2) { PerformParallelSearch = false };
        var s = _grid.FindFirst('S');
        var paths = bfs.FindAll(s, p => p == _usedNeighbor);
        _loop = paths.First().Steps;

        var xx = FindInnerPoints(Direction.Left);
        var yy = FindInnerPoints(Direction.Down);
        var yyy = FindInnerPoints(Direction.Right);
        var xxx = FindInnerPoints(Direction.Up);


        return "One of " + string.Join(", ", xx.Count, yy.Count, yyy.Count, xxx.Count);
    }

    public HashSet<Point> FindInnerPoints(Direction inside)
    {
        var edge = _loop.ToHashSet();
        var insidePoints = new List<Point>();

        foreach (var (pred, p) in _loop.Append(_loop[0]).PairwiseWithOverlap())
        {
            if (!edge.Contains(p.MoveTo(inside)))
            {
                insidePoints.Add(p.MoveTo(inside));
            }
            var tile = _grid[p];

            if (tile == '-' && !Directions.Vertical.Contains(inside))
                return [];
            if (tile == '|' && !Directions.Horizontal.Contains(inside))
                return [];

            if ("LJ7F".Contains(tile))
            {
                var predMove = GetMove(pred, p);
                if ((predMove == Direction.Left && tile == 'F')
                    || (predMove == Direction.Up && tile == '7')
                    || (predMove == Direction.Right && tile == 'J')
                    || (predMove == Direction.Down && tile == 'L'))
                    inside = inside.TurnCounterClockwise();

                else if ((predMove == Direction.Left && tile == 'L')
                    || (predMove == Direction.Up && tile == 'F')
                    || (predMove == Direction.Right && tile == '7')
                    || (predMove == Direction.Down && tile == 'J'))
                    inside = inside.TurnClockwise();
            }
            if (!edge.Contains(p.MoveTo(inside)))
            {
                insidePoints.Add(p.MoveTo(inside));
            }

        }
        var result = new HashSet<Point>();
        var fill = new BreadthFirstSearch<Point>(null, FloodFill);
        foreach (var p in insidePoints)
        {
            foreach (var np in fill.FindReachable(p))
                result.Add(np);
        }


        return result;


        static Direction GetMove(Point a, Point b)
        {
            return (b.X - a.X, b.Y - a.Y) switch
            {
                (1, 0) => Direction.Right,
                (0, 1) => Direction.Down,
                (0, -1) => Direction.Up,
                (-1, 0) => Direction.Left,
                _ => throw new NotImplementedException(),
            };
        }

        IEnumerable<Point> FloodFill(Point point) => _grid.Get4NeighborsOf(point).Where(p => !edge.Contains(p));
    }
}