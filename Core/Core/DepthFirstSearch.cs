
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using JetBrains.Annotations;

using MoreLinq;

namespace Core
{
    public class DepthFirstSearch<TNode> where TNode: notnull
    {
        public delegate void ProgressReporterCallback(int workingSetCount, int visitedCount);

        private readonly NodeComparer _comparer;
        private readonly Func<TNode, IEnumerable<TNode>> _expander;

        /// <summary>
        ///     Prepares a breadth first search.
        /// </summary>
        /// <param name="comparer">Comparison function that determines node equality</param>
        /// <param name="expander">Callback to get the possible nodes from a source node</param>
        public DepthFirstSearch(IEqualityComparer<TNode>? comparer, Func<TNode, IEnumerable<TNode>> expander)
        {
            _comparer = new NodeComparer(comparer ?? EqualityComparer<TNode>.Default);
            _expander = expander;
        }

        [CanBeNull]
        public IPath<TNode>? FindFirst(TNode initialNode,
                                      Func<TNode, bool> targetPredicate,
                                      ProgressReporterCallback? progressReporter = null)
        {
            var result = FindAll(initialNode, targetPredicate, progressReporter, 1);
            return result.FirstOrDefault();
        }

        [NotNull]
        public IList<IPath<TNode>> FindAll(TNode initialNode,
                                           Func<TNode, bool> targetPredicate,
                                           ProgressReporterCallback? progressReporter = null,
                                           int minResults = int.MaxValue)
        {
            return FindAll2(initialNode, dfsnode => targetPredicate(dfsnode.Item), progressReporter, minResults);
        }

        [NotNull]
        public IList<IPath<TNode>> FindAll2(TNode initialNode,
                                           Func<NodeWithPredecessor, bool> targetPredicate,
                                           ProgressReporterCallback? progressReporter = null,
                                           int minResults = int.MaxValue)
        {
            if (targetPredicate == null)
                throw new ArgumentNullException(nameof(targetPredicate), "A meaningful targetPredicate must be provided");

            var results = new List<IPath<TNode>>();
            var initial = new NodeWithPredecessor(initialNode);
            var work = new Stack<(NodeWithPredecessor node, HashSet<TNode> past)>();
            work.Push((initial, []));

            while (work.TryPop(out var current))
            {
                current.past.Add(current.node.Item);

                if (targetPredicate(current.node))
                {
                    results.Add(new DfsPath(current.node));
                   // progressReporter?.Invoke(work.Count, results.Count);
                }

                var neighbors = _expander(current.node.Item).ToList();
                neighbors.RemoveAll(current.past.Contains);

                for (var i = neighbors.Count - 1; i >= 0; i--)
                {
                    var next = new NodeWithPredecessor(neighbors[i], current.node);
                    if (i==0)
                        work.Push((next, current.past)); // Reuse hashset
                    else
                        work.Push((next, [.. current.past]));
                }                    
            }

            return results;
        }

        private IEnumerable<NodeWithPredecessor> SequentialExpand(IEnumerable<NodeWithPredecessor> nextNodes,
                                                                  HashSet<NodeWithPredecessor> visitedNodes)
        {
            return nextNodes
                .SelectMany(sourceNode => _expander(sourceNode.Item)
                    .Select(dest => new NodeWithPredecessor(dest, sourceNode))
                    .Where(dest => !visitedNodes.Contains(dest)));
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

        private class NodeComparer : EqualityComparer<NodeWithPredecessor>
        {
            public readonly IEqualityComparer<TNode> _comparer;

            public NodeComparer(IEqualityComparer<TNode> comparer)
            {
                _comparer = comparer;
            }

            public override bool Equals(NodeWithPredecessor? a, NodeWithPredecessor? b)
            {
                if (a is null || b is null)
                    return ReferenceEquals(a, b);

                return _comparer.Equals(a.Item, b.Item);
            }

            public override int GetHashCode(NodeWithPredecessor x)
            {
                return _comparer.GetHashCode(x.Item);
            }
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