using System.ComponentModel.DataAnnotations.Schema;

namespace SaaS.Data.Entities.View
{
    public class ViewUpgradeProduct
    {
        public string OwnerEmail { get; set; }
        public string ProductUnitName { get; set; }
        public string CurrencyISO { get; set; }
        public decimal Price { get; set; }
        public decimal PriceUsd { get; set; }
        public string SpId { get; set; }
        public int Quantity { get; set; }
        public int TimeStamp { get; set; }
        public bool IsUpgradable { get; set; }
        public bool IsTrial { get; set; }
        public bool IsFree { get; set; }

        [NotMapped]
        public ulong Status
        {
            get
            {
                ulong status = 0;

                status |= (ulong)(IsTrial ? ProductStatus.IsTrial : 0);
                status |= (ulong)(IsFree ? ProductStatus.IsFree : 0);
                status |= (ulong)(IsUpgradable ? ProductStatus.IsUpgradable : 0);

                status |= (ulong)(false ? ProductStatus.IsPaymentFailed : 0);

                return status;
            }
        }
    }
}
