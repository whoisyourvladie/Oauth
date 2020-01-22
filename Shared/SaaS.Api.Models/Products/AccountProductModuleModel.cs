using Newtonsoft.Json;

namespace SaaS.Api.Models.Products
{
    public class AccountProductModuleModel
    {
        public int? Allowed { get; set; }
        public int? Used { get; set; }

        [JsonProperty("name")]
        public string Module { get; set; }
    }
}
