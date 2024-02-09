using Microsoft.CodeAnalysis;

namespace PodNet.Analyzers.CodeAnalysis;

/// <summary>Extensions for authoring <see cref="IIncrementalGenerator"/>s.</summary>
public static class IncrementalSourceGeneratorExtensions
{
    /// <summary>Projects the provided <paramref name="valuesProvider"/> to the <see cref="Diagnostic"/>s contained within them.</summary>
    /// <typeparam name="T">The type parameter for the <see cref="DiagnosticsOrResults{T}"/> wrapper.</typeparam>
    /// <param name="valuesProvider">The provider to project the <see cref="Diagnostic"/>s from.</param>
    /// <returns>The <see cref="Diagnostic"/>s from the <paramref name="valuesProvider"/> (empty if they were <see langword="null"/>).</returns>
    public static IncrementalValuesProvider<Diagnostic> SelectDiagnostics<T>(this IncrementalValuesProvider<DiagnosticsOrResults<T>> valuesProvider)
        => valuesProvider.SelectMany((e, _) => e.Diagnostics ?? []);

    /// <summary>Registers a source output that reports the diagnostics for the given <see cref="Diagnostic"/>s.</summary>
    /// <param name="context">The context to call <see cref="IncrementalGeneratorInitializationContext.RegisterSourceOutput{TSource}(IncrementalValueProvider{TSource}, Action{SourceProductionContext, TSource})"/> and report the <see cref="Diagnostic"/>s on.</param>
    /// <param name="diagnosticsProvider">The provider of <see cref="Diagnostic"/>s.</param>
    public static void ReportDiagnostics(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<Diagnostic> diagnosticsProvider)
        => context.RegisterSourceOutput(diagnosticsProvider, (c, d) => c.ReportDiagnostic(d));

    /// <summary>Registers a source output that reports the diagnostics for the given <see cref="Diagnostic"/>s in the <see cref="DiagnosticsOrResults{T}"/> provider.</summary>
    /// <param name="context">The context to call <see cref="IncrementalGeneratorInitializationContext.RegisterSourceOutput{TSource}(IncrementalValueProvider{TSource}, Action{SourceProductionContext, TSource})"/> and report the <see cref="Diagnostic"/>s on.</param>
    /// <param name="diagnosticsOrResultProvider">The provider of <see cref="DiagnosticsOrResults{T}"/>s.</param>
    public static void ReportDiagnostics<T>(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<DiagnosticsOrResults<T>> diagnosticsOrResultProvider)
        => context.ReportDiagnostics(diagnosticsOrResultProvider.SelectDiagnostics());

    /// <summary>Projects the provided <paramref name="valuesProvider"/> to the <see cref="DiagnosticsOrResults{T}.Results"/> contained within them.</summary>
    /// <typeparam name="T">The type parameter for the <see cref="DiagnosticsOrResults{T}"/> wrapper.</typeparam>
    /// <param name="valuesProvider">The provider to project the <see cref="DiagnosticsOrResults{T}"/>s from.</param>
    /// <returns>The provider for <typeparamref name="T"/> items from the <paramref name="valuesProvider"/> (empty if they were <see langword="null"/>).</returns>
    public static IncrementalValuesProvider<T> SelectResults<T>(this IncrementalValuesProvider<DiagnosticsOrResults<T>> valuesProvider)
        => valuesProvider.SelectMany((e, _) => e.Results ?? []);

    /// <summary>Separately registers a source output for reporting diagnostics (see <seealso cref="ReportDiagnostics{T}(IncrementalGeneratorInitializationContext, IncrementalValuesProvider{DiagnosticsOrResults{T}})"/>) and another to produce source from the <see cref="DiagnosticsOrResults{T}.Results"/> in <paramref name="diagnosticsOrResultsProvider"/>.</summary>
    /// <typeparam name="T">The type of the results in <paramref name="diagnosticsOrResultsProvider"/>.</typeparam>
    /// <param name="context">The context to register the source outputs to.</param>
    /// <param name="diagnosticsOrResultsProvider">The provider for the diagnostics and results.</param>
    /// <param name="action">The action to execute on the registered source output.</param>
    public static void RegisterSourceOutputWithDiagnostics<T>(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<DiagnosticsOrResults<T>> diagnosticsOrResultsProvider,
        Action<SourceProductionContext, T> action)
    {
        context.ReportDiagnostics(diagnosticsOrResultsProvider);
        context.RegisterSourceOutput(diagnosticsOrResultsProvider.SelectResults(), action);
    }
}
