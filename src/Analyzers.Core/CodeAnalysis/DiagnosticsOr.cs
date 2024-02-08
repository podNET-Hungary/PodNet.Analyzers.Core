﻿using Microsoft.CodeAnalysis;
using PodNet.Analyzers.Equality;
using System.Collections.Immutable;

namespace PodNet.Analyzers.CodeAnalysis;

/// <summary>A simple wrapper that holds any number of <see cref="Diagnostic"/> or <typeparamref name="T"/> instances. Useful to return from the initialization of a <see cref="SyntaxValueProvider"/> (<see cref="IncrementalValuesProvider{TValue}"/>), as it implements sequential value equality over the wrapped values.</summary>
/// <typeparam name="T">The target result instances to wrap.</typeparam>
/// <param name="Diagnostics">The diagnostics that should be reported.</param>
/// <param name="Results">The results that can be used for generation in the pipeline.</param>
public record DiagnosticsOrResults<T>(EquatableArray<Diagnostic>? Diagnostics, EquatableArray<T>? Results)
{
    /// <summary>A simple wrapper that holds any number of <see cref="Diagnostic"/> or <typeparamref name="T"/> instances. Useful to return from the initialization of a <see cref="SyntaxValueProvider"/> (<see cref="IncrementalValuesProvider{TValue}"/>), as it implements sequential value equality over the wrapped values.</summary>
    /// <param name="Diagnostics">The diagnostics that should be reported.</param>
    /// <param name="Result">The result that can be used for generation in the pipeline. Will be wrapped in an <see cref="EquatableArray{T}"/> with it being the sole value (when not <see langword="null"/>).</param>
    public DiagnosticsOrResults(EquatableArray<Diagnostic>? Diagnostics, T? Result) : this(Diagnostics, Result is null ? default : ImmutableArray.Create(Result)) { }
};