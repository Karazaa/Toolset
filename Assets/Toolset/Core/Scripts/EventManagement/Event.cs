namespace Toolset.Core
{
    /// <summary>
    /// An interface that all future event classes should implement from. Provides a single utility method for quickly
    /// firing an event.
    /// </summary>
    public interface IEvent
    {
        public void Fire();
    }
}

