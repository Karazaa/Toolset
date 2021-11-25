namespace Toolset.Core
{
    /// <summary>
    /// Class used for tracking the state of running routines in the RoutineManager and requesting stoppages. 
    /// </summary>
    public class RoutineHandle
    {
        /// <summary>
        /// Whether or not the tracked Routine in the RoutineManager is finished iterating.
        /// </summary>
        public bool IsDone { get; internal set; }

        internal RoutineHandle() {}
    }
}

