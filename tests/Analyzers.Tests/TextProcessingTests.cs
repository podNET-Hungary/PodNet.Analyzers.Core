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

    [DataTestMethod]
    [DataRow("", "")]
    [DataRow("Namespace", "Namespace")]
    [DataRow("Namespace.SubNamespace", "Namespace.SubNamespace")]
    [DataRow("./Folder", "Folder")]
    [DataRow("A/B", "A.B")]
    [DataRow("A.B", "A.B")]
    [DataRow("..//../A/\\B/.\\", "A.B")]
    [DataRow("..\\..\\", "")]
    [DataRow("A & B/C & D", "A___B.C___D")]
    [DataRow("11", "_11")]
    [DataRow("11/22", "_11._22")]
    [DataRow("../ A & B.0//", "_A___B._0")]
    [DataRow("./Folder/File.ext", "Folder.File.ext")]
    public void NamespaceSanitization_WorksAsExpected(string input, string expected)
    {
        var actual = TextProcessing.GetNamespace(input);
        Assert.AreEqual(expected, actual);
    }


    [DataTestMethod]
    [DataRow("", "_")]
    [DataRow("Filename", "Filename")]
    [DataRow("Filename.ext", "Filename_ext")]
    [DataRow("1 File", "_1_File")]
    [DataRow("./Folder/1 File .ext", "__Folder_1_File__ext")]
    public void ClassNameSanitization_WorksAsExpected(string input, string expected)
    {
        var actual = TextProcessing.GetClassName(input);
        Assert.AreEqual(expected, actual);
    }
}
