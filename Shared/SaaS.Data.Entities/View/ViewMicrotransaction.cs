using System;

namespace SaaS.Data.Entities.View
{
    public abstract class ViewMicrotransaction
    {
        public Guid AccountMicrotransactionId { get; set; }
        public string ProductName { get; set; }
        public string ProductUnitName { get; set; }
        public string Plan { get; set; }
        public DateTime PurchaseDate { get; set; }

        public int? AllowedEsignCount { get; set; }
        public int UsedEsignCount { get; set; }
    }
}