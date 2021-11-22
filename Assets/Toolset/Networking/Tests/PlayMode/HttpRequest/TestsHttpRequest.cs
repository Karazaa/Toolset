using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine.TestTools;
using Toolset.ProtocolBuffers.Tests;
using Toolset.Global.Utils;

namespace Toolset.Networking.Tests
{
    /// <summary>
    /// Class for testing HttpRequests.
    /// </summary>
    public class TestsHttpRequest
    {
        private const long c_expectedHeadResponse = 2048L;
        private const string c_expectedGetResponse = "Example Get Response";
        private const string c_expectedOptionsResponse = "Some options.";

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_longTimeoutMilliseconds)]
        public IEnumerator TestDeleteHttpRequest()
        {
            ExampleHttpDeleteRequest deleteRequest = new ExampleHttpDeleteRequest();
            yield return deleteRequest.Send();

            AssertRequestSucceededWithProperHeaders(deleteRequest);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_longTimeoutMilliseconds)]
        public IEnumerator TestGetHttpRequest()
        {
            ExampleHttpGetRequest getRequest = new ExampleHttpGetRequest();
            yield return getRequest.Send();

            AssertRequestSucceededWithProperHeaders(getRequest);
            Assert.AreEqual(c_expectedGetResponse, System.Text.Encoding.Default.GetString(getRequest.RawBytesResponseData));
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_longTimeoutMilliseconds)]
        public IEnumerator TestHeadHttpRequest()
        {
            ExampleHttpHeadRequest headRequest = new ExampleHttpHeadRequest();
            yield return headRequest.Send();

            AssertRequestSucceededWithProperHeaders(headRequest);
            Assert.AreEqual(c_expectedHeadResponse, headRequest.ResponseContentLength);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_longTimeoutMilliseconds)]
        public IEnumerator TestOptionsHttpRequest()
        {
            ExampleHttpOptionsRequest optionsRequest = new ExampleHttpOptionsRequest();
            yield return optionsRequest.Send();

            AssertRequestSucceededWithProperHeaders(optionsRequest);
            Assert.AreEqual(c_expectedOptionsResponse, System.Text.Encoding.Default.GetString(optionsRequest.RawBytesResponseData));
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_longTimeoutMilliseconds)]
        public IEnumerator TestPatchHttpRequest()
        {
            ExamplePersistentProto upload = ProtoTestingUtils.GenerateRandomPersistentProto();

            ExampleHttpPatchRequest patchRequest = new ExampleHttpPatchRequest(upload);
            yield return patchRequest.Send();

            AssertRequestSucceededWithProperHeaders(patchRequest);
            ProtoTestingUtils.AssertGeneratedModelsAreEqual(upload, patchRequest.ResponseData);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_longTimeoutMilliseconds)]
        public IEnumerator TestPostHttpRequest()
        {
            ExamplePersistentProto upload = ProtoTestingUtils.GenerateRandomPersistentProto();

            ExampleHttpPostRequest postRequest = new ExampleHttpPostRequest(upload);
            yield return postRequest.Send();

            AssertRequestSucceededWithProperHeaders(postRequest);
            ProtoTestingUtils.AssertGeneratedModelsAreEqual(upload, postRequest.ResponseData);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_longTimeoutMilliseconds)]
        public IEnumerator TestPutHttpRequest()
        {
            ExamplePersistentProto upload = ProtoTestingUtils.GenerateRandomPersistentProto();

            ExampleHttpPutRequest putRequest = new ExampleHttpPutRequest(upload);
            yield return putRequest.Send();

            AssertRequestSucceededWithProperHeaders(putRequest);
            ProtoTestingUtils.AssertGeneratedModelsAreEqual(upload, putRequest.ResponseData);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_longTimeoutMilliseconds)]
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

        private void AssertRequestSucceededWithProperHeaders<TRequest, TResponse>(HttpRequest<TRequest, TResponse> httpRequest) where TRequest : class where TResponse : class
        {
            Assert.IsTrue(httpRequest.IsCompletedSuccessfully);
            Assert.AreNotEqual(default(string), httpRequest.ResponseServerName);
            Assert.AreNotEqual(default(DateTime), httpRequest.ResponseInitiatedDate);
        }
    }
}