using NUnit.Framework;
using UnityEngine;
using System;

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
        Debug.Log(result.Message);
        return result;
    }
}
