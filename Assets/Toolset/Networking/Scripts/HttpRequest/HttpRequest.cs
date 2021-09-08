using System;
using System.Collections;

namespace Toolset.Networking
{
    /// <summary>
    /// Base class for sending a request over a network via Http.
    /// </summary>
    /// <typeparam name="TRequestDataModel">The model for the data to upload in the server request.</typeparam>
    /// <typeparam name="TResponseDataModel">The model for the data packaged in the server response.</typeparam>
    public abstract class HttpRequest<TRequestDataModel, TResponseDataModel> : NetworkRequest<TRequestDataModel, TResponseDataModel> where TRequestDataModel : class where TResponseDataModel : class
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
            HttpRequestSettings baseRequestSettingsAsHttpSettings = NetworkRequestSettings as HttpRequestSettings;
            HttpRequestInternalOperation.HttpRequestParameters requestParameters = new HttpRequestInternalOperation.HttpRequestParameters()
            {
                Method = HttpRequestMethod,
                Url = this.Url,
                TimeoutSeconds = baseRequestSettingsAsHttpSettings.TimeoutSeconds,
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
            yield break;
        }

        protected override IEnumerator HandleExceedsMaximumRetries()
        {
            yield break;
        }
    }
}