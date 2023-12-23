
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
            Func<NodeWithPredecessor, bool> targetPredicate)
        {
            if (targetPredicate == null)
                throw new ArgumentNullException(nameof(targetPredicate), "A meaningful targetPredicate must be provided");

            var results = new List<IPath<TNode>>();
            var initial = new NodeWithPredecessor(initialNode);
            var work = new Stack<(NodeWithPredecessor node, ImmutableHashSet<TNode> past)>();
            work.Push((initial, ImmutableHashSet.Create(_comparer)));

            while (work.TryPop(out var current))
            {
                if (targetPredicate(current.node))
                    results.Add(new DfsPath(current.node));

                var newHistory = current.past.Add(current.node.Item);
                var neighbors = _expander(current.node.Item).ToList();
                neighbors.RemoveAll(newHistory.Contains);

                foreach (var neighbor in neighbors)
                {
                    var next = new NodeWithPredecessor(neighbor, current.node);
                    work.Push((next, newHistory));
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

            public DfsPath(NodeWithPredecessor target)
            {
                Target = target.Item;
                Steps = target.GetHistory().Reverse().ToArray();
                Length = Steps.Length - 1;
            }

            public TNode Target { get; }
            public int Length { get; }
            public TNode[] Steps { get; }
        }

        public class NodeWithPredecessor
        {
            public NodeWithPredecessor(TNode current, NodeWithPredecessor? predecessor = null)
            {
                Predecessor = predecessor;
                Item = current;
                Distance = (predecessor?.Distance + 1) ?? 0;
            }

            public TNode Item { get; }
            public NodeWithPredecessor? Predecessor { get; }
            public int Distance { get; set; }

            public IEnumerable<TNode> GetHistory()
            {
                NodeWithPredecessor? pointer = this;
                do
                {
                    yield return pointer.Item;
                    pointer = pointer.Predecessor;
                } while (pointer != null);
            }
        }
    }
}