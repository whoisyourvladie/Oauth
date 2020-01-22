using SaaS.Data.Entities.View;
using System;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public partial class AuthRepository
    {
        public async Task<ViewAccountDetails> AccountDetailsGetAsync(Guid accountId)
        {
            return await _context.AccountDetailsGetAsync(accountId);
        }
        public async Task AccountDetailsSetAsync(ViewAccountDetails accountDetails)
        {
            await _context.AccountDetailsSetAsync(accountDetails);
        }
        public async Task<int?> AccountUidGetAsync(Guid accountId)
        {
            return await _context.AccountUidGetAsync(accountId);
        }
    }
}