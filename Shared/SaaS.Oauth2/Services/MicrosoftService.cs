using Newtonsoft.Json;
using SaaS.Oauth2.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SaaS.Oauth2.Services
{
    public class MicrosoftService : OauthService
    {
        public MicrosoftService(ClientProvider client) : base(client) { }

        public override string GetAuthenticationUrl(string lang)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            query.Add("client_id", _clientProvider.ClientId);
            query.Add("redirect_uri", _clientProvider.CallBackUrl);
            query.Add("scope", _clientProvider.Scope);
            query.Add("response_type", "code");
            query.Add("response_mode", "query");

            return "https://login.microsoftonline.com/common/oauth2/v2.0/authorize/?" + query.ToString();
        }

        public override Size GetWindowSize()
        {
            return new Size(800, 600);
        }

        public override async Task<TokenResult> TokenAsync(string code, CancellationToken cancellationToken)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _clientProvider.ClientId),
                new KeyValuePair<string, string>("client_secret", _clientProvider.ClientSecret),
                new KeyValuePair<string, string>("scope", _clientProvider.Scope),

                new KeyValuePair<string, string>("redirect_uri", _clientProvider.CallBackUrl),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
            });

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://login.microsoftonline.com");

                var response = await client.PostAsync("common/oauth2/v2.0/token", content, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TokenResult>(json);
                }
            }

            return null;
        }
        public override async Task<ProfileResult> ProfileAsync(TokenResult token, CancellationToken cancellationToken)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://graph.microsoft.com");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);

                var response = await client.GetAsync("v1.0/me/", cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var profile = JsonConvert.DeserializeObject<MicrosoftProfileResult>(json);

                    return new ProfileResult(profile);
                }
            }

            return null;
        }
    }
}