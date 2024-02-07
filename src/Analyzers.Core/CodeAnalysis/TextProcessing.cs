namespace PodNet.Analyzers.CodeAnalysis;

public static class TextProcessing
{
    /// <summary>Trims the <c>Attribute</c> suffix from the name of <see cref="Attribute"/> types.</summary>
    /// <param name="attributeName">The attribute's name to trim.</param>
    /// <returns>The name of the attribute, without the <c>Attribute</c> suffix.</returns>
    public static string TrimAttributeSuffix(string attributeName)
        => attributeName.EndsWith("Attribute") && attributeName.Length > "Attribute".Length
            ? attributeName[..^"Attribute".Length]
            : attributeName;
}
