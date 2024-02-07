using PodNet.Analyzers.Equality;
using System.Collections.Immutable;

namespace PodNet.Analyzers.Tests;

[TestClass]
public class EquatableArrayTests
{
    [TestMethod]
    public void SameArrayIsEqual()
    {
        var obj1 = new object();
        var obj2 = Guid.NewGuid();
        var array = ImmutableArray.Create(obj1, obj2);
        var equatableArray1 = new EquatableArray<object>(array);
        var equatableArray2 = new EquatableArray<object>(array);
        Assert.IsFalse(equatableArray1 != equatableArray2);
        Assert.AreEqual(equatableArray1, equatableArray1);
        Assert.IsTrue(equatableArray2 == equatableArray1);
        Assert.IsTrue(equatableArray2.Equals(equatableArray2));
    }

    [TestMethod]
    public void ArrayIsEqualWhenEnumerableElementsAreEqual()
    {
        var guid = Guid.NewGuid();
        var array = new EquatableArray<Guid>([guid]);
        List<Guid> guidList = [guid];
        Assert.IsTrue(array == guidList);
    }

    [TestMethod]
    public void DifferentArraysAreEqualWhenTheyContainTheSameItemsInSequence()
    {
        var obj1 = new object();
        var obj2 = Guid.NewGuid();

        var equatableArray1 = new EquatableArray<object>([obj1]);
        var equatableArray2_1 = new EquatableArray<object>([obj1, obj2]);
        var equatableArray2_2 = new EquatableArray<object>([obj1, obj2]);
        var equatableArray3 = new EquatableArray<object>([obj2, obj1]);

        foreach (var (unequal1, unequal2) in new[]
            {
                (equatableArray1, equatableArray2_1),
                (equatableArray1, equatableArray2_2),
                (equatableArray1, equatableArray3),
                (equatableArray2_1, equatableArray3),
                (equatableArray2_2, equatableArray3)
            })
        {
            Assert.AreNotEqual(unequal1, unequal2);
            Assert.AreNotEqual(unequal2, unequal1);
        }

        Assert.AreEqual(equatableArray2_1, equatableArray2_2);
        Assert.AreEqual(equatableArray2_2, equatableArray2_1);
    }

    [TestMethod]
    public void EquatableArrayWrapsAndUnwrapsSameImmutableArray()
    {
        var array = ImmutableArray.Create(new object(), Guid.NewGuid());
        var equatableArray = new EquatableArray<object>(array);
        var array2 = equatableArray.ToImmutableArray();
        var array3 = equatableArray.Values;
        Assert.AreEqual(array, array2);
        Assert.AreEqual(array, array3);
    }
}