using Newtonsoft.Json;
using SaaS.Oauth2.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SaaS.Oauth2.Services
{
    public class FacebookService : OauthService
    {
        public FacebookService(ClientProvider client) : base(client) { }

        public override string GetAuthenticationUrl(string lang)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            query.Add("client_id", _clientProvider.ClientId);
            query.Add("redirect_uri", _clientProvider.CallBackUrl);
            query.Add("scope", _clientProvider.Scope);
            query.Add("auth_type", "reauthenticate");//rerequest

            query.Add("display", "popup");
            query.Add("locale", lang);

            return "https://www.facebook.com/dialog/oauth/?" + query.ToString();
        }

        public override Size GetWindowSize()
        {
            return new Size(700, 350);
        }

        public override async Task<TokenResult> TokenAsync(string code, CancellationToken cancellationToken)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _clientProvider.ClientId),
                new KeyValuePair<string, string>("client_secret", _clientProvider.ClientSecret),
                new KeyValuePair<string, string>("redirect_uri", _clientProvider.CallBackUrl),
                new KeyValuePair<string, string>("code", code)
            });

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://graph.facebook.com");

                var response = await client.PostAsync("oauth/access_token", content, cancellationToken);

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
            var query = HttpUtility.ParseQueryString(string.Empty);

            query.Add("fields", "email,first_name,last_name,verified");
            query.Add("access_token", token.AccessToken);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://graph.facebook.com");

                var response = await client.GetAsync("me/?" + query.ToString(), cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var profile = JsonConvert.DeserializeObject<FacebookProfileResult>(json);

                    return new ProfileResult(profile);
                }
            }

            return null;
        }
    }
}
