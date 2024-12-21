using Core;
using System.Drawing;

namespace AoC_2024.Days;

public sealed class Day_21 : BaseDay
{
    private readonly string[] _input;

    public Day_21()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override void Clear()
    {
        _cacheLong.Clear();
        _cache2.Clear();
    }

    public override async ValueTask<string> Solve_1()
        => _input.Select(code => CalculateComplexity(code, 2)).Sum().ToString();

    public override async ValueTask<string> Solve_2()
        => _input.Select(code => CalculateComplexity(code, 25)).Sum().ToString();

    private long CalculateComplexity(string code, int depth)
    {
        var codeLength = SolveFirstRobot(code, depth);
        var numericPart = int.Parse(string.Concat(code.Where(char.IsAsciiDigit)));
        return codeLength * numericPart;
    }

    private long SolveFirstRobot(string code, int depth)
    {
        var finger = DigitLocationNumeric('A');
        var result = 0L;
        foreach (var goal in code.Select(DigitLocationNumeric))
        {
            var possibleMovements = FindAllPathsNumeric(finger, goal);
            result += possibleMovements.Min(it => SolveRobotBefore(it, depth));
            finger = goal;
        }

        return result;
    }

    private readonly Dictionary<(string moves, int depth), long> _cacheLong = new();

    private long SolveRobotBefore(string moves, int depth)
    {
        if (depth == 0)
            return moves.Length;

        if (_cacheLong.TryGetValue((moves, depth), out var cached))
            return cached;

        var finger = DigitLocationDir('A');
        var result = 0L;
        foreach (var goal in moves.Select(DigitLocationDir))
        {
            var possibleMovements = FindAllPathsDirectional(finger, goal);
            result += possibleMovements.Min(it => SolveRobotBefore(it, depth - 1));
            finger = goal;
        }

        return _cacheLong[(moves, depth)] = result;
    }


    private static List<string> FindAllPathsNumeric(Point src, Point dest)
    {
        var results = new List<string>();
        FindAllPathsNumeric(src, dest, new List<char>(6), results);
        return results;
    }


    private static void FindAllPathsNumeric(Point src, Point dest, List<char> path, List<string> result)
    {
        if (src == dest)
        {
            result.Add(string.Concat(path.Append('A')));
            return;
        }

        if (src == new Point(0, 3)) // gap
        {
            return;
        }

        if (dest.Y < src.Y) // move up
        {
            path.Add('^');
            FindAllPathsNumeric(src.MoveTo(Direction.Up), dest, path, result);
            path.RemoveAt(path.Count - 1);
        }

        if (dest.X < src.X)
        {
            path.Add('<');
            FindAllPathsNumeric(src.MoveTo(Direction.Left), dest, path, result);
            path.RemoveAt(path.Count - 1);
        }

        if (dest.X > src.X)
        {
            path.Add('>');
            FindAllPathsNumeric(src.MoveTo(Direction.Right), dest, path, result);
            path.RemoveAt(path.Count - 1);
        }

        if (dest.Y > src.Y)
        {
            path.Add('v');
            FindAllPathsNumeric(src.MoveTo(Direction.Down), dest, path, result);
            path.RemoveAt(path.Count - 1);
        }
    }

    private readonly Dictionary<(Point, Point), List<string>> _cache2 = new();

    private List<string> FindAllPathsDirectional(Point src, Point dest)
    {
        if (_cache2.TryGetValue((src, dest), out var c))
            return c;

        var results = new List<string>();
        FindAllPathsDirectional(src, dest, new List<char>(5), results);
        return _cache2[(src, dest)] = results;
    }

    private static void FindAllPathsDirectional(Point src, Point dest, List<char> path, List<string> result)
    {
        if (src == dest)
        {
            result.Add(string.Concat(path.Append('A')));
            return;
        }

        if (src == new Point(0, 0)) // gap
            return;

        if (dest.Y < src.Y) // move up
        {
            path.Add('^');
            FindAllPathsDirectional(src.MoveTo(Direction.Up), dest, path, result);
            path.RemoveAt(path.Count - 1);
        }

        if (dest.X < src.X)
        {
            path.Add('<');
            FindAllPathsDirectional(src.MoveTo(Direction.Left), dest, path, result);
            path.RemoveAt(path.Count - 1);
        }

        if (dest.X > src.X)
        {
            path.Add('>');
            FindAllPathsDirectional(src.MoveTo(Direction.Right), dest, path, result);
            path.RemoveAt(path.Count - 1);
        }

        if (dest.Y > src.Y)
        {
            path.Add('v');
            FindAllPathsDirectional(src.MoveTo(Direction.Down), dest, path, result);
            path.RemoveAt(path.Count - 1);
        }
    }

    private static Point DigitLocationNumeric(char digit)
        => digit switch
        {
            'A' => new Point(2, 3),
            '0' => new Point(1, 3),
            '1' => new Point(0, 2),
            '2' => new Point(1, 2),
            '3' => new Point(2, 2),
            '4' => new Point(0, 1),
            '5' => new Point(1, 1),
            '6' => new Point(2, 1),
            '7' => new Point(0, 0),
            '8' => new Point(1, 0),
            '9' => new Point(2, 0),
            _ => throw new InvalidOperationException()
        };

    private static Point DigitLocationDir(char digit)
        => digit switch
        {
            '^' => new Point(1, 0),
            'A' => new Point(2, 0),
            '<' => new Point(0, 1),
            'v' => new Point(1, 1),
            '>' => new Point(2, 1),
            _ => throw new InvalidOperationException()
        };
}