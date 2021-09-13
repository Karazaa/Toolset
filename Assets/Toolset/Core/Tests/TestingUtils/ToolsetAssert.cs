using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections;
using System.Threading.Tasks;

/// <summary>
/// Static class used for adding wrapping methods around NUnit's Assert class.
/// </summary>
public static class ToolsetAssert
{
    /// <summary>
    /// Adds a Unity Debug.Log of the exception message when asserting that a test delegate throws an exception. 
    /// </summary>
    /// <typeparam name="TActual">The type of exception that is being Asserted.</typeparam>
    /// <param name="code">A delegate to the code that is being checked.</param>
    /// <returns>The exception that was thrown.</returns>
    public static TActual Throws<TActual>(TestDelegate code) where TActual : Exception
    {
        TActual result = Assert.Throws<TActual>(code);
        LogException(result);
        return result;
    }

    /// <summary>
    /// Asserts that the given exception occurred in the passed IEnumerator.
    /// </summary>
    /// <typeparam name="TActual">The type of exception that is being Asserted.</typeparam>
    /// <param name="enumerator">The IEnumerator to validate exceptions for.</param>
    public static TActual ThrowsIEnumerator<TActual>(IEnumerator enumerator) where TActual : Exception
    {
        bool running = true;
        TActual resultingException = null;

        while (running)
        {
            try
            {
                running = enumerator.MoveNext();
            }
            catch (TActual exception)
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
