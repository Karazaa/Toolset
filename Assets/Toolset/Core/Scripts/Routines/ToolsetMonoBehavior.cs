using UnityEngine;
using System;
using System.Collections;

namespace Toolset.Core
{
    /// <summary>
    /// A Toolset wrapper of MonoBehavior that allows for access to a RoutineRunner
    /// at the ToolsetMonoBehavior level. This is used to make the Routine API as similar
    /// as possible to Unity's Coroutine API
    /// </summary>
    public class ToolsetMonoBehavior : MonoBehaviour
    {
        private readonly RoutineRunner m_routineRunner = new RoutineRunner();

        protected virtual void Update()
        {
            m_routineRunner.Update();
        }

        protected virtual void FixedUpdate()
        {
            m_routineRunner.FixedUpdate();
        }

        protected virtual void LateUpdate()
        {
            m_routineRunner.LateUpdate();
        }

        /// <summary>
        /// Runs the given IEnumerator as if it were a Unity Coroutine, but if an exceptionHandler
        /// is passed, any exceptions thrown by any layer of child iterators will be caught
        /// and passed to the handler.
        /// </summary>
        /// <param name="routine">An IEnumerator for the RoutineRunner to run.</param>
        /// <param name="exceptionHandler">
        /// An optional exception handler which will be invoked when exceptions originating
        /// from the routine are caught by RoutineRunner
        /// </param>
        /// <returns>A RoutineHandle to track the state of the internal state of the Routine in the Routine Manager.</returns>
        public RoutineHandle StartRoutine(IEnumerator routine, Action<Exception> exceptionHandler = null)
        {
            return m_routineRunner.StartRoutine(routine, exceptionHandler);
        }

        /// <summary>
        /// Stops the routine in the RoutineRunner that is associated with the passed RoutineHandle.
        /// </summary>
        /// <param name="routineHandle">The handle object of the routine to stop.</param>
        public void StopRoutine(RoutineHandle routineHandle)
        {
            m_routineRunner.StopRoutine(routineHandle);
        }

        /// <summary>
        /// Stops all currently running routines in the RoutineRunner.
        /// </summary>
        public void StopAllRoutines()
        {
            m_routineRunner.StopAllRoutines();
        }
    }
}
