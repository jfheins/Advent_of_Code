using Core;
using static MoreLinq.Extensions.SplitExtension;

namespace AoC_2025.Days;

public sealed class Day_06 : BaseDay
{
    private readonly string[] _input;

    public Day_06()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var problems = _input.SelectArray(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries))
            .ZipMany(Problem.FromRowNumbers);
        return problems.Sum(it => it.Calculate()).ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var problems = _input.ZipMany(col => new string(col)).Split(it => it.IsWhiteSpace())
            .SelectList(Problem.FromColumnNumbers);
        return problems.Sum(it => it.Calculate()).ToString();
    }

    public record Problem(char Operator, IReadOnlyCollection<long> Operands)
    {
        public long Calculate() => Operator switch
        {
            '+' => Operands.Sum(),
            '*' => Operands.Product(),
            _ => throw new InvalidOperationException("Unknown operation")
        };

        public static Problem FromRowNumbers(IReadOnlyCollection<string> cells)
        {
            var op = cells.Last()[0];
            var operands = cells.SkipLast(1).SelectList(long.Parse);
            return new Problem(op, operands);
        }

        public static Problem FromColumnNumbers(IEnumerable<string> columns)
        {
            var cols = columns.ToArray();
            var op = cols[0].Last();
            var operands = cols.SelectArray(it => long.Parse(it.AsSpan()[..^1]));
            return new Problem(op, operands);
        }
    }
}