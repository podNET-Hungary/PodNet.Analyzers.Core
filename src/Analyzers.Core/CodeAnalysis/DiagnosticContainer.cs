using Microsoft.CodeAnalysis;
using PodNet.Analyzers.Equality;
using System.Collections.Immutable;

namespace PodNet.Analyzers;

/// <summary>A simple container that wraps an append-only list of <see cref="Diagnostic"/> instances and can be converted to an <see cref="EquatableArray{T}"/>. Useful for passing around during model construction to collect diagnostic information.</summary>
public sealed class DiagnosticContainer
{
    private readonly List<Diagnostic> _diagnostics = [];

    /// <summary>Adds a <see cref="Diagnostic"/> instance to the current container.</summary>
    /// <param name="diagnostic">The instance to add to the container.</param>
    public void Add(Diagnostic diagnostic) => _diagnostics.Add(diagnostic);

    /// <summary>Converts the <see cref="Diagnostic"/> instances in the container to an <see cref="EquatableArray{T}"/>.</summary>
    /// <returns>The array. The container can be appended to, while the returned array is immutable.</returns>
    public EquatableArray<Diagnostic> ToEquatableArray() => _diagnostics.ToImmutableArray();
}
