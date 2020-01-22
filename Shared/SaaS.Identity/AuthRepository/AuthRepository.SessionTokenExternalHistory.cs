using SaaS.Data.Entities.View.Oauth;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public partial class AuthRepository
    {
        public async Task<List<ViewSessionTokenExternalHistory>> SessionTokenExternalHistoriesAsync(Guid accountId)
        {
            return await _context.SessionTokenExternalHistoriesGetAsync(accountId);
        }
        public async Task SessionTokenExternalHistorySetAsync(Guid accountId, string externalClientName, string externalAccountId, string email)
        {
            await _context.SessionTokenExternalHistorySetAsync(accountId, externalClientName, externalAccountId, email);
        }
        public async Task SessionTokenExternalHistoryConnectAccountAsync(Guid accountId, string externalClientName, string externalAccountId, string email)
        {
            await _context.SessionTokenExternalHistoryConnectAccountAsync(accountId, externalClientName, externalAccountId, email);
        }
        public async Task SessionTokenExternalHistorySetStateAsync(Guid id, bool isUnlinked)
        {
            await _context.SessionTokenExternalHistorySetStateAsync(id, isUnlinked);
        }
    }
}