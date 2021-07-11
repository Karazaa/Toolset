using NUnit.Framework;

/// <summary>
/// Class of unit tests used to validate the EventManager class.
/// </summary>
public class TestsEventManager
{
    [Test]
    public void TestSingleSubscribeUnsubscribe()
    {
        int dummyValue1 = 10;
        int dummyValue2 = 20;

        ExampleEventHandler1 eventHandler1 = new ExampleEventHandler1();
        eventHandler1.Subscribe();

        (new ExampleEvent1() { m_passedIntValue = dummyValue1 }).Fire();

        Assert.AreEqual(dummyValue1, eventHandler1.ExampleValue);

        eventHandler1.Unsubscribe();

        (new ExampleEvent1() { m_passedIntValue = dummyValue2 }).Fire();

        Assert.AreNotEqual(dummyValue2, eventHandler1.ExampleValue);
    }

    [Test]
    public void TestMultipleSubscribeUnsubscribeToSingleEvent()
    {
        int dummyValue1 = 10;
        int dummyValue2 = 20;

        ExampleEventHandler1 eventHandler1 = new ExampleEventHandler1();
        ExampleEventHandler1 eventHandler2 = new ExampleEventHandler1();
        eventHandler1.Subscribe();
        eventHandler2.Subscribe();

        (new ExampleEvent1() { m_passedIntValue = dummyValue1 }).Fire();

        Assert.AreEqual(dummyValue1, eventHandler1.ExampleValue);
        Assert.AreEqual(dummyValue1, eventHandler2.ExampleValue);

        eventHandler1.Unsubscribe();

        (new ExampleEvent1() { m_passedIntValue = dummyValue2 }).Fire();

        Assert.AreNotEqual(dummyValue2, eventHandler1.ExampleValue);
        Assert.AreEqual(dummyValue2, eventHandler2.ExampleValue);

        eventHandler2.Unsubscribe();
    }

    [Test]
    public void TestMultipleSubscribeUnsubscribeToMultipleEvents()
    {
        int dummyValue1 = 10;
        int dummyValue2 = 20;

        ExampleEventHandler1 eventHandler1 = new ExampleEventHandler1();
        ExampleEventHandler2 eventHandler2 = new ExampleEventHandler2();
        eventHandler1.Subscribe();
        eventHandler2.Subscribe();

        (new ExampleEvent1() { m_passedIntValue = dummyValue1 }).Fire();

        Assert.AreEqual(dummyValue1, eventHandler1.ExampleValue);
        Assert.AreNotEqual(dummyValue1, eventHandler2.ExampleValue);

        eventHandler1.Unsubscribe();

        (new ExampleEvent2() { m_passedIntValue = dummyValue2 }).Fire();

        Assert.AreNotEqual(dummyValue2, eventHandler1.ExampleValue);
        Assert.AreEqual(dummyValue2, eventHandler2.ExampleValue);

        eventHandler2.Unsubscribe();
    }
}
