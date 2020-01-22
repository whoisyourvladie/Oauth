using Newtonsoft.Json;

namespace SaaS.Api.Client
{
    public class SaaSApiToken
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ulong Status { get; set; }
    }
}
