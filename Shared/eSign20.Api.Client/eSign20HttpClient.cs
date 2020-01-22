using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SaaS.IPDetect;

namespace eSign20.Api.Client
{
    internal class eSign20HttpClient : HttpClient
    {
        internal readonly static Uri _path = new Uri(ConfigurationManager.AppSettings["eSign20:path"]);
        internal readonly static string _token = ConfigurationManager.AppSettings["eSign20:token"];

        private readonly HttpRequestMessage _request;

        internal eSign20HttpClient(string token = null)
        {
            BaseAddress = _path;

            if (string.IsNullOrEmpty(token))
                token = _token;

            DefaultRequestHeaders.Add("Authorization", string.Format("Basic {0}", token));
        }

        internal eSign20HttpClient(HttpRequestMessage request, string apiKey) :
            this(apiKey)
        {
            _request = request;

            foreach (var header in _request.Headers.Where(header => !"Authorization".Equals(header.Key, StringComparison.InvariantCultureIgnoreCase)))
                DefaultRequestHeaders.Add(header.Key, header.Value);

            DefaultRequestHeaders.Remove("Host");
            DefaultRequestHeaders.Remove("Authorization");
            DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", apiKey));
            DefaultRequestHeaders.Add("X_FORWARDED_FOR", IpAddressDetector.IpAddress);
        }

        internal async Task<HttpResponseMessage> SendAsync(string requestUri, CancellationToken cancellationToken)
        {
            string query = _request.RequestUri.Query;

            if (!string.IsNullOrEmpty(query))
                requestUri += query;

            if (_request.Method == HttpMethod.Get)
                return await GetAsync(requestUri, cancellationToken);

            if (_request.Method == HttpMethod.Post)
                return await PostAsync(requestUri, _request.Content, cancellationToken);

            if (_request.Method == HttpMethod.Put)
                return await PutAsync(requestUri, _request.Content, cancellationToken);

            if (_request.Method == HttpMethod.Delete)
                return await DeleteAsync(requestUri, cancellationToken);

            return _request.CreateResponse(HttpStatusCode.BadRequest, new
            {
                error = "invalid_request"
            });
        }
    }
}
