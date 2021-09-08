using NUnit.Framework;
using UnityEngine.Networking;

namespace Toolset.Networking.Tests
{
    /// <summary>
    /// Class of tests to validate HttpRequestMethodExtensions.
    /// </summary>
    public class TestsHttpRequestMethodExtensions
    {
        [TestCase(HttpRequestMethod.Get, UnityWebRequest.kHttpVerbGET)]
        [TestCase(HttpRequestMethod.Head, UnityWebRequest.kHttpVerbHEAD)]
        [TestCase(HttpRequestMethod.Post, UnityWebRequest.kHttpVerbPOST)]
        [TestCase(HttpRequestMethod.Put, UnityWebRequest.kHttpVerbPUT)]
        [TestCase(HttpRequestMethod.Create, UnityWebRequest.kHttpVerbCREATE)]
        [TestCase(HttpRequestMethod.Delete, UnityWebRequest.kHttpVerbDELETE)]
        [TestCase(HttpRequestMethod.Connect, HttpRequestMethodExtensions.c_httpVerbConnect)]
        [TestCase(HttpRequestMethod.Options, HttpRequestMethodExtensions.c_httpVerbOptions)]
        [TestCase(HttpRequestMethod.Trace, HttpRequestMethodExtensions.c_httpVerbTrace)]
        [TestCase(HttpRequestMethod.Patch, HttpRequestMethodExtensions.c_httpVerbPatch)]
        public void TestGetMethodAsString(HttpRequestMethod method, string expectedString)
        {
            Assert.AreEqual(expectedString, method.GetMethodAsString());
        }
    }
}