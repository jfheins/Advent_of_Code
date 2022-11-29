using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            var cupCount = 1_000_000;
            var successorOf = new int[cupCount];

            foreach (var pair in input.Concat(Enumerable.Range(10, cupCount - input.Count + 1)).Select(x => x - 1).PairwiseWithOverlap())
            {
                successorOf[pair.Item1] = pair.Item2;
            }
            successorOf[cupCount - 1] = input[0] - 1;

            int[] GetNextThree(int cup)
            {
                var res = new int[3];
                for (int i = 0; i < res.Length; i++)
                {
                    cup = res[i] = successorOf[cup] % cupCount;
                }
                return res;
            }

            var currentCup = input[0] - 1;
            for (int i = 0; i < 10_000_000; i++)
            {
                var picked = GetNextThree(currentCup);
                // remove them from linked list
                successorOf[currentCup] = successorOf[picked[^1]];

                var dest = (currentCup - 1 + cupCount) % cupCount;
                while (Array.IndexOf(picked, dest) > -1)
                    dest = (dest + cupCount - 1) % cupCount;

                successorOf[picked[^1]] = successorOf[dest];
                successorOf[dest] = picked[0];

                currentCup = successorOf[currentCup];
            }

            var next = GetNextThree(0);
            var final = (next[0] + 1L) * (next[1] + 1L);

            return final.ToString();
        }


    }
}

