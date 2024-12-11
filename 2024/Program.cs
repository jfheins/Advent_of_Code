using AoC_2024.Days;

using AoCHelper;
using Core;
using Flurl.Http;

namespace AoC_2024;

static class Program
{
    static async Task Main()
    {
        if (File.Exists("cookie.secret"))
        {
            var missingDays = AllDays()
                .Select(className => (dayNumber: className.ParseInts(1).Last(), destPath: $"Inputs/{className}.txt"))
                .Where(t => t.dayNumber > 0)
                .ExceptWhere(t => File.Exists(t.destPath)).ToList();
            if (missingDays.Count > 0)
            {
                var cookie = await File.ReadAllTextAsync("cookie.secret");
                await Task.WhenAll(missingDays.Select(tuple => DownloadDay(tuple, cookie)));
                Console.WriteLine("Downloaded input for days: " + string.Join(" ", missingDays.Select2((n, _) => n)));
            }
        }

        await WarmUp<Day_11>();
        
        await Solver.SolveLast(c => 
        {
            c.ShowConstructorElapsedTime = true;
            c.ShowTotalElapsedTimePerDay = true;
            c.ElapsedTimeFormatSpecifier = "0.0";
            c.ClearConsole = false;
        });
    }

    private static async ValueTask WarmUp<T>() where T:BaseDay, new()
    {
        var instance = new T();
        for (var i = 0; i < 20; i++)
        {
            instance.Clear();
            await instance.Solve_1();
            await instance.Solve_2();
        }

        await Task.Delay(50);
    }

    private static IEnumerable<string> AllDays() => typeof(BaseDay)
        .Assembly.GetTypes()
        .Where(t => t.IsClass && t.IsSubclassOf(typeof(BaseDay)) && !t.IsAbstract)
        .Select(it => it.Name);
    
    private static async Task DownloadDay((int day, string destPath) input, string cookie)
    {
        var url = $"https://adventofcode.com/2024/day/{input.day}/input";
        var res = await url.WithCookie("session", cookie).GetStringAsync();
        await File.WriteAllTextAsync(input.destPath, res);
    }
}
