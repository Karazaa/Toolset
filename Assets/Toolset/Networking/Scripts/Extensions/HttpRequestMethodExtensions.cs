using UnityEngine.Networking;
using System;

namespace Toolset.Networking
{
    /// <summary>
    /// Static class of extension methods for the HttpRequestMethod enum
    /// </summary>
    public static class HttpRequestMethodExtensions
    {
        public const string c_httpVerbOptions = "OPTIONS";
        public const string c_httpVerbPatch = "PATCH";

        /// <summary>
        /// Gets the associated Http method name as a string.
        /// </summary>
        /// <param name="method">The method to get a string name for.</param>
        /// <returns>The string method name formatted in the manner UnityWebRequest is expecting.</returns>
        public static string GetMethodAsString(this HttpRequestMethod method)
        {
            switch (method)
            {
                case HttpRequestMethod.Delete:
                    return UnityWebRequest.kHttpVerbDELETE;
                case HttpRequestMethod.Get:
                    return UnityWebRequest.kHttpVerbGET;
                case HttpRequestMethod.Head:
                    return UnityWebRequest.kHttpVerbHEAD;
                case HttpRequestMethod.Options:
                    return c_httpVerbOptions;
                case HttpRequestMethod.Patch:
                    return c_httpVerbPatch;
                case HttpRequestMethod.Post:
                    return UnityWebRequest.kHttpVerbPOST;
                case HttpRequestMethod.Put:
                    return UnityWebRequest.kHttpVerbPUT;
                default:
                    throw new InvalidOperationException("[Toolset.HttpRequestMethod] Could not parse Http method!");
            }
        }
    }
}