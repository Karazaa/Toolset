using System.Collections;
using UnityEngine;

namespace Toolset.Core
{
    /// <summary>
    /// A static class of extension methods for AsyncOperation.
    /// </summary>
    public static class AsyncOperationExtensions
    {
        /// <summary>
        /// Returns the given AsyncOperation as an IEnumerator that can be yielded on in Unity Coroutines or Toolset Routines.
        /// </summary>
        /// <param name="operationToConvert">The operation to convert to an IEnumerator.</param>
        /// <returns>The operation in IEnumerator form.</returns>
        public static IEnumerator GetAsIEnumerator(this AsyncOperation operationToConvert)
        {
            return new AsyncOperationEnumerator(operationToConvert);
        }
    }

    /// <summary>
    /// Class that implements IEnumerator that AsyncOperations get converted into when
    /// GetAsIEnumerator is invoked.
    /// </summary>
    public class AsyncOperationEnumerator : IEnumerator
    {
        private AsyncOperation m_operation;

        public AsyncOperationEnumerator(AsyncOperation operationToConvert)
        {
            m_operation = operationToConvert;
        }

        public bool MoveNext()
        {
            if (!m_operation.isDone)
                return true;

            return false;
        }

        public object Current { get; }

        public void Reset()
        {
            Debug.LogWarning("[Toolset.AsyncOperationEnumerator] Calling reset on an AsyncOperationEnumerator does nothing since AsyncOperations can not automatically be reset without getting recreated.");
        }
    }
}
