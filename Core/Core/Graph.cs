using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Core
{
    public static class Graph
    {
        public static Graph<TNode, TEdge> FromEdges<TNode, TEdge>(IEnumerable<TEdge> edges, Func<TEdge, (TNode, TNode)> linker) where TNode : notnull where TEdge : notnull
        {
            Contract.Assert(edges != null);

            var graphEdges = new List<GraphEdge<TNode, TEdge>>();

            var nodes = new Dictionary<TNode, GraphNode<TNode, TEdge>>();

            foreach (var edge in edges)
            {
                var (src, dest) = linker(edge);
                var source = NodeFactory(src);
                var destination = NodeFactory(dest);

                var newEdge = new GraphEdge<TNode, TEdge>(edge, source, destination);
                graphEdges.Add(newEdge);
                source.OutgoingEdges.Add(newEdge);
                destination.IncomingEdges.Add(newEdge);
            }

            return new Graph<TNode, TEdge>(nodes.Values, graphEdges);

            GraphNode<TNode, TEdge> NodeFactory(TNode x)
                => nodes.GetOrAdd(x, it => new GraphNode<TNode, TEdge>(it));
        }

        // Creates an edge-less graph from a collection of nodes
        public static Graph<TNode, TEdge> FromNodeCloud<TNode, TEdge>(IEnumerable<TNode> nodes)
            where TNode : notnull where TEdge : notnull
        {
            Contract.Assert(nodes != null);
            var graphNodes = nodes.Select(n => new GraphNode<TNode, TEdge>(n)).ToList();
            return new Graph<TNode, TEdge>(graphNodes, []);
        }
        
    }

    public class Graph<TNode, TEdge> where TNode : notnull where TEdge : notnull
    {
        public IReadOnlyDictionary<TNode, GraphNode<TNode, TEdge>> Nodes { get; private set; }

        public IReadOnlyDictionary<TEdge, GraphEdge<TNode, TEdge>> Edges { get; private set; }

        public NodeComparer<TNode, TEdge> NodeComparer { get; set; }

        //public IEnumerable<GraphNode<TNode, TEdge>> Sources { get; set; }
        //public IEnumerable<GraphNode<TNode, TEdge>> Sinks { get; set; }

        internal Graph(ICollection<GraphNode<TNode, TEdge>> nodes, ICollection<GraphEdge<TNode, TEdge>> edges)
        {
            Nodes = nodes.ToDictionary(n => n.Value);
            Edges = edges.ToDictionary(e => e.Value);

            NodeComparer = new NodeComparer<TNode, TEdge>(EqualityComparer<TNode>.Default);
        }
    }

    [DebuggerDisplay("Node <{Value}>")]
    public sealed class GraphNode<TNode, TEdge>(TNode value)
    {
        public ICollection<GraphEdge<TNode, TEdge>> IncomingEdges { get; } = new List<GraphEdge<TNode, TEdge>>();

        public ICollection<GraphEdge<TNode, TEdge>> OutgoingEdges { get; } = new List<GraphEdge<TNode, TEdge>>();

        public TNode Value { get; set; } = value;
        public IEnumerable<GraphNode<TNode, TEdge>> Neighbors => IncomingEdges.Select(e => e.Source).Concat(OutgoingEdges.Select(e => e.Destination));
    }

    [DebuggerDisplay("Edge <{Value}>")]
    public sealed class GraphEdge<TNode, TEdge>(
        TEdge value,
        GraphNode<TNode, TEdge> source,
        GraphNode<TNode, TEdge> destination)
    {
        public GraphNode<TNode, TEdge> Source { get; } = source;

        public GraphNode<TNode, TEdge> Destination { get; } = destination;

        public TEdge Value { get; set; } = value;
    }

    public class NodeComparer<TNode, TEdge>(IEqualityComparer<TNode> comparer)
        : EqualityComparer<GraphNode<TNode, TEdge>>
        where TNode : notnull
    {
        public override bool Equals(GraphNode<TNode, TEdge>? a, GraphNode<TNode, TEdge>? b)
        {
            if (a is null || b is null)
                return ReferenceEquals(a, b);

            return comparer.Equals(a.Value, b.Value);
        }

        public override int GetHashCode(GraphNode<TNode, TEdge> x)
        {
            Contract.Assert(x != null);
            return comparer.GetHashCode(x.Value);
        }
    }
}
