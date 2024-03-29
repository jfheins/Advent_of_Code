﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Core;

public static class LinqHelpers
{
    /// <summary>
    ///     Verpackt den angegebenen Wert in eine Enumeration mit einem Element.
    /// </summary>
    /// <typeparam name="T">Ein beliebiger Typ.</typeparam>
    /// <param name="item">Der Wert, der verpackt werden soll.</param>
    /// <returns>Eine Enumeration, die genau einen Wert enthält.</returns>
    public static IEnumerable<T> ToEnumerable<T>(this T item)
    {
        yield return item;
    }

    public static IEnumerable<int> IndexWhere<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        return source.Select((value, index) => new { value, index })
            .Where(x => predicate(x.value))
            .Select(x => x.index);
    }

    public static IEnumerable<int> IndexWhereOn(this BitVector32 source)
    {
        var data = source.Data;
        for (int i = 0; i < 32; i++)
        {
            if ((data & (1 << i)) != 0)
                yield return i;
        }
    }

    public static int PopCount(this BitVector32 source)
    {
        return BitOperations.PopCount((uint)source.Data);
    }

    public static IEnumerable<T> ExceptFor<T>(this IEnumerable<T> source, T exception) where T : notnull
    {
        return source.Where(x => !exception.Equals(x));
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> factory) where TKey : notnull
    {
        if (dict.TryGetValue(key, out var data))
            return data;
        else
            return dict[key] = factory(key);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue) where TKey : notnull
    {
        if (dict.TryGetValue(key, out var data))
            return data;
        else
            return dict[key] = defaultValue;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
    public static void AddToList<TKey, TValue>(this IDictionary<TKey, List<TValue>> dict, TKey key, TValue value) where TKey : notnull
    {
        if (dict.TryGetValue(key, out var list))
            list.Add(value);
        else
            dict[key] = new List<TValue> { value };
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddOrModify<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue initValue, Func<TValue, TValue> modifier) where TKey : notnull
    {
        if (dict.TryGetValue(key, out var data))
            dict[key] = modifier(data);
        else
            dict[key] = modifier(initValue);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
    public static IEnumerable<TValue> GetOrEmpty<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TKey : notnull
    {
        if (dict.TryGetValue(key, out var data))
            yield return data;
    }

    public static IEnumerable<ValueTuple<T1, T2>> CartesianProduct<T1, T2>(this IEnumerable<T1> a,
        IEnumerable<T2> b)
    {
        if (b is not ICollection<T2>)
        {
            b = b.ToList();
        }

        return a.SelectMany(x => b, (x, y) => (x, y));
    }

    public static long Product(this IEnumerable<int> source)
    {
        return source.Aggregate(1L, (a, b) => a * b);
    }

    public static long Product(this IEnumerable<long> source)
    {
        return source.Aggregate(1L, (a, b) => a * b);
    }
    
    public static long LowestCommonMulti(this IEnumerable<long> source)
    {
        return source.Aggregate(LowestCommonMulti);
    }
    public static long LowestCommonMulti(long a, long b)
    {
        return Math.Abs(a * b) / GreatestCommonDivisor(a, b);
    }
    
    public static long GreatestCommonDivisor(this IEnumerable<long> source)
    {
        return source.Aggregate(GreatestCommonDivisor);
    }
    public static long GreatestCommonDivisor(long a, long b)
    {
        return b == 0 ? a : GreatestCommonDivisor(b, a % b);
    }
    
    public static int Modulo(this int x, int ringSize)
    {
        while (x < 0)
            x += ringSize;
        return x % ringSize;
    }
    public static long Modulo(this long x, long ringSize)
    {
        while (x < 0)
            x += ringSize;
        return x % ringSize;
    }

    public static int OneBasedModulo(this int x, int ringSize) => (x + ringSize - 1) % ringSize + 1;
    public static long OneBasedModulo(this long x, int ringSize) => (x + ringSize - 1) % ringSize + 1;

    public static T CenterItem<T>(this IList<T> source)
    {
        return source[source.Count / 2];
    }

    public static (T min, T max)? MinMax<T>(this IEnumerable<T> source)
    {
        return source.MinMax(x => x);
    }
    public static (T min, T max) MinMax<T>(params T[] source)
    {
        Debug.Assert(source.Length > 0);
        return source.MinMax(x => x)!.Value;
    }

    public static (TResult min, TResult max)? MinMax<T, TResult>(this IEnumerable<T> source,
        Func<T, TResult> selector)
    {
        var min = source.Min(selector);
        var max = source.Max(selector);
        return min is null || max is null ? null : (min, max);
    }

    public static (TResult min, TResult max) MinMax<T, TResult>(this NonEmptyList<T> source,
        Func<T, TResult> selector)
    {
        var min = source.Min(selector);
        var max = source.Max(selector);
        return (min!, max!);
    }

    public static (T first, T last) FirstLast<T>(this IEnumerable<T> source)
    {
        using var e = source.GetEnumerator();
        var res = e.MoveNext();
        Debug.Assert(res);

        T first = e.Current;
        T last = e.Current;

        while (e.MoveNext())
            last = e.Current;

        return (first, last);
    }

    public static bool AllEqual<T>(this IEnumerable<T> source, T value) where T : IEquatable<T>
    {
        return source.All(x => x.Equals(value));
    }

    public static bool AreAllEqual<T>(this IEnumerable<T> source) where T : IEquatable<T>
    {
        var first = source.First();
        return source.All(x => x.Equals(first));
    }

    public static bool AreAllDistinct<T>(this IEnumerable<T> source) where T : IEquatable<T>
    {
        var seen = new HashSet<T>();
        return source.All(item => seen.Add(item));
    }

    public static bool AreAllDistinct2<T>(this IList<T> source) where T : IEquatable<T>
    {
        for (int i = 0; i < source.Count - 1; i++)
        {
            for (int j = i + 1; j < source.Count; j++)
            {
                if (source[i].Equals(source[j]))
                    return false;
            }
        }
        return true;
    }

    public static int Count<T>(this IEnumerable<T> source, T value) where T: notnull
        => source.Count(it => value.Equals(it));

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
    {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        return source.Where(x => x != null);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : struct
    {
        return source.Where(x => x != null).Select(x => x!.Value);
    }

    public static T? FirstOrNull<T>(this IEnumerable<T> source) where T : struct => source.Cast<T?>().FirstOrDefault();
    public static T? FirstOrNull<T>(this IEnumerable<T> source, Func<T, bool> predicate) where T : struct
        => source.Cast<T?>().FirstOrDefault(it => predicate(it!.Value), null);

    /// <summary>
    ///     Liefert zu einer Enumeration alle Paare zurück. Eine Enumeration mit n Elementen hat genau n-1 Paare.
    ///     Die Quelle wird nur einmal durchlaufen. Für jedes Paar wird ein neues Tupel generiert.
    ///     Item1 ist stets das Element, dass in der Quelle zuerst vorkommt.
    /// </summary>
    /// <param name="source">Die Quelle, die paarweise enumeriert werden soll.</param>
    /// <returns>
    ///     Eine Enumeration mit n-1 überschneidenden Tupeln. Gibt eine leere Enumeration zurück, wenn die Quelle aus
    ///     weniger als zwei Elmenten besteht.
    /// </returns>
    public static IEnumerable<ValueTuple<T, T>> PairwiseWithOverlap<T>(this IEnumerable<T> source)
    {
        Contract.Assert(source != null, nameof(source));
        using var it = source.GetEnumerator();
        if (!it.MoveNext())
        {
            yield break;
        }

        var previous = it.Current;

        while (it.MoveNext())
        {
            yield return ValueTuple.Create(previous, previous = it.Current);
        }
    }

    public static IEnumerable<ValueTuple<T, T>> Pairwise<T>(this IEnumerable<T> source)
    {
        Contract.Assert(source != null, nameof(source));
        var isPair = false;
        var tempItem = default(T);
        foreach (var item in source)
        {
            if (isPair)
            {
                yield return ValueTuple.Create(tempItem!, item);
                isPair = false;
            }
            else
            {
                tempItem = item;
                isPair = true;
            }
        }
    }

    public static IEnumerable<ValueTuple<T, T, T>> Triplewise<T>(this IEnumerable<T> source)
    {
        Contract.Assert(source != null, nameof(source));
        var saved = 0;
        var temp1 = default(T);
        var temp2 = default(T);
        foreach (var item in source)
        {
            if (saved == 0)
            {
                temp1 = item;
                saved++;
            }
            else if (saved == 1)
            {
                temp2 = item;
                saved++;
            }
            else
            {
                saved = 0;
                yield return ValueTuple.Create(temp1!, temp2!, item);
            }
        }
    }

    public static IEnumerable<(T item, int count)> Histogram<T>(this IEnumerable<T> source) => source.Histogram(x => x);
    public static IEnumerable<(TKey item, int count)> Histogram<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        => source.ToLookup(keySelector).Select(group => (group.Key, group.Count()));

    public static IEnumerable<(int Count, T Element)> Runs<T>(this IEnumerable<T> enumerable, EqualityComparer<T>? comparer = null)
    {
        Contract.Assert(enumerable != null, nameof(enumerable));
        comparer ??= EqualityComparer<T>.Default;

        var count = -1;
        var reference = default(T);

        using var e = enumerable.GetEnumerator();
        while (e.MoveNext())
        {
            if (count == -1)
            {
                reference = e.Current;
                count = 0;
            }
            else if (!comparer.Equals(e.Current, reference))
            {
                yield return (count, reference!);
                reference = e.Current;
                count = 0;
            }
            count++;
        }
        yield return (count, reference!);
    }

    public static IEnumerable<ImmutableList<T>> Chunks<T>(this IEnumerable<T> enumerable, EqualityComparer<T>? comparer = null)
    {
        Contract.Assert(enumerable != null, nameof(enumerable));
        comparer ??= EqualityComparer<T>.Default;

        var chunk = ImmutableList.CreateBuilder<T>();
        var reference = default(T);

        using var e = enumerable.GetEnumerator();
        while (e.MoveNext())
        {
            if (chunk.Count == 0)
            {
                reference = e.Current;
            }
            else if (!comparer.Equals(e.Current, reference))
            {
                yield return chunk.ToImmutable();
                chunk = ImmutableList.CreateBuilder<T>();
                reference = e.Current;
            }
            chunk.Add(e.Current);
        }
        yield return chunk.ToImmutable();
    }

    public static IEnumerable<ReadOnlyMemory<char>> Chunks(this string source, EqualityComparer<char>? comparer = null)
    {
        Contract.Assert(source != null, nameof(source));
        comparer ??= EqualityComparer<char>.Default;

        var startIdx = 0;
        var strMem = source.AsMemory();

        for (var i = 0; i < source.Length; i++)
        {
            if (!comparer.Equals(source[i], source[startIdx]))
            {
                yield return strMem[startIdx..i];
                startIdx = i;
            }
        }
        yield return strMem[startIdx..];
    }

    // https://stackoverflow.com/questions/419019/split-list-into-sublists-with-linq/20953521#20953521
    [Obsolete("Use .net 7 Chunk")]
    public static IEnumerable<IEnumerable<T>> Chunks<T>(this IEnumerable<T> enumerable,
        int chunkSize)
    {
        Contract.Assert(enumerable != null, nameof(enumerable));
        if (chunkSize < 1)
        {
            throw new ArgumentException("chunkSize must be positive");
        }

        using var e = enumerable.GetEnumerator();
        while (e.MoveNext())
        {
            var remaining = chunkSize; // elements remaining in the current chunk
                                       // ReSharper disable once AccessToDisposedClosure
            var innerMoveNext = new Func<bool>(() => --remaining > 0 && e.MoveNext());

            yield return e.GetChunk(innerMoveNext);
            while (innerMoveNext())
            {
                /* discard elements skipped by inner iterator */
            }
        }
    }

    private static IEnumerable<T> GetChunk<T>(this IEnumerator<T> e,
        Func<bool> innerMoveNext)
    {
        do
        {
            yield return e.Current;
        } while (innerMoveNext());
    }

    public static IEnumerable<IList<Point>> PointsInBoundingRect(this IEnumerable<Point> points)
    {
        var list = new NonEmptyList<Point>(points);
        var (minx, maxx) = list.MinMax(p => p.X);
        var (miny, maxy) = list.MinMax(p => p.Y);

        for (var y = miny; y <= maxy; y++)
        {
            yield return Enumerable.Range(minx, maxx - minx + 1).Select(x => new Point(x, y)).ToList();
        }
    }

    public static IEnumerable<int> StartingIndex<T>(this IList<T> x, IList<T> y)
    {
        Contract.Assert(x != null, nameof(x));
        Contract.Assert(y != null, nameof(y));
        // https://stackoverflow.com/a/1780481
        var index = Enumerable.Range(0, x.Count - y.Count + 1);
        for (var i = 0; i < y.Count; i++)
        {
            index = index.Where(n => x[n + i]!.Equals(y[i])).ToArray();
        }
        return index;
    }

    public static IEnumerable<T> RepeatIndefinitely<T>(this IEnumerable<T> source)
    {
        var list = source.ToList();
        while (true)
        {
            foreach (var item in list)
            {
                yield return item;
            }
        }
    }

    public static IEnumerable<T> Repeat<T>(this IEnumerable<T> source, int count = 2)
    {
        var list = source.ToList();
        return Enumerable.Repeat(list, count).SelectMany(l => l);
    }

    public static IEnumerable<int> CumulativeSum(this IEnumerable<int> sequence)
    {
        Contract.Assert(sequence != null, nameof(sequence));
        var sum = 0;
        foreach (var item in sequence)
        {
            sum += item;
            yield return sum;
        }
    }
    public static IEnumerable<long> CumulativeSum(this IEnumerable<long> sequence)
    {
        Contract.Assert(sequence != null, nameof(sequence));
        long sum = 0;
        foreach (var item in sequence)
        {
            sum += item;
            yield return sum;
        }
    }
    public static IEnumerable<(int x, int y, T value)> WithXY<T>(this IEnumerable<IEnumerable<T>> sequence)
        => sequence.SelectMany((line, y) => line.Select((item, x) => (x, y, item)));

    public static IEnumerable<TResult> Select2D<T, TResult>(this IEnumerable<IEnumerable<T>> sequence, Func<int, int, T, TResult> selector)
        => sequence.SelectMany((line, y) => line.Select((item, x) => selector(x, y, item)));

    public static IEnumerable<string[]> MatchRegexGroups(this IEnumerable<string> source, string pattern, int? count = null)
    {
        var regex = new Regex(pattern, RegexOptions.Singleline, TimeSpan.FromMilliseconds(100));
        foreach (var line in source)
        {
            ICollection<Group> matches = regex.Match(line).Groups;
            if (count != null)
                Debug.Assert(matches.Count == count);
            yield return matches.Select(x => x.Value).ToArray();
        }
    }

    public static IEnumerable<T> StepBy<T>(this IEnumerable<T> source, int stepSize, int skipFirst = 0)
    {
        long counter = 0;
        foreach (var item in source.Skip(skipFirst))
        {
            if (counter == 0)
                yield return item;
            counter = (counter + 1) % stepSize;
        }
    }

    public static List<TResult> SelectList<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector)
    {
        return source.Select(selector).ToList();
    }

    public static TResult[] SelectArray<T, TResult>(this IList<T> source, Func<T, TResult> selector)
    {
        var result = new TResult[source.Count];
        for (var i = 0; i < source.Count; i++)
        {
            result[i] = selector(source[i]);
        }
        return result;
    }

    public static string AsString(this Range r)
    {
        Debug.Assert(!r.Start.IsFromEnd && !r.End.IsFromEnd);
        var length = r.End.Value - r.Start.Value + 1;
        var result = string.Create(length, r, static (span, r) =>
        {
            for (var i = 0; i < span.Length; i++)
                span[i] = (char)(i + r.Start.Value);
        });
        return result;
    }

    public static int IndexOfOrThrow(this string s, char c)
    {
        var idx = s.IndexOf(c);
        return idx >= 0 ? idx : throw new Exception("Char not found in string");
    }

    public static IEnumerable<T[]> SplitInto<T>(this ICollection<T> source, int chunks)
    {
        var chunkSize = source.Count / chunks;
        Debug.Assert(chunkSize * chunks == source.Count);
        return source.Chunk(chunkSize);
    }

    public static IEnumerable<char[]> SplitInto(this string source, int chunks)
    {
        var chunkSize = source.Length / chunks;
        Debug.Assert(chunkSize * chunks == source.Length);
        return source.Chunk(chunkSize);
    }

    public static (T first, IEnumerable<T> rest) FirstAndRest<T>(this IEnumerable<T> source)
    {
        var enumerator = source.GetEnumerator();
        var res = enumerator.MoveNext();
        Debug.Assert(res);
        return (enumerator.Current, enumerator.AsEnumerable());
    }

    public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> enumerator)
    {
        while (enumerator.MoveNext())
            yield return enumerator.Current;
    }

    public static IEnumerator<int> GetEnumerator(this Range range)
    {
        var start = range.Start.GetOffset(int.MaxValue);
        var end = range.End.GetOffset(int.MaxValue);
        var acc = end.CompareTo(start);
        var current = start;
        do
        {
            yield return current;
            current += acc;
        } while (current != end);
    }

    public static IEnumerable<T> PopMany<T>(this Stack<T> source, int count)
    {
        Debug.Assert(source.Count >= count);
        for (var i = 0; i < count; i++)
            yield return source.Pop();
    }

    public static IEnumerable<ArraySegment<TSource>> WindowArr<TSource>(this TSource[] source, int size)
    {
        for (var start = 0; start <= source.Length - size; start++)
        {
            yield return new ArraySegment<TSource>(source, start, size);
        }
    }

    public static bool TryPop<T>(this IList<T> source, [NotNullWhen(true)] out T? item)
    {
        if (source.Count > 0)
        {
            item = source[^1]!;
            source.RemoveAt(source.Count - 1);
            return true;
        }
        else
        {
            item = default;
            return false;
        }
    }

    public static int IndexOf<T>(this ReadOnlySpan<T> source, T value, int start) where T:IEquatable<T>
    {
        var idx = source[start..].IndexOf(value);
        return idx == -1 ? idx : idx + start;
    }

    public static IEnumerable<TResult> Select<T1, T2, TResult>(
        this IEnumerable<ValueTuple<T1, T2>> source, 
        Func<T1, T2, TResult> selector)
    {
        return source.Select(t => selector(t.Item1, t.Item2));
    }

    public static IEnumerable<TResult> RunningFold<T, TResult>(
        this IEnumerable<T> source,
        TResult seed,
        Func<TResult, T, TResult> aggregator)
    {
        var aggregate = seed;
        foreach (var elem in source)
            yield return aggregate = aggregator(aggregate, elem);
    }

    public static long PolygonArea(this IEnumerable<Point> points)
    {
        var area = 0L;
        foreach (var (l, r) in points.PairwiseWithOverlap())
        {
            var dx = l.X - r.X;
            var sumY = l.Y + r.Y;
            area += (dx * sumY) / 2;
        }

        return area;
    }

    /// <summary>
    /// This function only works if the polygon is axis aligned!
    /// </summary>
    public static (long Area, long Edge, long Inside) PickTheorem(this IReadOnlyList<Point> points)
    {
        // Pick theorem
        // Area = Inside + Edge/2 - 1
        // Inside = Area - Edge/2 + 1
        Debug.Assert(points[0] == points[^1]);

        var area = 0L;
        var edgePointCount = 0;
        foreach (var (l, r) in points.PairwiseWithOverlap())
        {
            Debug.Assert(l.X == r.X || l.Y == r.Y);
            long dx = l.X - r.X;
            long sumY = (l.Y + r.Y) / 2;
            area += dx * sumY;
            edgePointCount += l.ManhattanDistTo(r);
        }

        return (area, edgePointCount, area - edgePointCount / 2 + 1);
    }
}
