using Microsoft.CodeAnalysis;
using PodNet.Analyzers.Equality;
using System.Collections.Immutable;

namespace PodNet.Analyzers;

/// <summary>A simple container that wraps an append-only list of <see cref="Diagnostic"/> instances and can be converted to an <see cref="EquatableArray{T}"/>. Useful for passing around during model construction to collect diagnostic information.</summary>
public sealed class DiagnosticContainer
{
    private readonly List<Diagnostic> _diagnostics = [];
    public void Add(Diagnostic diagnostic) => _diagnostics.Add(diagnostic);
    public EquatableArray<Diagnostic> ToEquatableArray() => _diagnostics.ToImmutableArray();
}
