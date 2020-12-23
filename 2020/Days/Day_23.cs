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
    public class Day_23 : BaseDay
    {
        private readonly List<int> input;
        //private FiniteGrid2D<char> grid;

        public Day_23()
        {
            input = File.ReadAllText(InputFilePath).Select(c => c - '0').ToList();
        }

        public override string Solve_1()
        {
            var numbers = input.ToList();
            var minmax = numbers.MinMax().Value;

            for (int i = 0; i < 100; i++)
            {
                var curr = numbers[0];
                var picked = numbers.Skip(1).Take(3).ToList();
                numbers.RemoveRange(1, 3);

                var destidx = -1;
                var needle = curr;
                while (destidx == -1)
                {
                    needle = needle == 1 ? minmax.max : needle - 1;
                    destidx = numbers.FindIndex(x => x == needle);
                }
                numbers.Insert(destidx + 1, picked[2]);
                numbers.Insert(destidx + 1, picked[1]);
                numbers.Insert(destidx + 1, picked[0]);
                var first = numbers[0];
                numbers.RemoveAt(0);
                numbers.Add(first);
            }

            var oneidx = numbers.FindIndex(x => x == 1);
            var final = string.Concat(numbers.Skip(oneidx + 1).Concat(numbers.Take(oneidx)).Select(i => i.ToString()));

            return final;
        }

        public override string Solve_2()
        {
            var numbers = new LinkedList<int>(input.Concat(Enumerable.Range(10, 999991)));
            var max = 1_000_000;
            var nodeMap = new LinkedListNode<int>[max+1];
            var current = numbers.First;
            while (current != null)
            {
                nodeMap[current.Value] = current;
                current = current.Next;
            }

            current = numbers.First!;
            for (int i = 0; i < 10_000_000; i++)
            {
                var picked = GetNextThree(current);
                foreach (var node in picked)
                    numbers.Remove(node);
                
                var dest = current.Value == 1 ? max : current.Value - 1;
                while (picked.Any(p => p.Value == dest))
                    dest = dest == 1 ? max : dest - 1;

                var destNode = nodeMap[dest];

                numbers.AddAfter(destNode, picked[2]);
                numbers.AddAfter(destNode, picked[1]);
                numbers.AddAfter(destNode, picked[0]);
                current = current.Next ?? numbers.First!;
            }

            var oneNode = numbers.Find(1)!;
            var next = GetNextThree(oneNode);
            var final = next[0].Value * (long)next[1].Value;

            return final.ToString();
        }

        private LinkedListNode<int>[] GetNextThree(LinkedListNode<int> x)
        {
            var res = new LinkedListNode<int>[3];
            for (int i = 0; i < res.Length; i++)
            {
                x = x.Next ?? x.List!.First!;
                res[i] = x;
            }
            return res;
        }
    }
}

