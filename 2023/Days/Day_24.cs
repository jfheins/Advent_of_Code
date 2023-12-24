using Core;
using Core.Combinatorics;

using Microsoft.Z3;

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
        var testX = LongInterval.FromInclusiveEnd(200000000000000, 400000000000000);       
        var testY = LongInterval.FromInclusiveEnd(200000000000000, 400000000000000);

        var collisions = 0;
        foreach (var pair in new Combinations<Hail>(_input, 2))
        {
            var point = GetXYIntersection(pair[0], pair[1]);
            if (point.HasValue)
            {
                if (testX.Contains(point.Value.X) && testY.Contains(point.Value.Y))
                    collisions++;
            }
        }
        return collisions.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var (h0, h1, h2) = _input.ToTuple3();

        using var ctx = new Context();

        var t0 = ctx.MkConst("t0", ctx.RealSort);
        var t1 =  ctx.MkConst("t1", ctx.RealSort);
        var t2 =  ctx.MkConst("t2", ctx.RealSort);
        var sx =  ctx.MkConst("x", ctx.RealSort);
        var sy =  ctx.MkConst("y", ctx.RealSort);
        var sz =  ctx.MkConst("z", ctx.RealSort);
        var svx = ctx.MkConst("vx", ctx.RealSort);
        var svy = ctx.MkConst("vy", ctx.RealSort);
        var svz = ctx.MkConst("vz", ctx.RealSort);

        var solver = ctx.MkSolver();
        solver.Assert(
            MkEq(sx, t0, svx, h0.Pos.X, h0.V.X),
            MkEq(sy, t0, svy, h0.Pos.Y, h0.V.Y),
            MkEq(sz, t0, svz, h0.Pos.Z, h0.V.Z),
            MkEq(sx, t1, svx, h1.Pos.X, h1.V.X),
            MkEq(sy, t1, svy, h1.Pos.Y, h1.V.Y),
            MkEq(sz, t1, svz, h1.Pos.Z, h1.V.Z),
            MkEq(sx, t2, svx, h2.Pos.X, h2.V.X),
            MkEq(sy, t2, svy, h2.Pos.Y, h2.V.Y),
            MkEq(sz, t2, svz, h2.Pos.Z, h2.V.Z)
            );

        Console.WriteLine(solver.Check(
            ctx.MkGt((ArithExpr)t0, ctx.MkInt(0)),
            ctx.MkGt((ArithExpr)t1, ctx.MkInt(0)),
            ctx.MkGt((ArithExpr)t2, ctx.MkInt(0)),
            ctx.MkNot(ctx.MkEq((ArithExpr)t0, (ArithExpr)t1)),
            ctx.MkNot(ctx.MkEq((ArithExpr)t0, (ArithExpr)t2)),
            ctx.MkNot(ctx.MkEq((ArithExpr)t1, (ArithExpr)t2))
            ));
        Model m = solver.Model;

        foreach (var d in m.Decls.OrderBy(it => it.Name.ToString()))
            Console.WriteLine(d.Name + " = " + m.ConstInterp(d));

        return (Value(sx) + Value(sy) + Value(sz)).ToString();

        BoolExpr MkEq(Expr s, Expr t, Expr v, long right, long hailV)
            => ctx.MkEq(
                 ctx.MkAdd(
                    (ArithExpr)s, 
                    ctx.MkMul((ArithExpr)t, (ArithExpr)v)
                    ),
                 ctx.MkAdd(
                    ctx.MkInt(right),
                    ctx.MkMul((ArithExpr)t, ctx.MkInt(hailV))
                    ));
        BigInteger Value(Expr e) => BigInteger.Parse(m.ConstInterp(e).ToString());
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
}