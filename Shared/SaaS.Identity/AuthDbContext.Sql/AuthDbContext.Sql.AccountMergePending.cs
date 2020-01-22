using SaaS.Data.Entities.View.Accounts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public partial class AuthDbContext
    {
        internal async Task AccountMergeAsync(ViewAccountMergePending pending)
        {
            var sqlParams = new SqlParameter[]
            {
                pending.AccountIdTo.ToSql("accountIdTo"),
                pending.AccountIdFrom.ToSql("accountIdFrom"),
                pending.AccountIdPrimaryEmail.ToSql("accountIdOfEmailSetToPrimary")
            };

            await ExecuteScalarAsync("[accounts].[pAccountMerge]", sqlParams);
        }


        internal async Task<ViewAccountMergePending> AccountMergePendingSetAsync(Guid accountIdTo, Guid accountIdFrom, Guid accountIdPrimaryEmail)
        {
            var sqlParams = new SqlParameter[]
            {
                accountIdTo.ToSql("accountIdTo"),
                accountIdFrom.ToSql("accountIdFrom"),
                accountIdPrimaryEmail.ToSql("accountIdPrimaryEmail")
            };

            return await ExecuteReaderAsync<ViewAccountMergePending>("[accounts].[pAccountMergePendingSet]", sqlParams);
        }

        internal async Task<ViewAccountMergePending> AccountMergePendingGetAsync(Guid id)
        {
            var sqlParams = new SqlParameter[] { id.ToSql("id") };

            return await ExecuteReaderAsync<ViewAccountMergePending>("[accounts].[pAccountMergePendingGetById]", sqlParams);
        }
        internal async Task<List<ViewAccountMergePending>> AccountMergePendingsGetAsync(Guid accountId)
        {
            return await ExecuteReaderCollectionAsync<ViewAccountMergePending>("[accounts].[pAccountMergePendingGetByAccountId]", CreateSqlParams(accountId));
        }


        internal async Task AccountMergePendingDeleteAsync(Guid accountId)
        {
            await ExecuteScalarAsync("[accounts].[pAccountMergePendingDeleteByAccountId]", CreateSqlParams(accountId));
        }
    }
}