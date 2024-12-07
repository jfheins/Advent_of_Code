using Core;

namespace AoC_2024.Days;

public sealed class Day_07 : BaseDay
{
    private readonly List<Equation> _input;

    public Day_07()
    {
        _input = File.ReadAllLines(InputFilePath).SelectList(Equation.Parse);
    }

    public override async ValueTask<string> Solve_1()
    {
        long result = 0;
        _input.AsParallel()
            .Where(equation => CanBeSolved(equation, Addition, Multiplication))
            .ForAll(equation =>
            {
                Interlocked.Add(ref result, equation.TestValue);
                equation.SolvedPart1 = true;
            });

        return result.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        return _input
            .AsParallel()
            .Where(equation => equation.SolvedPart1 || CanBeSolved(equation, Addition, Multiplication, Concatenation))
            .Sum(equation => equation.TestValue).ToString();
    }

    private static bool CanBeSolved(Equation eq, params ReadOnlySpan<Operator> possibleOperations)
        => CanBeSolved(eq.TestValue, eq.Operands[0], eq.Operands.AsSpan()[1..], possibleOperations);

    private static bool CanBeSolved(
        long result,
        long accumulate,
        ReadOnlySpan<long> operands,
        params ReadOnlySpan<Operator> possibleOperations)
    {
        if (accumulate > result)
            return false;
        if (operands.Length == 0)
            return accumulate == result;

        foreach (var operation in possibleOperations)
        {
            if (CanBeSolved(result, operation(accumulate, operands[0]), operands[1..], possibleOperations))
                return true;
        }

        return false;
    }

    private delegate long Operator(long a, long b);

    private static long Addition(long a, long b) => a + b;
    private static long Multiplication(long a, long b) => a * b;
    private static long Concatenation(long a, long b)
        => b switch
        {
            < 10 => a * 10 + b,
            < 100 => a * 100 + b,
            _ => a * 1000 + b
        };

    private record Equation(long TestValue, long[] Operands)
    {
        public bool SolvedPart1 { get; set; }

        public static Equation Parse(string line)
        {
            var numbers = line.ParseLongs();
            return new Equation(numbers[0], numbers[1..]);
        }
    }
}