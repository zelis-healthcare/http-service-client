using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Zelis.Core.HttpServiceClient
{
    public class BasicAuthenticatingHttpServiceClient : HttpServiceClient
    {
        public BasicAuthenticatingHttpServiceClient(
            AuthenticatingHttpServiceClientConfiguration configuration
        )
            : base(configuration)
        {
        }

        protected override HttpClient InitializeHttpClient(HttpServiceClientConfiguration configuration)
        {
            if (!(configuration is AuthenticatingHttpServiceClientConfiguration))
                throw new ArgumentException("Expected a AuthenticatingHttpServiceClientConfiguration");
            var authenticatingConfiguration = (configuration as AuthenticatingHttpServiceClientConfiguration);

            var networkCredential = new NetworkCredential(authenticatingConfiguration.Username, authenticatingConfiguration.Password);
            var credentialCache = new CredentialCache();
            credentialCache.Add(authenticatingConfiguration.BaseAddress, "NTLM", networkCredential);

            HttpClientHandler handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                Credentials = credentialCache,
                UseDefaultCredentials = true,
            };
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = authenticatingConfiguration.BaseAddress
            };
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string credentials = Convert.ToBase64String(
                System.Text.Encoding.ASCII.GetBytes(
                    string.Format("{0}:{1}", authenticatingConfiguration.Username, authenticatingConfiguration.Password)
                )
            );
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            httpClient.Timeout = new TimeSpan(0, 0, 0, authenticatingConfiguration.Timeout);

            return httpClient;
        }
    }
}