using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SaaS.Api.Client
{
    public partial class SaaSApiOauthService
    {
        private StringBuilder GrantTypePassword()
        {
            var builder = new StringBuilder("grant_type=password");

            builder.AppendFormat("&client_id={0}", _clientId);
            builder.AppendFormat("&client_secret={0}", _clientSecret);

            return builder;
        }

        private async Task<SignInModel> _SignInAsync(StringBuilder data, string uri)
        {
            using (var client = CreateHttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();

                var request = new HttpRequestMessage(HttpMethod.Post, uri);
                request.Content = new StringContent(data.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    SaaSApiToken token = await response.Content.ReadAsAsync<SaaSApiToken>();

                    return new SignInModel(token);
                }

                return new SignInModel(SignInStatus.InvalidGrant);
            }
        }

        public async Task<SignInModel> SignInAsync(string userName, string password)
        {
            userName = Uri.EscapeDataString(userName);
            password = Uri.EscapeDataString(password);

            var data = GrantTypePassword();
            data.AppendFormat("&username={0}", userName);
            data.AppendFormat("&password={0}", password);

            return await _SignInAsync(data, "api/token");
        }

        public async Task<SignInModel> SignInAsync(string token)
        {
            token = Uri.EscapeDataString(token);

            var data = GrantTypePassword();
            data.AppendFormat("&token={0}", token);

            return await _SignInAsync(data, "api/token");
        }
    }

    //+ grant_type(string, required) - Should be 'password';
    //+ username(string, required) - Email;
    //+ password(string, required, minLength = 6, maxLength = 100) - Password;
    //+ client_id(string, required) - Should be `sodapdfdesktopapp`;
    //+ client_secret(string, required) - Should be `mfjmibwJ2ILGFPVV/S95K4hHzTD4ZBxV/bDS/TUKtw=`.    

    public enum SignInStatus : byte
    {
        Ok,
        InvalidGrant
    }

    public class SignInModel
    {
        public readonly SignInStatus Status = SignInStatus.Ok;
        public readonly SaaSApiToken Token = null;

        public SignInModel(SaaSApiToken token)
        {
            Token = token;
        }

        public SignInModel(SignInStatus status)
        {
            Status = status;
        }

        public SignInModel(SignInStatus status, SaaSApiToken token)
        {
            Status = status;
            Token = token;
        }
    }
}
