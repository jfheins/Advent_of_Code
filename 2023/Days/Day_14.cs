using Core;

using NoAlloq;

namespace AoC_2023.Days;

public sealed partial class Day_14 : BaseDay
{
    private readonly string[] _input;

    public Day_14()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var grid = new FiniteGrid2D<char>(_input);
        TiltGrid(grid, Direction.Up);
        Console.Write(grid.ToString());
        return GridWeight(grid).ToString(); // 106454 too high, 106276 wrong
    }

    public override async ValueTask<string> Solve_2()
    {
        return "-";
    }

    private long GridWeight(FiniteGrid2D<char> grid)
    {
        var rowWeights = Enumerable.Range(1, int.MaxValue);
        return grid.GetRowIndices().Reverse().Zip(rowWeights).Sum(SumRow);

        long SumRow((int y, int weight) it) => it.weight * grid.GetRow(it.y, '.').Count('O');
    }

    private static bool TiltGrid(FiniteGrid2D<char> grid, Direction dir)
    {
        var hasChanged = false;
        var reverse = dir == Direction.Down || dir == Direction.Right;

        if(Directions.Vertical.Contains(dir))
        {
            foreach (var x in grid.GetColIndices())
            {
                var chunk = grid.GetCol(x, '.').ToArray();
                Move(chunk);
                grid.SetCol(x, chunk);
            }
        }
        else
        {
            foreach (var x in grid.GetRowIndices())
            {
                var chunk = grid.GetRow(x, '.').ToArray();
                Move(chunk);
                grid.SetRow(x, chunk);
            }
        }
        return hasChanged;

        void Move(char[] arr)
        {
            if (reverse)
               Array.Reverse(arr);
            hasChanged |= MoveBallsToFront(arr);
            if (reverse)
                Array.Reverse(arr);
        }
    }

    private static bool MoveBallsToFront(Span<char> remainder)
    {
        var hasChanged = false;
        var fragmentEnd = FindBorder(remainder);
        while (fragmentEnd > -1)
        {
            Sort(remainder[..fragmentEnd]);
            if(fragmentEnd < remainder.Length)
                remainder = remainder[(fragmentEnd + 1)..];
            else
                break;
            fragmentEnd = FindBorder(remainder);
        }
        return hasChanged; 

        int FindBorder(ReadOnlySpan<char> it)
        {
            var res = it.IndexOf('#');
            return res == -1 ? it.Length : res;
        } 

        void Sort(Span<char> arr)
        {
            int dest;
            int src = -1;
            while (Search(arr))
            {
                arr[dest] = 'O';
                arr[src] = '.';
                arr = arr[(dest + 1)..];
                hasChanged = true;
            }

            bool Search(ReadOnlySpan<char> l)
            {
                dest = l.IndexOf('.');
                if (dest == -1)
                    return false;
                src = l.IndexOf('O', dest);
                return dest >= 0 && src > dest;
            }
        }
    }
}