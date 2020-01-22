using Newtonsoft.Json.Linq;
using SaaS.Api.Test.Models.Api.Oauth;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SaaS.Api.Test
{
    internal class ESignHelper
    {
        protected static readonly AppSettings _appSettings = new AppSettings();

        private static async Task<HttpClient> CreateHttpClient()
        {
            TokenResultModel token = await OAuthHelper.SignIn();

            var client = new HttpClient();

            client.BaseAddress = _appSettings.PathToESignApi;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", token.token_type, token.access_token));

            return client;
        }

        public static async Task<HttpResponseMessage> GetPackages()
        {
            using (var client = await CreateHttpClient())
                return await client.GetAsync("api-esign20/v1/packages?query=inbox");
        }

        public static async Task<HttpResponseMessage> PostPackages()
        {
            using (var client = await CreateHttpClient())
                return await client.PostAsync("api/packages", null);
        }

        public static async Task<HttpResponseMessage> GetPackageId(Guid packageId)
        {
            using (var client = await CreateHttpClient())
                return await client.GetAsync(string.Format("api/packages/{0}", packageId));
        }

        public static async Task<HttpResponseMessage> PostPackageId(Guid packageId, JObject content)
        {
            using (var client = await CreateHttpClient())
                return await client.PostAsJsonAsync(string.Format("api/packages/{0}", packageId), content);
        }

        public static async Task<HttpResponseMessage> DeletePackageId(Guid packageId)
        {
            using (var client = await CreateHttpClient())
                return await client.DeleteAsync(string.Format("api/packages/{0}", packageId));
        }
    }
}
