using System.Collections;

namespace Toolset.Networking.Tests
{
    /// <summary>
    /// Example class used for testing NetworkRequests.
    /// </summary>
    public class ExampleBaseRequest : NetworkRequest<NoData, NoData>
    {
        public int RetryPromptCounts { get; private set; }
        public bool HandledExceededMaximumRetries { get; private set; }
        private readonly IInternalRequestOperation m_exampleInternalRequestOperation = new ExampleInternalRequestOperation();

        public ExampleBaseRequest(NetworkRequestSettings settings = null) : base(settings: settings)
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
