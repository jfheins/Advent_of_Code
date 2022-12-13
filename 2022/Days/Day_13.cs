using System.Diagnostics;

using Newtonsoft.Json.Linq;

namespace AoC_2022.Days
{
    public sealed class Day_13 : BaseDay
    {
        private readonly string[][] _input;

        public Day_13()
        {
            _input = File.ReadAllLines(InputFilePath).Where(line => line.Length > 0).Chunk(2).ToArray();
            
        }

        public override async ValueTask<string> Solve_1()
        {
            var idx = 1;
            long sumofTrue = 0;
            foreach (var pair in _input)
            {
                var first = ParseLine(pair[0]);
                var second = ParseLine(pair[1]);
                var r = IsInOrder(first, second);

                if (r == true)
                    sumofTrue += idx;
                idx++;
            }
            return sumofTrue.ToString();
        }

        private static bool? IsInOrder(JArray first, JArray second)
        {
            /*
             If both values are integers, the lower integer should come first.
            If the left integer is lower than the right integer, the inputs are
            in the right order. If the left integer is higher than the right integer, 
            the inputs are not in the right order. Otherwise, the inputs are the same
            integer; continue checking the next part of the input.

If both values are lists, compare the first value of each list, then the second value,
            and so on. If the left list runs out of items first, the inputs are in the 
            right order. If the right list runs out of items first, the inputs are not 
            in the right order. If the lists are the same length and no comparison makes
            a decision about the order, continue checking the next part of the input.

If exactly one value is an integer, convert the integer to a list which contains that integer as its only value, then retry the comparison. For example, if comparing [0,0,0] and 2, convert the right value to [2] (a list containing 2); the result is then found by instead comparing [0,0,0] and [2].
             */
            foreach(var pair in first.Zip(second))
            {
                var elemCompare = IsInOrder(pair.Item1, pair.Item2);
                if (elemCompare != null)
                    return elemCompare;
            }
            // Length decides
            if (first.Count != second.Count)
                return first.Count < second.Count;
            return null;
        }

        private static bool? IsInOrder(JToken item1, JToken item2)
        {
            if(item1 is JValue a && item2 is JValue b)
            {
                var x = (long)a.Value!;
                var y = (long)b.Value!;
                return x == y ? null : x < y;
            }
            if(item1 is JArray aa && item2 is JArray bb)
            {
                return IsInOrder(aa, bb);
            }
            var aaa = item1 is JArray ? item1 : new JArray(item1);
            var bbb = item2 is JArray ? item2 : new JArray(item2);
            return IsInOrder(aaa, bbb);
        }

        private static JArray ParseLine(string v)
        {
            Debug.Assert(v[0] == '[' && v.Last() == ']');
            return JArray.Parse(v);
        }

        public override async ValueTask<string> Solve_2()
        {
            var lines = _input.Append(new[] { "[[2]]", "[[6]]" })
                .SelectMany(it => it)
                .Order(new Day13Comparer())
                .ToList();

            var score = lines.IndexOf("[[2]]") + 1;
            score *= lines.IndexOf("[[6]]") + 1;

            return score.ToString();
        }

        private class Day13Comparer : IComparer<string>
        {
            public int Compare(string? x, string? y)
            {
                var first = ParseLine(x);
                var second = ParseLine(y);
                var r = IsInOrder(first, second);
                return r == null ? 0 : (r.Value ? -1 : 1);
            }
        }
    }
}