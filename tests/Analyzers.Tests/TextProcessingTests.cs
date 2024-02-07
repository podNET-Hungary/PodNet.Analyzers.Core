using PodNet.Analyzers.CodeAnalysis;

namespace PodNet.Analyzers.Tests;

[TestClass]
public class TextProcessingTests
{
    [DataTestMethod]
    [DataRow("ValuesAttribute", "Values")]
    [DataRow("ValuesAttributeAttribute", "ValuesAttribute")]
    [DataRow("AttributeAttribute", "Attribute")]
    [DataRow("ValuesAttributes")]
    [DataRow("Attribute")]
    [DataRow("Attributes")]
    [DataRow("Valuesattribute")]
    public void TrimAttributeSuffix_TrimsAsExpected(string input, string? expected = null)
    {
        var actual = TextProcessing.TrimAttributeSuffix(input);
        expected ??= input;
        Assert.AreEqual(expected, actual);
    }
}
