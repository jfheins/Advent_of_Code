using static MoreLinq.Extensions.IndexExtension;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Core
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "Collection is a minor concern")]
    public class FiniteGrid2D<TNode> : ICollection<(Point pos, TNode value)>
        where TNode : notnull
    {
        public Rectangle Bounds { get => _bounds.Clone(); private set => _bounds = value; }
        public int Count => _values.Count;
        public bool IsReadOnly { get; }
        public int Width => Bounds.Width;
        public int Height => Bounds.Height;
        public Size Size => Bounds.Size;
        public Point TopLeft => Bounds.Location;
        public Point BottomRight => new(Width - 1, Height - 1);

        protected readonly Dictionary<Point, TNode> _values = new();
        private Rectangle _bounds;

        public FiniteGrid2D(int width, int height, TNode value)
            : this(width, height, p => value) { }

        public FiniteGrid2D(int width, int height, Func<int, int, TNode> dataCallback)
            : this(width, height, p => dataCallback(p.X, p.Y)) { }

        public FiniteGrid2D(Size size, Func<Point, TNode> dataCallback)
            : this(new Rectangle(Point.Empty, size), dataCallback) { }

        public FiniteGrid2D(int width, int height, Func<Point, TNode> dataCallback)
            : this(new Rectangle(0, 0, width, height), dataCallback) { }

        public FiniteGrid2D(Rectangle bounds, Func<Point, TNode> dataCallback)
        {
            Bounds = bounds;
            Fill(dataCallback);
        }

        public FiniteGrid2D(IEnumerable<IEnumerable<TNode>> data)
        {
            foreach (var (x, y, value) in data.WithXY())
                this[x, y] = value;
            var maxx = _values.Max(kvp => kvp.Key.X);
            var maxy = _values.Max(kvp => kvp.Key.Y);
            Bounds = new Rectangle(0, 0, maxx + 1, maxy + 1);
        }
        public FiniteGrid2D(FiniteGrid2D<TNode> source)
        {
            Contract.Assert(source != null);
            _values = new Dictionary<Point, TNode>(source._values);
            Bounds = source.Bounds;
        }
        public FiniteGrid2D(FiniteGrid2D<TNode> source, int inflation, TNode fillValue)
        {
            Contract.Assert(source != null);
            Contract.Assert(inflation >= 0, "inflation must be positive");

            Bounds = source.Bounds.InflatedCopy(inflation, inflation);
            Fill(p => source.GetValueOrDefault(p, fillValue));
        }

        public IEnumerable<Point> Keys => _values.Keys;

        public void EnlargeTop(int amount)
        {
            Bounds = new Rectangle(Bounds.X, Bounds.Y - amount, Bounds.Width, Bounds.Height + amount);
        }

        public void SizeToFit()
        {
            var (minX, maxX, minY, maxY) = (int.MaxValue, int.MinValue, int.MaxValue, int.MinValue);
            foreach (var p in _values.Keys)
            {
                if (p.X < minX)
                    minX = p.X;
                if (p.X > maxX)
                    maxX = p.X;
                if (p.Y < minY)
                    minY = p.Y;
                if (p.Y > maxY)
                    maxY = p.Y;
            }
            _bounds = new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }

        private void Fill(Func<Point, TNode?> dataCallback)
        {
            for (int y = Bounds.Top; y < Bounds.Bottom; y++)
            {
                for (int x = Bounds.Left; x < Bounds.Right; x++)
                {
                    var p = new Point(x, y);
                    var val = dataCallback(p);
                    if (val != null)
                        _values[p] = val;
                }
            }
        }

        public void FillGaps(TNode fillValue)
        {
            for (int y = Bounds.Top; y < Bounds.Bottom; y++)
            {
                for (int x = Bounds.Left; x < Bounds.Right; x++)
                {
                    var p = new Point(x, y);
                    if (!_values.ContainsKey(p))
                        _values[p] = fillValue;
                }
            }
        }

        public virtual bool Contains(Point pos) => _bounds.Contains(pos);
        public virtual bool ContainsKey(Point pos) => _values.ContainsKey(pos);

        public virtual TNode this[int x, int y]
        {
            get => this[new Point(x, y)];
            set => this[new Point(x, y)] = value;
        }

        public virtual TNode this[Point pos]
        {
            get => _values[pos];
            set => _values[pos] = value;
        }

        public virtual TNode GetValueOrDefault(Point pos, TNode defaultValue)
            => _values.GetValueOrDefault(pos, defaultValue);

        private static int Wrap(int coord, int limit) => ((coord % limit) + limit) % limit;
        public virtual TNode GetValueWraparound(Point p) => GetValueWraparound(p.X, p.Y);
        public virtual TNode GetValueWraparound(int x, int y)
        {
            var wrapped = new Point(Wrap(x, Width), Wrap(y, Height));
            return _values[wrapped];

        }
        public Point FindFirst(TNode x) => _values.First(kvp => kvp.Value.Equals(x)).Key;
        public virtual (Point pos, TNode value) GetTupleWraparound(Point p) => GetTupleWraparound(p.X, p.Y);
        public virtual (Point pos, TNode value) GetTupleWraparound(int x, int y)
        {
            var wrapped = new Point(Wrap(x, Width), Wrap(y, Height));
            return (wrapped, _values[wrapped]);
        }

        public virtual void SetValueWraparound(Point p, TNode value)
        {
            var wrapped = new Point(Wrap(p.X, Width), Wrap(p.Y, Height));
            _values[wrapped] = value;

            static int Wrap(int coord, int limit) => ((coord % limit) + limit) % limit;
        }

        public virtual IEnumerable<Point> Get4NeighborsOf(Point pos)
            => pos.MoveLURD().Where(Contains);

        public virtual IEnumerable<Point> Get8NeighborsOf(Point pos)
            => pos.MoveLURDDiag().Where(Contains);

        public IEnumerable<TNode> GetPointWith8Neighbors(Point pos, TNode defaultValue)
        {
            for (int dy = -1; dy <= 1; dy++)
                for (int dx = -1; dx <= 1; dx++)
                    yield return _values.GetValueOrDefault(pos.MoveBy(dx, dy), defaultValue);
        }

        public override string ToString() => ToString('?');
        public string ToString(char filler)
        {
            var sb = new StringBuilder();
            for (int y = Bounds.Top; y < Bounds.Bottom; y++)
            {
                sb.AppendFormat("{0,5}|", y);
                for (int x = Bounds.Left; x < Bounds.Right; x++)
                {
                    if (_values.TryGetValue(new Point(x, y), out var v))
                        sb.Append(v.ToString());
                    else
                        sb.Append(filler);
                }
                _ = sb.AppendLine();
            }
            return sb.ToString();
        }

        public IEnumerable<TNode> LineV(Point pos, Size direction) => LineP(pos, direction).Select(it => this[it]);

        public IEnumerable<Point> LineP(Point pos, Size direction)
        {
            while (Contains(pos))
            {
                yield return pos;
                pos += direction;
            }
        }


        public IEnumerable<IEnumerable<Point>> Lines(Point exclusiveStart, Direction[] dirs)
        {
            var range = Enumerable.Range(1, int.MaxValue);
            return dirs.Select(d => d.ToSize())
                .Select(dir => range.Select(n => exclusiveStart + n * dir)
                .TakeWhile(it => _bounds.Contains(it)));
        }

        public IEnumerable<TNode> Line(Point pos, Direction dir) => LineP(pos, dir.ToSize()).Select(it => this[it]);

        public void Add((Point pos, TNode value) item) => _values.Add(item.pos, item.value);
        public void Clear() => _values.Clear();
        public bool Contains((Point pos, TNode value) item)
            => _values.Contains(new KeyValuePair<Point, TNode>(item.pos, item.value));
        public void CopyTo((Point pos, TNode value)[] array, int arrayIndex)
        {
            foreach (var kvp in _values)
                array[arrayIndex++] = (kvp.Key, kvp.Value);
        }

        public bool Remove((Point pos, TNode value) item) => _values.Remove(item.pos);
        public IEnumerator<(Point pos, TNode value)> GetEnumerator() => new EnumWrapper(_values);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<TNode> GetEdge(Direction dir)
        {
            var dx = Enumerable.Empty<int>();
            var dy = Enumerable.Empty<int>();
            if (dir == Direction.Up)
            {
                dx = Enumerable.Range(Bounds.X, Bounds.Width);
                dy = Bounds.Y.ToEnumerable();
            }
            if (dir == Direction.Down)
            {
                dx = Enumerable.Range(Bounds.X, Bounds.Width);
                dy = (Bounds.Bottom - 1).ToEnumerable();
            }
            if (dir == Direction.Left)
            {
                dx = Bounds.X.ToEnumerable();
                dy = Enumerable.Range(Bounds.Y, Bounds.Height);
            }
            if (dir == Direction.Right)
            {
                dx = (Bounds.Right - 1).ToEnumerable();
                dy = Enumerable.Range(Bounds.Y, Bounds.Height);
            }
            return dx.CartesianProduct(dy).Select(t => this[t.Item1, t.Item2]);
        }

        public void SetRow(int y, IEnumerable<TNode> values)
        {
            foreach (var item in values.Index())
            {
                this[item.Key + Bounds.Left, y] = item.Value;
            }
        }

        public IEnumerable<TNode> GetRow(int y, TNode defaultValue)
        {
            return Enumerable.Range(Bounds.Left, Bounds.Width)
                .Select(x => GetValueOrDefault(new Point(x, y), defaultValue));
        }

        public IEnumerable<(int y, IReadOnlyList<TNode> cells)> AllRows(TNode defaultValue)
        {
            return GetRowIndices().Select(y => (y, GetRow(y, defaultValue).ToList() as IReadOnlyList<TNode>));
        }

        public IEnumerable<(int y, IEnumerable<TNode> cells)> FilledRows()
        {
            var byY = _values.ToLookup(it => it.Key.Y, it => it.Value);
            return GetRowIndices().Select(y => (y, byY[y]));
        }

        public IEnumerable<(Point pos, TNode value)> GetRowTuple(int y, TNode defaultValue)
        {
            return Enumerable.Range(Bounds.Left, Bounds.Width)
                .Select(x => new Point(x, y))
                .Select(p => (p, GetValueOrDefault(p, defaultValue)));
        }

        public IEnumerable<(Point pos, TNode value)> GetRowTuple(int y)
        {
            return _values.Where(p => p.Key.Y == y).Select(t => (t.Key, t.Value));
        }

        public IEnumerable<int> GetRowIndices() => Enumerable.Range(Bounds.Top, Bounds.Height);

        public void RemoveAt(Point oldpoint)
        {
            _ = _values.Remove(oldpoint);
        }

        public IEnumerable<int> GetColIndices() => Enumerable.Range(Bounds.Left, Bounds.Width);

        public IEnumerable<TNode> GetCol(int x, TNode defaultValue)
        {
            return Enumerable.Range(Bounds.Top, Bounds.Height)
                .Select(y => GetValueOrDefault(new Point(x, y), defaultValue));
        }

        public IEnumerable<(Point pos, TNode value)> GetColTuple(int x)
        {
            return _values.Where(p => p.Key.X == x).Select(t => (t.Key, t.Value));
        }

        public void SetCol(int x, IEnumerable<TNode> values)
        {
            foreach (var item in values.Index())
            {
                this[x, item.Key + Bounds.Top] = item.Value;
            }
        }

        public IEnumerable<(int x, IReadOnlyList<TNode> cells)> AllCols(TNode defaultValue)
        {
            return GetColIndices().Select(x => (x, GetCol(x, defaultValue).ToList() as IReadOnlyList<TNode>));
        }

        public IEnumerable<(int x, IEnumerable<TNode> cells)> FilledCols()
        {
            var byX = _values.ToLookup(it => it.Key.X, it => it.Value);
            return GetColIndices().Select(x => (x, byX[x]));
        }

        public IEnumerable<(Point pos, TNode value)> GetColTuple(int x, TNode defaultValue)
        {
            return Enumerable.Range(Bounds.Top, Bounds.Height)
                .Select(y => new Point(x, y))
                .Select(p => (p, GetValueOrDefault(p, defaultValue)));
        }

        public void RemoveWhere(Func<Point, bool> predicate)
        {
            foreach (var item in _values.Keys.Where(predicate).ToList())
            {
                _values.Remove(item);
            }
        }

        public void RemoveAll(TNode value)
        {
            foreach (var item in _values.Where(kvp => kvp.Value.Equals(value)).ToList())
            {
                _values.Remove(item.Key);
            }
        }

        public Point MoveWhile(Direction dir, Point pos, Func<TNode, bool> predicate)
        {
            Debug.Assert(predicate(this[pos]));
            var next = pos.MoveTo(dir);
            while (Contains(next) && predicate(this[next]))
            {
                pos = next;
                next = pos.MoveTo(dir);
            }
            return pos;
        }

        public void InsertRow(int y, TNode value)
        {
            Bounds = Bounds with { Height = Height + 1 };
            for (var i = Bounds.Bottom - 1; i >= y; i--)
                SetRow(i, GetRow(i - 1, value));
            SetRow(y, Enumerable.Repeat(value, Bounds.Width));
        }

        public void InsertEmptyRows(int y, int num = 1)
        {
            var moveInterval = new Interval(y, Bounds.Bottom);
            Bounds = Bounds with { Height = Height + num };
            var points = _values.Where(t => moveInterval.Contains(t.Key.Y)).ToList();
            foreach (var (pos, value) in points)
            {
                RemoveAt(pos);
                this[pos.X, pos.Y + num] = value;
            }
        }

        public void InsertCol(int x, TNode value)
        {
            Bounds = Bounds with { Width = Width + 1 };
            for (var i = Bounds.Right - 1; i >= x; i--)
                SetCol(i, GetCol(i - 1, value));
            SetCol(x, Enumerable.Repeat(value, Bounds.Height));
        }

        public void InsertEmptyCols(int x, int num = 1)
        {
            var moveInterval = new Interval(x, Bounds.Right);
            Bounds = Bounds with { Width = Width + num };
            var points = _values.Where(t => moveInterval.Contains(t.Key.X)).ToList();
            foreach (var (pos, value) in points)
            {
                RemoveAt(pos);
                this[pos.X + num, pos.Y] = value;
            }
        }

        private readonly struct EnumWrapper : IEnumerator<(Point pos, TNode value)>
        {
            private readonly IEnumerator<KeyValuePair<Point, TNode>> _enumerator;

            public EnumWrapper(Dictionary<Point, TNode> values) : this()
            {
                this._enumerator = values.GetEnumerator();
            }

            private (Point pos, TNode value) Convert(KeyValuePair<Point, TNode> kvp)
                => (kvp.Key, kvp.Value);

            public (Point pos, TNode value) Current => Convert(_enumerator.Current);
            object? IEnumerator.Current => Current;

            public void Dispose() => _enumerator.Dispose();
            public bool MoveNext() => _enumerator.MoveNext();
            public void Reset() => _enumerator.Reset();
        }

        public bool Equals(FiniteGrid2D<TNode> other)
        {
            if (Bounds != other.Bounds)
                return false;

            return Keys.All(p => this[p].Equals(other[p]));
        }
    }
}
