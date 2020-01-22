using SaaS.Data.Entities.View.Oauth;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public partial class AuthDbContext
    {
        internal async Task<List<ViewSessionTokenExternalHistory>> SessionTokenExternalHistoriesGetAsync(Guid accountId)
        {
            return await ExecuteReaderCollectionAsync<ViewSessionTokenExternalHistory>("[oauth].[pSessionTokenExternalHistoryGetByAccountId]", accountId);
        }
        internal async Task SessionTokenExternalHistorySetAsync(Guid accountId, string externalClientName, string externalAccountId, string email)
        {
            var sqlParams = new SqlParameter[]
            {
                accountId.ToSql("accountId"),
                externalClientName.ToSql("externalClientName"),
                externalAccountId.ToSql("externalAccountId"),
                email.ToSql("email")
            };

            await ExecuteScalarAsync("[oauth].[pSessionTokenExternalHistorySet]", sqlParams);
        }
        internal async Task SessionTokenExternalHistoryConnectAccountAsync(Guid accountId, string externalClientName, string externalAccountId, string email)
        {
            var sqlParams = new SqlParameter[]
            {
                accountId.ToSql("accountId"),
                externalClientName.ToSql("externalClientName"),
                externalAccountId.ToSql("externalAccountId"),
                email.ToSql("email")
            };

            await ExecuteScalarAsync("[oauth].[pSessionTokenExternalHistoryConnectAccount]", sqlParams);
        }
        
        internal async Task SessionTokenExternalHistorySetStateAsync(Guid id, bool isUnlinked)
        {
            var sqlParams = new SqlParameter[]
            {
                id.ToSql("id"),
                isUnlinked.ToSql("isUnlinked")
            };

            await ExecuteScalarAsync("[oauth].[pSessionTokenExternalHistorySetState]", sqlParams);
        }
    }
}