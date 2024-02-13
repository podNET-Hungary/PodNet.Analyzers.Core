using System.Text.RegularExpressions;

namespace PodNet.Analyzers.CodeAnalysis;

/// <summary>Contains helpers to process text values common in analyzer authoring.</summary>
public static class TextProcessing
{
    /// <summary>Trims the <c>Attribute</c> suffix from the name of <see cref="Attribute"/> types.</summary>
    /// <param name="attributeName">The attribute's name to trim.</param>
    /// <returns>The name of the attribute, without the <c>Attribute</c> suffix.</returns>
    public static string TrimAttributeSuffix(string attributeName)
        => attributeName.EndsWith("Attribute") && attributeName.Length > "Attribute".Length
            ? attributeName[..^"Attribute".Length]
            : attributeName;

    /// <summary>
    /// Sanitizes the given <paramref name="namespaceCandidate"/> to adhere to basic namespacing rules:
    /// <list type="bullet">
    /// <item>replaces consequtive path and segment separator characters ("separators") <c>'\'</c>, <c>'/'</c> and <c>'.'</c> with a single <c>'.'</c>,</item>
    /// <item>replaces characters that are not letters, numbers, underscores or separators each with one <c>'_'</c>,</item>
    /// <item>removes leading and trailing separators,</item>
    /// <item>appends an <c>'_'</c> before leading numbers in each segment.</item>
    /// </list>
    /// </summary>
    /// <param name="namespaceCandidate">The namespace to sanitize. Can be a relative path, but make sure that the path doesn't contain the filename itself, as it would become multiple namespace segments itself. It's advised to get make the path relative to the file's containing folder instead. See also <seealso cref="PathProcessing.GetRelativePath(string, string)"/>.</param>
    /// <returns>The sanitized namespace. Can only contain letters, (non-leading) numbers, underscores and dots. Can be empty, but cannot be whitespace or <see langword="null"/>.</returns>
    public static string GetNamespace(string namespaceCandidate)
        => NamespaceSanitizer.Replace(namespaceCandidate, static match =>
        {
            if (match.Groups["leadingOrTrailingDots"].Success)
                return "";
            if (match.Groups["namespaceSegmentLeadingNumber"].Success)
                return $"{(match.Groups["namespaceSegmentLeadingNumberDot"].Success ? "." : "")}_{match.Groups["namespaceSegmentLeadingNumber"].Value}";
            if (match.Groups["pathOrSegmentSeparators"].Success)
                return ".";
            if (match.Groups["invalidChars"] is { Success: true, Length: var invalidCharCount })
                return new('_', invalidCharCount);
            throw new InvalidOperationException("Regex pattern matched, but none of the provided cases handled the match.");
        });

    private static Regex NamespaceSanitizer { get; } = new Regex(@"(?<leadingOrTrailingDots>^[./\\]+|[./\\]+$)|(?:(?:^|(?<namespaceSegmentLeadingNumberDot>[./\\]+))(?<namespaceSegmentLeadingNumber>\d))|(?<pathOrSegmentSeparators>[./\\]+)|(?<invalidChars>[^._\w\d]+)", RegexOptions.Compiled
#if !DEBUG
        , TimeSpan.FromMilliseconds(100));
#else
        );
#endif

    /// <summary>
    /// Sanitizes the given <paramref name="classNameCandidate"/> to adhere to basic class naming rules:
    /// <list type="bullet">
    /// <item>replaces characters that are not letters, numbers or underscores each with one <c>'_'</c>,</item>
    /// <item>if <paramref name="classNameCandidate"/> starts with a number, appends an <c>'_'</c> before it,</item>
    /// <item>if <paramref name="classNameCandidate"/> is empty, returns <c>'_'</c>.</item>
    /// </list>
    /// </summary>
    /// <param name="classNameCandidate">The classname to sanitize. Possibly a file name without the extension (see <see cref="Path.GetFileNameWithoutExtension(string)"/>).</param>
    /// <returns>The sanitized class name. Can only contain letters, (non-leading) numbers and underscores. Can not be empty, whitespace or <see langword="null"/>.</returns>
    public static string GetClassName(string classNameCandidate)
        => ClassNameSanitizer.Replace(classNameCandidate, static match =>
        {
            if (match.Groups["leadingNumber"] is { Success: true, Value: var leadingNumber })
                return $"_{leadingNumber}";
            if (match.Groups["invalidChars"] is { Success: true, Length: var invalidCharCount })
                return new('_', invalidCharCount);
            if (match.Groups["isEmpty"].Success)
                return "_";
            throw new InvalidOperationException("Regex pattern matched, but none of the provided cases handled the match.");
        });

    private static Regex ClassNameSanitizer { get; } = new Regex(@"(?:^(?<leadingNumber>\d))|(?<invalidChars>[^_\w\d]+)|(?<isEmpty>^$)", RegexOptions.Compiled
#if !DEBUG
        , TimeSpan.FromMilliseconds(100));
#else
    );
#endif
}
