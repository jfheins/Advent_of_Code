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
        var total = _input.Sum(bank => long.Parse(MaxJoltage(bank, batteryCount: 2)));
        return total.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var total = _input.Sum(bank => long.Parse(MaxJoltage(bank, batteryCount: 12)));
        return total.ToString();
    }

    /// <summary>
    /// Finds the maximum number that can be formed by selecting 'batteryCount' digits
    /// from the bank in order (greedy approach: always pick the largest available digit).
    /// </summary>
    private static string MaxJoltage(ReadOnlySpan<char> bank, int batteryCount)
    {
        if (batteryCount == 0)
            return string.Empty;
        
        // Greedy algorithm: try to pick the largest digit ('9' to '0')
        // that still leaves enough characters to select all remaining digits
        for (var digit = '9'; digit >= '0'; digit--)
        {
            var digitIndex = bank.IndexOf(digit);
            if (digitIndex < 0)
                continue;
            
            // Check if we have enough characters from this position onwards (including this digit)
            // We need at least 'batteryCount' characters total
            var remainingLength = bank.Length - digitIndex;
            if (remainingLength < batteryCount)
                continue;
            
            // Found a valid digit, recurse for the rest
            var remainingBank = bank.Slice(digitIndex + 1);
            return digit + MaxJoltage(remainingBank, batteryCount - 1);
        }

        // This should only be reached if the bank contains invalid characters
        throw new InvalidOperationException(
            $"Unable to find {batteryCount} valid digits in the remaining bank '{bank.ToString()}'");
    }
}