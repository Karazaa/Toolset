public abstract class BaseExampleEvent
{
    public int ExampleValue { get; set; }
    public abstract void Subscribe();
    public abstract void Unsubscribe();
}

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