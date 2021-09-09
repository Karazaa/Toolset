using System.Collections;
using System.Threading.Tasks;

namespace Toolset.Core
{
    /// <summary>
    /// A static class of extension methods for Task.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Returns the given task as an IEnumerator that can be yielded on in Unity Coroutines.
        /// </summary>
        /// <param name="taskToConvert">The task to convert to an IEnumerator.</param>
        /// <returns>The task in IEnumerator form.</returns>
        public static IEnumerator GetAsIEnumerator(this Task taskToConvert)
        {
            return new TaskEnumerator(taskToConvert);
        }
    }

    /// <summary>
    /// Class that implements IEnumerator that tasks get converted into when
    /// GetAsIEnumerator is invoked.
    /// </summary>
    public class TaskEnumerator : IEnumerator
    {
        private Task m_task;

        public TaskEnumerator(Task taskToConvert)
        {
            m_task = taskToConvert;
        }

        public bool MoveNext()
        {
            if (!m_task.IsCompleted)
                return true;

            return false;
        }

        public object Current { get; }

        public void Reset()
        {
        }
    }
}

