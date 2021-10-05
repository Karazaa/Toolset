using UnityEngine;

namespace Toolset.Core
{
    /// <summary>
    /// This is the RoutineManager compliant equivalent to the WaitForSeconds YieldInstruction in
    /// Unity. Unity's Time Scale is applied! The reason this class is required is because the number of seconds a WaitForSeconds waits for
    /// is not publically accessible post instantiation. 
    /// </summary>
    public class ToolsetWaitForSeconds : YieldInstruction
    {
        /// <summary>
        /// The number of seconds this yield instruction is set to wait for.
        /// </summary>
        public float Seconds { get; private set; }

        public ToolsetWaitForSeconds(float seconds)
        {
            Seconds = seconds;
        }
    }
}