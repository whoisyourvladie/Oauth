using SaaS.Data.Entities.Oauth;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    //TODO remove sp. [oauth].[pSessionTokenGetByParentId]

    public partial class AuthDbContext
    {
        internal async Task<SessionToken> SessionTokenGetAsync(Guid id)
        {
            var sqlParams = new SqlParameter[] { id.ToSql("id") };

            return await ExecuteReaderAsync<SessionToken>("[oauth].[pSessionTokenGetById]", sqlParams);
        }
        internal async Task<List<SessionToken>> SessionTokensGetAsync(Guid accountId)
        {
            return await ExecuteReaderCollectionAsync<SessionToken>("[oauth].[pSessionTokenGetByAccountId]", CreateSqlParams(accountId));
        }
        internal async Task<int> SessionTokenDeleteAsync(Guid id)
        {
            var sqlParams = new SqlParameter[] { id.ToSql("id") };

            return await ExecuteNonQueryAsync("[oauth].[pSessionTokenDelete]", sqlParams);
        }
        internal async Task SessionTokenInsertAsync(SessionToken token, Guid? oldId, bool isRemoveOldSessions, bool isInsertAccountSystem)
        {
            var sqlParams = new SqlParameter[]
            {
                token.Id.ToSql("id"),
                token.ParentId.ToSql("parentId"),
                token.ClientId.ToSql("clientId"),
                token.ClientVersion.ToSql("clientVersion"),
                token.ExternalClient.ToSql("externalClientId"),
                token.IssuedUtc.ToSql("issuedUtc"),
                token.ExpiresUtc.ToSql("expiresUtc"),
                token.Scope.ToSql("scope"),
                token.ProtectedTicket.ToSql("protectedTicket"),
                token.AccountId.ToSql("accountId"),
                token.SystemId.ToSql("systemId"),
                token.AccountProductId.ToSql("accountProductId"),
//Warning! All transmitted parameters must be implemented on the DB side in this procedure of appropriate product.
#if LuluSoft
                token.InstallationID.ToSql("installationID"),
#endif

                oldId.ToSql("oldId"),
                isRemoveOldSessions.ToSql("isRemoveOldSessions"),
                isInsertAccountSystem.ToSql("isInsertAccountSystem")
            };
            await ExecuteNonQueryAsync("[oauth].[pSessionTokenInsert]", sqlParams);
        }
    }
}