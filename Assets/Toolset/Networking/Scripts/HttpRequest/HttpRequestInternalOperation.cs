using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using Toolset.Core;
using UnityEngine;

namespace Toolset.Networking
{
    /// <summary>
    /// Enum of Http Request Methods as defined by Mozilla here: https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods
    /// </summary>
    public enum HttpRequestMethod { Delete, Get, Head, Options, Patch, Post, Put }

    /// <summary>
    /// Implementation of IInternalRequestOperation for HttpRequests.
    /// </summary>
    public class HttpRequestInternalOperation : IInternalRequestOperation
    {
        /// <summary>
        /// Struct of parameters to be passed into HttpRequestInternalOperation that
        /// will define how the request is carried out. Unlike settings objects,
        /// all of these values need to be defined before initiating the request.
        /// </summary>
        public struct HttpRequestParameters
        {
            /// <summary>
            /// The HttpRequest method to use for this specific request.
            /// </summary>
            public HttpRequestMethod Method { get; set; }

            /// <summary>
            /// The Url endpoint to send the request to.
            /// Even though the type is Uri, the Url should be
            /// formatted like a Url.
            /// </summary>
            public Uri Url { get; set; }

            /// <summary>
            /// The amount of time the request can be outstanding for before it
            /// is considered timed out.
            /// </summary>
            public int TimeoutSeconds { get; set; }

            /// <summary>
            /// The data to send to the server in the body of the request.
            /// </summary>
            public byte[] Data { get; set; }
        }

        private enum States { Instantiated, WaitingToSend, WaitingForResponse, Errored, Succeeded }
        private enum Events { RequestCreated, RequestSent, ErrorOccurred, ReceivedSuccessfulResponse, Reset }

        public bool IsCompletedSuccessfully { get; private set; }
        public bool ShouldRetry { get; private set; }
        public byte[] ResponseData { get; private set; }

        /// <summary>
        /// The result that signifies whether or not the request succeeded or had any errors.
        /// </summary>
        public UnityWebRequest.Result Result { get; private set; }

        /// <summary>
        /// The server name returned in the header of the response.
        /// </summary>
        public string ResponseServerName { get; private set; }

        /// <summary>
        /// The date initiated returned in the header of the response.
        /// </summary>
        public DateTime ResponseInitiatedDate { get; private set; }

        /// <summary>
        /// The date content length returned in the header of the response.
        /// </summary>
        public long ResponseContentLength { get; private set; }

        public object Current { get; }

        private const string c_headerServerNameKey = "Server";
        private const string c_headerDateKey = "Date";
        private const string c_headerContentLengthKey = "Content-Length";

        private HttpRequestParameters m_requestParameters;
        private StateMachine<States, Events> m_stateMachine;
        private UnityWebRequest m_unityWebRequest;
        private AsyncOperation m_webRequestRoutine;

        public HttpRequestInternalOperation(HttpRequestParameters requestParameters)
        {
            m_requestParameters = requestParameters;

            m_stateMachine = new StateMachine<States, Events>(States.Instantiated);
            m_stateMachine.OnEventGoto(States.Instantiated, Events.RequestCreated, States.WaitingToSend);
            m_stateMachine.OnEventGoto(States.WaitingToSend, Events.RequestSent, States.WaitingForResponse);
            m_stateMachine.OnEventGoto(States.WaitingForResponse, Events.ErrorOccurred, States.Errored);
            m_stateMachine.ExecuteOnEnter(States.Errored, OnEnterErroredState);
            m_stateMachine.OnEventGoto(States.WaitingForResponse, Events.ReceivedSuccessfulResponse, States.Succeeded);
            m_stateMachine.ExecuteOnEnter(States.Succeeded, OnEnterSuccededState);
            m_stateMachine.OnEventGoto(States.Errored, Events.Reset, States.Instantiated);
        }

        /// <summary>
        /// Iterates the HttpRequestInternalOperation
        /// </summary>
        /// <returns>True if the IEnumerator should continue iterating, false if not.</returns>
        public bool MoveNext()
        {
            if (m_stateMachine.CurrentState == States.WaitingToSend)
            {
                m_stateMachine.Fire(Events.RequestSent);
                m_webRequestRoutine = m_unityWebRequest.SendWebRequest();
                return true;
            }

            if (!m_webRequestRoutine.isDone)
            {
                return true;
            }
            else
            {
                Result = m_unityWebRequest.result;
                ResponseData = m_unityWebRequest.downloadHandler.data;
                ParseResponseHeaders(m_unityWebRequest.GetResponseHeaders());

                if (Result == UnityWebRequest.Result.Success)
                {
                    m_stateMachine.Fire(Events.ReceivedSuccessfulResponse);
                    return false;
                }
                else
                {
                    ShouldRetry = Result == UnityWebRequest.Result.ConnectionError;
                    m_stateMachine.Fire(Events.ErrorOccurred);
                    return false;
                }
            }
        }

        /// <summary>
        /// Resets the state of the HttpRequestInternalOperation so that it can be used to send
        /// data to the server again.
        /// </summary>
        public void Reset()
        {
            m_stateMachine.Fire(Events.Reset);

            m_unityWebRequest = new UnityWebRequest(m_requestParameters.Url);
            m_unityWebRequest.method = m_requestParameters.Method.GetMethodAsString();
            m_unityWebRequest.timeout = m_requestParameters.TimeoutSeconds;

            if (m_requestParameters.Data != null)
                m_unityWebRequest.uploadHandler = new UploadHandlerRaw(m_requestParameters.Data);
            m_unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

            m_stateMachine.Fire(Events.RequestCreated);
        }

        private void ParseResponseHeaders(Dictionary<string, string> headerDict)
        {
            if (headerDict == null)
                return;

            if (headerDict.TryGetValue(c_headerServerNameKey, out string serverName))
                ResponseServerName = serverName;

            if (headerDict.TryGetValue(c_headerDateKey, out string dateString))
                if (DateTime.TryParse(dateString, out DateTime parsedDate))
                    ResponseInitiatedDate = parsedDate;

            if (headerDict.TryGetValue(c_headerContentLengthKey, out string contentLengthString))
                if (long.TryParse(contentLengthString, out long contentLength))
                    ResponseContentLength = contentLength;
        }

        private void OnEnterSuccededState(States previousState, Events triggeredEvent, States currentState)
        {
            IsCompletedSuccessfully = true;
            DisposeWebRequest();
        }

        private void OnEnterErroredState(States previousState, Events triggeredEvent, States currentState)
        {
            IsCompletedSuccessfully = false;
            DisposeWebRequest();
        }

        private void DisposeWebRequest()
        {
            m_unityWebRequest.Dispose();
            m_unityWebRequest = null;
            m_webRequestRoutine = null;
        }
    }
}