using System.Buffers;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Core;
using Core.Combinatorics;

namespace AoC_2020.Days
{
    public class Day_02 : BaseDay
    {
        private readonly string[] _input;

        public Day_02()
        {
            _input = File.ReadAllLines(InputFilePath);//.Select(int.Parse).ToArray();
        }

        public override string Solve_1()
        {
            var isvalid = 0;
            foreach (var line in _input)
            {
                var reps = Regex.Match(line, @"(\d+)-(\d+) (\w): (\w+)");
                if (reps.Success)
                {
                    var min = int.Parse(reps.Groups[1].Value);
                    var max = int.Parse(reps.Groups[2].Value);
                    var need = reps.Groups[3].Value;
                    var pwd = reps.Groups[4].Value;

                    var count = pwd.Count(x => x == need[0]);
                    if (count >= min && count <= max)
                        isvalid++;
                }
            }
            return isvalid.ToString();
        }

        public override string Solve_2()
        {

            var isvalid = 0;
            foreach (var line in _input)
            {
                var reps = Regex.Match(line, @"(\d+)-(\d+) (\w): (\w+)");
             
                var min = int.Parse(reps.Groups[1].Value);
                var max = int.Parse(reps.Groups[2].Value);
                var need = reps.Groups[3].Value[0];
                var pwd = reps.Groups[4].Value;

                if (pwd[min-1] == need ^ pwd[max-1] == need)
                    isvalid++;
                
            }
            return isvalid.ToString();
        }
    }
}
