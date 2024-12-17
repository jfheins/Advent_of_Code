using Core;
using System.Runtime.InteropServices;

namespace AoC_2024.Days;

public sealed class Day_17 : BaseDay
{
    private readonly string[] _input;

    public Day_17()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var parts = _input.SplitBy("");
        var regA = parts[0][0].ParseLongs().Single();
        var regB = parts[0][1].ParseLongs().Single();
        var regC = parts[0][2].ParseLongs().Single();
        var program = parts[1].Single().ParseInts();

        return string.Join(",", Calc(regA, program));
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

        foreach (var s in solutions.Where(it => it < long.MaxValue).Distinct().Order())
        {
            Console.WriteLine($"Potential Solution: {s}");
        }

        return solutions.Min().ToString(); //  216148338630335 too high
    }

    private long TrySolve(long state, int digit, int[] program)
    {
        if (digit > program.Length)
            return long.MaxValue;

        var output = new List<int>(16);
        for (long i = 0; i < 64; i++)
        {
            var newState = state ^ (i << (digit * 3));
            Calc(newState, program, output);
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

    private static List<int> Calc(long regA, int[] program)
    {
        var res = new List<int>(16);
        Calc(regA, program, res);
        return res;
    }

    private static void Calc(long regA, int[] program, List<int> output)
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
                //    Console.WriteLine($"A = {num} / {den} = {regA}");
            }
            else if (opCode == 1)
            {
                /*
                 * The bxl instruction (opcode 1) calculates the bitwise XOR of register B
                 * and the instruction's literal operand, then stores the result in register B.
                 */
                var operand = program[ip++];
                regB ^= operand;
                // Console.WriteLine($"B = B ^ {operand} = {regB}");
            }
            /*
             *The bst instruction (opcode 2) calculates the value of its combo operand modulo 8
             * (thereby keeping only its lowest 3 bits), then writes that value to the B register.
             */
            else if (opCode == 2)
            {
                var operand = Combo(program[ip++]);
                regB = operand % 8;
                // Console.WriteLine($"B = {operand} % 8 = {regB}");
            }
            /*
           The jnz instruction (opcode 3) does nothing if the A register is 0. However,
           if the A register is not zero, it jumps by setting the instruction pointer to the value of its literal operand; if this instruction jumps, the instruction pointer is not increased by 2 after this instruction.
           */
            else if (opCode == 3)
            {
                var operand = program[ip++];
                if (regA != 0)
                {
                    ip = operand;
                    // Console.WriteLine($"Jump to {operand}");
                }
            }
            /*
             The bxc instruction (opcode 4) calculates the bitwise XOR of register B and
             register C, then stores the result in register B. (For legacy reasons, this instruction reads an operand but ignores it.)
            */
            else if (opCode == 4)
            {
                _ = program[ip++];
                regB ^= regC;
                // Console.WriteLine($"B = B ^ C = {regB}");
            }
            /*
            The out instruction (opcode 5) calculates the value of its combo operand modulo 8,
            then outputs that value. (If a program outputs multiple values, they are separated by commas.)
            */
            else if (opCode == 5)
            {
                var rawOp = program[ip];
                var operand = Combo(program[ip++]);
                output.Add((int)(operand % 8));
                // if (rawOp == 5)
                //     Console.WriteLine($"Output: B => {operand % 8}");
                // else
                //     Console.WriteLine($"Output: {rawOp} => {operand % 8}");
            }
            /*
            The bdv instruction (opcode 6) works exactly like the adv instruction except that
            the result is stored in the B register. (The numerator is still read from the A register.)
            */
            else if (opCode == 6)
            {
                var operand = Combo(program[ip++]);
                var num = regA;
                var den = 1 << (int)operand;
                regB = num / den;
                //Console.WriteLine($"B = {num} / {den} = {regB}");
            }
            /*
            The cdv instruction (opcode 7) works exactly like the adv instruction except that
            the result is stored in the C register. (The numerator is still read from the A register.)
            */
            else if (opCode == 7)
            {
                var operand = Combo(program[ip++]);
                regC = regA / (1 << (int)operand);
                // Console.WriteLine($"C = A / 2^{operand} = {regC}");
            }
        }

        // Console.WriteLine($"Reg a: {regA}");
        // Console.WriteLine($"Reg b: {regB}");
        // Console.WriteLine($"Reg c: {regC}");

        // for (int i = 0; i < 8; i++)
        // {
        //     Console.WriteLine(string.Concat("241375031541553".Reverse().Select(it => (it ^ i)%8)));
        // }

        long Combo(int i) => i switch
        {
            < 4 => i,
            4 => regA,
            5 => regB,
            6 => regC,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}