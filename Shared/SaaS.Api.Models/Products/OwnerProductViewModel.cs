using Newtonsoft.Json;
using SaaS.Data.Entities.View;
using System.Collections.Generic;

namespace SaaS.Api.Models.Products
{
    public class OwnerProductViewModel : ProductViewModel
    {
        public string OwnerEmail { get; set; }

        [JsonProperty("Allowed")]
        public int AllowedCount { get; set; }

        [JsonProperty("Used")]
        public int UsedCount { get; set; }

        public List<ViewOwnerAccount> Accounts { get; set; }
    }
}
