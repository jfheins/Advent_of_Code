using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Core;

namespace AoC_2020.Days
{
    public class Day_18 : BaseDay
    {
        private readonly string[] input;
        //private readonly int[] numbers;
        //private FiniteGrid2D<char> grid;

        public Day_18()
        {
            input = File.ReadAllLines(InputFilePath);
            //numbers = input.Select(int.Parse).ToArray();
            //grid = Grid2D.FromFile(InputFilePath);
        }

        public override string Solve_1()
        {
            var sum = 0L;
            foreach (var line in input)
            {
                sum += long.Parse(Eval(line));
            }
            
            return sum.ToString();
        }

        string EvalMatch(Match match) => Eval(match.Value[1..^1]);
        string EvalBaseMatch(Match match)
        {
            var operands = match.Value.ParseLongs();
            var op = match.Groups[2].Value;
            return op switch
            {
                "+" => operands.Sum().ToString(),
                "*" => operands.Product().ToString(),
                _ => "x"
            };
        }

        private string Eval(string expression)
        {
            var parens = new Regex(@"\([^()]+\)");
            var match = parens.Match(expression);
            while (parens.IsMatch(expression))
            {
                expression = parens.Replace(expression, EvalMatch);
            }
            var op = new Regex(@"(-?\d+) ([+]) (-?\d+)");

            while (op.IsMatch(expression))
            {
                expression = op.Replace(expression, EvalBaseMatch);
            }

            op = new Regex(@"(-?\d+) ([*]) (-?\d+)");

            while (op.IsMatch(expression))
            {
                expression = op.Replace(expression, EvalBaseMatch);
            }
            return expression;
        }


        public override string Solve_2()
        {

            return "_";
        }
    }
}

