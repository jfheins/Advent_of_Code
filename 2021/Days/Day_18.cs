using Core.Combinatorics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace AoC_2021.Days
{
    public class Day_18 : BaseDay
    {
        private List<JArray> _input;

        public Day_18()
        {
            _input = File.ReadAllLines(InputFilePath).Select(ParseLine).ToList();
        }

        private JArray ParseLine(string line)
        {
            return JsonConvert.DeserializeObject<JArray>(line)!;
        }

        public override async ValueTask<string> Solve_1()
        {
            var result = _input[0];
            foreach (var line in _input.Skip(1))
            {
                result = Add(result, line);
            }
            var magnitude = CalcMagnitude(result);
            return magnitude.ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var lines = File.ReadAllLines(InputFilePath);
            var newInput = new Variations<string>(lines, 2, GenerateOption.WithoutRepetition);

            var combis = newInput.Select(t => (ParseLine(t[0]), ParseLine(t[1])));
            var mag = combis.Select(t => Add(t.Item1, t.Item2)).ToList();
            var mags = mag.Select(CalcMagnitude)
             .ToList();
            return mags.Max().ToString();
        }

        private int CalcMagnitude(JArray x)
        {
            int left;
            if (x[0] is JArray leftTree)
                left = CalcMagnitude(leftTree);
            else
                left = x[0].Value<int>();

            int right;
            if (x[1] is JArray rightTree)
                right = CalcMagnitude(rightTree);
            else
                right = x[1].Value<int>();
            return 3 * left + 2 * right;
        }

        private JArray Add(JArray a, JArray b)
        {
            return Reduce(new JArray { a, b });
        }

        private JArray Reduce(JArray a)
        {
            var didExplode = true;
            var didSplit = true;

            while (didExplode || didSplit)
            {
                didExplode = Explode(a);
                if (didExplode)
                    continue;
                didSplit = Split(a);
            }
            return a;
        }

        private bool Explode(JArray a)
        {
            var (didExplode, _, _) = Explode(a, 4);
            return didExplode;
        }
        private (bool didExplode, int? forLeft, int? forRight) Explode(JArray a, int level)
        {
            if (level == 0)
            {
                Debug.Assert(a[0] is JValue);
                Debug.Assert(a[1] is JValue);
                return (true, (int)a[0], (int)a[1]);
            }
            else
            {
                var leftSubtree = a[0] as JArray;
                var rightSubtree = a[1] as JArray;
                if (leftSubtree != null)
                {
                    var subresult = Explode(leftSubtree, level - 1);
                    if (subresult.forRight is int addRight) // left exploded
                    {
                        if (subresult.forLeft != null)
                            a[0] = new JValue(0);
                        if (a[1] is JValue x)
                            a[1] = x.Value<int>() + addRight;
                        else if (rightSubtree != null)
                            SubAddLeft(rightSubtree, addRight); // Add it to the leftmost value in the right subtree
                        return (subresult.didExplode, subresult.forLeft, null);
                    }
                    if (subresult.didExplode)
                        return subresult;
                }
                if (rightSubtree != null)
                {
                    var subresult = Explode(rightSubtree, level - 1);
                    if (subresult.forLeft is int addLeft) // right exploded
                    {
                        if (subresult.forRight != null)
                            a[1] = new JValue(0);
                        if (a[0] is JValue x)
                            a[0] = x.Value<int>() + addLeft;
                        else if (leftSubtree != null)
                            SubAddRight(leftSubtree, addLeft); // Add it to the rightmost value of left subtree
                        return (subresult.didExplode, null, subresult.forRight);
                    }
                    if (subresult.didExplode)
                        return subresult;
                }
            }
            return (false, null, null);
        }

        private static void SubAddLeft(JArray number, int value)
        {
            while (number[0] is JArray left)
                number = left;
            number[0] = (int)number[0] + value;
        }

        private static void SubAddRight(JArray number, int value)
        {
            while (number[1] is JArray right)
                number = right;
            number[1] = (int)number[1] + value;
        }

        private bool Split(JArray x)
        {
            var leftSubtree = x[0] as JArray;
            var rightSubtree = x[1] as JArray;
            if (leftSubtree != null)
            {
                var didSplit = Split(leftSubtree);
                if (didSplit) return true;
            }
            else
            {
                var left = x[0].Value<int>();
                if (left >= 10)
                {
                    x[0] = SplitNumber(left);
                    return true;
                }
            }
            if (rightSubtree != null)
            {
                var didSplit = Split(rightSubtree);
                if (didSplit) return true;
            }
            else
            {
                var right = x[1].Value<int>();
                if (right >= 10)
                {
                    x[1] = SplitNumber(right);
                    return true;
                }
            }
            return false;
        }

        private JToken SplitNumber(int number)
        {
            var left = number / 2;
            var right = number - left;
            return new JArray(left, right);
        }
    }
}
