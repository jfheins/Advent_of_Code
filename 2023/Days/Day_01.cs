using Core;
using static MoreLinq.Extensions.SplitExtension;
using static MoreLinq.Extensions.PartialSortByExtension;
using System.Xml;

namespace AoC_2023.Days;

public sealed class Day_01 : BaseDay
{
    private string[] _input;

    public Day_01()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        return _input.Select(CalibrationValue).Sum().ToString();

        static int CalibrationValue(string str) => ExtractFirstAndLastDigit(str);
    }

    public override async ValueTask<string> Solve_2()
    {
        return _input.Select(CalibrationValue).Sum().ToString();

        static int CalibrationValue(string str)
        {
            var replaced = str
                .Replace("one", "o1e")
                .Replace("two", "t2o")
                .Replace("three", "t3ree")
                .Replace("four", "f4ur")
                .Replace("five", "f5ve")
                .Replace("six", "s6x")
                .Replace("seven", "s7ven")
                .Replace("eight", "e8ght")
                .Replace("nine", "n9ne");
            return ExtractFirstAndLastDigit(replaced);
        }
    }

    private static int ExtractFirstAndLastDigit(string str)
    {
        var (first, last) = str.Where(char.IsDigit).FirstLast();
        return int.Parse([first, last]);
    }
}