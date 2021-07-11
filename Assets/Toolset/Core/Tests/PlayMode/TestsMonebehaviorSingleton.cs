using NUnit.Framework;

/// <summary>
/// Class of integration tests used to validate the MonobehaviorSingleton class.
/// </summary>
public class TestsMonebehaviorSingleton
{
    [Test]
    public void TestNotNull()
    {
        Assert.NotNull(ExampleMonobehaviorSingleton.I);
    }

    [Test]
    public void TestRepeatedAccess()
    {
        Assert.AreEqual(0, ExampleMonobehaviorSingleton.I.ExampleValue);

        ExampleMonobehaviorSingleton.I.ExampleValue++;

        Assert.AreEqual(1, ExampleMonobehaviorSingleton.I.ExampleValue);
    }
}
