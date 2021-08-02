using NUnit.Framework;

/// <summary>
/// Class of unit tests used to validate the Singleton class.
/// </summary>
public class TestsSingleton
{
    [SetUp]
    public void SetUp()
    {
        ExampleSingleton.I.ExampleValue = 0;
    }

    [Test]
    public void TestNotNull()
    {
        Assert.NotNull(ExampleSingleton.I);
    }

    [Test]
    public void TestRepeatedAccess()
    {
        Assert.AreEqual(0, ExampleSingleton.I.ExampleValue);

        ExampleSingleton.I.ExampleValue++;

        Assert.AreEqual(1, ExampleSingleton.I.ExampleValue);
    }
}

