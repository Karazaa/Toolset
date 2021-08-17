using NUnit.Framework;

namespace Toolset.Core.Tests
{
    /// <summary>
    /// Class of integration tests used to validate the MonoBehaviourSingleton class.
    /// </summary>
    public class TestsMonoBehaviorSingleton
    {
        [Test]
        public void TestNotNull()
        {
            Assert.NotNull(ExampleMonoBehaviorSingleton.I);
        }

        [Test]
        public void TestRepeatedAccess()
        {
            Assert.AreEqual(0, ExampleMonoBehaviorSingleton.I.ExampleValue);

            ExampleMonoBehaviorSingleton.I.ExampleValue++;

            Assert.AreEqual(1, ExampleMonoBehaviorSingleton.I.ExampleValue);
        }
    }
}