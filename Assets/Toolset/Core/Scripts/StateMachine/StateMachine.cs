using System;
using System.Collections.Generic;

namespace Toolset.Core
{
    /// <summary>
    /// A finite state machine that is generic and operates
    /// over a series of states defined by an enum. Transitions
    /// between states are defined by an enum of events.
    /// </summary>
    /// <typeparam name="TStates">The enum of states represented by this state machine.</typeparam>
    /// <typeparam name="TEvents">The enum of events that can cause transitions between states when fired.</typeparam>
    public class StateMachine<TStates, TEvents> where TStates : Enum
                                                where TEvents : Enum
    {
        private readonly Dictionary<TStates, Dictionary<TEvents, TStates>> m_eventTransitionDict = new Dictionary<TStates, Dictionary<TEvents, TStates>>();
        private readonly Dictionary<TStates, Action<TStates, TEvents, TStates>> m_onStateEnteredCallbacksDict = new Dictionary<TStates, Action<TStates, TEvents, TStates>>();
        private readonly Dictionary<TStates, Action<TStates, TEvents, TStates>> m_onStateExitedCallbacksDict = new Dictionary<TStates, Action<TStates, TEvents, TStates>>();

        /// <summary>
        /// The current state of the StateMachine.
        /// </summary>
        public TStates CurrentState { get; private set; }

        /// <summary>
        /// Create a state machine with an initial state. No OnEnter callback will
        /// be executed when entering this initial state.
        /// </summary>
        /// <param name="initialState"></param>
        public StateMachine(TStates initialState)
        {
            CurrentState = initialState;
        }

        /// <summary>
        /// Register a transition between states. When
        /// the passed event is fired, the machine will transition
        /// between the initial state and final state invoking any assigned
        /// callbacks along the way.
        /// </summary>
        /// <param name="initialState">The state that is being transitioned from.</param>
        /// <param name="transitionEvent">The event that causes the transition.</param>
        /// <param name="finalState">The state that results from the transition.</param>
        public void OnEventGoto(TStates initialState, TEvents transitionEvent, TStates finalState)
        {
            if (!m_eventTransitionDict.ContainsKey(initialState))
            {
                Dictionary<TEvents, TStates> internalDict = new Dictionary<TEvents, TStates>();
                internalDict.Add(transitionEvent, finalState);
                m_eventTransitionDict.Add(initialState, internalDict);
                return;
            }

            if (m_eventTransitionDict[initialState].ContainsKey(transitionEvent))
                throw new InvalidOperationException("[Toolset.StateMachine] State {0} already has a tranistion registered for Event {1}!"
                                                        .StringBuilderFormat(initialState, transitionEvent));

            m_eventTransitionDict[initialState].Add(transitionEvent, finalState);
        }

        /// <summary>
        /// Registers a callback to be invoked when entering the specified state.
        /// </summary>
        /// <param name="stateContext">The state the passed callback is associated with.</param>
        /// <param name="methodToExecute">A callback that has the previous state, causing event, and new state passed as parameters</param>
        public void ExecuteOnEnter(TStates stateContext, Action<TStates, TEvents, TStates> methodToExecute)
        {
            if (m_onStateEnteredCallbacksDict.ContainsKey(stateContext))
                throw new InvalidOperationException("[Toolset.StateMachine] State {0} already has an OnEnter callback assigned!"
                                                        .StringBuilderFormat(stateContext));

            m_onStateEnteredCallbacksDict.Add(stateContext, methodToExecute);
        }

        /// <summary>
        /// Registers a callback to be invoked when exiting the specified state.
        /// </summary>
        /// <param name="stateContext">The state the passed callback is associated with.</param>
        /// <param name="methodToExecute">A callback that has the previous state, causing event, and new state passed as parameters</param>
        public void ExecuteOnExit(TStates stateContext, Action<TStates, TEvents, TStates> methodToExecute)
        {
            if (m_onStateExitedCallbacksDict.ContainsKey(stateContext))
                throw new InvalidOperationException("[Toolset.StateMachine] State {0} already has an OnExit callback assigned!"
                                                        .StringBuilderFormat(stateContext));

            m_onStateExitedCallbacksDict.Add(stateContext, methodToExecute);
        }

        /// <summary>
        /// Fires an event, potentially causing a state transition and callback invokation.
        /// </summary>
        /// <param name="eventToFire">The event to fire.</param>
        public void Fire(TEvents eventToFire)
        {
            if (!m_eventTransitionDict.ContainsKey(CurrentState))
                return;

            if (!m_eventTransitionDict[CurrentState].ContainsKey(eventToFire))
                return;

            TStates previousState = CurrentState;
            TStates nextState = m_eventTransitionDict[CurrentState][eventToFire];
            CurrentState = nextState;

            if (m_onStateExitedCallbacksDict.ContainsKey(previousState))
                m_onStateExitedCallbacksDict[previousState].Invoke(previousState, eventToFire, nextState);

            if (m_onStateEnteredCallbacksDict.ContainsKey(nextState))
                m_onStateEnteredCallbacksDict[nextState].Invoke(previousState, eventToFire, nextState);
        }

        /// <summary>
        /// Forces the state machine to the passed state. This does not
        /// invoke any state entry/exit related callbacks when called.
        /// </summary>
        /// <param name="nextState">The state the machine is being forced to.</param>
        public void ForceState(TStates nextState)
        {
            CurrentState = nextState;
        }
    }
}