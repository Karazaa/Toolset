using System;
using System.Collections;
using Toolset.Core;

namespace Toolset.Networking
{
    /// <summary>
    /// The type of retry that should occur in the event of a Network Request Timeout or retryable error.
    /// </summary>
    public enum RequestRetryPolicy { None, Silent, Prompt }

    /// <summary>
    /// Base class used for sending and receiving a network request.
    /// </summary>
    public abstract class NetworkRequest
    {
        public int AttemptCount { get; private set; }
        public bool IsCompletedSuccessfully { get; private set; }
        private NetworkRequestSettings m_networkRequestSettings;

        public NetworkRequest(NetworkRequestSettings settings = null)
        {
            m_networkRequestSettings = settings ?? new NetworkRequestSettings();
        }

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
            while (!internalRequestOperation.IsCompletedSuccessfully && AttemptCount < m_networkRequestSettings.MaximumAttemptCount)
            {
                yield return m_networkRequestSettings.RetryPolicy == RequestRetryPolicy.Prompt ? PromptRetryWait() : SilentRetryWait();

                internalRequestOperation = InternalSend();
                AttemptCount++;
                yield return internalRequestOperation;
            }

            // If we get here and the internal operation still hasn't completed successfully, handle exceeding above maximum retries.
            if (!internalRequestOperation.IsCompletedSuccessfully)
                yield return HandleExceedsMaximumRetries();

            Complete(internalRequestOperation, onCompletionCallback);
        }

        protected abstract IInternalRequestOperation InternalSend();

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

        protected abstract IEnumerator HandleExceedsMaximumRetries();

        private void Complete(IInternalRequestOperation internalRequestOperation, Action<NetworkRequest> onCompletionCallback = null)
        {
            IsCompletedSuccessfully = internalRequestOperation.IsCompletedSuccessfully;
            onCompletionCallback?.Invoke(this);
        }
    }
}
