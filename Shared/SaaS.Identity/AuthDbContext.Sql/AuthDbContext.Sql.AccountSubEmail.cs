using SaaS.Data.Entities.Accounts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public partial class AuthDbContext
    {
        internal async Task AccountEmailSetAsync(AccountSubEmailPending pending)
        {
            var sqlParams = new SqlParameter[] { pending.Id.ToSql("accountSubEmailPendingId") };

            await ExecuteScalarAsync("[accounts].[pAccountSetEmail]", sqlParams);
        }

        internal async Task<List<AccountSubEmail>> AccountSubEmailsGetAsync(Guid accountId)
        {
            return await ExecuteReaderCollectionAsync<AccountSubEmail>("[accounts].[pAccountSubEmailGetByAccountId]", CreateSqlParams(accountId));
        }
        internal async Task AccountSubEmailDeleteAsync(int id)
        {
            var sqlParams = new SqlParameter[] { id.ToSql("id") };

            await ExecuteNonQueryAsync("[accounts].[pAccountSubEmailDeleteById]", sqlParams);
        }
        

        internal async Task<AccountSubEmailPending> AccountSubEmailPendingSetAsync(Guid accountId, string email)
        {
            var sqlParams = new SqlParameter[]
            {
                accountId.ToSql("accountId"),
                email.ToSql("email")
            };

            return await ExecuteReaderAsync<AccountSubEmailPending>("[accounts].[pAccountSubEmailPendingSet]", sqlParams);
        }
        internal async Task<AccountSubEmailPending> AccountSubEmailPendingGetAsync(Guid id)
        {
            var sqlParams = new SqlParameter[] { id.ToSql("id") };

            return await ExecuteReaderAsync<AccountSubEmailPending>("[accounts].[pAccountSubEmailPendingGetById]", sqlParams);
        }
        internal async Task<List<AccountSubEmailPending>> AccountSubEmailPendingsGetAsync(Guid accountId)
        {
            return await ExecuteReaderCollectionAsync<AccountSubEmailPending>("[accounts].[pAccountSubEmailPendingGetByAccountId]", CreateSqlParams(accountId));
        }        

        internal async Task AccountSubEmailPendingDeleteAsync(Guid accountId)
        {
            await ExecuteScalarAsync("[accounts].[pAccountSubEmailPendingDeleteByAccountId]", CreateSqlParams(accountId));
        }
    }
}