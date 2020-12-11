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
    public class Day_00 : BaseDay
    {
        private readonly string[] input;
        private readonly int[] numbers;
        //private FiniteGrid2D<char> grid;

        public Day_00()
        {
            input = File.ReadAllLines(InputFilePath);
            numbers = input.Select(int.Parse).ToArray();
            //grid = Grid2D.FromFile(InputFilePath);
        }

        public override string Solve_1()
        {
            return "_";
        }

        public override string Solve_2()
        {

            return "_";
        }
    }
}

