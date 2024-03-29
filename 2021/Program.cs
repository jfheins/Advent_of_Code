﻿using AoCHelper;

namespace AoC_2021
{
    class Program
    {
        static void Main()
        {
            Solver.SolveAll(new SolverConfiguration
            {
                ShowConstructorElapsedTime = true,
                ShowTotalElapsedTimePerDay = true,
                ElapsedTimeFormatSpecifier = "0.0",
            });
            _ = Console.ReadLine();
        }
    }
}
