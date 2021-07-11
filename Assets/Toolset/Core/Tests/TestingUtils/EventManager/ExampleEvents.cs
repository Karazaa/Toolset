public class ExampleEvent1 : Event
{
    public int m_passedIntValue;

    public override void Fire()
    {
        EventManager.Fire(this);
    }
}

public class ExampleEvent2 : Event
{
    public int m_passedIntValue;

    public override void Fire()
    {
        EventManager.Fire(this);
    }
}