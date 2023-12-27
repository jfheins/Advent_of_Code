

using AoC_2023.Days;

using AoCHelper;

using System.Reflection;

namespace AoC_2023;

static class Program
{
    static async Task Main()
    {
        await Warmup();

        await Solver.SolveAll(c =>
        {
            c.ShowConstructorElapsedTime = true;
            c.ShowTotalElapsedTimePerDay = true;
            c.ElapsedTimeFormatSpecifier = "0.000";
            c.ClearConsole = false;
        });
    }

    private static async Task Warmup()
    {
        var days = Assembly.GetExecutingAssembly().GetExportedTypes()
                .Where(type => typeof(BaseDay).IsAssignableFrom(type) && !type.IsAbstract);

        var tasks = days.Select(async day =>
        {
            var d = (BaseDay)Activator.CreateInstance(day)!;
            for (var i = 0; i < 5; i++)
            {
                await d.Solve_1();
                await d.Solve_2();
            }
            await Console.Out.WriteLineAsync($"Warmed up {day}");
        });
        await Task.WhenAll(tasks.ToArray());
    }

    private static async Task Warmup<T>() where T: BaseDay, new()
    {
        var d = new T();
        for (var i = 0; i < 5; i++)
        {
            await d.Solve_1();
            await d.Solve_2();
        }
        await Task.Delay(600);
    }
}
