using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections;
using System.Threading.Tasks;
using Toolset.Core;

namespace Toolset.Global.Utils
{
    /// <summary>
    /// Static class used for adding wrapping methods around NUnit's Assert class.
    /// </summary>
    public static class ToolsetAssert
    {
        /// <summary>
        /// Adds a Unity Debug.Log of the exception message when asserting that a test delegate throws an exception. 
        /// </summary>
        /// <typeparam name="TException">The type of exception that is being Asserted.</typeparam>
        /// <param name="code">A delegate to the code that is being checked.</param>
        /// <returns>The exception that was thrown.</returns>
        public static TException Throws<TException>(TestDelegate code) where TException : Exception
        {
            TException result = Assert.Throws<TException>(code);
            LogException(result);
            return result;
        }

        /// <summary>
        /// Asserts that the given exception occurred in the passed IEnumerator.
        /// </summary>
        /// <typeparam name="TException">The type of exception that is being Asserted.</typeparam>
        /// <param name="enumerator">The IEnumerator to validate exceptions for.</param>
        /// <returns>The exception that was thrown.</returns>
        public static TException ThrowsIEnumerator<TException>(IEnumerator enumerator) where TException : Exception
        {
            bool running = true;
            TException resultingException = null;

            while (running)
            {
                try
                {
                    running = enumerator.MoveNext();
                }
                catch (TException exception)
                {
                    resultingException = exception;
                    running = false;
                    LogException(exception);
                }
            }

            Assert.IsNotNull(resultingException);
            return resultingException;
        }

        /// <summary>
        /// Asserts that the given exception occurred in the passed async Task.
        /// </summary>
        /// <typeparam name="TException">The type of exception that is being Asserted.</typeparam>
        /// <param name="task">The async task to validate exceptions for.</param>
        /// <returns>A task containing the exception that was thrown as a result.</returns>
        public static async Task<TException> ThrowsAsync<TException>(Task task) where TException : Exception
        {
            TException returnedException = null;
            try
            {
                await task;
            }
            catch (Exception exception)
            {
                LogException(exception);
                if (exception is TException tException)
                {
                    returnedException = tException;
                }
                else
                {
                    Assert.Fail("[Toolset.ToolsetAssert] Exception was not of type ".StringBuilderAppend(typeof(TException).Name));
                }
            }

            if (returnedException == null)
                Assert.Fail("[Toolset.ToolsetAssert] No exception occurred.");

            return returnedException;
        }

        /// <summary>
        /// Uses a try-catch-finally to Assert that any exception has occurred as opposed to specific exceptions. Logs
        /// the exception message.
        /// </summary>
        /// <param name="code">A delegate to the code that is being checked.</param>
        public static void ThrowsAny(TestDelegate code)
        {
            bool exceptionOccurred = false;
            try
            {
                code.DynamicInvoke();
            }
            catch (Exception exception)
            {
                exceptionOccurred = true;
                LogException(exception);
            }
            finally
            {
                Assert.IsTrue(exceptionOccurred);
            }
        }

        /// <summary>
        /// Uses a try-catch-finally to assert that any exception has occurred within an Async task.
        /// Logs the exception message.
        /// </summary>
        /// <param name="task">The async operation to validate exceptions for.</param>
        /// <returns>A task that can be yielded on.</returns>
        public static async Task ThrowsAnyAsync(Task task)
        {
            bool exceptionOccurred = false;
            try
            {
                await task;
            }
            catch (Exception exception)
            {
                exceptionOccurred = true;
                LogException(exception);
            }
            finally
            {
                Assert.IsTrue(exceptionOccurred);
            }
        }

        private static void LogException(Exception exception)
        {
            Debug.Log("[Toolset.ToolsetAssert] " + (exception.InnerException ?? exception));
        }
    }
}
