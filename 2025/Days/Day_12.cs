using Core;
using System.Drawing;

namespace AoC_2025.Days;

public sealed class Day_12 : BaseDay
{
    private readonly IReadOnlyDictionary<int, Shape> _shapes;
    private readonly IReadOnlyList<TreeRegion> _regions;

    public Day_12()
    {
        var input = File.ReadAllLines(InputFilePath);
        var parts = input.SplitBy("");
        _shapes = parts.SkipLast(1).Select(ParseShape).ToDictionary(s => s.Id);
        _regions = parts.Last().Select(ParseRegion).ToArray();
    }

    private Shape ParseShape(ArraySegment<string> arg)
    {
        var id = arg[0].ParseInts(1).First();
        var grid = new FiniteGrid2D<char>(arg[1..]);
        return new Shape(id, grid);
    }

    private TreeRegion ParseRegion(string arg)
    {
        var x = arg.ParseInts();
        var width = x[0];
        var height = x[1];
        var presents = x.Skip(2).ToArray();
        return new TreeRegion(new Rectangle2D(0, 0, width, height), presents);
    }

    private record Shape(int Id, FiniteGrid2D<char> Outline);

    private record TreeRegion(Rectangle2D Size, int[] Presents);


    public override async ValueTask<string> Solve_1()
    {
        var count = _regions.Count(region => new ShapePacker(_shapes).HasPacking(region));
        return count.ToString();
    }

    public override async ValueTask<string> Solve_2() => "just click";

    class ShapePacker
    {
        private readonly Dictionary<int, List<FiniteGrid2D<char>>> _shapeRotations;
        private bool[,] _grid = new bool[0, 0];

        public ShapePacker(IReadOnlyDictionary<int, Shape> shapes)
        {
            _shapeRotations = shapes.ToDictionary(
                kvp => kvp.Key,
                kvp => GenerateRotations(kvp.Value.Outline)
            );
        }

        public bool HasPacking(TreeRegion region)
        {
            _grid = new bool[region.Size.Height, region.Size.Width];

            // Expand the Presents array into a list of shape IDs
            // e.g., [0, 0, 0, 0, 2, 0] becomes [4, 4] (two copies of shape 4)
            var shapesToPlace = region.Presents
                .Select((count, shapeId) => Enumerable.Repeat(shapeId, count))
                .SelectMany(x => x)
                .ToList();

            // Sort shapes by filled cell count (descending) to fail faster
            // More constrained shapes first leads to earlier pruning
            shapesToPlace = shapesToPlace
                .OrderByDescending(id => CountFilledCells(_shapeRotations[id][0]))
                .ToList();

            return Backtrack(shapesToPlace, 0);
        }

        private static int CountFilledCells(FiniteGrid2D<char> shape)
        {
            return shape.Count(t => t.value != '.');
        }

        private bool Backtrack(List<int> shapeIds, int index)
        {
            if (index == shapeIds.Count)
                return true; // Found valid packing

            // Early termination: check if remaining shapes can fit in remaining space
            var filledCells = CountFilledGridCells();
            var remainingArea = (_grid.GetLength(0) * _grid.GetLength(1)) - filledCells;
            var remainingShapeArea = 0;
            for (int i = index; i < shapeIds.Count; i++)
                remainingShapeArea += CountFilledCells(_shapeRotations[shapeIds[i]][0]);

            if (remainingShapeArea > remainingArea)
                return false; // Not enough space for remaining shapes

            int shapeId = shapeIds[index];

            // Optimization: First shape must be placed at top-left corner (0,0)
            // This fixes the starting position and eliminates symmetric duplicates
            if (index == 0)
            {
                var offset = new Size(0, 0);
                for (int rotation = 0; rotation < _shapeRotations[shapeId].Count; rotation++)
                {
                    var rotatedShape = _shapeRotations[shapeId][rotation];
                    if (CanPlace(rotatedShape, offset))
                    {
                        Place(rotatedShape, offset, true);

                        if (Backtrack(shapeIds, index + 1))
                            return true;

                        Place(rotatedShape, offset, false);
                    }
                }

                return false;
            }

            // Try all valid positions and rotations for subsequent shapes
            for (int y = 0; y <= _grid.GetLength(0) - 3; y++)
            {
                for (int x = 0; x <= _grid.GetLength(1) - 3; x++)
                {
                    var offset = new Size(x, y);

                    for (int rotation = 0; rotation < _shapeRotations[shapeId].Count; rotation++)
                    {
                        var rotatedShape = _shapeRotations[shapeId][rotation];

                        if (CanPlace(rotatedShape, offset))
                        {
                            // Optimization: Must touch existing shapes (8-neighbors including diagonals)
                            if (!TouchesExistingShape(rotatedShape, offset))
                                continue;

                            Place(rotatedShape, offset, true);

                            if (Backtrack(shapeIds, index + 1))
                                return true; // Early exit on first solution

                            Place(rotatedShape, offset, false);
                        }
                    }
                }
            }

            return false;
        }

        private bool TouchesExistingShape(FiniteGrid2D<char> shape, Size offset)
        {
            // Check if any filled cell in the shape is adjacent (8-neighbors including diagonals) to an existing filled cell
            foreach (var (pos, _) in shape.Where(t => t.value != '.'))
            {
                var gridPos = pos + offset;
                // Check all 8 neighbors (including diagonals)
                foreach (var neighbor in gridPos.MoveLURDDiag())
                {
                    if (neighbor.X >= 0 && neighbor.X < _grid.GetLength(1) &&
                        neighbor.Y >= 0 && neighbor.Y < _grid.GetLength(0) &&
                        _grid[neighbor.Y, neighbor.X])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private int CountFilledGridCells()
        {
            int count = 0;
            for (int y = 0; y < _grid.GetLength(0); y++)
            {
                for (int x = 0; x < _grid.GetLength(1); x++)
                    if (_grid[y, x])
                        count++;
            }

            return count;
        }

        private bool CanPlace(FiniteGrid2D<char> shape, Size offset)
        {
            return !shape.Where(t => t.value != '.')
                .Any(t =>
                {
                    var pos = t.pos + offset;
                    return _grid[pos.Y, pos.X];
                });
        }

        private void Place(FiniteGrid2D<char> shape, Size offset, bool place)
        {
            foreach (var (pos, _) in shape.Where(t => t.value != '.'))
            {
                var gridPos = pos + offset;
                _grid[gridPos.Y, gridPos.X] = place;
            }
        }

        private static List<FiniteGrid2D<char>> GenerateRotations(FiniteGrid2D<char> shape)
        {
            // Generate all 8 orientations (4 rotations + 4 flipped rotations)
            // Remove duplicates for symmetric shapes
            var orientations = GetMixed(shape).ToList();

            // Remove duplicate orientations
            var unique = new List<FiniteGrid2D<char>>();
            foreach (var orientation in orientations)
            {
                if (!unique.Any(u => AreEqual(u, orientation)))
                    unique.Add(orientation);
            }

            return unique;
        }

        private static IEnumerable<FiniteGrid2D<char>> GetMixed(FiniteGrid2D<char> grid)
        {
            // 4 rotations
            var dim = grid.Width - 1;
            yield return grid;
            yield return new FiniteGrid2D<char>(grid.Width, grid.Height, (x, y) => grid[dim - y, x]); // rotated 90°
            yield return
                new FiniteGrid2D<char>(grid.Width, grid.Height, (x, y) => grid[dim - x, dim - y]); // rotated 180°
            yield return new FiniteGrid2D<char>(grid.Width, grid.Height, (x, y) => grid[y, dim - x]); // rotated 270°

            // 4 flipped rotations
            var flipped = new FiniteGrid2D<char>(grid.Width, grid.Height, (x, y) => grid[dim - x, y]);
            yield return flipped;
            yield return
                new FiniteGrid2D<char>(flipped.Width, flipped.Height, (x, y) => flipped[dim - y, x]); // rotated 90°
            yield return
                new FiniteGrid2D<char>(flipped.Width, flipped.Height,
                    (x, y) => flipped[dim - x, dim - y]); // rotated 180°
            yield return
                new FiniteGrid2D<char>(flipped.Width, flipped.Height, (x, y) => flipped[y, dim - x]); // rotated 270°
        }

        private static bool AreEqual(FiniteGrid2D<char> a, FiniteGrid2D<char> b)
        {
            if (a.Width != b.Width || a.Height != b.Height) return false;

            for (int y = 0; y < a.Height; y++)
            for (int x = 0; x < a.Width; x++)
                if (a[x, y] != b[x, y])
                    return false;

            return true;
        }
    }
}