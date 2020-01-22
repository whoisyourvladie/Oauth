using SaaS.Data.Entities;
using SaaS.Data.Entities.View;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public class AuthProductRepository : IAuthProductRepository
    {
        private readonly AuthDbContext _context;

        public AuthProductRepository()
        {
            _context = new AuthDbContext();
        }
        public async Task<AssignStatus> ProductAssignAsync(AccountProductPair pair, bool isIgnoreBillingCycle = false)
        {
            return await _context.ProductAssignAsync(pair, isIgnoreBillingCycle);
        }
        public async Task<ViewUnassignProduct> ProductUnassignAsync(AccountProductPair pair)
        {
            return await _context.ProductUnassignAsync(pair);
        }
        public async Task<AllowedCountStatus> ProductAllowedSetAsync(AccountProductPair pair, int allowedCount)
        {
            return await _context.ProductAllowedSetAsync(pair, allowedCount);
        }
        public async Task ProductDeactivateAsync(AccountProductPair pair)
        {
            await _context.ProductDeactivateAsync(pair);
        }
        public async Task ProductNextRebillDateSetAsync(AccountProductPair pair, DateTime? nextRebillDate)
        {
            await _context.ProductNextRebillDateSetAsync(pair, nextRebillDate);
        }
        public async Task ProductEndDateSetAsync(AccountProductPair pair, DateTime endDate)
        {
            await _context.ProductEndDateSetAsync(pair, endDate);
        }
        
        public async Task ProductIsNewAsync(AccountProductPair pair, bool isNew)
        {
            await _context.ProductIsNewAsync(pair, isNew);
        }

        public async Task<ViewUpgradeProduct> UpgradeProductGetAsync(Guid accountProductId)
        {
            return await _context.UpgradeProductGetAsync(accountProductId);
        }

        public async Task<List<ViewAccountProduct>> AccountProductsGetAsync(Guid accountId, Guid? systemId = null)
        {
            return await _context.AccountProductsGetAsync(accountId, systemId);
        }

        public async Task<List<ViewAccountMicrotransaction>> AccountMicrotransactionsGetAsync(Guid accountId)
        {
            return await _context.AccountMicrotransactionsGetAsync(accountId);
        }

        public async Task<List<ViewUpclickProduct>> UpclickProductsGetAsync()
        {
            return await _context.UpclickProductsGetAsync();
        }

        public async Task<ViewOwnerProduct> OwnerProductGetAsync(AccountProductPair pair)
        {
            return await _context.OwnerProductGetAsync(pair);
        }
        public async Task<ViewOwnerProduct> OwnerProductDetailsGetAsync(AccountProductPair pair)
        {
            return await _context.OwnerProductDetailsGetAsync(pair);
        }
        public async Task<List<ViewOwnerProduct>> OwnerProductsGetAsync(Guid accountId)
        {
            return await _context.OwnerProductsGetAsync(accountId);
        }
        public async Task OwnerProductInsertAsync(Guid accountId, string productUid, string currency, decimal price, decimal priceUsd, int quantity)
        {
            await _context.OwnerProductInsertAsync(accountId, productUid, currency, price, priceUsd, quantity);
        }


        public void Dispose()
        {
            if (!object.Equals(_context, null))
                _context.Dispose();
        }
    }
}