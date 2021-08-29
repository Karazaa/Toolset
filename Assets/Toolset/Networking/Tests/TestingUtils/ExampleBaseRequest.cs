using System;
using System.Collections;

namespace Toolset.Networking.Tests
{
    public class ExampleBaseRequest : NetworkRequest
    {
        public int RetryPromptCounts { get; private set; }
        public bool HandledExceededMaximumRetries { get; private set; }
        private readonly IInternalRequestOperation m_exampleInternalRequestOperation = new ExampleInternalRequestOperation();

        public ExampleBaseRequest(NetworkRequestSettings settings = null) : base(settings)
        {
        }

        protected override IEnumerator HandleExceedsMaximumRetries()
        {
            HandledExceededMaximumRetries = true;
            yield break;
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

        public int GetAttemptsForSuccessCount()
        {
            return ExampleInternalRequestOperation.c_numberOfAttemptsForSuccess;
        }
    }
}
