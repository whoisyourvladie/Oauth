using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SaaS.PdfEscape.Api.Client
{
    public partial class PdfEscapeClient: IPdfEscapeClient
    {
        private readonly Uri _uri;

        public PdfEscapeClient(string uri = "https://www.pdfescape.com")
        {
            _uri = new Uri(uri);
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();

            client.BaseAddress = _uri;
            client.DefaultRequestHeaders.Accept.Clear();            
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }

    public interface IPdfEscapeClient
    {

    }
}
