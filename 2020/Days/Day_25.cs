using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;

using Core;

namespace AoC_2020.Days
{
    public class Day_25 : BaseDay
    {
        private readonly int[] numbers;

        public Day_25()
        {
            var input = File.ReadAllLines(InputFilePath);
            numbers = input.Select(int.Parse).ToArray();
        }

        public override string Solve_1()
        {
            const int mod = 20201227;

            long Transform(long value, int loops)
            {
                var val = 1L;
                for (int i = 0; i < loops; i++)
                    val = checked(val * value) % mod;
                return val;
            }


            var cardLoops = GetLoops(numbers[0], mod);
            var doorLoops = GetLoops(numbers[1], mod);

            var key = Transform(numbers[0], doorLoops);
            key %= mod;
            //for (int i = 0; i < doorLoops; i++)
            //{
            //    key = (7 * key) % mod;
            //}
            
            return  key.ToString();
        }

        private int GetLoops(int v, int mod)
        {
            const long subject = 7L;
            var val = 1L;
            var loops = 0;
            while (val != v)
            {
                loops++;
                val = checked(val * subject) % mod;
            }
            return loops;
        }

        public override string Solve_2()
        {

            return "_";
        }
    }
}

