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
            string output = string.Empty;
            switch (method)
            {
                case HttpRequestMethod.Delete:
                    output = UnityWebRequest.kHttpVerbDELETE;
                    break;
                case HttpRequestMethod.Get:
                    output = UnityWebRequest.kHttpVerbGET;
                    break;
                case HttpRequestMethod.Head:
                    output = UnityWebRequest.kHttpVerbHEAD;
                    break;
                case HttpRequestMethod.Options:
                    output = c_httpVerbOptions;
                    break;
                case HttpRequestMethod.Patch:
                    output = c_httpVerbPatch;
                    break;
                case HttpRequestMethod.Post:
                    output = UnityWebRequest.kHttpVerbPOST;
                    break;
                case HttpRequestMethod.Put:
                    output = UnityWebRequest.kHttpVerbPUT;
                    break;
            }
            return output;
        }
    }
}