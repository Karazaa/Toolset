using NUnit.Framework;
using System;
using Toolset.Global.Utils;

namespace Toolset.Core.Tests
{
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
            string expectedString = "a b c";

            ToolsetAssert.Throws<ArgumentNullException>(() =>
            {
                exampleString.StringBuilderFormat(null);
            });

            ToolsetAssert.Throws<FormatException>(() =>
            {
                exampleString.StringBuilderFormat("a", " b ");
            });

            Assert.AreEqual(expectedString, exampleString.StringBuilderFormat("a", " b ", "c"));
            Assert.AreEqual(expectedString, exampleString.StringBuilderFormat("a", " b ", "c", "d", "e", "f"));
        }

        [Test]
        public void TestStringBuilderAppend()
        {
            string exampleString = null;

            Assert.AreEqual(exampleString, exampleString.StringBuilderAppend(1, 2, 3, 4));

            exampleString = "hello";
            string expectedString = "helloabcdef";

            ToolsetAssert.Throws<ArgumentNullException>(() =>
            {
                exampleString.StringBuilderFormat(null);
            });

            Assert.AreEqual(expectedString, exampleString.StringBuilderAppend("a", "b", "c", "d", "e", "f"));
        }
    }
}