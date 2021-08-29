namespace Toolset.Networking
{
    /// <summary>
    /// A class of settings that can be passed into a NetworkRequest to modify its behavior.
    /// </summary>
    public class NetworkRequestSettings
    {
        /// <summary>
        /// An enum value representing how this request should behave in the case of a 
        /// timeout or a retryable error. 
        /// </summary>
        public RequestRetryPolicy RetryPolicy { get; set; } = RequestRetryPolicy.Silent;

        /// <summary>
        /// The maximum number of attempts allowed for this request.
        /// </summary>
        public int MaximumAttemptCount { get; set; } = 5;

        /// <summary>
        /// The initial time, in milliseconds, of the fibonacci backoff sequence
        /// for Silent request retries.
        /// </summary>
        public int SilentRetryInitialWaitMilliseconds { get; set; } = 1000;
    }
}
