using System;
using UnityEngine.Networking;
using Toolset.Core;
using UnityEngine;
using System.Threading;

namespace Toolset.Networking
{
    /// <summary>
    /// Enum of Http Request Methods as defined by Mozilla here: https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods
    /// </summary>
    public enum HttpRequestMethod { Get, Head, Post, Put, Create, Delete, Connect, Options, Trace, Patch }

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
            /// An int that limits the rate of iteration of the HttpRequestInternalOperation's enumeration.
            /// needed to prevent potential stack overflows in the UnityWebRequest class.
            /// </summary>
            public int IterationMinDelayMilliseconds { get; set; }

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
        /// The download handler for the request which is used for retrieving data from the body of the
        /// server's response.
        /// </summary>
        public DownloadHandler DownloadHandler { get; private set; }

        /// <summary>
        /// Returns a reference to the current IEnumerator.
        /// </summary>
        public object Current => this;

        private const string c_httpVerbConnect = "CONNECT";
        private const string c_httpVerbOptions = "OPTIONS";
        private const string c_httpVerbTrace = "TRACE";
        private const string c_httpVerbPatch = "PATCH";

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
#if UNITY_EDITOR
                // Due to Unity awfulness, there is a virtual memory access issue within
                // UnityWebRequest that can cause the editor to crash during PlayMode tests.
                // Sleeping the main thread for 1 ms while waiting for m_webRequestRoutine.isDone
                // resolves the issue in editor however.
                Thread.Sleep(1);
#endif
                return true;
            }
            else
            {
                Result = m_unityWebRequest.result;
                DownloadHandler = m_unityWebRequest.downloadHandler;
                ResponseData = DownloadHandler.data;

                if (Result == UnityWebRequest.Result.Success)
                {
                    m_stateMachine.Fire(Events.ReceivedSuccessfulResponse);
                    return false;
                }
                else
                {
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

            m_webRequestRoutine = null;
            m_unityWebRequest = new UnityWebRequest(m_requestParameters.Url);
            m_unityWebRequest.method = GetMethodString(m_requestParameters.Method);
            m_unityWebRequest.timeout = m_requestParameters.TimeoutSeconds;

            if (m_requestParameters.Data != null)
                m_unityWebRequest.uploadHandler = new UploadHandlerRaw(m_requestParameters.Data);
            m_unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

            m_stateMachine.Fire(Events.RequestCreated);
        }

        private string GetMethodString(HttpRequestMethod method)
        {
            switch (method)
            {
                case HttpRequestMethod.Get:
                    return UnityWebRequest.kHttpVerbGET;
                case HttpRequestMethod.Head:
                    return UnityWebRequest.kHttpVerbHEAD;
                case HttpRequestMethod.Post:
                    return UnityWebRequest.kHttpVerbPOST;
                case HttpRequestMethod.Put:
                    return UnityWebRequest.kHttpVerbPUT;
                case HttpRequestMethod.Create:
                    return UnityWebRequest.kHttpVerbCREATE;
                case HttpRequestMethod.Delete:
                    return UnityWebRequest.kHttpVerbDELETE;
                case HttpRequestMethod.Connect:
                    return c_httpVerbConnect;
                case HttpRequestMethod.Options:
                    return c_httpVerbOptions;
                case HttpRequestMethod.Trace:
                    return c_httpVerbTrace;
                case HttpRequestMethod.Patch:
                    return c_httpVerbPatch;
                default:
                    throw new InvalidOperationException("[Toolset.HttpRequestInternalOperation] Could not parse Http method!");
            }
        }

        private void OnEnterSuccededState(States previousState, Events triggeredEvent, States currentState)
        {
            IsCompletedSuccessfully = true;
        }

        private void OnEnterErroredState(States previousState, Events triggeredEvent, States currentState)
        {
            IsCompletedSuccessfully = false;
        }
    }
}
