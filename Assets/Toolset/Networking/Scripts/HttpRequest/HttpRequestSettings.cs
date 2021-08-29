namespace Toolset.Networking
{
    /// <summary>
    /// A class of settings that can be passed into a HttpRequest to modify its behavior.
    /// </summary>
    public class HttpRequestSettings : NetworkRequestSettings
    {
        /// <summary>
        /// The number of seconds this request can be outstanding for before
        /// it is considered timed out.
        /// </summary>
        public int RequestTimeoutSeconds { get; set; } = 30;
    }
}
