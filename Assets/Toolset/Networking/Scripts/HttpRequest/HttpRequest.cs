using System;
using System.Collections;
using Toolset.ProtocolBuffers;

namespace Toolset.Networking
{
    /// <summary>
    /// Base clase used for sending HttpRequests. Inherits from NetworkRequest.
    /// </summary>
    public abstract class HttpRequest<TResponsePayload> : NetworkRequest where TResponsePayload : class
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

        /// <summary>
        /// The Deserialized response data object.
        /// </summary>
        protected TResponsePayload ResponsePayload { get; private set; }

        private readonly HttpRequestInternalOperation m_internalRequestOperation;

        public HttpRequest(HttpRequestSettings httpRequestSettings = null) : base(httpRequestSettings ?? new HttpRequestSettings())
        {
            HttpRequestInternalOperation.HttpRequestParameters requestParameters = new HttpRequestInternalOperation.HttpRequestParameters()
            {
                Method = HttpRequestMethod,
                Url = this.Url,
                TimeoutSeconds = (NetworkRequestSettings as HttpRequestSettings).RequestTimeoutSeconds,
                Payload = GetPayloadData()
            };

            m_internalRequestOperation = new HttpRequestInternalOperation(requestParameters);
        }

        protected abstract object GetPayloadObject();

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
            ResponsePayload = ProtoBufUtils.Deserialize<TResponsePayload>(m_internalRequestOperation.DownloadHandler.data);
        }

        private byte[] GetPayloadData()
        {
            object payloadObject = GetPayloadObject();

            if (payloadObject == null || !ProtoBufUtils.IsSerializableProtobuf(payloadObject.GetType()))
                return null;

            return ProtoBufUtils.Serialize(payloadObject);
        }
    }
}
