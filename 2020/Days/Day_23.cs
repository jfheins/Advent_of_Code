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

            var cardCount = 1_000_000;
            var nodeMap = new (int value, int next)[cardCount];


            foreach (var pair in input.Concat(Enumerable.Range(10, cardCount - input.Count + 1)).Select(x => x - 1).PairwiseWithOverlap())
            {
                nodeMap[pair.Item1] = pair;
            }
            nodeMap[cardCount - 1] = (cardCount - 1, input[0] - 1);

            (int value, int next)[] GetNextThree((int value, int next) item)
            {
                var res = new (int value, int next)[3];
                for (int i = 0; i < res.Length; i++)
                {
                    var nextIdx = item.next % cardCount;
                    item = res[i] = nodeMap[nextIdx];
                }
                return res;
            }

            void Print((int value, int next) item)
            {
                Console.Write($"({item.value + 1}) ");
                var pointer = item;
                for (int j = 0; j < 7; j++)
                {
                    pointer = nodeMap[pointer.next];
                    Console.Write($"{pointer.value + 1} ");
                }
                Console.WriteLine();
            }

            var current = nodeMap[input[0] - 1];
            for (int i = 0; i < 10_000_000; i++)
            {
                // Output
                //Print(current);

                var picked = GetNextThree(current);
                current.next = picked[^1].next; // remove them from linked list
                nodeMap[current.value] = current;
                //Print(current);

                var dest = (current.value - 1 + cardCount) % cardCount;
                while (picked.Any(p => p.value == dest))
                    dest = (dest + cardCount - 1) % cardCount;

                nodeMap[picked[^1].value].next = nodeMap[dest].next;
                nodeMap[dest].next = picked[0].value;
                //Print(current);

                current = nodeMap[current.next];
            }

            var oneNode = nodeMap[0];
            var next = GetNextThree(oneNode);
            var final = (next[0].value + 1L) * (next[1].value + 1L);

            return final.ToString();
        }


    }
}

