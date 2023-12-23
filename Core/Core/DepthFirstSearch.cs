
using JetBrains.Annotations;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Core
{
    public class DepthFirstSearch<TNode> where TNode : notnull
    {
        public delegate void ProgressReporterCallback(int workingSetCount, int visitedCount);

        private readonly IEqualityComparer<TNode>? _comparer;
        private readonly Func<TNode, IEnumerable<TNode>> _expander;

        /// <summary>
        ///     Prepares a breadth first search.
        /// </summary>
        /// <param name="comparer">Comparison function that determines node equality</param>
        /// <param name="expander">Callback to get the possible nodes from a source node</param>
        public DepthFirstSearch(IEqualityComparer<TNode>? comparer, Func<TNode, IEnumerable<TNode>> expander)
        {
            _comparer = comparer;
            _expander = expander;
        }

        [CanBeNull]
        public IPath<TNode>? FindFirst(TNode initialNode,
                                      Func<TNode, bool> targetPredicate,
                                      ProgressReporterCallback? progressReporter = null)
        {
            var result = FindAll(initialNode, targetPredicate);
            return result.FirstOrDefault();
        }

        [NotNull]
        public IList<IPath<TNode>> FindAll(TNode initialNode,
                                           Func<TNode, bool> targetPredicate)
        {
            return FindAll2(initialNode, dfsnode => targetPredicate(dfsnode.Item));
        }

        [NotNull]
        public IList<IPath<TNode>> FindAll2(
            TNode initialNode,
            Func<DfsNode, bool> targetPredicate)
        {
            if (targetPredicate == null)
                throw new ArgumentNullException(nameof(targetPredicate), "A meaningful targetPredicate must be provided");

            var results = new List<IPath<TNode>>();
            var work = new Stack<DfsNode>(200);
            work.Push(new DfsNode(initialNode, _comparer));

            while (work.TryPop(out var current))
            {
                if (targetPredicate(current))
                    results.Add(new DfsPath(current));

                var neighbors = _expander(current.Item).ToList();
                neighbors.RemoveAll(current.Visited.Contains);

                foreach (var neighbor in neighbors)
                {
                    work.Push(new DfsNode(neighbor, current));
                }
            }

            return results;
        }

        private class DfsPath : IPath<TNode>
        {
            public DfsPath(TNode singleNode)
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

            public TNode Target { get; }
            public int Length { get; }
            public TNode[] Steps { get; }
        }

        public class DfsNode
        {
            public DfsNode(TNode current, IEqualityComparer<TNode>? cmp)
            {
                Predecessor = null;
                Item = current;
                Visited = ImmutableHashSet.Create(cmp, current);
            }

            public DfsNode(TNode current, DfsNode predecessor)
            {
                Predecessor = predecessor;
                Item = current;
                Visited = predecessor.Visited.Add(current);
            }

            public TNode Item { get; }
            public DfsNode? Predecessor { get; }
            public ImmutableHashSet<TNode> Visited { get; }

            public IEnumerable<TNode> GetHistory()
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