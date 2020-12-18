using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Core;

namespace AoC_2020.Days
{
    public class Day_18 : BaseDay
    {
        private readonly string[] input;

        public Day_18()
        {
            input = File.ReadAllLines(InputFilePath);
        }

        public override string Solve_1()
        {
            return input
                .Select(line => long.Parse(
                    EvalLeftToRight(line)))
                .Sum().ToString();
        }


        public override string Solve_2()
        {
            return input
                .Select(line => long.Parse(
                    EvalAddBeforeMul(line)))
                .Sum().ToString();
        }

        private string EvalLeftToRight(string expression)
            => Eval(expression, new Regex(@"^(-?\d+) ([+*]) (-?\d+)"));

        private string EvalAddBeforeMul(string expression)
        {
            return Eval(expression,
                new Regex(@"(-?\d+) ([+]) (-?\d+)"),
                new Regex(@"(-?\d+) ([*]) (-?\d+)"));
        }

        private string Eval(string expression, params Regex[] operationsInOrder)
        {
            string EvalParenthesis(Match match) => Eval(match.Value[1..^1], operationsInOrder);

            var parens = new Regex(@"\([^()]+\)");
            while (parens.IsMatch(expression))
                expression = parens.Replace(expression, EvalParenthesis);

            foreach (var regex in operationsInOrder)
            {
                while (regex.IsMatch(expression))
                {
                    expression = regex.Replace(expression, EvalSingleOperation);
                }
            }
            return expression;
        }

        string EvalSingleOperation(Match match)
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
    }
}

