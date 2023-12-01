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
        return _input.Select(x => string.Concat(x.Where(char.IsDigit))).Select(x => x[0] + "" + x[^1]).Select(int.Parse).Sum().ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var replaced = _input.SelectList(Replace);
        var values = replaced.Select(x => string.Concat(x.Where(char.IsDigit))).Select(x => x.First() + "" + x.Last()).ToList();
        for (int i = 0; i < replaced.Count; i++)
        {
            Console.WriteLine(values[i] + "\t" + replaced[i]);
        }
        return values.Select(int.Parse).Sum().ToString(); // 55230
    }

    private string Replace(string x)
    {
        var xx = new[] {
           ("one", "1"),
           ("two", "2"),
           ("three", "3"),
           ("four", "4"),
           ("five", "5"),
           ("six", "6"),
           ("seven", "7"),
           ("eight", "8"),
           ("nine", "9") };

        var arr = x.ToCharArray();

        foreach (var number in xx)
        {
            foreach (var idx in AllIndexesOf(x, number.Item1))
            {
                arr[idx + 1] = number.Item2[0];
            }
        }
        return new string(arr);
    }

    public static List<int> AllIndexesOf(string str, string value)
    {
        if (String.IsNullOrEmpty(value))
            throw new ArgumentException("the string to find may not be empty", "value");
        List<int> indexes = [];
        for (int index = 0; ; index += value.Length)
        {
            index = str.IndexOf(value, index);
            if (index == -1)
                return indexes;
            indexes.Add(index);
        }
    }
}