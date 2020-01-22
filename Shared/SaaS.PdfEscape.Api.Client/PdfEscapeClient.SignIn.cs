using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace SaaS.PdfEscape.Api.Client
{
    public partial class PdfEscapeClient
    {
        public async Task<SignInModel> SignInAsync(string token)
        {
            using (var client = CreateHttpClient())
            {
                var response = await client.GetAsync(string.Format("_desktop/verify/?token={0}", token));

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var singInToken = JsonConvert.DeserializeObject<PdfEscapeApiToken>(json);
                    return new SignInModel(singInToken);
                }
                return new SignInModel(SignInStatus.InvalidGrant);
            }
        }
    }

    public enum SignInStatus : byte
    {
        Ok,
        InvalidGrant
    }

    public class SignInModel
    {
        public readonly SignInStatus Status = SignInStatus.Ok;
        public readonly PdfEscapeApiToken Token = null;

        public SignInModel(PdfEscapeApiToken token)
        {
            Token = token;
        }

        public SignInModel(SignInStatus status)
        {
            Status = status;
        }

        public SignInModel(SignInStatus status, PdfEscapeApiToken token)
        {
            Status = status;
            Token = token;
        }
    }
}
