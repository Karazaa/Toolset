using NUnit.Framework;

namespace Toolset.Core.Tests
{
    /// <summary>
    /// Class of integration tests used to validate the ToolsetMonoBehaviorSingleton class.
    /// </summary>
    public class TestsToolsetMonoBehaviorSingleton
    {
        [Test]
        public void TestNotNull()
        {
            Assert.NotNull(ExampleToolsetMonoBehaviorSingleton.I);
        }

        [Test]
        public void TestRepeatedAccess()
        {
            Assert.AreEqual(0, ExampleToolsetMonoBehaviorSingleton.I.ExampleValue);

            ExampleToolsetMonoBehaviorSingleton.I.ExampleValue++;

            Assert.AreEqual(1, ExampleToolsetMonoBehaviorSingleton.I.ExampleValue);
        }
    }
}
