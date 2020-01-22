using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SaaS.Api.Mc.Http
{
    internal class McHttpClient : HttpClient
    {
        internal readonly static Uri _path = new Uri(ConfigurationManager.AppSettings["mc:path"]);
        internal readonly static string _token = ConfigurationManager.AppSettings["mc:token"];

        private readonly HttpRequestMessage _request;

        internal McHttpClient(string token = null)
        {
            BaseAddress = _path;

            if (string.IsNullOrEmpty(token))
                token = _token;

            DefaultRequestHeaders.Add("Authorization", string.Format("Basic {0}", token));
        }

        internal McHttpClient(HttpRequestMessage request, string apiKey = null) :
            this(apiKey)
        {
            _request = request;

            foreach (var header in _request.Headers.Where(header => !"Authorization".Equals(header.Key, StringComparison.InvariantCultureIgnoreCase)))
                DefaultRequestHeaders.Add(header.Key, header.Value);

            DefaultRequestHeaders.Remove("Host");
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

            return _request.CreateResponse<dynamic>(System.Net.HttpStatusCode.BadRequest, new
            {
                error = "invalid_grant"
            });
        }
    }
}