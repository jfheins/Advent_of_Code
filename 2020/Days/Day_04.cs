using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Core;

using MoreLinq;

namespace AoC_2020.Days
{
    public class Day_04 : BaseDay
    {
        //private readonly WrappingXGrid2D<char> _input;
        private readonly List<string> _input;

        public Day_04()
        {
            //_input = new WrappingXGrid2D<char>(File.ReadAllLines(InputFilePath));
            _input = File.ReadAllLines(InputFilePath).Split("")
                .Select(b => string.Join(" ", b)).ToList();
        }

        public override string Solve_1()
        {
            var count = 0;
            var fields = new string[] { "byr",
"iyr",
"eyr",
"hgt",
"hcl",
"ecl",
"pid",
};
            foreach (var line in _input)
            {
                if (fields.All(x => line.Contains(x + ":")))
                {
                    count++;
                }
            }
            return count.ToString();
        }

        public override string Solve_2()
        {

            /*
             byr (Birth Year) - four digits; at least 1920 and at most 2002.
    iyr (Issue Year) - four digits; at least 2010 and at most 2020.
    eyr (Expiration Year) - four digits; at least 2020 and at most 2030.
    hgt (Height) - a number followed by either cm or in:
    If cm, the number must be at least 150 and at most 193.
    If in, the number must be at least 59 and at most 76.
    hcl (Hair Color) - a # followed by exactly six characters 0-9 or a-f.
    ecl (Eye Color) - exactly one of: amb blu brn gry grn hzl oth.
    pid (Passport ID) - a nine-digit number, including leading zeroes.
    cid (Country ID) - ignored, missing or not.
             */
            var count = 0;
            var fields = new string[] { "byr",
                "iyr",
                "eyr",
                "hgt",
                "hcl",
                "ecl",
                "pid",
                };
            foreach (var line in _input)
            {
                var details = line.Split(" ").Select(field => field.Split(":").ToTuple2())
                    .ToDictionary(x => x.Item1, x => x.Item2);

                if (details.TryGetValue("hgt", out var hgt))

                    if (hgt.Contains("cm") && (int.Parse(hgt[..^2]) >= 150 && int.Parse(hgt[..^2]) <= 193)
                         || hgt.Contains("in") && (int.Parse(hgt[..^2]) >= 59 && int.Parse(hgt[..^2]) <= 76)
                                )

                        if (inRange(details, "byr", 1920, 2002))
                            if (details.TryGetValue("iyr", out var iyr) && int.Parse(iyr) >= 2010 && int.Parse(iyr) <= 2020)
                                if (details.TryGetValue("eyr", out var eyr) && int.Parse(eyr) >= 2020 && int.Parse(eyr) <= 2030)
                                    if (details.TryGetValue("hcl", out var hcl) && Regex.IsMatch(hcl, @"#[0-9a-f]{6}"))
                                        if (details.TryGetValue("ecl", out var ecl) && Regex.IsMatch(ecl, @"amb|blu|brn|gry|grn|hzl|oth"))
                                            if (inRange(details, "pid", 0, 999999999) && details.TryGetValue("pid", out var pid) && pid.Length == 9)
                                            {
                                                count++;
                                            }
            }
            return count.ToString();
        }

        private static bool InRange(Dictionary<string, string> details, string key, int min, int max)
        {
            try
            {
                return details.TryGetValue(key, out var val) && int.Parse(val) >= min && int.Parse(val) <= max;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
