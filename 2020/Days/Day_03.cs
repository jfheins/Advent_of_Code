using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;


using Core;
using Core.Combinatorics;

namespace AoC_2020.Days
{
    public class Day_03 : BaseDay
    {
        private readonly string[] _input;

        public Day_03()
        {
            _input = File.ReadAllLines(InputFilePath);
        }

        public override string Solve_1()
        {
            int treecount = Calc(1, 3);
            return treecount.ToString();
        }

        private int Calc(int dy, int dx)
        {
            var x = 0;
            var maxy = _input.Length - 1;
            var treecount = 0;
            for (int y = 0; y <= maxy; y += dy)
            {
                if (_input[y][x] == '#')
                {
                    treecount++;
                }
                x = (x + dx) % _input[0].Length;
            }

            return treecount;
        }

        public override string Solve_2()
        {
            //            Right 1, down 1.
            //Right 3, down 1. (This is the slope you already checked.)
            //Right 5, down 1.
            //Right 7, down 1.
            //Right 1, down 2.
            var res = new List<long>
            {
                Calc(1, 1),
                Calc(1, 3),
                Calc(1, 5),
                Calc(1, 7),
                Calc(2, 1)
            };
            return res.Product().ToString();
        }
    }
}
