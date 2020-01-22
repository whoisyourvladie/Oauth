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
    public class GoogleService : OauthService
    {
        public GoogleService(ClientProvider client) : base(client) { }

        public override string GetAuthenticationUrl(string lang)
        {
            var query = QueryStringBuilder.BuildCompex(new[] { "scope" },
                "scope", _clientProvider.Scope,
                "redirect_uri", _clientProvider.CallBackUrl,
                "client_id", _clientProvider.ClientId,
                "response_type", "code",
                "approval_prompt", "force", //auto
                                            //"prompt", "select_account", //https://stackoverflow.com/questions/14384354/force-google-account-chooser
                "access_type", "online",
                "hl", lang
            );

            return "https://accounts.google.com/o/oauth2/auth?" + query;
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
                new KeyValuePair<string, string>("redirect_uri", _clientProvider.CallBackUrl),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            });

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://www.googleapis.com");

                var response = await client.PostAsync("oauth2/v4/token", content, cancellationToken);

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

            query.Add("access_token", token.AccessToken);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://www.googleapis.com");

                var response = await client.GetAsync("oauth2/v1/userinfo/?" + query.ToString(), cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var profile = JsonConvert.DeserializeObject<GoogleProfileResult>(json);

                    return new ProfileResult(profile);
                }
            }

            return null;
        }
        //public override async Task<bool> RevokeAsync(TokenResult token, CancellationToken cancellationToken)
        //{
        //    var query = HttpUtility.ParseQueryString(string.Empty);

        //    query.Add("token", token.AccessToken);

        //    var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
        //    {

        //    });

        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri("https://accounts.google.com");

        //        var response = await client.PostAsync("o/oauth2/revoke/?" + query.ToString(), content, cancellationToken);

        //        return response.IsSuccessStatusCode;
        //    }
        //}
    }
}