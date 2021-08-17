namespace Toolset.Core
{
    /// <summary>
    /// A base class that all future event classes should inherit from. Provides a single utility method for quickly
    /// firing an event.
    /// </summary>
    public abstract class Event
    {
        public abstract void Fire();
    }
}

