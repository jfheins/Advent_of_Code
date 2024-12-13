using Core;
using Spectre.Console;
using System.Drawing;
using Microsoft.Z3;

namespace AoC_2024.Days;

public sealed partial class Day_13 : BaseDay
{
    private readonly string[] _input;

    public Day_13()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        return "*";
        // var blocks = _input.SplitBy("");
        // long cost = 0;
        // foreach (var block in blocks)
        // {
        //     var a = block[0].ParseInts(2);
        //     var b = block[1].ParseInts(2);
        //     var target = block[2].ParseInts(2);
        //     Console.WriteLine(block[2]);
        //
        //     var matrix = DenseMatrix.OfArray(new double[,]
        //     {
        //         { a[0], b[0] },
        //         { a[1], b[1] }
        //     });
        //     try
        //     {
        //         if (Math.Abs(matrix.Determinant()) < 0.001)
        //         {
        //             ;
        //         }
        //         
        //         var inv = matrix.Inverse();
        //         var res = inv * DenseVector.OfArray([..target]);
        //         if (IsValid(res[0], out var x) && IsValid(res[1], out var y))
        //         {
        //             var test0 = a[0] * x + b[0] * y;
        //             var test1 = a[1] * x + b[1] * y;
        //             if (test0 != target[0] || test1 != target[1])
        //             {
        //                 Console.WriteLine($"[red]Error: {test0}, {test1} != {target}[/]");
        //             }
        //             else
        //             {
        //                 Console.WriteLine($"Push A {x} times and B {y} times: ");
        //                 Console.WriteLine($"{x} * {a[0]} + {y} * {b[0]} = {test0} == {target[0]}");
        //                 Console.WriteLine($"{x} * {a[1]} + {y} * {b[1]} = {test1} == {target[1]}");
        //                 cost += x * 3 + y;
        //             }
        //         }
        //         else
        //         {
        //             Console.WriteLine($"Rejected {res[0]} A presses with {res[1]} B presses");
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine(e);
        //     }
        // }
        //
        // return cost.ToString(); // not 17361
        //
        // bool IsValid(double x, out long rounded)
        // {
        //     if (x is > -0.1 and < 100.1 && Math.Abs(x - Math.Round(x)) < 0.01)
        //     {
        //         rounded = (long)Math.Round(x);
        //         return true;
        //     }
        //
        //     rounded = 0;
        //     return false;
        // }
    }

    public override async ValueTask<string> Solve_2()
    {
        var blocks = _input.SplitBy("");
        long cost = 0;
        foreach (var block in blocks)
        {
            var a = block[0].ParseLongs(2);
            var b = block[1].ParseLongs(2);
            var target = block[2].ParseLongs(2);
            target[0] *= 10000000000000L;
            target[1] += 10000000000000L;
            Console.WriteLine(block[2]);


            using var ctx = new Context();
            
            var x = ctx.MkConst("x", ctx.IntSort);
            var y = ctx.MkConst("y", ctx.IntSort);
            var a0 = ctx.MkConst("a0", ctx.IntSort);
            var a1 = ctx.MkConst("a1", ctx.IntSort);
            var b0 = ctx.MkConst("b0", ctx.IntSort);
            var b1 = ctx.MkConst("b1", ctx.IntSort);
            
            var solver = ctx.MkSolver();
            solver.Assert(
                MkEq(x, a0, y, b0, target[0]),
                MkEq(x, a1, y, b1, target[1])
            );
            
            Console.WriteLine(solver.Check(
                ctx.MkGt((ArithExpr)x, ctx.MkInt(0)),
                ctx.MkGt((ArithExpr)y, ctx.MkInt(0))
            ));
            
            Model m = solver.Model;
            
            foreach (var d in m.Decls.OrderBy(it => it.Name.ToString()))
                Console.WriteLine(d.Name + " = " + m.ConstInterp(d));
            
            
            BoolExpr MkEq(Expr xx, Expr vv, Expr yy, Expr ww, long rightSide)
                => ctx.MkEq(
                    ctx.MkAdd(
                        ctx.MkMul((ArithExpr)xx, (ArithExpr)vv), 
                        ctx.MkMul((ArithExpr)yy, (ArithExpr)ww)
                    ), 
                    ctx.MkInt(rightSide)
                    );
            // try
            // {
            //     z3
            //     var inv = matrix.Inverse();
            //     var res = inv * DenseVector.OfArray([..target]);
            //     if (IsValid(res[0], out var x) && IsValid(res[1], out var y))
            //     {
            //         var test0 = a[0] * x + b[0] * y;
            //         var test1 = a[1] * x + b[1] * y;
            //         if (test0 != target[0] || test1 != target[1])
            //         {
            //             Console.WriteLine($"Error: {test0}, {test1} != {target[0]}, {target[1]}");
            //         }
            //         else
            //         {
            //             Console.WriteLine($"Push A {x} times and B {y} times: ");
            //             Console.WriteLine($"{x} * {a[0]} + {y} * {b[0]} = {test0} == {target[0]}");
            //             Console.WriteLine($"{x} * {a[1]} + {y} * {b[1]} = {test1} == {target[1]}");
            //             cost += x * 3 + y;
            //         }
            //     }
            //     else
            //     {
            //         Console.WriteLine($"Rejected {res[0]} A presses with {res[1]} B presses");
            //     }
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine(e);
            // }
        }

        return cost.ToString(); 
    }
}