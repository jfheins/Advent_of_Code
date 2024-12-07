using System.Collections.ObjectModel;
using Core;
using Spectre.Console;
using System.Drawing;
using Core.Combinatorics;

namespace AoC_2024.Days;

public sealed partial class Day_07 : BaseDay
{
    private readonly string[] _input;

    public Day_07()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        long result = 0;
        foreach (var line in _input)
        {
            var eq = line.ParseLongs();
            var res = eq[0];
            var places = eq.Length - 2;
            var op = new char[places];
            for (int i = 0; i < (1 << places); i++)
            {
                for (int j = 0; j < places; j++)
                {
                    op[j] = (i & (1 << j)) != 0 ? '+' : '*';
                }
                if (Eval(eq.AsSpan(1), op) == res)
                {
                    result += res;
                    break;
                }
            }
        }
        
        return result.ToString();
    }

    private long Eval(Span<long> operands, IList<char> operators)
    {
        var res = operands[0];
        for (var i = 0; i < operators.Count; i++)
        {
            var op = operators[i];
            var operand = operands[i + 1];
            switch (op)
            {
                case '+':
                    res += operand;
                    break;
                case '*':
                    res *= operand;
                    break;
            }
        }

        return res;
    }

    public override async ValueTask<string> Solve_2()
    {
        long result = 0;
        foreach (var line in _input)
        {
            var eq = line.ParseLongs();
            var res = eq[0];
            var places = eq.Length - 2;
            for (int i = 0; i < (int)Math.Pow(3, places); i++)
            {
                var se = Int32ToString(i, 3).PadLeft(places, '0');
                if (Eval(eq.AsSpan(1), se) == res)
                {
                    result += res;
                    break;
                }
            }
        }
        
        return result.ToString(); // not 574911920703362
    }
    
    public static string Int32ToString(long value, int toBase)
    {
        string result = string.Empty;
        do
        {
            result = "0123456789ABCDEF"[(int)(value % toBase)] + result;
            value /= toBase;
        }
        while (value > 0);

        return result;
    }

    private long Eval(Span<long> operands, string operators)
    {
        if (operands[0] == 6)
            ;
        var res = operands[0];
        for (var i = 0; i < operators.Length; i++)
        {
            var op = operators[i];
            var operand = operands[i + 1];
            switch (op)
            {
                case '0':
                    res += operand;
                    break;
                case '1':
                    res *= operand;
                    break;
                case '2':
                    res = long.Parse(res.ToString() + operand);
                    break;
            }
        }

        return res;
    }
}