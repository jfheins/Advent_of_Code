using Core;
using System.Runtime.InteropServices;

namespace AoC_2024.Days;

public sealed class Day_17 : BaseDay
{
    private readonly string[] _input;

#if DEBUG
    private static readonly Action<FormattableString> Logger = Console.WriteLine;
#else
    private static readonly Action<FormattableString> Logger = _ => {};
#endif

    public Day_17()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var parts = _input.SplitBy("");
        var regA = parts[0][0].ParseLongs().Single();
        var program = parts[1].Single().ParseInts();
        return string.Join(",", Interpreter(regA, program));
    }

    public override async ValueTask<string> Solve_2()
    {
        var parts = _input.SplitBy("");
        var program = parts[1].Single().ParseInts();

        var solutions = new List<long>();
        for (long i = 0; i < 64; i++)
        {
            solutions.Add(TrySolve(i, 1, program));
        }

        return solutions.Min().ToString(); // 216148338630253
    }

    private static long TrySolve(long state, int digit, int[] program)
    {
        if (digit > program.Length)
            return long.MaxValue;

        Action<long, List<int>> calculator =
            program.SequenceEqual([2, 4, 1, 3, 7, 5, 0, 3, 1, 5, 4, 1, 5, 5, 3, 0])
                ? CalcSpecial
                : (a, list) => Interpreter(a, program, list);

        var output = new List<int>(16);
        for (long i = 0; i < 64; i++)
        {
            var newState = state ^ (i << (digit * 3));
            calculator(newState, output);
            if (IsEqualToProgram(output))
            {
                if (digit == program.Length)
                    return newState;

                var solution = TrySolve(newState, digit + 1, program);
                if (solution < long.MaxValue)
                    return solution;
            }
        }

        return long.MaxValue;

        bool IsEqualToProgram(List<int> res)
        {
            var resSpan = CollectionsMarshal.AsSpan(res);
            return resSpan.Length >= digit && resSpan[..digit].SequenceEqual(program.AsSpan(0, digit));
        }
    }

    private static List<int> Interpreter(long regA, int[] program)
    {
        var res = new List<int>(16);
        Interpreter(regA, program, res);
        return res;
    }

    private static void Interpreter(long regA, int[] program, List<int> output)
    {
        var regB = 0L;
        var regC = 0L;
        output.Clear();

        for (var ip = 0; ip < program.Length;)
        {
            var opCode = program[ip++];
            if (opCode == 0)
            {
                var operand = Combo(program[ip++]);

                var num = regA;
                var den = 1 << (int)operand;
                regA = num / den;
                Logger($"A = {num} / {den} = {regA}");
            }
            else if (opCode == 1)
            {
                var operand = program[ip++];
                regB ^= operand;
                Logger($"B = B ^ {operand} = {regB}");
            }
            else if (opCode == 2)
            {
                var operand = Combo(program[ip++]);
                regB = operand % 8;
                Logger($"B = {operand} % 8 = {regB}");
            }
            else if (opCode == 3)
            {
                var operand = program[ip++];
                if (regA != 0)
                {
                    ip = operand;
                    Logger($"Jump to {operand}");
                }
            }
            else if (opCode == 4)
            {
                _ = program[ip++];
                regB ^= regC;
                Logger($"B = B ^ C = {regB}");
            }
            else if (opCode == 5)
            {
                var rawOp = program[ip];
                var operand = Combo(program[ip++]);
                output.Add((int)(operand % 8));
                if (rawOp == 5)
                    Logger($"Output: B => {operand % 8}");
                else
                    Logger($"Output: {rawOp} => {operand % 8}");
            }
            else if (opCode == 6)
            {
                var operand = Combo(program[ip++]);
                var num = regA;
                var den = 1 << (int)operand;
                regB = num / den;
                Logger($"B = {num} / {den} = {regB}");
            }
            else if (opCode == 7)
            {
                var operand = Combo(program[ip++]);
                regC = regA / (1 << (int)operand);
                Logger($"C = A / 2^{operand} = {regC}");
            }
        }

        long Combo(int i) => i switch
        {
            < 4 => i,
            4 => regA,
            5 => regB,
            6 => regC,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static void CalcSpecial(long regA, List<int> output)
    {
        long regB;
        long regC;
        output.Clear();
        while (regA > 0)
        {
            regB = regA % 8;
            regB ^= 3;
            regC = regA / (1 << (int)regB);
            regA /= 8;
            regB ^= 5;
            regB ^= regC;
            output.Add((int)(regB % 8));
        }
    }
}