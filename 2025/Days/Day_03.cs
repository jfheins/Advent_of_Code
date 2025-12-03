namespace AoC_2025.Days;

public sealed class Day_03 : BaseDay
{
    private readonly string[] _input;

    public Day_03()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var total = _input.Sum(bank => long.Parse(MaxJoltage(bank, 2)));
        return total.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var total = _input.Sum(bank => long.Parse(MaxJoltage(bank, 12)));
        return total.ToString();
    }

    private static string MaxJoltage(ReadOnlySpan<char> bank, int batteryCount)
    {
        if (batteryCount == 0)
            return "";
        
        for (var d = '9'; d >= '0'; d--)
        {
            var digitIdx = bank.IndexOf(d);
            var remainingLength = bank.Length - digitIdx;
            if (digitIdx < 0 || remainingLength < batteryCount) 
                continue;
            
            var remainder = bank.Slice(digitIdx + 1);
            return d + MaxJoltage(remainder, batteryCount - 1);
        }

        return "xxx";
    }
}