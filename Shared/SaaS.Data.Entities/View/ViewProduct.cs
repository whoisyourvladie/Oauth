using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaaS.Data.Entities.View
{
    public abstract class ViewProduct
    {
        public Guid AccountProductId { get; set; }
        public string SpId { get; set; }
        public string ProductName { get; set; }
        public string ProductUnitName { get; set; }
        public string Plan { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? NextRebillDate { get; set; }
        public DateTime? CreditCardExpiryDate { get; set; }

        public bool IsNew { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsTrial { get; set; }
        public bool IsFree { get; set; }
        public bool IsActive { get; set; }
        public bool IsMinor { get; set; }
        public bool IsPPC { get; set; }
        public bool IsUpgradable { get; set; }
        public bool IsPaymentFailed { get; set; }
        public bool IsRenewal { get; set; }
        public bool IsOwner { get; set; }

        public int? AllowedEsignCount { get; set; }
        public int UsedEsignCount { get; set; }

        public int? AllowedPcCount { get; set; }


        [NotMapped]
        public bool IsExpired
        {
            get
            {
                if (EndDate.HasValue)
                    return DateTime.UtcNow >= EndDate.Value;

                return false;
            }
        }

        [NotMapped]
        public ulong Status
        {
            get
            {
                ulong status = 0;

                status |= (ulong)(IsDisabled ? ProductStatus.IsDisabled : 0);
                status |= (ulong)(IsExpired ? ProductStatus.IsExpired : 0);
                status |= (ulong)(IsTrial ? ProductStatus.IsTrial : 0);
                status |= (ulong)(IsFree ? ProductStatus.IsFree : 0);
                status |= (ulong)(IsMinor ? ProductStatus.IsMinor : 0);
                status |= (ulong)(IsActive ? ProductStatus.IsActive : 0);
                status |= (ulong)(IsPPC ? ProductStatus.IsPPC : 0);
                status |= (ulong)(IsUpgradable ? ProductStatus.IsUpgradable : 0);
                status |= (ulong)(IsNew ? ProductStatus.IsNew : 0);

                status |= (ulong)(IsPaymentFailed ? ProductStatus.IsPaymentFailed : 0);
                status |= (ulong)(IsRenewal ? ProductStatus.IsRenewal : 0);
                status |= (ulong)(IsOwner ? ProductStatus.IsOwner : 0);

                status |= (ulong)(IsNotAbleToRenewCreditCartExpired ? ProductStatus.IsNotAbleToRenewCreditCartExpired : 0);
                status |= (ulong)(IsRefund ? ProductStatus.IsRefund : 0);
                status |= (ulong)(IsChargeback ? ProductStatus.IsChargeback : 0);
                status |= (ulong)(IsUpgraded ? ProductStatus.IsUpgraded : 0);
                status |= (ulong)(IsManualCancelled ? ProductStatus.IsManualCancelled : 0);
                status |= (ulong)(IsNotSupportedClient ? ProductStatus.IsNotSupportedClient : 0);
                status |= (ulong)(IsBusiness ? ProductStatus.IsBusiness : 0);

                return status;
            }
        }


        public bool IsNotAbleToRenewCreditCartExpired { get; set; }
        public bool IsRefund { get; set; }
        public bool IsChargeback { get; set; }
        public bool IsUpgraded { get; set; }
        public bool IsManualCancelled { get; set; }
        public bool IsNotSupportedClient { get; set; }
        public bool IsBusiness { get; set; }
    }
}