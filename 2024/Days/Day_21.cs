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
            var x = Num2Dir(code);
            var xx = Dir2Dir(x);
            Console.WriteLine("--_ " + xx);
            var xxx = Dir2Dir(xx);
            Console.WriteLine("-__ " + xxx);
            res.Add(xxx);
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

    // Translate numeric keystrokes to directional
    private static string Num2Dir(string code)
    {
        var finger = DigitLocationNumeric('A');
        var commands = new List<string>();
        foreach (var goal in code.Select(DigitLocationNumeric))
        {
            if (goal.Y < finger.Y) 
                commands.Add(new string('^', finger.Y - goal.Y));
            if (goal.X < finger.X) 
                commands.Add(new string('<', finger.X - goal.X));
            if (goal.X > finger.X) 
                commands.Add(new string('>', goal.X - finger.X));
            if (goal.Y > finger.Y) 
                commands.Add(new string('v', goal.Y - finger.Y));
            finger = goal;
            commands.Add("A");
        }

        return string.Concat(commands);
    }

    // Translate direction keystrokes to directional
    private static string Dir2Dir(string code)
    {
        var finger = DigitLocationDir('A');
        var commands = new List<string>();
        foreach (var goal in code.Select(DigitLocationDir))
        {
            if (goal.Y > finger.Y) 
                commands.Add(new string('v', goal.Y - finger.Y));
            if (goal.X > finger.X) 
                commands.Add(new string('>', goal.X - finger.X));
            if (goal.X < finger.X) 
                commands.Add(new string('<', finger.X - goal.X));
            if (goal.Y < finger.Y) 
                commands.Add(new string('^', finger.Y - goal.Y));
            finger = goal;
            commands.Add("A");
        }

        return string.Concat(commands);
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