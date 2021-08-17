using System;
using System.Collections.Generic;

namespace Toolset.Core
{
    /// <summary>
    /// A static class used to manage the Subscribing to, Unsubscribing from, and firing of events.
    /// </summary>
    public static class EventManager
    {
        private static readonly Dictionary<Type, HashSet<IBaseEventHandler>> s_eventDictionary = new Dictionary<Type, HashSet<IBaseEventHandler>>();

        /// <summary>
        /// Subscribes a handler to an event so that every time that event is fired, the HandleEvent method on the handler will be invoked.
        /// </summary>
        /// <typeparam name="T">The type of event that is being subscribed to.</typeparam>
        /// <param name="handler">The desired handler for the event that is being subscribed to.</param>
        public static void Subscribe<T>(IEventHandler<T> handler) where T : Event
        {
            Type eventType = typeof(T);

            if (!s_eventDictionary.ContainsKey(eventType))
            {
                s_eventDictionary.Add(eventType, new HashSet<IBaseEventHandler>() { handler });
                return;
            }

            if (s_eventDictionary[eventType] == null)
                throw new InvalidOperationException("[Toolset.EventManager] An entry exists in the event dictionary for type {0}, but the associated hash set is null.".StringBuilderFormat(eventType));

            if (s_eventDictionary[eventType].Contains(handler))
                return;

            s_eventDictionary[eventType].Add(handler);
        }

        /// <summary>
        /// Unsubscribes a handler from an event so that when the event is fired, the HandleEvent method on the handler will no longer be invoked.
        /// </summary>
        /// <typeparam name="T">The type of event that is being unsubscribed from.</typeparam>
        /// <param name="handler">The handler that is being removed as a subscriber from the event.</param>
        public static void Unsubscribe<T>(IEventHandler<T> handler) where T : Event
        {
            Type eventType = typeof(T);

            if (!ValidateEntryAndHashSetExsist(eventType))
                return;

            if (!s_eventDictionary[eventType].Contains(handler))
                return;

            s_eventDictionary[eventType].Remove(handler);

            if (s_eventDictionary[eventType].Count == 0)
                s_eventDictionary.Remove(eventType);
        }

        /// <summary>
        /// Fires an event so that all handlers of the event get their HandleEvent method invoked.
        /// </summary>
        /// <typeparam name="T">The type of event that is being fired.</typeparam>
        /// <param name="eventToFire">The specific instance of the event that will be passed to all handlers.</param>
        public static void Fire<T>(T eventToFire) where T : Event
        {
            Type eventType = typeof(T);

            if (!ValidateEntryAndHashSetExsist(eventType))
                return;

            foreach (IBaseEventHandler handler in s_eventDictionary[eventType])
            {
                (handler as IEventHandler<T>).HandleEvent(eventToFire);
            }
        }

        /// Validates that there is an entry for the event type and that there is a valid associated hash set with that event type.
        private static bool ValidateEntryAndHashSetExsist(Type eventType)
        {
            if (!s_eventDictionary.ContainsKey(eventType))
                return false;

            if (s_eventDictionary[eventType] == null)
                throw new InvalidOperationException("[Toolset.EventManager] An entry exists in the event dictionary for type {0}, but the associated hash set is null.".StringBuilderFormat(eventType));

            return true;
        }
    }
}