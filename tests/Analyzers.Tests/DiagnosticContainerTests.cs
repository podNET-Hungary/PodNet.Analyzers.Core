using Microsoft.CodeAnalysis;

namespace PodNet.Analyzers.Tests;

[TestClass]
public class DiagnosticContainerTests
{
    [TestMethod]
    public void DiagnosticContainerCanBeAddedTo()
    {
        var container = new DiagnosticContainer();
        Assert.AreEqual(container.ToEquatableArray(), []);
        var diagnostic = Diagnostic.Create(new DiagnosticDescriptor("Fake", "Fake", "Fake", "Fake", DiagnosticSeverity.Warning, true), null);
        container.Add(diagnostic);
        var results = container.ToEquatableArray();
        Assert.AreEqual(results.Single(), diagnostic);
    }
}
