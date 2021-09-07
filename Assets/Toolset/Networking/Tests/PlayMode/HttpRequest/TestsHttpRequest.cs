using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;

namespace Toolset.Networking.Tests
{
    /// <summary>
    /// Class for testing HttpRequests.
    /// </summary>
    public class TestsHttpRequest
    {
        public const int c_timeoutMilliseconds = 100000;

        [UnityTest]
        [Timeout(c_timeoutMilliseconds)]
        public IEnumerator TestGetHttpRequest()
        {
            ExampleNoResponseHttpGetRequest getRequest = new ExampleNoResponseHttpGetRequest();
            yield return getRequest.Send();

            //var str = System.Text.Encoding.Default.GetString(getRequest.RawBytesResponseData);

            Assert.IsTrue(getRequest.IsCompletedSuccessfully);
        }
    }
}
