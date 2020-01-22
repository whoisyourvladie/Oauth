using SaaS.Data.Entities.Accounts;
using System;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public partial class AuthRepository
    {
        public async Task<AccountPreference> AccountPreferenceGetAsync(Guid accountId)
        {
            return await _context.AccountPreferenceGetAsync(accountId);
        }
        public async Task AccountPreferenceSetAsync(Guid accountId, string json)
        {
            await _context.AccountPreferenceSetAsync(accountId, json);
        }
        public async Task AccountPreferenceDeleteAsync(Guid accountId)
        {
            await _context.AccountPreferenceDeleteAsync(accountId);
        }
    }
}