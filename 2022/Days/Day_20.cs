using Core;

using Spectre.Console;

using System.Data;
using System.Runtime.Intrinsics;

namespace AoC_2022.Days
{
    public sealed class Day_20 : BaseDay
    {
        private IReadOnlyList<int> _input;

        public Day_20()
        {
            _input = File.ReadAllLines(InputFilePath).SelectList(int.Parse);
        }

        public override async ValueTask<string> Solve_1()
        {
            var ll = new LinkedList<int>();
            foreach (var item in _input)
            {
               _= ll.AddLast(item);
            }
            var end = ll.Last;
            var current = ll.First;
            var wasMoved = new HashSet<LinkedListNode<int>>();

            while(current != null)
            {
                if (!wasMoved.Add(current))
                {
                    current = current.Next;
                    continue;
                }
                var next = current.Next;
                var toMove = current.Value;
                while(toMove < 0)
                {
                    var pred = current.Previous ?? ll.Last!;
                    ll.Remove(current);
                    if (pred == ll.First)
                    {
                        ll.AddLast(current);
                    }
                    else
                    {
                        ll.AddBefore(pred, current);
                    }
                    toMove++;
                }
                while (toMove > 0)
                {
                    var succ = current.Next ?? ll.First!;
                    ll.Remove(current);
                    ll.AddAfter(succ, current);
                    toMove--;
                }
                current = next;
            }
            var zeroIdx = ll.IndexWhere(it => it == 0).First();

            return (GetValue(zeroIdx+1000, ll) + GetValue(zeroIdx+2000, ll) + GetValue(zeroIdx+3000, ll)).ToString();
        }

        public T GetValue<T>(int idx, LinkedList<T> list)
        {
            var rem = idx.Modulo(list.Count);
            var node = list.First;
            while (rem-- > 0)
                node = node.Next ?? list.First!;
            return node.Value;
        }

        public override async ValueTask<string> Solve_2()
        {
            return "-";
        }
    }
}