using System.Collections;
using System.Collections.Immutable;

namespace PodNet.Analyzers.Equality;

/// <summary>
/// A simple struct that holds an <see cref="ImmutableArray{T}"/> instance and implements value equality over its items using sequence equality. Can be implicitly converted to and from an <see cref="ImmutableArray{T}"/>. Forces an empty array instead of an uninitialized (null) one.
/// </summary>
/// <typeparam name="T">The type of items in the array.</typeparam>
/// <param name="array">The array to wrap.</param>
public readonly struct EquatableArray<T>(ImmutableArray<T> array) : IEquatable<EquatableArray<T>>, IEnumerable<T>
{
    /// <summary>Creates an empty array.</summary>
    public EquatableArray() : this([]) { }

    /// <summary>The values being wrapped (or <see cref="ImmutableArray{T}.Empty"/> if empty or unitialized).</summary>
    public ImmutableArray<T> Values { get; } = array.IsDefault ? [] : array;

    /// <summary>The number of items in the array.</summary>
    public int Length => Values.Length;

    /// <summary>Gets the element at the given index.</summary>
    /// <param name="index">The index of the element.</param>
    /// <returns>The element at the given index.</returns>
    public T this[int index] => Values[index];

    /// <summary>Indicates if the current instance is <b>structurally equivalent</b> to <paramref name="other"/>.</summary>
    /// <inheritdoc/>
    /// <remarks>Uses sequential equality for comparison.</remarks>
    public bool Equals(EquatableArray<T> other) => (Values.IsDefaultOrEmpty && other.Values.IsDefaultOrEmpty) || Values.SequenceEqual(other.Values);

    /// <inheritdoc cref="Equals(EquatableArray{T})"/>
    public bool Equals(IEnumerable<T> other) => Values.SequenceEqual(other);

    /// <returns><see langword="true"/> only if <paramref name="other"/> is an <see cref="EquatableArray{T}"/> or <see cref="IEnumerable{T}"/> instance that is deemed <b>structurally equivalent</b>.</returns>
    /// <inheritdoc cref="Equals(EquatableArray{T})"/>
    public override bool Equals(object other) => (other is EquatableArray<T> equatableArray && Equals(equatableArray)) || (other is IEnumerable<T> enumerable && Equals(enumerable));

    /// <inheritdoc/>
    public override int GetHashCode() => Values.Aggregate(0x5bd1e995, (acc, v) => (acc >> 17 | acc << sizeof(int) - 17) ^ (v?.GetHashCode() ?? 0));

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Values).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Values).GetEnumerator();

    /// <summary>Implicitly converts <paramref name="array"/> to its wrapped <see cref="Values"/>.</summary>
    /// <param name="array">The wrapper to unwrap.</param>
    public static implicit operator ImmutableArray<T>(EquatableArray<T> array) => array.Values;

    /// <summary>Implicitly wraps <paramref name="array"/>.</summary>
    /// <param name="array">The wrapper that contains a reference to <paramref name="array"/>.</param>
    public static implicit operator EquatableArray<T>(ImmutableArray<T> array) => new(array.IsDefault ? [] : array);

    /// <summary>Return the <see cref="Values"/> wrapped by the current instance.</summary>
    /// <returns>The <see cref="Values"/> wrapped by the current instance.</returns>
    public ImmutableArray<T> ToImmutableArray() => Values;

    /// <summary>Checks if the provided arrays are <see cref="Equals(EquatableArray{T})"/>.</summary>
    /// <param name="left">One of the instances.</param>
    /// <param name="right">Another instance.</param>
    /// <returns><see langword="true"/> if <see cref="Equals(EquatableArray{T})"/>.</returns>
    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right) => left.Equals(right);

    /// <summary>Checks if the provided arrays are not <see cref="Equals(EquatableArray{T})"/>.</summary>
    /// <param name="left">One of the instances.</param>
    /// <param name="right">Another instance.</param>
    /// <returns><see langword="false"/> if <see cref="Equals(EquatableArray{T})"/>.</returns>
    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right) => !(left == right);

    /// <summary>Checks if the provided arrays are <see cref="Equals(IEnumerable{T})"/>.</summary>
    /// <param name="left">One of the instances.</param>
    /// <param name="right">Another instance.</param>
    /// <returns><see langword="true"/> if <see cref="Equals(IEnumerable{T})"/>.</returns>
    public static bool operator ==(EquatableArray<T> left, IEnumerable<T> right) => left.Equals(right);

    /// <summary>Checks if the provided arrays are not <see cref="Equals(IEnumerable{T})"/>.</summary>
    /// <param name="left">One of the instances.</param>
    /// <param name="right">Another instance.</param>
    /// <returns><see langword="false"/> if <see cref="Equals(IEnumerable{T})"/>.</returns>
    public static bool operator !=(EquatableArray<T> left, IEnumerable<T> right) => !(left == right);
}