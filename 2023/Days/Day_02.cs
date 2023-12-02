using Core;
using System.Text.RegularExpressions;

namespace AoC_2023.Days;

public sealed class Day_02 : BaseDay
{
    private readonly Game[] _input;

    public Day_02()
    {
        _input = File.ReadAllLines(InputFilePath).SelectArray(ParseLine);
    }

    private Game ParseLine(string line)
    {
        var a = line.Split(':');
        var id = a[0].ParseInts(1)[0];
        var draws = a[1].Split(';');
        return new Game(id, draws.SelectList(Parse));

        Draw Parse(string bb) {
            var r = Regex.Match(bb, @"\d+(?= red)");
            var g = Regex.Match(bb, @"\d+(?= green)");
            var b = Regex.Match(bb, @"\d+(?= blue)");
            return new Draw(Get(r), Get(g), Get(b));
            int Get(Match m) => m.Success ? int.Parse(m.Value) : 0;
        }
    }

    record Game(int Id, List<Draw> Draws);

    record Draw(int red, int green, int blue);

    public override async ValueTask<string> Solve_1()
    {
        return _input.Where(g => g.Draws.All(IsValid)).Sum(it => it.Id).ToString();

        bool IsValid(Draw d) => d.red <= 12 && d.green <= 13 && d.blue <= 14;
    }

    public override async ValueTask<string> Solve_2()
    {
        return _input.Select(GetPower).Sum().ToString();

        long GetPower(Game d)
        {
            var rmin = d.Draws.Max(it => it.red);
            var gmin = d.Draws.Max(it => it.green);
            var bmin = d.Draws.Max(it => it.blue);
            return rmin * gmin * bmin;
        }
    }
}