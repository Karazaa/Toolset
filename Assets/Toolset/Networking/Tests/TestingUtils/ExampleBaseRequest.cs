using System;
using System.Collections;

namespace Toolset.Networking.Tests
{
    public class ExampleBaseRequest : NetworkRequest
    {
        protected override RetryPolicy RequestRetryPolicy { get => RetryPolicy.Silent; }
        private readonly ExampleInternalRequestOperation m_exampleInternalRequestOperation = new ExampleInternalRequestOperation();

        protected override IEnumerator HandleExceedsMaximumRetries()
        {
            throw new NotImplementedException();
        }

        protected override IInternalRequestOperation InternalSend()
        {
            m_exampleInternalRequestOperation.Reset();
            return m_exampleInternalRequestOperation;
        }

        protected override IEnumerator PromptRetryWait()
        {
            throw new NotImplementedException();
        }
    }
}
