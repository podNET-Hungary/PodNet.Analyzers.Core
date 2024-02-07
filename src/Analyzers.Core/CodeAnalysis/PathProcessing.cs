using System.Runtime.InteropServices;
using System.Text;

namespace PodNet.Analyzers.CodeAnalysis;

public static class PathProcessing
{
    /// <summary>
    /// Create a relative path from one path to another. Paths will be resolved before calculating the difference.
    /// Default path comparison for the active platform will be used (OrdinalIgnoreCase for Windows or Mac, Ordinal for Unix).
    /// </summary>
    /// <remarks>.NET Standard 2.0 doesn't include the <c>Path.GetRelativePath</c> method. This clone is based on 
    /// <see href="https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/IO/Path.cs"/>.</remarks>
    /// <param name="relativeTo">The source path the output should be relative to. This path is always considered to be a directory.</param>
    /// <param name="path">The destination path.</param>
    /// <returns>The relative path or <paramref name="path"/> if the paths don't share the same root.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="relativeTo"/> or <paramref name="path"/> is <c>null</c> or an empty string.</exception>
    public static string GetRelativePath(string relativeTo, string path)
    {
        if (string.IsNullOrWhiteSpace(relativeTo))
            throw new ArgumentException($"Path cannot be null or whitespace.", nameof(relativeTo));
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException($"Path cannot be null or whitespace.", nameof(path));

        var ignoreCase = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        var comparisonType = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        relativeTo = Path.GetFullPath(relativeTo);
        path = Path.GetFullPath(path);

        if (string.Compare(Path.GetPathRoot(relativeTo), Path.GetPathRoot(path), comparisonType) != 0)
            return path;

        var commonLength = GetCommonPathLength(relativeTo, path, ignoreCase);
        if (commonLength == 0)
            return path;

        var relativeToLength = IsDirectorySeparator(relativeTo[^1]) ? relativeTo.Length - 1 : relativeTo.Length;
        var pathEndsInSeparator = IsDirectorySeparator(path[^1]);
        var pathLength = pathEndsInSeparator ? path.Length - 1 : path.Length;

        if (relativeToLength == pathLength && commonLength >= relativeToLength)
            return ".";

        var sb = new StringBuilder(Math.Max(relativeTo.Length, path.Length));

        if (commonLength < relativeToLength)
        {
            sb.Append("..");

            for (int i = commonLength + 1; i < relativeToLength; i++)
            {
                if (IsDirectorySeparator(relativeTo[i]))
                {
                    sb.Append(Path.DirectorySeparatorChar);
                    sb.Append("..");
                }
            }
        }
        else if (IsDirectorySeparator(path[commonLength]))
            commonLength++;

        int differenceLength = pathLength - commonLength;
        if (pathEndsInSeparator)
            differenceLength++;

        if (differenceLength > 0)
        {
            if (sb.Length > 0)
                sb.Append(Path.DirectorySeparatorChar);
            
            sb.Append(path[commonLength..(commonLength + differenceLength)]);
        }

        return sb.ToString();

        static bool IsDirectorySeparator(char c) => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;

        static int GetCommonPathLength(string first, string second, bool ignoreCase)
        {
            var commonChars = 0;
            while (first.Length > commonChars && second.Length > commonChars && string.Compare(first, commonChars, second, commonChars, 1, ignoreCase) == 0)
                commonChars++;

            if (commonChars == 0
                || (commonChars == first.Length && (commonChars == second.Length || IsDirectorySeparator(second[commonChars])))
                || (commonChars == second.Length && IsDirectorySeparator(first[commonChars])))
                return commonChars;

            while (commonChars > 0 && !IsDirectorySeparator(first[commonChars - 1]))
                commonChars--;

            return commonChars;
        }
    }
}
