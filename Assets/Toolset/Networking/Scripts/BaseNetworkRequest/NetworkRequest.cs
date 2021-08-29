using System;
using System.Collections;
using Toolset.Core;

namespace Toolset.Networking
{
    /// <summary>
    /// The type of retry that should occur in the event of a Network Request Timeout or retryable error.
    /// <br/><br/><b>Silent</b> will cause the request to automatically
    /// retry without any player input.
    /// <br/><br/><b>Prompt</b> will invoke an IEnumerator that should
    /// show the player some UI indicating that they need to retry the request.
    /// <br/><br/><b>None</b> will cause the request to just fail without any retry attempts if it times out.
    /// </summary>
    public enum RequestRetryPolicy { None, Silent, Prompt }

    /// <summary>
    /// Base class used for sending a network request.
    /// </summary>
    public abstract class NetworkRequest
    {
        /// <summary>
        /// How many times the Network Request has been attempted.
        /// </summary>
        public int AttemptCount { get; private set; }

        /// <summary>
        /// Whether or not the NetworkRequest successfully obtained a valid response.
        /// </summary>
        public bool IsCompletedSuccessfully { get; private set; }

        private NetworkRequestSettings m_networkRequestSettings;

        public NetworkRequest(NetworkRequestSettings settings = null)
        {
            m_networkRequestSettings = settings ?? new NetworkRequestSettings();
        }

        /// <summary>
        /// Initiates sending the NetworkRequest. Can be yielded on to wait until the request's completion.
        /// Any automatic retries due to a timeout or other retryable error will also occur within the scope
        /// of the returned IEnumerator.
        /// </summary>
        /// <param name="onCompletionCallback">An optional parameter to pass a callback invoked that will be invoked upon completion of the request.
        /// Useful for when the calling scope initiates the request but does not yield on the returned IEnumerator.
        /// </param>
        /// <returns>An IEnumerator that can be yielded on.</returns>
        public virtual IEnumerator Send(Action<NetworkRequest> onCompletionCallback = null)
        {
            // Send the initial attempt of the request.
            IInternalRequestOperation internalRequestOperation = InternalSend();
            AttemptCount++;
            yield return internalRequestOperation;

            // If we don't have a retry policy specified, break right here.
            if (m_networkRequestSettings.RetryPolicy == RequestRetryPolicy.None)
            {
                Complete(internalRequestOperation, onCompletionCallback);
                yield break;
            }

            // As long as the operation has not completed successfully and we are beneath the maximum attempt countm keep retrying.
            while (internalRequestOperation.ShouldRetry && AttemptCount < m_networkRequestSettings.MaximumAttemptCount)
            {
                yield return m_networkRequestSettings.RetryPolicy == RequestRetryPolicy.Prompt ? PromptRetryWait() : SilentRetryWait();

                internalRequestOperation = InternalSend();
                AttemptCount++;
                yield return internalRequestOperation;
            }

            // If we get here and the internal operation still hasn't completed successfully, handle exceeding above maximum retries.
            if (internalRequestOperation.ShouldRetry)
                yield return HandleExceedsMaximumRetries();

            Complete(internalRequestOperation, onCompletionCallback);
        }

        /// <summary>
        /// Initiates the internal routine that actually sends the the request over the network.
        /// Needs to be implemented by inheritors of NetworkRequest to adhere to a certain network communication protocol.
        /// </summary>
        /// <returns>An IEnumerator that will be yielded on in the Send method of NetworkRequest.</returns>
        protected abstract IInternalRequestOperation InternalSend();

        /// <summary>
        /// A routine implemented by inheritors of NetworkRequest that needs to show some UI to the player indicating that
        /// a request needs to be retried. The entire flow for prompting the player should occur within the scope of the 
        /// returned IEnumerator.
        /// </summary>
        /// <returns>An IEnumerator that will be yielded on in the Send method of if the request's retry policy is set to Prompt.</returns>
        protected abstract IEnumerator PromptRetryWait();

        protected IEnumerator SilentRetryWait()
        {
            // Calculate a number of milliseconds to wait along a fibonacci sequence 
            // based on the initial wait time and the number of attempts that have occurred.
            int iterations = 0;
            int lastWaitTimeMilliseconds = 0;
            int totalWaitTimeMilliseconds = m_networkRequestSettings.SilentRetryInitialWaitMilliseconds;

            while (iterations < AttemptCount)
            {
                iterations++;
                int cachedWaitTimeMilliseconds = totalWaitTimeMilliseconds;
                totalWaitTimeMilliseconds += lastWaitTimeMilliseconds;
                lastWaitTimeMilliseconds = cachedWaitTimeMilliseconds;
            }

            // Calculate the time in the future this routine will be done waiting, and then just
            // yield until that time has been reached.
            long finalTime = DateTime.UtcNow.MillisecondsSinceUnixEpoch() + totalWaitTimeMilliseconds;
            while (DateTime.UtcNow.MillisecondsSinceUnixEpoch() < finalTime)
            {
                yield return null;
            }
        }

        /// <summary>
        /// A routine implemented by inheritors of NetworkRequest that gets invoked
        /// when the request has exceeded the given maximum number of retries allowed.
        /// </summary>
        /// <returns>An IEnumerator that will be yielded on in the Send method of NetworkRequest if the retry policy is not
        /// None, and the request has exceeded the maximum number of retries allowed.</returns>
        protected abstract IEnumerator HandleExceedsMaximumRetries();

        /// <summary>
        /// Method that gets invoked upon successful completion of the request to parse the data
        /// from the request into a usable object. Needs to be implemented by inheritors of NetworkRequest.
        /// </summary>
        protected abstract void ParseResponse(bool isCompletedSuccessfully);

        private void Complete(IInternalRequestOperation internalRequestOperation, Action<NetworkRequest> onCompletionCallback = null)
        {
            IsCompletedSuccessfully = internalRequestOperation.IsCompletedSuccessfully;
            ParseResponse(IsCompletedSuccessfully);
            onCompletionCallback?.Invoke(this);
        }
    }
}
