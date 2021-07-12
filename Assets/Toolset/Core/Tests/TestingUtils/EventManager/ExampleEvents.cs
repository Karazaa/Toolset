/// <summary>
/// Example event used to validate EventManager class through unit tests.
/// </summary>
public class ExampleEvent1 : Event
{
    public int m_passedIntValue;

    public override void Fire()
    {
        EventManager.Fire(this);
    }
}

/// <summary>
/// Example event used to validate EventManager class through unit tests.
/// </summary>
public class ExampleEvent2 : Event
{
    public int m_passedIntValue;

    public override void Fire()
    {
        EventManager.Fire(this);
    }
}