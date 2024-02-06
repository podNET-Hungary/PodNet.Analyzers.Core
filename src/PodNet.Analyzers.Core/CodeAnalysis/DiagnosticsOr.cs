using Microsoft.CodeAnalysis;
using PodNet.Analyzers.Equality;
using System.Collections.Immutable;

namespace PodNet.Analyzers.CodeAnalysis;

/// <summary>A simple wrapper that holds any number of <see cref="Diagnostic"/> or <typeparamref name="T"/> instances. Useful to return from the initialization of a <see cref="SyntaxValueProvider"/> (<see cref="IncrementalValuesProvider{TValues}"/>), as it implements sequential value equality over the wrapped values.</summary>
/// <typeparam name="T">The target result instances to wrap.</typeparam>
/// <param name="Diagnostics">The diagnostics that should be reported.</param>
/// <param name="Results">The results that can be used for generation in the pipeline.</param>
public record DiagnosticsOrResults<T>(EquatableArray<Diagnostic>? Diagnostics, EquatableArray<T>? Results)
{
    public DiagnosticsOrResults(EquatableArray<Diagnostic>? Diagnostics, T? Result) : this(Diagnostics, Result is null ? default : ImmutableArray.Create(Result)) { }
};