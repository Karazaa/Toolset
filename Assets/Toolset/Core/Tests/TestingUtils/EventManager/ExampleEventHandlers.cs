namespace Toolset.Core.Tests
{
    /// <summary>
    /// Base class that other example classes for the EventManager unit tests inherit from.
    /// </summary>
    public abstract class BaseExampleEvent
    {
        public int ExampleValue { get; set; }
        public abstract void Subscribe();
        public abstract void Unsubscribe();
    }

    /// <summary>
    /// Example class used to validate EventManager class through unit tests.
    /// </summary>
    public class ExampleEventHandler1 : BaseExampleEvent, IEventHandler<ExampleEvent1>
    {
        public override void Subscribe()
        {
            EventManager.Subscribe(this);
        }

        public override void Unsubscribe()
        {
            EventManager.Unsubscribe(this);
        }

        public void HandleEvent(ExampleEvent1 exampleEvent)
        {
            ExampleValue = exampleEvent.m_passedIntValue;
        }
    }

    /// <summary>
    /// Example class used to validate EventManager class through unit tests.
    /// </summary>
    public class ExampleEventHandler2 : BaseExampleEvent, IEventHandler<ExampleEvent2>
    {
        public override void Subscribe()
        {
            EventManager.Subscribe(this);
        }

        public override void Unsubscribe()
        {
            EventManager.Unsubscribe(this);
        }

        public void HandleEvent(ExampleEvent2 exampleEvent)
        {
            ExampleValue = exampleEvent.m_passedIntValue;
        }
    }

    /// <summary>
    /// Example class used to validate EventManager class through unit tests.
    /// </summary>
    public class ExampleEventHandler3 : BaseExampleEvent, IEventHandler<ExampleEvent1>
    {
        public override void Subscribe()
        {
            EventManager.Subscribe(this);
        }

        public override void Unsubscribe()
        {
            EventManager.Unsubscribe(this);
        }

        public void HandleEvent(ExampleEvent1 exampleEvent)
        {
            ExampleValue += exampleEvent.m_passedIntValue;
        }
    }
}