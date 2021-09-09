using NUnit.Framework;
using UnityEngine.Networking;

namespace Toolset.Networking.Tests
{
    /// <summary>
    /// Class of tests to validate HttpRequestMethodExtensions.
    /// </summary>
    public class TestsHttpRequestMethodExtensions
    {
        [TestCase(HttpRequestMethod.Delete, UnityWebRequest.kHttpVerbDELETE)]
        [TestCase(HttpRequestMethod.Get, UnityWebRequest.kHttpVerbGET)]
        [TestCase(HttpRequestMethod.Head, UnityWebRequest.kHttpVerbHEAD)]
        [TestCase(HttpRequestMethod.Options, HttpRequestMethodExtensions.c_httpVerbOptions)]
        [TestCase(HttpRequestMethod.Patch, HttpRequestMethodExtensions.c_httpVerbPatch)]
        [TestCase(HttpRequestMethod.Post, UnityWebRequest.kHttpVerbPOST)]
        [TestCase(HttpRequestMethod.Put, UnityWebRequest.kHttpVerbPUT)]
        public void TestGetMethodAsString(HttpRequestMethod method, string expectedString)
        {
            Assert.AreEqual(expectedString, method.GetMethodAsString());
        }
    }
}