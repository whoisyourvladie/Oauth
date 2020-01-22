using SaaS.Data.Entities.eSign;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public interface IeSignRepository : IDisposable
    {
        Task eSignUseIncreaseAsync(Guid accountId, Guid? accountProductId = null, Guid? accountMicrotransactionId = null);
        Task<int> eSignPackageHistoryGetFreeNumberOfSigns(int oauthClientId, eSignClient eSignClient, string ipAddressHash);
        Task eSignPackageHistorySetAsync(eSignPackageHistory history);

        Task<eSignApiKey> eSignApiKeyGetAsync(Guid accountId, eSignClient eSignClient);
        List<eSignApiKey> eSignApiKeysNeedRefreshGet(int top = 10);

        Task eSignApiKeySetAsync(Guid accountId, string email, eSignClient eSignClient, string apiKey);
        void eSignApiKeyDelete(int id);
    }
}
