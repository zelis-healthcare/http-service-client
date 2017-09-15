using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Zelis.Core.HttpServiceClient
{
    public class AzureActiveDirectoryAuthenticatingHttpServiceClient : HttpServiceClient
    {
        #region Internal Classes

        private class AzureActiveDirectoryAuthenticatingHttpClient : HttpClient
        {
            public delegate Task<string> GetAzureActiveDirectoryBearerToken(CancellationToken cancellationToken);

            private readonly GetAzureActiveDirectoryBearerToken _getAzureActiveDirectoryBearerToken;

            public AzureActiveDirectoryAuthenticatingHttpClient(GetAzureActiveDirectoryBearerToken getAzureActiveDirectoryBearerToken)
            {
                _getAzureActiveDirectoryBearerToken = getAzureActiveDirectoryBearerToken ?? throw new ArgumentNullException(nameof(getAzureActiveDirectoryBearerToken));
            }

            public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request == null)
                    throw new ArgumentNullException(nameof(request));
                if (cancellationToken == null)
                    throw new ArgumentNullException(nameof(cancellationToken));

                var headers = new WebHeaderCollection();
                headers["Authorization"] = $"Bearer {await _getAzureActiveDirectoryBearerToken(cancellationToken)}";

                return await base.SendAsync(request, cancellationToken);
            }
        }

        #endregion

        private readonly AzureActiveDirectoryAuthenticatingHttpServiceClientConfiguration _azureActiveDirectoryAuthenticatingHttpServiceClientConfiguration;

        private AuthenticationContext _authenticationContext;
        private readonly ClientCredential _clientCredential;

        private string _azureActiveDirectoryBearerToken;

        public AzureActiveDirectoryAuthenticatingHttpServiceClient(
            AzureActiveDirectoryAuthenticatingHttpServiceClientConfiguration azureActiveDirectoryAuthenticatingHttpServiceClientConfiguration
        )
            : base(azureActiveDirectoryAuthenticatingHttpServiceClientConfiguration)
        {
            _azureActiveDirectoryAuthenticatingHttpServiceClientConfiguration = azureActiveDirectoryAuthenticatingHttpServiceClientConfiguration
                ?? throw new ArgumentNullException(nameof(azureActiveDirectoryAuthenticatingHttpServiceClientConfiguration));

            _authenticationContext = new AuthenticationContext(_azureActiveDirectoryAuthenticatingHttpServiceClientConfiguration.Authority, false);
            _clientCredential = new ClientCredential(
                _azureActiveDirectoryAuthenticatingHttpServiceClientConfiguration.ClientId,
                _azureActiveDirectoryAuthenticatingHttpServiceClientConfiguration.ClientKey
            );
        }

        protected override HttpClient InitializeHttpClient(HttpServiceClientConfiguration configuration)
        {
            var httpClient = new AzureActiveDirectoryAuthenticatingHttpClient((ct) =>
            {
                Task.Run(async() => await RefreshBearerToken(ct));
                return Task.FromResult(_azureActiveDirectoryBearerToken);
            });

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.Timeout = new TimeSpan(0, 0, 0, configuration.Timeout);

            return httpClient;
        }

        private async Task RefreshBearerToken(CancellationToken cancellationToken)
        {
            if (cancellationToken == null)
                throw new ArgumentNullException(nameof(cancellationToken));

            AuthenticationResult result = await _authenticationContext.AcquireTokenAsync(
                _azureActiveDirectoryAuthenticatingHttpServiceClientConfiguration.ResourceId,
                _clientCredential
            );
            _azureActiveDirectoryBearerToken = result.AccessToken;
        }
    }
}