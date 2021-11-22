using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;
using Toolset.Global.Utils;

namespace Toolset.Networking.Tests
{
    /// <summary>
    /// Class for running Integration Tests for the the NetworkRequest class.
    /// </summary>
    public class TestsNetworkRequest
    {
        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestSilentRetry()
        {
            ExampleBaseRequest baseRequest = new ExampleBaseRequest(new NetworkRequestSettings()
            { 
                MaximumAttemptCount = 7,
                SilentRetryInitialWaitMilliseconds = 250
            });

            bool didCallbackInvoke = false;
            yield return baseRequest.Send((networkRequest) => 
            {
                Assert.AreEqual(baseRequest.IsCompletedSuccessfully, networkRequest.IsCompletedSuccessfully);
                Assert.AreEqual(baseRequest.AttemptCount, networkRequest.AttemptCount);
                didCallbackInvoke = true;
            });

            Assert.IsTrue(baseRequest.IsCompletedSuccessfully);
            Assert.IsTrue(didCallbackInvoke);
            Assert.AreEqual(baseRequest.GetAttemptsForSuccessCount(), baseRequest.AttemptCount);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestExceedMaximumRetries()
        {
            int attemptCount = 3;
            ExampleBaseRequest baseRequest = new ExampleBaseRequest(new NetworkRequestSettings() 
            { 
                MaximumAttemptCount = attemptCount, 
            });

            yield return baseRequest.Send();

            Assert.IsFalse(baseRequest.IsCompletedSuccessfully);
            Assert.AreEqual(attemptCount, baseRequest.AttemptCount);
            Assert.IsTrue(baseRequest.HandledExceededMaximumRetries);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestNoRetry()
        {
            ExampleBaseRequest baseRequest = new ExampleBaseRequest(new NetworkRequestSettings()
            {
                RetryPolicy = RequestRetryPolicy.None
            });

            yield return baseRequest.Send();

            Assert.IsFalse(baseRequest.IsCompletedSuccessfully);
            Assert.AreEqual(1, baseRequest.AttemptCount);
            Assert.IsFalse(baseRequest.HandledExceededMaximumRetries);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestPromptRetry()
        {
            ExampleBaseRequest baseRequest = new ExampleBaseRequest(new NetworkRequestSettings()
            {
                RetryPolicy = RequestRetryPolicy.Prompt,
                MaximumAttemptCount = 7
            });

            yield return baseRequest.Send();

            Assert.IsTrue(baseRequest.IsCompletedSuccessfully);
            Assert.AreEqual(baseRequest.GetAttemptsForSuccessCount(), baseRequest.AttemptCount);
            Assert.AreEqual(baseRequest.GetAttemptsForSuccessCount() - 1, baseRequest.RetryPromptCounts);
        }

        [TearDown]
        public void TearDown()
        {
            ExampleInternalRequestOperation.ExampleAttemptCounter = 0;
        }
    }
}
