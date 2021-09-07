namespace Toolset.Networking
{
    /// <summary>
    /// A class of settings that can be passed into a HttpRequest to modify its behavior.
    /// </summary>
    public class HttpRequestSettings : NetworkRequestSettings
    {
        /// <summary>
        /// The amount of time the request can be outstanding for before it
        /// is considered timed out.
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// An int that limits the rate of iteration of the HttpRequestInternalOperation's enumeration.
        /// needed to prevent potential stack overflows in the UnityWebRequest class.
        /// </summary>
        public int IterationMinDelayMilliseconds { get; set; } = 200;
    }
}
