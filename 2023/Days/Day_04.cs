using Core;

namespace AoC_2023.Days;

public sealed partial class Day_04 : BaseDay
{
    private readonly string[] _input;

    public Day_04()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        long total = 0L;
        foreach (var line in _input)
        {
            var xx = line.Split(':');
            var yy = xx[1].Split('|');
            var winning = yy[0].ParseInts().ToHashSet();
            var have = yy[1].ParseInts().ToHashSet();
            var overlap = winning.Intersect(have).Count();
            if (overlap > 0)
                total += (long)Math.Pow(2, overlap-1);
        }
        return total.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var temp = new (int id, int overlap, long count)[_input.Length];
        var idx = 0;
        foreach (var line in _input)
        {
            var xx = line.Split(':');
            var yy = xx[1].Split('|');
            var winning = yy[0].ParseInts().ToHashSet();
            var have = yy[1].ParseInts().ToHashSet();
            var overlap = winning.Intersect(have).Count();

            temp[idx++] = (xx[0].ParseInts(1)[0], overlap, 1);
        }

        for (int i = 0; i < temp.Length; i++)
        {
            var card = temp[i];
            var endIdx = card.overlap + i + 1;
            for (int j = i+1; j < Math.Min(temp.Length, endIdx); j++)
            {
                temp[j].count += card.count;
            }
        }

        return temp.Sum(t => t.count).ToString();
    }
}