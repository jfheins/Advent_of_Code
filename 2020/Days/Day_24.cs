using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

using Core;

namespace AoC_2020.Days
{
    public class Day_24 : BaseDay
    {
        private readonly string[] input;
        private HashSet<(int x, int y)> blackTiles = new();

        public Day_24()
        {
            input = File.ReadAllLines(InputFilePath);
        }

        public override string Solve_1()
        {
            var flipped = new Dictionary<(int x, int y), bool>();
            foreach (var line in input)
            {
                var tile = GetTile(line);
                flipped.AddOrModify(tile, false, x => !x);
            }
            blackTiles = flipped.Where(x => x.Value).Select(x => x.Key).ToHashSet();
            return flipped.Values.Count(x => x).ToString();
        }

        private (int x, int y) GetTile(string line)
        {
            var regex = new Regex("(se|sw|ne|nw|w|e)");
            var moves = regex.Matches(line).Select(g => (g.Value switch
                {
                    "se" => (1, -1),
                    "sw" => (0, -1),
                    "ne" => (0, 1),
                    "nw" => (-1, 1),
                    "w" => (-1, 0),
                    "e" => (1, 0),
                    _ => throw new NotImplementedException()
                })
            );
            return moves.Aggregate((x: 0, y: 0), (pos, move) => (pos.x + move.Item1, pos.y + move.Item2));
        }

        private IEnumerable<(int x, int y)> GetNeighbors((int x, int y) p)
        {
            return new[] { (p.x, p.y + 1), (p.x, p.y - 1),
                (p.x +1 , p.y), (p.x - 1, p.y),
                (p.x - 1, p.y + 1), (p.x + 1, p.y - 1)
            };
        }

        public override string Solve_2()
        {
            for (int i = 0; i < 100; i++)
            {
                blackTiles = Iterate(blackTiles);
            }

            return blackTiles.Count.ToString();
        }

        private HashSet<(int x, int y)> Iterate(HashSet<(int x, int y)> state)
        {
            var relevant = new HashSet<(int x, int y)>(state);

            foreach (var point in state)
                relevant.UnionWith(GetNeighbors(point));

            return new HashSet<(int x, int y)>(relevant.Where(p => Life(p, state)));
        }

        private bool Life((int x, int y) p, HashSet<(int x, int y)> state)
        {
            var active = GetNeighbors(p).Count(n => state.Contains(n));
            return state.Contains(p) ? active == 1 || active == 2 : active == 2;
        }
    }
}

