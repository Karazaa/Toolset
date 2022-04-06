namespace Toolset.Core.Tests
{
    /// <summary>
    /// Example event used to validate EventManager class through unit tests.
    /// </summary>
    public struct ExampleEvent1 : IEvent
    {
        public int PassedIntValue { get; set; }

        public void Fire()
        {
            EventManager.Fire(this);
        }
    }

    /// <summary>
    /// Example event used to validate EventManager class through unit tests.
    /// </summary>
    public struct ExampleEvent2 : IEvent
    {
        public int PassedIntValue { get; set; }

        public void Fire()
        {
            EventManager.Fire(this);
        }
    }
}