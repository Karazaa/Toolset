using System;
using System.Collections;

namespace Toolset.Networking
{
    /// <summary>
    /// Base class for sending a request over a network.
    /// </summary>
    /// <typeparam name="TResponseModel">The model of the expected data in the request's response.</typeparam>
    public abstract class HttpRequest<TResponseModel> : NetworkRequest<TResponseModel> where TResponseModel : class
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

        private readonly HttpRequestInternalOperation m_internalRequestOperation;

        public HttpRequest(object payloadData = null, HttpRequestSettings httpRequestSettings = null) : base(payloadData, httpRequestSettings ?? new HttpRequestSettings())
        {
            HttpRequestInternalOperation.HttpRequestParameters requestParameters = new HttpRequestInternalOperation.HttpRequestParameters()
            {
                Method = HttpRequestMethod,
                Url = this.Url,
                TimeoutSeconds = (NetworkRequestSettings as HttpRequestSettings).RequestTimeoutSeconds,
                Data = PayloadData
            };

            m_internalRequestOperation = new HttpRequestInternalOperation(requestParameters);
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
    }
}
