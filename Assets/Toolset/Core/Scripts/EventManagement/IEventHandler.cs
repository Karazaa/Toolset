namespace Toolset.Core
{
    /// <summary>
    /// An interface that all handlers of events from the EventManager must implement. Contains
    /// a single method for handling the event.
    /// </summary>
    /// <typeparam name="T">The type of event this handler is responsible for handling.</typeparam>
    public interface IEventHandler<T> : IBaseEventHandler where T : Event
    {
        /// <summary>
        /// Invoked by the EventManager when a corresponding event is fired. That event object is then
        /// passed to this method.
        /// </summary>
        /// <param name="firedEvent">The event that has been fired.</param>
        void HandleEvent(T firedEvent);
    }

    /// <summary>
    /// This is an empty interface created solely to facilitate generic interface polymorphism found in
    /// EventManager. This should not ever be directly implemented.
    /// </summary>
    public interface IBaseEventHandler
    {
    }
}