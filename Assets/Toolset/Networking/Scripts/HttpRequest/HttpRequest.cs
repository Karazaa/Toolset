using System;
using System.Collections;

namespace Toolset.Networking
{
    /// <summary>
    /// Base clase used for sending HttpRequests. Inherits from NetworkRequest.
    /// </summary>
    public abstract class HttpRequest : NetworkRequest
    {
        /// <summary>
        /// Gets the HttpRequestMethod to use for this HttpRequest.
        /// Needs to be implemented by inheritors of HttpRequest.
        /// </summary>
        protected abstract HttpRequestMethod HttpRequestMethod { get; }

        /// <summary>
        /// Gets the Url to use for this HttpRequest.
        /// Needs to be implemented by inheritors of HttpRequest.
        /// </summary>
        protected abstract Uri Url { get; }

        private HttpRequestSettings m_httpRequestSettings;
        private readonly HttpRequestInternalOperation m_internalRequestOperation;

        public HttpRequest(HttpRequestSettings httpRequestSettings) : base(httpRequestSettings)
        {
            m_httpRequestSettings = httpRequestSettings;
            m_internalRequestOperation = new HttpRequestInternalOperation(HttpRequestMethod, Url, m_httpRequestSettings.RequestTimeoutSeconds);
        }

        protected override IInternalRequestOperation InternalSend()
        {
            m_internalRequestOperation.Reset();
            return m_internalRequestOperation;
        }

        protected override IEnumerator PromptRetryWait()
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerator HandleExceedsMaximumRetries()
        {
            throw new System.NotImplementedException();
        }

        protected override void ParseResponse(bool isCompletedSuccessfully)
        {
        }
    }
}
