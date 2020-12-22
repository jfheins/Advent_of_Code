using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static MoreLinq.Extensions.SplitExtension;

using Core;
using System.Collections.Immutable;

namespace AoC_2020.Days
{
    public class Day_22 : BaseDay
    {
        private readonly LinkedList<int> player1;
        private readonly LinkedList<int> player2;
        private ImmutableList<int> p1list;
        private ImmutableList<int> p2list;

        public Day_22()
        {
            var input = File.ReadAllLines(InputFilePath).Split("").ToArray(); ;
            player1 = new LinkedList<int>(input[0].Skip(1).Select(int.Parse));
            player2 = new LinkedList<int>(input[1].Skip(1).Select(int.Parse));
            p1list = player1.ToImmutableList();
            p2list = player2.ToImmutableList();
        }

        public override string Solve_1()
        {
            while (player1.Count * player2.Count > 0)
            {
                var played = (player1.First!.Value, player2.First!.Value);

                player1.RemoveFirst();
                player2.RemoveFirst();

                if (played.Item1 > played.Item2)
                {
                    player1.AddLast(played.Item1);
                    player1.AddLast(played.Item2);
                }
                else
                {
                    player2.AddLast(played.Item2);
                    player2.AddLast(played.Item1);
                }
            }

            var winner = player1.Any() ? player1 : player2;
            var count = winner.Count;
            var score = winner.Select((item, idx) => (count - idx) * (long)item).Sum();

            return score.ToString();
        }

        public override string Solve_2()
        {
            var winner = RecurseCombat(p1list, p2list);
            var winnerdeck = winner.player == 1 ? p1list : p2list;
            var count = winner.deck.Count;
            var score = winner.deck.Select((item, idx) => (count - idx) * (long)item).Sum();
            return score.ToString();
        }

        //Dictionary<ImmutableList<int>, int> seen = new();
        int game = 1;
        private (int player, ImmutableList<int> deck) RecurseCombat(ImmutableList<int> deck1, ImmutableList<int> deck2)
        {
            HashSet<string> seen = new();
            var thisgame = game++;
            while (deck1.Any() && deck2.Any())
            {
                //var builder = ImmutableList.CreateBuilder<int>();
                //builder.AddRange(deck1);
                //builder.Add(-1);
                //builder.AddRange(deck2);
                //var state =  builder.ToImmutable();
                var state = string.Join(',', deck1) + "/" + string.Join(',', deck2);
                if (!seen.Add(state))
                    return (1, deck1);

                var drawn = new[] { deck1[0], deck2[0] };
                int winner;
                if (deck1.Count > drawn[0] && deck2.Count > drawn[1])
                    winner = RecurseCombat(deck1.GetRange(1, drawn[0]), deck2.GetRange(1, drawn[1])).player;
                else
                    winner = drawn[0] > drawn[1] ? 1 : 2;

                if (winner == 1)
                {
                    deck1 = deck1.RemoveAt(0).AddRange(drawn);
                    deck2 = deck2.RemoveAt(0);
                }
                else if(winner == 2)
                {
                    deck1 = deck1.RemoveAt(0);
                    deck2 = deck2.RemoveAt(0).AddRange(drawn.Reverse());
                }
            }
            return deck1.Any() ? (1, deck1) : (2, deck2);
        }

        public static long GetSequenceHashCode<T>(IEnumerable<T> sequence) where T: notnull
        {
            const long seed = 487;
            const long modifier = 31;

            unchecked
            {
                return sequence.Aggregate(seed, (current, item) =>
                    (current * modifier) + item.GetHashCode());
            }
        }
    }
}

