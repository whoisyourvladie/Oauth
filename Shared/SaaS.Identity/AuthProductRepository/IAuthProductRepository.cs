using SaaS.Data.Entities;
using SaaS.Data.Entities.View;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public interface IAuthProductRepository : IDisposable
    {
        Task<AssignStatus> ProductAssignAsync(AccountProductPair pair, bool isIgnoreBillingCycle = false);
        Task<ViewUnassignProduct> ProductUnassignAsync(AccountProductPair pair);
        Task<AllowedCountStatus> ProductAllowedSetAsync(AccountProductPair pair, int allowedCount);
        Task ProductDeactivateAsync(AccountProductPair pair);
        Task ProductNextRebillDateSetAsync(AccountProductPair pair, DateTime? nextRebillDate);
        Task ProductEndDateSetAsync(AccountProductPair pair, DateTime endDate);
        Task ProductIsNewAsync(AccountProductPair pair, bool isNew);

        Task<ViewUpgradeProduct> UpgradeProductGetAsync(Guid accountProductId);

        Task<List<ViewAccountProduct>> AccountProductsGetAsync(Guid accountId, Guid? systemId = null);
        Task<List<ViewAccountMicrotransaction>> AccountMicrotransactionsGetAsync(Guid accountId);

        Task<List<ViewUpclickProduct>> UpclickProductsGetAsync();

        Task<ViewOwnerProduct> OwnerProductGetAsync(AccountProductPair pair);
        Task<ViewOwnerProduct> OwnerProductDetailsGetAsync(AccountProductPair pair);
        Task<List<ViewOwnerProduct>> OwnerProductsGetAsync(Guid accountId);
        Task OwnerProductInsertAsync(Guid accountId, string productUid, string currency, decimal price, decimal priceUsd, int quantity);
    }
}