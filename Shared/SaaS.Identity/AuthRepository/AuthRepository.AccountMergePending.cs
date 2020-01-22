using SaaS.Data.Entities.View.Accounts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace SaaS.Identity
{
    public partial class AuthRepository
    {
        public Uri GenerateMergeConfirmationTokenLinkAsync(Uri uri, ViewAccountMergePending pending)
        {
            var uriBuilder = new UriBuilder(uri);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query.Add("token", pending.Id.Value.ToString("N"));

            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri;
        }
        public async Task AccountMergeAsync(ViewAccountMergePending pending)
        {
            await _context.AccountMergeAsync(pending);
        }
        public async Task<ViewAccountMergePending> AccountMergePendingMergeAsync(Guid accountIdTo, Guid accountIdFrom, Guid accountIdPrimaryEmail)
        {
            return await _context.AccountMergePendingSetAsync(accountIdTo, accountIdFrom, accountIdPrimaryEmail);
        }
        public async Task<ViewAccountMergePending> AccountMergePendingGetAsync(Guid id)
        {
            return await _context.AccountMergePendingGetAsync(id);
        }
        public async Task<List<ViewAccountMergePending>> AccountMergePendingsGetAsync(Guid accountId)
        {
            return await _context.AccountMergePendingsGetAsync(accountId);
        }
        public async Task AccountMergePendingDeleteAsync(Guid accountId)
        {
            await _context.AccountMergePendingDeleteAsync(accountId);
        }
    }
}