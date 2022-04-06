using System;
using Toolset.Core;
using Toolset.ProtocolBuffers.Tests;
using Toolset.Global.Utils;

namespace Toolset.Networking.Tests
{
    /// <summary>
    /// Example class for testing DELETE http requests.
    /// </summary>
    public class ExampleHttpDeleteRequest : HttpRequest<NoData, NoData>
    {
        protected override HttpRequestMethod HttpRequestMethod => HttpRequestMethod.Delete;
        
        protected override Uri Url => new Uri(ToolsetTestingConstants.c_remoteTestingUrl.StringBuilderAppend("ExampleDelete"));
    }

    /// <summary>
    /// Example class for testing GET http requests.
    /// </summary>
    public class ExampleHttpGetRequest : HttpRequest<NoData, NoData>
    {
        protected override HttpRequestMethod HttpRequestMethod => HttpRequestMethod.Get;

        protected override Uri Url => new Uri(ToolsetTestingConstants.c_remoteTestingUrl.StringBuilderAppend("ExampleGet"));
    }

    /// <summary>
    /// Example class for testing HEAD http requests.
    /// </summary>
    public class ExampleHttpHeadRequest : HttpRequest<NoData, NoData>
    {
        protected override HttpRequestMethod HttpRequestMethod => HttpRequestMethod.Head;

        protected override Uri Url => new Uri(ToolsetTestingConstants.c_remoteTestingUrl.StringBuilderAppend("ExampleHead"));
    }

    /// <summary>
    /// Example class for testing OPTIONS http requests.
    /// </summary>
    public class ExampleHttpOptionsRequest : HttpRequest<NoData, NoData>
    {
        protected override HttpRequestMethod HttpRequestMethod => HttpRequestMethod.Options;

        protected override Uri Url => new Uri(ToolsetTestingConstants.c_remoteTestingUrl.StringBuilderAppend("ExampleOptions"));
    }

    /// <summary>
    /// Example class for testing PATCH http requests.
    /// </summary>
    public class ExampleHttpPatchRequest : HttpRequest<ExamplePersistentProto, ExamplePersistentProto>
    {
        public ExampleHttpPatchRequest(ExamplePersistentProto uploadData) : base(uploadData)
        {
        }

        protected override HttpRequestMethod HttpRequestMethod => HttpRequestMethod.Patch;

        protected override Uri Url => new Uri(ToolsetTestingConstants.c_remoteTestingUrl.StringBuilderAppend("ExamplePatch"));
    }

    /// <summary>
    /// Example class for testing POST http requests.
    /// </summary>
    public class ExampleHttpPostRequest : HttpRequest<ExamplePersistentProto, ExamplePersistentProto>
    {
        public ExampleHttpPostRequest(ExamplePersistentProto uploadData) : base(uploadData)
        {
        }

        protected override HttpRequestMethod HttpRequestMethod => HttpRequestMethod.Post;

        protected override Uri Url => new Uri(ToolsetTestingConstants.c_remoteTestingUrl.StringBuilderAppend("ExamplePost"));
    }

    /// <summary>
    /// Example class for testing PUT http requests.
    /// </summary>
    public class ExampleHttpPutRequest : HttpRequest<ExamplePersistentProto, ExamplePersistentProto>
    {
        public ExampleHttpPutRequest(ExamplePersistentProto uploadData) : base(uploadData)
        {
        }

        protected override HttpRequestMethod HttpRequestMethod => HttpRequestMethod.Put;

        protected override Uri Url => new Uri(ToolsetTestingConstants.c_remoteTestingUrl.StringBuilderAppend("ExamplePut"));
    }

    public class ExampleHttpTimeoutRequest : HttpRequest<NoData, NoData>
    {
        public ExampleHttpTimeoutRequest(HttpRequestSettings settings) : base(httpRequestSettings: settings)
        {
        }

        protected override HttpRequestMethod HttpRequestMethod => HttpRequestMethod.Get;

        protected override Uri Url => new Uri("https://localhost:8000/SomeRandomNonsenseToForceATimeout");
    }
}