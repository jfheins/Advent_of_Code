using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Core;

/// <summary>
///  Keeps track of several disjoint sets of items.
/// </summary>
public class DisjointSet<T>(int capacity = 0)
    where T : notnull
{
    /// <summary>
    /// Every subset has a root item. This dictionary maps every item to its associated parent,
    /// traversing the parent recursively will yield the root item.
    /// A root item points to itself as parent.
    /// </summary>
    private readonly Dictionary<T, T> _associatedParent = capacity > 0 ? new Dictionary<T, T>(capacity) : new Dictionary<T, T>();
    
    /// <summary>
    /// The number of disjoint sets currently tracked.
    /// </summary>
    public int DisjointSetCount { get; private set; }
    
    /// <summary>
    /// The total number of items in the disjoint set.
    /// </summary>
    public int TotalItemCount => _associatedParent.Count;

    public bool AreInSameSet(T a, T b)
    {
        if (_associatedParent.ContainsKey(a) && _associatedParent.ContainsKey(b))
            return FindSetRoot(a).Equals(FindSetRoot(b));
        return false;
    }
    
    // Returns null if the item is not known. Otherwise, returns the root of the set the item belongs to.
    public T FindSetRoot(T item)
    {
        Debug.Assert(_associatedParent.ContainsKey(item));
        var parent = _associatedParent[item];
        
        if (item.Equals(parent))
            return item; // Root

        return _associatedParent[item] = FindSetRoot(parent); // Path compression
    }
    
    public void Add(T item)
    {
        if (_associatedParent.TryAdd(item, item))
        {
            DisjointSetCount++;
        }
    }
    
    public void AddConnection(T a, T b)
    {
        Add(a);
        Add(b);
        Union(a, b);
    }
    
    public void Union(T a, T b)
    {
        var rootA = FindSetRoot(a);
        var rootB = FindSetRoot(b);
        
        if (rootA.Equals(rootB))
            return; // Already in the same set
        
        _associatedParent[rootB] = rootA; // Merge sets
        FindSetRoot(b); // compress path for b
        DisjointSetCount--;
    }

    public IReadOnlyCollection<IReadOnlyList<T>> GetAllConnectedSets()
    {
        var rootToItems = new Dictionary<T, List<T>>();
        foreach (var item in _associatedParent.Keys)
        {
            var root = FindSetRoot(item);
            rootToItems.AddOrModifyAction(root, () => [], l => l.Add(item));
        }

        return rootToItems.Values;
    }
}