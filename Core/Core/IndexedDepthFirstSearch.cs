
using JetBrains.Annotations;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class IndexedDepthFirstSearch
    {
        private readonly int _elementCount;
        private readonly Func<byte, IEnumerable<byte>> _expander;

        /// <summary>
        ///     Prepares a depth first search.
        /// </summary>
        /// <param name="expander">Callback to get the possible nodes from a source node</param>
        public IndexedDepthFirstSearch(int elemCount, Func<byte, IEnumerable<byte>> expander)
        {
            _elementCount = elemCount;
            _expander = expander;
        }

        [CanBeNull]
        public IPath<byte>? FindFirst(byte initialNode,
                                      Func<byte, bool> targetPredicate)
        {
            var result = FindAll(initialNode, targetPredicate);
            return result.FirstOrDefault();
        }

        [NotNull]
        public IList<IPath<byte>> FindAll(byte initialNode,
                                           Func<byte, bool> targetPredicate)
        {
            return FindAll2(initialNode, dfsnode => targetPredicate(dfsnode.Item));
        }

        [NotNull]
        public IList<IPath<byte>> FindAll2(
            byte initialNode,
            Func<DfsNode, bool> targetPredicate)
        {
            if (targetPredicate == null)
                throw new ArgumentNullException(nameof(targetPredicate), "A meaningful targetPredicate must be provided");

            var results = new List<IPath<byte>>();
            var work = new Stack<DfsNode>(200);
            work.Push(new DfsNode(_elementCount, initialNode));

            while (work.TryPop(out var current))
            {
                if (targetPredicate(current))
                    results.Add(new DfsPath(current));

                var neighbors = _expander(current.Item).ToList();
                neighbors.RemoveAll(n => current.Visited[n]);

                foreach (var neighbor in neighbors)
                {
                    work.Push(new DfsNode(neighbor, current));
                }
            }

            return results;
        }

        private class DfsPath : IPath<byte>
        {
            public DfsPath(byte singleNode)
            {
                Target = singleNode;
                Length = 0;
                Steps = [singleNode];
            }

            public DfsPath(DfsNode target)
            {
                Target = target.Item;
                Steps = target.GetHistory().Reverse().ToArray();
                Length = Steps.Length - 1;
            }

            public byte Target { get; }
            public int Length { get; }
            public byte[] Steps { get; }
        }

        public class DfsNode
        {
            public DfsNode(int bitCount, byte current)
            {
                Predecessor = null;
                Item = current;
                Visited = new BitArray(bitCount);
            }

            public DfsNode(byte current, DfsNode predecessor)
            {
                Predecessor = predecessor;
                Item = current;
                Visited = new BitArray(predecessor.Visited);
                Visited.Set(current, true);
            }

            public byte Item { get; }
            public DfsNode? Predecessor { get; }
            public BitArray Visited { get; }

            public IEnumerable<byte> GetHistory()
            {
                DfsNode? pointer = this;
                do
                {
                    yield return pointer.Item;
                    pointer = pointer.Predecessor;
                } while (pointer != null);
            }
        }
    }
}