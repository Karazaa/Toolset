using System;
using System.Collections;
using UnityEngine.Networking;
using Toolset.Core;

namespace Toolset.Networking
{
    /// <summary>
    /// Enum of Http Request Methods as defined by Mozilla here: https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods
    /// </summary>
    public enum HttpRequestMethod { Get, Head, Post, Put, Create, Delete, Connect, Options, Trace, Patch  }

    public class HttpRequestInternalOperation : IInternalRequestOperation
    {
        public struct HttpRequestParameters
        {
            public HttpRequestMethod Method { get; set; }
            public Uri Url { get; set; }
            public int TimeoutSeconds { get; set; }
            public byte[] Payload { get; set; }
        }

        private enum States { Instantiated, WaitingToSend, WaitingForResponse, Errored, Succeeded }
        private enum Events { RequestCreated, RequestSent, ErrorOccurred, ReceivedSuccessfulResponse, Reset }

        public bool IsCompletedSuccessfully { get; private set; }
        public bool ShouldRetry { get; private set; }
        public UnityWebRequest.Result Result { get; private set; }
        public DownloadHandler DownloadHandler { get; private set; }
        public object Current => throw new NotImplementedException();

        private const string c_httpVerbConnect = "CONNECT";
        private const string c_httpVerbOptions = "OPTIONS";
        private const string c_httpVerbTrace = "TRACE";
        private const string c_httpVerbPatch = "PATCH";

        private HttpRequestParameters m_requestParameters;

        private StateMachine<States, Events> m_stateMachine;
        private UnityWebRequest m_unityWebRequest;
        private IEnumerator m_webRequestRoutine;

        public HttpRequestInternalOperation(HttpRequestParameters requestParameters)
        {
            m_requestParameters = requestParameters;

            m_stateMachine = new StateMachine<States, Events>(States.Instantiated);
            m_stateMachine.OnEventGoto(States.Instantiated, Events.RequestCreated, States.WaitingToSend);
            m_stateMachine.OnEventGoto(States.WaitingToSend, Events.RequestSent, States.WaitingForResponse);
            m_stateMachine.OnEventGoto(States.WaitingForResponse, Events.ErrorOccurred, States.Errored);
            m_stateMachine.OnEventGoto(States.WaitingForResponse, Events.ReceivedSuccessfulResponse, States.Succeeded);
            m_stateMachine.OnEventGoto(States.Errored, Events.Reset, States.Instantiated);
        }

        public bool MoveNext()
        {
            if (m_stateMachine.CurrentState == States.WaitingToSend)
            {
                m_stateMachine.Fire(Events.RequestSent);
                m_webRequestRoutine = SendWebRequest();
            }

            if (m_webRequestRoutine.MoveNext())
            {
                return true;
            }
            else
            {
                Result = m_unityWebRequest.result;
                DownloadHandler = m_unityWebRequest.downloadHandler;

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

        public void Reset()
        {
            m_stateMachine.Fire(Events.Reset);

            m_webRequestRoutine = null;
            m_unityWebRequest = new UnityWebRequest(m_requestParameters.Url);
            m_unityWebRequest.method = GetMethodString(m_requestParameters.Method);
            m_unityWebRequest.timeout = m_requestParameters.TimeoutSeconds;

            if (m_requestParameters.Payload != null)
                m_unityWebRequest.uploadHandler = new UploadHandlerRaw(m_requestParameters.Payload);

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

        private IEnumerator SendWebRequest()
        {
            yield return m_unityWebRequest.SendWebRequest();
        }
    }
}
