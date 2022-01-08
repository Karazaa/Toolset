namespace Toolset.Core
{
    /// <summary>
    /// Class used for tracking the state of running routines in the RoutineRunner and requesting stoppages. 
    /// </summary>
    public class RoutineHandle
    {
        /// <summary>
        /// Whether or not the tracked Routine in the RoutineRunner is finished iterating.
        /// </summary>
        public bool IsDone { get; internal set; }

        internal RoutineHandle() {}
    }
}

