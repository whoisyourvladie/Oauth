using Newtonsoft.Json;

namespace SaaS.Api.Models.Api
{
    public class UpgradeProductViewModel
    {
        public string OwnerEmail { get; set; }

        [JsonProperty("UnitName")]
        public string ProductUnitName { get; set; }

        [JsonProperty("Currency")]
        public string CurrencyISO { get; set; }
        public decimal Price { get; set; }
        public decimal PriceUsd { get; set; }

        public string SpId { get; set; }
        public int Quantity { get; set; }
        public int TimeStamp { get; set; }
        public ulong Status { get; set; }
    }
}