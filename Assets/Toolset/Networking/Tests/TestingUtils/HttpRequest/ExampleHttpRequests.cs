using System;

namespace Toolset.Networking.Tests
{
    public class ExampleNoResponseHttpGetRequest : HttpRequest<NoResponseData>
    {
        protected override HttpRequestMethod HttpRequestMethod => HttpRequestMethod.Get;

        protected override Uri Url => new Uri("https://localhost:44345/ExampleGet");
    }

    public class ExampleNoResponseHttpPutRequest : HttpRequest<NoResponseData>
    {
        protected override HttpRequestMethod HttpRequestMethod => HttpRequestMethod.Put;

        protected override Uri Url => new Uri("http://localhost/");
    }

    public class ExampleNoResponseHttpPostRequest : HttpRequest<NoResponseData>
    {
        protected override HttpRequestMethod HttpRequestMethod => HttpRequestMethod.Post;

        protected override Uri Url => new Uri("http://localhost/");
    }

}
