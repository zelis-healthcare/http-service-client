using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Zelis.Core.HttpServiceClient
{
    public interface IJsonHttpServiceClient : IHttpServiceClient
    {
        Task<HttpResponseMessage> PostRequest(Uri uri, object body, CancellationToken cancellationToken, WebHeaderCollection requestHeaders = null);
        Task<HttpResponseMessage> PutRequest(Uri uri, object body, CancellationToken cancellationToken, WebHeaderCollection requestHeaders = null);
    }
}