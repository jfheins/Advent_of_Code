using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AoC_2020.Days
{
#if use_regex
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
#else
    public class Day_18 : BaseDay
    {
        private readonly ImmutableList<Token>[] termList;

        public Day_18()
        {
            var input = File.ReadAllLines(InputFilePath);
            termList = input.Select(Tokenize).ToArray();
        }

        private ImmutableList<Token> Tokenize(string term)
        {
            var builder = ImmutableList.CreateBuilder<Token>();
            foreach (var chr in term.Replace(" ", ""))
            {
                builder.Add(chr switch
                {
                    '(' => new ParensOpenToken(),
                    ')' => new ParensCloseToken(),
                    '+' => new PlusToken(),
                    '*' => new StarToken(),
                    _ => new NumberToken(chr)
                });
            }
            return builder.ToImmutable();
        }

        public override string Solve_1()
        {
            return termList.Select(term => EvalLeftToRight(term).Value).Sum().ToString();
        }

        public override string Solve_2()
        {
            return termList.Select(term => EvalAddBeforeMul(term).Value).Sum().ToString();
        }

        private NumberToken EvalLeftToRight(ImmutableList<Token> term)
        {

            (int start, int end)? parensGroup;
            while ((parensGroup = DeepestParenthesis(term)) != null)
            {
                var (start, end) = parensGroup.Value;
                var result = EvalParenthesis(term.GetRange(start, end - start + 1), EvalLeftToRight);
                term = term.RemoveRange(start, end - start + 1).Insert(start, result);
            }

            while (term.Count >= 3)
            {
                var result = EvalOperator(term);
                term = term.RemoveRange(0, 3).Insert(0, result);
            }

            Debug.Assert(term.First() is NumberToken);
            return (NumberToken)term.First();
        }

        private NumberToken EvalAddBeforeMul(ImmutableList<Token> term)
        {

            (int start, int end)? parensGroup;
            while ((parensGroup = DeepestParenthesis(term)) != null)
            {
                var (start, end) = parensGroup.Value;
                var result = EvalParenthesis(term.GetRange(start, end - start + 1), EvalAddBeforeMul);
                term = term.RemoveRange(start, end - start + 1).Insert(start, result);
            }

            var index = -1;
            while ((index = term.FindIndex(t => t is PlusToken)) > -1)
            {
                var result = EvalOperator(term.GetRange(index - 1, 3));
                term = term.RemoveRange(index - 1, 3).Insert(index - 1, result);
            }
            while ((index = term.FindIndex(t => t is StarToken)) > -1)
            {
                var result = EvalOperator(term.GetRange(index - 1, 3));
                term = term.RemoveRange(index - 1, 3).Insert(index - 1, result);
            }

            Debug.Assert(term.First() is NumberToken);
            return (NumberToken)term.First();
        }

        private (int start, int end)? DeepestParenthesis(ImmutableList<Token> term)
        {
            var end = term.FindIndex(t => t is ParensCloseToken);
            if (end == -1)
                return null;
            else
                return (term.FindLastIndex(end, t => t is ParensOpenToken), end);
        }

        private NumberToken EvalParenthesis(ImmutableList<Token> term, Func<ImmutableList<Token>, NumberToken> evaluator)
        {
            Debug.Assert(term[0] is ParensOpenToken);
            Debug.Assert(term[^1] is ParensCloseToken);
            return evaluator(term.GetRange(1, term.Count - 2));
        }

        private NumberToken EvalOperator(ImmutableList<Token> term)
        {
            var left = term[0] as NumberToken;
            var right = term[2] as NumberToken;
            Debug.Assert(left != null);
            Debug.Assert(term[1] is PlusToken || term[1] is StarToken);
            Debug.Assert(right != null);
            return term[1] is PlusToken
                ? new NumberToken(left.Value + right.Value)
                : new NumberToken(left.Value * right.Value);
        }

        [DebuggerDisplay("Token {Content}")]
        private abstract record Token(string Content);
        private abstract record ParensToken(string Content) : Token(Content);
        private record ParensOpenToken() : ParensToken("(");
        private record ParensCloseToken() : ParensToken(")");
        private record PlusToken() : Token("+");
        private record StarToken() : Token("*");
        private record NumberToken : Token
        {
            public long Value { get; }
            public NumberToken(char digit) : base(digit.ToString())
            {
                Value = long.Parse(Content);
            }
            public NumberToken(long value) : base(value.ToString())
            {
                Value = value;
            }
        }
    }
#endif
}

