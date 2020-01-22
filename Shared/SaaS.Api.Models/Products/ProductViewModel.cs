using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SaaS.Api.Models.Products
{
    public class ProductViewModel
    {
        [JsonProperty("id")]
        public Guid AccountProductId { get; set; }

        public string SpId { get; set; }

        [JsonProperty("name")]
        public string ProductName { get; set; }

        [JsonProperty("unitName")]
        public string ProductUnitName { get; set; }
        public string Plan { get; set; }

        public DateTime PurchaseDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? NextRebillDate { get; set; }
        public DateTime? CreditCardExpiryDate { get; set; }

        public ulong Status { get; set; }

        public List<AccountProductModuleModel> Modules { get; set; }

        public ushort? Order { get; set; }
    }
}
