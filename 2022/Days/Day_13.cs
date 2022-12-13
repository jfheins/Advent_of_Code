using System.Diagnostics;
using System.Text.Json.Nodes;
using Core;

namespace AoC_2022.Days
{
    public sealed class Day_13 : BaseDay
    {
        private readonly IReadOnlyList<JsonNode> _input;

        public Day_13()
        {
            _input = File.ReadAllLines(InputFilePath)
                .Where(line => line.Length > 1)
                .SelectList(it => JsonNode.Parse(it))!;
        }

        public override async ValueTask<string> Solve_1()
        {
            var pairs = _input.Chunk(2).Select(pair => Comparer.Compare(pair[0], pair[1]));
            return pairs.IndexWhere(it => it < 0).Sum(it => it+1).ToString();
        }
        
        public override async ValueTask<string> Solve_2()
        {
            var dividers = new[] { JsonNode.Parse("[[2]]"), JsonNode.Parse("[[6]]") };
            var sorted = _input.Concat(dividers).Order(Comparer).ToList();
            return dividers.Select(it => sorted.IndexOf(it) + 1).Product().ToString();
        }

        private static readonly IComparer<JsonNode?> Comparer = new Day13Comparer();
        private class Day13Comparer : IComparer<JsonNode?>
        {
            public int Compare(JsonNode? a, JsonNode? b)
            {
                Debug.Assert(a != null && b != null);
                return (a, b) switch
                {
                    (JsonValue val1, JsonValue val2) => val1.GetValue<int>().CompareTo(val2.GetValue<int>()),
                    (JsonArray arr1, JsonArray arr2) => Compare(arr1, arr2),
                    (_, _) => Compare(AsArray(a), AsArray(b))
                };
                JsonArray AsArray(JsonNode x) => x as JsonArray ?? new JsonArray(x.GetValue<int>());
            }

            private int Compare(JsonArray a, JsonArray b)
            {
                var elementComparisons = a.Zip(b).Select(t => Compare(t.First, t.Second)).FirstOrDefault(it => it != 0);
                return elementComparisons != 0
                    ? elementComparisons
                    : a.Count - b.Count;
            }
        }
    }
}