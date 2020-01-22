using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SaaS.Api.Client
{
    public partial class SaaSApiOauthService : ISaaSApiOauthService
    {
        private readonly Uri _uri;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public SaaSApiOauthService(string uri = "https://oauth.sodapdf.com", string clientId = "web", string clientSecret = "")
        {
            _uri = new Uri(uri);
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();

            client.BaseAddress = _uri;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public string AbsolutePath
        {
            get
            {
                return _uri.AbsoluteUri;
            }
        }
    }

    public interface ISaaSApiOauthService
    {
        string AbsolutePath { get; }

        Task<SignInModel> SignInAsync(string userName, string password);
        Task<SignInModel> SignInAsync(string token);

        Task<RefreshTokenModel> RefreshTokenAsync(string token);
    }
}
