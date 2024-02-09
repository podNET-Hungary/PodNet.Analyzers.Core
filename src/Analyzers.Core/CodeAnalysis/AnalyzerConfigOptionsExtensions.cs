using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace PodNet.Analyzers.CodeAnalysis;

/// <summary>Extensions to retrieve values from <see cref="AnalyzerConfigOptions"/>.</summary>
public static class AnalyzerConfigOptionsExtensions
{
    /// <summary>Gets the value corresponding to <paramref name="key"/> from <paramref name="options"/>, or <see langword="null"/> if not found.</summary>
    /// <param name="options">The options to look up the provided key in.</param>
    /// <param name="key">The key to look up.</param>
    /// <returns>The value corresponding to <paramref name="key"/> if found, <see langword="null"/> otherwise.</returns>
    public static string? GetValue(this AnalyzerConfigOptions options, string key)
    {
        options.TryGetValue(key, out var value);
        return value;
    }

    /// <summary>Gets the value corresponding to <c>$"build_property.{<paramref name="key"/>}"</c> from <paramref name="options"/>, or <see langword="null"/> if not found.</summary>
    /// <param name="options">The options to look up the provided key in.</param>
    /// <param name="key">The key to look up.</param>
    /// <returns>The value corresponding to <paramref name="key"/> if found, <see langword="null"/> otherwise.</returns>
    public static string? GetBuildProperty(this AnalyzerConfigOptions options, string key)
        => options.GetValue($"build_property.{key}");

    /// <summary>Gets the value corresponding to <c>$"build_metadata.{<paramref name="key"/>}"</c> from <paramref name="options"/>, or <see langword="null"/> if not found.</summary>
    /// <remarks>To be used with <see cref="AnalyzerConfigOptionsProvider.GetOptions(AdditionalText)"/> or  <see cref="AnalyzerConfigOptionsProvider.GetOptions(SyntaxTree)"/>, and not with <see cref="AnalyzerConfigOptionsProvider.GlobalOptions"/>.</remarks>
    /// <param name="options">The options to look up the provided key in.</param>
    /// <param name="key">The key to look up.</param>
    /// <returns>The value corresponding to <paramref name="key"/> if found, <see langword="null"/> otherwise.</returns>
    public static string? GetMetadata(this AnalyzerConfigOptions options, string key)
        => options.GetValue($"build_metadata.{key}");

    /// <summary>Gets the value corresponding to <c>$"build_metadata.additionalfiles.{<paramref name="key"/>}"</c> from <paramref name="options"/>, or <see langword="null"/> if not found.</summary>
    /// <remarks>To be used with <see cref="AnalyzerConfigOptionsProvider.GetOptions(AdditionalText)"/> or  <see cref="AnalyzerConfigOptionsProvider.GetOptions(SyntaxTree)"/>, and not with <see cref="AnalyzerConfigOptionsProvider.GlobalOptions"/>.</remarks>
    /// <param name="options">The options to look up the provided key in.</param>
    /// <param name="key">The key to look up.</param>
    /// <returns>The value corresponding to <paramref name="key"/> if found, <see langword="null"/> otherwise.</returns>
    public static string? GetAdditionalTextMetadata(this AnalyzerConfigOptions options, string key)
        => options.GetMetadata($"additionalfiles.{key}");

    /// <summary>Gets the root namespace by using the key <c>build_property.rootnamespace</c> in MSBuild.</summary>
    public static string? GetRootNamespace(this AnalyzerConfigOptions options)
        => options.GetBuildProperty("rootnamespace");

    /// <summary>Gets the target framework from MSBuild by using the key <c>build_property.targetframework</c>. If multitargeting, will return the currently executing target only.</summary>
    public static string? GetTargetFramework(this AnalyzerConfigOptions options)
        => options.GetBuildProperty("targetframework");

    /// <summary>Gets the project directory (if defined) in MSBuild by using the keys <c>build_property.projectdir</c> and <c>build_property.msbuildprojectdirectory</c>.</summary>
    public static string? GetProjectDirectory(this AnalyzerConfigOptions options)
        => options.GetBuildProperty("projectdir") ?? options.GetBuildProperty("msbuildprojectdirectory");

    /// <summary>Gets all avaiable key-values as they are avaiable in the current <paramref name="options"/>. Note that available keys might still contain <see langword="null"/> or empty values.</summary>
    public static ImmutableArray<(string Key, string? Value)> GetValues(this AnalyzerConfigOptions options)
        => options.Keys.Select(key => (key, options.GetValue(key))).ToImmutableArray();
}
