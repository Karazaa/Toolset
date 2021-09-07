using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;
using Toolset.ProtocolBuffers.Tests;

namespace Toolset.Networking.Tests
{
    /// <summary>
    /// Class for testing HttpRequests.
    /// </summary>
    public class TestsHttpRequest
    {
        public const int c_timeoutMilliseconds = 30000;

        [UnityTest]
        [Timeout(c_timeoutMilliseconds)]
        public IEnumerator TestGetHttpRequest()
        {
            ExampleHttpGetRequest getRequest = new ExampleHttpGetRequest();
            yield return getRequest.Send();

            Assert.IsTrue(getRequest.IsCompletedSuccessfully);
            Assert.AreEqual("Example Get Response", System.Text.Encoding.Default.GetString(getRequest.RawBytesResponseData));
        }

        [UnityTest]
        [Timeout(c_timeoutMilliseconds)]
        public IEnumerator TestPutHttpRequest()
        {
            ExamplePersistentProto upload = ProtoTestingUtils.GenerateRandomPersistentProto();

            ExampleHttpPutRequest putRequest = new ExampleHttpPutRequest(upload);
            yield return putRequest.Send();

            Assert.IsTrue(putRequest.IsCompletedSuccessfully);
            ProtoTestingUtils.AssertGeneratedModelsAreEqual(upload, putRequest.ResponseData);
        }

        [UnityTest]
        [Timeout(c_timeoutMilliseconds)]
        public IEnumerator TestPostHttpRequest()
        {
            ExamplePersistentProto upload = ProtoTestingUtils.GenerateRandomPersistentProto();

            ExampleHttpPostRequest postRequest = new ExampleHttpPostRequest(upload);
            yield return postRequest.Send();

            Assert.IsTrue(postRequest.IsCompletedSuccessfully);
            ProtoTestingUtils.AssertGeneratedModelsAreEqual(upload, postRequest.ResponseData);
        }

        [UnityTest]
        [Timeout(c_timeoutMilliseconds)]
        public IEnumerator TestTimeoutHttpRequest()
        {
            ExampleHttpTimeoutRequest getRequest = new ExampleHttpTimeoutRequest(new HttpRequestSettings()
            {
                SilentRetryInitialWaitMilliseconds = 100
            });
            yield return getRequest.Send();

            Assert.IsFalse(getRequest.IsCompletedSuccessfully);
            Assert.AreEqual((new HttpRequestSettings()).MaximumAttemptCount, getRequest.AttemptCount);
        }
    }
}
