using SaaS.Data.Entities.eSign;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public partial class AuthDbContext
    {
        internal async Task eSignUseIncreaseAsync(Guid accountId, Guid? accountProductId = null, Guid? accountMicrotransactionId = null)
        {
            var sqlParams = new SqlParameter[]
            {
                accountId.ToSql("accountId"),
                accountProductId.ToSql("accountProductId"),
                accountMicrotransactionId.ToSql("accountMicrotransactionId")
            };

            await ExecuteNonQueryAsync("[eSign].[peSignUseIncrease]", sqlParams);
        }

        internal async Task<int> eSignPackageHistoryGetFreeNumberOfSigns(int oauthClientId, eSignClient eSignClient, string ipAddressHash)
        {
            var sqlParams = new SqlParameter[]
            {
                oauthClientId.ToSql("oauthClientId"),
                eSignClient.ToSql("eSignClientId"),

                ipAddressHash.ToSql("ipAddressHash")
            };

            return (int)(await ExecuteScalarAsync("[eSign].[peSignPackageHistoryGetFreeNumberOfSigns]", sqlParams));
        }
        internal async Task eSignPackageHistorySetAsync(eSignPackageHistory history)
        {
            var sqlParams = new SqlParameter[]
            {
                history.AccountId.ToSql("accountId"),
                history.oAuthClientId.ToSql("oauthClientId"),
                history.eSignClientId.ToSql("eSignClientId"),
                history.PackageId.ToSql("packageId"),
                history.IsSuccess.ToSql("isSuccess"),
                history.HttpStatusCode.ToSql("httpStatusCode"),
                history.IpAddressHash.ToSql("ipAddressHash"),

                history.AccountProductId.ToSql("accountProductId"),
                history.AccountMicrotransactionId.ToSql("accountMicrotransactionId"),
            };

            await ExecuteNonQueryAsync("[eSign].[peSignPackageHistorySet]", sqlParams);
        }

        internal async Task<eSignApiKey> eSignApiKeyGetAsync(Guid accountId, eSignClient eSignClient)
        {
            var sqlParams = new SqlParameter[]
            {
                accountId.ToSql("accountId"),
                eSignClient.ToSql("eSignClientId")
            };

            return await ExecuteReaderAsync<eSignApiKey>("[eSign].[peSignApiKeyGetByAccountId]", sqlParams);
        }
        public List<eSignApiKey> eSignApiKeysNeedRefreshGet(int top = 10)
        {
            var sqlParams = new SqlParameter[] { top.ToSql("top") };

            return ExecuteReaderCollection<eSignApiKey>("[eSign].[peSignApiKeyGetNeedRefresh]", sqlParams);
        }


        internal async Task eSignApiKeySetAsync(Guid accountId, string email, eSignClient eSignClient, string apiKey)
        {
            var sqlParams = new SqlParameter[]
            {
                accountId.ToSql("accountId"),
                email.ToSql("email"),
                eSignClient.ToSql("eSignClientId"),
                apiKey.ToSql("apiKey")
            };

            await ExecuteNonQueryAsync("[eSign].[peSignApiKeySetByAccountId]", sqlParams);
        }

        internal void eSignApiKeyDelete(int id)
        {
            var sqlParams = new SqlParameter[] { id.ToSql("id") };

            ExecuteNonQuery("[eSign].[peSignApiKeyDelete]", sqlParams);
        }
    }
}