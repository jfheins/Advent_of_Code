using Core;
using System.Linq;
using System.Drawing;

namespace AoC_2025.Days;

public sealed partial class Day_06 : BaseDay
{
    private readonly string[] _input;

    public Day_06()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var input = _input.SelectArray(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries));
        var total = 0L;
        for (int i = 0; i < input[0].Length; i++)
        {
            var x = input.SelectArray(l => l[i]);
            var op = x.Last();
            var num = x.SkipLast(1).Select(long.Parse).ToArray();
            if (op == "+")
                total += num.Sum();
            else if (op == "*")
            {
                total += num.Product();
            }
            else
            {
                throw new Exception();
            }
        }

        return total.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var total = 0L;
        var op = 'x';
        List<long> problem = new();
        for (int colIdx = 0; colIdx < _input[0].Length; colIdx++)
        {
            var col = _input.SelectArray(l => l[colIdx]);
            if (col.Last() != ' ')
                op = col.Last();

            var operand = new string(col.SkipLast(1).ToArray());

            if (string.IsNullOrWhiteSpace(operand))
            {
                // perform op
                if (op == '+')
                    total += problem.Sum();
                else if (op == '*')
                {
                    total += problem.Product();
                }
                else
                {
                    throw new Exception();
                }

                problem.Clear();
            }
            else
            {
                // add operand
                var num = long.Parse(operand);
                problem.Add(num);
            }
        }

        if (problem.Count > 0)
        {
            if (op == '+')
                total += problem.Sum();
            else if (op == '*')
            {
                total += problem.Product();
            }
            else
            {
                throw new Exception();
            }

            problem.Clear();
        }

        return total.ToString();
    }
}