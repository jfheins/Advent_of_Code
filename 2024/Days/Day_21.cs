using Core;
using System.Linq;
using System.Drawing;

namespace AoC_2024.Days;

public sealed partial class Day_21 : BaseDay
{
    private readonly string[] _input;

    public Day_21()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var res = new List<string>();
        foreach (var code in _input)
        {
            res.Add(SolveFirstRobot(code, 2));
        }

        var cx = res.Zip(_input).Select2(Complexity).ToList();
        return cx.Sum().ToString();
    }

    private int Complexity(string code, string orig)
    {
        var numericPart = int.Parse(string.Concat(orig.Where(char.IsAsciiDigit)));
        Console.WriteLine($"len: {code.Length} * {numericPart} = {code.Length * numericPart}");
        return code.Length * numericPart;
    }

    private string SolveFirstRobot(string code, int depth)
    {
        var finger = DigitLocationNumeric('A');
        var result = new List<string>();
        foreach (var x in code)
        {
            var goal = DigitLocationNumeric(x);
            var possibleMovements = FindAllPathsNumeric(finger, goal);
            var shortestMovement = possibleMovements.Select(m => SolveRobotBefore(m, depth)).MinBy(it => it.Length);
            
            Console.WriteLine($"Move {x} expanded to {shortestMovement}");
            result.Add(shortestMovement!);
            finger = goal;
        }

        return string.Concat(result);
    }

    private string SolveRobotBefore(string moves, int depth)
    {
        if (depth == 0)
            return moves;
        Console.WriteLine($"depth: {depth}, solving {moves}");
        var finger = DigitLocationDir('A');
        var result = new List<string>();
        foreach (var goal in moves.Select(DigitLocationDir))
        {
            var possibleMovements = FindAllPathsDirectional(finger, goal);
            var shortestMovement = possibleMovements.Select(m => SolveRobotBefore(m, depth-1)).MinBy(it => it.Length);
            result.Add(shortestMovement);
            finger = goal;
        }
        Console.WriteLine($"depth: {depth}, solved to {string.Concat(result)}");
        return string.Concat(result);
    }

    private List<string> FindAllPathsNumeric(Point src, Point dest)
    {
        var results = new List<string>();
        FindAllPathsNumeric(src, dest, new List<char>(6), results);
        return results;
    }
    

    private void FindAllPathsNumeric(Point src, Point dest, List<char> path, List<string> result)
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
    
    

    private List<string> FindAllPathsDirectional(Point src, Point dest)
    {
        var results = new List<string>();
        FindAllPathsDirectional(src, dest, new List<char>(5), results);
        return results;
    }
    private void FindAllPathsDirectional(Point src, Point dest, List<char> path, List<string> result)
    {
        if (src == dest)
        {
            result.Add(string.Concat(path.Append('A')));
            return;
        }
        if (src == new Point(0, 0)) // gap
        {
            return;
        }
        
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


    public override async ValueTask<string> Solve_2()
    {
        return "-";
    }
}