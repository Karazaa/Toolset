using NUnit.Framework;
using System;

/// <summary>
/// Test class for validating string extensions.
/// </summary>
public class TestsStringExtensions
{
    [Test]
    public void TestIsNullOrWhiteSpace()
    {
        string exampleString = null;
        Assert.IsTrue(exampleString.IsNullOrWhiteSpace());

        exampleString = string.Empty;
        Assert.IsTrue(exampleString.IsNullOrWhiteSpace());

        exampleString = "       ";
        Assert.IsTrue(exampleString.IsNullOrWhiteSpace());

        exampleString = "asdfamhefhruivcgyz";
        Assert.IsFalse(exampleString.IsNullOrWhiteSpace());
    }

    [Test]
    public void TestStringBuilderFormat()
    {
        string exampleString = null;

        Assert.AreEqual(exampleString, exampleString.StringBuilderFormat(1, 2, 3, 4));

        exampleString = "{0}{1}{2}";
        string expectedString1 = "a b c";

        ToolsetAssert.Throws<ArgumentNullException>(() =>
        {
            exampleString.StringBuilderFormat(null);
        });

        ToolsetAssert.Throws<FormatException>(() =>
        {
            exampleString.StringBuilderFormat("a", " b ");
        });

        Assert.AreEqual(expectedString1, exampleString.StringBuilderFormat("a", " b ", "c"));
        Assert.AreEqual(expectedString1, exampleString.StringBuilderFormat("a", " b ", "c", "d", "e", "f"));
    }
}
