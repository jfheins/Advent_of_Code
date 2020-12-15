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
    public class Day_15 : BaseDay
    {
        private readonly int[] numbers;

        public Day_15()
        {
            numbers = File.ReadAllText(InputFilePath).ParseInts();
        }

        public override string Solve_1()
        {
            var spoken = new List<int>();
            spoken.AddRange(numbers);

            for (int i = numbers.Length; i < 2020; i++)
            {
                var mostrecent = spoken[i - 1];
                var idx = spoken.IndexWhere(x => x == mostrecent).ToList();
                var wasBefore = idx.Count > 1;
                if (wasBefore)
                {
                    spoken.Add(idx[^1] - idx[^2]);
                }
                else
                    spoken.Add(0);
            }
            return spoken.Last().ToString();
        }

        public override string Solve_2()
        {
            var mapNumToTurn = new Dictionary<int, int>(4_000_000);
            var prevNum = -1;

            for (int i = 0; i < numbers.Length; i++)
            {
                mapNumToTurn[numbers[i]] = i;
                prevNum = numbers[i];
            }

            for (int i = numbers.Length; i < 30000000; i++)
            {
                var newNumber = 0;
                if (mapNumToTurn.TryGetValue(prevNum, out var idx))
                    newNumber = i - 1 - idx;
                
                mapNumToTurn[prevNum] = i-1;
                prevNum = newNumber;
            }
            return prevNum.ToString();
        }
    }
}

