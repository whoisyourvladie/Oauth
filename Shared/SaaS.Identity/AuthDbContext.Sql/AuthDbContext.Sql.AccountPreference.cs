using SaaS.Data.Entities.Accounts;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public partial class AuthDbContext
    {
        internal async Task<AccountPreference> AccountPreferenceGetAsync(Guid accountId)
        {
            return await ExecuteReaderAsync<AccountPreference>("[accounts].[pAccountPreferenceGetByAccountId]", accountId);
        }
        internal async Task AccountPreferenceSetAsync(Guid accountId, string json)
        {
            var sqlParams = new SqlParameter[]
            {
                accountId.ToSql("accountId"),
                json.ToSql("json")
            };

            await ExecuteNonQueryAsync("[accounts].[pAccountPreferenceSetByAccountId]", sqlParams);
        }
        internal async Task AccountPreferenceDeleteAsync(Guid accountId)
        {
            await ExecuteNonQueryAsync("[accounts].[pAccountPreferenceDeleteByAccountId]", CreateSqlParams(accountId));
        }
    }
}