using System;
using System.Collections;
using Toolset.Core;

namespace Toolset.Networking
{
    /// <summary>
    /// Base class used for sending and receiving a network request.
    /// </summary>
    public abstract class NetworkRequest
    {
        protected enum RetryPolicy { None, Silent, Prompt }
        public int AttemptCount { get; private set; }
        protected abstract RetryPolicy RequestRetryPolicy { get; set; }
        protected virtual int MaximumAttemptCount => 5;
        protected virtual int SilentRetryInitialWaitMilliseconds => 1000;

        public virtual IEnumerator Send()
        {
            // Send the initial attempt of the request.
            IInternalRequestOperation internalRequestOperation = InternalSend();
            AttemptCount++;
            yield return internalRequestOperation;

            // If we don't have a retry policy specified, break right here.
            if (RequestRetryPolicy == RetryPolicy.None)
                yield break;

            // Otherwise prepare the retry wait routine for the given policy.
            IEnumerator retryWait = RequestRetryPolicy == RetryPolicy.Prompt ? PromptRetryWait() : SilentRetryWait();

            // As long as the operation has not completed successfully and we are beneath the maximum attempt countm keep retrying.
            while (!internalRequestOperation.IsCompletedSuccessfully && AttemptCount < MaximumAttemptCount)
            {
                yield return retryWait;

                internalRequestOperation = InternalSend();
                AttemptCount++;
                yield return internalRequestOperation;
            }

            // If we get here and the internal operation still hasn't completed successfully, handle exceeding above maximum retries.
            if (!internalRequestOperation.IsCompletedSuccessfully)
                yield return HandleExceedsMaximumRetries();
        }

        protected abstract IInternalRequestOperation InternalSend();

        protected abstract IEnumerator PromptRetryWait();

        protected IEnumerator SilentRetryWait()
        {
            // Calculate a number of milliseconds to wait along a fibonacci sequence 
            // based on the initial wait time and the number of attempts that have occurred.
            int iterations = 0;
            int lastWaitTimeMilliseconds = 0;
            int totalWaitTimeMilliseconds = SilentRetryInitialWaitMilliseconds;
           
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
            while(DateTime.UtcNow.MillisecondsSinceUnixEpoch() < finalTime)
            {
                yield return null;
            }
        }

        protected abstract IEnumerator HandleExceedsMaximumRetries();
    }
}
