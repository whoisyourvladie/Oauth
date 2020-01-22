using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace eSign20.Api.Client
{
    public class eSign20Client : IDisposable
    {
        private eSign20HttpClient _eSign20HttpClient;

        public eSign20Client()
        {
            _eSign20HttpClient = new eSign20HttpClient();
        }

        public eSign20Client(HttpRequestMessage request, string apiKey)
        {
            _eSign20HttpClient = new eSign20HttpClient(request, apiKey);
        }

        public async Task<HttpResponseMessage> SendAsync(string requestUri, CancellationToken cancellationToken)
        {
            return await _eSign20HttpClient.SendAsync(requestUri, cancellationToken);
        }

        internal static async Task<HttpResponseMessage> AccountSendersAsync(string email, CancellationToken cancellationToken)
        {
            using (var client = new eSign20HttpClient())
                return await client.PostAsJsonAsync<dynamic>("api/v1/account/senders/", new { email = email }, cancellationToken);
        }
        public static async Task<HttpResponseMessage> AccountSendersChangeEmailAsync(IEnumerable<string> emails)
        {
            using (var client = new eSign20HttpClient())
                return await client.PostAsJsonAsync<dynamic>("api/v1/account/senders/group/", new { emails = emails });
        }

        public static async Task<HttpResponseMessage> SenderApiKeyAsync(string email, CancellationToken cancellationToken)
        {
            var response = await AccountSendersAsync(email, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return response;

            var sender = await response.Content.ReadAsAsync<JObject>();

            using (var client = new eSign20HttpClient())
                return await client.GetAsync(string.Format("api/v1/account/senders/{0}/apiKey/", sender.Value<string>("id")));
        }

        public void Dispose()
        {
            if (!object.Equals(_eSign20HttpClient, null))
                _eSign20HttpClient.Dispose();
        }
    }
}
