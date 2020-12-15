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
        private readonly string input;
        private readonly int[] numbers;
        //private FiniteGrid2D<char> grid;

        public Day_15()
        {
            input = File.ReadAllText(InputFilePath);
            numbers = input.ParseInts();
            //grid = Grid2D.FromFile(InputFilePath);
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
            var spoken = new List<int>(30000000);
            spoken.AddRange(numbers);

            var mapNumToTurn = new Dictionary<int, List<int>>();

            for (int i = 0; i < spoken.Count; i++)
            {
                var list = mapNumToTurn.GetOrAdd(spoken[i], new List<int>());
                list.Add(i);
            }

            for (int i = numbers.Length; i < 30000000; i++)
            {
                var mostrecent = spoken[i - 1];
                var idx = mapNumToTurn.GetValueOrDefault(mostrecent, new List<int>());
                var wasBefore = idx.Count > 1;
                if (wasBefore)
                {
                    var newNumber = idx[^1] - idx[^2];
                    spoken.Add(newNumber);
                    var list = mapNumToTurn.GetOrAdd(newNumber, new List<int>());
                    list.Add(i);
                }
                else
                {
                    spoken.Add(0);
                    var list = mapNumToTurn.GetOrAdd(0, new List<int>());
                    list.Add(i);
                }
            }
            return spoken.Last().ToString();
        }
    }
}

