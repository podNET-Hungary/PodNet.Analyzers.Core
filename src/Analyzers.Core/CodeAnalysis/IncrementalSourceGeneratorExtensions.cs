using Microsoft.CodeAnalysis;

namespace PodNet.Analyzers.CodeAnalysis;

public static class IncrementalSourceGeneratorExtensions
{
    public static IncrementalValuesProvider<Diagnostic> SelectDiagnostics<T>(this IncrementalValuesProvider<DiagnosticsOrResults<T>> valuesProvider)
        => valuesProvider.SelectMany((e, _) => e.Diagnostics ?? []);

    public static void ReportDiagnostics(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<Diagnostic> diagnosticsProvider)
        => context.RegisterSourceOutput(diagnosticsProvider, (c, d) => c.ReportDiagnostic(d));

    public static void ReportDiagnostics<T>(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<DiagnosticsOrResults<T>> diagnosticsOrResultProvider)
        => context.ReportDiagnostics(diagnosticsOrResultProvider.SelectDiagnostics());

    public static IncrementalValuesProvider<T> SelectResults<T>(this IncrementalValuesProvider<DiagnosticsOrResults<T>> valuesProvider)
        => valuesProvider.SelectMany((e, _) => e.Results ?? []);

    public static void RegisterSourceOutputWithDiagnostics<T>(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<DiagnosticsOrResults<T>> diagnosticsOrResultsProvider,
        Action<SourceProductionContext, T> action)
    {
        context.ReportDiagnostics(diagnosticsOrResultsProvider);
        context.RegisterSourceOutput(diagnosticsOrResultsProvider.SelectResults(), action);
    }
}
