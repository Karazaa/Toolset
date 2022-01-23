using System;
using System.Collections;

namespace Toolset.Core
{
    /// <summary>
    /// Interface that specifies necessary API for a RoutineManager.
    /// </summary>
    public interface IRoutineManagerService : IInjectable
    {
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
        public RoutineHandle StartRoutine(IEnumerator routine, Action<Exception> exceptionHandler = null);

        /// <summary>
        /// Stops the routine in the RoutineRunner that is associated with the passed RoutineHandle.
        /// </summary>
        /// <param name="routineHandle">The handle object of the routine to stop.</param>
        public void StopRoutine(RoutineHandle routineHandle);

        /// <summary>
        /// Stops all currently running routines in the RoutineRunner.
        /// </summary>
        public void StopAllRoutines();
    }
}
