using System;
using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;
using Toolset.Global.Utils;

namespace Toolset.Core.Tests
{
    /// <summary>
    /// Class used for running integration tests on the TaskExtensions class.
    /// </summary>
    public class TestsTaskExtensions
    {
        private const int c_lengthDelay = 1000;
        private const int c_returnValue = 100;

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestGetAsIEnumerator()
        {
            long timeStarted = DateTimeExtensions.MillisecondsSinceUnixEpoch(DateTime.Now);

            Task taskToYieldOn = TaskToYieldOn();
            yield return taskToYieldOn.GetAsIEnumerator();

            long timeFinished = DateTimeExtensions.MillisecondsSinceUnixEpoch(DateTime.Now);

            Assert.IsTrue(taskToYieldOn.IsCompleted);
            Assert.GreaterOrEqual(timeFinished - timeStarted, c_lengthDelay);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestGetAsIEnumeratorWithReturnType()
        {
            long timeStarted = DateTimeExtensions.MillisecondsSinceUnixEpoch(DateTime.Now);

            Task<int> returnTypeTask = TaskToYieldOnWithReturnType();
            yield return returnTypeTask.GetAsIEnumerator();

            long timeFinished = DateTimeExtensions.MillisecondsSinceUnixEpoch(DateTime.Now);

            Assert.IsTrue(returnTypeTask.IsCompleted);
            Assert.GreaterOrEqual(timeFinished - timeStarted, c_lengthDelay);
            Assert.AreEqual(c_returnValue, returnTypeTask.Result);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestResetDoesNothing()
        {
            Task taskToYieldOn = TaskToYieldOn();
            TaskEnumerator taskAsEnumerator = taskToYieldOn.GetAsIEnumerator() as TaskEnumerator;

            yield return taskAsEnumerator;

            Assert.IsTrue(taskToYieldOn.IsCompleted);

            taskAsEnumerator.Reset();

            Assert.IsTrue(taskToYieldOn.IsCompleted);
        }

        private async Task TaskToYieldOn()
        {
            await Task.Delay(c_lengthDelay);
        }

        private async Task<int> TaskToYieldOnWithReturnType()
        {
            await Task.Delay(c_lengthDelay);
            return c_returnValue;
        }
    }
}
