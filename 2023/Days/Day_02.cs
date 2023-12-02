using Core;
using System.Text.RegularExpressions;

namespace AoC_2023.Days;

public sealed partial class Day_02 : BaseDay
{
    private readonly Game[] _input;

    public Day_02()
    {
        _input = File.ReadAllLines(InputFilePath).SelectArray(ParseLine);
    }

    private Game ParseLine(string line)
    {
        var gameInfo = line.Split(':');
        var id = gameInfo[0].ParseInts(1)[0];

        var r = RedRegex().Matches(gameInfo[1]);
        var g = GreenRegex().Matches(gameInfo[1]);
        var b = BlueRegex().Matches(gameInfo[1]);

        return new Game(id, GetMax(r), GetMax(g), GetMax(b));

        static int GetMax(IReadOnlyList<Match> m) => m.Max(m => int.Parse(m.Value));
    }

    record Game(int Id, int MaxRed, int MaxGreen, int MaxBlue)
    {
        public long Power => MaxRed * MaxGreen * MaxBlue;
    }

    public override async ValueTask<string> Solve_1()
    {
        return _input.Where(IsValid).Sum(it => it.Id).ToString();

        static bool IsValid(Game g) => g.MaxRed <= 12 && g.MaxGreen <= 13 && g.MaxBlue <= 14;
    }

    public override async ValueTask<string> Solve_2()
    {
        return _input.Sum(g => g.Power).ToString();
    }

    [GeneratedRegex(@"\d+(?= red)")]
    private static partial Regex RedRegex();
    [GeneratedRegex(@"\d+(?= green)")]
    private static partial Regex GreenRegex();
    [GeneratedRegex(@"\d+(?= blue)")]
    private static partial Regex BlueRegex();
}