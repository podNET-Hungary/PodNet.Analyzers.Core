using PodNet.Analyzers.CodeAnalysis;

namespace PodNet.Analyzers.Tests;

[TestClass]
public class PathProcessingTests
{
    [DataTestMethod]
    [DataRow(".", @"C:\Users\source\Project1", @"C:\Users\source\Project1")]
    [DataRow(".", @"C:\Users\source\Project1", @"C:\Users\source\Project1\")]
    [DataRow(".", @"C:\Users\source\Project1\", @"C:\Users\source\Project1\")]
    [DataRow(@"C:\Users\source\Project1\", @"D:\Users\source\Project1", @"C:\Users\source\Project1\")]
    [DataRow("Items", @"C:\Users\source\Project1", @"C:\Users\source\Project1\Items")]
    [DataRow(@"Items\", @"C:\Users\source\Project1", @"C:\Users\source\Project1\Items\")]
    [DataRow(@"Item.cs", @"C:\Users\source\Project1", @"C:\Users\source\Project1\Item.cs")]
    [DataRow(@"Items\Item", @"C:\Users\source\Project1", @"C:\Users\source\Project1\Items\Item")]
    [DataRow(@"Items\Item.cs", @"C:\Users\source\Project1", @"C:\Users\source\Project1\Items\Item.cs")]
    [DataRow(@"..\..", @"C:\Users\source\Project1", @"C:\Users\")]
    [DataRow(@"..\Users", @"C:\User", @"C:\Users")]
    [DataRow(@"..\..\Users\source\", @"C:\User\source", @"C:\Users\source\")]
    public void GetRelativePath_WorksAsExpected(string expected, string relativeTo, string path)
    {
        var bclActual = Path.GetRelativePath(relativeTo, path);
        Assert.AreEqual(expected, bclActual);

        var actual = PathProcessing.GetRelativePath(relativeTo, path);
        Assert.AreEqual(expected, actual);
    }
}
