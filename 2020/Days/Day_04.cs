using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using MoreLinq;

namespace AoC_2020.Days
{
    public class Day_04 : BaseDay
    {
        private readonly List<Dictionary<string, string>> _input;

        public Day_04()
        {
            _input = File.ReadAllLines(InputFilePath).Split("")
                .Select(block => block.SelectMany(line => line.Split(" ")))
                .Select(block => block.Select(item => item.Split(":")))
                .Select(block => block.ToDictionary(kvp => kvp[0], kvp => kvp[1]))
                .ToList();
        }

        public override string Solve_1()
        {
            var fields = new string[] { "byr","iyr","eyr","hgt","hcl","ecl","pid"};
            return _input.Count(passport => fields.All(x => passport.ContainsKey(x))).ToString();
        }

        private static bool IsValid(Dictionary<string, string> currentPassport)
        {
            bool InRange(string key, int min, int max)
                => currentPassport.TryGetValue(key, out var val) && int.Parse(val) >= min && int.Parse(val) <= max;

            bool Fulfills(string key, Func<string, bool> checker)
                => currentPassport.TryGetValue(key, out var val) && checker(val);

            bool FulfillsRegEx(string key, string pattern)
                => Fulfills(key, val => Regex.IsMatch(val, pattern));

            (int val, string unit) GetHeight()
                => currentPassport.TryGetValue("hgt", out var hgt) ? (int.Parse(hgt[..^2]), hgt[^2..]) : (0, "");

            try
            {
                if (InRange("byr", 1920, 2002)
                  && InRange("iyr", 2010, 2020)
                  && InRange("eyr", 2020, 2030)
                  && FulfillsRegEx("hcl", @"#[0-9a-f]{6}")
                  && FulfillsRegEx("ecl", @"amb|blu|brn|gry|grn|hzl|oth")
                  && Fulfills("pid", s => s.Length == 9)
                  && InRange("pid", 0, 999999999))
                {
                    var (val, unit) = GetHeight();
                    if (unit == "cm")
                        return val >= 150 && val <= 193;
                    if (unit == "in")
                        return val >= 59 && val <= 76;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public override string Solve_2()
        {
            return _input.Count(p => IsValid(p)).ToString();
        }
    }
}
