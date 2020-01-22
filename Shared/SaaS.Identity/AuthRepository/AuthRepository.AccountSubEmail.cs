using SaaS.Data.Entities.Accounts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace SaaS.Identity
{
    public partial class AuthRepository
    {
        public Uri GenerateEmailChangeConfirmationTokenLinkAsync(Uri uri, AccountSubEmailPending pending)
        {
            var uriBuilder = new UriBuilder(uri);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query.Add("token", pending.Id.ToString("N"));

            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri;
        }
        public async Task AccountEmailSetAsync(AccountSubEmailPending pending)
        {
            await _context.AccountEmailSetAsync(pending);
        }

        public async Task<List<AccountSubEmail>> AccountSubEmailsGetAsync(Guid accountId)
        {
            return await _context.AccountSubEmailsGetAsync(accountId);
        }

        public async Task AccountSubEmailDeleteAsync(int id)
        {
            await _context.AccountSubEmailDeleteAsync(id);
        }
        

        public async Task<AccountSubEmailPending> AccountSubEmailPendingSetAsync(Guid accountId, string email)
        {
            return await _context.AccountSubEmailPendingSetAsync(accountId, email);
        }
        public async Task<AccountSubEmailPending> AccountSubEmailPendingGetAsync(Guid id)
        {
            return await _context.AccountSubEmailPendingGetAsync(id);
        }
        public async Task<List<AccountSubEmailPending>> AccountSubEmailPendingsGetAsync(Guid accountId)
        {
            return await _context.AccountSubEmailPendingsGetAsync(accountId);
        }
        public async Task AccountSubEmailPendingDeleteAsync(Guid accountId)
        {
            await _context.AccountSubEmailPendingDeleteAsync(accountId);
        }
    }
}