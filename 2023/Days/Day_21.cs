using Core;


using MatrixDotNet;
using MatrixDotNet.Extensions.Solver;

using System.Drawing;

namespace AoC_2023.Days;

public sealed partial class Day_21 : BaseDay
{
    private readonly string[] _input;
    private readonly FiniteGrid2D<char> _grid;

    public Day_21()
    {
        _input = File.ReadAllLines(InputFilePath);
        _grid = new FiniteGrid2D<char>(_input);
    }

    public override async ValueTask<string> Solve_1()
    {
        var start = _grid.FindFirst('S');
        var places = new HashSet<Point>() { start };
        for (var i = 0; i < 64; i++)
            places = places.SelectMany(_grid.Get4NeighborsOf).Where(p => _grid[p] != '#').ToHashSet();
        return places.Count.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var start = _grid.FindFirst('S');

        var places = new HashSet<Point>() { start };
        var goalIter = 26501365;
        var points = new List<Point>();
        for (var i = 1; i < 10000; i++)
        {
            places = places.SelectMany(p => p.MoveLURD()).ToHashSet();
            places.RemoveWhere(p => _grid.GetValueWraparound(p) == '#');

            if ((goalIter - i) % _grid.Width == 0)
            {
                points.Add(new(i, places.Count));
                if (points.Count == 3)
                {
                    break;
                }
            }
        }

        // Find the parabola
        Matrix<double> matrix = new double[3, 3];
        SetRow(points[0], 0);
        SetRow(points[1], 1);
        SetRow(points[2], 2);
        var y = points.SelectArray(p => (double)p.Y);
        var polynom = matrix.GaussSolve(y);

        var result = (polynom[0] * goalIter * goalIter)
            + (polynom[1] * goalIter)
            + polynom[2];

        return Math.Round(result).ToString();

        void SetRow(Point p, int r)
        {
            matrix[r, 0] = p.X * p.X;
            matrix[r, 1] = p.X;
            matrix[r, 2] = 1;
        }
    }
}