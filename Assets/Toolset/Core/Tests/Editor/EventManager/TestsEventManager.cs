using NUnit.Framework;

namespace Toolset.Core.Tests
{
    /// <summary>
    /// Class of unit tests used to validate the EventManager class.
    /// </summary>
    public class TestsEventManager
    {
        private const int c_dummyValue1 = 10;
        private const int c_dummyValue2 = 20;

        [Test]
        public void TestSingleSubscribeUnsubscribe()
        {
            ExampleEventHandler1 eventHandler = new ExampleEventHandler1();
            eventHandler.Subscribe();

            (new ExampleEvent1() { m_passedIntValue = c_dummyValue1 }).Fire();

            Assert.AreEqual(c_dummyValue1, eventHandler.ExampleValue);

            eventHandler.Unsubscribe();

            (new ExampleEvent1() { m_passedIntValue = c_dummyValue2 }).Fire();

            Assert.AreNotEqual(c_dummyValue2, eventHandler.ExampleValue);
        }

        [Test]
        public void TestMultipleSubscribeUnsubscribeToSingleEvent()
        {
            ExampleEventHandler1 eventHandler1 = new ExampleEventHandler1();
            ExampleEventHandler1 eventHandler2 = new ExampleEventHandler1();
            eventHandler1.Subscribe();
            eventHandler2.Subscribe();

            (new ExampleEvent1() { m_passedIntValue = c_dummyValue1 }).Fire();

            Assert.AreEqual(c_dummyValue1, eventHandler1.ExampleValue);
            Assert.AreEqual(c_dummyValue1, eventHandler2.ExampleValue);

            eventHandler1.Unsubscribe();

            (new ExampleEvent1() { m_passedIntValue = c_dummyValue2 }).Fire();

            Assert.AreNotEqual(c_dummyValue2, eventHandler1.ExampleValue);
            Assert.AreEqual(c_dummyValue2, eventHandler2.ExampleValue);

            eventHandler2.Unsubscribe();
        }

        [Test]
        public void TestMultipleSubscribeUnsubscribeToMultipleEvents()
        {
            ExampleEventHandler1 eventHandler1 = new ExampleEventHandler1();
            ExampleEventHandler2 eventHandler2 = new ExampleEventHandler2();
            eventHandler1.Subscribe();
            eventHandler2.Subscribe();

            (new ExampleEvent1() { m_passedIntValue = c_dummyValue1 }).Fire();

            Assert.AreEqual(c_dummyValue1, eventHandler1.ExampleValue);
            Assert.AreNotEqual(c_dummyValue1, eventHandler2.ExampleValue);

            eventHandler1.Unsubscribe();

            (new ExampleEvent2() { m_passedIntValue = c_dummyValue2 }).Fire();

            Assert.AreNotEqual(c_dummyValue2, eventHandler1.ExampleValue);
            Assert.AreEqual(c_dummyValue2, eventHandler2.ExampleValue);

            eventHandler2.Unsubscribe();
        }

        [Test]
        public void TestDuplicateSubscriptionsToEvent()
        {
            ExampleEventHandler3 eventHandler = new ExampleEventHandler3();
            eventHandler.Subscribe();
            eventHandler.Subscribe();

            (new ExampleEvent1() { m_passedIntValue = c_dummyValue1 }).Fire();

            Assert.AreEqual(c_dummyValue1, eventHandler.ExampleValue);

            eventHandler.Unsubscribe();
        }

        [Test]
        public void TestFireWithoutAnySubscribers()
        {
            ExampleEventHandler1 eventHandler = new ExampleEventHandler1();

            (new ExampleEvent1() { m_passedIntValue = c_dummyValue1 }).Fire();

            Assert.IsFalse(EventManager.IsSubscribedToEvent(eventHandler));
            Assert.AreNotEqual(c_dummyValue1, eventHandler.ExampleValue);
        }

        [Test]
        public void TestUnsubscribeWithoutAnySubscribers()
        {
            ExampleEventHandler1 eventHandler = new ExampleEventHandler1();
            eventHandler.Unsubscribe();

            Assert.IsFalse(EventManager.IsSubscribedToEvent(eventHandler));
        }

        [Test]
        public void TestIsSubscribedToEvent()
        {
            ExampleEventHandler1 eventHandler = new ExampleEventHandler1();

            Assert.IsFalse(EventManager.IsSubscribedToEvent(eventHandler));

            eventHandler.Subscribe();

            Assert.IsTrue(EventManager.IsSubscribedToEvent(eventHandler));

            eventHandler.Unsubscribe();

            Assert.IsFalse(EventManager.IsSubscribedToEvent(eventHandler));
        }
    }
}