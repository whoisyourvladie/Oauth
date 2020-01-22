using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.Oauth;
using SaaS.Data.Entities.View.Accounts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public partial class AuthDbContext
    {
        internal async Task<Account> AccountGetAsync(Guid accountId)
        {
            var sqlParams = new SqlParameter[] { accountId.ToSql("id") };

            return await ExecuteReaderAsync<Account>("[accounts].[pAccountGetById]", sqlParams);
        }
        internal async Task<Account> AccountGetAsync(string email, bool isIncludeSubEmails)
        {
            var sqlParams = new SqlParameter[]
            {
                email.ToSql("email"),
                isIncludeSubEmails.ToSql("isIncludeSubEmails")
            };
            return await ExecuteReaderAsync<Account>("[accounts].[pAccountGetByEmail]", sqlParams);
        }
        internal async Task<Account> AccountGetAsync(string primaryEmail, string password)
        {
            var user = await AccountGetAsync(primaryEmail, isIncludeSubEmails: false);

            if (!object.Equals(user, null) && !PasswordIsEqual(user.Password, password))
                user = null;

            return user;
        }
        internal async Task<List<Account>> AccountsGetAsync(string filter, string globalOrderId)
        {
            var sqlParams = new SqlParameter[]
            {
                filter.ToSql("filter"),
                globalOrderId.ToSql("globalOrderId")
            };

            return await ExecuteReaderCollectionAsync<Account>("[accounts].[pAccountGetByFilter]", sqlParams);
        }
        internal async Task<Account> AccountGetByTransactionOrderUidAsync(string transactionOrderUid)
        {
            var sqlParams = new SqlParameter[]
            {
                transactionOrderUid.ToSql("transactionOrderUid")
            };

            return await ExecuteReaderAsync<Account>("[accounts].[pAccountGetByTransactionOrderUid]", sqlParams);
        }
        internal async Task AccountDeleteAsync(Guid accountId)
        {
            var status = await ExecuteReaderAsync<int>("[accounts].[pAccountDelete]", accountId);

            if (status != 0)
            {
                switch (status)
                {

                    case -50001: throw new Exception("Customer is not exists.");
                    case -50013: throw new Exception("Customer can't bee removed.");
                    default: throw new Exception();
                }
            }
        }
        internal async Task AccountActivateAsync(Guid accountId)
        {
            var sqlParams = new SqlParameter[]
            {
                accountId.ToSql("accountId"),
                0.ToSql("mode")
            };

            await ExecuteNonQueryAsync("[accounts].[pAccountActivate]", sqlParams);
        }
        internal async Task AccountVisitorIdSetAsync(Guid accountId, Guid visitorId)
        {
            var sqlParams = new SqlParameter[]
            {
                accountId.ToSql("accountId"),
                visitorId.ToSql("visitorId")
            };

            await ExecuteNonQueryAsync("[accounts].[pAccountVisitorSet]", sqlParams);
        }

        internal List<ViewAccountEmail> AccountEmailsGet(Guid accountId)
        {
            return ExecuteReaderCollection<ViewAccountEmail>("[accounts].[pAccountGetEmailsByAccountId]", CreateSqlParams(accountId));
        }

        public async Task<List<AccountSystem>> AccountSystemsGetAsync(AccountProductPair pair)
        {
            var sqlParams = new SqlParameter[]
            {
                pair.AccountId.ToSql("accountId"),
                pair.AccountProductId.ToSql("accountProductId")
            };

            return await ExecuteReaderCollectionAsync<AccountSystem>("[oauth].[pAccountSystemSelectByAccountIdProductId]", sqlParams);
        }
    }
}