using Core;
using Core.Combinatorics;

using System.Numerics;

namespace AoC_2023.Days;

public sealed partial class Day_24 : BaseDay
{
    private readonly Hail[] _input;

    public Day_24()
    {
        _input = File.ReadAllLines(InputFilePath).SelectArray(ParseLine);
    }

    public override async ValueTask<string> Solve_1()
    {
        var testX = LongInterval.FromInclusiveEnd(7, 27);
        var testY = LongInterval.FromInclusiveEnd(7, 27);
        testX = LongInterval.FromInclusiveEnd(200000000000000, 400000000000000);
        testY = LongInterval.FromInclusiveEnd(200000000000000, 400000000000000);

        var collisions = 0;
        foreach (var pair in new Combinations<Hail>(_input, 2))
        {
            var point = GetXYIntersection(pair[0], pair[1]);
            if (point.HasValue)
            {
                if (testX.Contains(point.Value.X) && testY.Contains(point.Value.Y))
                {
                    collisions++;
                  //  Console.WriteLine($"Hail {pair[0]} and {pair[1]} cross at {point.Value.X}/{point.Value.Y}");
                }
                else
                    ;
            }
        }


        return collisions.ToString(); // 28818 wrong; 20858 too low
    }

    private (double X, double Y)? GetXYIntersection(Hail a, Hail b)
    {
        BigInteger a1 = a.V.Y; //  y2-y1
        BigInteger b1 = -a.V.X; //  x1-x2
        BigInteger c1 = a1 * a.Pos.X + b1 * a.Pos.Y;


        BigInteger a2 = b.V.Y; //  y2-y1
        BigInteger b2 = -b.V.X; //  x1-x2
        BigInteger c2 = a2 * b.Pos.X + b2 * b.Pos.Y;

        var det = (long)(a1 * b2 - a2 * b1);

        if (det == 0)
        {
            return null; //Lines are parallel
        }
        else
        {
            var x = ((double)(b2 * c1 - b1 * c2)) / det;
            var y = ((double)(a1 * c2 - a2 * c1)) / det;

            var ta = (x - a.Pos.X) / a.V.X;
            var tb = (x - b.Pos.X) / b.V.X;

            return (ta >= 0 && tb >= 0) ? (x, y) : null;
        }
    }

    record Hail(LongPoint3 Pos, LongPoint3 V);

    Hail ParseLine(string line)
    {
        var n = line.ParseLongs(6);
        return new Hail(new LongPoint3(n[0], n[1], n[2]), new LongPoint3(n[3], n[4], n[5]));
    }

    public override async ValueTask<string> Solve_2()
    {
        return "-";
    }
}