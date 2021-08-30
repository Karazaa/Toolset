using System.Collections;

namespace Toolset.Networking.Tests
{
    /// <summary>
    /// Empty response model used for network request tests.
    /// </summary>
    public class ExampleResponseModel
    { 
    }

    /// <summary>
    /// Example class used for testing NetworkRequests.
    /// </summary>
    public class ExampleBaseRequest : NetworkRequest<ExampleResponseModel>
    {
        public int RetryPromptCounts { get; private set; }
        public bool HandledExceededMaximumRetries { get; private set; }
        private readonly IInternalRequestOperation m_exampleInternalRequestOperation = new ExampleInternalRequestOperation();

        public ExampleBaseRequest(NetworkRequestSettings settings = null) : base(null, settings)
        {
        }

        protected override IInternalRequestOperation InternalSend()
        {
            m_exampleInternalRequestOperation.Reset();
            return m_exampleInternalRequestOperation;
        }

        protected override IEnumerator PromptRetryWait()
        {
            RetryPromptCounts++;
            yield break;
        }

        protected override IEnumerator HandleExceedsMaximumRetries()
        {
            HandledExceededMaximumRetries = true;
            yield break;
        }

        public int GetAttemptsForSuccessCount()
        {
            return ExampleInternalRequestOperation.c_numberOfAttemptsForSuccess;
        }
    }
}
