using Newtonsoft.Json;
using System;

namespace SaaS.Api.Models.Products
{
    public class AddOwnerProductViewModel
    {
        public Guid UserId { get; set; }
        public string ProductUid { get; set; }
        [JsonProperty("Currency")]
        public string Currency { get; set; }
        public decimal Price { get; set; }
        public decimal PriceUsd { get; set; }
        public int Quantity { get; set; }
    }
}
