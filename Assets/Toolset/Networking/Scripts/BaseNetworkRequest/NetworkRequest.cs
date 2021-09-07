using System;
using System.Collections;
using Toolset.Core;
using Toolset.ProtocolBuffers;

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
    /// Base class for sending a request over a network.
    /// </summary>
    /// <typeparam name="TResponseModel">The model of the expected data in the request's response.</typeparam>
    public abstract class NetworkRequest<TResponseModel> where TResponseModel : class
    {
        /// <summary>
        /// How many times the Network Request has been attempted.
        /// </summary>
        public int AttemptCount { get; private set; }

        /// <summary>
        /// Whether or not the NetworkRequest successfully obtained a valid response.
        /// </summary>
        public bool IsCompletedSuccessfully { get; private set; }

        /// <summary>
        /// The Deserialized response data object.
        /// </summary>
        public TResponseModel ResponseData { get; private set; }

        /// <summary>
        /// The raw bytes of the response data object.
        /// </summary>
        public byte[] RawBytesResponseData { get; private set; }

        /// <summary>
        /// The settings object for this NetworkRequest.
        /// </summary>
        protected NetworkRequestSettings NetworkRequestSettings { get; private set; }

        /// <summary>
        /// The Serialized request payload data object.
        /// </summary>
        protected byte[] PayloadData { get; private set; }

        private IInternalRequestOperation m_internalRequestOperation;

        public NetworkRequest(object payloadObject = null, NetworkRequestSettings settings = null)
        {
            NetworkRequestSettings = settings ?? new NetworkRequestSettings();
            PayloadData = ProtoBufUtils.Serialize(payloadObject);
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
        public virtual IEnumerator Send(Action<NetworkRequest<TResponseModel>> onCompletionCallback = null)
        {
            // Send the initial attempt of the request.
            m_internalRequestOperation = InternalSend();
            AttemptCount++;
            yield return m_internalRequestOperation;

            // If we don't have a retry policy specified, break right here.
            if (NetworkRequestSettings.RetryPolicy == RequestRetryPolicy.None)
            {
                Complete(onCompletionCallback);
                yield break;
            }

            // As long as the operation has not completed successfully and we are beneath the maximum attempt countm keep retrying.
            while (m_internalRequestOperation.ShouldRetry && AttemptCount < NetworkRequestSettings.MaximumAttemptCount)
            {
                yield return NetworkRequestSettings.RetryPolicy == RequestRetryPolicy.Prompt ? PromptRetryWait() : SilentRetryWait();

                m_internalRequestOperation = InternalSend();
                AttemptCount++;
                yield return m_internalRequestOperation;
            }

            // If we get here and the internal operation still hasn't completed successfully, handle exceeding above maximum retries.
            if (m_internalRequestOperation.ShouldRetry)
                yield return HandleExceedsMaximumRetries();

            Complete(onCompletionCallback);
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

        private IEnumerator SilentRetryWait()
        {
            // Calculate a number of milliseconds to wait along a fibonacci sequence 
            // based on the initial wait time and the number of attempts that have occurred.
            int iterations = 0;
            int lastWaitTimeMilliseconds = 0;
            int totalWaitTimeMilliseconds = NetworkRequestSettings.SilentRetryInitialWaitMilliseconds;

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

        private void Complete(Action<NetworkRequest<TResponseModel>> onCompletionCallback = null)
        {
            IsCompletedSuccessfully = m_internalRequestOperation.IsCompletedSuccessfully;

            RawBytesResponseData = m_internalRequestOperation.ResponseData;
            if (typeof(TResponseModel) !=  typeof(NoResponseData))
                ResponseData = ProtoBufUtils.Deserialize<TResponseModel>(m_internalRequestOperation.ResponseData);

            onCompletionCallback?.Invoke(this);
        }
    }
}
