using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zelis.Core.HttpServiceClient
{
    public class JsonHttpServiceClient : HttpServiceClient, IJsonHttpServiceClient
    {
        public JsonHttpServiceClient(HttpServiceClientConfiguration configuration)
            : base(configuration)
        {
        }

        public Task<HttpResponseMessage> PostRequest(Uri uri, object body, CancellationToken cancellationToken, WebHeaderCollection requestHeaders = null)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> PutRequest(Uri uri, object body, CancellationToken cancellationToken, WebHeaderCollection requestHeaders = null)
        {
            throw new NotImplementedException();
        }
    }
}