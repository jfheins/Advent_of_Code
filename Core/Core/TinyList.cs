using System;
using System.Collections.Generic;

namespace Core;

public ref struct TinyList<T>(Span<T> buffer)
    where T : struct
{
    private readonly Span<T> _buffer = buffer;
    public int Count { get; private set; } = 0;

    public void Add(T value) => _buffer[Count++] = value;

    public readonly T Get(int index) => _buffer[index];

    public readonly Span<T> AsSpan() => _buffer[..Count];

    public readonly T this[int i] => _buffer[i];

    public void Clear() => Count = 0;
}