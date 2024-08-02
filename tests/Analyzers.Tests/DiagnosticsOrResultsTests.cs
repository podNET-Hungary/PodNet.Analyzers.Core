using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using D = PodNet.Analyzers.CodeAnalysis.DiagnosticsOrResults<string?>;
using E = PodNet.Analyzers.Equality.EquatableArray<Microsoft.CodeAnalysis.Diagnostic>;

namespace PodNet.Analyzers.Tests;

[TestClass]
public class DiagnosticsOrResultsTests
{
    [TestMethod]
    public void DiagnosticsOrResultsTests_CanBeCreatedAndObserved()
    {
        var dummyDiagnostic = Diagnostic.Create(new DiagnosticDescriptor("D01", "", "", "", DiagnosticSeverity.Hidden, false), null);
        Assert.IsTrue(new D(new E([dummyDiagnostic])) is { Diagnostics: [var diag], Results: null } && diag == dummyDiagnostic);
        Assert.IsTrue(new D([]).Diagnostics is []);
        Assert.IsTrue(new D(null).Diagnostics is null);
        Assert.IsTrue(new D(ImmutableArray.Create(dummyDiagnostic)).Diagnostics is [var d] && d == dummyDiagnostic);
        Assert.IsTrue(new D(ImmutableArray.Create<Diagnostic>()).Diagnostics is []);
        Assert.IsTrue(new D(null, (string?)null) is { Diagnostics: null, Results: null });
        Assert.IsTrue(new D(null, ImmutableArray.Create((string?)null)) is { Diagnostics: null, Results: [null] });
        Assert.IsTrue(new D(null, "") is { Diagnostics: null, Results: [""] });
        Assert.IsTrue(new D(null, ImmutableArray.Create("1", "2", null)) is { Diagnostics: null, Results: ["1", "2", null] });
    }
}
