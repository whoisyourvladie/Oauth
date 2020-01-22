using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SaaS.Api.Client
{
    public partial class SaaSApiOauthService
    {
        public async Task<RefreshTokenModel> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return new RefreshTokenModel(RefreshTokenStatus.InvalidGrant);

            var builder = new StringBuilder("grant_type=refresh_token");
            builder.AppendFormat("&refresh_token={0}", refreshToken);
            builder.AppendFormat("&client_id={0}", _clientId);
            builder.AppendFormat("&client_secret={0}", _clientSecret);

            using (var client = CreateHttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();

                var request = new HttpRequestMessage(HttpMethod.Post, "api/token");
                request.Content = new StringContent(builder.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    SaaSApiToken token = await response.Content.ReadAsAsync<SaaSApiToken>();

                    return new RefreshTokenModel(token);
                }

                return new RefreshTokenModel(RefreshTokenStatus.InvalidGrant);
            }
        }
    }

    //+ grant_type (string, required) - Should be 'refresh_token';
    //+ refresh_token (string, required) - refresh_token from previous SignIn;
    //+ client_id (string, required) - Should be `sodapdfdesktopapp`;
    //+ client_secret (string, required) - Should be `mfjmibwJ2ILGFPVV/S95K4hHzTD4ZBxV/bDS/TUKtw=`.

    public enum RefreshTokenStatus : byte
    {
        Ok,
        InvalidGrant
    }

    public class RefreshTokenModel
    {
        public readonly RefreshTokenStatus Status = RefreshTokenStatus.Ok;
        public readonly SaaSApiToken Token = null;

        public RefreshTokenModel(SaaSApiToken token)
        {
            Token = token;
        }

        public RefreshTokenModel(RefreshTokenStatus status)
        {
            Status = status;
        }

        public RefreshTokenModel(RefreshTokenStatus status, SaaSApiToken token)
        {
            Status = status;
            Token = token;
        }
    }
}
