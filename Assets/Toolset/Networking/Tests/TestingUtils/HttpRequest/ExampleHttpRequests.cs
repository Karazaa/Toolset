using System;
using Toolset.ProtocolBuffers.Tests;

namespace Toolset.Networking.Tests
{
    public class ExampleHttpGetRequest : HttpRequest<NoData, NoData>
    {
        protected override HttpRequestMethod HttpRequestMethod => HttpRequestMethod.Get;

        protected override Uri Url => new Uri("https://localhost:44345/ExampleGet");
    }

    public class ExampleHttpPutRequest : HttpRequest<ExamplePersistentProto, ExamplePersistentProto>
    {
        public ExampleHttpPutRequest(ExamplePersistentProto uploadData) : base(uploadData)
        {
        }

        protected override HttpRequestMethod HttpRequestMethod => HttpRequestMethod.Put;

        protected override Uri Url => new Uri("https://localhost:44345/ExamplePut");
    }

    public class ExampleHttpPostRequest : HttpRequest<ExamplePersistentProto, ExamplePersistentProto>
    {
        public ExampleHttpPostRequest(ExamplePersistentProto uploadData) : base(uploadData)
        {
        }

        protected override HttpRequestMethod HttpRequestMethod => HttpRequestMethod.Post;

        protected override Uri Url => new Uri("https://localhost:44345/ExamplePost");
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
