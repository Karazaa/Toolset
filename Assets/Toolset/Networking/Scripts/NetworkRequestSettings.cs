namespace Toolset.Networking
{
    public class NetworkRequestSettings
    {
        public RequestRetryPolicy RetryPolicy { get; set; } = RequestRetryPolicy.Silent;
        public int MaximumAttemptCount { get; set; } = 5;
        public int SilentRetryInitialWaitMilliseconds { get; set; } = 1000;
    }
}
