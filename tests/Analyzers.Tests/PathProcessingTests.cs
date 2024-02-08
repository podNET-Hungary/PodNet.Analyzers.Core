using PodNet.Analyzers.CodeAnalysis;

namespace PodNet.Analyzers.Tests;

[TestClass]
public class PathProcessingTests
{
    [DataTestMethod]
    [DataRow(@"C:\Users\source\Project1", @"C:\Users\source\Project1")]
    [DataRow(@"C:\Users\source\Project1", @"C:\Users\source\Project1\")]
    [DataRow(@"C:\Users\source\Project1\", @"C:\Users\source\Project1\")]
    [DataRow(@"D:\Users\source\Project1", @"C:\Users\source\Project1\")]
    [DataRow(@"C:\Users\source\Project1", @"C:\Users\source\Project1\Items")]
    [DataRow(@"C:\Users\source\Project1", @"C:\Users\source\Project1\Items\")]
    [DataRow(@"C:\Users\source\Project1", @"C:\Users\source\Project1\Item.cs")]
    [DataRow(@"C:\Users\source\Project1", @"C:\Users\source\Project1\Items\Item")]
    [DataRow(@"C:\Users\source\Project1", @"C:\Users\source\Project1\Items\Item.cs")]
    [DataRow(@"C:\Users\source\Project1", @"C:\Users\")]
    [DataRow(@"C:\User", @"C:\Users")]
    [DataRow(@"C:\User\source", @"C:\Users\source\")]
    public void GetRelativePath_WorksAsExpected(string relativeTo, string path)
    {
        var expected = Path.GetRelativePath(relativeTo, path);
        var actual = PathProcessing.GetRelativePath(relativeTo, path);
        Assert.AreEqual(expected, actual);
    }
}
