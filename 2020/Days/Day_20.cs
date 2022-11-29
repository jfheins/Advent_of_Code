using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core;
using static MoreLinq.Extensions.ToDictionaryExtension;

namespace AoC_2020.Days
{
    public class Day_20 : BaseDay
    {
        private Dictionary<int, FiniteGrid2D<char>> tiles = new();
        private Dictionary<int, List<char[]>> edges;
        private int cornerid;

        public Day_20()
        {
            var input = File.ReadAllLines(InputFilePath);
            for (int i = 0; i < input.Length; i += 12)
            {
                var id = input[i].ParseInts(1)[0];
                tiles.Add(id, Grid2D.FromArray(input, (i + 1)..(i + 11), 0..10));
            }
        }

        public override string Solve_1()
        {
            var result = new int[12, 12];

            edges = new Dictionary<int, List<char[]>>();

            foreach (var tile in tiles)
            {
                edges[tile.Key] = new List<char[]>();
                foreach (var dir in Directions.All4)
                {
                    edges[tile.Key].Add(tile.Value.GetEdge(dir).ToArray());
                    edges[tile.Key].Add(tile.Value.GetEdge(dir).Reverse().ToArray());
                }
            }

            var prod = 1L;
            foreach (var tile in tiles)
            {
                var tilesEdges = edges[tile.Key];
                var matchingEdges = edges.Count(other => ShareEdge(tilesEdges, other.Value));
                if (matchingEdges == 3)
                {
                    prod *= tile.Key;
                    cornerid = tile.Key;
                }

                Debug.Assert(matchingEdges <= 5);
            }

            return prod.ToString();
        }

        private static bool ShareEdge(List<char[]> a, List<char[]> b)
        {
            return a.CartesianProduct(b).Any(t => t.Item1.SequenceEqual(t.Item2));
        }

        private static List<(bool flipped, Direction dir, char[] edge)> GetEdgesWithFlips(FiniteGrid2D<char> grid,
            Direction[] dirs)
        {
            var edges = dirs.Select(d => (false, d, grid.GetEdge(d).ToArray()));
            return edges.Concat(edges.Select(e => (flipped: true, e.d, e.Item3.Reverse().ToArray()))).ToList();
        }

        private static IEnumerable<FiniteGrid2D<char>> GetMixed(FiniteGrid2D<char> grid)
        {
            // 4 rotations
            var dim = grid.Width - 1;
            yield return grid;
            yield return new FiniteGrid2D<char>(grid.Width, grid.Height, (x, y) => grid[dim - y, x]); // rotated
            yield return
                new FiniteGrid2D<char>(grid.Width, grid.Height, (x, y) => grid[dim - x, dim - y]); // rotated twice
            yield return
                new FiniteGrid2D<char>(grid.Width, grid.Height, (x, y) => grid[y, dim - x]); // rotated three times
            // 4 flipped rotations
            var flipped = new FiniteGrid2D<char>(grid.Width, grid.Height, (x, y) => grid[dim - x, y]);
            yield return flipped;
            yield return
                new FiniteGrid2D<char>(flipped.Width, flipped.Height, (x, y) => flipped[dim - y, x]); // rotated
            yield return
                new FiniteGrid2D<char>(flipped.Width, flipped.Height,
                    (x, y) => flipped[dim - x, dim - y]); // rotated twice
            yield return
                new FiniteGrid2D<char>(flipped.Width, flipped.Height,
                    (x, y) => flipped[y, dim - x]); // rotated three times
        }

        private KeyValuePair<int, List<char[]>> GetNeighbor(int exclude, List<char[]> boundary)
        {
            return edges
                .Where(e => e.Key != exclude)
                .Where(e => !placed.Contains(e.Key)).Single(e => ShareEdge(e.Value, boundary));
        }

        private KeyValuePair<int, List<char[]>> GetNeighbor(int exclude, List<char[]> boundary1, List<char[]> boundary2)
        {
            return edges
                .Where(e => e.Key != exclude)
                .Where(e => !placed.Contains(e.Key))
                .Single(e => ShareEdge(e.Value, boundary1) && ShareEdge(e.Value, boundary2));
        }

        private IList<KeyValuePair<int, List<char[]>>> GetNeighbors(int exclude, List<char[]> boundary)
        {
            return edges
                .Where(e => e.Key != exclude)
                .Where(e => !placed.Contains(e.Key))
                .Where(e => ShareEdge(e.Value, boundary)).ToList();
        }

        private IList<KeyValuePair<int, List<char[]>>> GetNeighbors(int exclude, List<char[]> boundary1,
            List<char[]> boundary2)
        {
            return edges
                .Where(e => e.Key != exclude)
                .Where(e => !placed.Contains(e.Key))
                .Where(e => ShareEdge(e.Value, boundary1) && ShareEdge(e.Value, boundary2)).ToList();
        }

        private int[,] field = new int[12, 12];
        private HashSet<int> placed = new();

        private Dictionary<(int x, int y), FiniteGrid2D<char>> orientedTiles = new();

        public override string Solve_2()
        {
            var dim = 12;

            var topLeft = edges[cornerid];
            var neighbors = edges
                .Where(e => e.Key != cornerid).Where(e => ShareEdge(e.Value, topLeft)).ToList();

            // First line
            field[0, 0] = cornerid;
            placed.Add(cornerid);

            var current = GetEdgesWithFlips(tiles[cornerid], Directions.Horizontal);
            for (int i = 1; i < dim; i++)
            {
                var rightN = neighbors.Where(n => n.Value.Any(ne => current.Any(te => te.edge.SequenceEqual(ne))))
                    .MinBy(n => GetNeighbors(n.Key, n.Value).Count)!;

                if (!placed.Add(rightN.Key))
                    break;
                field[i, 0] = rightN.Key;

                current = GetEdgesWithFlips(tiles[rightN.Key], Directions.All4);
                neighbors = edges
                    .Where(e => !placed.Contains(e.Key)).Where(e => ShareEdge(e.Value, rightN.Value)).ToList();
            }

            for (int y = 1; y < dim; y++)
            {
                // More lines
                var upperid = field[0, y - 1];
                var current2 = GetNeighbor(upperid, edges[upperid]);
                field[0, y] = current2.Key;
                placed.Add(current2.Key);

                for (int x = 1; x < dim; x++)
                {
                    upperid = field[x, y - 1];
                    var rightN = GetNeighbor(-1, current2.Value, edges[upperid]);

                    field[x, y] = rightN.Key;
                    if (!placed.Add(rightN.Key)) break;

                    current2 = rightN;
                }
            }
            // ids passen, nur noch richtig flippen und drehen

            // orient first tile correctly
            var orientation = GetMixed(tiles[field[0, 0]]).Single(p =>
            {
                var right = p.GetEdge(Direction.Right).ToArray();
                var down = p.GetEdge(Direction.Down).ToArray();
                return edges[field[1, 0]].Any(e => e.SequenceEqual(right)) &&
                       edges[field[0, 1]].Any(e => e.SequenceEqual(down));
            });
            orientedTiles[(0, 0)] = orientation;
            for (int x = 1; x < dim; x++)
            {
                var reference = orientedTiles[(x - 1, 0)];
                orientedTiles[(x, 0)] = GetMixed(tiles[field[x, 0]]).Single(t => MatchesLR(reference, t));
            }

            for (int y = 1; y < dim; y++)
            {
                for (int x = 0; x < dim; x++)
                {
                    var reference = orientedTiles[(x, y - 1)];
                    orientedTiles[(x, y)] = GetMixed(tiles[field[x, y]]).Single(t => MatchesTB(reference, t));
                }
            }

            var clipped = orientedTiles.Select(kvp => KeyValuePair.Create(kvp.Key, Clip(kvp.Value))).ToDictionary();

            var tileWidth = clipped.Values.First().Width;
            var bigDimension = tileWidth * dim;

            var merged = new FiniteGrid2D<char>(bigDimension, bigDimension, (x, y) =>
            {
                var tile = (x / tileWidth, y / tileWidth);
                var inner = new Point(x % tileWidth, y % tileWidth);
                return clipped[tile][inner];
            });

            var properImage = GetMixed(merged).MaxBy(CountMonstersInGrid)!;
            var water = properImage.Count(x => x.value == '#');
            var monsterCount = properImage.Count(x => IsMonsterAt(x.pos, properImage));

            return (water - 15 * monsterCount).ToString();
        }

        private bool MatchesLR(FiniteGrid2D<char> left, FiniteGrid2D<char> right)
            => left.GetEdge(Direction.Right).SequenceEqual(right.GetEdge(Direction.Left));

        private bool MatchesTB(FiniteGrid2D<char> top, FiniteGrid2D<char> bottom)
            => top.GetEdge(Direction.Down).SequenceEqual(bottom.GetEdge(Direction.Up));

        private static FiniteGrid2D<char> Clip(FiniteGrid2D<char> source)
            => new(source.Width - 2, source.Height - 2, (x, y) => source[x + 1, y + 1]);


        private int CountMonstersInGrid(FiniteGrid2D<char> grid)
            => grid.Count(x => IsMonsterAt(x.pos, grid));

        private static bool IsMonsterAt(Point p, FiniteGrid2D<char> grid)
        {
            string getLine(Point p) => string.Concat(Enumerable.Range(0, 20)
                .Select(dx => new Point(p.X + dx, p.Y)).Select(p => grid.GetValueOrDefault(p, 'x')));

            var lines = new[]
            {
                new Regex("..................#."),
                new Regex("#....##....##....###"),
                new Regex(".#..#..#..#..#..#...")
            };
            return lines[0].IsMatch(getLine(p))
                   && lines[1].IsMatch(getLine(p.MoveTo(Direction.Down)))
                   && lines[2].IsMatch(getLine(p.MoveTo(Direction.Down, 2)));
        }
    }
}