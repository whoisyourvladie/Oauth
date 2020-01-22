using SaaS.Data.Entities.eSign;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public class eSignRepository : IeSignRepository
    {
        private readonly AuthDbContext _context;

        public eSignRepository()
        {
            _context = new AuthDbContext();
        }
        public Task eSignUseIncreaseAsync(Guid accountId, Guid? accountProductId, Guid? accountMicrotransactionId)
        {
            return _context.eSignUseIncreaseAsync(accountId, accountProductId, accountMicrotransactionId);
        }

        public Task<int> eSignPackageHistoryGetFreeNumberOfSigns(int oauthClientId, eSignClient eSignClient, string ipAddressHash)
        {
            return _context.eSignPackageHistoryGetFreeNumberOfSigns(oauthClientId, eSignClient, ipAddressHash);
        }
        public Task eSignPackageHistorySetAsync(eSignPackageHistory history)
        {
            return _context.eSignPackageHistorySetAsync(history);
        }

        public Task<eSignApiKey> eSignApiKeyGetAsync(Guid accountId, eSignClient eSignClient)
        {
            return _context.eSignApiKeyGetAsync(accountId, eSignClient);
        }
        public List<eSignApiKey> eSignApiKeysNeedRefreshGet(int top = 10)
        {
            return _context.eSignApiKeysNeedRefreshGet(top);
        }

        public Task eSignApiKeySetAsync(Guid accountId, string email, eSignClient eSignClient, string apiKey)
        {
            return _context.eSignApiKeySetAsync(accountId, email, eSignClient, apiKey);
        }

        public void eSignApiKeyDelete(int id)
        {
            _context.eSignApiKeyDelete(id);
        }

        public void Dispose()
        {
            if (!object.Equals(_context, null))
                _context.Dispose();
        }

        
    }
}