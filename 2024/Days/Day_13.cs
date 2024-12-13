using System.Diagnostics;
using System.Numerics;
using Core;

namespace AoC_2024.Days;

public sealed class Day_13 : BaseDay
{
    private readonly Machine[] _input;

    public Day_13()
    {
        _input = File.ReadAllLines(InputFilePath).SplitBy("").SelectArray(Machine.Parse);
    }

    public record Machine(long[] BtnA, long[] BtnB, long[] Prize)
    {
        public static Machine Parse(ArraySegment<string> block)
        {
            var a = block[0].ParseLongs(2);
            var b = block[1].ParseLongs(2);
            var prize = block[2].ParseLongs(2);
            return new Machine(a, b, prize);
        }
    }

    public override async ValueTask<string> Solve_1()
    {
        long cost = 0;
        foreach (var block in _input)
        {
            var res = SolveEquations(block, block.Prize);

            if (!IsValid(res[0], out var btnAPresses) || !IsValid(res[1], out var btnBPresses))
                continue;

            var testX = btnAPresses * block.BtnA[0] + btnBPresses * block.BtnB[0];
            var testY = btnAPresses * block.BtnA[1] + btnBPresses * block.BtnB[1];

            if (testX == block.Prize[0] && testY == block.Prize[1])
                cost += btnAPresses * 3 + btnBPresses;
        }

        return cost.ToString();

        bool IsValid(double x, out long rounded)
        {
            if (x is >= 0 and <= 100 && Math.Abs(x - Math.Round(x)) < 0.001d)
            {
                rounded = (long)Math.Round(x);
                return true;
            }

            rounded = 0;
            return false;
        }
    }


    public override async ValueTask<string> Solve_2()
    {
        long cost = 0;
        foreach (var machine in _input)
        {
            var actualPrize = new[]
            {
                machine.Prize[0] + 10000000000000L,
                machine.Prize[1] + 10000000000000L
            };
            var res = SolveEquations(machine, actualPrize);

            if (!IsValid(res[0], out var btnAPresses) || !IsValid(res[1], out var btnBPresses))
                continue;

            var testX = btnAPresses * machine.BtnA[0] + btnBPresses * machine.BtnB[0];
            var testY = btnAPresses * machine.BtnA[1] + btnBPresses * machine.BtnB[1];

            if (testX == actualPrize[0] && testY == actualPrize[1])
                cost += btnAPresses * 3 + btnBPresses;
        }

        return cost.ToString();

        bool IsValid(double x, out long rounded)
        {
            if (x >= 0 && Math.Abs(x - Math.Round(x)) < 0.001d)
            {
                rounded = (long)Math.Round(x);
                return true;
            }

            rounded = 0;
            return false;
        }
    }

    private static double[] SolveEquations(Machine m, long[] rightSide)
    {
        var matrix = new double[,]
        {
            { m.BtnA[0], m.BtnB[0] },
            { m.BtnA[1], m.BtnB[1] }
        };
        var inv = MatrixInverse(matrix);
        return MatrixMultiply(inv, Array.ConvertAll(rightSide, it => (double)it));
    }

    public static T[,] MatrixInverse<T>(T[,] matrix) where T : INumber<T>
    {
        Debug.Assert(matrix.GetLength(0) == 2 && matrix.GetLength(1) == 2);

        var det = matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
        if (det == T.Zero)
            throw new InvalidOperationException("Matrix is singular");
        var invDet = T.One / det;
        return new[,]
        {
            { matrix[1, 1] * invDet, -matrix[0, 1] * invDet },
            { -matrix[1, 0] * invDet, matrix[0, 0] * invDet }
        };
    }

    public static T[] MatrixMultiply<T>(T[,] matrix, T[] vec) where T : INumber<T>
    {
        Debug.Assert(matrix.GetLength(1) == vec.Length);
        return
        [
            matrix[0, 0] * vec[0] + matrix[0, 1] * vec[1],
            matrix[1, 0] * vec[0] + matrix[1, 1] * vec[1]
        ];
    }
}